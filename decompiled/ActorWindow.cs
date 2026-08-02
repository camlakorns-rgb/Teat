using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Godot;
using Godot.Bridge;
using Godot.Collections;
using Godot.NativeInterop;

[ScriptPath("res://Scripts/CharacterScripts/ActorWindow.cs")]
public class ActorWindow : Window
{
	public new class MethodName : Window.MethodName
	{
		public new static readonly StringName _Ready = "_Ready";

		public static readonly StringName DEBUG_LoadInScene = "DEBUG_LoadInScene";

		public static readonly StringName SetupActorWindow = "SetupActorWindow";

		public static readonly StringName PopActor = "PopActor";

		public static readonly StringName HandlePop = "HandlePop";

		public static readonly StringName StartKill = "StartKill";

		public new static readonly StringName _Process = "_Process";

		public static readonly StringName WalkToPlayer = "WalkToPlayer";

		public static readonly StringName GetChaseStoppingDistance = "GetChaseStoppingDistance";

		public static readonly StringName GetCompanionStoppingDistance = "GetCompanionStoppingDistance";

		public static readonly StringName GetCompanionEffectiveX = "GetCompanionEffectiveX";

		public static readonly StringName IsCompanionIdleXOverlapping = "IsCompanionIdleXOverlapping";

		public static readonly StringName SyncCachesToScreen = "SyncCachesToScreen";

		public static readonly StringName HandleCompanionAI = "HandleCompanionAI";

		public static readonly StringName HandleEnemyAI = "HandleEnemyAI";

		public static readonly StringName IsOverlappingTargetWindow = "IsOverlappingTargetWindow";

		public static readonly StringName IsOverlappingActorWindows = "IsOverlappingActorWindows";

		public static readonly StringName SpawnItem = "SpawnItem";

		public static readonly StringName OnAggroTimerTimeout = "OnAggroTimerTimeout";

		public static readonly StringName HandleEnemyMouseFlee = "HandleEnemyMouseFlee";

		public static readonly StringName OnRandomAnimationTimerTimeout = "OnRandomAnimationTimerTimeout";

		public static readonly StringName OnRandomConvoTimerTimeout = "OnRandomConvoTimerTimeout";

		public static readonly StringName ComputeThinnerCollisionBox = "ComputeThinnerCollisionBox";

		public static readonly StringName IsInteractable = "IsInteractable";

		public static readonly StringName HandleCindeshCompanionKinks = "HandleCindeshCompanionKinks";
	}

	public new class PropertyName : Window.PropertyName
	{
		public static readonly StringName characterActor = "characterActor";

		public static readonly StringName spawnMargin = "spawnMargin";

		public static readonly StringName randomConvoTimer = "randomConvoTimer";

		public static readonly StringName randomAnimationTimer = "randomAnimationTimer";

		public static readonly StringName aggroTimer = "aggroTimer";

		public static readonly StringName SpawnOnLoad = "SpawnOnLoad";

		public static readonly StringName walkX = "walkX";

		public static readonly StringName softWalkTimer = "softWalkTimer";

		public static readonly StringName randomDistance = "randomDistance";

		public static readonly StringName distanceRanges = "distanceRanges";

		public static readonly StringName popProgress = "popProgress";

		public static readonly StringName popShader = "popShader";

		public static readonly StringName setAIType = "setAIType";

		public static readonly StringName cachedActorScreenIndex = "cachedActorScreenIndex";

		public static readonly StringName cachedActorScreenRect = "cachedActorScreenRect";

		public static readonly StringName cachedThinBox = "cachedThinBox";

		public static readonly StringName spawnTime = "spawnTime";

		public static readonly StringName inUse = "inUse";

		public static readonly StringName inUseByAttachment = "inUseByAttachment";

		public static readonly StringName inAggro = "inAggro";

		public static readonly StringName possibleAggroOverrideAnimations = "possibleAggroOverrideAnimations";

		public static readonly StringName menu = "menu";

		public static readonly StringName targetWindow = "targetWindow";

		public static readonly StringName mouseLingerTimer = "mouseLingerTimer";

		public static readonly StringName isFleeing = "isFleeing";

		public static readonly StringName fleeTimer = "fleeTimer";

		public static readonly StringName _enemyWalkDirection = "_enemyWalkDirection";

		public static readonly StringName _enemyBounced = "_enemyBounced";
	}

	public new class SignalName : Window.SignalName
	{
	}

	[Export(PropertyHint.None, "")]
	public ActorCharacter characterActor;

	[Export(PropertyHint.None, "")]
	public Vector2 spawnMargin = new Vector2(256f, 128f);

	[Export(PropertyHint.None, "")]
	public Timer randomConvoTimer;

	[Export(PropertyHint.None, "")]
	public Timer randomAnimationTimer;

	[Export(PropertyHint.None, "")]
	public Timer aggroTimer;

	[ExportGroup("Debug", "")]
	[Export(PropertyHint.None, "")]
	public bool SpawnOnLoad;

	public float walkX = -1f;

	private double softWalkTimer;

	public float randomDistance = -1f;

	private Vector2 distanceRanges = new Vector2(0.6f, 2f);

	private float popProgress = -1f;

	private ShaderMaterial popShader;

	private CharacterInfoDataRes.AITypes setAIType;

	private int cachedActorScreenIndex = -1;

	private Rect2I cachedActorScreenRect;

	private Rect2I cachedThinBox;

	private double spawnTime;

	public bool inUse;

	public bool inUseByAttachment;

	public bool inAggro;

	public Array<AttachDataRes> possibleAggroOverrideAnimations = new Array<AttachDataRes>();

	private ConfirmationMenu menu;

	public ActorWindow targetWindow;

	private double mouseLingerTimer;

	private const double MouseLingerThreshold = 0.4000000059604645;

	private bool isFleeing;

	private double fleeTimer;

	private const double FleeDuration = 2.5;

	private const float FleeSpeedMultiplier = 1.85f;

	private float _enemyWalkDirection;

	private bool _enemyBounced;

	public override void _Ready()
	{
		if (SpawnOnLoad && characterActor != null)
		{
			CallDeferred("DEBUG_LoadInScene");
		}
	}

	private void DEBUG_LoadInScene()
	{
		Main.Instance.spawnedActors.Add(this);
		if (characterActor.characterInformation.AITyping == CharacterInfoDataRes.AITypes.COMPANION)
		{
			Main.Instance.spawnedCompanions.Add(this);
		}
		CallDeferred(MethodName.SetupActorWindow, characterActor.characterInformation);
	}

	public void SetupActorWindow(CharacterInfoDataRes characterData = null)
	{
		SetupActorWindow(Vector2I.Zero, characterData);
	}

	public void SetupActorWindow(Vector2I overridePos, CharacterInfoDataRes characterData = null)
	{
		if (characterData != null)
		{
			characterActor.characterInformation = characterData;
		}
		characterActor.trueSize = (Vector2I)(characterActor.characterInformation.characterSize * characterActor.characterInformation.characterScale * Main.Instance.settingSpriteScaler);
		characterActor.SetupActor();
		base.MinSize = characterActor.trueSize;
		base.Size = base.MinSize;
		if (overridePos == Vector2I.Zero)
		{
			int pos = ((GD.RandRange(0, 1) == 0) ? (Main.Instance.screenDataHandler.EffectiveLeftX - (int)spawnMargin.X) : (Main.Instance.screenDataHandler.EffectiveRightX + (int)spawnMargin.X));
			int y = DisplayServer.ScreenGetUsableRect(Main.Instance.screenDataHandler.screenIndex).End.Y - characterActor.trueSize.Y;
			int num = Main.Instance.screenDataHandler.ClampAcrossAllScreensX(pos, characterActor.trueSize.X);
			base.Position = new Vector2I(num, y);
			walkX = num;
		}
		else
		{
			GD.Print("Override");
			base.Position = overridePos;
			walkX = overridePos.X;
		}
		base.ProcessMode = ProcessModeEnum.Inherit;
		base.Visible = true;
		setAIType = characterActor.characterInformation.AITyping;
		if (setAIType == CharacterInfoDataRes.AITypes.COMPANION)
		{
			randomAnimationTimer.Start();
			if (characterActor.characterInformation.isAggroActor)
			{
				aggroTimer.WaitTime = GD.RandRange(characterActor.characterInformation.aggroTimerRange.X, characterActor.characterInformation.aggroTimerRange.Y);
				aggroTimer.Start();
			}
		}
		cachedThinBox = ComputeThinnerCollisionBox();
	}

	private void PopActor()
	{
		if (characterActor.characterInformation.UnBlockable)
		{
			return;
		}
		switch (characterActor.characterInformation.AITyping)
		{
		case CharacterInfoDataRes.AITypes.COMPANION:
			if (Input.IsActionJustPressed("Despawn"))
			{
				HandlePop();
			}
			break;
		case CharacterInfoDataRes.AITypes.ENEMY:
			if (Input.IsActionJustPressed("Pet"))
			{
				HandlePop();
			}
			if (Input.IsActionJustPressed("Move"))
			{
				HandlePop();
			}
			if (Input.IsActionJustPressed("Despawn"))
			{
				HandlePop();
			}
			break;
		}
	}

	private void HandlePop()
	{
		if (!cachedThinBox.HasPoint(DisplayServer.MouseGetPosition()))
		{
			return;
		}
		if (characterActor.characterInformation.AITyping == CharacterInfoDataRes.AITypes.COMPANION)
		{
			if (menu != null)
			{
				return;
			}
			ConfirmationMenu confirmationMenu = Main.Instance.confirmationMenu.Instantiate<ConfirmationMenu>(PackedScene.GenEditState.Disabled);
			AddChild(confirmationMenu, forceReadableName: false, InternalMode.Disabled);
			confirmationMenu.label.Text = "[font_size=28]Do you wish to Destroy " + characterActor.characterInformation.Name + "?[/font_size]";
			confirmationMenu.Confirmed += delegate
			{
				StartKill();
				if (Main.Instance.mainCharacter.Visible)
				{
					Main.Instance.ClearAllAttachments();
					string name = characterActor.characterInformation.Name;
					DialogueDataRes dialogueDataRes = (DialogueDataRes)Main.Instance.mainCharacter.characterInformation.responseTexts[CharacterInfoDataRes.ResponseToSituation.COMPANION_KILLED].Duplicate();
					dialogueDataRes.Dialogue = dialogueDataRes.Dialogue.Replace("{NAME}", name);
					Main.Instance.dialogueStack.Add(dialogueDataRes);
					Main.Instance.PopDialogueInStack(skipTimer: true);
					Main.Instance.isInConvo = false;
				}
			};
			confirmationMenu.Deny += delegate
			{
				menu = null;
			};
			menu = confirmationMenu;
		}
		else
		{
			StartKill();
		}
	}

	public void StartKill()
	{
		if (characterActor.MainBody.Material is ShaderMaterial shaderMaterial)
		{
			popProgress = 0f;
			popShader = shaderMaterial;
			return;
		}
		Main.Instance.spawnedActors.Remove(this);
		if (Main.Instance.spawnedCompanions.Contains(this))
		{
			Main.Instance.spawnedCompanions.Remove(this);
		}
		QueueFree();
	}

	public override void _Process(double delta)
	{
		if (popShader != null)
		{
			characterActor.MainBody.Play("Idle");
			if (!(popProgress >= 0f))
			{
				return;
			}
			popProgress += (float)delta * 0.6f;
			popShader.SetShaderParameter("progress", popProgress);
			if (popProgress >= 1f)
			{
				Main.Instance.spawnedActors.Remove(this);
				if (Main.Instance.spawnedCompanions.Contains(this))
				{
					Main.Instance.spawnedCompanions.Remove(this);
				}
				QueueFree();
			}
			return;
		}
		if (characterActor.petMainBodyState != ActorCharacter.MainBodyStates.Forced_Animation && characterActor.petMainBodyState != ActorCharacter.MainBodyStates.Transition && !inUseByAttachment)
		{
			float chaseStoppingDistance = GetChaseStoppingDistance();
			WalkToPlayer(delta, (targetWindow != null) ? targetWindow.Position.X : Main.Instance.mainWindow.Position.X, chaseStoppingDistance);
			SpawnItem(delta);
		}
		if (!inUse)
		{
			PopActor();
		}
		switch (setAIType)
		{
		case CharacterInfoDataRes.AITypes.ENEMY:
			HandleEnemyAI(delta);
			break;
		case CharacterInfoDataRes.AITypes.COMPANION:
			HandleCompanionAI(delta);
			break;
		}
	}

	private void WalkToPlayer(double delta, float targetX, float stoppingDistance)
	{
		if (targetWindow != null)
		{
			if (!targetWindow.characterActor.Visible)
			{
				randomDistance = -1f;
				softWalkTimer = GD.RandRange(0.800000011920929, 1.2000000476837158);
				if (characterActor.MainBody.Animation == (StringName)"Walk")
				{
					characterActor.MainBody.Play("Idle");
				}
				return;
			}
		}
		else if (!Main.Instance.mainCharacter.Visible)
		{
			randomDistance = -1f;
			softWalkTimer = GD.RandRange(0.800000011920929, 1.2000000476837158);
			if (characterActor.MainBody.Animation == (StringName)"Walk")
			{
				characterActor.MainBody.Play("Idle");
			}
			return;
		}
		if (Mathf.Abs(walkX - targetX) >= stoppingDistance)
		{
			if (softWalkTimer > 0.0)
			{
				softWalkTimer -= delta;
				return;
			}
			if (walkX < 0f)
			{
				walkX = base.Position.X;
			}
			float num = characterActor.characterInformation.WalkSpeed;
			int num2;
			if (setAIType == CharacterInfoDataRes.AITypes.ENEMY && isFleeing)
			{
				num2 = ((!((float)DisplayServer.MouseGetPosition().X > walkX)) ? 1 : (-1));
				num *= 1.85f;
			}
			else
			{
				num2 = ((targetX > walkX) ? 1 : (-1));
			}
			walkX += num * (float)delta * (float)num2;
			if (setAIType == CharacterInfoDataRes.AITypes.ENEMY)
			{
				_enemyWalkDirection = num2;
				int num3 = Main.Instance.screenDataHandler.ClampAcrossAllScreensX(Mathf.RoundToInt(walkX), characterActor.trueSize.X);
				if (num3 != Mathf.RoundToInt(walkX))
				{
					_enemyWalkDirection *= -1f;
					walkX = (float)num3 + _enemyWalkDirection * num * (float)delta;
					if (isFleeing)
					{
						isFleeing = false;
					}
				}
				walkX = num3;
			}
			bool flipH = (characterActor.characterInformation.flipSpriteH ? (num2 == 1) : (num2 == -1));
			characterActor.FlipHSpriteTo(flipH);
			characterActor.MainBody.Play("Walk");
			int currentScreen = base.CurrentScreen;
			if (currentScreen != cachedActorScreenIndex)
			{
				cachedActorScreenIndex = currentScreen;
				cachedActorScreenRect = DisplayServer.ScreenGetUsableRect(currentScreen);
			}
			base.Position = new Vector2I(Main.Instance.screenDataHandler.ClampAcrossAllScreensX(Mathf.RoundToInt(walkX), characterActor.trueSize.X), cachedActorScreenRect.End.Y - characterActor.trueSize.Y);
			cachedThinBox = ComputeThinnerCollisionBox();
		}
		else
		{
			randomDistance = -1f;
			softWalkTimer = GD.RandRange(0.800000011920929, 1.2000000476837158);
			if (characterActor.MainBody.Animation == (StringName)"Walk")
			{
				characterActor.MainBody.Play("Idle");
			}
		}
	}

	private float GetChaseStoppingDistance()
	{
		CharacterInfoDataRes.AITypes aITyping = characterActor.characterInformation.AITyping;
		if (aITyping != CharacterInfoDataRes.AITypes.ENEMY && aITyping == CharacterInfoDataRes.AITypes.COMPANION)
		{
			if (inAggro)
			{
				return (float)Main.Instance.mainCharacter.trueSize.X / 2f;
			}
			return GetCompanionStoppingDistance();
		}
		return (float)Main.Instance.mainCharacter.trueSize.X / 2f;
	}

	private float GetCompanionStoppingDistance()
	{
		if (randomDistance < 0f)
		{
			randomDistance = (float)GD.RandRange(distanceRanges.X, distanceRanges.Y);
			_ = Main.Instance.mainWindow.Position;
			int x = characterActor.trueSize.X;
			for (int i = 0; i < 8; i++)
			{
				float num = (float)Main.Instance.mainWindow.Position.X + (float)Main.Instance.mainCharacter.trueSize.X / 2f;
				float num2 = ((walkX >= 0f) ? walkX : ((float)base.Position.X));
				float num3 = ((num > num2) ? (-1f) : 1f);
				float candidateX = num + num3 * (float)Main.Instance.mainCharacter.trueSize.X * randomDistance - (float)x / 2f;
				if (!IsCompanionIdleXOverlapping(candidateX, x))
				{
					break;
				}
				randomDistance += 0.25f;
			}
		}
		float num4 = (float)Main.Instance.mainCharacter.trueSize.X * randomDistance;
		if (Mathf.Abs(walkX - (float)Main.Instance.mainWindow.Position.X) >= num4 && characterActor.MainBody.Animation != (StringName)"Walk")
		{
			return (float)Main.Instance.mainCharacter.trueSize.X * distanceRanges.Y;
		}
		return num4;
	}

	private float GetCompanionEffectiveX(ActorWindow companion)
	{
		if (!(companion.walkX >= 0f))
		{
			return companion.Position.X;
		}
		return companion.walkX;
	}

	private bool IsCompanionIdleXOverlapping(float candidateX, int actorWidth)
	{
		int num = Mathf.RoundToInt((float)actorWidth * 0.33f);
		int num2 = Mathf.RoundToInt((float)(actorWidth - num) / 2f);
		foreach (ActorWindow spawnedCompanion in Main.Instance.spawnedCompanions)
		{
			if (spawnedCompanion != this && GodotObject.IsInstanceValid(spawnedCompanion))
			{
				float num5;
				if (spawnedCompanion.randomDistance >= 0f)
				{
					float num3 = (float)Main.Instance.mainWindow.Position.X + (float)Main.Instance.mainCharacter.trueSize.X / 2f;
					float companionEffectiveX = GetCompanionEffectiveX(spawnedCompanion);
					float num4 = ((num3 > companionEffectiveX) ? (-1f) : 1f);
					num5 = num3 + num4 * (float)Main.Instance.mainCharacter.trueSize.X * spawnedCompanion.randomDistance - (float)spawnedCompanion.characterActor.trueSize.X / 2f;
				}
				else
				{
					num5 = GetCompanionEffectiveX(spawnedCompanion);
				}
				int num6 = Mathf.RoundToInt((float)spawnedCompanion.characterActor.trueSize.X * 0.33f);
				int num7 = Mathf.RoundToInt((float)(spawnedCompanion.characterActor.trueSize.X - num6) / 2f);
				if (candidateX + (float)num2 < num5 + (float)num7 + (float)num6 && candidateX + (float)num2 + (float)num > num5 + (float)num7)
				{
					return true;
				}
			}
		}
		return false;
	}

	public void SyncCachesToScreen(int screenIndex, Rect2I screenRect)
	{
		walkX = base.Position.X;
		cachedActorScreenIndex = screenIndex;
		cachedActorScreenRect = screenRect;
		cachedThinBox = ComputeThinnerCollisionBox();
	}

	private void HandleCompanionAI(double delta)
	{
		if (!Main.Instance.mainCharacter.Visible && inUse)
		{
			base.Visible = false;
		}
		else if (inUseByAttachment)
		{
			base.Visible = false;
		}
		else
		{
			base.Visible = true;
			inUse = false;
		}
		if (inAggro)
		{
			IEnumerable<AttachDataRes> enumerable = characterActor.characterInformation.overrideAnimation.Concat(possibleAggroOverrideAnimations);
			if (!Main.Instance.mainCharacter.Visible || enumerable == null || enumerable.Count() == 0 || !base.Visible)
			{
				inAggro = false;
			}
			else if (IsOverlappingTargetWindow())
			{
				Main.Instance.ClearAllAttachments();
				Main.Instance.ForceDropCharacter();
				WeightGroup<AttachDataRes> weightGroup = BuildWeightedAttachments(enumerable);
				if (weightGroup.Count > 0)
				{
					Main.Instance.CallCharacterAttachmentSpawn(weightGroup.GetItem(GD.RandRange(0, 10000)));
				}
				inAggro = false;
				inUse = true;
				if (characterActor.characterInformation.isAggroActor)
				{
					aggroTimer.WaitTime = GD.RandRange(characterActor.characterInformation.aggroTimerRange.X * 2f, characterActor.characterInformation.aggroTimerRange.Y * 2f);
					aggroTimer.Start();
				}
			}
			return;
		}
		HandleCindeshCompanionKinks();
		if (Input.IsActionJustPressed("Pet") && Main.Instance.mainCharacter.Visible && !Main.Instance.SomethingHasBeenGrabbed && cachedThinBox.HasPoint(DisplayServer.MouseGetPosition()))
		{
			DialogueDataRes dialogueDataRes = Main.Instance.PickDialogue(characterActor.characterInformation.interactionTexts);
			if (dialogueDataRes != null)
			{
				Main.Instance.ClearAllAttachments();
				if (Main.Instance.isInConvo)
				{
					if (characterActor.characterInformation.responseTexts.ContainsKey(CharacterInfoDataRes.ResponseToSituation.IN_CONVO))
					{
						Main.Instance.dialogueStack.Add(characterActor.characterInformation.responseTexts[CharacterInfoDataRes.ResponseToSituation.IN_CONVO]);
						Main.Instance.PopDialogueInStack(skipTimer: true);
					}
					Main.Instance.isInConvo = false;
				}
				else
				{
					DialogueDataRes dialogueDataRes2 = (DialogueDataRes)dialogueDataRes.Duplicate();
					dialogueDataRes2.speakingActorID = characterActor.characterInformation._itemID;
					Main.Instance.dialogueStack.Add(dialogueDataRes2);
					Main.Instance.PopDialogueInStack(skipTimer: true);
				}
			}
		}
		float num = Main.Instance.mainCharacter.mouseVelocity.Length();
		if (IsOverlappingTargetWindow() && Input.IsActionJustReleased("Move") && IsInteractable() && num < 800f && new Rect2I(Main.Instance.mainWindow.Position, Main.Instance.mainWindow.Size).HasPoint(DisplayServer.MouseGetPosition()))
		{
			Main.Instance.ClearAllAttachments();
			Main.Instance.ForceDropCharacter();
			WeightGroup<AttachDataRes> weightGroup2 = BuildWeightedAttachments(characterActor.characterInformation.overrideAnimation);
			if (weightGroup2.Count > 0)
			{
				Main.Instance.CallCharacterAttachmentSpawn(weightGroup2.GetItem(GD.RandRange(0, 10000)));
				inUse = true;
			}
		}
	}

	private void HandleEnemyAI(double delta)
	{
		base.Visible = true;
		HandleEnemyMouseFlee(delta);
		if (IsOverlappingTargetWindow())
		{
			Main.Instance.ClearAllAttachments();
			Main.Instance.ForceDropCharacter();
			WeightGroup<AttachDataRes> weightGroup = BuildWeightedAttachments(characterActor.characterInformation.overrideAnimation);
			if (weightGroup.Count > 0)
			{
				Main.Instance.CallCharacterAttachmentSpawn(weightGroup.GetItem(GD.RandRange(0, 10000)), unclearableAttachment: false, targetWindow);
			}
			Main.Instance.spawnedActors.Remove(this);
			if (Main.Instance.spawnedCompanions.Contains(this))
			{
				Main.Instance.spawnedCompanions.Remove(this);
			}
			QueueFree();
		}
	}

	private bool IsOverlappingTargetWindow()
	{
		Rect2I b = new Rect2I(Main.Instance.mainWindow.Position, Main.Instance.mainWindow.Size);
		if (targetWindow != null)
		{
			b = new Rect2I(targetWindow.Position, targetWindow.Size);
		}
		return cachedThinBox.Intersects(b);
	}

	private bool IsOverlappingActorWindows()
	{
		Rect2I rect2I = new Rect2I(base.Position, base.Size);
		foreach (ActorWindow spawnedActor in Main.Instance.spawnedActors)
		{
			if (spawnedActor != this && GodotObject.IsInstanceValid(spawnedActor))
			{
				Rect2I b = new Rect2I(spawnedActor.Position, spawnedActor.Size);
				if (rect2I.Intersects(b))
				{
					return true;
				}
			}
		}
		return false;
	}

	private void SpawnItem(double delta)
	{
		if (characterActor.characterInformation.spawnsItems)
		{
			if (spawnTime <= 0.0)
			{
				spawnTime = GD.RandRange(characterActor.characterInformation.itemSpawnRate.X, characterActor.characterInformation.itemSpawnRate.Y);
				Main.Instance.OnSpawnerTimeout(base.Position.X + Mathf.RoundToInt(characterActor.trueSize.X / 2), base.Position.Y + Mathf.RoundToInt(characterActor.trueSize.Y / 2));
			}
			else
			{
				spawnTime -= delta;
			}
		}
	}

	public void OnAggroTimerTimeout()
	{
		if (aggroTimer != null)
		{
			aggroTimer.WaitTime = GD.RandRange(characterActor.characterInformation.aggroTimerRange.X / 2f, characterActor.characterInformation.aggroTimerRange.Y / 2f);
			aggroTimer.Start();
			if (Main.Instance.mainCharacter.Visible && characterActor.characterInformation.overrideAnimation != null && characterActor.characterInformation.overrideAnimation.Count() != 0 && !Main.Instance.SomethingHasBeenGrabbed && base.Visible && !inUse && !inUseByAttachment)
			{
				inAggro = true;
			}
		}
	}

	public WeightGroup<AttachDataRes> BuildWeightedAttachments(IEnumerable<AttachDataRes> attachments)
	{
		Array<string> array = Main.Instance.SeenObjects[SaveHandler.SeenObjectTypes.NSFW_SCENES];
		float num = 500f;
		WeightGroup<AttachDataRes> weightGroup = new WeightGroup<AttachDataRes>();
		foreach (AttachDataRes attachment in attachments)
		{
			if (Main.Instance.IsBlacklisted(attachment.taggedKinks))
			{
				continue;
			}
			bool flag = false;
			foreach (TagDataRes requiredTag in attachment.RequiredTags)
			{
				TagDataRes tag = Main.Instance.mainCharacter.GetTag(requiredTag.tagName);
				if (tag == null || (requiredTag.tagAmount != 0 && tag.tagAmount < requiredTag.tagAmount))
				{
					flag = true;
					break;
				}
			}
			if (!flag)
			{
				string baseName = attachment.ResourcePath.GetFile().GetBaseName();
				float num2 = (array.Contains(baseName) ? 0f : num);
				weightGroup.Add(attachment, attachment.attachmentAppeanceWeight + num2);
			}
		}
		return weightGroup;
	}

	private void HandleEnemyMouseFlee(double delta)
	{
		if (!characterActor.characterInformation.AvoidMouse)
		{
			return;
		}
		if (isFleeing)
		{
			fleeTimer -= delta;
			if (fleeTimer <= 0.0)
			{
				isFleeing = false;
				fleeTimer = 0.0;
			}
		}
		else if (cachedThinBox.HasPoint(DisplayServer.MouseGetPosition()))
		{
			mouseLingerTimer += delta;
			if (mouseLingerTimer >= 0.4000000059604645)
			{
				isFleeing = true;
				fleeTimer = 2.5;
				mouseLingerTimer = 0.0;
			}
		}
		else
		{
			mouseLingerTimer = 0.0;
		}
	}

	public void OnRandomAnimationTimerTimeout()
	{
		if (characterActor.characterInformation.randomAnimations.Count() == 0)
		{
			randomAnimationTimer.Stop();
			return;
		}
		randomAnimationTimer.WaitTime = GD.RandRange(characterActor.characterInformation.randomAnimationTimer.X, characterActor.characterInformation.randomAnimationTimer.Y);
		if (characterActor.MainBody.Animation != (StringName)"Idle" || !Main.Instance.mainCharacter.Visible || inUse || inUseByAttachment)
		{
			randomAnimationTimer.WaitTime = GD.RandRange(5.0, 10.0);
			randomAnimationTimer.Start();
			return;
		}
		randomAnimationTimer.Start();
		WeightGroup<AnimDataRes> weightGroup = new WeightGroup<AnimDataRes>();
		foreach (AnimDataRes randomAnimation in characterActor.characterInformation.randomAnimations)
		{
			if (randomAnimation.animationName == "INVALID_ANIMATION")
			{
				GD.PrintErr("Invalid Animation Name in Key, please put a valid animation name!!!");
			}
			else if (!Main.Instance.IsBlacklisted(randomAnimation.taggedKinks))
			{
				weightGroup.Add(randomAnimation, randomAnimation.animationAppeanceWeight);
			}
		}
		AnimDataRes item = weightGroup.GetItem(GD.RandRange(0, 10000));
		if (item.hasTransition)
		{
			characterActor.ForceMainBodyStateTransition(item.animationName, (float)GD.RandRange(item.randomTime.X, item.randomTime.Y));
		}
		else
		{
			characterActor.ForceMainBodyState(ActorCharacter.MainBodyStates.Forced_Animation, item.animationName, (float)GD.RandRange(item.randomTime.X, item.randomTime.Y));
		}
	}

	public void OnRandomConvoTimerTimeout()
	{
		if (characterActor.characterInformation.randomTexts == null || characterActor.characterInformation.randomTexts.Count() == 0)
		{
			randomConvoTimer.Stop();
			return;
		}
		randomConvoTimer.WaitTime = GD.RandRange(characterActor.characterInformation.randomDialogueTimer.X, characterActor.characterInformation.randomDialogueTimer.Y);
		if (!Main.Instance.mainCharacter.Visible || Main.Instance.isInConvo || Main.Instance.settingRemoveConvos || inUse || inUseByAttachment)
		{
			randomConvoTimer.WaitTime = GD.RandRange(5.0, 10.0);
			randomConvoTimer.Start();
			return;
		}
		randomConvoTimer.Start();
		Main.Instance.ClearAllAttachments();
		ConvoDataRes convoDataRes = Main.Instance.PickConvo(characterActor.characterInformation.randomTexts);
		if (convoDataRes == null)
		{
			return;
		}
		foreach (DialogueDataRes item in convoDataRes.convoStack)
		{
			DialogueDataRes dialogueDataRes = (DialogueDataRes)item.Duplicate();
			if (dialogueDataRes.speakingActorID == "")
			{
				dialogueDataRes.speakingActorID = characterActor.characterInformation._itemID;
			}
			Main.Instance.dialogueStack.Add(dialogueDataRes);
		}
		Main.Instance.PopDialogueInStack(skipTimer: true);
		Main.Instance.isInConvo = true;
	}

	private Rect2I ComputeThinnerCollisionBox()
	{
		float num = 0.33f;
		int num2 = Mathf.RoundToInt((float)base.Size.X * num);
		int num3 = Mathf.RoundToInt((float)(base.Size.X - num2) / 2f);
		return new Rect2I(new Vector2I(base.Position.X + num3, base.Position.Y), new Vector2I(num2, base.Size.Y));
	}

	private bool IsInteractable()
	{
		if (Main.Instance.mainCharacter.Visible && characterActor.characterInformation.overrideAnimation != null && characterActor.characterInformation.overrideAnimation.Count() > 0)
		{
			return !Main.Instance.SomethingHasBeenGrabbed;
		}
		return false;
	}

	private void HandleCindeshCompanionKinks()
	{
		if (Main.Instance.IsBlacklisted(SaveHandler.Kinks.CUCKING))
		{
			return;
		}
		if (!Main.Instance.mainCharacter.Visible && characterActor.characterInformation.SpeicalAnimations != null && !inUse)
		{
			if (characterActor.MainBody.Animation != (StringName)characterActor.characterInformation.SpeicalAnimations.animationName && characterActor.MainBody.Animation != (StringName)(characterActor.characterInformation.SpeicalAnimations.animationName + "_Transition"))
			{
				if (characterActor.characterInformation.SpeicalAnimations.hasTransition)
				{
					characterActor.ForceMainBodyStateTransition(characterActor.characterInformation.SpeicalAnimations.animationName, (float)GD.RandRange(characterActor.characterInformation.SpeicalAnimations.randomTime.X, characterActor.characterInformation.SpeicalAnimations.randomTime.Y));
				}
				else
				{
					characterActor.ForceMainBodyState(ActorCharacter.MainBodyStates.Forced_Animation, characterActor.characterInformation.SpeicalAnimations.animationName, (float)GD.RandRange(characterActor.characterInformation.SpeicalAnimations.randomTime.X, characterActor.characterInformation.SpeicalAnimations.randomTime.Y));
				}
			}
			else
			{
				characterActor.mainBodyTimer.Stop();
			}
		}
		else if (characterActor.mainBodyTimer.IsStopped())
		{
			string text = characterActor.characterInformation.SpeicalAnimations?.animationName ?? "";
			if (!string.IsNullOrEmpty(text) && (characterActor.MainBody.Animation == (StringName)text || characterActor.MainBody.Animation == (StringName)(text + "_Transition")))
			{
				characterActor.mainBodyTimer.WaitTime = 0.009999999776482582;
			}
			characterActor.mainBodyTimer.Start();
		}
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	internal static List<MethodInfo> GetGodotMethodList()
	{
		return new List<MethodInfo>(26)
		{
			new MethodInfo(MethodName._Ready, new PropertyInfo(Variant.Type.Nil, "", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false), MethodFlags.Normal, null, null),
			new MethodInfo(MethodName.DEBUG_LoadInScene, new PropertyInfo(Variant.Type.Nil, "", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false), MethodFlags.Normal, null, null),
			new MethodInfo(MethodName.SetupActorWindow, new PropertyInfo(Variant.Type.Nil, "", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false), MethodFlags.Normal, new List<PropertyInfo>
			{
				new PropertyInfo(Variant.Type.Object, "characterData", PropertyHint.None, "", PropertyUsageFlags.Default, new StringName("Resource"), exported: false)
			}, null),
			new MethodInfo(MethodName.SetupActorWindow, new PropertyInfo(Variant.Type.Nil, "", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false), MethodFlags.Normal, new List<PropertyInfo>
			{
				new PropertyInfo(Variant.Type.Vector2I, "overridePos", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false),
				new PropertyInfo(Variant.Type.Object, "characterData", PropertyHint.None, "", PropertyUsageFlags.Default, new StringName("Resource"), exported: false)
			}, null),
			new MethodInfo(MethodName.PopActor, new PropertyInfo(Variant.Type.Nil, "", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false), MethodFlags.Normal, null, null),
			new MethodInfo(MethodName.HandlePop, new PropertyInfo(Variant.Type.Nil, "", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false), MethodFlags.Normal, null, null),
			new MethodInfo(MethodName.StartKill, new PropertyInfo(Variant.Type.Nil, "", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false), MethodFlags.Normal, null, null),
			new MethodInfo(MethodName._Process, new PropertyInfo(Variant.Type.Nil, "", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false), MethodFlags.Normal, new List<PropertyInfo>
			{
				new PropertyInfo(Variant.Type.Float, "delta", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false)
			}, null),
			new MethodInfo(MethodName.WalkToPlayer, new PropertyInfo(Variant.Type.Nil, "", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false), MethodFlags.Normal, new List<PropertyInfo>
			{
				new PropertyInfo(Variant.Type.Float, "delta", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false),
				new PropertyInfo(Variant.Type.Float, "targetX", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false),
				new PropertyInfo(Variant.Type.Float, "stoppingDistance", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false)
			}, null),
			new MethodInfo(MethodName.GetChaseStoppingDistance, new PropertyInfo(Variant.Type.Float, "", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false), MethodFlags.Normal, null, null),
			new MethodInfo(MethodName.GetCompanionStoppingDistance, new PropertyInfo(Variant.Type.Float, "", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false), MethodFlags.Normal, null, null),
			new MethodInfo(MethodName.GetCompanionEffectiveX, new PropertyInfo(Variant.Type.Float, "", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false), MethodFlags.Normal, new List<PropertyInfo>
			{
				new PropertyInfo(Variant.Type.Object, "companion", PropertyHint.None, "", PropertyUsageFlags.Default, new StringName("Window"), exported: false)
			}, null),
			new MethodInfo(MethodName.IsCompanionIdleXOverlapping, new PropertyInfo(Variant.Type.Bool, "", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false), MethodFlags.Normal, new List<PropertyInfo>
			{
				new PropertyInfo(Variant.Type.Float, "candidateX", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false),
				new PropertyInfo(Variant.Type.Int, "actorWidth", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false)
			}, null),
			new MethodInfo(MethodName.SyncCachesToScreen, new PropertyInfo(Variant.Type.Nil, "", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false), MethodFlags.Normal, new List<PropertyInfo>
			{
				new PropertyInfo(Variant.Type.Int, "screenIndex", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false),
				new PropertyInfo(Variant.Type.Rect2I, "screenRect", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false)
			}, null),
			new MethodInfo(MethodName.HandleCompanionAI, new PropertyInfo(Variant.Type.Nil, "", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false), MethodFlags.Normal, new List<PropertyInfo>
			{
				new PropertyInfo(Variant.Type.Float, "delta", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false)
			}, null),
			new MethodInfo(MethodName.HandleEnemyAI, new PropertyInfo(Variant.Type.Nil, "", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false), MethodFlags.Normal, new List<PropertyInfo>
			{
				new PropertyInfo(Variant.Type.Float, "delta", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false)
			}, null),
			new MethodInfo(MethodName.IsOverlappingTargetWindow, new PropertyInfo(Variant.Type.Bool, "", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false), MethodFlags.Normal, null, null),
			new MethodInfo(MethodName.IsOverlappingActorWindows, new PropertyInfo(Variant.Type.Bool, "", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false), MethodFlags.Normal, null, null),
			new MethodInfo(MethodName.SpawnItem, new PropertyInfo(Variant.Type.Nil, "", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false), MethodFlags.Normal, new List<PropertyInfo>
			{
				new PropertyInfo(Variant.Type.Float, "delta", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false)
			}, null),
			new MethodInfo(MethodName.OnAggroTimerTimeout, new PropertyInfo(Variant.Type.Nil, "", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false), MethodFlags.Normal, null, null),
			new MethodInfo(MethodName.HandleEnemyMouseFlee, new PropertyInfo(Variant.Type.Nil, "", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false), MethodFlags.Normal, new List<PropertyInfo>
			{
				new PropertyInfo(Variant.Type.Float, "delta", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false)
			}, null),
			new MethodInfo(MethodName.OnRandomAnimationTimerTimeout, new PropertyInfo(Variant.Type.Nil, "", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false), MethodFlags.Normal, null, null),
			new MethodInfo(MethodName.OnRandomConvoTimerTimeout, new PropertyInfo(Variant.Type.Nil, "", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false), MethodFlags.Normal, null, null),
			new MethodInfo(MethodName.ComputeThinnerCollisionBox, new PropertyInfo(Variant.Type.Rect2I, "", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false), MethodFlags.Normal, null, null),
			new MethodInfo(MethodName.IsInteractable, new PropertyInfo(Variant.Type.Bool, "", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false), MethodFlags.Normal, null, null),
			new MethodInfo(MethodName.HandleCindeshCompanionKinks, new PropertyInfo(Variant.Type.Nil, "", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false), MethodFlags.Normal, null, null)
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
		if (method == MethodName.DEBUG_LoadInScene && args.Count == 0)
		{
			DEBUG_LoadInScene();
			ret = default(godot_variant);
			return true;
		}
		if (method == MethodName.SetupActorWindow && args.Count == 1)
		{
			SetupActorWindow(VariantUtils.ConvertTo<CharacterInfoDataRes>(in args[0]));
			ret = default(godot_variant);
			return true;
		}
		if (method == MethodName.SetupActorWindow && args.Count == 2)
		{
			SetupActorWindow(VariantUtils.ConvertTo<Vector2I>(in args[0]), VariantUtils.ConvertTo<CharacterInfoDataRes>(in args[1]));
			ret = default(godot_variant);
			return true;
		}
		if (method == MethodName.PopActor && args.Count == 0)
		{
			PopActor();
			ret = default(godot_variant);
			return true;
		}
		if (method == MethodName.HandlePop && args.Count == 0)
		{
			HandlePop();
			ret = default(godot_variant);
			return true;
		}
		if (method == MethodName.StartKill && args.Count == 0)
		{
			StartKill();
			ret = default(godot_variant);
			return true;
		}
		if (method == MethodName._Process && args.Count == 1)
		{
			_Process(VariantUtils.ConvertTo<double>(in args[0]));
			ret = default(godot_variant);
			return true;
		}
		if (method == MethodName.WalkToPlayer && args.Count == 3)
		{
			WalkToPlayer(VariantUtils.ConvertTo<double>(in args[0]), VariantUtils.ConvertTo<float>(in args[1]), VariantUtils.ConvertTo<float>(in args[2]));
			ret = default(godot_variant);
			return true;
		}
		if (method == MethodName.GetChaseStoppingDistance && args.Count == 0)
		{
			float from = GetChaseStoppingDistance();
			ret = VariantUtils.CreateFrom(in from);
			return true;
		}
		if (method == MethodName.GetCompanionStoppingDistance && args.Count == 0)
		{
			float from2 = GetCompanionStoppingDistance();
			ret = VariantUtils.CreateFrom(in from2);
			return true;
		}
		if (method == MethodName.GetCompanionEffectiveX && args.Count == 1)
		{
			float from3 = GetCompanionEffectiveX(VariantUtils.ConvertTo<ActorWindow>(in args[0]));
			ret = VariantUtils.CreateFrom(in from3);
			return true;
		}
		if (method == MethodName.IsCompanionIdleXOverlapping && args.Count == 2)
		{
			bool from4 = IsCompanionIdleXOverlapping(VariantUtils.ConvertTo<float>(in args[0]), VariantUtils.ConvertTo<int>(in args[1]));
			ret = VariantUtils.CreateFrom(in from4);
			return true;
		}
		if (method == MethodName.SyncCachesToScreen && args.Count == 2)
		{
			SyncCachesToScreen(VariantUtils.ConvertTo<int>(in args[0]), VariantUtils.ConvertTo<Rect2I>(in args[1]));
			ret = default(godot_variant);
			return true;
		}
		if (method == MethodName.HandleCompanionAI && args.Count == 1)
		{
			HandleCompanionAI(VariantUtils.ConvertTo<double>(in args[0]));
			ret = default(godot_variant);
			return true;
		}
		if (method == MethodName.HandleEnemyAI && args.Count == 1)
		{
			HandleEnemyAI(VariantUtils.ConvertTo<double>(in args[0]));
			ret = default(godot_variant);
			return true;
		}
		if (method == MethodName.IsOverlappingTargetWindow && args.Count == 0)
		{
			bool from5 = IsOverlappingTargetWindow();
			ret = VariantUtils.CreateFrom(in from5);
			return true;
		}
		if (method == MethodName.IsOverlappingActorWindows && args.Count == 0)
		{
			bool from6 = IsOverlappingActorWindows();
			ret = VariantUtils.CreateFrom(in from6);
			return true;
		}
		if (method == MethodName.SpawnItem && args.Count == 1)
		{
			SpawnItem(VariantUtils.ConvertTo<double>(in args[0]));
			ret = default(godot_variant);
			return true;
		}
		if (method == MethodName.OnAggroTimerTimeout && args.Count == 0)
		{
			OnAggroTimerTimeout();
			ret = default(godot_variant);
			return true;
		}
		if (method == MethodName.HandleEnemyMouseFlee && args.Count == 1)
		{
			HandleEnemyMouseFlee(VariantUtils.ConvertTo<double>(in args[0]));
			ret = default(godot_variant);
			return true;
		}
		if (method == MethodName.OnRandomAnimationTimerTimeout && args.Count == 0)
		{
			OnRandomAnimationTimerTimeout();
			ret = default(godot_variant);
			return true;
		}
		if (method == MethodName.OnRandomConvoTimerTimeout && args.Count == 0)
		{
			OnRandomConvoTimerTimeout();
			ret = default(godot_variant);
			return true;
		}
		if (method == MethodName.ComputeThinnerCollisionBox && args.Count == 0)
		{
			Rect2I from7 = ComputeThinnerCollisionBox();
			ret = VariantUtils.CreateFrom(in from7);
			return true;
		}
		if (method == MethodName.IsInteractable && args.Count == 0)
		{
			bool from8 = IsInteractable();
			ret = VariantUtils.CreateFrom(in from8);
			return true;
		}
		if (method == MethodName.HandleCindeshCompanionKinks && args.Count == 0)
		{
			HandleCindeshCompanionKinks();
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
		if (method == MethodName.DEBUG_LoadInScene)
		{
			return true;
		}
		if (method == MethodName.SetupActorWindow)
		{
			return true;
		}
		if (method == MethodName.PopActor)
		{
			return true;
		}
		if (method == MethodName.HandlePop)
		{
			return true;
		}
		if (method == MethodName.StartKill)
		{
			return true;
		}
		if (method == MethodName._Process)
		{
			return true;
		}
		if (method == MethodName.WalkToPlayer)
		{
			return true;
		}
		if (method == MethodName.GetChaseStoppingDistance)
		{
			return true;
		}
		if (method == MethodName.GetCompanionStoppingDistance)
		{
			return true;
		}
		if (method == MethodName.GetCompanionEffectiveX)
		{
			return true;
		}
		if (method == MethodName.IsCompanionIdleXOverlapping)
		{
			return true;
		}
		if (method == MethodName.SyncCachesToScreen)
		{
			return true;
		}
		if (method == MethodName.HandleCompanionAI)
		{
			return true;
		}
		if (method == MethodName.HandleEnemyAI)
		{
			return true;
		}
		if (method == MethodName.IsOverlappingTargetWindow)
		{
			return true;
		}
		if (method == MethodName.IsOverlappingActorWindows)
		{
			return true;
		}
		if (method == MethodName.SpawnItem)
		{
			return true;
		}
		if (method == MethodName.OnAggroTimerTimeout)
		{
			return true;
		}
		if (method == MethodName.HandleEnemyMouseFlee)
		{
			return true;
		}
		if (method == MethodName.OnRandomAnimationTimerTimeout)
		{
			return true;
		}
		if (method == MethodName.OnRandomConvoTimerTimeout)
		{
			return true;
		}
		if (method == MethodName.ComputeThinnerCollisionBox)
		{
			return true;
		}
		if (method == MethodName.IsInteractable)
		{
			return true;
		}
		if (method == MethodName.HandleCindeshCompanionKinks)
		{
			return true;
		}
		return base.HasGodotClassMethod(in method);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override bool SetGodotClassPropertyValue(in godot_string_name name, in godot_variant value)
	{
		if (name == PropertyName.characterActor)
		{
			characterActor = VariantUtils.ConvertTo<ActorCharacter>(in value);
			return true;
		}
		if (name == PropertyName.spawnMargin)
		{
			spawnMargin = VariantUtils.ConvertTo<Vector2>(in value);
			return true;
		}
		if (name == PropertyName.randomConvoTimer)
		{
			randomConvoTimer = VariantUtils.ConvertTo<Timer>(in value);
			return true;
		}
		if (name == PropertyName.randomAnimationTimer)
		{
			randomAnimationTimer = VariantUtils.ConvertTo<Timer>(in value);
			return true;
		}
		if (name == PropertyName.aggroTimer)
		{
			aggroTimer = VariantUtils.ConvertTo<Timer>(in value);
			return true;
		}
		if (name == PropertyName.SpawnOnLoad)
		{
			SpawnOnLoad = VariantUtils.ConvertTo<bool>(in value);
			return true;
		}
		if (name == PropertyName.walkX)
		{
			walkX = VariantUtils.ConvertTo<float>(in value);
			return true;
		}
		if (name == PropertyName.softWalkTimer)
		{
			softWalkTimer = VariantUtils.ConvertTo<double>(in value);
			return true;
		}
		if (name == PropertyName.randomDistance)
		{
			randomDistance = VariantUtils.ConvertTo<float>(in value);
			return true;
		}
		if (name == PropertyName.distanceRanges)
		{
			distanceRanges = VariantUtils.ConvertTo<Vector2>(in value);
			return true;
		}
		if (name == PropertyName.popProgress)
		{
			popProgress = VariantUtils.ConvertTo<float>(in value);
			return true;
		}
		if (name == PropertyName.popShader)
		{
			popShader = VariantUtils.ConvertTo<ShaderMaterial>(in value);
			return true;
		}
		if (name == PropertyName.setAIType)
		{
			setAIType = VariantUtils.ConvertTo<CharacterInfoDataRes.AITypes>(in value);
			return true;
		}
		if (name == PropertyName.cachedActorScreenIndex)
		{
			cachedActorScreenIndex = VariantUtils.ConvertTo<int>(in value);
			return true;
		}
		if (name == PropertyName.cachedActorScreenRect)
		{
			cachedActorScreenRect = VariantUtils.ConvertTo<Rect2I>(in value);
			return true;
		}
		if (name == PropertyName.cachedThinBox)
		{
			cachedThinBox = VariantUtils.ConvertTo<Rect2I>(in value);
			return true;
		}
		if (name == PropertyName.spawnTime)
		{
			spawnTime = VariantUtils.ConvertTo<double>(in value);
			return true;
		}
		if (name == PropertyName.inUse)
		{
			inUse = VariantUtils.ConvertTo<bool>(in value);
			return true;
		}
		if (name == PropertyName.inUseByAttachment)
		{
			inUseByAttachment = VariantUtils.ConvertTo<bool>(in value);
			return true;
		}
		if (name == PropertyName.inAggro)
		{
			inAggro = VariantUtils.ConvertTo<bool>(in value);
			return true;
		}
		if (name == PropertyName.possibleAggroOverrideAnimations)
		{
			possibleAggroOverrideAnimations = VariantUtils.ConvertToArray<AttachDataRes>(in value);
			return true;
		}
		if (name == PropertyName.menu)
		{
			menu = VariantUtils.ConvertTo<ConfirmationMenu>(in value);
			return true;
		}
		if (name == PropertyName.targetWindow)
		{
			targetWindow = VariantUtils.ConvertTo<ActorWindow>(in value);
			return true;
		}
		if (name == PropertyName.mouseLingerTimer)
		{
			mouseLingerTimer = VariantUtils.ConvertTo<double>(in value);
			return true;
		}
		if (name == PropertyName.isFleeing)
		{
			isFleeing = VariantUtils.ConvertTo<bool>(in value);
			return true;
		}
		if (name == PropertyName.fleeTimer)
		{
			fleeTimer = VariantUtils.ConvertTo<double>(in value);
			return true;
		}
		if (name == PropertyName._enemyWalkDirection)
		{
			_enemyWalkDirection = VariantUtils.ConvertTo<float>(in value);
			return true;
		}
		if (name == PropertyName._enemyBounced)
		{
			_enemyBounced = VariantUtils.ConvertTo<bool>(in value);
			return true;
		}
		return base.SetGodotClassPropertyValue(in name, in value);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override bool GetGodotClassPropertyValue(in godot_string_name name, out godot_variant value)
	{
		if (name == PropertyName.characterActor)
		{
			value = VariantUtils.CreateFrom(in characterActor);
			return true;
		}
		if (name == PropertyName.spawnMargin)
		{
			value = VariantUtils.CreateFrom(in spawnMargin);
			return true;
		}
		if (name == PropertyName.randomConvoTimer)
		{
			value = VariantUtils.CreateFrom(in randomConvoTimer);
			return true;
		}
		if (name == PropertyName.randomAnimationTimer)
		{
			value = VariantUtils.CreateFrom(in randomAnimationTimer);
			return true;
		}
		if (name == PropertyName.aggroTimer)
		{
			value = VariantUtils.CreateFrom(in aggroTimer);
			return true;
		}
		if (name == PropertyName.SpawnOnLoad)
		{
			value = VariantUtils.CreateFrom(in SpawnOnLoad);
			return true;
		}
		if (name == PropertyName.walkX)
		{
			value = VariantUtils.CreateFrom(in walkX);
			return true;
		}
		if (name == PropertyName.softWalkTimer)
		{
			value = VariantUtils.CreateFrom(in softWalkTimer);
			return true;
		}
		if (name == PropertyName.randomDistance)
		{
			value = VariantUtils.CreateFrom(in randomDistance);
			return true;
		}
		if (name == PropertyName.distanceRanges)
		{
			value = VariantUtils.CreateFrom(in distanceRanges);
			return true;
		}
		if (name == PropertyName.popProgress)
		{
			value = VariantUtils.CreateFrom(in popProgress);
			return true;
		}
		if (name == PropertyName.popShader)
		{
			value = VariantUtils.CreateFrom(in popShader);
			return true;
		}
		if (name == PropertyName.setAIType)
		{
			value = VariantUtils.CreateFrom(in setAIType);
			return true;
		}
		if (name == PropertyName.cachedActorScreenIndex)
		{
			value = VariantUtils.CreateFrom(in cachedActorScreenIndex);
			return true;
		}
		if (name == PropertyName.cachedActorScreenRect)
		{
			value = VariantUtils.CreateFrom(in cachedActorScreenRect);
			return true;
		}
		if (name == PropertyName.cachedThinBox)
		{
			value = VariantUtils.CreateFrom(in cachedThinBox);
			return true;
		}
		if (name == PropertyName.spawnTime)
		{
			value = VariantUtils.CreateFrom(in spawnTime);
			return true;
		}
		if (name == PropertyName.inUse)
		{
			value = VariantUtils.CreateFrom(in inUse);
			return true;
		}
		if (name == PropertyName.inUseByAttachment)
		{
			value = VariantUtils.CreateFrom(in inUseByAttachment);
			return true;
		}
		if (name == PropertyName.inAggro)
		{
			value = VariantUtils.CreateFrom(in inAggro);
			return true;
		}
		if (name == PropertyName.possibleAggroOverrideAnimations)
		{
			value = VariantUtils.CreateFromArray(possibleAggroOverrideAnimations);
			return true;
		}
		if (name == PropertyName.menu)
		{
			value = VariantUtils.CreateFrom(in menu);
			return true;
		}
		if (name == PropertyName.targetWindow)
		{
			value = VariantUtils.CreateFrom(in targetWindow);
			return true;
		}
		if (name == PropertyName.mouseLingerTimer)
		{
			value = VariantUtils.CreateFrom(in mouseLingerTimer);
			return true;
		}
		if (name == PropertyName.isFleeing)
		{
			value = VariantUtils.CreateFrom(in isFleeing);
			return true;
		}
		if (name == PropertyName.fleeTimer)
		{
			value = VariantUtils.CreateFrom(in fleeTimer);
			return true;
		}
		if (name == PropertyName._enemyWalkDirection)
		{
			value = VariantUtils.CreateFrom(in _enemyWalkDirection);
			return true;
		}
		if (name == PropertyName._enemyBounced)
		{
			value = VariantUtils.CreateFrom(in _enemyBounced);
			return true;
		}
		return base.GetGodotClassPropertyValue(in name, out value);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	internal static List<PropertyInfo> GetGodotPropertyList()
	{
		return new List<PropertyInfo>
		{
			new PropertyInfo(Variant.Type.Object, PropertyName.characterActor, PropertyHint.NodeType, "Node2D", PropertyUsageFlags.Default | PropertyUsageFlags.ScriptVariable, exported: true),
			new PropertyInfo(Variant.Type.Vector2, PropertyName.spawnMargin, PropertyHint.None, "", PropertyUsageFlags.Default | PropertyUsageFlags.ScriptVariable, exported: true),
			new PropertyInfo(Variant.Type.Object, PropertyName.randomConvoTimer, PropertyHint.NodeType, "Timer", PropertyUsageFlags.Default | PropertyUsageFlags.ScriptVariable, exported: true),
			new PropertyInfo(Variant.Type.Object, PropertyName.randomAnimationTimer, PropertyHint.NodeType, "Timer", PropertyUsageFlags.Default | PropertyUsageFlags.ScriptVariable, exported: true),
			new PropertyInfo(Variant.Type.Object, PropertyName.aggroTimer, PropertyHint.NodeType, "Timer", PropertyUsageFlags.Default | PropertyUsageFlags.ScriptVariable, exported: true),
			new PropertyInfo(Variant.Type.Nil, "Debug", PropertyHint.None, "", PropertyUsageFlags.Group, exported: true),
			new PropertyInfo(Variant.Type.Bool, PropertyName.SpawnOnLoad, PropertyHint.None, "", PropertyUsageFlags.Default | PropertyUsageFlags.ScriptVariable, exported: true),
			new PropertyInfo(Variant.Type.Float, PropertyName.walkX, PropertyHint.None, "", PropertyUsageFlags.ScriptVariable, exported: false),
			new PropertyInfo(Variant.Type.Float, PropertyName.softWalkTimer, PropertyHint.None, "", PropertyUsageFlags.ScriptVariable, exported: false),
			new PropertyInfo(Variant.Type.Float, PropertyName.randomDistance, PropertyHint.None, "", PropertyUsageFlags.ScriptVariable, exported: false),
			new PropertyInfo(Variant.Type.Vector2, PropertyName.distanceRanges, PropertyHint.None, "", PropertyUsageFlags.ScriptVariable, exported: false),
			new PropertyInfo(Variant.Type.Float, PropertyName.popProgress, PropertyHint.None, "", PropertyUsageFlags.ScriptVariable, exported: false),
			new PropertyInfo(Variant.Type.Object, PropertyName.popShader, PropertyHint.None, "", PropertyUsageFlags.ScriptVariable, exported: false),
			new PropertyInfo(Variant.Type.Int, PropertyName.setAIType, PropertyHint.None, "", PropertyUsageFlags.ScriptVariable, exported: false),
			new PropertyInfo(Variant.Type.Int, PropertyName.cachedActorScreenIndex, PropertyHint.None, "", PropertyUsageFlags.ScriptVariable, exported: false),
			new PropertyInfo(Variant.Type.Rect2I, PropertyName.cachedActorScreenRect, PropertyHint.None, "", PropertyUsageFlags.ScriptVariable, exported: false),
			new PropertyInfo(Variant.Type.Rect2I, PropertyName.cachedThinBox, PropertyHint.None, "", PropertyUsageFlags.ScriptVariable, exported: false),
			new PropertyInfo(Variant.Type.Float, PropertyName.spawnTime, PropertyHint.None, "", PropertyUsageFlags.ScriptVariable, exported: false),
			new PropertyInfo(Variant.Type.Bool, PropertyName.inUse, PropertyHint.None, "", PropertyUsageFlags.ScriptVariable, exported: false),
			new PropertyInfo(Variant.Type.Bool, PropertyName.inUseByAttachment, PropertyHint.None, "", PropertyUsageFlags.ScriptVariable, exported: false),
			new PropertyInfo(Variant.Type.Bool, PropertyName.inAggro, PropertyHint.None, "", PropertyUsageFlags.ScriptVariable, exported: false),
			new PropertyInfo(Variant.Type.Array, PropertyName.possibleAggroOverrideAnimations, PropertyHint.None, "", PropertyUsageFlags.ScriptVariable, exported: false),
			new PropertyInfo(Variant.Type.Object, PropertyName.menu, PropertyHint.None, "", PropertyUsageFlags.ScriptVariable, exported: false),
			new PropertyInfo(Variant.Type.Object, PropertyName.targetWindow, PropertyHint.None, "", PropertyUsageFlags.ScriptVariable, exported: false),
			new PropertyInfo(Variant.Type.Float, PropertyName.mouseLingerTimer, PropertyHint.None, "", PropertyUsageFlags.ScriptVariable, exported: false),
			new PropertyInfo(Variant.Type.Bool, PropertyName.isFleeing, PropertyHint.None, "", PropertyUsageFlags.ScriptVariable, exported: false),
			new PropertyInfo(Variant.Type.Float, PropertyName.fleeTimer, PropertyHint.None, "", PropertyUsageFlags.ScriptVariable, exported: false),
			new PropertyInfo(Variant.Type.Float, PropertyName._enemyWalkDirection, PropertyHint.None, "", PropertyUsageFlags.ScriptVariable, exported: false),
			new PropertyInfo(Variant.Type.Bool, PropertyName._enemyBounced, PropertyHint.None, "", PropertyUsageFlags.ScriptVariable, exported: false)
		};
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override void SaveGodotObjectData(GodotSerializationInfo info)
	{
		base.SaveGodotObjectData(info);
		info.AddProperty(PropertyName.characterActor, Variant.From(in characterActor));
		info.AddProperty(PropertyName.spawnMargin, Variant.From(in spawnMargin));
		info.AddProperty(PropertyName.randomConvoTimer, Variant.From(in randomConvoTimer));
		info.AddProperty(PropertyName.randomAnimationTimer, Variant.From(in randomAnimationTimer));
		info.AddProperty(PropertyName.aggroTimer, Variant.From(in aggroTimer));
		info.AddProperty(PropertyName.SpawnOnLoad, Variant.From(in SpawnOnLoad));
		info.AddProperty(PropertyName.walkX, Variant.From(in walkX));
		info.AddProperty(PropertyName.softWalkTimer, Variant.From(in softWalkTimer));
		info.AddProperty(PropertyName.randomDistance, Variant.From(in randomDistance));
		info.AddProperty(PropertyName.distanceRanges, Variant.From(in distanceRanges));
		info.AddProperty(PropertyName.popProgress, Variant.From(in popProgress));
		info.AddProperty(PropertyName.popShader, Variant.From(in popShader));
		info.AddProperty(PropertyName.setAIType, Variant.From(in setAIType));
		info.AddProperty(PropertyName.cachedActorScreenIndex, Variant.From(in cachedActorScreenIndex));
		info.AddProperty(PropertyName.cachedActorScreenRect, Variant.From(in cachedActorScreenRect));
		info.AddProperty(PropertyName.cachedThinBox, Variant.From(in cachedThinBox));
		info.AddProperty(PropertyName.spawnTime, Variant.From(in spawnTime));
		info.AddProperty(PropertyName.inUse, Variant.From(in inUse));
		info.AddProperty(PropertyName.inUseByAttachment, Variant.From(in inUseByAttachment));
		info.AddProperty(PropertyName.inAggro, Variant.From(in inAggro));
		info.AddProperty(PropertyName.possibleAggroOverrideAnimations, Variant.CreateFrom(possibleAggroOverrideAnimations));
		info.AddProperty(PropertyName.menu, Variant.From(in menu));
		info.AddProperty(PropertyName.targetWindow, Variant.From(in targetWindow));
		info.AddProperty(PropertyName.mouseLingerTimer, Variant.From(in mouseLingerTimer));
		info.AddProperty(PropertyName.isFleeing, Variant.From(in isFleeing));
		info.AddProperty(PropertyName.fleeTimer, Variant.From(in fleeTimer));
		info.AddProperty(PropertyName._enemyWalkDirection, Variant.From(in _enemyWalkDirection));
		info.AddProperty(PropertyName._enemyBounced, Variant.From(in _enemyBounced));
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override void RestoreGodotObjectData(GodotSerializationInfo info)
	{
		base.RestoreGodotObjectData(info);
		if (info.TryGetProperty(PropertyName.characterActor, out var value))
		{
			characterActor = value.As<ActorCharacter>();
		}
		if (info.TryGetProperty(PropertyName.spawnMargin, out var value2))
		{
			spawnMargin = value2.As<Vector2>();
		}
		if (info.TryGetProperty(PropertyName.randomConvoTimer, out var value3))
		{
			randomConvoTimer = value3.As<Timer>();
		}
		if (info.TryGetProperty(PropertyName.randomAnimationTimer, out var value4))
		{
			randomAnimationTimer = value4.As<Timer>();
		}
		if (info.TryGetProperty(PropertyName.aggroTimer, out var value5))
		{
			aggroTimer = value5.As<Timer>();
		}
		if (info.TryGetProperty(PropertyName.SpawnOnLoad, out var value6))
		{
			SpawnOnLoad = value6.As<bool>();
		}
		if (info.TryGetProperty(PropertyName.walkX, out var value7))
		{
			walkX = value7.As<float>();
		}
		if (info.TryGetProperty(PropertyName.softWalkTimer, out var value8))
		{
			softWalkTimer = value8.As<double>();
		}
		if (info.TryGetProperty(PropertyName.randomDistance, out var value9))
		{
			randomDistance = value9.As<float>();
		}
		if (info.TryGetProperty(PropertyName.distanceRanges, out var value10))
		{
			distanceRanges = value10.As<Vector2>();
		}
		if (info.TryGetProperty(PropertyName.popProgress, out var value11))
		{
			popProgress = value11.As<float>();
		}
		if (info.TryGetProperty(PropertyName.popShader, out var value12))
		{
			popShader = value12.As<ShaderMaterial>();
		}
		if (info.TryGetProperty(PropertyName.setAIType, out var value13))
		{
			setAIType = value13.As<CharacterInfoDataRes.AITypes>();
		}
		if (info.TryGetProperty(PropertyName.cachedActorScreenIndex, out var value14))
		{
			cachedActorScreenIndex = value14.As<int>();
		}
		if (info.TryGetProperty(PropertyName.cachedActorScreenRect, out var value15))
		{
			cachedActorScreenRect = value15.As<Rect2I>();
		}
		if (info.TryGetProperty(PropertyName.cachedThinBox, out var value16))
		{
			cachedThinBox = value16.As<Rect2I>();
		}
		if (info.TryGetProperty(PropertyName.spawnTime, out var value17))
		{
			spawnTime = value17.As<double>();
		}
		if (info.TryGetProperty(PropertyName.inUse, out var value18))
		{
			inUse = value18.As<bool>();
		}
		if (info.TryGetProperty(PropertyName.inUseByAttachment, out var value19))
		{
			inUseByAttachment = value19.As<bool>();
		}
		if (info.TryGetProperty(PropertyName.inAggro, out var value20))
		{
			inAggro = value20.As<bool>();
		}
		if (info.TryGetProperty(PropertyName.possibleAggroOverrideAnimations, out var value21))
		{
			possibleAggroOverrideAnimations = value21.AsGodotArray<AttachDataRes>();
		}
		if (info.TryGetProperty(PropertyName.menu, out var value22))
		{
			menu = value22.As<ConfirmationMenu>();
		}
		if (info.TryGetProperty(PropertyName.targetWindow, out var value23))
		{
			targetWindow = value23.As<ActorWindow>();
		}
		if (info.TryGetProperty(PropertyName.mouseLingerTimer, out var value24))
		{
			mouseLingerTimer = value24.As<double>();
		}
		if (info.TryGetProperty(PropertyName.isFleeing, out var value25))
		{
			isFleeing = value25.As<bool>();
		}
		if (info.TryGetProperty(PropertyName.fleeTimer, out var value26))
		{
			fleeTimer = value26.As<double>();
		}
		if (info.TryGetProperty(PropertyName._enemyWalkDirection, out var value27))
		{
			_enemyWalkDirection = value27.As<float>();
		}
		if (info.TryGetProperty(PropertyName._enemyBounced, out var value28))
		{
			_enemyBounced = value28.As<bool>();
		}
	}
}
