using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using Godot;
using Godot.Bridge;
using Godot.Collections;
using Godot.NativeInterop;

[ScriptPath("res://Scripts/AttachmentScripts/AttachObjWindow.cs")]
public partial class AttachObjWindow : Window
{
    

    

    

    [Export(PropertyHint.None, "")]
    public AnimatedSprite2D mainBody;

    [Export(PropertyHint.None, "")]
    public AttachmentObject attachObject;

    [Export(PropertyHint.None, "")]
    public AnimationPlayer FadeKill;

    [Export(PropertyHint.None, "")]
    private int AttachmentStackSpacing = 8;

    [ExportGroup("Debug", "")]
    [Export(PropertyHint.None, "")]
    public bool SpawnOnLoad;

    [Export(PropertyHint.None, "")]
    public DialogueDataRes DEBUG_dialogueData;

    public Window parentWindow;

    private float _bounceSpeed;

    private float _bounceAmplitude;

    private float _bounceTime;

    private int _bounceBaseY;

    private bool _bounceInitialized;

    private double _maxSpan;

    private Vector2 _maxSpamLifeSpanInPassivePlay = new Vector2(5f, 15f);

    private bool _isSetup;

    private bool _forcedMovementActive;

    private bool _forcedMovementWalking;

    private bool _forcedMovementFinalIdle;

    private float _forcedMoveDirection = 1f;

    private double _forcedMoveStateTimer;

    private double _forcedMovementElapsed;

    private float _forcedMovementDuration;

    private float _forcedMovementX;

    public override void _Ready()
    {
        if (SpawnOnLoad && attachObject != null)
        {
            Main.Instance.spawnedAttachments.Add(this);
            SetupAttachmentWindow(DEBUG_dialogueData);
        }
        _maxSpan = GD.RandRange(_maxSpamLifeSpanInPassivePlay.X, _maxSpamLifeSpanInPassivePlay.Y);
    }

    private float _savedBytePosX;  // Save Byte's position before OVERRIDE animation

    public override void _Process(double delta)
    {
        if (parentWindow == null || !GodotObject.IsInstanceValid(parentWindow))
        {
            SignalEventBus.Instance.EmitSignal(SignalEventBus.SignalName.AttachmentEnd, Variant.From(in attachObject.attachedItemInformation.attachmentTyping));
            Main.Instance.spawnedAttachments.Remove(this);
            QueueFree();
            return;
        }
        switch (attachObject.attachedItemInformation.attachmentTyping)
        {
        case AttachDataRes.AttachmentType.OVERRIDE:
            if (_forcedMovementActive)
            {
                HandleForcedActorMovement(delta);
            }
            LayerOntoParent();
            break;
        case AttachDataRes.AttachmentType.RANDOM_CLICKED_WINDOW:
            BounceWindow(delta);
            if (Input.IsActionJustPressed("Pet") && !FadeKill.IsPlaying())
            {
                RandomObjectClicked();
            }
            if (Input.IsActionJustPressed("Move") && !FadeKill.IsPlaying())
            {
                // Don't open URLs on mobile — just dismiss the popup
                if (!Main._isMobile && attachObject.attachedItemInformation.popupURL != "")
                {
                    OS.ShellOpen(attachObject.attachedItemInformation.popupURL);
                }
                RandomObjectClicked();
            }
            if (Main.Instance.settingPassivePlayMode)
            {
                _maxSpan -= delta;
                if (_maxSpan <= 0.0)
                {
                    FadeKill.Play("FadeOutKill");
                }
            }
            break;
        }
    }

    // Public method to force-dismiss this popup (for mobile tap-to-close)
    // Bypasses the desktop-only mouse position check in RandomObjectClicked
    public void ForceDismiss()
    {
        if (FadeKill != null && !FadeKill.IsPlaying())
        {
            FadeKill.Play("FadeOutKill");
            // The animation's method track should call QueueFree at the end,
            // but schedule it as safety net in case it doesn't
            GetTree().CreateTimer(FadeKill.GetAnimation("FadeOutKill").Length).Timeout += () =>
            {
                if (GodotObject.IsInstanceValid(this))
                    QueueFree();
            };
        }
        else
        {
            // No animation player or it's already playing — just kill it
            QueueFree();
        }
    }

    public void SetupAttachmentWindow(DialogueDataRes passedText = null)
    {
        if (attachObject.attachedItemInformation == null)
        {
            GD.PrintErr("No Attached Information actor! Killing Object");
            Main.Instance.spawnedAttachments.Remove(this);
            QueueFree();
            return;
        }
        if (parentWindow == null)
        {
            parentWindow = Main.Instance.mainWindow;
        }
        switch (attachObject.attachedItemInformation.attachmentTyping)
        {
        case AttachDataRes.AttachmentType.IMAGE:
            attachObject.SetupImageAttachment();
            break;
        case AttachDataRes.AttachmentType.TEXT:
            attachObject.SetupTextAttachment(passedText);
            if (Main._isMobile)
            {
                base.MousePassthrough = true;
            }
            break;
        case AttachDataRes.AttachmentType.OVERRIDE:
            attachObject.SetupImageAttachment();
            // Save Byte's position before hiding her for the animation
            _savedBytePosX = Main.Instance.Position.X;
            if (parentWindow == Main.Instance.mainWindow)
            {
                Main.Instance.AbortActivePickup();
                Main.Instance.ClearItemTarget(usingItem: true);
                Main.Instance.mainCharacter.ForceMainBodyState(Character.MainBodyStates.Idle, "Idle");
                Main.Instance.mainCharacter.mainBodyTimer.WaitTime = 1.0;
                Main.Instance.mainCharacter.mainBodyTimer.Start();
                Main.Instance.mainCharacter.Visible = false;
            }
            else
            {
                parentWindow.Visible = false;
                if (parentWindow is ActorWindow actorWindow)
                {
                    actorWindow.inUseByAttachment = true;
                }
            }
            break;
        case AttachDataRes.AttachmentType.RANDOM_CLICKED_WINDOW:
            attachObject.SetupImageAttachment();
            // On mobile, let touches pass through to Main._Input which will
            // detect taps on this window and press Pet to dismiss it.
            // On desktop, keep MousePassthrough=false so the window captures clicks.
            base.MousePassthrough = Main._isMobile;
            break;
        }
        base.MinSize = attachObject.trueSize;
        base.Size = base.MinSize;
        base.ProcessMode = ProcessModeEnum.Inherit;
        switch (attachObject.attachedItemInformation.attachmentTyping)
        {
        case AttachDataRes.AttachmentType.OVERRIDE:
            LayerOntoParent();
            if (attachObject.attachedItemInformation.movesActorAttachment)
            {
                bool num = mainBody.SpriteFrames != null && mainBody.SpriteFrames.HasAnimation("Walk");
                bool flag = mainBody.SpriteFrames != null && mainBody.SpriteFrames.HasAnimation("Idle");
                if (num && flag)
                {
                    _forcedMovementActive = true;
                    _forcedMovementWalking = false;
                    _forcedMovementFinalIdle = false;
                    _forcedMoveStateTimer = 0.0;
                    _forcedMovementElapsed = 0.0;
                    _forcedMovementDuration = attachObject.attachedItemInformation.movementDuration;
                    // On mobile, use Main.Instance.Position instead of parentWindow.Position
                    _forcedMovementX = (Main._isMobile && parentWindow == Main.Instance.mainWindow) ? Main.Instance.Position.X : parentWindow.Position.X;
                    if (parentWindow == Main.Instance.mainWindow)
                    {
                        Main.Instance.ClearItemTarget(usingItem: true);
                    }
                }
                else
                {
                    GD.PrintErr("AttachDataRes [" + attachObject.attachedItemInformation.itemID + "] has movesActorAttachment set but is missing a Walk or Idle animation. Skipping forced movement.");
                }
            }
            if (!Main.Instance.SeenObjects[SaveHandler.SeenObjectTypes.NSFW_SCENES].Contains(attachObject.attachedItemInformation.itemID))
            {
                Main.Instance.SeenObjects[SaveHandler.SeenObjectTypes.NSFW_SCENES].Add(attachObject.attachedItemInformation.itemID);
                Main.Instance.saveHandler.SaveSettings();
            }
            break;
        case AttachDataRes.AttachmentType.RANDOM_CLICKED_WINDOW:
            RandomObjectPosition();
            if (!Main.Instance.SeenObjects[SaveHandler.SeenObjectTypes.POP_UPS].Contains(attachObject.attachedItemInformation.itemID))
            {
                Main.Instance.SeenObjects[SaveHandler.SeenObjectTypes.POP_UPS].Add(attachObject.attachedItemInformation.itemID);
                Main.Instance.saveHandler.SaveSettings();
            }
            PlaceTags();
            break;
        default:
            FollowParent();
            PlaceTags();
            break;
        }
        SignalEventBus.Instance.EmitSignal(SignalEventBus.SignalName.AttachmentStart, Variant.From(in attachObject.attachedItemInformation.attachmentTyping));
        CallDeferred("DelayedSetupFlag");
    }

    private void DelayedSetupFlag()
    {
        if (Main._isMobile)
        {
            base.Transparent = true;
            base.TransparentBg = true;
        }
        base.Visible = true;
        _isSetup = true;
    }

    private void PlaceTags()
    {
        if (attachObject.attachedItemInformation.TagPair.Count() <= 0)
        {
            return;
        }
        foreach (TagDataRes item in attachObject.attachedItemInformation.TagPair)
        {
            switch (item.tagAction)
            {
            case TagDataRes.actionEnum.ADD:
                Main.Instance.mainCharacter.AddTag(item);
                break;
            case TagDataRes.actionEnum.REMOVE:
                Main.Instance.mainCharacter.RemoveTag(item);
                break;
            }
        }
    }

    public void FollowParent()
    {
        if (!_isSetup)
        {
            return;
        }
        if (parentWindow == null || !GodotObject.IsInstanceValid(parentWindow))
        {
            SignalEventBus.Instance.EmitSignal(SignalEventBus.SignalName.AttachmentEnd, Variant.From(in attachObject.attachedItemInformation.attachmentTyping));
            Main.Instance.spawnedAttachments.Remove(this);
            QueueFree();
            return;
        }
        Vector2 attachmentMargin = attachObject.attachedItemInformation.attachmentMargin;
        Vector2I size = parentWindow.Size;
        Vector2I trueSize = attachObject.trueSize;
        if (Main._isMobile && parentWindow == Main.Instance.mainWindow)
        {
            // On mobile the main window is fullscreen at (0,0); position the bubble
            // relative to Byte's actual Position, centered above her head.
            // Ignore attachmentMargin on mobile (it's designed for desktop multi-monitor
            // layout and causes offset on mobile).
            Vector2I mainPos = (Vector2I)Main.Instance.Position;
            Vector2I mainSize = Main.Instance.mainCharacter.trueSize;
            int bx = mainPos.X + mainSize.X / 2 - trueSize.X / 2;
            int by = mainPos.Y - trueSize.Y - 20;
            // Clamp to screen
            Vector2I screenSize = DisplayServer.ScreenGetSize();
            if (bx < 0) bx = 0;
            if (bx + trueSize.X > screenSize.X) bx = screenSize.X - trueSize.X;
            if (by < 0) by = 0;
            base.Position = new Vector2I(bx, by);
            base.MousePassthrough = true;
            return;
        }
        List<AttachObjWindow> list = new List<AttachObjWindow>();
        foreach (AttachObjWindow spawnedAttachment in Main.Instance.spawnedAttachments)
        {
            if (spawnedAttachment.attachObject.attachedItemInformation.attachmentTyping != AttachDataRes.AttachmentType.OVERRIDE)
            {
                list.Add(spawnedAttachment);
            }
        }
        int num = list.IndexOf(this);
        if (num < 0)
        {
            num = 0;
        }
        bool isLeft = false;
        int num2;
        if ((num2 = parentWindow.Position.X + size.X + (int)attachmentMargin.X) + trueSize.X > Main.Instance.screenDataHandler.leftmostScreenX + Main.Instance.screenDataHandler.totalScreenWidth)
        {
            num2 = parentWindow.Position.X - trueSize.X - (int)attachmentMargin.X;
            isLeft = true;
        }
        attachObject.FlipHorizontal(isLeft);
        int num3 = parentWindow.Position.Y + size.Y / 2 - trueSize.Y / 2 + (int)attachmentMargin.Y;
        Rect2I rect2I = new Rect2I(num2, num3, trueSize.X, trueSize.Y);
        int num4 = int.MaxValue;
        for (int i = 0; i < num; i++)
        {
            AttachObjWindow attachObjWindow = list[i];
            Vector2I trueSize2 = attachObjWindow.attachObject.trueSize;
            Rect2I b = new Rect2I(attachObjWindow.Position.X, attachObjWindow.Position.Y, trueSize2.X, trueSize2.Y);
            if (rect2I.Intersects(b) && attachObjWindow.Position.Y < num4)
            {
                num4 = attachObjWindow.Position.Y;
            }
        }
        int num5 = num3;
        bool flag = true;
        while (flag)
        {
            flag = false;
            Rect2I rect2I2 = new Rect2I(num2, num5, trueSize.X, trueSize.Y);
            for (int j = 0; j < num; j++)
            {
                AttachObjWindow attachObjWindow2 = list[j];
                Vector2I trueSize3 = attachObjWindow2.attachObject.trueSize;
                Rect2I b2 = new Rect2I(attachObjWindow2.Position.X, attachObjWindow2.Position.Y, trueSize3.X, trueSize3.Y);
                if (rect2I2.Intersects(b2))
                {
                    num5 = attachObjWindow2.Position.Y - trueSize.Y - AttachmentStackSpacing;
                    flag = true;
                    break;
                }
            }
        }
        if (num5 < Main.Instance.screenDataHandler.currentScreenTop)
        {
            num5 = parentWindow.Position.Y + size.Y + (int)attachmentMargin.Y;
        }
        base.Position = new Vector2I(Main.Instance.screenDataHandler.ClampAcrossAllScreensX(num2, trueSize.X), Main.Instance.screenDataHandler.ClampAcrossAllScreensY(num5));
    }

    private void LayerOntoParent()
    {
        _ = Main.Instance.screenDataHandler;
        Vector2 attachmentMargin = attachObject.attachedItemInformation.attachmentMargin;
        Vector2I position = (Main._isMobile && parentWindow == Main.Instance.mainWindow) ? (Vector2I)Main.Instance.Position : parentWindow.Position;
        // On mobile, use Byte's actual size instead of fullscreen window size
        Vector2I size = (Main._isMobile && parentWindow == Main.Instance.mainWindow) ? Main.Instance.mainCharacter.trueSize : parentWindow.Size;
        Vector2I trueSize = attachObject.trueSize;
        // V38: on mobile for OVERRIDE (sex scenes) ignore margin to avoid teleport to right
        int marginX = Main._isMobile ? 0 : (int)attachmentMargin.X;
        int marginY = Main._isMobile ? 0 : (int)attachmentMargin.Y;
        int num = position.X + size.X / 2 - trueSize.X / 2 + marginX;
        int num2 = position.Y + size.Y / 2 - trueSize.Y / 2 + marginY;
        Rect2I rect2I = DisplayServer.ScreenGetUsableRect(parentWindow.CurrentScreen);
        int max = rect2I.End.Y - trueSize.Y;
        int x = Mathf.Clamp(num, rect2I.Position.X, rect2I.End.X - trueSize.X);
        if (_forcedMovementActive)
        {
            if (!attachObject.attachedItemInformation.overrideTaskbar)
            {
                int y = Mathf.Clamp(num2, rect2I.Position.Y, max);
                base.Position = new Vector2I(num, y);
            }
            else
            {
                base.Position = new Vector2I(num, num2);
            }
        }
        else if (!attachObject.attachedItemInformation.overrideTaskbar)
        {
            int y2 = Mathf.Clamp(num2, rect2I.Position.Y, max);
            base.Position = new Vector2I(x, y2);
        }
        else
        {
            base.Position = new Vector2I(x, num2);
        }
    }

    private void HandleForcedActorMovement(double delta)
    {
        if (parentWindow == null || !GodotObject.IsInstanceValid(parentWindow))
        {
            return;
        }
        _forcedMovementElapsed += delta;
        if (!_forcedMovementFinalIdle && _forcedMovementDuration > 0f && _forcedMovementElapsed >= (double)(_forcedMovementDuration * 0.9f))
        {
            _forcedMovementFinalIdle = true;
            _forcedMovementWalking = false;
            mainBody.Play("Idle");
        }
        if (_forcedMovementDuration > 0f && _forcedMovementElapsed >= (double)_forcedMovementDuration)
        {
            StopForcedMovement();
        }
        else
        {
            if (_forcedMovementFinalIdle)
            {
                return;
            }
            _forcedMoveStateTimer -= delta;
            if (_forcedMoveStateTimer <= 0.0)
            {
                _forcedMovementWalking = !_forcedMovementWalking;
                if (_forcedMovementWalking)
                {
                    _forcedMoveStateTimer = GD.RandRange(attachObject.attachedItemInformation.ForcedWalkTimeRange.X, attachObject.attachedItemInformation.ForcedWalkTimeRange.Y);
                    _forcedMoveDirection = ((GD.RandRange(0, 1) == 0) ? (-1f) : 1f);
                    mainBody.Play("Walk");
                    attachObject.FlipHorizontal(_forcedMoveDirection < 0f);
                }
                else
                {
                    _forcedMoveStateTimer = GD.RandRange(attachObject.attachedItemInformation.ForcedIdleTimeRange.X, attachObject.attachedItemInformation.ForcedIdleTimeRange.Y);
                    mainBody.Play("Idle");
                }
            }
            if (_forcedMovementWalking)
            {
                int x = (Main._isMobile && parentWindow == Main.Instance.mainWindow) ? Main.Instance.mainCharacter.trueSize.X : parentWindow.Size.X;
                int effectiveLeftX = Main.Instance.screenDataHandler.EffectiveLeftX;
                int max = Main.Instance.screenDataHandler.EffectiveRightX - x;
                _forcedMovementX += (float)((double)attachObject.attachedItemInformation.ForcedWalkSpeed * delta * (double)_forcedMoveDirection);
                int num = Mathf.RoundToInt(_forcedMovementX);
                int num2 = Mathf.Clamp(num, effectiveLeftX, max);
                if (num2 != num)
                {
                    _forcedMoveDirection *= -1f;
                    attachObject.FlipHorizontal(_forcedMoveDirection < 0f);
                    _forcedMovementX = num2;
                }
                // V38: on mobile don't move Byte during sex scene (was causing teleport right)
                if (Main._isMobile && parentWindow == Main.Instance.mainWindow)
                {
                    // Keep Byte stationary on mobile during OVERRIDE scenes
                    // Main.Instance.Position stays at _savedBytePosX
                }
                else
                {
                    parentWindow.Position = new Vector2I(num2, parentWindow.Position.Y);
                }
            }
        }
    }

    private void StopForcedMovement()
    {
        if (!_forcedMovementActive)
        {
            return;
        }
        _forcedMovementActive = false;
        // Restore Byte's position on mobile
        if (Main._isMobile && parentWindow == Main.Instance.mainWindow)
        {
            Main.Instance.Position = new Vector2(_savedBytePosX, Main.Instance.Position.Y);
        }
        if (parentWindow != null && GodotObject.IsInstanceValid(parentWindow))
        {
            if (parentWindow == Main.Instance.mainWindow)
            {
                Main.Instance.ClearItemTarget(usingItem: true);
            }
            else if (parentWindow is ActorWindow actorWindow)
            {
                actorWindow.characterActor.ForceMainBodyState(ActorCharacter.MainBodyStates.Idle, "Idle");
            }
            RepositionInstigatingActor();
        }
    }

    private void RepositionInstigatingActor()
    {
        if (parentWindow == null || !GodotObject.IsInstanceValid(parentWindow))
        {
            return;
        }
        ScreenDataHandler screenDataHandler = Main.Instance.screenDataHandler;
        int currentScreen = parentWindow.CurrentScreen;
        Rect2I screenRect = DisplayServer.ScreenGetUsableRect(currentScreen);
        foreach (ActorWindow spawnedCompanion in Main.Instance.spawnedCompanions)
        {
            if (GodotObject.IsInstanceValid(spawnedCompanion) && spawnedCompanion.inUse && !spawnedCompanion.Visible)
            {
                Vector2I trueSize = spawnedCompanion.characterActor.trueSize;
                int num = parentWindow.Position.X + parentWindow.Size.X / 2;
                int x = screenDataHandler.ClampAcrossAllScreensX(num - trueSize.X / 2, trueSize.X);
                int y = screenRect.End.Y - trueSize.Y;
                spawnedCompanion.Position = new Vector2I(x, y);
                spawnedCompanion.SyncCachesToScreen(currentScreen, screenRect);
            }
        }
    }

    private void InitializeBounce()
    {
        _bounceSpeed = (float)GD.RandRange(1.5, 4.0);
        _bounceAmplitude = (float)GD.RandRange(4.0, 12.0);
        _bounceTime = (float)GD.RandRange(0.0, 6.2831854820251465);
        _bounceBaseY = base.Position.Y;
        _bounceInitialized = true;
    }

    private void BounceWindow(double delta)
    {
        if (!_bounceInitialized)
        {
            InitializeBounce();
        }
        _bounceTime += (float)delta * _bounceSpeed;
        int num = (int)(Mathf.Sin(_bounceTime) * _bounceAmplitude);
        base.Position = new Vector2I(base.Position.X, _bounceBaseY + num);
    }

    private void RandomObjectPosition()
    {
        Vector2I trueSize = attachObject.trueSize;
        ScreenDataHandler screenDataHandler = Main.Instance.screenDataHandler;
        int effectiveLeftX = screenDataHandler.EffectiveLeftX;
        int effectiveRightX = screenDataHandler.EffectiveRightX;
        int currentScreenTop = screenDataHandler.currentScreenTop;
        int taskbarPos = screenDataHandler.taskbarPos;
        int x = GD.RandRange(effectiveLeftX, effectiveRightX - trueSize.X);
        int y = GD.RandRange(currentScreenTop, taskbarPos - trueSize.Y);
        base.Position = new Vector2I(x, y);
    }

    private void RandomObjectClicked()
    {
        if (!Input.IsActionJustPressed("Pet") || FadeKill.IsPlaying())
        {
            return;
        }
        Vector2 vector = DisplayServer.MouseGetPosition();
        if (!new Rect2I(base.Position, base.Size).HasPoint(new Vector2I((int)vector.X, (int)vector.Y)))
        {
            return;
        }
        AttachObjWindow attachObjWindow = null;
        int num = -1;
        Array<Node> children = Main.Instance.GetChildren();
        for (int i = 0; i < children.Count; i++)
        {
            if (children[i] is AttachObjWindow attachObjWindow2 && attachObjWindow2.attachObject.attachedItemInformation.attachmentTyping == AttachDataRes.AttachmentType.RANDOM_CLICKED_WINDOW && new Rect2I(attachObjWindow2.Position, attachObjWindow2.Size).HasPoint(new Vector2I((int)vector.X, (int)vector.Y)) && i > num)
            {
                num = i;
                attachObjWindow = attachObjWindow2;
            }
        }
        if (attachObjWindow == this)
        {
            FadeKill.Play("FadeOutKill");
        }
    }

    public void PlayDialogueOnFrame()
    {
        if (attachObject.attachedItemInformation.dialogueStack.Count() != 0 && attachObject.attachedItemInformation.dialogueStack.ContainsKey(mainBody.Frame - 1))
        {
            DialogueDataRes dialogueDataRes = attachObject.attachedItemInformation.dialogueStack[mainBody.Frame - 1];
            if (!Main.Instance.IsBlacklisted(dialogueDataRes.taggedKinks) && IsSpeakingActorPresent(dialogueDataRes) && HasRequiredTags(dialogueDataRes))
            {
                Main.Instance.dialogueStack.Add(dialogueDataRes);
                Main.Instance.PopDialogueInStack(skipTimer: true);
            }
        }
        if (attachObject.attachedItemInformation.attachmentStack.Count() != 0 && attachObject.attachedItemInformation.attachmentStack.ContainsKey(mainBody.Frame - 1))
        {
            Main.Instance.CallCharacterAttachmentSpawn(attachObject.attachedItemInformation.attachmentStack[mainBody.Frame - 1]);
        }
    }

    private bool IsSpeakingActorPresent(DialogueDataRes dialogue)
    {
        if (string.IsNullOrEmpty(dialogue.speakingActorID))
        {
            return true;
        }
        CharacterInfoDataRes characterInfoDataRes = (CharacterInfoDataRes)ResourceCache.resourcesLoaded[ResourceCache.ResourceTyping.CHARACTER][dialogue.speakingActorID];
        if (characterInfoDataRes == Main.Instance.mainCharacter.characterInformation)
        {
            return true;
        }
        foreach (ActorWindow spawnedActor in Main.Instance.spawnedActors)
        {
            if (GodotObject.IsInstanceValid(spawnedActor) && spawnedActor.characterActor.characterInformation == characterInfoDataRes)
            {
                return true;
            }
        }
        return false;
    }

    private bool HasRequiredTags(DialogueDataRes dialogue)
    {
        foreach (TagDataRes requiredTag in dialogue.RequiredTags)
        {
            TagDataRes tag = Main.Instance.mainCharacter.GetTag(requiredTag.tagName);
            if (tag == null || (requiredTag.tagAmount > 0 && tag.tagAmount < requiredTag.tagAmount))
            {
                return false;
            }
        }
        return true;
    }

    public void OnAnimationTimeout()
    {
        if (_forcedMovementActive)
        {
            return;
        }
        if (attachObject.attachedItemInformation.attachmentTyping == AttachDataRes.AttachmentType.OVERRIDE)
        {
            if (attachObject.attachedItemInformation.ChainedOverride != null)
            {
                GD.Print("DEBUG - Override Chainned");
                Main.Instance.CallCharacterAttachmentSpawn(attachObject.attachedItemInformation.ChainedOverride);
            }
            else if (parentWindow == Main.Instance.mainWindow)
            {
                Main.Instance.mainCharacter.Visible = true;
                // Restore Byte's saved position on mobile
                if (Main._isMobile)
                {
                    Main.Instance.Position = new Vector2(_savedBytePosX, Main.Instance.Position.Y);
                }
            }
            else
            {
                parentWindow.Visible = true;
                if (parentWindow is ActorWindow actorWindow)
                {
                    actorWindow.inUseByAttachment = false;
                }
            }
            if (GodotObject.IsInstanceValid(this))
            {
                PlaceTags();
                SpawnItemOnClose();
                SignalEventBus.Instance.EmitSignal(SignalEventBus.SignalName.AttachmentEnd, Variant.From(in attachObject.attachedItemInformation.attachmentTyping));
                Main.Instance.spawnedAttachments.Remove(this);
                QueueFree();
            }
        }
        else if (GodotObject.IsInstanceValid(this))
        {
            FadeKill.Play("FadeOutKill");
        }
    }

    public async void OnTextFinish()
    {
        Main.Instance.PopDialogueInStack();
        await Task.Delay(TimeSpan.FromSeconds((float)GD.RandRange(attachObject.dialogueDataRes.TextHalfLife.X, attachObject.dialogueDataRes.TextHalfLife.Y)));
        if (GodotObject.IsInstanceValid(this))
        {
            FadeKill.Play("FadeOutKill");
        }
    }

    public void KillText(StringName stringName)
    {
        if (GodotObject.IsInstanceValid(this))
        {
            SpawnItemOnClose();
            SignalEventBus.Instance.EmitSignal(SignalEventBus.SignalName.AttachmentEnd, Variant.From(in attachObject.attachedItemInformation.attachmentTyping));
            Main.Instance.spawnedAttachments.Remove(this);
            QueueFree();
        }
    }

    private void SpawnItemOnClose()
    {
        if (attachObject.attachedItemInformation.possibleItems.Count() <= 0 || !((float)GD.RandRange(0, 100) <= attachObject.attachedItemInformation.chanceOfItem))
        {
            return;
        }
        WeightGroup<ItemDataRes> weightGroup = new WeightGroup<ItemDataRes>();
        foreach (ItemDataRes possibleItem in attachObject.attachedItemInformation.possibleItems)
        {
            if (!Main.Instance.IsBlacklisted(possibleItem.taggedKinks) && (!Main.Instance.settingPassivePlayMode || !possibleItem.NoPassivePickup))
            {
                if (possibleItem.itemPopupWeight != 0.0)
                {
                    weightGroup.Add(possibleItem, Mathf.Clamp(possibleItem.itemSpawnWeight, 1.0, 10000.0));
                }
                else
                {
                    weightGroup.Add(possibleItem, Mathf.Clamp(possibleItem.itemPopupWeight, 1.0, 10000.0));
                }
            }
        }
        if (weightGroup.Count() != 0)
        {
            ItemDataRes item = weightGroup.GetItem(GD.RandRange(0, 10000));
            Vector2I spawningPosition = new Vector2I(base.Position.X + base.Size.X / 2, base.Position.Y + base.Size.Y / 2);
            Main.Instance.CallItemSpawn(item, spawningPosition);
        }
    }
















}
