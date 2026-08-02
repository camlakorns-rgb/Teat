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
public class Main : Node2D
{
	[Signal]
	public delegate void ReachedTargetEventHandler();

	public enum ItemTargetMode
	{
		UNTYPED,
		RANDOM,
		CLOSEST,
		FARTHEST
	}

	public new class MethodName : Node2D.MethodName
	{
		public new static readonly StringName _Ready = "_Ready";

		public new static readonly StringName _Process = "_Process";

		public new static readonly StringName _PhysicsProcess = "_PhysicsProcess";

		public static readonly StringName Magnify = "Magnify";

		public static readonly StringName RepositionAllItemsToMouseScreen = "RepositionAllItemsToMouseScreen";

		public static readonly StringName BootDialogue = "BootDialogue";

		public static readonly StringName PauseGame = "PauseGame";

		public static readonly StringName OpenTerminal = "OpenTerminal";

		public static readonly StringName CallCharacterForcedAnimation = "CallCharacterForcedAnimation";

		public static readonly StringName CallCharacterAttachmentSpawn = "CallCharacterAttachmentSpawn";

		public static readonly StringName PopDialogueInStack = "PopDialogueInStack";

		public static readonly StringName CallCharacterDialogueAttachmentSpawn = "CallCharacterDialogueAttachmentSpawn";

		public static readonly StringName CallActorSpawn = "CallActorSpawn";

		public static readonly StringName CallItemSpawn = "CallItemSpawn";

		public static readonly StringName CallPackedSceneSpawn = "CallPackedSceneSpawn";

		public static readonly StringName CallMinigameSpawn = "CallMinigameSpawn";

		public static readonly StringName OnSpawnerTimeout = "OnSpawnerTimeout";

		public static readonly StringName OnSpawnerActorTimeout = "OnSpawnerActorTimeout";

		public static readonly StringName OnAnimationTimerTimeout = "OnAnimationTimerTimeout";

		public static readonly StringName OnPassivePlayTimerTimeout = "OnPassivePlayTimerTimeout";

		public static readonly StringName OnRandomDialogueTimerTimeout = "OnRandomDialogueTimerTimeout";

		public static readonly StringName ClearAllAttachments = "ClearAllAttachments";

		public static readonly StringName ForceDropCharacter = "ForceDropCharacter";

		public static readonly StringName TrySetTargetToItem = "TrySetTargetToItem";

		public static readonly StringName IsBlacklisted = "IsBlacklisted";

		public static readonly StringName FollowMouse = "FollowMouse";

		public static readonly StringName MovePet = "MovePet";

		public static readonly StringName TweenGrabRelease = "TweenGrabRelease";

		public static readonly StringName OnGrabReleased = "OnGrabReleased";

		public static readonly StringName GetResolvedTargetX = "GetResolvedTargetX";

		public static readonly StringName StartAutoWalkToTarget = "StartAutoWalkToTarget";

		public static readonly StringName UpdateWalkDirection = "UpdateWalkDirection";

		public static readonly StringName Walk = "Walk";

		public static readonly StringName ReachedItemTarget = "ReachedItemTarget";

		public static readonly StringName AbortActivePickup = "AbortActivePickup";

		public static readonly StringName ReleaseItem = "ReleaseItem";

		public static readonly StringName ClearItemTarget = "ClearItemTarget";

		public static readonly StringName ChooseDirection = "ChooseDirection";

		public static readonly StringName OnCharacterWalking = "OnCharacterWalking";

		public static readonly StringName OnCharacterFinishedWalking = "OnCharacterFinishedWalking";

		public static readonly StringName GetThinnerCollisionBox = "GetThinnerCollisionBox";

		public static readonly StringName CheckLandingInteraction = "CheckLandingInteraction";
	}

	public new class PropertyName : Node2D.PropertyName
	{
		public static readonly StringName mainWindow = "mainWindow";

		public static readonly StringName mainCharacter = "mainCharacter";

		public static readonly StringName saveHandler = "saveHandler";

		public static readonly StringName spawnedItems = "spawnedItems";

		public static readonly StringName maxItems = "maxItems";

		public static readonly StringName spawnMargin = "spawnMargin";

		public static readonly StringName spawnerTimer = "spawnerTimer";

		public static readonly StringName spawnerTimeRange = "spawnerTimeRange";

		public static readonly StringName spawnedAttachments = "spawnedAttachments";

		public static readonly StringName dialogueStack = "dialogueStack";

		public static readonly StringName timeBetweenStacks = "timeBetweenStacks";

		public static readonly StringName defaultTextDataRes = "defaultTextDataRes";

		public static readonly StringName randomDialogueTimer = "randomDialogueTimer";

		public static readonly StringName spawnedActors = "spawnedActors";

		public static readonly StringName spawnedCompanions = "spawnedCompanions";

		public static readonly StringName companionLimit = "companionLimit";

		public static readonly StringName spawnerActorTimer = "spawnerActorTimer";

		public static readonly StringName spawnerActorTimerRange = "spawnerActorTimerRange";

		public static readonly StringName randomAnimationTimer = "randomAnimationTimer";

		public static readonly StringName passivePlayTimer = "passivePlayTimer";

		public static readonly StringName passivePlayTimerOffRange = "passivePlayTimerOffRange";

		public static readonly StringName passivePlayTimerOnRange = "passivePlayTimerOnRange";

		public static readonly StringName throwSoftReactionChance = "throwSoftReactionChance";

		public static readonly StringName throwHardReactionChance = "throwHardReactionChance";

		public static readonly StringName landSoftReactionChance = "landSoftReactionChance";

		public static readonly StringName landHardReactionChance = "landHardReactionChance";

		public static readonly StringName ItemObjectScene = "ItemObjectScene";

		public static readonly StringName AttachmentObjectScene = "AttachmentObjectScene";

		public static readonly StringName actorScene = "actorScene";

		public static readonly StringName pauseMenu = "pauseMenu";

		public static readonly StringName terminalMenu = "terminalMenu";

		public static readonly StringName confirmationMenu = "confirmationMenu";

		public static readonly StringName eulaMenu = "eulaMenu";

		public static readonly StringName magnifierScene = "magnifierScene";

		public static readonly StringName settingWindowThrowPhysics = "settingWindowThrowPhysics";

		public static readonly StringName settingEULA = "settingEULA";

		public static readonly StringName settingSpriteScaler = "settingSpriteScaler";

		public static readonly StringName settingItemScaler = "settingItemScaler";

		public static readonly StringName settingUIScaler = "settingUIScaler";

		public static readonly StringName settingSpawnItems = "settingSpawnItems";

		public static readonly StringName settingSpawnActors = "settingSpawnActors";

		public static readonly StringName settingPassivePlayMode = "settingPassivePlayMode";

		public static readonly StringName settingRemovePopups = "settingRemovePopups";

		public static readonly StringName settingRemoveConvos = "settingRemoveConvos";

		public static readonly StringName settingAudioOn = "settingAudioOn";

		public static readonly StringName settingMods = "settingMods";

		public static readonly StringName settingEnabledMods = "settingEnabledMods";

		public static readonly StringName userInfoName = "userInfoName";

		public static readonly StringName SeenObjects = "SeenObjects";

		public static readonly StringName userTickets = "userTickets";

		public static readonly StringName minigameData = "minigameData";

		public static readonly StringName settingBlacklistedContent = "settingBlacklistedContent";

		public static readonly StringName mouseOffset = "mouseOffset";

		public static readonly StringName selected = "selected";

		public static readonly StringName SomethingHasBeenGrabbed = "SomethingHasBeenGrabbed";

		public static readonly StringName grabReleaseTween = "grabReleaseTween";

		public static readonly StringName screenDataHandler = "screenDataHandler";

		public static readonly StringName isWalking = "isWalking";

		public static readonly StringName walkDirection = "walkDirection";

		public static readonly StringName pickupWatchdogTimer = "pickupWatchdogTimer";

		public static readonly StringName mouseVelocityScaler = "mouseVelocityScaler";

		public static readonly StringName windowBounceDamping = "windowBounceDamping";

		public static readonly StringName windowAirResist = "windowAirResist";

		public static readonly StringName windowVelocity = "windowVelocity";

		public static readonly StringName isThrown = "isThrown";

		public static readonly StringName isWalkingToTargetPosition = "isWalkingToTargetPosition";

		public static readonly StringName targetX = "targetX";

		public static readonly StringName walkTargetItem = "walkTargetItem";

		public static readonly StringName storedItem = "storedItem";

		public static readonly StringName inPickup = "inPickup";

		public static readonly StringName isInConvo = "isInConvo";

		public static readonly StringName spawnedMinigames = "spawnedMinigames";

		public static readonly StringName Pause = "Pause";

		public static readonly StringName Terminal = "Terminal";

		public static readonly StringName AdminAccess = "AdminAccess";

		public static readonly StringName Magnifier = "Magnifier";

		public static readonly StringName _magnifierActive = "_magnifierActive";
	}

	public new class SignalName : Node2D.SignalName
	{
		public static readonly StringName ReachedTarget = "ReachedTarget";
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

	private ReachedTargetEventHandler backing_ReachedTarget;

	public static Main Instance { get; private set; }

	public event ReachedTargetEventHandler ReachedTarget
	{
		add
		{
			backing_ReachedTarget = (ReachedTargetEventHandler)Delegate.Combine(backing_ReachedTarget, value);
		}
		remove
		{
			backing_ReachedTarget = (ReachedTargetEventHandler)Delegate.Remove(backing_ReachedTarget, value);
		}
	}

	public override void _Ready()
	{
		PowerThrottling.DisableThrottling();
		Instance = this;
		mainWindow = GetWindow();
		mainWindow.TransparentBg = true;
		saveHandler.AttemptLoad();
		mainCharacter.trueSize = (Vector2I)(mainCharacter.characterInformation.characterSize * mainCharacter.characterInformation.characterScale * settingSpriteScaler);
		mainCharacter.SetupCharacter();
		screenDataHandler.UpdateScreenInfo(mainCharacter.trueSize);
		mainWindow.MinSize = mainCharacter.trueSize;
		mainWindow.Size = mainWindow.MinSize;
		mainWindow.Borderless = true;
		mainWindow.Unresizable = true;
		mainWindow.AlwaysOnTop = true;
		mainWindow.GuiEmbedSubwindows = false;
		mainWindow.Transparent = true;
		mainWindow.Position = new Vector2I(DisplayServer.ScreenGetSize(screenDataHandler.screenIndex).X / 2 - mainCharacter.trueSize.X / 2, screenDataHandler.taskbarPos);
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
		if (Input.IsActionJustPressed("Screen_Lock") && GetThinnerCollisionBox().HasPoint(DisplayServer.MouseGetPosition()))
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
		else if (mainWindow.Position.Y < screenDataHandler.taskbarPos)
		{
			mainWindow.Position += new Vector2I(0, Mathf.RoundToInt(980f * (float)delta));
		}
		else
		{
			mainWindow.Position = new Vector2I(mainWindow.Position.X, screenDataHandler.taskbarPos);
		}
		if (mainCharacter.Visible)
		{
			MovePet();
			if (Input.IsActionJustPressed("Pet") && GetThinnerCollisionBox().HasPoint(DisplayServer.MouseGetPosition()))
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
			if (Input.IsActionPressed("Pet") && GetThinnerCollisionBox().HasPoint(DisplayServer.MouseGetPosition()) && mainCharacter.petMainBodyState == Character.MainBodyStates.Idle)
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
		else if (Input.IsActionJustPressed("Despawn") && GetThinnerCollisionBox().HasPoint(DisplayServer.MouseGetPosition()))
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

	private void RepositionAllItemsToMouseScreen()
	{
		if (spawnedItems.Count == 0)
		{
			return;
		}
		Vector2I vector2I = DisplayServer.MouseGetPosition();
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
		itemWindow.SetupItemWindow(spawningItem);
		AddChild(itemWindow, forceReadableName: false, InternalMode.Disabled);
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
			spawnerTimer.WaitTime = GD.RandRange(spawnerTimeRange.X, spawnerTimeRange.Y);
			spawnerTimer.Start();
			if (spawnedItems.Count >= maxItems)
			{
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
			string item = weightGroup.GetItem(GD.RandRange(0, 10000));
			ItemWindow itemWindow = ItemObjectScene.Instantiate<ItemWindow>(PackedScene.GenEditState.Disabled);
			itemWindow.SetupItemWindow((ItemDataRes)ResourceCache.resourcesLoaded[ResourceCache.ResourceTyping.ITEM][item]);
			AddChild(itemWindow, forceReadableName: false, InternalMode.Disabled);
			if (x == -1 && y == -1)
			{
				int num = (int)GD.RandRange((float)screenDataHandler.EffectiveLeftX + spawnMargin.X, (float)screenDataHandler.EffectiveRightX - spawnMargin.X);
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
			else
			{
				itemWindow.Position = new Vector2I(x, y);
			}
			spawnedItems.Add(itemWindow);
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
			where item.Visible
			select item).ToList();
		if (possibleArray != null)
		{
			list.Clear();
			list = (from item in possibleArray
				where GodotObject.IsInstanceValid(item)
				where settingPassivePlayMode || !item.itemObject.itemInformation.NontargetablePickup
				where !item.itemObject.itemInformation.NoPassivePickup
				where item.Visible
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
		Vector2I mousePos = DisplayServer.MouseGetPosition();
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
		if (Input.IsActionPressed("Move") && !SomethingHasBeenGrabbed && GetThinnerCollisionBox().HasPoint(DisplayServer.MouseGetPosition()))
		{
			selected = true;
			SomethingHasBeenGrabbed = true;
			mouseOffset = base.Position - GetGlobalMousePosition();
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
		Vector2I position = mainWindow.Position;
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
			mainWindow.Position = new Vector2I(screenDataHandler.ClampAcrossAllScreensX(position.X, mainCharacter.trueSize.X), screenDataHandler.ClampAcrossAllScreensY(position.Y));
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
			mainWindow.Position = new Vector2I(screenDataHandler.ClampAcrossAllScreensX(position.X, mainCharacter.trueSize.X), screenDataHandler.ClampAcrossAllScreensY(position.Y));
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

	private Rect2I GetThinnerCollisionBox()
	{
		float num = 0.33f;
		int num2 = Mathf.RoundToInt((float)mainWindow.Size.X * num);
		int num3 = Mathf.RoundToInt((float)(mainWindow.Size.X - num2) / 2f);
		return new Rect2I(new Vector2I(mainWindow.Position.X + num3, mainWindow.Position.Y), new Vector2I(num2, mainWindow.Size.Y));
	}

	private void CheckLandingInteraction(bool hardLanding = false)
	{
		Rect2I collisionBox = GetThinnerCollisionBox();
		int centerX = collisionBox.Position.X + collisionBox.Size.X / 2;
		List<ActorWindow> source = (from a in spawnedCompanions
			where GodotObject.IsInstanceValid(a) && a.Visible && !a.inUse && !a.inUseByAttachment
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
			if (!GodotObject.IsInstanceValid(spawnedItem) || !spawnedItem.Visible || spawnedItem.CurrentlyPickedUp || !spawnedItem.itemObject.itemInformation.isUsableDroppedOn)
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

	[EditorBrowsable(EditorBrowsableState.Never)]
	internal static List<MethodInfo> GetGodotMethodList()
	{
		return new List<MethodInfo>(44)
		{
			new MethodInfo(MethodName._Ready, new PropertyInfo(Variant.Type.Nil, "", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false), MethodFlags.Normal, null, null),
			new MethodInfo(MethodName._Process, new PropertyInfo(Variant.Type.Nil, "", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false), MethodFlags.Normal, new List<PropertyInfo>
			{
				new PropertyInfo(Variant.Type.Float, "delta", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false)
			}, null),
			new MethodInfo(MethodName._PhysicsProcess, new PropertyInfo(Variant.Type.Nil, "", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false), MethodFlags.Normal, new List<PropertyInfo>
			{
				new PropertyInfo(Variant.Type.Float, "delta", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false)
			}, null),
			new MethodInfo(MethodName.Magnify, new PropertyInfo(Variant.Type.Nil, "", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false), MethodFlags.Normal, new List<PropertyInfo>
			{
				new PropertyInfo(Variant.Type.Float, "delta", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false)
			}, null),
			new MethodInfo(MethodName.RepositionAllItemsToMouseScreen, new PropertyInfo(Variant.Type.Nil, "", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false), MethodFlags.Normal, null, null),
			new MethodInfo(MethodName.BootDialogue, new PropertyInfo(Variant.Type.Nil, "", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false), MethodFlags.Normal, new List<PropertyInfo>
			{
				new PropertyInfo(Variant.Type.Bool, "firstTime", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false)
			}, null),
			new MethodInfo(MethodName.PauseGame, new PropertyInfo(Variant.Type.Nil, "", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false), MethodFlags.Normal, null, null),
			new MethodInfo(MethodName.OpenTerminal, new PropertyInfo(Variant.Type.Nil, "", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false), MethodFlags.Normal, null, null),
			new MethodInfo(MethodName.CallCharacterForcedAnimation, new PropertyInfo(Variant.Type.Nil, "", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false), MethodFlags.Normal, new List<PropertyInfo>
			{
				new PropertyInfo(Variant.Type.String, "animationName", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false),
				new PropertyInfo(Variant.Type.Float, "animationTime", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false)
			}, null),
			new MethodInfo(MethodName.CallCharacterAttachmentSpawn, new PropertyInfo(Variant.Type.Nil, "", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false), MethodFlags.Normal, new List<PropertyInfo>
			{
				new PropertyInfo(Variant.Type.Object, "objData", PropertyHint.None, "", PropertyUsageFlags.Default, new StringName("Resource"), exported: false),
				new PropertyInfo(Variant.Type.Bool, "unclearableAttachment", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false)
			}, null),
			new MethodInfo(MethodName.CallCharacterAttachmentSpawn, new PropertyInfo(Variant.Type.Nil, "", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false), MethodFlags.Normal, new List<PropertyInfo>
			{
				new PropertyInfo(Variant.Type.Object, "objData", PropertyHint.None, "", PropertyUsageFlags.Default, new StringName("Resource"), exported: false),
				new PropertyInfo(Variant.Type.Bool, "unclearableAttachment", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false),
				new PropertyInfo(Variant.Type.Object, "targetWindow", PropertyHint.None, "", PropertyUsageFlags.Default, new StringName("Window"), exported: false)
			}, null),
			new MethodInfo(MethodName.PopDialogueInStack, new PropertyInfo(Variant.Type.Nil, "", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false), MethodFlags.Normal, new List<PropertyInfo>
			{
				new PropertyInfo(Variant.Type.Bool, "skipTimer", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false)
			}, null),
			new MethodInfo(MethodName.CallCharacterDialogueAttachmentSpawn, new PropertyInfo(Variant.Type.Nil, "", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false), MethodFlags.Normal, new List<PropertyInfo>
			{
				new PropertyInfo(Variant.Type.Object, "diaData", PropertyHint.None, "", PropertyUsageFlags.Default, new StringName("Resource"), exported: false)
			}, null),
			new MethodInfo(MethodName.CallActorSpawn, new PropertyInfo(Variant.Type.Nil, "", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false), MethodFlags.Normal, new List<PropertyInfo>
			{
				new PropertyInfo(Variant.Type.Object, "spawningActor", PropertyHint.None, "", PropertyUsageFlags.Default, new StringName("Resource"), exported: false)
			}, null),
			new MethodInfo(MethodName.CallActorSpawn, new PropertyInfo(Variant.Type.Nil, "", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false), MethodFlags.Normal, new List<PropertyInfo>
			{
				new PropertyInfo(Variant.Type.Object, "spawningActor", PropertyHint.None, "", PropertyUsageFlags.Default, new StringName("Resource"), exported: false),
				new PropertyInfo(Variant.Type.Vector2I, "Pos", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false),
				new PropertyInfo(Variant.Type.Object, "possibleTarget", PropertyHint.None, "", PropertyUsageFlags.Default, new StringName("Window"), exported: false)
			}, null),
			new MethodInfo(MethodName.CallItemSpawn, new PropertyInfo(Variant.Type.Nil, "", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false), MethodFlags.Normal, new List<PropertyInfo>
			{
				new PropertyInfo(Variant.Type.Object, "spawningItem", PropertyHint.None, "", PropertyUsageFlags.Default, new StringName("Resource"), exported: false),
				new PropertyInfo(Variant.Type.Vector2I, "spawningPosition", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false)
			}, null),
			new MethodInfo(MethodName.CallPackedSceneSpawn, new PropertyInfo(Variant.Type.Nil, "", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false), MethodFlags.Normal, new List<PropertyInfo>
			{
				new PropertyInfo(Variant.Type.String, "genericID", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false)
			}, null),
			new MethodInfo(MethodName.CallMinigameSpawn, new PropertyInfo(Variant.Type.Nil, "", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false), MethodFlags.Normal, new List<PropertyInfo>
			{
				new PropertyInfo(Variant.Type.String, "minigameID", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false)
			}, null),
			new MethodInfo(MethodName.OnSpawnerTimeout, new PropertyInfo(Variant.Type.Nil, "", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false), MethodFlags.Normal, new List<PropertyInfo>
			{
				new PropertyInfo(Variant.Type.Int, "x", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false),
				new PropertyInfo(Variant.Type.Int, "y", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false)
			}, null),
			new MethodInfo(MethodName.OnSpawnerActorTimeout, new PropertyInfo(Variant.Type.Nil, "", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false), MethodFlags.Normal, null, null),
			new MethodInfo(MethodName.OnAnimationTimerTimeout, new PropertyInfo(Variant.Type.Nil, "", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false), MethodFlags.Normal, null, null),
			new MethodInfo(MethodName.OnPassivePlayTimerTimeout, new PropertyInfo(Variant.Type.Nil, "", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false), MethodFlags.Normal, null, null),
			new MethodInfo(MethodName.OnRandomDialogueTimerTimeout, new PropertyInfo(Variant.Type.Nil, "", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false), MethodFlags.Normal, null, null),
			new MethodInfo(MethodName.ClearAllAttachments, new PropertyInfo(Variant.Type.Nil, "", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false), MethodFlags.Normal, null, null),
			new MethodInfo(MethodName.ForceDropCharacter, new PropertyInfo(Variant.Type.Nil, "", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false), MethodFlags.Normal, null, null),
			new MethodInfo(MethodName.TrySetTargetToItem, new PropertyInfo(Variant.Type.Bool, "", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false), MethodFlags.Normal, new List<PropertyInfo>
			{
				new PropertyInfo(Variant.Type.Int, "mode", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false),
				new PropertyInfo(Variant.Type.Array, "possibleArray", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false)
			}, null),
			new MethodInfo(MethodName.IsBlacklisted, new PropertyInfo(Variant.Type.Bool, "", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false), MethodFlags.Normal, new List<PropertyInfo>
			{
				new PropertyInfo(Variant.Type.Int, "taggedKink", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false)
			}, null),
			new MethodInfo(MethodName.FollowMouse, new PropertyInfo(Variant.Type.Nil, "", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false), MethodFlags.Normal, new List<PropertyInfo>
			{
				new PropertyInfo(Variant.Type.Bool, "Unlocked", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false)
			}, null),
			new MethodInfo(MethodName.MovePet, new PropertyInfo(Variant.Type.Nil, "", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false), MethodFlags.Normal, null, null),
			new MethodInfo(MethodName.TweenGrabRelease, new PropertyInfo(Variant.Type.Nil, "", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false), MethodFlags.Normal, null, null),
			new MethodInfo(MethodName.OnGrabReleased, new PropertyInfo(Variant.Type.Nil, "", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false), MethodFlags.Normal, null, null),
			new MethodInfo(MethodName.GetResolvedTargetX, new PropertyInfo(Variant.Type.Int, "", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false), MethodFlags.Normal, null, null),
			new MethodInfo(MethodName.StartAutoWalkToTarget, new PropertyInfo(Variant.Type.Nil, "", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false), MethodFlags.Normal, null, null),
			new MethodInfo(MethodName.UpdateWalkDirection, new PropertyInfo(Variant.Type.Nil, "", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false), MethodFlags.Normal, null, null),
			new MethodInfo(MethodName.Walk, new PropertyInfo(Variant.Type.Nil, "", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false), MethodFlags.Normal, new List<PropertyInfo>
			{
				new PropertyInfo(Variant.Type.Float, "delta", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false)
			}, null),
			new MethodInfo(MethodName.ReachedItemTarget, new PropertyInfo(Variant.Type.Nil, "", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false), MethodFlags.Normal, null, null),
			new MethodInfo(MethodName.AbortActivePickup, new PropertyInfo(Variant.Type.Nil, "", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false), MethodFlags.Normal, null, null),
			new MethodInfo(MethodName.ReleaseItem, new PropertyInfo(Variant.Type.Nil, "", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false), MethodFlags.Normal, new List<PropertyInfo>
			{
				new PropertyInfo(Variant.Type.Object, "item", PropertyHint.None, "", PropertyUsageFlags.Default, new StringName("Window"), exported: false)
			}, null),
			new MethodInfo(MethodName.ClearItemTarget, new PropertyInfo(Variant.Type.Nil, "", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false), MethodFlags.Normal, new List<PropertyInfo>
			{
				new PropertyInfo(Variant.Type.Bool, "usingItem", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false)
			}, null),
			new MethodInfo(MethodName.ChooseDirection, new PropertyInfo(Variant.Type.Nil, "", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false), MethodFlags.Normal, null, null),
			new MethodInfo(MethodName.OnCharacterWalking, new PropertyInfo(Variant.Type.Nil, "", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false), MethodFlags.Normal, null, null),
			new MethodInfo(MethodName.OnCharacterFinishedWalking, new PropertyInfo(Variant.Type.Nil, "", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false), MethodFlags.Normal, null, null),
			new MethodInfo(MethodName.GetThinnerCollisionBox, new PropertyInfo(Variant.Type.Rect2I, "", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false), MethodFlags.Normal, null, null),
			new MethodInfo(MethodName.CheckLandingInteraction, new PropertyInfo(Variant.Type.Nil, "", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false), MethodFlags.Normal, new List<PropertyInfo>
			{
				new PropertyInfo(Variant.Type.Bool, "hardLanding", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false)
			}, null)
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
		if (method == MethodName._PhysicsProcess && args.Count == 1)
		{
			_PhysicsProcess(VariantUtils.ConvertTo<double>(in args[0]));
			ret = default(godot_variant);
			return true;
		}
		if (method == MethodName.Magnify && args.Count == 1)
		{
			Magnify(VariantUtils.ConvertTo<double>(in args[0]));
			ret = default(godot_variant);
			return true;
		}
		if (method == MethodName.RepositionAllItemsToMouseScreen && args.Count == 0)
		{
			RepositionAllItemsToMouseScreen();
			ret = default(godot_variant);
			return true;
		}
		if (method == MethodName.BootDialogue && args.Count == 1)
		{
			BootDialogue(VariantUtils.ConvertTo<bool>(in args[0]));
			ret = default(godot_variant);
			return true;
		}
		if (method == MethodName.PauseGame && args.Count == 0)
		{
			PauseGame();
			ret = default(godot_variant);
			return true;
		}
		if (method == MethodName.OpenTerminal && args.Count == 0)
		{
			OpenTerminal();
			ret = default(godot_variant);
			return true;
		}
		if (method == MethodName.CallCharacterForcedAnimation && args.Count == 2)
		{
			CallCharacterForcedAnimation(VariantUtils.ConvertTo<string>(in args[0]), VariantUtils.ConvertTo<float>(in args[1]));
			ret = default(godot_variant);
			return true;
		}
		if (method == MethodName.CallCharacterAttachmentSpawn && args.Count == 2)
		{
			CallCharacterAttachmentSpawn(VariantUtils.ConvertTo<AttachDataRes>(in args[0]), VariantUtils.ConvertTo<bool>(in args[1]));
			ret = default(godot_variant);
			return true;
		}
		if (method == MethodName.CallCharacterAttachmentSpawn && args.Count == 3)
		{
			CallCharacterAttachmentSpawn(VariantUtils.ConvertTo<AttachDataRes>(in args[0]), VariantUtils.ConvertTo<bool>(in args[1]), VariantUtils.ConvertTo<Window>(in args[2]));
			ret = default(godot_variant);
			return true;
		}
		if (method == MethodName.PopDialogueInStack && args.Count == 1)
		{
			PopDialogueInStack(VariantUtils.ConvertTo<bool>(in args[0]));
			ret = default(godot_variant);
			return true;
		}
		if (method == MethodName.CallCharacterDialogueAttachmentSpawn && args.Count == 1)
		{
			CallCharacterDialogueAttachmentSpawn(VariantUtils.ConvertTo<DialogueDataRes>(in args[0]));
			ret = default(godot_variant);
			return true;
		}
		if (method == MethodName.CallActorSpawn && args.Count == 1)
		{
			CallActorSpawn(VariantUtils.ConvertTo<CharacterInfoDataRes>(in args[0]));
			ret = default(godot_variant);
			return true;
		}
		if (method == MethodName.CallActorSpawn && args.Count == 3)
		{
			CallActorSpawn(VariantUtils.ConvertTo<CharacterInfoDataRes>(in args[0]), VariantUtils.ConvertTo<Vector2I>(in args[1]), VariantUtils.ConvertTo<ActorWindow>(in args[2]));
			ret = default(godot_variant);
			return true;
		}
		if (method == MethodName.CallItemSpawn && args.Count == 2)
		{
			CallItemSpawn(VariantUtils.ConvertTo<ItemDataRes>(in args[0]), VariantUtils.ConvertTo<Vector2I>(in args[1]));
			ret = default(godot_variant);
			return true;
		}
		if (method == MethodName.CallPackedSceneSpawn && args.Count == 1)
		{
			CallPackedSceneSpawn(VariantUtils.ConvertTo<string>(in args[0]));
			ret = default(godot_variant);
			return true;
		}
		if (method == MethodName.CallMinigameSpawn && args.Count == 1)
		{
			CallMinigameSpawn(VariantUtils.ConvertTo<string>(in args[0]));
			ret = default(godot_variant);
			return true;
		}
		if (method == MethodName.OnSpawnerTimeout && args.Count == 2)
		{
			OnSpawnerTimeout(VariantUtils.ConvertTo<int>(in args[0]), VariantUtils.ConvertTo<int>(in args[1]));
			ret = default(godot_variant);
			return true;
		}
		if (method == MethodName.OnSpawnerActorTimeout && args.Count == 0)
		{
			OnSpawnerActorTimeout();
			ret = default(godot_variant);
			return true;
		}
		if (method == MethodName.OnAnimationTimerTimeout && args.Count == 0)
		{
			OnAnimationTimerTimeout();
			ret = default(godot_variant);
			return true;
		}
		if (method == MethodName.OnPassivePlayTimerTimeout && args.Count == 0)
		{
			OnPassivePlayTimerTimeout();
			ret = default(godot_variant);
			return true;
		}
		if (method == MethodName.OnRandomDialogueTimerTimeout && args.Count == 0)
		{
			OnRandomDialogueTimerTimeout();
			ret = default(godot_variant);
			return true;
		}
		if (method == MethodName.ClearAllAttachments && args.Count == 0)
		{
			ClearAllAttachments();
			ret = default(godot_variant);
			return true;
		}
		if (method == MethodName.ForceDropCharacter && args.Count == 0)
		{
			ForceDropCharacter();
			ret = default(godot_variant);
			return true;
		}
		if (method == MethodName.TrySetTargetToItem && args.Count == 2)
		{
			bool from = TrySetTargetToItem(VariantUtils.ConvertTo<ItemTargetMode>(in args[0]), VariantUtils.ConvertToArray<ItemWindow>(in args[1]));
			ret = VariantUtils.CreateFrom(in from);
			return true;
		}
		if (method == MethodName.IsBlacklisted && args.Count == 1)
		{
			bool from2 = IsBlacklisted(VariantUtils.ConvertTo<SaveHandler.Kinks>(in args[0]));
			ret = VariantUtils.CreateFrom(in from2);
			return true;
		}
		if (method == MethodName.FollowMouse && args.Count == 1)
		{
			FollowMouse(VariantUtils.ConvertTo<bool>(in args[0]));
			ret = default(godot_variant);
			return true;
		}
		if (method == MethodName.MovePet && args.Count == 0)
		{
			MovePet();
			ret = default(godot_variant);
			return true;
		}
		if (method == MethodName.TweenGrabRelease && args.Count == 0)
		{
			TweenGrabRelease();
			ret = default(godot_variant);
			return true;
		}
		if (method == MethodName.OnGrabReleased && args.Count == 0)
		{
			OnGrabReleased();
			ret = default(godot_variant);
			return true;
		}
		if (method == MethodName.GetResolvedTargetX && args.Count == 0)
		{
			int from3 = GetResolvedTargetX();
			ret = VariantUtils.CreateFrom(in from3);
			return true;
		}
		if (method == MethodName.StartAutoWalkToTarget && args.Count == 0)
		{
			StartAutoWalkToTarget();
			ret = default(godot_variant);
			return true;
		}
		if (method == MethodName.UpdateWalkDirection && args.Count == 0)
		{
			UpdateWalkDirection();
			ret = default(godot_variant);
			return true;
		}
		if (method == MethodName.Walk && args.Count == 1)
		{
			Walk(VariantUtils.ConvertTo<double>(in args[0]));
			ret = default(godot_variant);
			return true;
		}
		if (method == MethodName.ReachedItemTarget && args.Count == 0)
		{
			ReachedItemTarget();
			ret = default(godot_variant);
			return true;
		}
		if (method == MethodName.AbortActivePickup && args.Count == 0)
		{
			AbortActivePickup();
			ret = default(godot_variant);
			return true;
		}
		if (method == MethodName.ReleaseItem && args.Count == 1)
		{
			ReleaseItem(VariantUtils.ConvertTo<ItemWindow>(in args[0]));
			ret = default(godot_variant);
			return true;
		}
		if (method == MethodName.ClearItemTarget && args.Count == 1)
		{
			ClearItemTarget(VariantUtils.ConvertTo<bool>(in args[0]));
			ret = default(godot_variant);
			return true;
		}
		if (method == MethodName.ChooseDirection && args.Count == 0)
		{
			ChooseDirection();
			ret = default(godot_variant);
			return true;
		}
		if (method == MethodName.OnCharacterWalking && args.Count == 0)
		{
			OnCharacterWalking();
			ret = default(godot_variant);
			return true;
		}
		if (method == MethodName.OnCharacterFinishedWalking && args.Count == 0)
		{
			OnCharacterFinishedWalking();
			ret = default(godot_variant);
			return true;
		}
		if (method == MethodName.GetThinnerCollisionBox && args.Count == 0)
		{
			Rect2I from4 = GetThinnerCollisionBox();
			ret = VariantUtils.CreateFrom(in from4);
			return true;
		}
		if (method == MethodName.CheckLandingInteraction && args.Count == 1)
		{
			CheckLandingInteraction(VariantUtils.ConvertTo<bool>(in args[0]));
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
		if (method == MethodName._PhysicsProcess)
		{
			return true;
		}
		if (method == MethodName.Magnify)
		{
			return true;
		}
		if (method == MethodName.RepositionAllItemsToMouseScreen)
		{
			return true;
		}
		if (method == MethodName.BootDialogue)
		{
			return true;
		}
		if (method == MethodName.PauseGame)
		{
			return true;
		}
		if (method == MethodName.OpenTerminal)
		{
			return true;
		}
		if (method == MethodName.CallCharacterForcedAnimation)
		{
			return true;
		}
		if (method == MethodName.CallCharacterAttachmentSpawn)
		{
			return true;
		}
		if (method == MethodName.PopDialogueInStack)
		{
			return true;
		}
		if (method == MethodName.CallCharacterDialogueAttachmentSpawn)
		{
			return true;
		}
		if (method == MethodName.CallActorSpawn)
		{
			return true;
		}
		if (method == MethodName.CallItemSpawn)
		{
			return true;
		}
		if (method == MethodName.CallPackedSceneSpawn)
		{
			return true;
		}
		if (method == MethodName.CallMinigameSpawn)
		{
			return true;
		}
		if (method == MethodName.OnSpawnerTimeout)
		{
			return true;
		}
		if (method == MethodName.OnSpawnerActorTimeout)
		{
			return true;
		}
		if (method == MethodName.OnAnimationTimerTimeout)
		{
			return true;
		}
		if (method == MethodName.OnPassivePlayTimerTimeout)
		{
			return true;
		}
		if (method == MethodName.OnRandomDialogueTimerTimeout)
		{
			return true;
		}
		if (method == MethodName.ClearAllAttachments)
		{
			return true;
		}
		if (method == MethodName.ForceDropCharacter)
		{
			return true;
		}
		if (method == MethodName.TrySetTargetToItem)
		{
			return true;
		}
		if (method == MethodName.IsBlacklisted)
		{
			return true;
		}
		if (method == MethodName.FollowMouse)
		{
			return true;
		}
		if (method == MethodName.MovePet)
		{
			return true;
		}
		if (method == MethodName.TweenGrabRelease)
		{
			return true;
		}
		if (method == MethodName.OnGrabReleased)
		{
			return true;
		}
		if (method == MethodName.GetResolvedTargetX)
		{
			return true;
		}
		if (method == MethodName.StartAutoWalkToTarget)
		{
			return true;
		}
		if (method == MethodName.UpdateWalkDirection)
		{
			return true;
		}
		if (method == MethodName.Walk)
		{
			return true;
		}
		if (method == MethodName.ReachedItemTarget)
		{
			return true;
		}
		if (method == MethodName.AbortActivePickup)
		{
			return true;
		}
		if (method == MethodName.ReleaseItem)
		{
			return true;
		}
		if (method == MethodName.ClearItemTarget)
		{
			return true;
		}
		if (method == MethodName.ChooseDirection)
		{
			return true;
		}
		if (method == MethodName.OnCharacterWalking)
		{
			return true;
		}
		if (method == MethodName.OnCharacterFinishedWalking)
		{
			return true;
		}
		if (method == MethodName.GetThinnerCollisionBox)
		{
			return true;
		}
		if (method == MethodName.CheckLandingInteraction)
		{
			return true;
		}
		return base.HasGodotClassMethod(in method);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override bool SetGodotClassPropertyValue(in godot_string_name name, in godot_variant value)
	{
		if (name == PropertyName.mainWindow)
		{
			mainWindow = VariantUtils.ConvertTo<Window>(in value);
			return true;
		}
		if (name == PropertyName.mainCharacter)
		{
			mainCharacter = VariantUtils.ConvertTo<Character>(in value);
			return true;
		}
		if (name == PropertyName.saveHandler)
		{
			saveHandler = VariantUtils.ConvertTo<SaveHandler>(in value);
			return true;
		}
		if (name == PropertyName.spawnedItems)
		{
			spawnedItems = VariantUtils.ConvertToArray<ItemWindow>(in value);
			return true;
		}
		if (name == PropertyName.maxItems)
		{
			maxItems = VariantUtils.ConvertTo<int>(in value);
			return true;
		}
		if (name == PropertyName.spawnMargin)
		{
			spawnMargin = VariantUtils.ConvertTo<Vector2>(in value);
			return true;
		}
		if (name == PropertyName.spawnerTimer)
		{
			spawnerTimer = VariantUtils.ConvertTo<Timer>(in value);
			return true;
		}
		if (name == PropertyName.spawnerTimeRange)
		{
			spawnerTimeRange = VariantUtils.ConvertTo<Vector2>(in value);
			return true;
		}
		if (name == PropertyName.spawnedAttachments)
		{
			spawnedAttachments = VariantUtils.ConvertToArray<AttachObjWindow>(in value);
			return true;
		}
		if (name == PropertyName.dialogueStack)
		{
			dialogueStack = VariantUtils.ConvertToArray<DialogueDataRes>(in value);
			return true;
		}
		if (name == PropertyName.timeBetweenStacks)
		{
			timeBetweenStacks = VariantUtils.ConvertTo<Vector2>(in value);
			return true;
		}
		if (name == PropertyName.defaultTextDataRes)
		{
			defaultTextDataRes = VariantUtils.ConvertTo<AttachDataRes>(in value);
			return true;
		}
		if (name == PropertyName.randomDialogueTimer)
		{
			randomDialogueTimer = VariantUtils.ConvertTo<Timer>(in value);
			return true;
		}
		if (name == PropertyName.spawnedActors)
		{
			spawnedActors = VariantUtils.ConvertToArray<ActorWindow>(in value);
			return true;
		}
		if (name == PropertyName.spawnedCompanions)
		{
			spawnedCompanions = VariantUtils.ConvertToArray<ActorWindow>(in value);
			return true;
		}
		if (name == PropertyName.companionLimit)
		{
			companionLimit = VariantUtils.ConvertTo<int>(in value);
			return true;
		}
		if (name == PropertyName.spawnerActorTimer)
		{
			spawnerActorTimer = VariantUtils.ConvertTo<Timer>(in value);
			return true;
		}
		if (name == PropertyName.spawnerActorTimerRange)
		{
			spawnerActorTimerRange = VariantUtils.ConvertTo<Vector2>(in value);
			return true;
		}
		if (name == PropertyName.randomAnimationTimer)
		{
			randomAnimationTimer = VariantUtils.ConvertTo<Timer>(in value);
			return true;
		}
		if (name == PropertyName.passivePlayTimer)
		{
			passivePlayTimer = VariantUtils.ConvertTo<Timer>(in value);
			return true;
		}
		if (name == PropertyName.passivePlayTimerOffRange)
		{
			passivePlayTimerOffRange = VariantUtils.ConvertTo<Vector2>(in value);
			return true;
		}
		if (name == PropertyName.passivePlayTimerOnRange)
		{
			passivePlayTimerOnRange = VariantUtils.ConvertTo<Vector2>(in value);
			return true;
		}
		if (name == PropertyName.throwSoftReactionChance)
		{
			throwSoftReactionChance = VariantUtils.ConvertTo<float>(in value);
			return true;
		}
		if (name == PropertyName.throwHardReactionChance)
		{
			throwHardReactionChance = VariantUtils.ConvertTo<float>(in value);
			return true;
		}
		if (name == PropertyName.landSoftReactionChance)
		{
			landSoftReactionChance = VariantUtils.ConvertTo<float>(in value);
			return true;
		}
		if (name == PropertyName.landHardReactionChance)
		{
			landHardReactionChance = VariantUtils.ConvertTo<float>(in value);
			return true;
		}
		if (name == PropertyName.ItemObjectScene)
		{
			ItemObjectScene = VariantUtils.ConvertTo<PackedScene>(in value);
			return true;
		}
		if (name == PropertyName.AttachmentObjectScene)
		{
			AttachmentObjectScene = VariantUtils.ConvertTo<PackedScene>(in value);
			return true;
		}
		if (name == PropertyName.actorScene)
		{
			actorScene = VariantUtils.ConvertTo<PackedScene>(in value);
			return true;
		}
		if (name == PropertyName.pauseMenu)
		{
			pauseMenu = VariantUtils.ConvertTo<PackedScene>(in value);
			return true;
		}
		if (name == PropertyName.terminalMenu)
		{
			terminalMenu = VariantUtils.ConvertTo<PackedScene>(in value);
			return true;
		}
		if (name == PropertyName.confirmationMenu)
		{
			confirmationMenu = VariantUtils.ConvertTo<PackedScene>(in value);
			return true;
		}
		if (name == PropertyName.eulaMenu)
		{
			eulaMenu = VariantUtils.ConvertTo<PackedScene>(in value);
			return true;
		}
		if (name == PropertyName.magnifierScene)
		{
			magnifierScene = VariantUtils.ConvertTo<PackedScene>(in value);
			return true;
		}
		if (name == PropertyName.settingWindowThrowPhysics)
		{
			settingWindowThrowPhysics = VariantUtils.ConvertTo<bool>(in value);
			return true;
		}
		if (name == PropertyName.settingEULA)
		{
			settingEULA = VariantUtils.ConvertTo<bool>(in value);
			return true;
		}
		if (name == PropertyName.settingSpriteScaler)
		{
			settingSpriteScaler = VariantUtils.ConvertTo<float>(in value);
			return true;
		}
		if (name == PropertyName.settingItemScaler)
		{
			settingItemScaler = VariantUtils.ConvertTo<float>(in value);
			return true;
		}
		if (name == PropertyName.settingUIScaler)
		{
			settingUIScaler = VariantUtils.ConvertTo<float>(in value);
			return true;
		}
		if (name == PropertyName.settingSpawnItems)
		{
			settingSpawnItems = VariantUtils.ConvertTo<bool>(in value);
			return true;
		}
		if (name == PropertyName.settingSpawnActors)
		{
			settingSpawnActors = VariantUtils.ConvertTo<bool>(in value);
			return true;
		}
		if (name == PropertyName.settingPassivePlayMode)
		{
			settingPassivePlayMode = VariantUtils.ConvertTo<bool>(in value);
			return true;
		}
		if (name == PropertyName.settingRemovePopups)
		{
			settingRemovePopups = VariantUtils.ConvertTo<bool>(in value);
			return true;
		}
		if (name == PropertyName.settingRemoveConvos)
		{
			settingRemoveConvos = VariantUtils.ConvertTo<bool>(in value);
			return true;
		}
		if (name == PropertyName.settingAudioOn)
		{
			settingAudioOn = VariantUtils.ConvertTo<bool>(in value);
			return true;
		}
		if (name == PropertyName.settingMods)
		{
			settingMods = VariantUtils.ConvertTo<bool>(in value);
			return true;
		}
		if (name == PropertyName.settingEnabledMods)
		{
			settingEnabledMods = VariantUtils.ConvertToArray<string>(in value);
			return true;
		}
		if (name == PropertyName.userInfoName)
		{
			userInfoName = VariantUtils.ConvertTo<string>(in value);
			return true;
		}
		if (name == PropertyName.SeenObjects)
		{
			SeenObjects = VariantUtils.ConvertToDictionary<SaveHandler.SeenObjectTypes, Array<string>>(in value);
			return true;
		}
		if (name == PropertyName.userTickets)
		{
			userTickets = VariantUtils.ConvertTo<int>(in value);
			return true;
		}
		if (name == PropertyName.minigameData)
		{
			minigameData = VariantUtils.ConvertToDictionary<string, Variant>(in value);
			return true;
		}
		if (name == PropertyName.settingBlacklistedContent)
		{
			settingBlacklistedContent = VariantUtils.ConvertToArray<SaveHandler.Kinks>(in value);
			return true;
		}
		if (name == PropertyName.mouseOffset)
		{
			mouseOffset = VariantUtils.ConvertTo<Vector2>(in value);
			return true;
		}
		if (name == PropertyName.selected)
		{
			selected = VariantUtils.ConvertTo<bool>(in value);
			return true;
		}
		if (name == PropertyName.SomethingHasBeenGrabbed)
		{
			SomethingHasBeenGrabbed = VariantUtils.ConvertTo<bool>(in value);
			return true;
		}
		if (name == PropertyName.grabReleaseTween)
		{
			grabReleaseTween = VariantUtils.ConvertTo<Tween>(in value);
			return true;
		}
		if (name == PropertyName.screenDataHandler)
		{
			screenDataHandler = VariantUtils.ConvertTo<ScreenDataHandler>(in value);
			return true;
		}
		if (name == PropertyName.isWalking)
		{
			isWalking = VariantUtils.ConvertTo<bool>(in value);
			return true;
		}
		if (name == PropertyName.walkDirection)
		{
			walkDirection = VariantUtils.ConvertTo<int>(in value);
			return true;
		}
		if (name == PropertyName.pickupWatchdogTimer)
		{
			pickupWatchdogTimer = VariantUtils.ConvertTo<float>(in value);
			return true;
		}
		if (name == PropertyName.mouseVelocityScaler)
		{
			mouseVelocityScaler = VariantUtils.ConvertTo<float>(in value);
			return true;
		}
		if (name == PropertyName.windowBounceDamping)
		{
			windowBounceDamping = VariantUtils.ConvertTo<float>(in value);
			return true;
		}
		if (name == PropertyName.windowAirResist)
		{
			windowAirResist = VariantUtils.ConvertTo<float>(in value);
			return true;
		}
		if (name == PropertyName.windowVelocity)
		{
			windowVelocity = VariantUtils.ConvertTo<Vector2>(in value);
			return true;
		}
		if (name == PropertyName.isThrown)
		{
			isThrown = VariantUtils.ConvertTo<bool>(in value);
			return true;
		}
		if (name == PropertyName.isWalkingToTargetPosition)
		{
			isWalkingToTargetPosition = VariantUtils.ConvertTo<bool>(in value);
			return true;
		}
		if (name == PropertyName.targetX)
		{
			targetX = VariantUtils.ConvertTo<int>(in value);
			return true;
		}
		if (name == PropertyName.walkTargetItem)
		{
			walkTargetItem = VariantUtils.ConvertTo<ItemWindow>(in value);
			return true;
		}
		if (name == PropertyName.storedItem)
		{
			storedItem = VariantUtils.ConvertTo<ItemWindow>(in value);
			return true;
		}
		if (name == PropertyName.inPickup)
		{
			inPickup = VariantUtils.ConvertTo<bool>(in value);
			return true;
		}
		if (name == PropertyName.isInConvo)
		{
			isInConvo = VariantUtils.ConvertTo<bool>(in value);
			return true;
		}
		if (name == PropertyName.spawnedMinigames)
		{
			spawnedMinigames = VariantUtils.ConvertToArray<Window>(in value);
			return true;
		}
		if (name == PropertyName.Pause)
		{
			Pause = VariantUtils.ConvertTo<PauseMenu>(in value);
			return true;
		}
		if (name == PropertyName.Terminal)
		{
			Terminal = VariantUtils.ConvertTo<TerminalWindow>(in value);
			return true;
		}
		if (name == PropertyName.AdminAccess)
		{
			AdminAccess = VariantUtils.ConvertTo<bool>(in value);
			return true;
		}
		if (name == PropertyName.Magnifier)
		{
			Magnifier = VariantUtils.ConvertTo<MagnifierWindow>(in value);
			return true;
		}
		if (name == PropertyName._magnifierActive)
		{
			_magnifierActive = VariantUtils.ConvertTo<bool>(in value);
			return true;
		}
		return base.SetGodotClassPropertyValue(in name, in value);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override bool GetGodotClassPropertyValue(in godot_string_name name, out godot_variant value)
	{
		if (name == PropertyName.mainWindow)
		{
			value = VariantUtils.CreateFrom(in mainWindow);
			return true;
		}
		if (name == PropertyName.mainCharacter)
		{
			value = VariantUtils.CreateFrom(in mainCharacter);
			return true;
		}
		if (name == PropertyName.saveHandler)
		{
			value = VariantUtils.CreateFrom(in saveHandler);
			return true;
		}
		if (name == PropertyName.spawnedItems)
		{
			value = VariantUtils.CreateFromArray(spawnedItems);
			return true;
		}
		if (name == PropertyName.maxItems)
		{
			value = VariantUtils.CreateFrom(in maxItems);
			return true;
		}
		if (name == PropertyName.spawnMargin)
		{
			value = VariantUtils.CreateFrom(in spawnMargin);
			return true;
		}
		if (name == PropertyName.spawnerTimer)
		{
			value = VariantUtils.CreateFrom(in spawnerTimer);
			return true;
		}
		if (name == PropertyName.spawnerTimeRange)
		{
			value = VariantUtils.CreateFrom(in spawnerTimeRange);
			return true;
		}
		if (name == PropertyName.spawnedAttachments)
		{
			value = VariantUtils.CreateFromArray(spawnedAttachments);
			return true;
		}
		if (name == PropertyName.dialogueStack)
		{
			value = VariantUtils.CreateFromArray(dialogueStack);
			return true;
		}
		if (name == PropertyName.timeBetweenStacks)
		{
			value = VariantUtils.CreateFrom(in timeBetweenStacks);
			return true;
		}
		if (name == PropertyName.defaultTextDataRes)
		{
			value = VariantUtils.CreateFrom(in defaultTextDataRes);
			return true;
		}
		if (name == PropertyName.randomDialogueTimer)
		{
			value = VariantUtils.CreateFrom(in randomDialogueTimer);
			return true;
		}
		if (name == PropertyName.spawnedActors)
		{
			value = VariantUtils.CreateFromArray(spawnedActors);
			return true;
		}
		if (name == PropertyName.spawnedCompanions)
		{
			value = VariantUtils.CreateFromArray(spawnedCompanions);
			return true;
		}
		if (name == PropertyName.companionLimit)
		{
			value = VariantUtils.CreateFrom(in companionLimit);
			return true;
		}
		if (name == PropertyName.spawnerActorTimer)
		{
			value = VariantUtils.CreateFrom(in spawnerActorTimer);
			return true;
		}
		if (name == PropertyName.spawnerActorTimerRange)
		{
			value = VariantUtils.CreateFrom(in spawnerActorTimerRange);
			return true;
		}
		if (name == PropertyName.randomAnimationTimer)
		{
			value = VariantUtils.CreateFrom(in randomAnimationTimer);
			return true;
		}
		if (name == PropertyName.passivePlayTimer)
		{
			value = VariantUtils.CreateFrom(in passivePlayTimer);
			return true;
		}
		if (name == PropertyName.passivePlayTimerOffRange)
		{
			value = VariantUtils.CreateFrom(in passivePlayTimerOffRange);
			return true;
		}
		if (name == PropertyName.passivePlayTimerOnRange)
		{
			value = VariantUtils.CreateFrom(in passivePlayTimerOnRange);
			return true;
		}
		if (name == PropertyName.throwSoftReactionChance)
		{
			value = VariantUtils.CreateFrom(in throwSoftReactionChance);
			return true;
		}
		if (name == PropertyName.throwHardReactionChance)
		{
			value = VariantUtils.CreateFrom(in throwHardReactionChance);
			return true;
		}
		if (name == PropertyName.landSoftReactionChance)
		{
			value = VariantUtils.CreateFrom(in landSoftReactionChance);
			return true;
		}
		if (name == PropertyName.landHardReactionChance)
		{
			value = VariantUtils.CreateFrom(in landHardReactionChance);
			return true;
		}
		if (name == PropertyName.ItemObjectScene)
		{
			value = VariantUtils.CreateFrom(in ItemObjectScene);
			return true;
		}
		if (name == PropertyName.AttachmentObjectScene)
		{
			value = VariantUtils.CreateFrom(in AttachmentObjectScene);
			return true;
		}
		if (name == PropertyName.actorScene)
		{
			value = VariantUtils.CreateFrom(in actorScene);
			return true;
		}
		if (name == PropertyName.pauseMenu)
		{
			value = VariantUtils.CreateFrom(in pauseMenu);
			return true;
		}
		if (name == PropertyName.terminalMenu)
		{
			value = VariantUtils.CreateFrom(in terminalMenu);
			return true;
		}
		if (name == PropertyName.confirmationMenu)
		{
			value = VariantUtils.CreateFrom(in confirmationMenu);
			return true;
		}
		if (name == PropertyName.eulaMenu)
		{
			value = VariantUtils.CreateFrom(in eulaMenu);
			return true;
		}
		if (name == PropertyName.magnifierScene)
		{
			value = VariantUtils.CreateFrom(in magnifierScene);
			return true;
		}
		if (name == PropertyName.settingWindowThrowPhysics)
		{
			value = VariantUtils.CreateFrom(in settingWindowThrowPhysics);
			return true;
		}
		if (name == PropertyName.settingEULA)
		{
			value = VariantUtils.CreateFrom(in settingEULA);
			return true;
		}
		if (name == PropertyName.settingSpriteScaler)
		{
			value = VariantUtils.CreateFrom(in settingSpriteScaler);
			return true;
		}
		if (name == PropertyName.settingItemScaler)
		{
			value = VariantUtils.CreateFrom(in settingItemScaler);
			return true;
		}
		if (name == PropertyName.settingUIScaler)
		{
			value = VariantUtils.CreateFrom(in settingUIScaler);
			return true;
		}
		if (name == PropertyName.settingSpawnItems)
		{
			value = VariantUtils.CreateFrom(in settingSpawnItems);
			return true;
		}
		if (name == PropertyName.settingSpawnActors)
		{
			value = VariantUtils.CreateFrom(in settingSpawnActors);
			return true;
		}
		if (name == PropertyName.settingPassivePlayMode)
		{
			value = VariantUtils.CreateFrom(in settingPassivePlayMode);
			return true;
		}
		if (name == PropertyName.settingRemovePopups)
		{
			value = VariantUtils.CreateFrom(in settingRemovePopups);
			return true;
		}
		if (name == PropertyName.settingRemoveConvos)
		{
			value = VariantUtils.CreateFrom(in settingRemoveConvos);
			return true;
		}
		if (name == PropertyName.settingAudioOn)
		{
			value = VariantUtils.CreateFrom(in settingAudioOn);
			return true;
		}
		if (name == PropertyName.settingMods)
		{
			value = VariantUtils.CreateFrom(in settingMods);
			return true;
		}
		if (name == PropertyName.settingEnabledMods)
		{
			value = VariantUtils.CreateFromArray(settingEnabledMods);
			return true;
		}
		if (name == PropertyName.userInfoName)
		{
			value = VariantUtils.CreateFrom(in userInfoName);
			return true;
		}
		if (name == PropertyName.SeenObjects)
		{
			value = VariantUtils.CreateFromDictionary(SeenObjects);
			return true;
		}
		if (name == PropertyName.userTickets)
		{
			value = VariantUtils.CreateFrom(in userTickets);
			return true;
		}
		if (name == PropertyName.minigameData)
		{
			value = VariantUtils.CreateFromDictionary(minigameData);
			return true;
		}
		if (name == PropertyName.settingBlacklistedContent)
		{
			value = VariantUtils.CreateFromArray(settingBlacklistedContent);
			return true;
		}
		if (name == PropertyName.mouseOffset)
		{
			value = VariantUtils.CreateFrom(in mouseOffset);
			return true;
		}
		if (name == PropertyName.selected)
		{
			value = VariantUtils.CreateFrom(in selected);
			return true;
		}
		if (name == PropertyName.SomethingHasBeenGrabbed)
		{
			value = VariantUtils.CreateFrom(in SomethingHasBeenGrabbed);
			return true;
		}
		if (name == PropertyName.grabReleaseTween)
		{
			value = VariantUtils.CreateFrom(in grabReleaseTween);
			return true;
		}
		if (name == PropertyName.screenDataHandler)
		{
			value = VariantUtils.CreateFrom(in screenDataHandler);
			return true;
		}
		if (name == PropertyName.isWalking)
		{
			value = VariantUtils.CreateFrom(in isWalking);
			return true;
		}
		if (name == PropertyName.walkDirection)
		{
			value = VariantUtils.CreateFrom(in walkDirection);
			return true;
		}
		if (name == PropertyName.pickupWatchdogTimer)
		{
			value = VariantUtils.CreateFrom(in pickupWatchdogTimer);
			return true;
		}
		if (name == PropertyName.mouseVelocityScaler)
		{
			value = VariantUtils.CreateFrom(in mouseVelocityScaler);
			return true;
		}
		if (name == PropertyName.windowBounceDamping)
		{
			value = VariantUtils.CreateFrom(in windowBounceDamping);
			return true;
		}
		if (name == PropertyName.windowAirResist)
		{
			value = VariantUtils.CreateFrom(in windowAirResist);
			return true;
		}
		if (name == PropertyName.windowVelocity)
		{
			value = VariantUtils.CreateFrom(in windowVelocity);
			return true;
		}
		if (name == PropertyName.isThrown)
		{
			value = VariantUtils.CreateFrom(in isThrown);
			return true;
		}
		if (name == PropertyName.isWalkingToTargetPosition)
		{
			value = VariantUtils.CreateFrom(in isWalkingToTargetPosition);
			return true;
		}
		if (name == PropertyName.targetX)
		{
			value = VariantUtils.CreateFrom(in targetX);
			return true;
		}
		if (name == PropertyName.walkTargetItem)
		{
			value = VariantUtils.CreateFrom(in walkTargetItem);
			return true;
		}
		if (name == PropertyName.storedItem)
		{
			value = VariantUtils.CreateFrom(in storedItem);
			return true;
		}
		if (name == PropertyName.inPickup)
		{
			value = VariantUtils.CreateFrom(in inPickup);
			return true;
		}
		if (name == PropertyName.isInConvo)
		{
			value = VariantUtils.CreateFrom(in isInConvo);
			return true;
		}
		if (name == PropertyName.spawnedMinigames)
		{
			value = VariantUtils.CreateFromArray(spawnedMinigames);
			return true;
		}
		if (name == PropertyName.Pause)
		{
			value = VariantUtils.CreateFrom(in Pause);
			return true;
		}
		if (name == PropertyName.Terminal)
		{
			value = VariantUtils.CreateFrom(in Terminal);
			return true;
		}
		if (name == PropertyName.AdminAccess)
		{
			value = VariantUtils.CreateFrom(in AdminAccess);
			return true;
		}
		if (name == PropertyName.Magnifier)
		{
			value = VariantUtils.CreateFrom(in Magnifier);
			return true;
		}
		if (name == PropertyName._magnifierActive)
		{
			value = VariantUtils.CreateFrom(in _magnifierActive);
			return true;
		}
		return base.GetGodotClassPropertyValue(in name, out value);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	internal static List<PropertyInfo> GetGodotPropertyList()
	{
		return new List<PropertyInfo>
		{
			new PropertyInfo(Variant.Type.Object, PropertyName.mainWindow, PropertyHint.None, "", PropertyUsageFlags.ScriptVariable, exported: false),
			new PropertyInfo(Variant.Type.Object, PropertyName.mainCharacter, PropertyHint.NodeType, "Node2D", PropertyUsageFlags.Default | PropertyUsageFlags.ScriptVariable, exported: true),
			new PropertyInfo(Variant.Type.Object, PropertyName.saveHandler, PropertyHint.NodeType, "Node", PropertyUsageFlags.Default | PropertyUsageFlags.ScriptVariable, exported: true),
			new PropertyInfo(Variant.Type.Nil, "Item Spawner Logic", PropertyHint.None, "", PropertyUsageFlags.Group, exported: true),
			new PropertyInfo(Variant.Type.Array, PropertyName.spawnedItems, PropertyHint.None, "", PropertyUsageFlags.ScriptVariable, exported: false),
			new PropertyInfo(Variant.Type.Int, PropertyName.maxItems, PropertyHint.None, "", PropertyUsageFlags.Default | PropertyUsageFlags.ScriptVariable, exported: true),
			new PropertyInfo(Variant.Type.Vector2, PropertyName.spawnMargin, PropertyHint.None, "", PropertyUsageFlags.Default | PropertyUsageFlags.ScriptVariable, exported: true),
			new PropertyInfo(Variant.Type.Object, PropertyName.spawnerTimer, PropertyHint.NodeType, "Timer", PropertyUsageFlags.Default | PropertyUsageFlags.ScriptVariable, exported: true),
			new PropertyInfo(Variant.Type.Vector2, PropertyName.spawnerTimeRange, PropertyHint.None, "", PropertyUsageFlags.Default | PropertyUsageFlags.ScriptVariable, exported: true),
			new PropertyInfo(Variant.Type.Nil, "Attachment Logic", PropertyHint.None, "", PropertyUsageFlags.Subgroup, exported: true),
			new PropertyInfo(Variant.Type.Array, PropertyName.spawnedAttachments, PropertyHint.None, "", PropertyUsageFlags.ScriptVariable, exported: false),
			new PropertyInfo(Variant.Type.Nil, "Dialogue Logic", PropertyHint.None, "", PropertyUsageFlags.Subgroup, exported: true),
			new PropertyInfo(Variant.Type.Array, PropertyName.dialogueStack, PropertyHint.None, "", PropertyUsageFlags.ScriptVariable, exported: false),
			new PropertyInfo(Variant.Type.Vector2, PropertyName.timeBetweenStacks, PropertyHint.None, "", PropertyUsageFlags.Default | PropertyUsageFlags.ScriptVariable, exported: true),
			new PropertyInfo(Variant.Type.Object, PropertyName.defaultTextDataRes, PropertyHint.ResourceType, "AttachDataRes", PropertyUsageFlags.Default | PropertyUsageFlags.ScriptVariable, exported: true),
			new PropertyInfo(Variant.Type.Object, PropertyName.randomDialogueTimer, PropertyHint.NodeType, "Timer", PropertyUsageFlags.Default | PropertyUsageFlags.ScriptVariable, exported: true),
			new PropertyInfo(Variant.Type.Nil, "Actor Spawner Logic", PropertyHint.None, "", PropertyUsageFlags.Group, exported: true),
			new PropertyInfo(Variant.Type.Array, PropertyName.spawnedActors, PropertyHint.None, "", PropertyUsageFlags.ScriptVariable, exported: false),
			new PropertyInfo(Variant.Type.Array, PropertyName.spawnedCompanions, PropertyHint.None, "", PropertyUsageFlags.ScriptVariable, exported: false),
			new PropertyInfo(Variant.Type.Int, PropertyName.companionLimit, PropertyHint.None, "", PropertyUsageFlags.Default | PropertyUsageFlags.ScriptVariable, exported: true),
			new PropertyInfo(Variant.Type.Object, PropertyName.spawnerActorTimer, PropertyHint.NodeType, "Timer", PropertyUsageFlags.Default | PropertyUsageFlags.ScriptVariable, exported: true),
			new PropertyInfo(Variant.Type.Vector2, PropertyName.spawnerActorTimerRange, PropertyHint.None, "", PropertyUsageFlags.Default | PropertyUsageFlags.ScriptVariable, exported: true),
			new PropertyInfo(Variant.Type.Nil, "Random Animation Logic", PropertyHint.None, "", PropertyUsageFlags.Group, exported: true),
			new PropertyInfo(Variant.Type.Object, PropertyName.randomAnimationTimer, PropertyHint.NodeType, "Timer", PropertyUsageFlags.Default | PropertyUsageFlags.ScriptVariable, exported: true),
			new PropertyInfo(Variant.Type.Nil, "Passive Play Logic", PropertyHint.None, "", PropertyUsageFlags.Group, exported: true),
			new PropertyInfo(Variant.Type.Object, PropertyName.passivePlayTimer, PropertyHint.NodeType, "Timer", PropertyUsageFlags.Default | PropertyUsageFlags.ScriptVariable, exported: true),
			new PropertyInfo(Variant.Type.Vector2, PropertyName.passivePlayTimerOffRange, PropertyHint.None, "", PropertyUsageFlags.Default | PropertyUsageFlags.ScriptVariable, exported: true),
			new PropertyInfo(Variant.Type.Vector2, PropertyName.passivePlayTimerOnRange, PropertyHint.None, "", PropertyUsageFlags.Default | PropertyUsageFlags.ScriptVariable, exported: true),
			new PropertyInfo(Variant.Type.Nil, "Reaction Dialogue Chances", PropertyHint.None, "", PropertyUsageFlags.Group, exported: true),
			new PropertyInfo(Variant.Type.Float, PropertyName.throwSoftReactionChance, PropertyHint.None, "", PropertyUsageFlags.Default | PropertyUsageFlags.ScriptVariable, exported: true),
			new PropertyInfo(Variant.Type.Float, PropertyName.throwHardReactionChance, PropertyHint.None, "", PropertyUsageFlags.Default | PropertyUsageFlags.ScriptVariable, exported: true),
			new PropertyInfo(Variant.Type.Float, PropertyName.landSoftReactionChance, PropertyHint.None, "", PropertyUsageFlags.Default | PropertyUsageFlags.ScriptVariable, exported: true),
			new PropertyInfo(Variant.Type.Float, PropertyName.landHardReactionChance, PropertyHint.None, "", PropertyUsageFlags.Default | PropertyUsageFlags.ScriptVariable, exported: true),
			new PropertyInfo(Variant.Type.Nil, "Reference UIDs", PropertyHint.None, "", PropertyUsageFlags.Group, exported: true),
			new PropertyInfo(Variant.Type.Object, PropertyName.ItemObjectScene, PropertyHint.ResourceType, "PackedScene", PropertyUsageFlags.Default | PropertyUsageFlags.ScriptVariable, exported: true),
			new PropertyInfo(Variant.Type.Object, PropertyName.AttachmentObjectScene, PropertyHint.ResourceType, "PackedScene", PropertyUsageFlags.Default | PropertyUsageFlags.ScriptVariable, exported: true),
			new PropertyInfo(Variant.Type.Object, PropertyName.actorScene, PropertyHint.ResourceType, "PackedScene", PropertyUsageFlags.Default | PropertyUsageFlags.ScriptVariable, exported: true),
			new PropertyInfo(Variant.Type.Object, PropertyName.pauseMenu, PropertyHint.ResourceType, "PackedScene", PropertyUsageFlags.Default | PropertyUsageFlags.ScriptVariable, exported: true),
			new PropertyInfo(Variant.Type.Object, PropertyName.terminalMenu, PropertyHint.ResourceType, "PackedScene", PropertyUsageFlags.Default | PropertyUsageFlags.ScriptVariable, exported: true),
			new PropertyInfo(Variant.Type.Object, PropertyName.confirmationMenu, PropertyHint.ResourceType, "PackedScene", PropertyUsageFlags.Default | PropertyUsageFlags.ScriptVariable, exported: true),
			new PropertyInfo(Variant.Type.Object, PropertyName.eulaMenu, PropertyHint.ResourceType, "PackedScene", PropertyUsageFlags.Default | PropertyUsageFlags.ScriptVariable, exported: true),
			new PropertyInfo(Variant.Type.Object, PropertyName.magnifierScene, PropertyHint.ResourceType, "PackedScene", PropertyUsageFlags.Default | PropertyUsageFlags.ScriptVariable, exported: true),
			new PropertyInfo(Variant.Type.Nil, "Internal Settings", PropertyHint.None, "", PropertyUsageFlags.Group, exported: true),
			new PropertyInfo(Variant.Type.Bool, PropertyName.settingWindowThrowPhysics, PropertyHint.None, "", PropertyUsageFlags.Default | PropertyUsageFlags.ScriptVariable, exported: true),
			new PropertyInfo(Variant.Type.Bool, PropertyName.settingEULA, PropertyHint.None, "", PropertyUsageFlags.ScriptVariable, exported: false),
			new PropertyInfo(Variant.Type.Float, PropertyName.settingSpriteScaler, PropertyHint.None, "", PropertyUsageFlags.ScriptVariable, exported: false),
			new PropertyInfo(Variant.Type.Float, PropertyName.settingItemScaler, PropertyHint.None, "", PropertyUsageFlags.ScriptVariable, exported: false),
			new PropertyInfo(Variant.Type.Float, PropertyName.settingUIScaler, PropertyHint.None, "", PropertyUsageFlags.ScriptVariable, exported: false),
			new PropertyInfo(Variant.Type.Bool, PropertyName.settingSpawnItems, PropertyHint.None, "", PropertyUsageFlags.ScriptVariable, exported: false),
			new PropertyInfo(Variant.Type.Bool, PropertyName.settingSpawnActors, PropertyHint.None, "", PropertyUsageFlags.ScriptVariable, exported: false),
			new PropertyInfo(Variant.Type.Bool, PropertyName.settingPassivePlayMode, PropertyHint.None, "", PropertyUsageFlags.ScriptVariable, exported: false),
			new PropertyInfo(Variant.Type.Bool, PropertyName.settingRemovePopups, PropertyHint.None, "", PropertyUsageFlags.ScriptVariable, exported: false),
			new PropertyInfo(Variant.Type.Bool, PropertyName.settingRemoveConvos, PropertyHint.None, "", PropertyUsageFlags.ScriptVariable, exported: false),
			new PropertyInfo(Variant.Type.Bool, PropertyName.settingAudioOn, PropertyHint.None, "", PropertyUsageFlags.ScriptVariable, exported: false),
			new PropertyInfo(Variant.Type.Bool, PropertyName.settingMods, PropertyHint.None, "", PropertyUsageFlags.ScriptVariable, exported: false),
			new PropertyInfo(Variant.Type.Array, PropertyName.settingEnabledMods, PropertyHint.None, "", PropertyUsageFlags.ScriptVariable, exported: false),
			new PropertyInfo(Variant.Type.String, PropertyName.userInfoName, PropertyHint.None, "", PropertyUsageFlags.ScriptVariable, exported: false),
			new PropertyInfo(Variant.Type.Dictionary, PropertyName.SeenObjects, PropertyHint.None, "", PropertyUsageFlags.ScriptVariable, exported: false),
			new PropertyInfo(Variant.Type.Int, PropertyName.userTickets, PropertyHint.None, "", PropertyUsageFlags.ScriptVariable, exported: false),
			new PropertyInfo(Variant.Type.Dictionary, PropertyName.minigameData, PropertyHint.None, "", PropertyUsageFlags.ScriptVariable, exported: false),
			new PropertyInfo(Variant.Type.Array, PropertyName.settingBlacklistedContent, PropertyHint.None, "", PropertyUsageFlags.ScriptVariable, exported: false),
			new PropertyInfo(Variant.Type.Vector2, PropertyName.mouseOffset, PropertyHint.None, "", PropertyUsageFlags.ScriptVariable, exported: false),
			new PropertyInfo(Variant.Type.Bool, PropertyName.selected, PropertyHint.None, "", PropertyUsageFlags.ScriptVariable, exported: false),
			new PropertyInfo(Variant.Type.Bool, PropertyName.SomethingHasBeenGrabbed, PropertyHint.None, "", PropertyUsageFlags.ScriptVariable, exported: false),
			new PropertyInfo(Variant.Type.Object, PropertyName.grabReleaseTween, PropertyHint.None, "", PropertyUsageFlags.ScriptVariable, exported: false),
			new PropertyInfo(Variant.Type.Object, PropertyName.screenDataHandler, PropertyHint.None, "", PropertyUsageFlags.ScriptVariable, exported: false),
			new PropertyInfo(Variant.Type.Bool, PropertyName.isWalking, PropertyHint.None, "", PropertyUsageFlags.ScriptVariable, exported: false),
			new PropertyInfo(Variant.Type.Int, PropertyName.walkDirection, PropertyHint.None, "", PropertyUsageFlags.ScriptVariable, exported: false),
			new PropertyInfo(Variant.Type.Float, PropertyName.pickupWatchdogTimer, PropertyHint.None, "", PropertyUsageFlags.ScriptVariable, exported: false),
			new PropertyInfo(Variant.Type.Float, PropertyName.mouseVelocityScaler, PropertyHint.None, "", PropertyUsageFlags.ScriptVariable, exported: false),
			new PropertyInfo(Variant.Type.Float, PropertyName.windowBounceDamping, PropertyHint.None, "", PropertyUsageFlags.ScriptVariable, exported: false),
			new PropertyInfo(Variant.Type.Float, PropertyName.windowAirResist, PropertyHint.None, "", PropertyUsageFlags.ScriptVariable, exported: false),
			new PropertyInfo(Variant.Type.Vector2, PropertyName.windowVelocity, PropertyHint.None, "", PropertyUsageFlags.ScriptVariable, exported: false),
			new PropertyInfo(Variant.Type.Bool, PropertyName.isThrown, PropertyHint.None, "", PropertyUsageFlags.ScriptVariable, exported: false),
			new PropertyInfo(Variant.Type.Bool, PropertyName.isWalkingToTargetPosition, PropertyHint.None, "", PropertyUsageFlags.ScriptVariable, exported: false),
			new PropertyInfo(Variant.Type.Int, PropertyName.targetX, PropertyHint.None, "", PropertyUsageFlags.ScriptVariable, exported: false),
			new PropertyInfo(Variant.Type.Object, PropertyName.walkTargetItem, PropertyHint.None, "", PropertyUsageFlags.ScriptVariable, exported: false),
			new PropertyInfo(Variant.Type.Object, PropertyName.storedItem, PropertyHint.None, "", PropertyUsageFlags.ScriptVariable, exported: false),
			new PropertyInfo(Variant.Type.Bool, PropertyName.inPickup, PropertyHint.None, "", PropertyUsageFlags.ScriptVariable, exported: false),
			new PropertyInfo(Variant.Type.Bool, PropertyName.isInConvo, PropertyHint.None, "", PropertyUsageFlags.ScriptVariable, exported: false),
			new PropertyInfo(Variant.Type.Array, PropertyName.spawnedMinigames, PropertyHint.None, "", PropertyUsageFlags.ScriptVariable, exported: false),
			new PropertyInfo(Variant.Type.Object, PropertyName.Pause, PropertyHint.None, "", PropertyUsageFlags.ScriptVariable, exported: false),
			new PropertyInfo(Variant.Type.Object, PropertyName.Terminal, PropertyHint.None, "", PropertyUsageFlags.ScriptVariable, exported: false),
			new PropertyInfo(Variant.Type.Bool, PropertyName.AdminAccess, PropertyHint.None, "", PropertyUsageFlags.ScriptVariable, exported: false),
			new PropertyInfo(Variant.Type.Object, PropertyName.Magnifier, PropertyHint.None, "", PropertyUsageFlags.ScriptVariable, exported: false),
			new PropertyInfo(Variant.Type.Bool, PropertyName._magnifierActive, PropertyHint.None, "", PropertyUsageFlags.ScriptVariable, exported: false)
		};
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override void SaveGodotObjectData(GodotSerializationInfo info)
	{
		base.SaveGodotObjectData(info);
		info.AddProperty(PropertyName.mainWindow, Variant.From(in mainWindow));
		info.AddProperty(PropertyName.mainCharacter, Variant.From(in mainCharacter));
		info.AddProperty(PropertyName.saveHandler, Variant.From(in saveHandler));
		info.AddProperty(PropertyName.spawnedItems, Variant.CreateFrom(spawnedItems));
		info.AddProperty(PropertyName.maxItems, Variant.From(in maxItems));
		info.AddProperty(PropertyName.spawnMargin, Variant.From(in spawnMargin));
		info.AddProperty(PropertyName.spawnerTimer, Variant.From(in spawnerTimer));
		info.AddProperty(PropertyName.spawnerTimeRange, Variant.From(in spawnerTimeRange));
		info.AddProperty(PropertyName.spawnedAttachments, Variant.CreateFrom(spawnedAttachments));
		info.AddProperty(PropertyName.dialogueStack, Variant.CreateFrom(dialogueStack));
		info.AddProperty(PropertyName.timeBetweenStacks, Variant.From(in timeBetweenStacks));
		info.AddProperty(PropertyName.defaultTextDataRes, Variant.From(in defaultTextDataRes));
		info.AddProperty(PropertyName.randomDialogueTimer, Variant.From(in randomDialogueTimer));
		info.AddProperty(PropertyName.spawnedActors, Variant.CreateFrom(spawnedActors));
		info.AddProperty(PropertyName.spawnedCompanions, Variant.CreateFrom(spawnedCompanions));
		info.AddProperty(PropertyName.companionLimit, Variant.From(in companionLimit));
		info.AddProperty(PropertyName.spawnerActorTimer, Variant.From(in spawnerActorTimer));
		info.AddProperty(PropertyName.spawnerActorTimerRange, Variant.From(in spawnerActorTimerRange));
		info.AddProperty(PropertyName.randomAnimationTimer, Variant.From(in randomAnimationTimer));
		info.AddProperty(PropertyName.passivePlayTimer, Variant.From(in passivePlayTimer));
		info.AddProperty(PropertyName.passivePlayTimerOffRange, Variant.From(in passivePlayTimerOffRange));
		info.AddProperty(PropertyName.passivePlayTimerOnRange, Variant.From(in passivePlayTimerOnRange));
		info.AddProperty(PropertyName.throwSoftReactionChance, Variant.From(in throwSoftReactionChance));
		info.AddProperty(PropertyName.throwHardReactionChance, Variant.From(in throwHardReactionChance));
		info.AddProperty(PropertyName.landSoftReactionChance, Variant.From(in landSoftReactionChance));
		info.AddProperty(PropertyName.landHardReactionChance, Variant.From(in landHardReactionChance));
		info.AddProperty(PropertyName.ItemObjectScene, Variant.From(in ItemObjectScene));
		info.AddProperty(PropertyName.AttachmentObjectScene, Variant.From(in AttachmentObjectScene));
		info.AddProperty(PropertyName.actorScene, Variant.From(in actorScene));
		info.AddProperty(PropertyName.pauseMenu, Variant.From(in pauseMenu));
		info.AddProperty(PropertyName.terminalMenu, Variant.From(in terminalMenu));
		info.AddProperty(PropertyName.confirmationMenu, Variant.From(in confirmationMenu));
		info.AddProperty(PropertyName.eulaMenu, Variant.From(in eulaMenu));
		info.AddProperty(PropertyName.magnifierScene, Variant.From(in magnifierScene));
		info.AddProperty(PropertyName.settingWindowThrowPhysics, Variant.From(in settingWindowThrowPhysics));
		info.AddProperty(PropertyName.settingEULA, Variant.From(in settingEULA));
		info.AddProperty(PropertyName.settingSpriteScaler, Variant.From(in settingSpriteScaler));
		info.AddProperty(PropertyName.settingItemScaler, Variant.From(in settingItemScaler));
		info.AddProperty(PropertyName.settingUIScaler, Variant.From(in settingUIScaler));
		info.AddProperty(PropertyName.settingSpawnItems, Variant.From(in settingSpawnItems));
		info.AddProperty(PropertyName.settingSpawnActors, Variant.From(in settingSpawnActors));
		info.AddProperty(PropertyName.settingPassivePlayMode, Variant.From(in settingPassivePlayMode));
		info.AddProperty(PropertyName.settingRemovePopups, Variant.From(in settingRemovePopups));
		info.AddProperty(PropertyName.settingRemoveConvos, Variant.From(in settingRemoveConvos));
		info.AddProperty(PropertyName.settingAudioOn, Variant.From(in settingAudioOn));
		info.AddProperty(PropertyName.settingMods, Variant.From(in settingMods));
		info.AddProperty(PropertyName.settingEnabledMods, Variant.CreateFrom(settingEnabledMods));
		info.AddProperty(PropertyName.userInfoName, Variant.From(in userInfoName));
		info.AddProperty(PropertyName.SeenObjects, Variant.CreateFrom(SeenObjects));
		info.AddProperty(PropertyName.userTickets, Variant.From(in userTickets));
		info.AddProperty(PropertyName.minigameData, Variant.CreateFrom(minigameData));
		info.AddProperty(PropertyName.settingBlacklistedContent, Variant.CreateFrom(settingBlacklistedContent));
		info.AddProperty(PropertyName.mouseOffset, Variant.From(in mouseOffset));
		info.AddProperty(PropertyName.selected, Variant.From(in selected));
		info.AddProperty(PropertyName.SomethingHasBeenGrabbed, Variant.From(in SomethingHasBeenGrabbed));
		info.AddProperty(PropertyName.grabReleaseTween, Variant.From(in grabReleaseTween));
		info.AddProperty(PropertyName.screenDataHandler, Variant.From(in screenDataHandler));
		info.AddProperty(PropertyName.isWalking, Variant.From(in isWalking));
		info.AddProperty(PropertyName.walkDirection, Variant.From(in walkDirection));
		info.AddProperty(PropertyName.pickupWatchdogTimer, Variant.From(in pickupWatchdogTimer));
		info.AddProperty(PropertyName.mouseVelocityScaler, Variant.From(in mouseVelocityScaler));
		info.AddProperty(PropertyName.windowBounceDamping, Variant.From(in windowBounceDamping));
		info.AddProperty(PropertyName.windowAirResist, Variant.From(in windowAirResist));
		info.AddProperty(PropertyName.windowVelocity, Variant.From(in windowVelocity));
		info.AddProperty(PropertyName.isThrown, Variant.From(in isThrown));
		info.AddProperty(PropertyName.isWalkingToTargetPosition, Variant.From(in isWalkingToTargetPosition));
		info.AddProperty(PropertyName.targetX, Variant.From(in targetX));
		info.AddProperty(PropertyName.walkTargetItem, Variant.From(in walkTargetItem));
		info.AddProperty(PropertyName.storedItem, Variant.From(in storedItem));
		info.AddProperty(PropertyName.inPickup, Variant.From(in inPickup));
		info.AddProperty(PropertyName.isInConvo, Variant.From(in isInConvo));
		info.AddProperty(PropertyName.spawnedMinigames, Variant.CreateFrom(spawnedMinigames));
		info.AddProperty(PropertyName.Pause, Variant.From(in Pause));
		info.AddProperty(PropertyName.Terminal, Variant.From(in Terminal));
		info.AddProperty(PropertyName.AdminAccess, Variant.From(in AdminAccess));
		info.AddProperty(PropertyName.Magnifier, Variant.From(in Magnifier));
		info.AddProperty(PropertyName._magnifierActive, Variant.From(in _magnifierActive));
		info.AddSignalEventDelegate(SignalName.ReachedTarget, backing_ReachedTarget);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override void RestoreGodotObjectData(GodotSerializationInfo info)
	{
		base.RestoreGodotObjectData(info);
		if (info.TryGetProperty(PropertyName.mainWindow, out var value))
		{
			mainWindow = value.As<Window>();
		}
		if (info.TryGetProperty(PropertyName.mainCharacter, out var value2))
		{
			mainCharacter = value2.As<Character>();
		}
		if (info.TryGetProperty(PropertyName.saveHandler, out var value3))
		{
			saveHandler = value3.As<SaveHandler>();
		}
		if (info.TryGetProperty(PropertyName.spawnedItems, out var value4))
		{
			spawnedItems = value4.AsGodotArray<ItemWindow>();
		}
		if (info.TryGetProperty(PropertyName.maxItems, out var value5))
		{
			maxItems = value5.As<int>();
		}
		if (info.TryGetProperty(PropertyName.spawnMargin, out var value6))
		{
			spawnMargin = value6.As<Vector2>();
		}
		if (info.TryGetProperty(PropertyName.spawnerTimer, out var value7))
		{
			spawnerTimer = value7.As<Timer>();
		}
		if (info.TryGetProperty(PropertyName.spawnerTimeRange, out var value8))
		{
			spawnerTimeRange = value8.As<Vector2>();
		}
		if (info.TryGetProperty(PropertyName.spawnedAttachments, out var value9))
		{
			spawnedAttachments = value9.AsGodotArray<AttachObjWindow>();
		}
		if (info.TryGetProperty(PropertyName.dialogueStack, out var value10))
		{
			dialogueStack = value10.AsGodotArray<DialogueDataRes>();
		}
		if (info.TryGetProperty(PropertyName.timeBetweenStacks, out var value11))
		{
			timeBetweenStacks = value11.As<Vector2>();
		}
		if (info.TryGetProperty(PropertyName.defaultTextDataRes, out var value12))
		{
			defaultTextDataRes = value12.As<AttachDataRes>();
		}
		if (info.TryGetProperty(PropertyName.randomDialogueTimer, out var value13))
		{
			randomDialogueTimer = value13.As<Timer>();
		}
		if (info.TryGetProperty(PropertyName.spawnedActors, out var value14))
		{
			spawnedActors = value14.AsGodotArray<ActorWindow>();
		}
		if (info.TryGetProperty(PropertyName.spawnedCompanions, out var value15))
		{
			spawnedCompanions = value15.AsGodotArray<ActorWindow>();
		}
		if (info.TryGetProperty(PropertyName.companionLimit, out var value16))
		{
			companionLimit = value16.As<int>();
		}
		if (info.TryGetProperty(PropertyName.spawnerActorTimer, out var value17))
		{
			spawnerActorTimer = value17.As<Timer>();
		}
		if (info.TryGetProperty(PropertyName.spawnerActorTimerRange, out var value18))
		{
			spawnerActorTimerRange = value18.As<Vector2>();
		}
		if (info.TryGetProperty(PropertyName.randomAnimationTimer, out var value19))
		{
			randomAnimationTimer = value19.As<Timer>();
		}
		if (info.TryGetProperty(PropertyName.passivePlayTimer, out var value20))
		{
			passivePlayTimer = value20.As<Timer>();
		}
		if (info.TryGetProperty(PropertyName.passivePlayTimerOffRange, out var value21))
		{
			passivePlayTimerOffRange = value21.As<Vector2>();
		}
		if (info.TryGetProperty(PropertyName.passivePlayTimerOnRange, out var value22))
		{
			passivePlayTimerOnRange = value22.As<Vector2>();
		}
		if (info.TryGetProperty(PropertyName.throwSoftReactionChance, out var value23))
		{
			throwSoftReactionChance = value23.As<float>();
		}
		if (info.TryGetProperty(PropertyName.throwHardReactionChance, out var value24))
		{
			throwHardReactionChance = value24.As<float>();
		}
		if (info.TryGetProperty(PropertyName.landSoftReactionChance, out var value25))
		{
			landSoftReactionChance = value25.As<float>();
		}
		if (info.TryGetProperty(PropertyName.landHardReactionChance, out var value26))
		{
			landHardReactionChance = value26.As<float>();
		}
		if (info.TryGetProperty(PropertyName.ItemObjectScene, out var value27))
		{
			ItemObjectScene = value27.As<PackedScene>();
		}
		if (info.TryGetProperty(PropertyName.AttachmentObjectScene, out var value28))
		{
			AttachmentObjectScene = value28.As<PackedScene>();
		}
		if (info.TryGetProperty(PropertyName.actorScene, out var value29))
		{
			actorScene = value29.As<PackedScene>();
		}
		if (info.TryGetProperty(PropertyName.pauseMenu, out var value30))
		{
			pauseMenu = value30.As<PackedScene>();
		}
		if (info.TryGetProperty(PropertyName.terminalMenu, out var value31))
		{
			terminalMenu = value31.As<PackedScene>();
		}
		if (info.TryGetProperty(PropertyName.confirmationMenu, out var value32))
		{
			confirmationMenu = value32.As<PackedScene>();
		}
		if (info.TryGetProperty(PropertyName.eulaMenu, out var value33))
		{
			eulaMenu = value33.As<PackedScene>();
		}
		if (info.TryGetProperty(PropertyName.magnifierScene, out var value34))
		{
			magnifierScene = value34.As<PackedScene>();
		}
		if (info.TryGetProperty(PropertyName.settingWindowThrowPhysics, out var value35))
		{
			settingWindowThrowPhysics = value35.As<bool>();
		}
		if (info.TryGetProperty(PropertyName.settingEULA, out var value36))
		{
			settingEULA = value36.As<bool>();
		}
		if (info.TryGetProperty(PropertyName.settingSpriteScaler, out var value37))
		{
			settingSpriteScaler = value37.As<float>();
		}
		if (info.TryGetProperty(PropertyName.settingItemScaler, out var value38))
		{
			settingItemScaler = value38.As<float>();
		}
		if (info.TryGetProperty(PropertyName.settingUIScaler, out var value39))
		{
			settingUIScaler = value39.As<float>();
		}
		if (info.TryGetProperty(PropertyName.settingSpawnItems, out var value40))
		{
			settingSpawnItems = value40.As<bool>();
		}
		if (info.TryGetProperty(PropertyName.settingSpawnActors, out var value41))
		{
			settingSpawnActors = value41.As<bool>();
		}
		if (info.TryGetProperty(PropertyName.settingPassivePlayMode, out var value42))
		{
			settingPassivePlayMode = value42.As<bool>();
		}
		if (info.TryGetProperty(PropertyName.settingRemovePopups, out var value43))
		{
			settingRemovePopups = value43.As<bool>();
		}
		if (info.TryGetProperty(PropertyName.settingRemoveConvos, out var value44))
		{
			settingRemoveConvos = value44.As<bool>();
		}
		if (info.TryGetProperty(PropertyName.settingAudioOn, out var value45))
		{
			settingAudioOn = value45.As<bool>();
		}
		if (info.TryGetProperty(PropertyName.settingMods, out var value46))
		{
			settingMods = value46.As<bool>();
		}
		if (info.TryGetProperty(PropertyName.settingEnabledMods, out var value47))
		{
			settingEnabledMods = value47.AsGodotArray<string>();
		}
		if (info.TryGetProperty(PropertyName.userInfoName, out var value48))
		{
			userInfoName = value48.As<string>();
		}
		if (info.TryGetProperty(PropertyName.SeenObjects, out var value49))
		{
			SeenObjects = value49.AsGodotDictionary<SaveHandler.SeenObjectTypes, Array<string>>();
		}
		if (info.TryGetProperty(PropertyName.userTickets, out var value50))
		{
			userTickets = value50.As<int>();
		}
		if (info.TryGetProperty(PropertyName.minigameData, out var value51))
		{
			minigameData = value51.AsGodotDictionary<string, Variant>();
		}
		if (info.TryGetProperty(PropertyName.settingBlacklistedContent, out var value52))
		{
			settingBlacklistedContent = value52.AsGodotArray<SaveHandler.Kinks>();
		}
		if (info.TryGetProperty(PropertyName.mouseOffset, out var value53))
		{
			mouseOffset = value53.As<Vector2>();
		}
		if (info.TryGetProperty(PropertyName.selected, out var value54))
		{
			selected = value54.As<bool>();
		}
		if (info.TryGetProperty(PropertyName.SomethingHasBeenGrabbed, out var value55))
		{
			SomethingHasBeenGrabbed = value55.As<bool>();
		}
		if (info.TryGetProperty(PropertyName.grabReleaseTween, out var value56))
		{
			grabReleaseTween = value56.As<Tween>();
		}
		if (info.TryGetProperty(PropertyName.screenDataHandler, out var value57))
		{
			screenDataHandler = value57.As<ScreenDataHandler>();
		}
		if (info.TryGetProperty(PropertyName.isWalking, out var value58))
		{
			isWalking = value58.As<bool>();
		}
		if (info.TryGetProperty(PropertyName.walkDirection, out var value59))
		{
			walkDirection = value59.As<int>();
		}
		if (info.TryGetProperty(PropertyName.pickupWatchdogTimer, out var value60))
		{
			pickupWatchdogTimer = value60.As<float>();
		}
		if (info.TryGetProperty(PropertyName.mouseVelocityScaler, out var value61))
		{
			mouseVelocityScaler = value61.As<float>();
		}
		if (info.TryGetProperty(PropertyName.windowBounceDamping, out var value62))
		{
			windowBounceDamping = value62.As<float>();
		}
		if (info.TryGetProperty(PropertyName.windowAirResist, out var value63))
		{
			windowAirResist = value63.As<float>();
		}
		if (info.TryGetProperty(PropertyName.windowVelocity, out var value64))
		{
			windowVelocity = value64.As<Vector2>();
		}
		if (info.TryGetProperty(PropertyName.isThrown, out var value65))
		{
			isThrown = value65.As<bool>();
		}
		if (info.TryGetProperty(PropertyName.isWalkingToTargetPosition, out var value66))
		{
			isWalkingToTargetPosition = value66.As<bool>();
		}
		if (info.TryGetProperty(PropertyName.targetX, out var value67))
		{
			targetX = value67.As<int>();
		}
		if (info.TryGetProperty(PropertyName.walkTargetItem, out var value68))
		{
			walkTargetItem = value68.As<ItemWindow>();
		}
		if (info.TryGetProperty(PropertyName.storedItem, out var value69))
		{
			storedItem = value69.As<ItemWindow>();
		}
		if (info.TryGetProperty(PropertyName.inPickup, out var value70))
		{
			inPickup = value70.As<bool>();
		}
		if (info.TryGetProperty(PropertyName.isInConvo, out var value71))
		{
			isInConvo = value71.As<bool>();
		}
		if (info.TryGetProperty(PropertyName.spawnedMinigames, out var value72))
		{
			spawnedMinigames = value72.AsGodotArray<Window>();
		}
		if (info.TryGetProperty(PropertyName.Pause, out var value73))
		{
			Pause = value73.As<PauseMenu>();
		}
		if (info.TryGetProperty(PropertyName.Terminal, out var value74))
		{
			Terminal = value74.As<TerminalWindow>();
		}
		if (info.TryGetProperty(PropertyName.AdminAccess, out var value75))
		{
			AdminAccess = value75.As<bool>();
		}
		if (info.TryGetProperty(PropertyName.Magnifier, out var value76))
		{
			Magnifier = value76.As<MagnifierWindow>();
		}
		if (info.TryGetProperty(PropertyName._magnifierActive, out var value77))
		{
			_magnifierActive = value77.As<bool>();
		}
		if (info.TryGetSignalEventDelegate<ReachedTargetEventHandler>(SignalName.ReachedTarget, out var value78))
		{
			backing_ReachedTarget = value78;
		}
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	internal static List<MethodInfo> GetGodotSignalList()
	{
		return new List<MethodInfo>(1)
		{
			new MethodInfo(SignalName.ReachedTarget, new PropertyInfo(Variant.Type.Nil, "", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false), MethodFlags.Normal, null, null)
		};
	}

	protected void EmitSignalReachedTarget()
	{
		EmitSignal(SignalName.ReachedTarget);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override void RaiseGodotClassSignalCallbacks(in godot_string_name signal, NativeVariantPtrArgs args)
	{
		if (signal == SignalName.ReachedTarget && args.Count == 0)
		{
			backing_ReachedTarget?.Invoke();
		}
		else
		{
			base.RaiseGodotClassSignalCallbacks(in signal, args);
		}
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override bool HasGodotClassSignal(in godot_string_name signal)
	{
		if (signal == SignalName.ReachedTarget)
		{
			return true;
		}
		return base.HasGodotClassSignal(in signal);
	}
}
