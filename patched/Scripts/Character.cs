using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using Godot;
using Godot.Bridge;
using Godot.Collections;
using Godot.NativeInterop;

[ScriptPath("res://Scripts/CharacterScripts/Character.cs")]
public partial class Character : Node2D
{
	public enum MainBodyStates
	{
		Idle,
		Walk,
		Sit,
		Dangled,
		Thrown,
		Forced_Animation,
		Transition
	}

	public struct MainBodyAnimationStack
	{
		public MainBodyStates State;

		public string animationName;

		public bool reversed;

		public Action AttachedTask;

		public MainBodyAnimationStack(MainBodyStates state, string animationName, bool reversed = false, Action attachedTask = null)
		{
			State = state;
			this.animationName = animationName;
			this.reversed = reversed;
			AttachedTask = attachedTask;
		}
	}

	[Signal]
	public delegate void WalkingEventHandler();

	[Signal]
	public delegate void FinishedWalkingEventHandler();

	[Export(PropertyHint.None, "")]
	public CharacterInfoDataRes characterInformation;

	[Export(PropertyHint.None, "")]
	public SpriteParentController spriteParentController;

	[Export(PropertyHint.None, "")]
	public Node TimerParent;

	[Export(PropertyHint.None, "")]
	public Timer mainBodyTimer;

	[Export(PropertyHint.None, "")]
	public Timer faceBlinkTimer;

	public MainBodyStates petMainBodyState;

	public string SelectedClothingState;

	private int SelectedClothingIndex;

	public Vector2I trueSize;

	public Array<TagDataRes> existingTags = new Array<TagDataRes>();

	public AnimatedSprite2D MainBody;

	public AnimatedSprite2D FaceBody;

	public Godot.Collections.Dictionary<AnimatedSprite2D, CharAnimDataRes> attachedBodies = new Godot.Collections.Dictionary<AnimatedSprite2D, CharAnimDataRes>();

	private Action _sitTransitionHandler;

	private Action _animationTransitionHandler;

	private Action _animationEndingHandler;

	private Action _currentAnimationCallback;

	private bool allowBlink;

	private string heldAnimation;

	private bool _checkingStateOverrides;

	private Vector2 dandleOffset = Vector2.Zero;

	private Vector2 dandleVelocity = Vector2.Zero;

	private const float DandleStiffness = 30f;

	private const float DandleDamping = 0.92f;

	private const float DandleInfluence = 0.35f;

	private const float DandleRotationInfluence = -0.015f;

	public bool isDangling;

	private float throwRotation;

	private float throwRotationVelocity;

	private const float ThrowRotationDecay = 2.5f;

	private const float ThrowSpeedThreshold = 4000f;

	public bool throwWasHard;

	private Vector2 spriteParentOriginalPos = Vector2.Zero;

	private Vector2 lastMousePos = Vector2.Zero;

	public Vector2 mouseVelocity = Vector2.Zero;

	public void SetupCharacter()
	{
		SelectedClothingState = characterInformation.ClothingStates[0];
		petMainBodyState = MainBodyStates.Idle;
		mainBodyTimer.Start();
		spriteParentController.setupCharacterSprites(this);
		// V32: reset offset on mobile to avoid floating
		if (Main._isMobile)
		{
			spriteParentController.Position = Vector2.Zero;
		}
		spriteParentOriginalPos = spriteParentController.Position;
		MainBody.AnimationLooped += Blink;
		StartBlinkTimer();
		CheckAttachedBodies();
	}

	public override void _Process(double delta)
	{
		UpdateTags(delta);
	}

	public void FlipHSpriteTo(bool FlipH)
	{
		foreach (AnimatedSprite2D child in spriteParentController.GetChildren())
		{
			child.FlipH = !FlipH;
		}
	}

	public void SelectClothing(int i)
	{
		int num = characterInformation.ClothingStates.Count();
		int selectedClothingIndex = SelectedClothingIndex;
		int num2 = selectedClothingIndex;
		for (int j = 0; j < num; j++)
		{
			num2 = (num2 + i + num) % num;
			if (!characterInformation.ClothingStates[num2].Contains("substar") || OS.HasFeature("substar") || OS.HasFeature("editor"))
			{
				break;
			}
		}
		if (MainBody.SpriteFrames.GetAnimationNames().Contains(characterInformation.ClothingStates[num2]))
		{
			SelectedClothingState = characterInformation.ClothingStates[num2];
		}
		else
		{
			AddTag(new TagDataRes
			{
				tagName = characterInformation.ClothingStates[num2],
				tagDuration = -1999f,
				tagOriginalDuration = -1999f
			});
			TagDataRes tag = GetTag(characterInformation.ClothingStates[selectedClothingIndex]);
			if (tag != null)
			{
				RemoveTag(tag);
			}
		}
		if (petMainBodyState == MainBodyStates.Idle)
		{
			ForceMainBodyState(MainBodyStates.Forced_Animation, "Pet", 0.5f);
		}
		SelectedClothingIndex = num2;
	}

	public void Grabbed(bool isGrabbed)
	{
		if (isGrabbed)
		{
			if (_animationEndingHandler != null)
			{
				MainBody.AnimationLooped -= _animationEndingHandler;
				MainBody.AnimationFinished -= _animationEndingHandler;
				_animationEndingHandler = null;
			}
			if (_animationTransitionHandler != null)
			{
				MainBody.AnimationLooped -= _animationTransitionHandler;
				MainBody.AnimationFinished -= _animationTransitionHandler;
				_animationTransitionHandler = null;
			}
			petMainBodyState = MainBodyStates.Dangled;
			ForceMainBodyState(petMainBodyState, "Dangled");
			mainBodyTimer.Stop();
			StartDangle();
		}
		else
		{
			petMainBodyState = MainBodyStates.Idle;
			ForceMainBodyState(petMainBodyState, "Idle");
			mainBodyTimer.Start();
			StopDangle();
		}
	}

	public void StartDangle()
	{
		isDangling = true;
		dandleOffset = Vector2.Zero;
		dandleVelocity = Vector2.Zero;
		lastMousePos = Main._isMobile ? (Vector2)Main.Instance.MobileMousePos() : DisplayServer.MouseGetPosition();
	}

	public void UpdateDangle(double delta)
	{
		if (isDangling)
		{
			Vector2 vector = Main._isMobile ? (Vector2)Main.Instance.MobileMousePos() : DisplayServer.MouseGetPosition();
			mouseVelocity = (vector - lastMousePos) / (float)delta;
			lastMousePos = vector;
			float num = mouseVelocity.X * 0.35f * (float)delta / spriteParentController.Scale.X;
			dandleVelocity.X -= num;
			float num2 = (spriteParentOriginalPos.X - (spriteParentOriginalPos.X + dandleOffset.X)) * 30f;
			dandleVelocity.X += num2 * (float)delta;
			dandleVelocity.X *= Mathf.Pow(0.92f, (float)delta * 60f);
			dandleOffset.X += dandleVelocity.X * (float)delta;
			dandleOffset.Y = 0f;
			spriteParentController.Position = spriteParentOriginalPos + dandleOffset;
			spriteParentController.Rotation = dandleOffset.X * -0.015f;
		}
	}

	public void StopDangle()
	{
		isDangling = false;
		spriteParentController.Position = spriteParentOriginalPos;
		if (Main.Instance.settingWindowThrowPhysics)
		{
			throwRotation = spriteParentController.Rotation;
			throwRotationVelocity = dandleVelocity.X * -0.015f * -8f;
		}
		else
		{
			spriteParentController.Rotation = 0f;
		}
	}

	public void BeginThrow(Vector2 throwVelocity)
	{
		throwWasHard = throwVelocity.Length() >= 4000f;
		petMainBodyState = MainBodyStates.Thrown;
		MasterAnimationHandler(throwWasHard ? "Hard_Throw" : "Soft_Throw");
		TryQueueThrowReaction();
	}

	public void ApplyBounceRotation(float newXVelocity)
	{
		throwRotationVelocity = newXVelocity * -0.015f * 6f;
	}

	public void UpdateThrowRotation(double delta)
	{
		if (Mathf.Abs(throwRotation) < 0.001f && Mathf.Abs(throwRotationVelocity) < 0.001f)
		{
			throwRotation = 0f;
			throwRotationVelocity = 0f;
			spriteParentController.Rotation = 0f;
			return;
		}
		float num = (0f - throwRotation) * 2.5f;
		throwRotationVelocity += num * (float)delta;
		throwRotationVelocity *= Mathf.Pow(0.9f, (float)delta * 60f);
		throwRotation += throwRotationVelocity * (float)delta;
		if (!throwWasHard)
		{
			throwRotation = Mathf.Clamp(throwRotation, -0.6f, 0.6f);
		}
		spriteParentController.Rotation = throwRotation;
	}

	public void CancelThrowRotation()
	{
		throwRotation = 0f;
		throwRotationVelocity = 0f;
		spriteParentController.Rotation = 0f;
	}

	public void BeginLand()
	{
		string animationName = (throwWasHard ? "Hard_Land" : "Soft_Land");
		ForceMainBodyStateQueue(new MainBodyAnimationStack[1]
		{
			new MainBodyAnimationStack(MainBodyStates.Thrown, animationName)
		});
		TryQueueLandReaction();
	}

	private void TryQueueThrowReaction()
	{
		float chancePercent = (throwWasHard ? Main.Instance.throwHardReactionChance : Main.Instance.throwSoftReactionChance);
		TryQueueThrowLandReaction(chancePercent);
	}

	private void TryQueueLandReaction()
	{
		float chancePercent = (throwWasHard ? Main.Instance.landHardReactionChance : Main.Instance.landSoftReactionChance);
		TryQueueThrowLandReaction(chancePercent);
	}

	private void TryQueueThrowLandReaction(float chancePercent)
	{
		if (!(GD.RandRange(0.0, 100.0) > (double)chancePercent))
		{
			CharacterInfoDataRes.ResponseToSituation key = (throwWasHard ? CharacterInfoDataRes.ResponseToSituation.THROWN_HARD : CharacterInfoDataRes.ResponseToSituation.THROWN_SOFT);
			if (characterInformation.responseTexts.ContainsKey(key))
			{
				Main.Instance.ClearAllAttachments();
				Main.Instance.dialogueStack.Add(characterInformation.responseTexts[key]);
				Main.Instance.PopDialogueInStack(skipTimer: true);
			}
		}
	}

	public void MasterAnimationHandler(string newAnimation, bool reverse = false)
	{
		if (!MainBody.SpriteFrames.GetAnimationNames().Contains(newAnimation + "_" + SelectedClothingState))
		{
			GD.PrintErr("[" + newAnimation + "] is not a valid animation! Check to make sure it has the proper _ clothing or if it exists");
			return;
		}
		if (!reverse)
		{
			MainBody.Play(newAnimation + "_" + SelectedClothingState);
			if (FaceBody != null)
			{
				FaceBody.Play(newAnimation + "_Face");
			}
			{
				foreach (AnimatedSprite2D key in attachedBodies.Keys)
				{
					if (key.SpriteFrames.HasAnimation(newAnimation))
					{
						key.Play(newAnimation);
					}
					else if (key.SpriteFrames.HasAnimation(newAnimation + "_" + SelectedClothingState))
					{
						key.Play(newAnimation + "_" + SelectedClothingState);
					}
				}
				return;
			}
		}
		MainBody.PlayBackwards(newAnimation + "_" + SelectedClothingState);
		if (FaceBody != null)
		{
			FaceBody.PlayBackwards(newAnimation + "_Face");
		}
		foreach (AnimatedSprite2D key2 in attachedBodies.Keys)
		{
			if (key2.SpriteFrames.HasAnimation(newAnimation))
			{
				key2.PlayBackwards(newAnimation);
			}
			else if (key2.SpriteFrames.HasAnimation(newAnimation + "_" + SelectedClothingState))
			{
				key2.PlayBackwards(newAnimation + "_" + SelectedClothingState);
			}
		}
	}

	public void ForceMainBodyState(MainBodyStates newState, string newAnimation, bool reverse = false)
	{
		petMainBodyState = newState;
		MasterAnimationHandler(newAnimation, reverse);
		if (petMainBodyState == MainBodyStates.Idle || petMainBodyState == MainBodyStates.Walk)
		{
			mainBodyTimer.WaitTime = GD.RandRange(5, 10);
			mainBodyTimer.Start();
		}
	}

	public void ForceMainBodyState(MainBodyStates newState, string newAnimation, float newTime, bool reverse = false)
	{
		petMainBodyState = newState;
		MasterAnimationHandler(newAnimation, reverse);
		mainBodyTimer.WaitTime = newTime;
		mainBodyTimer.Start();
	}

	public void ForceMainBodyStateQueue(MainBodyAnimationStack[] animationQueue)
	{
		if (animationQueue == null || animationQueue.Length == 0)
		{
			return;
		}
		int currentIndex = 0;
		Action playNext = null;
		Action onAnimationStep = null;
		playNext = delegate
		{
			if (currentIndex >= animationQueue.Length)
			{
				if (mainBodyTimer.IsStopped())
				{
					ForceMainBodyState(MainBodyStates.Idle, "Idle");
					mainBodyTimer.WaitTime = 1.0;
					mainBodyTimer.Start();
				}
			}
			else
			{
				MainBodyAnimationStack entry = animationQueue[currentIndex];
				currentIndex++;
				if (onAnimationStep != null)
				{
					MainBody.AnimationLooped -= onAnimationStep;
					MainBody.AnimationFinished -= onAnimationStep;
					onAnimationStep = null;
				}
				petMainBodyState = entry.State;
				MasterAnimationHandler(entry.animationName, entry.reversed);
				bool fired = false;
				onAnimationStep = delegate
				{
					if (!fired)
					{
						fired = true;
						MainBody.AnimationLooped -= onAnimationStep;
						MainBody.AnimationFinished -= onAnimationStep;
						onAnimationStep = null;
						entry.AttachedTask?.Invoke();
						playNext();
					}
				};
				MainBody.AnimationLooped += onAnimationStep;
				MainBody.AnimationFinished += onAnimationStep;
			}
		};
		mainBodyTimer.Stop();
		playNext();
	}

	public void ForceMainBodyStateTransition(string newAnimation, float newTime)
	{
		heldAnimation = newAnimation;
		if (_animationTransitionHandler != null)
		{
			MainBody.AnimationLooped -= _animationTransitionHandler;
			MainBody.AnimationFinished -= _animationTransitionHandler;
			_animationTransitionHandler = null;
		}
		petMainBodyState = MainBodyStates.Transition;
		MasterAnimationHandler(newAnimation + "_Transition");
		_animationTransitionHandler = delegate
		{
			MainBody.AnimationLooped -= _animationTransitionHandler;
			MainBody.AnimationFinished -= _animationTransitionHandler;
			_animationTransitionHandler = null;
			MasterAnimationHandler(newAnimation);
			mainBodyTimer.WaitTime = newTime;
			mainBodyTimer.Start();
		};
		MainBody.AnimationLooped += _animationTransitionHandler;
		MainBody.AnimationFinished += _animationTransitionHandler;
	}

	public async void MainBodyTimerTimeOut()
	{
		if (petMainBodyState == MainBodyStates.Transition)
		{
			if (_animationEndingHandler != null)
			{
				MainBody.AnimationLooped -= _animationEndingHandler;
				MainBody.AnimationFinished -= _animationEndingHandler;
				_animationEndingHandler = null;
			}
			petMainBodyState = MainBodyStates.Transition;
			MasterAnimationHandler(heldAnimation + "_Transition", reverse: true);
			_animationEndingHandler = delegate
			{
				if (petMainBodyState == MainBodyStates.Transition)
				{
					MainBody.AnimationLooped -= _animationEndingHandler;
					MainBody.AnimationFinished -= _animationEndingHandler;
					_animationEndingHandler = null;
					heldAnimation = "";
					petMainBodyState = MainBodyStates.Idle;
					MasterAnimationHandler(petMainBodyState.ToString());
					mainBodyTimer.WaitTime = 5.0;
					mainBodyTimer.Start();
				}
				else
				{
					MainBody.AnimationLooped -= _animationEndingHandler;
					MainBody.AnimationFinished -= _animationEndingHandler;
					_animationEndingHandler = null;
					heldAnimation = "";
					mainBodyTimer.WaitTime = 5.0;
					mainBodyTimer.Start();
				}
			};
			MainBody.AnimationLooped += _animationEndingHandler;
			MainBody.AnimationFinished += _animationEndingHandler;
		}
		else if (petMainBodyState == MainBodyStates.Forced_Animation)
		{
			if (_animationEndingHandler != null)
			{
				MainBody.AnimationLooped -= _animationEndingHandler;
				MainBody.AnimationFinished -= _animationEndingHandler;
				_animationEndingHandler = null;
			}
			_animationEndingHandler = delegate
			{
				if (petMainBodyState == MainBodyStates.Forced_Animation)
				{
					MainBody.AnimationLooped -= _animationEndingHandler;
					MainBody.AnimationFinished -= _animationEndingHandler;
					_animationEndingHandler = null;
					petMainBodyState = MainBodyStates.Idle;
					MasterAnimationHandler(petMainBodyState.ToString());
					mainBodyTimer.WaitTime = 5.0;
					mainBodyTimer.Start();
				}
				else
				{
					MainBody.AnimationLooped -= _animationEndingHandler;
					MainBody.AnimationFinished -= _animationEndingHandler;
					_animationEndingHandler = null;
					mainBodyTimer.WaitTime = 5.0;
					mainBodyTimer.Start();
				}
			};
			MainBody.AnimationLooped += _animationEndingHandler;
			MainBody.AnimationFinished += _animationEndingHandler;
		}
		else
		{
			if (petMainBodyState == MainBodyStates.Walk)
			{
				EmitSignal(SignalName.FinishedWalking);
			}
			await ChangeStateBetweenIdleWalk();
			switch (petMainBodyState)
			{
			case MainBodyStates.Idle:
				mainBodyTimer.WaitTime = GD.RandRange(30, 60);
				MasterAnimationHandler(petMainBodyState.ToString());
				break;
			case MainBodyStates.Walk:
				mainBodyTimer.WaitTime = GD.RandRange(5, 10);
				MasterAnimationHandler(petMainBodyState.ToString());
				break;
			}
			mainBodyTimer.Start();
		}
	}

	private async Task ChangeStateBetweenIdleWalk()
	{
		if (petMainBodyState == MainBodyStates.Idle || petMainBodyState == MainBodyStates.Walk)
		{
			petMainBodyState = (MainBodyStates)GD.RandRange(0, 1);
			if (petMainBodyState == MainBodyStates.Walk)
			{
				EmitSignal(SignalName.Walking);
			}
			await ToSignal(GetTree(), SceneTree.SignalName.ProcessFrame);
		}
	}

	private void StartBlinkTimer()
	{
		Vector2 randomStateTimer = characterInformation.characterAnimationLayers[1].RandomStateTimer;
		faceBlinkTimer.WaitTime = GD.RandRange(randomStateTimer.X, randomStateTimer.Y);
		faceBlinkTimer.Start();
	}

	public void FaceBlinkTimerTimeOut()
	{
		allowBlink = true;
	}

	private void Blink()
	{
		if (allowBlink && FaceBody != null)
		{
			if (FaceBody.SpriteFrames.GetAnimationNames().Contains(petMainBodyState.ToString() + "_Face_Blink"))
			{
				FaceBody.Play(petMainBodyState.ToString() + "_Face_Blink");
			}
			else
			{
				StartBlinkTimer();
			}
		}
	}

	public void OnBlinkFinished()
	{
		if (FaceBody.SpriteFrames.GetAnimationNames().Contains(petMainBodyState.ToString() + "_Face_Blink"))
		{
			FaceBody.Play(petMainBodyState.ToString() + "_Face");
		}
		allowBlink = false;
		StartBlinkTimer();
	}

	public void BeginSit()
	{
		if (_sitTransitionHandler != null)
		{
			MainBody.AnimationFinished -= _sitTransitionHandler;
			_sitTransitionHandler = null;
		}
		if (petMainBodyState == MainBodyStates.Sit)
		{
			ForceMainBodyState(MainBodyStates.Idle, "Sit_Transition", reverse: true);
			_sitTransitionHandler = delegate
			{
				SitTransitionFinish(GetUp: true);
			};
		}
		else
		{
			ForceMainBodyState(MainBodyStates.Sit, "Sit_Transition");
			_sitTransitionHandler = delegate
			{
				SitTransitionFinish(GetUp: false);
			};
		}
		MainBody.AnimationFinished += _sitTransitionHandler;
	}

	public void SitTransitionFinish(bool GetUp)
	{
		MainBody.AnimationFinished -= _sitTransitionHandler;
		_sitTransitionHandler = null;
		if (!GetUp)
		{
			ForceMainBodyState(MainBodyStates.Sit, "Sit");
		}
		else
		{
			ForceMainBodyState(MainBodyStates.Idle, "Idle");
		}
	}

	private (string baseName, int amount) ParseTagString(string raw)
	{
		raw = raw.Trim();
		int num = raw.LastIndexOf(' ');
		if (num >= 0 && int.TryParse(raw.Substring(num + 1), out var result))
		{
			return (baseName: raw.Substring(0, num).TrimEnd(), amount: result);
		}
		return (baseName: raw, amount: 0);
	}

	public void AddTag(TagDataRes tag)
	{
		if (tag.savedTag && tag.tagDuration != -1999f)
		{
			if (tag.tagDuration != 0f)
			{
				GD.PrintErr("[Tag] '" + tag.tagName + "' is a savedTag but was given a duration. Duration will be ignored.");
			}
			tag.tagDuration = -1999f;
			tag.tagOriginalDuration = -1999f;
		}
		TagDataRes tag2 = GetTag(tag.tagName);
		if (tag2 != null)
		{
			if (tag.tagAmount != 0)
			{
				tag2.tagAmount += tag.tagAmount;
			}
			if (!tag2.savedTag)
			{
				tag2.tagDuration = tag.tagDuration;
				tag2.tagOriginalDuration = tag.tagDuration;
			}
		}
		else
		{
			existingTags.Add((TagDataRes)tag.Duplicate());
		}
		if (tag.savedTag)
		{
			Main.Instance.saveHandler.SaveSettings();
		}
		CheckAttachedBodies();
		CheckStateOverridesMain();
	}

	private void CheckStateOverridesMain()
	{
		if (_checkingStateOverrides || characterInformation == null || characterInformation.possibleStateOverride == null)
		{
			return;
		}
		_checkingStateOverrides = true;
		foreach (TagOverrideDataRes item2 in characterInformation.possibleStateOverride)
		{
			if (item2.requiredTags == null || item2.possibleOverrides == null || item2.possibleOverrides.Count == 0)
			{
				continue;
			}
			TagDataRes tag = GetTag(item2.requiredTags.tagName);
			if (tag == null || (item2.requiredTags.tagAmount != 0 && tag.tagAmount < item2.requiredTags.tagAmount))
			{
				continue;
			}
			existingTags.Remove(tag);
			CheckAttachedBodies();
			_checkingStateOverrides = false;
			Main.Instance.ClearAllAttachments();
			Main.Instance.ForceDropCharacter();
			WeightGroup<AttachDataRes> weightGroup = new WeightGroup<AttachDataRes>();
			foreach (AttachDataRes possibleOverride in item2.possibleOverrides)
			{
				if (!Main.Instance.IsBlacklisted(possibleOverride.taggedKinks))
				{
					weightGroup.Add(possibleOverride, possibleOverride.chanceOfItem);
				}
			}
			if (weightGroup.Count() == 0)
			{
				_checkingStateOverrides = false;
				CheckStateOverridesSub();
			}
			else
			{
				AttachDataRes item = weightGroup.GetItem(GD.RandRange(0, 10000));
				Main.Instance.CallCharacterAttachmentSpawn(item);
			}
			return;
		}
		_checkingStateOverrides = false;
		CheckStateOverridesSub();
	}

	private void CheckStateOverridesSub()
	{
		if (_checkingStateOverrides)
		{
			return;
		}
		_checkingStateOverrides = true;
		foreach (ActorWindow spawnedCompanion in Main.Instance.spawnedCompanions)
		{
			if (!GodotObject.IsInstanceValid(spawnedCompanion))
			{
				continue;
			}
			CharacterInfoDataRes characterInfoDataRes = spawnedCompanion.characterActor.characterInformation;
			if (characterInfoDataRes == null || characterInfoDataRes.possibleStateOverride == null)
			{
				continue;
			}
			foreach (TagOverrideDataRes item2 in characterInfoDataRes.possibleStateOverride)
			{
				if (item2.requiredTags == null || item2.possibleOverrides == null || item2.possibleOverrides.Count == 0)
				{
					continue;
				}
				TagDataRes tag = GetTag(item2.requiredTags.tagName);
				if (tag == null || (item2.requiredTags.tagAmount != 0 && tag.tagAmount < item2.requiredTags.tagAmount))
				{
					continue;
				}
				existingTags.Remove(tag);
				CheckAttachedBodies();
				_checkingStateOverrides = false;
				Main.Instance.ClearAllAttachments();
				Main.Instance.ForceDropCharacter();
				WeightGroup<AttachDataRes> weightGroup = new WeightGroup<AttachDataRes>();
				foreach (AttachDataRes possibleOverride in item2.possibleOverrides)
				{
					if (!Main.Instance.IsBlacklisted(possibleOverride.taggedKinks))
					{
						weightGroup.Add(possibleOverride, possibleOverride.chanceOfItem);
					}
				}
				if (weightGroup.Count() != 0)
				{
					AttachDataRes item = weightGroup.GetItem(GD.RandRange(0, 10000));
					Main.Instance.CallCharacterAttachmentSpawn(item, unclearableAttachment: false, spawnedCompanion);
					return;
				}
			}
		}
		_checkingStateOverrides = false;
	}

	public void RemoveTag(TagDataRes tag, bool tickDown = false)
	{
		TagDataRes tag2 = GetTag(tag.tagName);
		if (tag2 == null)
		{
			return;
		}
		if (tickDown && tag2.tagAmount > 0)
		{
			if (!tag2.savedTag)
			{
				tag2.tagAmount--;
				tag2.tagDuration = tag2.tagOriginalDuration;
				if (tag2.tagAmount <= 0)
				{
					existingTags.Remove(tag2);
					CheckAttachedBodies();
				}
			}
		}
		else if (tag2.tagAmount > 0 && tag.tagAmount > 0)
		{
			tag2.tagAmount -= tag.tagAmount;
			if (!tag2.savedTag)
			{
				tag2.tagDuration = tag2.tagOriginalDuration;
			}
			if (tag2.tagAmount <= 0 && !tag2.savedTag)
			{
				existingTags.Remove(tag2);
				CheckAttachedBodies();
			}
		}
		else if (!tag2.savedTag)
		{
			existingTags.Remove(tag2);
			CheckAttachedBodies();
		}
	}

	public TagDataRes GetTag(string tagName)
	{
		string item = ParseTagString(tagName).baseName;
		foreach (TagDataRes existingTag in existingTags)
		{
			if (existingTag.tagName == item)
			{
				return existingTag;
			}
		}
		return null;
	}

	private void CheckAttachedBodies()
	{
		foreach (KeyValuePair<AnimatedSprite2D, CharAnimDataRes> attachedBody in attachedBodies)
		{
			bool flag = false;
			bool flag2 = false;
			foreach (TagDataRes tagData in existingTags)
			{
				if (attachedBody.Value.blacklistedTags.Any((TagDataRes t) => t.tagName == tagData.tagName && (t.tagAmount == 0 || tagData.tagAmount >= t.tagAmount)))
				{
					flag2 = true;
					break;
				}
				if (attachedBody.Value.whitelistedTags.Any((TagDataRes t) => t.tagName == tagData.tagName && (t.tagAmount == 0 || tagData.tagAmount >= t.tagAmount)))
				{
					flag = true;
				}
			}
			if (flag2)
			{
				attachedBody.Key.Visible = false;
			}
			else if (flag)
			{
				attachedBody.Key.Visible = true;
			}
			else
			{
				attachedBody.Key.Visible = attachedBody.Value.defaultVisibility;
			}
		}
	}

	private void UpdateTags(double delta)
	{
		if (existingTags.Count == 0)
		{
			return;
		}
		Array<TagDataRes> array = new Array<TagDataRes>();
		foreach (TagDataRes existingTag in existingTags)
		{
			if (existingTag.tagDuration != -1999f)
			{
				existingTag.tagDuration -= (float)delta;
				if (existingTag.tagDuration <= 0f)
				{
					array.Add(existingTag);
				}
			}
		}
		foreach (TagDataRes item in array)
		{
			RemoveTag(item, tickDown: true);
		}
	}

}
