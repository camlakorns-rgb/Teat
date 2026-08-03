using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Godot;
using Godot.Bridge;
using Godot.Collections;
using Godot.NativeInterop;

[GlobalClass]
[ScriptPath("res://Scripts/Main.cs")]
public partial class Main : Node2D
{
    public static bool _isMobile = false;
    private static bool _isMobileChecked = false;
    public static Vector2I _lastTouchPos = new Vector2I(0,0);
    public static readonly string V13_BUILD = "V13_ITEMDRAG_AWAYFIX_BUBBLEFIX";
    public static readonly string V30_BUILD = "V30_BLACKSCREEN_ITEMSPAWN_FIX";
    public static readonly string V30_BUILD2 = "V30_MOBILE_RENDERER_VISIBLE_FALSE";
    public static readonly string V31_BUILD = "V31_ITEM_VISIBLE_FIX";
    public static readonly string V32_BUILD = "V32_SCALE_FLOAT_FIX";
    public static readonly string V33_BUILD = "V33_REVERT_SCALE_FIX_BIT";
    public static readonly string V34_BUILD = "V34_FIX_FLOAT_TINY_BIT";
    public static readonly string V35_BUILD = "V35_FLOAT_SMALL_FIX";
    public static readonly string V36_BUILD = "V36_REVERT_BYTE_FIX_BIT_FINAL";
    public static readonly string V37_BUILD = "V37_BIT_HIGHER_SMALLER";
    public static readonly string V38_BUILD = "V38_FLICKER_TELEPORT_FIX";
    public static readonly string V39_BUILD = "V39_DEEP_DIVE_BIT_TOP_SMALL";
    public static readonly string V29_BUILD = "V29_ITEM_SPAWN_RENDERER_FIX_BUILD";
    public enum MobileTouchTargetKind { None, Byte, Item, Actor }
    public int _mobileTouchIndex = -1;
    public Vector2I _mobileTouchStart;
    public double _mobileTouchStartTime;
    public bool _mobileTouchMoved;
    public MobileTouchTargetKind _mobileTouchTarget = MobileTouchTargetKind.None;
    public Vector2 _mobileDragVelocity;
    private Vector2I _prevTouchPos;
    private double _lastDragTime;
    private Vector2I _savedTouchPos;
    private int _resourceLoadRetries = 0;

    public void TryLoadResources()
    {
        if (ResourceCache.Instance != null)
        {
            bool needLoad = false;
            if (!ResourceCache.resourcesLoaded.ContainsKey(ResourceCache.ResourceTyping.ITEM) || ResourceCache.resourcesLoaded[ResourceCache.ResourceTyping.ITEM].Count == 0)
                needLoad = true;
            if (needLoad)
            {
                GD.Print("[Mobile] TryLoadResources attempt " + _resourceLoadRetries + " - calling LoadData");
                ResourceCache.Instance.CallDeferred("LoadData");
            }
            else
            {
                GD.Print("[Mobile] ResourceCache already loaded: ITEM=" + ResourceCache.resourcesLoaded[ResourceCache.ResourceTyping.ITEM].Count);
                return;
            }
        }
        else
        {
            GD.Print("[Mobile] ResourceCache.Instance null, retrying...");
        }
        _resourceLoadRetries++;
        if (_resourceLoadRetries < 20)
        {
            GetTree().CreateTimer(0.6).Timeout += () => TryLoadResources();
        }
    }
    // Item drag state
    private ItemWindow _grabbedItem;
    private Vector2I _itemGrabOffset;
    private Vector2I _itemGrabStartPos;
    private Vector2 _itemVelocity;
    private Vector2I _itemPrevPos;
    private double _itemLastDragTime;
    public Vector2I MobileMousePos()
    {
        if (_isMobile)
        {
            if (_lastTouchPos != Vector2I.Zero)
                return _lastTouchPos;
        }
        return DisplayServer.MouseGetPosition();
    }

    public bool IsPointOnAnyItem(Vector2I p)
    {
        for (int i = spawnedItems.Count - 1; i >= 0; i--)
        {
            ItemWindow w = spawnedItems[i];
            if (!GodotObject.IsInstanceValid(w) || w.CurrentlyPickedUp)
                continue;
            bool logicallyVisible = _isMobile ? w.IsActiveForMobile : w.Visible;
            if (logicallyVisible && new Rect2I(w.Position, w.Size).HasPoint(p))
                return true;
        }
        return false;
    }

    public bool IsItemLogicallyVisible(ItemWindow w)
    {
        if (!GodotObject.IsInstanceValid(w)) return false;
        if (_isMobile) return w.IsActiveForMobile;
        return w.Visible;
    }

    public bool IsActorLogicallyVisible(ActorWindow w)
    {
        if (!GodotObject.IsInstanceValid(w)) return false;
        if (_isMobile) return w.IsActiveForMobile;
        return w.Visible;
    }

    public bool IsPointOnAnyActor(Vector2I p)
    {
        for (int i = spawnedActors.Count - 1; i >= 0; i--)
        {
            ActorWindow w = spawnedActors[i];
            if (!GodotObject.IsInstanceValid(w)) continue;
            bool logicallyVisible = _isMobile ? w.IsActiveForMobile : w.Visible;
            if (logicallyVisible && new Rect2I(w.Position, w.Size).HasPoint(p))
                return true;
        }
        return false;
    }

    // Find a RANDOM_CLICKED_WINDOW popup attachment at the given point.
    // These are the "popup ads" that bounce around the screen.
    // IMPORTANT: They are spawned with unclearableAttachment=true, so they
    // are NOT in spawnedAttachments. We must search all scene children.
    private AttachObjWindow FindPopupAttachmentAtPoint(Vector2I p)
    {
        foreach (Node child in GetChildren())
        {
            if (child is AttachObjWindow w && GodotObject.IsInstanceValid(w) && w.Visible
                && w.attachObject != null
                && w.attachObject.attachedItemInformation != null
                && w.attachObject.attachedItemInformation.attachmentTyping == AttachDataRes.AttachmentType.RANDOM_CLICKED_WINDOW)
            {
                Rect2I rect = new Rect2I(w.Position, w.Size);
                if (rect.HasPoint(p))
                    return w;
            }
        }
        return null;
    }

    // Fake a touch at Byte's center so hit-tested actions (Screen_Lock, Despawn)
    // work when triggered from the on-screen button bar.
    public void MobileActionOnByte(string action, bool pressed)
    {
        if (pressed)
        {
            _savedTouchPos = _lastTouchPos;
            _lastTouchPos = (Vector2I)(Position + (Vector2)mainCharacter.trueSize * 0.5f);
            Input.ActionPress(action);
        }
        else
        {
            Input.ActionRelease(action);
            _lastTouchPos = _savedTouchPos;
        }
    }

    public void CloseMagnifier()
    {
        _magnifierActive = false;
        if (Magnifier != null && GodotObject.IsInstanceValid(Magnifier))
        {
            Magnifier.QueueFree();
        }
        Magnifier = null;
    }

    public void ToggleDespawnMobile()
    {
        if (!_isMobile)
        {
            return;
        }
        // IMPORTANT: ClearAllAttachments() sets mainCharacter.Visible = true internally,
        // so we must snapshot visibility BEFORE calling it — otherwise the else branch
        // (restore) is unreachable (HANDOFF7 §5 / HANDOFF8 fix).
        bool wasVisible = mainCharacter.Visible;
        ClearAllAttachments();
        dialogueStack.Clear();
        dialogueStack.Add(mainCharacter.characterInformation.responseTexts[CharacterInfoDataRes.ResponseToSituation.IN_CONVO]);
        PopDialogueInStack(skipTimer: true);
        if (wasVisible)
        {
            mainCharacter.Visible = false;
        }
        else
        {
            mainCharacter.Visible = true;
            Vector2I screenSize = DisplayServer.ScreenGetSize();
            Position = new Vector2(screenSize.X / 2 - mainCharacter.trueSize.X / 2, screenSize.Y - mainCharacter.trueSize.Y);
            mainCharacter.ForceMainBodyState(Character.MainBodyStates.Forced_Animation, "Pet", 0.5f);
        }
        saveHandler.SaveSettings();
    }

    public override void _Input(InputEvent @event)
    {
        if (_isMobile)
        {
            if (@event is InputEventScreenTouch touch)
            {
                Vector2I pos = (Vector2I)touch.Position;
                _lastTouchPos = pos;
                if (touch.Pressed)
                {
                    if (MobileUI.IsPointInUI(pos))
                    {
                        return;
                    }
                    // Check for popup attachments first ("ads" that bounce around)
                    AttachObjWindow popupAtPoint = FindPopupAttachmentAtPoint(pos);
                    if (popupAtPoint != null)
                    {
                        _mobileTouchTarget = MobileTouchTargetKind.None;
                        popupAtPoint.ForceDismiss();
                        return;
                    }
                    _mobileTouchIndex = touch.Index;
                    _mobileTouchStart = pos;
                    _mobileTouchStartTime = Time.GetTicksMsec() / 1000.0;
                    _mobileTouchMoved = false;
                    _prevTouchPos = pos;
                    _mobileDragVelocity = Vector2.Zero;
                    _lastDragTime = _mobileTouchStartTime;
                    if (IsPointOnAnyItem(pos))
                    {
                        _mobileTouchTarget = MobileTouchTargetKind.Item;
                        // Find which item was touched and grab it
                        _grabbedItem = FindItemAtPoint(pos);
                        if (_grabbedItem != null && GodotObject.IsInstanceValid(_grabbedItem))
                        {
                            _itemGrabOffset = pos - _grabbedItem.Position;
                            _itemGrabStartPos = _grabbedItem.Position;
                            _itemPrevPos = _grabbedItem.Position;
                            _itemVelocity = Vector2.Zero;
                            _itemLastDragTime = _mobileTouchStartTime;
                            SomethingHasBeenGrabbed = true;
                            _grabbedItem.UpdateCombinationShaders(enable: true);
                            // Pickup dialogue chance
                            if (_grabbedItem.itemObject.itemInformation.possiblePickUpDialogue.Count() > 0 && mainCharacter.Visible && (float)GD.RandRange(0, 100) < _grabbedItem.itemObject.itemInformation.PerchentChanceOfDialogue)
                            {
                                DialogueDataRes d = PickDialogue(_grabbedItem.itemObject.itemInformation.possiblePickUpDialogue);
                                if (d != null)
                                {
                                    dialogueStack.Add(d);
                                    PopDialogueInStack(skipTimer: true);
                                }
                            }
                        }
                    }
                    else if (IsPointOnAnyActor(pos))
                    {
                        _mobileTouchTarget = MobileTouchTargetKind.Actor;
                        Input.ActionPress("Pet");
                    }
                    else if (GetThinnerCollisionBox().HasPoint(pos))
                    {
                        _mobileTouchTarget = MobileTouchTargetKind.Byte;
                        Input.ActionPress("Move");
                        Input.ActionPress("Pet");
                    }
                    else
                    {
                        _mobileTouchTarget = MobileTouchTargetKind.None;
                    }
                }
                else if (touch.Index == _mobileTouchIndex)
                {
                    // Touch released
                    double duration = Time.GetTicksMsec() / 1000.0 - _mobileTouchStartTime;
                    if (_mobileTouchTarget == MobileTouchTargetKind.Actor)
                        Input.ActionRelease("Pet");
                    if (_mobileTouchTarget == MobileTouchTargetKind.Byte)
                    {
                        Input.ActionRelease("Move");
                        Input.ActionRelease("Pet");
                    }
                    if (_mobileTouchTarget == MobileTouchTargetKind.Item && _grabbedItem != null && GodotObject.IsInstanceValid(_grabbedItem))
                    {
                        // Release item: check use/combine/throw
                        ReleaseGrabbedItem(pos);
                    }
                    if (_mobileTouchTarget == MobileTouchTargetKind.None && duration >= 0.5 && !_mobileTouchMoved)
                    {
                        Input.ActionPress("PauseGame");
                        Callable.From(() => Input.ActionRelease("PauseGame")).CallDeferred();
                    }
                    _mobileTouchTarget = MobileTouchTargetKind.None;
                    _mobileTouchIndex = -1;
                    _grabbedItem = null;
                }
            }
            else if (@event is InputEventScreenDrag drag)
            {
                if (drag.Index == _mobileTouchIndex)
                {
                    Vector2I pos = (Vector2I)drag.Position;
                    _lastTouchPos = pos;
                    if (pos.DistanceTo(_mobileTouchStart) > 24f)
                        _mobileTouchMoved = true;
                    double now = Time.GetTicksMsec() / 1000.0;
                    double dt = Math.Max(now - _lastDragTime, 0.001);
                    _mobileDragVelocity = (Vector2)(pos - _prevTouchPos) / (float)dt;
                    _prevTouchPos = pos;
                    _lastDragTime = now;
                    if (_mobileTouchTarget == MobileTouchTargetKind.Byte && !Input.IsActionPressed("Move"))
                        Input.ActionPress("Move");
                    // Drag item
                    if (_mobileTouchTarget == MobileTouchTargetKind.Item && _grabbedItem != null && GodotObject.IsInstanceValid(_grabbedItem))
                    {
                        Vector2I newPos = pos - _itemGrabOffset;
                        Vector2I screenSize = DisplayServer.ScreenGetSize();
                        newPos.X = Mathf.Clamp(newPos.X, 0, Mathf.Max(0, screenSize.X - _grabbedItem.Size.X));
                        newPos.Y = Mathf.Clamp(newPos.Y, 0, Mathf.Max(0, screenSize.Y - _grabbedItem.Size.Y));
                        _grabbedItem.Position = newPos;
                        // Track velocity for throw
                        double itemDt = Math.Max(now - _itemLastDragTime, 0.001);
                        _itemVelocity = (Vector2)(newPos - _itemPrevPos) / (float)itemDt;
                        _itemPrevPos = newPos;
                        _itemLastDragTime = now;
                    }
                }
            }
            return;
        }
        base._Input(@event);
    }

    private ItemWindow FindItemAtPoint(Vector2I pos)
    {
        // Iterate backwards to find topmost item (last in list = drawn on top)
        for (int i = spawnedItems.Count - 1; i >= 0; i--)
        {
            ItemWindow w = spawnedItems[i];
            if (!GodotObject.IsInstanceValid(w) || w.CurrentlyPickedUp) continue;
            bool logicallyVisible = _isMobile ? w.IsActiveForMobile : w.Visible;
            if (logicallyVisible)
            {
                Rect2I itemRect = new Rect2I(w.Position, w.Size);
                if (itemRect.HasPoint(pos))
                    return w;
            }
        }
        return null;
    }

    private void ReleaseGrabbedItem(Vector2I releasePos)
    {
        if (_grabbedItem == null || !GodotObject.IsInstanceValid(_grabbedItem))
            return;
        ItemWindow item = _grabbedItem;
        item.UpdateCombinationShaders(enable: false);
        SomethingHasBeenGrabbed = false;

        // Throw check
        if (settingWindowThrowPhysics && _itemVelocity.Length() > 260f)
        {
            Vector2 throwVel = _itemVelocity * mouseVelocityScaler;
            throwVel.Y = Mathf.Clamp(throwVel.Y, -1600f, float.MaxValue);
            item.itemWindowVelocity = throwVel;
            item.isThrown = true;
            _grabbedItem = null;
            return;
        }

        // Check if dropped on Byte
        Rect2I itemRect = new Rect2I(item.Position, item.Size);
        if (mainCharacter.Visible && GetThinnerCollisionBox().Intersects(itemRect))
        {
            item.UseOnMainActor();
            if (!item.itemObject.itemInformation.isReusable)
            {
                TweenGrabRelease();
                spawnedItems.Remove(item);
                item.CallDeferred("queue_free");
            }
            _grabbedItem = null;
            return;
        }

        // Check if dropped on NPC
        if (item.itemObject.itemInformation.possibleUsableAIs.Count() > 0)
        {
            foreach (ActorWindow actor in spawnedActors)
            {
                if (!GodotObject.IsInstanceValid(actor) || !actor.Visible)
                    continue;
                AiItemDataRes aiData = null;
                foreach (AiItemDataRes usable in item.itemObject.itemInformation.possibleUsableAIs)
                {
                    if (usable.targetActorsID == actor.characterActor.characterInformation._itemID)
                    {
                        aiData = usable;
                        break;
                    }
                }
                if (aiData == null) continue;
                if (new Rect2I(actor.Position, actor.Size).Intersects(itemRect))
                {
                    item.UseOnOtherActor(actor, aiData);
                    if (!item.itemObject.itemInformation.isReusable)
                    {
                        TweenGrabRelease();
                        spawnedItems.Remove(item);
                        item.CallDeferred("queue_free");
                    }
                    _grabbedItem = null;
                    return;
                }
            }
        }

        // Check combine with other items
        item.CombineItem(itemRect);
        _grabbedItem = null;
    }

    [Signal]
    public delegate void ReachedTargetEventHandler();

    public enum ItemTargetMode
    {
        UNTYPED,
        RANDOM,
        CLOSEST,
        FARTHEST
    }

    

    

    

    public Window mainWindow;

    [Export(PropertyHint.None, "")]
    public Character mainCharacter;

    [Export(PropertyHint.None, "")]
    public SaveHandler saveHandler;

    [ExportGroup("Item Spawner Logic", "")]
    public Array<ItemWindow> spawnedItems = new Array<ItemWindow>();

    [Export(PropertyHint.None, "")]
    public int maxItems = 10;

    [Export(PropertyHint.None, "")]
    public Vector2 spawnMargin = new Vector2(256f, 128f);

    [Export(PropertyHint.None, "")]
    public Timer spawnerTimer;

    [Export(PropertyHint.None, "")]
    public Vector2 spawnerTimeRange = new Vector2(30f, 120f);

    [ExportSubgroup("Attachment Logic", "")]
    public Array<AttachObjWindow> spawnedAttachments = new Array<AttachObjWindow>();

    [ExportSubgroup("Dialogue Logic", "")]
    public Array<DialogueDataRes> dialogueStack = new Array<DialogueDataRes>();

    [Export(PropertyHint.None, "")]
    public Vector2 timeBetweenStacks = new Vector2(0.5f, 1.5f);

    [Export(PropertyHint.None, "")]
    public AttachDataRes defaultTextDataRes;

    [Export(PropertyHint.None, "")]
    public Timer randomDialogueTimer;

    [ExportGroup("Actor Spawner Logic", "")]
    public Array<ActorWindow> spawnedActors = new Array<ActorWindow>();

    public Array<ActorWindow> spawnedCompanions = new Array<ActorWindow>();

    [Export(PropertyHint.None, "")]
    public int companionLimit = 3;

    [Export(PropertyHint.None, "")]
    public Timer spawnerActorTimer;

    [Export(PropertyHint.None, "")]
    public Vector2 spawnerActorTimerRange = new Vector2(300f, 600f);

    [ExportGroup("Random Animation Logic", "")]
    [Export(PropertyHint.None, "")]
    public Timer randomAnimationTimer;

    [ExportGroup("Passive Play Logic", "")]
    [Export(PropertyHint.None, "")]
    public Timer passivePlayTimer;

    [Export(PropertyHint.None, "")]
    public Vector2 passivePlayTimerOffRange = new Vector2(60f, 120f);

    [Export(PropertyHint.None, "")]
    public Vector2 passivePlayTimerOnRange = new Vector2(15f, 30f);

    [ExportGroup("Reaction Dialogue Chances", "")]
    [Export(PropertyHint.None, "")]
    public float throwSoftReactionChance = 5f;

    [Export(PropertyHint.None, "")]
    public float throwHardReactionChance = 20f;

    [Export(PropertyHint.None, "")]
    public float landSoftReactionChance = 5f;

    [Export(PropertyHint.None, "")]
    public float landHardReactionChance = 20f;

    [ExportGroup("Reference UIDs", "")]
    [Export(PropertyHint.None, "")]
    public PackedScene ItemObjectScene;

    [Export(PropertyHint.None, "")]
    public PackedScene AttachmentObjectScene;

    [Export(PropertyHint.None, "")]
    public PackedScene actorScene;

    [Export(PropertyHint.None, "")]
    public PackedScene pauseMenu;

    [Export(PropertyHint.None, "")]
    public PackedScene terminalMenu;

    [Export(PropertyHint.None, "")]
    public PackedScene confirmationMenu;

    [Export(PropertyHint.None, "")]
    public PackedScene eulaMenu;

    [Export(PropertyHint.None, "")]
    public PackedScene magnifierScene;

    [ExportGroup("Internal Settings", "")]
    [Export(PropertyHint.None, "")]
    public bool settingWindowThrowPhysics = true;

    public bool settingEULA;

    public float settingSpriteScaler = 1f;

    public float settingItemScaler = 1f;

    public float settingUIScaler = 1f;

    public bool settingSpawnItems = true;

    public bool settingSpawnActors = true;

    public bool settingPassivePlayMode;

    public bool settingRemovePopups;

    public bool settingRemoveConvos;

    public bool settingAudioOn = true;

    public bool settingMods;

    public Array<string> settingEnabledMods = new Array<string>();

    public string userInfoName = "USER";

    public Godot.Collections.Dictionary<SaveHandler.SeenObjectTypes, Array<string>> SeenObjects = new Godot.Collections.Dictionary<SaveHandler.SeenObjectTypes, Array<string>>
    {
        {
            SaveHandler.SeenObjectTypes.ITEMS,
            new Array<string>()
        },
        {
            SaveHandler.SeenObjectTypes.NSFW_SCENES,
            new Array<string>()
        },
        {
            SaveHandler.SeenObjectTypes.BRAIN_DANCE_SCENES,
            new Array<string>()
        },
        {
            SaveHandler.SeenObjectTypes.POP_UPS,
            new Array<string>()
        }
    };

    public int userTickets;

    public Godot.Collections.Dictionary<string, Variant> minigameData = new Godot.Collections.Dictionary<string, Variant>();

    public Array<SaveHandler.Kinks> settingBlacklistedContent = new Array<SaveHandler.Kinks>();

    private Vector2 mouseOffset = Vector2.Zero;

    private bool selected;

    public bool SomethingHasBeenGrabbed;

    private Tween grabReleaseTween;

    public ScreenDataHandler screenDataHandler = new ScreenDataHandler();

    private bool isWalking;

    private int walkDirection = 1;

    private const float WalkSpeed = 250f;

    private float pickupWatchdogTimer;

    public float mouseVelocityScaler = 0.45f;

    public float windowBounceDamping = -0.55f;

    public float windowAirResist = 60f;

    private Vector2 windowVelocity = Vector2.Zero;

    private bool isThrown;

    private const float windowGravity = 1400f;

    private const float windowDamping = 0.995f;

    private bool isWalkingToTargetPosition;

    private int targetX;

    private ItemWindow walkTargetItem;

    private ItemWindow storedItem;

    private bool inPickup;

    public bool isInConvo;

    public Array<Window> spawnedMinigames = new Array<Window>();

    public PauseMenu Pause;

    public TerminalWindow Terminal;

    public bool AdminAccess;

    public MagnifierWindow Magnifier;

    private bool _magnifierActive;

        public static Main Instance { get; private set; }

    

    public override void _Ready()
    {
        if (!_isMobileChecked)
        {
            _isMobile = OS.HasFeature("mobile") || OS.HasFeature("android");
            _isMobileChecked = true;
        }

        PowerThrottling.DisableThrottling();
        Instance = this;
        mainWindow = GetWindow();
        if (!_isMobile) mainWindow.TransparentBg = true;
        saveHandler.AttemptLoad();
        mainCharacter.trueSize = (Vector2I)(mainCharacter.characterInformation.characterSize * mainCharacter.characterInformation.characterScale * settingSpriteScaler);
        mainCharacter.SetupCharacter();
        screenDataHandler.UpdateScreenInfo(mainCharacter.trueSize);
        mainWindow.MinSize = mainCharacter.trueSize;
        mainWindow.Size = mainWindow.MinSize;
        if (!_isMobile) mainWindow.Borderless = true;
        if (!_isMobile) mainWindow.Unresizable = true;
        if (!_isMobile) mainWindow.AlwaysOnTop = true;
        if (!_isMobile) mainWindow.GuiEmbedSubwindows = false;
        if (!_isMobile) mainWindow.Transparent = true;
        if (!_isMobile) mainWindow.Position = new Vector2I(DisplayServer.ScreenGetSize(screenDataHandler.screenIndex).X / 2 - mainCharacter.trueSize.X / 2, screenDataHandler.taskbarPos);
        else
        {
            Vector2I screenSize = DisplayServer.ScreenGetSize();
            Position = new Vector2(screenSize.X / 2 - mainCharacter.trueSize.X / 2, screenSize.Y - mainCharacter.trueSize.Y);
        }
        if (_isMobile)
        {
            AddChild(new MobileUI());
            if (spawnerTimer != null && spawnerTimer.IsStopped())
            {
                spawnerTimer.WaitTime = 4f;
                spawnerTimer.Start();
            }
            if (spawnerActorTimer != null && spawnerActorTimer.IsStopped())
            {
                spawnerActorTimer.WaitTime = 20f;
                spawnerActorTimer.Start();
            }
            _resourceLoadRetries = 0;
            TryLoadResources();
        }
        Callable.From(delegate
        {
            BootDialogue(!settingEULA);
        }).CallDeferred();
    }

    public override void _Process(double delta)
    {
        if (!settingEULA)
        {
            GetTree().Paused = true;
            ConfirmationMenu confirmationMenu = eulaMenu.Instantiate<ConfirmationMenu>(PackedScene.GenEditState.Disabled);
            AddChild(confirmationMenu, forceReadableName: false, InternalMode.Disabled);
            confirmationMenu.Confirmed += delegate
            {
                settingEULA = true;
                GetTree().Paused = false;
                saveHandler.SaveSettings();
            };
            confirmationMenu.Deny += delegate
            {
                GetTree().Quit();
            };
        }
        if (inPickup)
        {
            pickupWatchdogTimer += (float)delta;
            if (pickupWatchdogTimer > 6f)
            {
                GD.PrintErr("Pickup watchdog triggered — forcing release of stuck item.");
                AbortActivePickup();
                pickupWatchdogTimer = 0f;
            }
        }
        else
        {
            pickupWatchdogTimer = 0f;
        }
        if (Input.IsActionJustPressed("PauseGame"))
        {
            PauseGame();
        }
        if (Input.IsActionJustPressed("Terminal"))
        {
            if (Terminal == null)
            {
                OpenTerminal();
            }
            else
            {
                Terminal.CallDeferred("queue_free");
                Terminal = null;
            }
        }
        if (Input.IsActionJustPressed("Screen_Lock") && GetThinnerCollisionBox().HasPoint(MobileMousePos()))
        {
            if (screenDataHandler.IsScreenLocked)
            {
                screenDataHandler.UnlockScreen();
                ClearAllAttachments();
                dialogueStack.Add(mainCharacter.characterInformation.responseTexts[CharacterInfoDataRes.ResponseToSituation.UNLOCKED_FROM_MONITOR]);
                PopDialogueInStack(skipTimer: true);
            }
            else
            {
                screenDataHandler.LockToCurrentScreen(mainCharacter.trueSize);
                ClearAllAttachments();
                dialogueStack.Add(mainCharacter.characterInformation.responseTexts[CharacterInfoDataRes.ResponseToSituation.LOCKED_TO_MONITOR]);
                PopDialogueInStack(skipTimer: true);
            }
        }
        if (Input.IsActionPressed("Shift_Toggle") && Input.IsActionJustPressed("Despawn"))
        {
            RepositionAllItemsToMouseScreen();
        }
        Magnify(delta);
        if (selected && mainCharacter.Visible)
        {
            FollowMouse(Unlocked: true);
        }
        else if (isThrown)
        {
            windowVelocity.Y += 1400f * (float)delta;
            windowVelocity *= Mathf.Pow(0.995f, (float)delta * windowAirResist);
            if (_isMobile)
            {
                Vector2I screenSize = DisplayServer.ScreenGetSize(screenDataHandler.screenIndex);
                int groundY = screenSize.Y - mainCharacter.trueSize.Y;
                Vector2 pos = Position + windowVelocity * (float)delta;
                if (pos.X < 0f)
                {
                    pos.X = 0f;
                    windowVelocity.X *= windowBounceDamping;
                    mainCharacter.ApplyBounceRotation(windowVelocity.X);
                }
                else if (pos.X > (float)(screenSize.X - mainCharacter.trueSize.X))
                {
                    pos.X = screenSize.X - mainCharacter.trueSize.X;
                    windowVelocity.X *= windowBounceDamping;
                    mainCharacter.ApplyBounceRotation(windowVelocity.X);
                }
                if (pos.Y >= (float)groundY)
                {
                    pos.Y = groundY;
                    isThrown = false;
                    windowVelocity = Vector2.Zero;
                    mainCharacter.CancelThrowRotation();
                    mainCharacter.BeginLand();
                    CheckLandingInteraction(mainCharacter.throwWasHard);
                }
                Position = pos;
                mainCharacter.UpdateThrowRotation(delta);
            }
            else
            {
                Vector2I position = new Vector2I(mainWindow.Position.X + Mathf.RoundToInt(windowVelocity.X * (float)delta), mainWindow.Position.Y + Mathf.RoundToInt(windowVelocity.Y * (float)delta));
                int num = screenDataHandler.ClampAcrossAllScreensX(position.X, mainCharacter.trueSize.X);
                if (num != position.X)
                {
                    windowVelocity.X *= windowBounceDamping;
                    position.X = num;
                    mainCharacter.ApplyBounceRotation(windowVelocity.X);
                }
                if (position.Y >= screenDataHandler.taskbarPos)
                {
                    position.Y = screenDataHandler.taskbarPos;
                    isThrown = false;
                    mainWindow.Position = position;
                    mainCharacter.BeginLand();
                    windowVelocity = Vector2.Zero;
                    mainCharacter.CancelThrowRotation();
                    CheckLandingInteraction(mainCharacter.throwWasHard);
                }
                mainWindow.Position = position;
                mainCharacter.UpdateThrowRotation(delta);
            }
        }
        else if (_isMobile)
        {
            Vector2I screenSize = DisplayServer.ScreenGetSize(screenDataHandler.screenIndex);
            int groundY = screenSize.Y - mainCharacter.trueSize.Y;
            if (Position.Y < (float)groundY)
            {
                Position += new Vector2(0f, 980f * (float)delta);
                if (Position.Y > (float)groundY)
                {
                    Position = new Vector2(Position.X, groundY);
                }
            }
            else
            {
                Position = new Vector2(Position.X, groundY);
            }
        }
        else
        {
            if (mainWindow.Position.Y < screenDataHandler.taskbarPos)
            {
                mainWindow.Position += new Vector2I(0, Mathf.RoundToInt(980f * (float)delta));
            }
            else
            {
                mainWindow.Position = new Vector2I(mainWindow.Position.X, screenDataHandler.taskbarPos);
            }
        }
        if (mainCharacter.Visible)
        {
            MovePet();
            if (Input.IsActionJustPressed("Pet") && GetThinnerCollisionBox().HasPoint(MobileMousePos()))
            {
                if (isInConvo)
                {
                    dialogueStack.Clear();
                    dialogueStack.Add(mainCharacter.characterInformation.responseTexts[CharacterInfoDataRes.ResponseToSituation.IN_CONVO]);
                    PopDialogueInStack(skipTimer: true);
                    isInConvo = false;
                }
                else
                {
                    ClearAllAttachments();
                    DialogueDataRes dialogueDataRes = PickDialogue(mainCharacter.characterInformation.interactionTexts);
                    if (dialogueDataRes != null)
                    {
                        dialogueStack.Add(dialogueDataRes);
                        PopDialogueInStack(skipTimer: true);
                    }
                    if (mainCharacter.petMainBodyState == Character.MainBodyStates.Idle)
                    {
                        mainCharacter.ForceMainBodyState(Character.MainBodyStates.Forced_Animation, "Pet", 0.5f);
                    }
                }
            }
            if (Input.IsActionPressed("Pet") && GetThinnerCollisionBox().HasPoint(MobileMousePos()) && mainCharacter.petMainBodyState == Character.MainBodyStates.Idle)
            {
                mainCharacter.ForceMainBodyState(Character.MainBodyStates.Forced_Animation, "Pet", 0.5f);
            }
            if (Input.IsActionJustPressed("Sit") && (mainCharacter.petMainBodyState == Character.MainBodyStates.Idle || mainCharacter.petMainBodyState == Character.MainBodyStates.Walk))
            {
                isWalking = false;
                AbortActivePickup();
                mainCharacter.BeginSit();
            }
            if (Input.IsActionJustPressed("Clothing_Up"))
            {
                isWalking = false;
                AbortActivePickup();
                mainCharacter.SelectClothing(1);
            }
            if (Input.IsActionJustPressed("Clothing_Down"))
            {
                isWalking = false;
                AbortActivePickup();
                mainCharacter.SelectClothing(-1);
            }
            if (Input.IsActionJustPressed("Debug") && OS.HasFeature("editor"))
            {
                TrySetTargetToItem();
                if (Mathf.Abs(mainWindow.Position.X - GetResolvedTargetX()) <= 5)
                {
                    ReachedItemTarget();
                }
                else
                {
                    StartAutoWalkToTarget();
                }
            }
        }
        else if (Input.IsActionJustPressed("Despawn") && GetThinnerCollisionBox().HasPoint(MobileMousePos()))
        {
            ClearAllAttachments();
            dialogueStack.Clear();
            dialogueStack.Add(mainCharacter.characterInformation.responseTexts[CharacterInfoDataRes.ResponseToSituation.IN_CONVO]);
            PopDialogueInStack(skipTimer: true);
            mainCharacter.ForceMainBodyState(Character.MainBodyStates.Forced_Animation, "Pet", 0.5f);
        }
        screenDataHandler.UpdateCurrentScreen(mainWindow, mainCharacter.trueSize);
        if (spawnedAttachments.Count() <= 0)
        {
            return;
        }
        foreach (AttachObjWindow spawnedAttachment in spawnedAttachments)
        {
            if (GodotObject.IsInstanceValid(spawnedAttachment) && spawnedAttachment.attachObject.attachedItemInformation.attachmentTyping != AttachDataRes.AttachmentType.OVERRIDE)
            {
                spawnedAttachment.FollowParent();
            }
        }
    }

    public override void _PhysicsProcess(double delta)
    {
        if (isWalking && mainCharacter.Visible)
        {
            Walk(delta);
        }
        if (selected)
        {
            mainCharacter.UpdateDangle(delta);
        }
    }

    public void Magnify(double delta)
    {
        if (Input.IsActionJustPressed("Magnifier") && Terminal == null && spawnedMinigames.Count == 0)
        {
            if (!_magnifierActive)
            {
                _magnifierActive = true;
                MagnifierWindow node = (Magnifier = magnifierScene.Instantiate<MagnifierWindow>(PackedScene.GenEditState.Disabled));
                Magnifier.Size = new Vector2I((int)((float)Magnifier.Size.X * settingUIScaler), (int)((float)Magnifier.Size.Y * settingUIScaler));
                GetTree().Root.AddChild(node, forceReadableName: false, InternalMode.Disabled);
            }
            else
            {
                _magnifierActive = false;
                if (Magnifier != null && GodotObject.IsInstanceValid(Magnifier))
                {
                    Magnifier.QueueFree();
                }
                Magnifier = null;
            }
        }
        if (Magnifier != null && GodotObject.IsInstanceValid(Magnifier))
        {
            if (Input.IsActionJustReleased("MouseWheelUp"))
            {
                Magnifier.AdjustMagnification(0.5f);
            }
            else if (Input.IsActionJustReleased("MouseWheelDown"))
            {
                Magnifier.AdjustMagnification(-0.5f);
            }
        }
    }

    public void RepositionAllItemsToMouseScreen()
    {
        if (spawnedItems.Count == 0)
        {
            return;
        }
        Vector2I vector2I = MobileMousePos();
        int screenCount = DisplayServer.GetScreenCount();
        Rect2I rect2I = DisplayServer.ScreenGetUsableRect(0);
        for (int i = 0; i < screenCount; i++)
        {
            Rect2I rect2I2 = DisplayServer.ScreenGetUsableRect(i);
            if (vector2I.X >= rect2I2.Position.X && vector2I.X < rect2I2.Position.X + rect2I2.Size.X && vector2I.Y >= rect2I2.Position.Y && vector2I.Y < rect2I2.Position.Y + rect2I2.Size.Y)
            {
                rect2I = rect2I2;
                break;
            }
        }
        foreach (ItemWindow spawnedItem in spawnedItems)
        {
            if (GodotObject.IsInstanceValid(spawnedItem))
            {
                int x = (int)GD.RandRange((float)rect2I.Position.X + spawnMargin.X, (float)(rect2I.Position.X + rect2I.Size.X) - spawnMargin.X);
                int y = rect2I.Position.Y - Mathf.RoundToInt(spawnMargin.Y);
                spawnedItem.Position = new Vector2I(x, y);
            }
        }
    }

    public void BootDialogue(bool firstTime = false)
    {
        if (firstTime && mainCharacter.characterInformation.firstTimeStartupMessage != null)
        {
            if (userInfoName == "USER")
            {
                string text = System.Environment.GetEnvironmentVariable("USERNAME") ?? System.Environment.GetEnvironmentVariable("USER") ?? "USER";
                userInfoName = text;
                saveHandler.SaveSettings();
            }
            ClearAllAttachments();
            dialogueStack.Add(mainCharacter.characterInformation.firstTimeStartupMessage);
            PopDialogueInStack(skipTimer: true);
            if (mainCharacter.petMainBodyState == Character.MainBodyStates.Idle)
            {
                mainCharacter.ForceMainBodyState(Character.MainBodyStates.Forced_Animation, "Wave", 0.5f);
            }
        }
        else
        {
            if (mainCharacter.characterInformation.welcomeMessages.Count() == 0)
            {
                return;
            }
            ClearAllAttachments();
            List<DialogueDataRes> list = mainCharacter.characterInformation.welcomeMessages.Where((DialogueDataRes d) => !IsBlacklisted(d.taggedKinks)).ToList();
            if (list.Count > 0)
            {
                dialogueStack.Add(list[(int)(GD.Randi() % list.Count)]);
                PopDialogueInStack(skipTimer: true);
                if (mainCharacter.petMainBodyState == Character.MainBodyStates.Idle)
                {
                    mainCharacter.ForceMainBodyState(Character.MainBodyStates.Forced_Animation, "Wave", 0.5f);
                }
            }
        }
    }

    public void PauseGame()
    {
        bool flag = false;
        foreach (MinigameBase spawnedMinigame in spawnedMinigames)
        {
            if (spawnedMinigame.OverridePause)
            {
                flag = true;
            }
            spawnedMinigame.PauseGame(Pause: true);
        }
        if (!flag)
        {
            AbortActivePickup();
            PauseMenu pauseMenu = this.pauseMenu.Instantiate<PauseMenu>(PackedScene.GenEditState.Disabled);
            pauseMenu.Size = new Vector2I(Mathf.RoundToInt((float)pauseMenu.Size.X * settingUIScaler), Mathf.RoundToInt((float)pauseMenu.Size.Y * settingUIScaler));
            Pause = pauseMenu;
            AddChild(pauseMenu, forceReadableName: false, InternalMode.Disabled);
            GetTree().Paused = true;
        }
    }

    public void OpenTerminal()
    {
        AbortActivePickup();
        TerminalWindow terminalWindow = terminalMenu.Instantiate<TerminalWindow>(PackedScene.GenEditState.Disabled);
        terminalWindow.Size = new Vector2I(Mathf.RoundToInt((float)terminalWindow.Size.X * settingUIScaler), Mathf.RoundToInt((float)terminalWindow.Size.Y * settingUIScaler));
        AddChild(terminalWindow, forceReadableName: false, InternalMode.Disabled);
        Terminal = terminalWindow;
    }

    public void CallCharacterForcedAnimation(string animationName, float animationTime)
    {
        isWalking = false;
        mainCharacter.ForceMainBodyState(Character.MainBodyStates.Forced_Animation, animationName, animationTime);
    }

    public void CallCharacterAttachmentSpawn(AttachDataRes objData, bool unclearableAttachment = false)
    {
        CallCharacterAttachmentSpawn(objData, unclearableAttachment, null);
    }

    public void CallCharacterAttachmentSpawn(AttachDataRes objData, bool unclearableAttachment, Window targetWindow)
    {
        if (settingRemovePopups && objData.attachmentTyping == AttachDataRes.AttachmentType.RANDOM_CLICKED_WINDOW)
        {
            if (objData.possibleItems.Count() <= 0 || !((float)GD.RandRange(0, 100) <= objData.chanceOfItem))
            {
                return;
            }
            WeightGroup<ItemDataRes> weightGroup = new WeightGroup<ItemDataRes>();
            foreach (ItemDataRes possibleItem in objData.possibleItems)
            {
                if (!IsBlacklisted(possibleItem.taggedKinks))
                {
                    weightGroup.Add(possibleItem, Mathf.Clamp(possibleItem.itemSpawnWeight, 1.0, 10000.0));
                }
            }
            if (weightGroup.Count() == 0)
            {
                return;
            }
            ItemDataRes item = weightGroup.GetItem(GD.RandRange(0, 10000));
            int num = (int)GD.RandRange((float)screenDataHandler.EffectiveLeftX + spawnMargin.X, (float)screenDataHandler.EffectiveRightX - spawnMargin.X);
            int y = DisplayServer.ScreenGetUsableRect(screenDataHandler.screenIndex).Position.Y;
            int screenCount = DisplayServer.GetScreenCount();
            for (int i = 0; i < screenCount; i++)
            {
                Rect2I rect2I = DisplayServer.ScreenGetUsableRect(i);
                if (num >= rect2I.Position.X && num < rect2I.Position.X + rect2I.Size.X)
                {
                    y = rect2I.Position.Y;
                    break;
                }
            }
            Vector2I spawningPosition = new Vector2I(num, y - Mathf.RoundToInt(spawnMargin.Y));
            CallItemSpawn(item, spawningPosition);
        }
        else
        {
            AttachObjWindow attachObjWindow = AttachmentObjectScene.Instantiate<AttachObjWindow>(PackedScene.GenEditState.Disabled);
            attachObjWindow.attachObject.attachedItemInformation = objData;
            if (targetWindow != null)
            {
                attachObjWindow.parentWindow = targetWindow;
            }
            attachObjWindow.SetupAttachmentWindow();
            AddChild(attachObjWindow, forceReadableName: false, InternalMode.Disabled);
            if (!unclearableAttachment)
            {
                spawnedAttachments.Add(attachObjWindow);
            }
        }
    }

    public async void PopDialogueInStack(bool skipTimer = false)
    {
        if (dialogueStack != null && dialogueStack.Count() > 0)
        {
            if (!skipTimer)
            {
                float num = (float)GD.RandRange(timeBetweenStacks.X, timeBetweenStacks.Y);
                await ToSignal(GetTree().CreateTimer(num), SceneTreeTimer.SignalName.Timeout);
            }
            if (dialogueStack.Count() != 0)
            {
                CallCharacterDialogueAttachmentSpawn(dialogueStack[0]);
                dialogueStack.RemoveAt(0);
            }
        }
        else if (isInConvo)
        {
            isInConvo = false;
        }
    }

    public void CallCharacterDialogueAttachmentSpawn(DialogueDataRes diaData)
    {
        AttachObjWindow attachObjWindow = AttachmentObjectScene.Instantiate<AttachObjWindow>(PackedScene.GenEditState.Disabled);
        attachObjWindow.attachObject.attachedItemInformation = (AttachDataRes)defaultTextDataRes.Duplicate(deep: true);
        if (diaData.BubbleMargin != attachObjWindow.attachObject.attachedItemInformation.attachmentMargin)
        {
            attachObjWindow.attachObject.attachedItemInformation.attachmentMargin = diaData.BubbleMargin;
            if (diaData.BubbleMargin.Y == 0f)
            {
                GD.PrintErr("Dialogue Y Margin set to 0, please set it to a number above!");
            }
        }
        if (!string.IsNullOrEmpty(diaData.speakingActorID) && !(diaData.speakingActorID == mainCharacter.characterInformation._itemID))
        {
            foreach (ActorWindow spawnedActor in spawnedActors)
            {
                if (GodotObject.IsInstanceValid(spawnedActor) && spawnedActor.characterActor.characterInformation == ResourceCache.resourcesLoaded[ResourceCache.ResourceTyping.CHARACTER][diaData.speakingActorID])
                {
                    attachObjWindow.parentWindow = spawnedActor;
                    break;
                }
            }
        }
        AddChild(attachObjWindow, forceReadableName: false, InternalMode.Disabled);
        attachObjWindow.CallDeferred("SetupAttachmentWindow", diaData);
        spawnedAttachments.Add(attachObjWindow);
    }

    public void CallActorSpawn(CharacterInfoDataRes spawningActor)
    {
        CallActorSpawn(spawningActor, Vector2I.Zero);
    }

    public void CallActorSpawn(CharacterInfoDataRes spawningActor, Vector2I Pos, ActorWindow possibleTarget = null)
    {
        ActorWindow actorWindow = actorScene.Instantiate<ActorWindow>(PackedScene.GenEditState.Disabled);
        actorWindow.characterActor.characterInformation = spawningActor;
        actorWindow.targetWindow = possibleTarget;
        AddChild(actorWindow, forceReadableName: false, InternalMode.Disabled);
        actorWindow.CallDeferred("SetupActorWindow", Pos, spawningActor);
        spawnedActors.Add(actorWindow);
        if (spawningActor.AITyping == CharacterInfoDataRes.AITypes.COMPANION)
        {
            spawnedCompanions.Add(actorWindow);
        }
    }

    public void CallItemSpawn(ItemDataRes spawningItem, Vector2I spawningPosition)
    {
        ItemWindow itemWindow = ItemObjectScene.Instantiate<ItemWindow>(PackedScene.GenEditState.Disabled);
        AddChild(itemWindow, forceReadableName: false, InternalMode.Disabled);
        itemWindow.SetupItemWindow(spawningItem);
        GD.Print("Item Spawn Position [" + spawningPosition.ToString() + "] - Current Byte Position [" + mainWindow.Position.ToString() + "]");
        itemWindow.Position = spawningPosition;
        spawnedItems.Add(itemWindow);
    }

    public void CallPackedSceneSpawn(string genericID)
    {
        if (string.IsNullOrEmpty(genericID))
        {
            GD.PrintErr("CallGenericSpawn: genericID is null or empty.");
            return;
        }
        if (!ResourceCache.prefabsLoaded.ContainsKey(ResourceCache.PrefabTyping.UNTYPED) || !ResourceCache.prefabsLoaded[ResourceCache.PrefabTyping.UNTYPED].ContainsKey(genericID))
        {
            GD.PrintErr("CallGenericSpawn: No generic found with ID '" + genericID + "'.");
            return;
        }
        PackedScene packedScene = ResourceCache.prefabsLoaded[ResourceCache.PrefabTyping.UNTYPED][genericID];
        Window genericInstance = packedScene.Instantiate<Window>(PackedScene.GenEditState.Disabled);
        genericInstance.Name = genericID;
        genericInstance.Size = new Vector2I(Mathf.RoundToInt((float)genericInstance.Size.X * settingUIScaler), Mathf.RoundToInt((float)genericInstance.Size.Y * settingUIScaler));
        GetTree().Root.AddChild(genericInstance, forceReadableName: false, InternalMode.Disabled);
        genericInstance.CloseRequested += delegate
        {
            mainWindow.GrabFocus();
            genericInstance.QueueFree();
        };
        GD.Print("CallGenericSpawn: Spawned generic '" + genericID + "'.");
    }

    public void CallMinigameSpawn(string minigameID)
    {
        if (string.IsNullOrEmpty(minigameID))
        {
            GD.PrintErr("CallMinigameSpawn: minigameID is null or empty.");
            return;
        }
        if (!ResourceCache.prefabsLoaded.ContainsKey(ResourceCache.PrefabTyping.MINIGAME) || !ResourceCache.prefabsLoaded[ResourceCache.PrefabTyping.MINIGAME].ContainsKey(minigameID))
        {
            GD.PrintErr("CallMinigameSpawn: No minigame found with ID '" + minigameID + "'.");
            return;
        }
        foreach (Window spawnedMinigame in spawnedMinigames)
        {
            if (GodotObject.IsInstanceValid(spawnedMinigame) && spawnedMinigame.Name == (StringName)minigameID)
            {
                GD.Print("CallMinigameSpawn: '" + minigameID + "' is already open.");
                spawnedMinigame.GrabFocus();
                return;
            }
        }
        PackedScene packedScene = ResourceCache.prefabsLoaded[ResourceCache.PrefabTyping.MINIGAME][minigameID];
        Window minigameInstance = packedScene.Instantiate<Window>(PackedScene.GenEditState.Disabled);
        minigameInstance.Name = minigameID;
        minigameInstance.Size = new Vector2I(Mathf.RoundToInt((float)minigameInstance.Size.X * settingUIScaler), Mathf.RoundToInt((float)minigameInstance.Size.Y * settingUIScaler));
        GetTree().Root.AddChild(minigameInstance, forceReadableName: false, InternalMode.Disabled);
        spawnedMinigames.Add(minigameInstance);
        minigameInstance.CloseRequested += delegate
        {
            mainWindow.GrabFocus();
            spawnedMinigames.Remove(minigameInstance);
            minigameInstance.QueueFree();
        };
        GD.Print("CallMinigameSpawn: Spawned minigame '" + minigameID + "'.");
    }

    public void OnSpawnerTimeout(int x = -1, int y = -1)
    {
        if (settingSpawnItems)
        {
            // Spawn 2x faster on mobile
            float speedMultiplier = _isMobile ? 0.5f : 1f;
            spawnerTimer.WaitTime = GD.RandRange(spawnerTimeRange.X * speedMultiplier, spawnerTimeRange.Y * speedMultiplier);
            spawnerTimer.Start();
            if (spawnedItems.Count >= maxItems)
            {
                return;
            }
            if (!ResourceCache.resourcesLoaded.ContainsKey(ResourceCache.ResourceTyping.ITEM) || ResourceCache.resourcesLoaded[ResourceCache.ResourceTyping.ITEM].Count == 0)
            {
                GD.PrintErr("[Spawner] ITEM resources not ready yet - retrying in 5s (attempting LoadData again)");
                if (ResourceCache.Instance != null)
                {
                    ResourceCache.Instance.CallDeferred("LoadData");
                }
                else
                {
                    TryLoadResources();
                }
                spawnerTimer.WaitTime = 5f;
                spawnerTimer.Start();
                return;
            }
            ICollection<string> keys = ResourceCache.resourcesLoaded[ResourceCache.ResourceTyping.ITEM].Keys;
            WeightGroup<string> weightGroup = new WeightGroup<string>();
            foreach (string item2 in keys)
            {
                if (ResourceCache.resourcesLoaded[ResourceCache.ResourceTyping.ITEM][item2] is ItemDataRes itemDataRes && !IsBlacklisted(itemDataRes.taggedKinks))
                {
                    weightGroup.Add(item2, itemDataRes.itemSpawnWeight);
                }
            }
            // Spawn 2 items at once on mobile
            int itemsToSpawn = _isMobile ? 2 : 1;
            for (int spawnCount = 0; spawnCount < itemsToSpawn && spawnedItems.Count < maxItems; spawnCount++)
            {
                string item = weightGroup.GetItem(GD.RandRange(0, 10000));
                ItemWindow itemWindow = ItemObjectScene.Instantiate<ItemWindow>(PackedScene.GenEditState.Disabled);
                AddChild(itemWindow, forceReadableName: false, InternalMode.Disabled);
                itemWindow.SetupItemWindow((ItemDataRes)ResourceCache.resourcesLoaded[ResourceCache.ResourceTyping.ITEM][item]);
                if (x == -1 && y == -1)
                {
                    int num = (int)GD.RandRange((float)screenDataHandler.EffectiveLeftX + spawnMargin.X, (float)screenDataHandler.EffectiveRightX - spawnMargin.X);
                    if (Main._isMobile)
                    {
                        Vector2I sz = DisplayServer.ScreenGetSize(screenDataHandler.screenIndex);
                        num = Mathf.Clamp(num, 0, Mathf.Max(0, sz.X - itemWindow.Size.X));
                        int yTop = Mathf.Max(DisplayServer.ScreenGetUsableRect(screenDataHandler.screenIndex).Position.Y, 0);
                        itemWindow.Position = new Vector2I(num, yTop);
                    }
                    else
                    {
                        int screenCount = DisplayServer.GetScreenCount();
                        int y2 = DisplayServer.ScreenGetUsableRect(screenDataHandler.screenIndex).Position.Y;
                        for (int i = 0; i < screenCount; i++)
                        {
                            Rect2I rect2I = DisplayServer.ScreenGetUsableRect(i);
                            if (num >= rect2I.Position.X && num < rect2I.Position.X + rect2I.Size.X)
                            {
                                y2 = rect2I.Position.Y;
                                break;
                            }
                        }
                        itemWindow.Position = new Vector2I(num, y2 - Mathf.RoundToInt(spawnMargin.Y));
                    }
                }
                else
                {
                    itemWindow.Position = new Vector2I(x, y);
                }
                spawnedItems.Add(itemWindow);
            }
        }
        else
        {
            spawnerTimer.WaitTime = GD.RandRange(spawnerTimeRange.X / 4f, spawnerTimeRange.Y / 4f);
            spawnerTimer.Start();
        }
    }

    public void OnSpawnerActorTimeout()
    {
        if (mainCharacter.Visible && settingSpawnActors)
        {
            spawnerActorTimer.WaitTime = GD.RandRange(spawnerActorTimerRange.X, spawnerActorTimerRange.Y);
            spawnerActorTimer.Start();
            ICollection<string> keys = ResourceCache.resourcesLoaded[ResourceCache.ResourceTyping.CHARACTER].Keys;
            WeightGroup<string> weightGroup = new WeightGroup<string>();
            foreach (string item2 in keys)
            {
                Resource resource = ResourceCache.resourcesLoaded[ResourceCache.ResourceTyping.CHARACTER][item2];
                CharacterInfoDataRes charData = resource as CharacterInfoDataRes;
                if (charData != null && (charData.AITyping != CharacterInfoDataRes.AITypes.COMPANION || (!spawnedCompanions.Any((ActorWindow companion) => companion.characterActor.characterInformation == charData) && spawnedCompanions.Count < companionLimit)) && !IsBlacklisted(charData.taggedKinks))
                {
                    weightGroup.Add(item2, charData.actorSpawnWeight);
                }
            }
            string item = weightGroup.GetItem(GD.RandRange(0, 10000));
            CallActorSpawn((CharacterInfoDataRes)ResourceCache.resourcesLoaded[ResourceCache.ResourceTyping.CHARACTER][item]);
        }
        else
        {
            spawnerActorTimer.WaitTime = GD.RandRange(spawnerActorTimerRange.X / 4f, spawnerActorTimerRange.Y / 4f);
            spawnerActorTimer.Start();
        }
    }

    public void OnAnimationTimerTimeout()
    {
        randomAnimationTimer.WaitTime = GD.RandRange(mainCharacter.characterInformation.randomAnimationTimer.X, mainCharacter.characterInformation.randomAnimationTimer.Y);
        if (mainCharacter.petMainBodyState != 0 || !mainCharacter.Visible)
        {
            randomAnimationTimer.WaitTime = GD.RandRange(5.0, 10.0);
            randomAnimationTimer.Start();
            return;
        }
        randomAnimationTimer.Start();
        WeightGroup<AnimDataRes> weightGroup = new WeightGroup<AnimDataRes>();
        foreach (AnimDataRes randomAnimation in mainCharacter.characterInformation.randomAnimations)
        {
            if (randomAnimation.animationName == "INVALID_ANIMATION")
            {
                GD.PrintErr("Invalid Animation Name in Key, please put a valid animation name!!!");
                continue;
            }
            bool flag = true;
            foreach (TagDataRes requiredTag in randomAnimation.RequiredTags)
            {
                TagDataRes tag = mainCharacter.GetTag(requiredTag.tagName);
                if (tag == null || (requiredTag.tagAmount > 0 && tag.tagAmount < requiredTag.tagAmount))
                {
                    flag = false;
                    break;
                }
            }
            bool flag2 = IsBlacklisted(randomAnimation.taggedKinks);
            if (flag && !flag2)
            {
                weightGroup.Add(randomAnimation, randomAnimation.animationAppeanceWeight);
            }
        }
        AnimDataRes item = weightGroup.GetItem(GD.RandRange(0, 10000));
        if (item.hasTransition)
        {
            isWalking = false;
            mainCharacter.ForceMainBodyStateTransition(item.animationName, (float)GD.RandRange(item.randomTime.X, item.randomTime.Y));
        }
        else
        {
            isWalking = false;
            mainCharacter.ForceMainBodyState(Character.MainBodyStates.Forced_Animation, item.animationName, (float)GD.RandRange(item.randomTime.X, item.randomTime.Y));
        }
    }

    public void OnPassivePlayTimerTimeout()
    {
        if (settingPassivePlayMode)
        {
            passivePlayTimer.WaitTime = GD.RandRange(passivePlayTimerOnRange.X, passivePlayTimerOnRange.Y);
        }
        else
        {
            passivePlayTimer.WaitTime = GD.RandRange(passivePlayTimerOffRange.X, passivePlayTimerOffRange.Y);
        }
        if ((mainCharacter.petMainBodyState != 0 && mainCharacter.petMainBodyState != Character.MainBodyStates.Walk) || isWalkingToTargetPosition || spawnedItems.Count() == 0 || mainCharacter.isDangling)
        {
            passivePlayTimer.WaitTime = GD.RandRange(passivePlayTimerOnRange.X, passivePlayTimerOnRange.Y);
            passivePlayTimer.Start();
            return;
        }
        passivePlayTimer.Start();
        if (TrySetTargetToItem())
        {
            if (Mathf.Abs(mainWindow.Position.X - GetResolvedTargetX()) <= 5)
            {
                ReachedItemTarget();
            }
            else
            {
                StartAutoWalkToTarget();
            }
        }
    }

    public void OnRandomDialogueTimerTimeout()
    {
        randomDialogueTimer.WaitTime = GD.RandRange(mainCharacter.characterInformation.randomDialogueTimer.X, mainCharacter.characterInformation.randomDialogueTimer.Y);
        if (!mainCharacter.Visible || isInConvo || settingRemoveConvos || isThrown)
        {
            randomDialogueTimer.WaitTime = GD.RandRange(5.0, 10.0);
            randomDialogueTimer.Start();
            return;
        }
        randomDialogueTimer.Start();
        ClearAllAttachments();
        ConvoDataRes convoDataRes = PickConvo(mainCharacter.characterInformation.randomTexts);
        if (convoDataRes != null)
        {
            dialogueStack = convoDataRes.convoStack.Duplicate(deep: true);
            PopDialogueInStack(skipTimer: true);
            isInConvo = true;
        }
    }

    public void ClearAllAttachments()
    {
        foreach (AttachObjWindow spawnedAttachment in spawnedAttachments)
        {
            if (GodotObject.IsInstanceValid(spawnedAttachment))
            {
                spawnedAttachment.QueueFree();
            }
        }
        spawnedAttachments.Clear();
        dialogueStack.Clear();
        foreach (ActorWindow spawnedCompanion in spawnedCompanions)
        {
            if (GodotObject.IsInstanceValid(spawnedCompanion))
            {
                spawnedCompanion.inUse = false;
                spawnedCompanion.inUseByAttachment = false;
                spawnedCompanion.Visible = true;
            }
        }
        mainCharacter.Visible = true;
    }

    public void ForceDropCharacter()
    {
        selected = false;
        SomethingHasBeenGrabbed = false;
        isWalking = false;
        mainCharacter.Grabbed(isGrabbed: false);
    }

    public bool TrySetTargetToItem(ItemTargetMode mode = ItemTargetMode.RANDOM, Array<ItemWindow> possibleArray = null)
    {
        if (spawnedItems == null || spawnedItems.Count == 0)
        {
            return false;
        }
        List<ItemWindow> list = (from item in spawnedItems
            where GodotObject.IsInstanceValid(item)
            where settingPassivePlayMode || !item.itemObject.itemInformation.NontargetablePickup
            where !item.itemObject.itemInformation.NoPassivePickup
            where IsItemLogicallyVisible(item)
            select item).ToList();
        if (possibleArray != null)
        {
            list.Clear();
            list = (from item in possibleArray
                where GodotObject.IsInstanceValid(item)
                where settingPassivePlayMode || !item.itemObject.itemInformation.NontargetablePickup
                where !item.itemObject.itemInformation.NoPassivePickup
                where IsItemLogicallyVisible(item)
                select item).ToList();
        }
        if (list.Count == 0)
        {
            return false;
        }
        ItemWindow itemWindow = null;
        int currentCenterX = mainWindow.Position.X + mainCharacter.trueSize.X / 2;
        switch (mode)
        {
        case ItemTargetMode.RANDOM:
            itemWindow = list[GD.RandRange(0, list.Count - 1)];
            break;
        case ItemTargetMode.CLOSEST:
            itemWindow = list.OrderBy((ItemWindow item) => Mathf.Abs(item.Position.X - currentCenterX)).First();
            break;
        case ItemTargetMode.FARTHEST:
            itemWindow = list.OrderByDescending((ItemWindow item) => Mathf.Abs(item.Position.X - currentCenterX)).First();
            break;
        }
        if (itemWindow == null)
        {
            return false;
        }
        walkTargetItem = itemWindow;
        targetX = GetResolvedTargetX();
        return true;
    }

    public bool IsBlacklisted(SaveHandler.Kinks taggedKink)
    {
        return settingBlacklistedContent.Contains(taggedKink);
    }

    public bool IsBlacklisted(Array<SaveHandler.Kinks> taggedContent)
    {
        foreach (SaveHandler.Kinks item in taggedContent)
        {
            if (settingBlacklistedContent.Contains(item))
            {
                return true;
            }
        }
        return false;
    }

    public DialogueDataRes PickDialogue(IEnumerable<DialogueDataRes> pool)
    {
        WeightGroup<DialogueDataRes> weightGroup = new WeightGroup<DialogueDataRes>();
        foreach (DialogueDataRes item in pool)
        {
            if (Instance.IsBlacklisted(item.taggedKinks))
            {
                continue;
            }
            bool flag = false;
            foreach (TagDataRes requiredTag in item.RequiredTags)
            {
                TagDataRes tag = Instance.mainCharacter.GetTag(requiredTag.tagName);
                if (tag == null || (requiredTag.tagAmount != 0 && tag.tagAmount < requiredTag.tagAmount))
                {
                    flag = true;
                    break;
                }
            }
            if (!flag)
            {
                weightGroup.Add(item, item.dialogueAppeanceWeight);
            }
        }
        if (weightGroup.Count == 0)
        {
            return null;
        }
        return weightGroup.GetItem(GD.RandRange(0, 10000));
    }

    public ConvoDataRes PickConvo(IEnumerable<ConvoDataRes> pool, CharacterInfoDataRes subActorSpeaker = null)
    {
        WeightGroup<ConvoDataRes> weightGroup = new WeightGroup<ConvoDataRes>();
        foreach (ConvoDataRes item in pool)
        {
            if (Instance.IsBlacklisted(item.taggedKinks))
            {
                continue;
            }
            bool flag = false;
            foreach (TagDataRes requiredTag in item.RequiredTags)
            {
                TagDataRes tag = Instance.mainCharacter.GetTag(requiredTag.tagName);
                if (tag == null || (requiredTag.tagAmount != 0 && tag.tagAmount < requiredTag.tagAmount))
                {
                    flag = true;
                    break;
                }
            }
            if (flag)
            {
                continue;
            }
            bool flag2 = false;
            foreach (DialogueDataRes item2 in item.convoStack)
            {
                if (string.IsNullOrEmpty(item2.speakingActorID) || item2.speakingActorID == mainCharacter.characterInformation._itemID)
                {
                    continue;
                }
                bool flag3 = false;
                foreach (ActorWindow spawnedActor in spawnedActors)
                {
                    if (GodotObject.IsInstanceValid(spawnedActor) && spawnedActor.characterActor.characterInformation == ResourceCache.resourcesLoaded[ResourceCache.ResourceTyping.CHARACTER][item2.speakingActorID])
                    {
                        flag3 = true;
                        break;
                    }
                }
                if (!flag3)
                {
                    flag2 = true;
                    break;
                }
            }
            if (!flag2)
            {
                weightGroup.Add(item, item.Weight);
            }
        }
        if (weightGroup.Count == 0)
        {
            return null;
        }
        return weightGroup.GetItem(GD.RandRange(0, 10000));
    }

    private void FollowMouse(bool Unlocked = false)
    {
        Vector2I mousePos = MobileMousePos();
        if (_isMobile)
        {
            Vector2I screenSize = DisplayServer.ScreenGetSize(screenDataHandler.screenIndex);
            Vector2 newPos = (Vector2)mousePos + mouseOffset;
            newPos.X = Mathf.Clamp(newPos.X, 0f, (float)(screenSize.X - mainCharacter.trueSize.X));
            newPos.Y = Mathf.Clamp(newPos.Y, 0f, (float)(screenSize.Y - mainCharacter.trueSize.Y));
            Position = newPos;
            return;
        }
        if (!Unlocked)
        {
            mainWindow.Position = new Vector2I(screenDataHandler.ClampAcrossAllScreensX((int)((float)mousePos.X + mouseOffset.X), mainCharacter.trueSize.X), screenDataHandler.taskbarPos);
            return;
        }
        int x = screenDataHandler.ClampAcrossAllScreensX((int)((float)mousePos.X + mouseOffset.X), mainCharacter.trueSize.X);
        int verticalAdjacentTaskbarY = screenDataHandler.GetVerticalAdjacentTaskbarY(mousePos, mainCharacter.trueSize);
        int y = ((verticalAdjacentTaskbarY >= 0) ? verticalAdjacentTaskbarY : screenDataHandler.ClampAcrossAllScreensY((int)((float)mousePos.Y + mouseOffset.Y)));
        mainWindow.Position = new Vector2I(x, y);
    }

    private void MovePet()
    {
        if (Input.IsActionPressed("Move") && !SomethingHasBeenGrabbed && GetThinnerCollisionBox().HasPoint(MobileMousePos()))
        {
            selected = true;
            SomethingHasBeenGrabbed = true;
            mouseOffset = base.Position - (Vector2)MobileMousePos();
            AbortActivePickup();
            isWalking = false;
            mainCharacter.Grabbed(isGrabbed: true);
        }
        if (Input.IsActionJustReleased("Move") && selected)
        {
            selected = false;
            SomethingHasBeenGrabbed = false;
            isWalking = false;
            mainCharacter.Grabbed(isGrabbed: false);
            if (settingWindowThrowPhysics)
            {
                windowVelocity = mainCharacter.mouseVelocity * mouseVelocityScaler;
                windowVelocity.Y = Mathf.Clamp(windowVelocity.Y, -1600f, float.MaxValue);
                isThrown = true;
                mainCharacter.BeginThrow(windowVelocity);
            }
        }
    }

    public void TweenGrabRelease()
    {
        grabReleaseTween = GetTree().CreateTween();
        grabReleaseTween.TweenInterval(0.5);
        grabReleaseTween.TweenCallback(Callable.From(OnGrabReleased));
    }

    private void OnGrabReleased()
    {
        SomethingHasBeenGrabbed = false;
    }

    private int GetResolvedTargetX()
    {
        if (walkTargetItem != null && GodotObject.IsInstanceValid(walkTargetItem))
        {
            return Mathf.Clamp(walkTargetItem.Position.X + walkTargetItem.Size.X - mainCharacter.trueSize.X / 4, screenDataHandler.EffectiveLeftX, screenDataHandler.EffectiveRightX - mainCharacter.trueSize.X);
        }
        return targetX;
    }

    private void StartAutoWalkToTarget()
    {
        isWalkingToTargetPosition = true;
        isWalking = true;
        UpdateWalkDirection();
        mainCharacter.ForceMainBodyState(Character.MainBodyStates.Walk, "Walk");
        mainCharacter.mainBodyTimer.WaitTime = 1000.0;
        mainCharacter.mainBodyTimer.Start();
    }

    private void UpdateWalkDirection()
    {
        int num = ((GetResolvedTargetX() > mainWindow.Position.X) ? 1 : (-1));
        if (num != walkDirection)
        {
            walkDirection = num;
            mainCharacter.FlipHSpriteTo(walkDirection > 0);
        }
    }

    private void Walk(double delta)
    {
        Vector2I position = (_isMobile) ? (Vector2I)Position : mainWindow.Position;
        if (isWalkingToTargetPosition)
        {
            if (walkTargetItem != null)
            {
                if (!GodotObject.IsInstanceValid(walkTargetItem))
                {
                    ClearItemTarget();
                    return;
                }
                UpdateWalkDirection();
            }
            int resolvedTargetX = GetResolvedTargetX();
            int num = (int)((double)position.X + 250.0 * delta * (double)walkDirection);
            if ((walkDirection > 0) ? (num >= resolvedTargetX) : (num <= resolvedTargetX))
            {
                num = resolvedTargetX;
                ReachedItemTarget();
            }
            position.X = num;
            if (_isMobile) Position = new Vector2(position.X, Position.Y);
            else mainWindow.Position = new Vector2I(screenDataHandler.ClampAcrossAllScreensX(position.X, mainCharacter.trueSize.X), screenDataHandler.ClampAcrossAllScreensY(position.Y));
        }
        else
        {
            int effectiveLeftX = screenDataHandler.EffectiveLeftX;
            int num2 = screenDataHandler.EffectiveRightX - mainCharacter.trueSize.X;
            int num3 = (int)((double)position.X + 250.0 * delta * (double)walkDirection);
            if (num3 < effectiveLeftX)
            {
                num3 = effectiveLeftX;
                walkDirection = 1;
                mainCharacter.FlipHSpriteTo(FlipH: true);
            }
            else if (num3 > num2)
            {
                num3 = num2;
                walkDirection = -1;
                mainCharacter.FlipHSpriteTo(FlipH: false);
            }
            position.X = num3;
            if (_isMobile) Position = new Vector2(position.X, Position.Y);
            else mainWindow.Position = new Vector2I(screenDataHandler.ClampAcrossAllScreensX(position.X, mainCharacter.trueSize.X), screenDataHandler.ClampAcrossAllScreensY(position.Y));
        }
    }

    private void ReachedItemTarget()
    {
        if (inPickup)
        {
            return;
        }
        if (walkTargetItem == null || !GodotObject.IsInstanceValid(walkTargetItem) || walkTargetItem.CurrentlyPickedUp)
        {
            passivePlayTimer.WaitTime = GD.RandRange(passivePlayTimerOnRange.X, passivePlayTimerOnRange.Y);
            passivePlayTimer.Start();
            ClearItemTarget();
            return;
        }
        if (settingPassivePlayMode)
        {
            if (storedItem != null && GodotObject.IsInstanceValid(storedItem))
            {
                ItemWindow itemA = storedItem;
                ItemWindow itemB = walkTargetItem;
                Vector2I spawnPos = new Vector2I(mainWindow.Position.X + mainCharacter.trueSize.X / 2, mainWindow.Position.Y + mainCharacter.trueSize.Y / 2);
                CombinationDataRes combinationDataRes = null;
                foreach (CombinationDataRes possibleCombination in itemA.itemObject.itemInformation.possibleCombinations)
                {
                    if (possibleCombination.requiredItem == itemB.itemObject.itemInformation && possibleCombination.outputItem != null && !possibleCombination.outputItem.NoPassivePickup)
                    {
                        combinationDataRes = possibleCombination;
                        break;
                    }
                }
                if (combinationDataRes == null)
                {
                    foreach (CombinationDataRes possibleCombination2 in itemB.itemObject.itemInformation.possibleCombinations)
                    {
                        if (possibleCombination2.requiredItem == itemA.itemObject.itemInformation && possibleCombination2.outputItem != null && !possibleCombination2.outputItem.NoPassivePickup)
                        {
                            combinationDataRes = possibleCombination2;
                            break;
                        }
                    }
                }
                if (combinationDataRes != null)
                {
                    ItemDataRes outputItem = combinationDataRes.outputItem;
                    inPickup = true;
                    itemB.MousePassthrough = true;
                    mainCharacter.ForceMainBodyStateQueue(new Character.MainBodyAnimationStack[2]
                    {
                        new Character.MainBodyAnimationStack(Character.MainBodyStates.Forced_Animation, "Pickup", reversed: false, delegate
                        {
                            if (!GodotObject.IsInstanceValid(itemB) || !mainCharacter.Visible)
                            {
                                ReleaseItem(itemB);
                                ClearItemTarget();
                            }
                            else
                            {
                                itemB.CurrentlyPickedUp = true;
                            }
                        }),
                        new Character.MainBodyAnimationStack(Character.MainBodyStates.Forced_Animation, "Pickup", reversed: true, delegate
                        {
                            if (!GodotObject.IsInstanceValid(itemB) || !mainCharacter.Visible)
                            {
                                storedItem.CurrentlyPickedUp = false;
                                storedItem.MousePassthrough = false;
                                storedItem.Position = spawnPos;
                                storedItem = null;
                                ReleaseItem(itemB);
                                ClearItemTarget();
                            }
                            else
                            {
                                spawnedItems.Remove(itemA);
                                itemA.QueueFree();
                                spawnedItems.Remove(itemB);
                                itemB.QueueFree();
                                storedItem = null;
                                CallItemSpawn(outputItem, spawnPos);
                                ItemWindow itemWindow = spawnedItems[spawnedItems.Count - 1];
                                itemWindow.UsePickedUpItem();
                                if (!itemWindow.itemObject.itemInformation.isReusable)
                                {
                                    spawnedItems.Remove(itemWindow);
                                    itemWindow.QueueFree();
                                }
                                ClearItemTarget(usingItem: true);
                            }
                        })
                    });
                    return;
                }
                storedItem.CurrentlyPickedUp = false;
                storedItem.Position = spawnPos;
                storedItem = null;
                inPickup = true;
                itemB.MousePassthrough = true;
                mainCharacter.ForceMainBodyStateQueue(new Character.MainBodyAnimationStack[2]
                {
                    new Character.MainBodyAnimationStack(Character.MainBodyStates.Forced_Animation, "Pickup"),
                    new Character.MainBodyAnimationStack(Character.MainBodyStates.Forced_Animation, "Pickup", reversed: true, delegate
                    {
                        if (GodotObject.IsInstanceValid(itemB))
                        {
                            storedItem = itemB;
                            storedItem.CurrentlyPickedUp = true;
                        }
                        ClearItemTarget();
                    })
                });
                return;
            }
            ItemWindow capturedTarget2 = walkTargetItem;
            Array<ItemWindow> possibleCombinations = new Array<ItemWindow>();
            foreach (ItemWindow other in spawnedItems)
            {
                if (other != capturedTarget2 && (capturedTarget2.itemObject.itemInformation.possibleCombinations.Any((CombinationDataRes combo) => combo.outputItem != null && !combo.outputItem.NoPassivePickup && combo.requiredItem == other.itemObject.itemInformation) || other.itemObject.itemInformation.possibleCombinations.Any((CombinationDataRes combo) => combo.outputItem != null && !combo.outputItem.NoPassivePickup && combo.requiredItem == capturedTarget2.itemObject.itemInformation)))
                {
                    possibleCombinations.Add(other);
                }
            }
            inPickup = true;
            capturedTarget2.MousePassthrough = true;
            mainCharacter.ForceMainBodyStateQueue(new Character.MainBodyAnimationStack[2]
            {
                new Character.MainBodyAnimationStack(Character.MainBodyStates.Forced_Animation, "Pickup", reversed: false, delegate
                {
                    if (!GodotObject.IsInstanceValid(capturedTarget2) || !mainCharacter.Visible)
                    {
                        ReleaseItem(capturedTarget2);
                        ClearItemTarget();
                    }
                    else
                    {
                        capturedTarget2.CurrentlyPickedUp = true;
                    }
                }),
                new Character.MainBodyAnimationStack(Character.MainBodyStates.Forced_Animation, "Pickup", reversed: true, delegate
                {
                    if (!GodotObject.IsInstanceValid(capturedTarget2) || !mainCharacter.Visible)
                    {
                        ClearItemTarget();
                    }
                    else if (possibleCombinations.Count() > 0)
                    {
                        storedItem = capturedTarget2;
                        storedItem.CurrentlyPickedUp = true;
                        isWalking = false;
                        isWalkingToTargetPosition = false;
                        walkTargetItem = null;
                        TrySetTargetToItem(ItemTargetMode.RANDOM, possibleCombinations);
                        StartAutoWalkToTarget();
                        inPickup = false;
                    }
                    else if (!GodotObject.IsInstanceValid(capturedTarget2) || !mainCharacter.Visible)
                    {
                        ClearItemTarget();
                    }
                    else
                    {
                        capturedTarget2.UsePickedUpItem();
                        if (!capturedTarget2.itemObject.itemInformation.isReusable)
                        {
                            spawnedItems.Remove(capturedTarget2);
                            capturedTarget2.QueueFree();
                        }
                        else
                        {
                            ReleaseItem(capturedTarget2);
                        }
                        ClearItemTarget(usingItem: true);
                    }
                })
            });
            return;
        }
        ItemWindow capturedTarget = walkTargetItem;
        inPickup = true;
        capturedTarget.MousePassthrough = true;
        mainCharacter.ForceMainBodyStateQueue(new Character.MainBodyAnimationStack[2]
        {
            new Character.MainBodyAnimationStack(Character.MainBodyStates.Forced_Animation, "Pickup", reversed: false, delegate
            {
                if (!GodotObject.IsInstanceValid(capturedTarget) || !mainCharacter.Visible)
                {
                    ClearItemTarget();
                }
                else
                {
                    capturedTarget.CurrentlyPickedUp = true;
                }
            }),
            new Character.MainBodyAnimationStack(Character.MainBodyStates.Forced_Animation, "Pickup", reversed: true, delegate
            {
                if (!GodotObject.IsInstanceValid(capturedTarget) || !mainCharacter.Visible)
                {
                    ClearItemTarget();
                }
                else
                {
                    capturedTarget.UsePickedUpItem();
                    if (!capturedTarget.itemObject.itemInformation.isReusable)
                    {
                        spawnedItems.Remove(capturedTarget);
                        capturedTarget.QueueFree();
                    }
                    ClearItemTarget(usingItem: true);
                }
            })
        });
    }

    public void AbortActivePickup()
    {
        if (inPickup || walkTargetItem != null || storedItem != null)
        {
            if (walkTargetItem != null && GodotObject.IsInstanceValid(walkTargetItem))
            {
                walkTargetItem.CurrentlyPickedUp = false;
            }
            if (storedItem != null && GodotObject.IsInstanceValid(storedItem))
            {
                storedItem.CurrentlyPickedUp = false;
            }
            inPickup = false;
            isWalking = false;
            isWalkingToTargetPosition = false;
            walkTargetItem = null;
            storedItem = null;
        }
    }

    private void ReleaseItem(ItemWindow item)
    {
        if (item != null && GodotObject.IsInstanceValid(item))
        {
            item.CurrentlyPickedUp = false;
            item.MousePassthrough = false;
        }
    }

    public void ClearItemTarget(bool usingItem = false)
    {
        isWalking = false;
        isWalkingToTargetPosition = false;
        walkTargetItem = null;
        inPickup = false;
        if (!usingItem)
        {
            mainCharacter.ForceMainBodyState(Character.MainBodyStates.Idle, "Idle");
            mainCharacter.mainBodyTimer.WaitTime = 1.0;
            mainCharacter.mainBodyTimer.Start();
        }
    }

    private void ChooseDirection()
    {
        if (GD.RandRange(1, 2) == 1)
        {
            walkDirection = 1;
            mainCharacter.FlipHSpriteTo(FlipH: true);
        }
        else
        {
            walkDirection = -1;
            mainCharacter.FlipHSpriteTo(FlipH: false);
        }
    }

    private void OnCharacterWalking()
    {
        isWalking = true;
        ChooseDirection();
    }

    private void OnCharacterFinishedWalking()
    {
        isWalking = false;
    }

        public Rect2I GetThinnerCollisionBox()
    {
        float num = 0.33f;
        if (_isMobile)
        {
            int w = mainCharacter.trueSize.X;
            int x = Mathf.RoundToInt(Position.X);
            int y = Mathf.RoundToInt(Position.Y);
            return new Rect2I(new Vector2I(x, y), new Vector2I(w, mainCharacter.trueSize.Y));
        }
        int num2 = Mathf.RoundToInt((float)mainWindow.Size.X * num);
        int num3 = Mathf.RoundToInt((float)(mainWindow.Size.X - num2) / 2f);
        return new Rect2I(new Vector2I(mainWindow.Position.X + num3, mainWindow.Position.Y), new Vector2I(num2, mainWindow.Size.Y));
    }

    private void CheckLandingInteraction(bool hardLanding = false)
    {
        Rect2I collisionBox = GetThinnerCollisionBox();
        int centerX = collisionBox.Position.X + collisionBox.Size.X / 2;
        List<ActorWindow> source = (from a in spawnedCompanions
            where GodotObject.IsInstanceValid(a) && IsActorLogicallyVisible(a) && !a.inUse && !a.inUseByAttachment
            orderby Mathf.Abs(a.Position.X + a.Size.X / 2 - centerX)
            select a).ToList();
        if (hardLanding)
        {
            ActorWindow actorWindow = source.FirstOrDefault((ActorWindow a) => a.characterActor.characterInformation.isAggroActor);
            if (actorWindow != null)
            {
                actorWindow.OnAggroTimerTimeout();
                return;
            }
        }
        ActorWindow actorWindow2 = source.FirstOrDefault((ActorWindow a) => collisionBox.Intersects(new Rect2I(a.Position, a.Size)));
        if (actorWindow2 != null)
        {
            WeightGroup<AttachDataRes> weightGroup = actorWindow2.BuildWeightedAttachments(actorWindow2.characterActor.characterInformation.overrideAnimation);
            if (weightGroup.Count() > 0)
            {
                ClearAllAttachments();
                ForceDropCharacter();
                CallCharacterAttachmentSpawn(weightGroup.GetItem(GD.RandRange(0, 10000)));
                actorWindow2.inUse = true;
            }
            return;
        }
        ItemWindow itemWindow = null;
        int num = int.MaxValue;
        foreach (ItemWindow spawnedItem in spawnedItems)
        {
            if (!GodotObject.IsInstanceValid(spawnedItem) || !IsItemLogicallyVisible(spawnedItem) || spawnedItem.CurrentlyPickedUp || !spawnedItem.itemObject.itemInformation.isUsableDroppedOn)
            {
                continue;
            }
            Rect2I b = new Rect2I(spawnedItem.Position, spawnedItem.Size);
            if (collisionBox.Intersects(b))
            {
                int num2 = Mathf.Abs(b.Position.X + b.Size.X / 2 - centerX);
                if (num2 < num)
                {
                    num = num2;
                    itemWindow = spawnedItem;
                }
            }
        }
        itemWindow?.UsePickedUpItem();
    }



















    




}
