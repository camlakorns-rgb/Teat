using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Godot;
using Godot.Bridge;
using Godot.Collections;
using Godot.NativeInterop;

[ScriptPath("res://Scripts/CharacterScripts/ActorWindow.cs")]
public partial class ActorWindow : Window
{

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


	private bool isSetup = false;
	public bool IsSetup => isSetup;
	public bool IsActiveForMobile => isSetup;

		// Mobile renderer: render NPC as sprite in scene root viewport to avoid Window flickering
	private Node2D _mobileSpriteRoot;
	private AnimatedSprite2D _mobileSprite;

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
        // V34: Bit and Trojan tiny and floating
        // V35: still small + floating higher - boosted too much and mismatched sizes
        // V37: Bit higher and smaller - boost 4.5x and full screen ground
        if (Main._isMobile && characterActor.characterInformation != null)
        {
            string id = characterActor.characterInformation._itemID?.ToLower() ?? "";
            string name = characterActor.characterInformation.Name?.ToLower() ?? "";
            if (id.Contains("bit") || id.Contains("qubit") || id.Contains("trojan") || name.Contains("bit") || name.Contains("trojan") || id.Contains("1_bit"))
            {
                float boost = 4.5f;
                characterActor.trueSize = (Vector2I)((Vector2)characterActor.trueSize * boost);
                if (characterActor.MainBody != null)
                    characterActor.MainBody.Scale *= boost;
            }
        }
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
		setAIType = characterActor.characterInformation.AITyping;
		
		isSetup = true;
		// Mobile renderer: V30 Visible=false, V33 fix Bit floating
		if (Main._isMobile)
		{
			base.Transparent = true;
			base.TransparentBg = true;
			base.Visible = false;
			// V33: reset actor sprite offset to avoid floating (Bit was floating)
			if (characterActor != null && characterActor.spriteParentController != null)
			{
				characterActor.spriteParentController.Position = Vector2.Zero;
			}
			if (characterActor != null && characterActor.MainBody != null)
			{
				characterActor.MainBody.Visible = false;
			}
			_mobileSprite = new AnimatedSprite2D();
			if (characterActor != null && characterActor.MainBody != null)
			{
				_mobileSprite.SpriteFrames = characterActor.MainBody.SpriteFrames;
				_mobileSprite.Scale = characterActor.MainBody.Scale;
				_mobileSprite.Position = characterActor.MainBody.Position;
				_mobileSprite.Play(characterActor.MainBody.Animation);
				_mobileSprite.Frame = characterActor.MainBody.Frame;
				_mobileSprite.FlipH = characterActor.MainBody.FlipH;
				_mobileSprite.FlipV = characterActor.MainBody.FlipV;
			}
			
			_mobileSpriteRoot = new Node2D();
			_mobileSpriteRoot.Position = (Vector2)base.Position;
			_mobileSpriteRoot.Visible = true;
			_mobileSpriteRoot.AddChild(_mobileSprite);
			
			// Add to scene tree root (NOT Main.Instance) so it doesn't move with Byte
			try { GetTree().Root.AddChild(_mobileSpriteRoot); } catch {}
		}
		else
		{
			base.Visible = true;
		}
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
		if (!cachedThinBox.HasPoint(Main._isMobile ? (Vector2I)Main.Instance.MobileMousePos() : DisplayServer.MouseGetPosition()))
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
			WalkToPlayer(delta, (targetWindow != null) ? targetWindow.Position.X : (Main._isMobile ? Main.Instance.Position.X : Main.Instance.mainWindow.Position.X), chaseStoppingDistance);
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
		if (Main._isMobile && _mobileSpriteRoot != null && GodotObject.IsInstanceValid(_mobileSpriteRoot))
		{
			_mobileSpriteRoot.Position = (Vector2)base.Position;
			_mobileSpriteRoot.Visible = IsActiveForMobile && characterActor != null && characterActor.Visible;
			if (_mobileSprite != null && GodotObject.IsInstanceValid(_mobileSprite) && characterActor != null && characterActor.MainBody != null)
			{
				if (_mobileSprite.SpriteFrames != characterActor.MainBody.SpriteFrames)
					_mobileSprite.SpriteFrames = characterActor.MainBody.SpriteFrames;
				_mobileSprite.Animation = characterActor.MainBody.Animation;
				_mobileSprite.Frame = characterActor.MainBody.Frame;
				_mobileSprite.Scale = characterActor.MainBody.Scale;
				_mobileSprite.Position = characterActor.MainBody.Position;
				_mobileSprite.FlipH = characterActor.MainBody.FlipH;
				_mobileSprite.FlipV = characterActor.MainBody.FlipV;
			}
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
		if (Mathf.Abs(walkX - targetX) > stoppingDistance * 0.9f)
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
				num2 = ((!((float)(Main._isMobile ? Main.Instance.MobileMousePos() : DisplayServer.MouseGetPosition()).X > walkX)) ? 1 : (-1));
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
			int groundY;
			if (Main._isMobile)
			{
				Vector2I screenSize = DisplayServer.ScreenGetSize(Main.Instance.screenDataHandler.screenIndex);
				groundY = screenSize.Y - characterActor.trueSize.Y;
			}
			else
			{
				groundY = cachedActorScreenRect.End.Y - characterActor.trueSize.Y;
			}
			Vector2I newPos = new Vector2I(Main.Instance.screenDataHandler.ClampAcrossAllScreensX(Mathf.RoundToInt(walkX), characterActor.trueSize.X), groundY);
			if (newPos != base.Position) base.Position = newPos;
			// Sync mobile sprite
			if (Main._isMobile && _mobileSpriteRoot != null && GodotObject.IsInstanceValid(_mobileSpriteRoot))
			{
				_mobileSpriteRoot.Position = new Vector2(newPos.X, newPos.Y);
				if (_mobileSprite != null && GodotObject.IsInstanceValid(_mobileSprite))
				{
					_mobileSprite.Play(characterActor.MainBody.Animation);
					_mobileSprite.FlipH = characterActor.MainBody.FlipH;
				}
			}
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
				float num = (float)(Main._isMobile ? Main.Instance.Position.X : Main.Instance.mainWindow.Position.X) + (float)Main.Instance.mainCharacter.trueSize.X / 2f;
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
		if (Mathf.Abs(walkX - (float)(Main._isMobile ? Main.Instance.Position.X : Main.Instance.mainWindow.Position.X)) >= num4 && characterActor.MainBody.Animation != (StringName)"Walk")
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
		bool shouldShow = true;
		if (!Main.Instance.mainCharacter.Visible && inUse)
		{
			shouldShow = false;
		}
		else if (inUseByAttachment)
		{
			shouldShow = false;
		}
		else
		{
			inUse = false;
		}
		if (base.Visible != shouldShow && !Main._isMobile) base.Visible = shouldShow;
		if (Main._isMobile && _mobileSpriteRoot != null && GodotObject.IsInstanceValid(_mobileSpriteRoot))
			_mobileSpriteRoot.Visible = shouldShow;
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
		if (Input.IsActionJustPressed("Pet") && Main.Instance.mainCharacter.Visible && !Main.Instance.SomethingHasBeenGrabbed && cachedThinBox.HasPoint(Main._isMobile ? (Vector2I)Main.Instance.MobileMousePos() : DisplayServer.MouseGetPosition()))
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
		if (IsOverlappingTargetWindow() && Input.IsActionJustReleased("Move") && IsInteractable() && num < 800f && new Rect2I(Main._isMobile ? (Vector2I)Main.Instance.Position : Main.Instance.mainWindow.Position, Main._isMobile ? Main.Instance.mainCharacter.trueSize : Main.Instance.mainWindow.Size).HasPoint(Main._isMobile ? (Vector2I)Main.Instance.MobileMousePos() : DisplayServer.MouseGetPosition()))
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
		if (!base.Visible && !Main._isMobile) base.Visible = true;
		if (Main._isMobile && _mobileSpriteRoot != null && GodotObject.IsInstanceValid(_mobileSpriteRoot))
			_mobileSpriteRoot.Visible = true;
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
		Rect2I b = Main._isMobile ? new Rect2I((Vector2I)Main.Instance.Position, Main.Instance.mainCharacter.trueSize) : new Rect2I(Main.Instance.mainWindow.Position, Main.Instance.mainWindow.Size);
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
		else if (cachedThinBox.HasPoint(Main._isMobile ? (Vector2I)Main.Instance.MobileMousePos() : DisplayServer.MouseGetPosition()))
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


	public override void _ExitTree()
	{
		if (_mobileSpriteRoot != null && GodotObject.IsInstanceValid(_mobileSpriteRoot))
		{
			_mobileSpriteRoot.QueueFree();
			_mobileSpriteRoot = null;
		}
		base._ExitTree();
	}

}
