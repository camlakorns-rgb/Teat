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
public class AttachObjWindow : Window
{
	public new class MethodName : Window.MethodName
	{
		public new static readonly StringName _Ready = "_Ready";

		public new static readonly StringName _Process = "_Process";

		public static readonly StringName SetupAttachmentWindow = "SetupAttachmentWindow";

		public static readonly StringName DelayedSetupFlag = "DelayedSetupFlag";

		public static readonly StringName PlaceTags = "PlaceTags";

		public static readonly StringName FollowParent = "FollowParent";

		public static readonly StringName LayerOntoParent = "LayerOntoParent";

		public static readonly StringName HandleForcedActorMovement = "HandleForcedActorMovement";

		public static readonly StringName StopForcedMovement = "StopForcedMovement";

		public static readonly StringName RepositionInstigatingActor = "RepositionInstigatingActor";

		public static readonly StringName InitializeBounce = "InitializeBounce";

		public static readonly StringName BounceWindow = "BounceWindow";

		public static readonly StringName RandomObjectPosition = "RandomObjectPosition";

		public static readonly StringName RandomObjectClicked = "RandomObjectClicked";

		public static readonly StringName PlayDialogueOnFrame = "PlayDialogueOnFrame";

		public static readonly StringName IsSpeakingActorPresent = "IsSpeakingActorPresent";

		public static readonly StringName HasRequiredTags = "HasRequiredTags";

		public static readonly StringName OnAnimationTimeout = "OnAnimationTimeout";

		public static readonly StringName OnTextFinish = "OnTextFinish";

		public static readonly StringName KillText = "KillText";

		public static readonly StringName SpawnItemOnClose = "SpawnItemOnClose";
	}

	public new class PropertyName : Window.PropertyName
	{
		public static readonly StringName mainBody = "mainBody";

		public static readonly StringName attachObject = "attachObject";

		public static readonly StringName FadeKill = "FadeKill";

		public static readonly StringName AttachmentStackSpacing = "AttachmentStackSpacing";

		public static readonly StringName SpawnOnLoad = "SpawnOnLoad";

		public static readonly StringName DEBUG_dialogueData = "DEBUG_dialogueData";

		public static readonly StringName parentWindow = "parentWindow";

		public static readonly StringName _bounceSpeed = "_bounceSpeed";

		public static readonly StringName _bounceAmplitude = "_bounceAmplitude";

		public static readonly StringName _bounceTime = "_bounceTime";

		public static readonly StringName _bounceBaseY = "_bounceBaseY";

		public static readonly StringName _bounceInitialized = "_bounceInitialized";

		public static readonly StringName _maxSpan = "_maxSpan";

		public static readonly StringName _maxSpamLifeSpanInPassivePlay = "_maxSpamLifeSpanInPassivePlay";

		public static readonly StringName _isSetup = "_isSetup";

		public static readonly StringName _forcedMovementActive = "_forcedMovementActive";

		public static readonly StringName _forcedMovementWalking = "_forcedMovementWalking";

		public static readonly StringName _forcedMovementFinalIdle = "_forcedMovementFinalIdle";

		public static readonly StringName _forcedMoveDirection = "_forcedMoveDirection";

		public static readonly StringName _forcedMoveStateTimer = "_forcedMoveStateTimer";

		public static readonly StringName _forcedMovementElapsed = "_forcedMovementElapsed";

		public static readonly StringName _forcedMovementDuration = "_forcedMovementDuration";

		public static readonly StringName _forcedMovementX = "_forcedMovementX";
	}

	public new class SignalName : Window.SignalName
	{
	}

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
				if (attachObject.attachedItemInformation.popupURL != "")
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
			break;
		case AttachDataRes.AttachmentType.OVERRIDE:
			attachObject.SetupImageAttachment();
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
			base.MousePassthrough = false;
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
					_forcedMovementX = parentWindow.Position.X;
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
		Vector2I position = parentWindow.Position;
		Vector2I size = parentWindow.Size;
		Vector2I trueSize = attachObject.trueSize;
		int num = position.X + size.X / 2 - trueSize.X / 2 + (int)attachmentMargin.X;
		int num2 = position.Y + size.Y / 2 - trueSize.Y / 2 + (int)attachmentMargin.Y;
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
				int x = parentWindow.Size.X;
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
				parentWindow.Position = new Vector2I(num2, parentWindow.Position.Y);
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

	[EditorBrowsable(EditorBrowsableState.Never)]
	internal static List<MethodInfo> GetGodotMethodList()
	{
		return new List<MethodInfo>(21)
		{
			new MethodInfo(MethodName._Ready, new PropertyInfo(Variant.Type.Nil, "", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false), MethodFlags.Normal, null, null),
			new MethodInfo(MethodName._Process, new PropertyInfo(Variant.Type.Nil, "", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false), MethodFlags.Normal, new List<PropertyInfo>
			{
				new PropertyInfo(Variant.Type.Float, "delta", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false)
			}, null),
			new MethodInfo(MethodName.SetupAttachmentWindow, new PropertyInfo(Variant.Type.Nil, "", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false), MethodFlags.Normal, new List<PropertyInfo>
			{
				new PropertyInfo(Variant.Type.Object, "passedText", PropertyHint.None, "", PropertyUsageFlags.Default, new StringName("Resource"), exported: false)
			}, null),
			new MethodInfo(MethodName.DelayedSetupFlag, new PropertyInfo(Variant.Type.Nil, "", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false), MethodFlags.Normal, null, null),
			new MethodInfo(MethodName.PlaceTags, new PropertyInfo(Variant.Type.Nil, "", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false), MethodFlags.Normal, null, null),
			new MethodInfo(MethodName.FollowParent, new PropertyInfo(Variant.Type.Nil, "", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false), MethodFlags.Normal, null, null),
			new MethodInfo(MethodName.LayerOntoParent, new PropertyInfo(Variant.Type.Nil, "", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false), MethodFlags.Normal, null, null),
			new MethodInfo(MethodName.HandleForcedActorMovement, new PropertyInfo(Variant.Type.Nil, "", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false), MethodFlags.Normal, new List<PropertyInfo>
			{
				new PropertyInfo(Variant.Type.Float, "delta", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false)
			}, null),
			new MethodInfo(MethodName.StopForcedMovement, new PropertyInfo(Variant.Type.Nil, "", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false), MethodFlags.Normal, null, null),
			new MethodInfo(MethodName.RepositionInstigatingActor, new PropertyInfo(Variant.Type.Nil, "", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false), MethodFlags.Normal, null, null),
			new MethodInfo(MethodName.InitializeBounce, new PropertyInfo(Variant.Type.Nil, "", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false), MethodFlags.Normal, null, null),
			new MethodInfo(MethodName.BounceWindow, new PropertyInfo(Variant.Type.Nil, "", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false), MethodFlags.Normal, new List<PropertyInfo>
			{
				new PropertyInfo(Variant.Type.Float, "delta", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false)
			}, null),
			new MethodInfo(MethodName.RandomObjectPosition, new PropertyInfo(Variant.Type.Nil, "", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false), MethodFlags.Normal, null, null),
			new MethodInfo(MethodName.RandomObjectClicked, new PropertyInfo(Variant.Type.Nil, "", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false), MethodFlags.Normal, null, null),
			new MethodInfo(MethodName.PlayDialogueOnFrame, new PropertyInfo(Variant.Type.Nil, "", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false), MethodFlags.Normal, null, null),
			new MethodInfo(MethodName.IsSpeakingActorPresent, new PropertyInfo(Variant.Type.Bool, "", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false), MethodFlags.Normal, new List<PropertyInfo>
			{
				new PropertyInfo(Variant.Type.Object, "dialogue", PropertyHint.None, "", PropertyUsageFlags.Default, new StringName("Resource"), exported: false)
			}, null),
			new MethodInfo(MethodName.HasRequiredTags, new PropertyInfo(Variant.Type.Bool, "", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false), MethodFlags.Normal, new List<PropertyInfo>
			{
				new PropertyInfo(Variant.Type.Object, "dialogue", PropertyHint.None, "", PropertyUsageFlags.Default, new StringName("Resource"), exported: false)
			}, null),
			new MethodInfo(MethodName.OnAnimationTimeout, new PropertyInfo(Variant.Type.Nil, "", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false), MethodFlags.Normal, null, null),
			new MethodInfo(MethodName.OnTextFinish, new PropertyInfo(Variant.Type.Nil, "", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false), MethodFlags.Normal, null, null),
			new MethodInfo(MethodName.KillText, new PropertyInfo(Variant.Type.Nil, "", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false), MethodFlags.Normal, new List<PropertyInfo>
			{
				new PropertyInfo(Variant.Type.StringName, "stringName", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false)
			}, null),
			new MethodInfo(MethodName.SpawnItemOnClose, new PropertyInfo(Variant.Type.Nil, "", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false), MethodFlags.Normal, null, null)
		};
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override bool InvokeGodotClassMethod(in godot_string_name method, NativeVariantPtrArgs args, out godot_variant ret)
	{
		if (method == MethodName._Ready && args.Count == 0)
		{
			_Ready();
			ret = default(godot_variant);
			return true;
		}
		if (method == MethodName._Process && args.Count == 1)
		{
			_Process(VariantUtils.ConvertTo<double>(in args[0]));
			ret = default(godot_variant);
			return true;
		}
		if (method == MethodName.SetupAttachmentWindow && args.Count == 1)
		{
			SetupAttachmentWindow(VariantUtils.ConvertTo<DialogueDataRes>(in args[0]));
			ret = default(godot_variant);
			return true;
		}
		if (method == MethodName.DelayedSetupFlag && args.Count == 0)
		{
			DelayedSetupFlag();
			ret = default(godot_variant);
			return true;
		}
		if (method == MethodName.PlaceTags && args.Count == 0)
		{
			PlaceTags();
			ret = default(godot_variant);
			return true;
		}
		if (method == MethodName.FollowParent && args.Count == 0)
		{
			FollowParent();
			ret = default(godot_variant);
			return true;
		}
		if (method == MethodName.LayerOntoParent && args.Count == 0)
		{
			LayerOntoParent();
			ret = default(godot_variant);
			return true;
		}
		if (method == MethodName.HandleForcedActorMovement && args.Count == 1)
		{
			HandleForcedActorMovement(VariantUtils.ConvertTo<double>(in args[0]));
			ret = default(godot_variant);
			return true;
		}
		if (method == MethodName.StopForcedMovement && args.Count == 0)
		{
			StopForcedMovement();
			ret = default(godot_variant);
			return true;
		}
		if (method == MethodName.RepositionInstigatingActor && args.Count == 0)
		{
			RepositionInstigatingActor();
			ret = default(godot_variant);
			return true;
		}
		if (method == MethodName.InitializeBounce && args.Count == 0)
		{
			InitializeBounce();
			ret = default(godot_variant);
			return true;
		}
		if (method == MethodName.BounceWindow && args.Count == 1)
		{
			BounceWindow(VariantUtils.ConvertTo<double>(in args[0]));
			ret = default(godot_variant);
			return true;
		}
		if (method == MethodName.RandomObjectPosition && args.Count == 0)
		{
			RandomObjectPosition();
			ret = default(godot_variant);
			return true;
		}
		if (method == MethodName.RandomObjectClicked && args.Count == 0)
		{
			RandomObjectClicked();
			ret = default(godot_variant);
			return true;
		}
		if (method == MethodName.PlayDialogueOnFrame && args.Count == 0)
		{
			PlayDialogueOnFrame();
			ret = default(godot_variant);
			return true;
		}
		if (method == MethodName.IsSpeakingActorPresent && args.Count == 1)
		{
			bool from = IsSpeakingActorPresent(VariantUtils.ConvertTo<DialogueDataRes>(in args[0]));
			ret = VariantUtils.CreateFrom(in from);
			return true;
		}
		if (method == MethodName.HasRequiredTags && args.Count == 1)
		{
			bool from2 = HasRequiredTags(VariantUtils.ConvertTo<DialogueDataRes>(in args[0]));
			ret = VariantUtils.CreateFrom(in from2);
			return true;
		}
		if (method == MethodName.OnAnimationTimeout && args.Count == 0)
		{
			OnAnimationTimeout();
			ret = default(godot_variant);
			return true;
		}
		if (method == MethodName.OnTextFinish && args.Count == 0)
		{
			OnTextFinish();
			ret = default(godot_variant);
			return true;
		}
		if (method == MethodName.KillText && args.Count == 1)
		{
			KillText(VariantUtils.ConvertTo<StringName>(in args[0]));
			ret = default(godot_variant);
			return true;
		}
		if (method == MethodName.SpawnItemOnClose && args.Count == 0)
		{
			SpawnItemOnClose();
			ret = default(godot_variant);
			return true;
		}
		return base.InvokeGodotClassMethod(in method, args, out ret);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override bool HasGodotClassMethod(in godot_string_name method)
	{
		if (method == MethodName._Ready)
		{
			return true;
		}
		if (method == MethodName._Process)
		{
			return true;
		}
		if (method == MethodName.SetupAttachmentWindow)
		{
			return true;
		}
		if (method == MethodName.DelayedSetupFlag)
		{
			return true;
		}
		if (method == MethodName.PlaceTags)
		{
			return true;
		}
		if (method == MethodName.FollowParent)
		{
			return true;
		}
		if (method == MethodName.LayerOntoParent)
		{
			return true;
		}
		if (method == MethodName.HandleForcedActorMovement)
		{
			return true;
		}
		if (method == MethodName.StopForcedMovement)
		{
			return true;
		}
		if (method == MethodName.RepositionInstigatingActor)
		{
			return true;
		}
		if (method == MethodName.InitializeBounce)
		{
			return true;
		}
		if (method == MethodName.BounceWindow)
		{
			return true;
		}
		if (method == MethodName.RandomObjectPosition)
		{
			return true;
		}
		if (method == MethodName.RandomObjectClicked)
		{
			return true;
		}
		if (method == MethodName.PlayDialogueOnFrame)
		{
			return true;
		}
		if (method == MethodName.IsSpeakingActorPresent)
		{
			return true;
		}
		if (method == MethodName.HasRequiredTags)
		{
			return true;
		}
		if (method == MethodName.OnAnimationTimeout)
		{
			return true;
		}
		if (method == MethodName.OnTextFinish)
		{
			return true;
		}
		if (method == MethodName.KillText)
		{
			return true;
		}
		if (method == MethodName.SpawnItemOnClose)
		{
			return true;
		}
		return base.HasGodotClassMethod(in method);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override bool SetGodotClassPropertyValue(in godot_string_name name, in godot_variant value)
	{
		if (name == PropertyName.mainBody)
		{
			mainBody = VariantUtils.ConvertTo<AnimatedSprite2D>(in value);
			return true;
		}
		if (name == PropertyName.attachObject)
		{
			attachObject = VariantUtils.ConvertTo<AttachmentObject>(in value);
			return true;
		}
		if (name == PropertyName.FadeKill)
		{
			FadeKill = VariantUtils.ConvertTo<AnimationPlayer>(in value);
			return true;
		}
		if (name == PropertyName.AttachmentStackSpacing)
		{
			AttachmentStackSpacing = VariantUtils.ConvertTo<int>(in value);
			return true;
		}
		if (name == PropertyName.SpawnOnLoad)
		{
			SpawnOnLoad = VariantUtils.ConvertTo<bool>(in value);
			return true;
		}
		if (name == PropertyName.DEBUG_dialogueData)
		{
			DEBUG_dialogueData = VariantUtils.ConvertTo<DialogueDataRes>(in value);
			return true;
		}
		if (name == PropertyName.parentWindow)
		{
			parentWindow = VariantUtils.ConvertTo<Window>(in value);
			return true;
		}
		if (name == PropertyName._bounceSpeed)
		{
			_bounceSpeed = VariantUtils.ConvertTo<float>(in value);
			return true;
		}
		if (name == PropertyName._bounceAmplitude)
		{
			_bounceAmplitude = VariantUtils.ConvertTo<float>(in value);
			return true;
		}
		if (name == PropertyName._bounceTime)
		{
			_bounceTime = VariantUtils.ConvertTo<float>(in value);
			return true;
		}
		if (name == PropertyName._bounceBaseY)
		{
			_bounceBaseY = VariantUtils.ConvertTo<int>(in value);
			return true;
		}
		if (name == PropertyName._bounceInitialized)
		{
			_bounceInitialized = VariantUtils.ConvertTo<bool>(in value);
			return true;
		}
		if (name == PropertyName._maxSpan)
		{
			_maxSpan = VariantUtils.ConvertTo<double>(in value);
			return true;
		}
		if (name == PropertyName._maxSpamLifeSpanInPassivePlay)
		{
			_maxSpamLifeSpanInPassivePlay = VariantUtils.ConvertTo<Vector2>(in value);
			return true;
		}
		if (name == PropertyName._isSetup)
		{
			_isSetup = VariantUtils.ConvertTo<bool>(in value);
			return true;
		}
		if (name == PropertyName._forcedMovementActive)
		{
			_forcedMovementActive = VariantUtils.ConvertTo<bool>(in value);
			return true;
		}
		if (name == PropertyName._forcedMovementWalking)
		{
			_forcedMovementWalking = VariantUtils.ConvertTo<bool>(in value);
			return true;
		}
		if (name == PropertyName._forcedMovementFinalIdle)
		{
			_forcedMovementFinalIdle = VariantUtils.ConvertTo<bool>(in value);
			return true;
		}
		if (name == PropertyName._forcedMoveDirection)
		{
			_forcedMoveDirection = VariantUtils.ConvertTo<float>(in value);
			return true;
		}
		if (name == PropertyName._forcedMoveStateTimer)
		{
			_forcedMoveStateTimer = VariantUtils.ConvertTo<double>(in value);
			return true;
		}
		if (name == PropertyName._forcedMovementElapsed)
		{
			_forcedMovementElapsed = VariantUtils.ConvertTo<double>(in value);
			return true;
		}
		if (name == PropertyName._forcedMovementDuration)
		{
			_forcedMovementDuration = VariantUtils.ConvertTo<float>(in value);
			return true;
		}
		if (name == PropertyName._forcedMovementX)
		{
			_forcedMovementX = VariantUtils.ConvertTo<float>(in value);
			return true;
		}
		return base.SetGodotClassPropertyValue(in name, in value);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override bool GetGodotClassPropertyValue(in godot_string_name name, out godot_variant value)
	{
		if (name == PropertyName.mainBody)
		{
			value = VariantUtils.CreateFrom(in mainBody);
			return true;
		}
		if (name == PropertyName.attachObject)
		{
			value = VariantUtils.CreateFrom(in attachObject);
			return true;
		}
		if (name == PropertyName.FadeKill)
		{
			value = VariantUtils.CreateFrom(in FadeKill);
			return true;
		}
		if (name == PropertyName.AttachmentStackSpacing)
		{
			value = VariantUtils.CreateFrom(in AttachmentStackSpacing);
			return true;
		}
		if (name == PropertyName.SpawnOnLoad)
		{
			value = VariantUtils.CreateFrom(in SpawnOnLoad);
			return true;
		}
		if (name == PropertyName.DEBUG_dialogueData)
		{
			value = VariantUtils.CreateFrom(in DEBUG_dialogueData);
			return true;
		}
		if (name == PropertyName.parentWindow)
		{
			value = VariantUtils.CreateFrom(in parentWindow);
			return true;
		}
		if (name == PropertyName._bounceSpeed)
		{
			value = VariantUtils.CreateFrom(in _bounceSpeed);
			return true;
		}
		if (name == PropertyName._bounceAmplitude)
		{
			value = VariantUtils.CreateFrom(in _bounceAmplitude);
			return true;
		}
		if (name == PropertyName._bounceTime)
		{
			value = VariantUtils.CreateFrom(in _bounceTime);
			return true;
		}
		if (name == PropertyName._bounceBaseY)
		{
			value = VariantUtils.CreateFrom(in _bounceBaseY);
			return true;
		}
		if (name == PropertyName._bounceInitialized)
		{
			value = VariantUtils.CreateFrom(in _bounceInitialized);
			return true;
		}
		if (name == PropertyName._maxSpan)
		{
			value = VariantUtils.CreateFrom(in _maxSpan);
			return true;
		}
		if (name == PropertyName._maxSpamLifeSpanInPassivePlay)
		{
			value = VariantUtils.CreateFrom(in _maxSpamLifeSpanInPassivePlay);
			return true;
		}
		if (name == PropertyName._isSetup)
		{
			value = VariantUtils.CreateFrom(in _isSetup);
			return true;
		}
		if (name == PropertyName._forcedMovementActive)
		{
			value = VariantUtils.CreateFrom(in _forcedMovementActive);
			return true;
		}
		if (name == PropertyName._forcedMovementWalking)
		{
			value = VariantUtils.CreateFrom(in _forcedMovementWalking);
			return true;
		}
		if (name == PropertyName._forcedMovementFinalIdle)
		{
			value = VariantUtils.CreateFrom(in _forcedMovementFinalIdle);
			return true;
		}
		if (name == PropertyName._forcedMoveDirection)
		{
			value = VariantUtils.CreateFrom(in _forcedMoveDirection);
			return true;
		}
		if (name == PropertyName._forcedMoveStateTimer)
		{
			value = VariantUtils.CreateFrom(in _forcedMoveStateTimer);
			return true;
		}
		if (name == PropertyName._forcedMovementElapsed)
		{
			value = VariantUtils.CreateFrom(in _forcedMovementElapsed);
			return true;
		}
		if (name == PropertyName._forcedMovementDuration)
		{
			value = VariantUtils.CreateFrom(in _forcedMovementDuration);
			return true;
		}
		if (name == PropertyName._forcedMovementX)
		{
			value = VariantUtils.CreateFrom(in _forcedMovementX);
			return true;
		}
		return base.GetGodotClassPropertyValue(in name, out value);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	internal static List<PropertyInfo> GetGodotPropertyList()
	{
		return new List<PropertyInfo>
		{
			new PropertyInfo(Variant.Type.Object, PropertyName.mainBody, PropertyHint.NodeType, "AnimatedSprite2D", PropertyUsageFlags.Default | PropertyUsageFlags.ScriptVariable, exported: true),
			new PropertyInfo(Variant.Type.Object, PropertyName.attachObject, PropertyHint.NodeType, "Node2D", PropertyUsageFlags.Default | PropertyUsageFlags.ScriptVariable, exported: true),
			new PropertyInfo(Variant.Type.Object, PropertyName.FadeKill, PropertyHint.NodeType, "AnimationPlayer", PropertyUsageFlags.Default | PropertyUsageFlags.ScriptVariable, exported: true),
			new PropertyInfo(Variant.Type.Int, PropertyName.AttachmentStackSpacing, PropertyHint.None, "", PropertyUsageFlags.Default | PropertyUsageFlags.ScriptVariable, exported: true),
			new PropertyInfo(Variant.Type.Nil, "Debug", PropertyHint.None, "", PropertyUsageFlags.Group, exported: true),
			new PropertyInfo(Variant.Type.Bool, PropertyName.SpawnOnLoad, PropertyHint.None, "", PropertyUsageFlags.Default | PropertyUsageFlags.ScriptVariable, exported: true),
			new PropertyInfo(Variant.Type.Object, PropertyName.DEBUG_dialogueData, PropertyHint.ResourceType, "DialogueDataRes", PropertyUsageFlags.Default | PropertyUsageFlags.ScriptVariable, exported: true),
			new PropertyInfo(Variant.Type.Object, PropertyName.parentWindow, PropertyHint.None, "", PropertyUsageFlags.ScriptVariable, exported: false),
			new PropertyInfo(Variant.Type.Float, PropertyName._bounceSpeed, PropertyHint.None, "", PropertyUsageFlags.ScriptVariable, exported: false),
			new PropertyInfo(Variant.Type.Float, PropertyName._bounceAmplitude, PropertyHint.None, "", PropertyUsageFlags.ScriptVariable, exported: false),
			new PropertyInfo(Variant.Type.Float, PropertyName._bounceTime, PropertyHint.None, "", PropertyUsageFlags.ScriptVariable, exported: false),
			new PropertyInfo(Variant.Type.Int, PropertyName._bounceBaseY, PropertyHint.None, "", PropertyUsageFlags.ScriptVariable, exported: false),
			new PropertyInfo(Variant.Type.Bool, PropertyName._bounceInitialized, PropertyHint.None, "", PropertyUsageFlags.ScriptVariable, exported: false),
			new PropertyInfo(Variant.Type.Float, PropertyName._maxSpan, PropertyHint.None, "", PropertyUsageFlags.ScriptVariable, exported: false),
			new PropertyInfo(Variant.Type.Vector2, PropertyName._maxSpamLifeSpanInPassivePlay, PropertyHint.None, "", PropertyUsageFlags.ScriptVariable, exported: false),
			new PropertyInfo(Variant.Type.Bool, PropertyName._isSetup, PropertyHint.None, "", PropertyUsageFlags.ScriptVariable, exported: false),
			new PropertyInfo(Variant.Type.Bool, PropertyName._forcedMovementActive, PropertyHint.None, "", PropertyUsageFlags.ScriptVariable, exported: false),
			new PropertyInfo(Variant.Type.Bool, PropertyName._forcedMovementWalking, PropertyHint.None, "", PropertyUsageFlags.ScriptVariable, exported: false),
			new PropertyInfo(Variant.Type.Bool, PropertyName._forcedMovementFinalIdle, PropertyHint.None, "", PropertyUsageFlags.ScriptVariable, exported: false),
			new PropertyInfo(Variant.Type.Float, PropertyName._forcedMoveDirection, PropertyHint.None, "", PropertyUsageFlags.ScriptVariable, exported: false),
			new PropertyInfo(Variant.Type.Float, PropertyName._forcedMoveStateTimer, PropertyHint.None, "", PropertyUsageFlags.ScriptVariable, exported: false),
			new PropertyInfo(Variant.Type.Float, PropertyName._forcedMovementElapsed, PropertyHint.None, "", PropertyUsageFlags.ScriptVariable, exported: false),
			new PropertyInfo(Variant.Type.Float, PropertyName._forcedMovementDuration, PropertyHint.None, "", PropertyUsageFlags.ScriptVariable, exported: false),
			new PropertyInfo(Variant.Type.Float, PropertyName._forcedMovementX, PropertyHint.None, "", PropertyUsageFlags.ScriptVariable, exported: false)
		};
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override void SaveGodotObjectData(GodotSerializationInfo info)
	{
		base.SaveGodotObjectData(info);
		info.AddProperty(PropertyName.mainBody, Variant.From(in mainBody));
		info.AddProperty(PropertyName.attachObject, Variant.From(in attachObject));
		info.AddProperty(PropertyName.FadeKill, Variant.From(in FadeKill));
		info.AddProperty(PropertyName.AttachmentStackSpacing, Variant.From(in AttachmentStackSpacing));
		info.AddProperty(PropertyName.SpawnOnLoad, Variant.From(in SpawnOnLoad));
		info.AddProperty(PropertyName.DEBUG_dialogueData, Variant.From(in DEBUG_dialogueData));
		info.AddProperty(PropertyName.parentWindow, Variant.From(in parentWindow));
		info.AddProperty(PropertyName._bounceSpeed, Variant.From(in _bounceSpeed));
		info.AddProperty(PropertyName._bounceAmplitude, Variant.From(in _bounceAmplitude));
		info.AddProperty(PropertyName._bounceTime, Variant.From(in _bounceTime));
		info.AddProperty(PropertyName._bounceBaseY, Variant.From(in _bounceBaseY));
		info.AddProperty(PropertyName._bounceInitialized, Variant.From(in _bounceInitialized));
		info.AddProperty(PropertyName._maxSpan, Variant.From(in _maxSpan));
		info.AddProperty(PropertyName._maxSpamLifeSpanInPassivePlay, Variant.From(in _maxSpamLifeSpanInPassivePlay));
		info.AddProperty(PropertyName._isSetup, Variant.From(in _isSetup));
		info.AddProperty(PropertyName._forcedMovementActive, Variant.From(in _forcedMovementActive));
		info.AddProperty(PropertyName._forcedMovementWalking, Variant.From(in _forcedMovementWalking));
		info.AddProperty(PropertyName._forcedMovementFinalIdle, Variant.From(in _forcedMovementFinalIdle));
		info.AddProperty(PropertyName._forcedMoveDirection, Variant.From(in _forcedMoveDirection));
		info.AddProperty(PropertyName._forcedMoveStateTimer, Variant.From(in _forcedMoveStateTimer));
		info.AddProperty(PropertyName._forcedMovementElapsed, Variant.From(in _forcedMovementElapsed));
		info.AddProperty(PropertyName._forcedMovementDuration, Variant.From(in _forcedMovementDuration));
		info.AddProperty(PropertyName._forcedMovementX, Variant.From(in _forcedMovementX));
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override void RestoreGodotObjectData(GodotSerializationInfo info)
	{
		base.RestoreGodotObjectData(info);
		if (info.TryGetProperty(PropertyName.mainBody, out var value))
		{
			mainBody = value.As<AnimatedSprite2D>();
		}
		if (info.TryGetProperty(PropertyName.attachObject, out var value2))
		{
			attachObject = value2.As<AttachmentObject>();
		}
		if (info.TryGetProperty(PropertyName.FadeKill, out var value3))
		{
			FadeKill = value3.As<AnimationPlayer>();
		}
		if (info.TryGetProperty(PropertyName.AttachmentStackSpacing, out var value4))
		{
			AttachmentStackSpacing = value4.As<int>();
		}
		if (info.TryGetProperty(PropertyName.SpawnOnLoad, out var value5))
		{
			SpawnOnLoad = value5.As<bool>();
		}
		if (info.TryGetProperty(PropertyName.DEBUG_dialogueData, out var value6))
		{
			DEBUG_dialogueData = value6.As<DialogueDataRes>();
		}
		if (info.TryGetProperty(PropertyName.parentWindow, out var value7))
		{
			parentWindow = value7.As<Window>();
		}
		if (info.TryGetProperty(PropertyName._bounceSpeed, out var value8))
		{
			_bounceSpeed = value8.As<float>();
		}
		if (info.TryGetProperty(PropertyName._bounceAmplitude, out var value9))
		{
			_bounceAmplitude = value9.As<float>();
		}
		if (info.TryGetProperty(PropertyName._bounceTime, out var value10))
		{
			_bounceTime = value10.As<float>();
		}
		if (info.TryGetProperty(PropertyName._bounceBaseY, out var value11))
		{
			_bounceBaseY = value11.As<int>();
		}
		if (info.TryGetProperty(PropertyName._bounceInitialized, out var value12))
		{
			_bounceInitialized = value12.As<bool>();
		}
		if (info.TryGetProperty(PropertyName._maxSpan, out var value13))
		{
			_maxSpan = value13.As<double>();
		}
		if (info.TryGetProperty(PropertyName._maxSpamLifeSpanInPassivePlay, out var value14))
		{
			_maxSpamLifeSpanInPassivePlay = value14.As<Vector2>();
		}
		if (info.TryGetProperty(PropertyName._isSetup, out var value15))
		{
			_isSetup = value15.As<bool>();
		}
		if (info.TryGetProperty(PropertyName._forcedMovementActive, out var value16))
		{
			_forcedMovementActive = value16.As<bool>();
		}
		if (info.TryGetProperty(PropertyName._forcedMovementWalking, out var value17))
		{
			_forcedMovementWalking = value17.As<bool>();
		}
		if (info.TryGetProperty(PropertyName._forcedMovementFinalIdle, out var value18))
		{
			_forcedMovementFinalIdle = value18.As<bool>();
		}
		if (info.TryGetProperty(PropertyName._forcedMoveDirection, out var value19))
		{
			_forcedMoveDirection = value19.As<float>();
		}
		if (info.TryGetProperty(PropertyName._forcedMoveStateTimer, out var value20))
		{
			_forcedMoveStateTimer = value20.As<double>();
		}
		if (info.TryGetProperty(PropertyName._forcedMovementElapsed, out var value21))
		{
			_forcedMovementElapsed = value21.As<double>();
		}
		if (info.TryGetProperty(PropertyName._forcedMovementDuration, out var value22))
		{
			_forcedMovementDuration = value22.As<float>();
		}
		if (info.TryGetProperty(PropertyName._forcedMovementX, out var value23))
		{
			_forcedMovementX = value23.As<float>();
		}
	}
}
