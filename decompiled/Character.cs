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
public class Character : Node2D
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

	public new class MethodName : Node2D.MethodName
	{
		public static readonly StringName SetupCharacter = "SetupCharacter";

		public new static readonly StringName _Process = "_Process";

		public static readonly StringName FlipHSpriteTo = "FlipHSpriteTo";

		public static readonly StringName SelectClothing = "SelectClothing";

		public static readonly StringName Grabbed = "Grabbed";

		public static readonly StringName StartDangle = "StartDangle";

		public static readonly StringName UpdateDangle = "UpdateDangle";

		public static readonly StringName StopDangle = "StopDangle";

		public static readonly StringName BeginThrow = "BeginThrow";

		public static readonly StringName ApplyBounceRotation = "ApplyBounceRotation";

		public static readonly StringName UpdateThrowRotation = "UpdateThrowRotation";

		public static readonly StringName CancelThrowRotation = "CancelThrowRotation";

		public static readonly StringName BeginLand = "BeginLand";

		public static readonly StringName TryQueueThrowReaction = "TryQueueThrowReaction";

		public static readonly StringName TryQueueLandReaction = "TryQueueLandReaction";

		public static readonly StringName TryQueueThrowLandReaction = "TryQueueThrowLandReaction";

		public static readonly StringName MasterAnimationHandler = "MasterAnimationHandler";

		public static readonly StringName ForceMainBodyState = "ForceMainBodyState";

		public static readonly StringName ForceMainBodyStateTransition = "ForceMainBodyStateTransition";

		public static readonly StringName MainBodyTimerTimeOut = "MainBodyTimerTimeOut";

		public static readonly StringName StartBlinkTimer = "StartBlinkTimer";

		public static readonly StringName FaceBlinkTimerTimeOut = "FaceBlinkTimerTimeOut";

		public static readonly StringName Blink = "Blink";

		public static readonly StringName OnBlinkFinished = "OnBlinkFinished";

		public static readonly StringName BeginSit = "BeginSit";

		public static readonly StringName SitTransitionFinish = "SitTransitionFinish";

		public static readonly StringName AddTag = "AddTag";

		public static readonly StringName CheckStateOverridesMain = "CheckStateOverridesMain";

		public static readonly StringName CheckStateOverridesSub = "CheckStateOverridesSub";

		public static readonly StringName RemoveTag = "RemoveTag";

		public static readonly StringName GetTag = "GetTag";

		public static readonly StringName CheckAttachedBodies = "CheckAttachedBodies";

		public static readonly StringName UpdateTags = "UpdateTags";
	}

	public new class PropertyName : Node2D.PropertyName
	{
		public static readonly StringName characterInformation = "characterInformation";

		public static readonly StringName spriteParentController = "spriteParentController";

		public static readonly StringName TimerParent = "TimerParent";

		public static readonly StringName mainBodyTimer = "mainBodyTimer";

		public static readonly StringName faceBlinkTimer = "faceBlinkTimer";

		public static readonly StringName petMainBodyState = "petMainBodyState";

		public static readonly StringName SelectedClothingState = "SelectedClothingState";

		public static readonly StringName SelectedClothingIndex = "SelectedClothingIndex";

		public static readonly StringName trueSize = "trueSize";

		public static readonly StringName existingTags = "existingTags";

		public static readonly StringName MainBody = "MainBody";

		public static readonly StringName FaceBody = "FaceBody";

		public static readonly StringName attachedBodies = "attachedBodies";

		public static readonly StringName allowBlink = "allowBlink";

		public static readonly StringName heldAnimation = "heldAnimation";

		public static readonly StringName _checkingStateOverrides = "_checkingStateOverrides";

		public static readonly StringName dandleOffset = "dandleOffset";

		public static readonly StringName dandleVelocity = "dandleVelocity";

		public static readonly StringName isDangling = "isDangling";

		public static readonly StringName throwRotation = "throwRotation";

		public static readonly StringName throwRotationVelocity = "throwRotationVelocity";

		public static readonly StringName throwWasHard = "throwWasHard";

		public static readonly StringName spriteParentOriginalPos = "spriteParentOriginalPos";

		public static readonly StringName lastMousePos = "lastMousePos";

		public static readonly StringName mouseVelocity = "mouseVelocity";
	}

	public new class SignalName : Node2D.SignalName
	{
		public static readonly StringName Walking = "Walking";

		public static readonly StringName FinishedWalking = "FinishedWalking";
	}

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

	private WalkingEventHandler backing_Walking;

	private FinishedWalkingEventHandler backing_FinishedWalking;

	public event WalkingEventHandler Walking
	{
		add
		{
			backing_Walking = (WalkingEventHandler)Delegate.Combine(backing_Walking, value);
		}
		remove
		{
			backing_Walking = (WalkingEventHandler)Delegate.Remove(backing_Walking, value);
		}
	}

	public event FinishedWalkingEventHandler FinishedWalking
	{
		add
		{
			backing_FinishedWalking = (FinishedWalkingEventHandler)Delegate.Combine(backing_FinishedWalking, value);
		}
		remove
		{
			backing_FinishedWalking = (FinishedWalkingEventHandler)Delegate.Remove(backing_FinishedWalking, value);
		}
	}

	public void SetupCharacter()
	{
		SelectedClothingState = characterInformation.ClothingStates[0];
		petMainBodyState = MainBodyStates.Idle;
		mainBodyTimer.Start();
		spriteParentController.setupCharacterSprites(this);
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
		lastMousePos = DisplayServer.MouseGetPosition();
	}

	public void UpdateDangle(double delta)
	{
		if (isDangling)
		{
			Vector2 vector = DisplayServer.MouseGetPosition();
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

	[EditorBrowsable(EditorBrowsableState.Never)]
	internal static List<MethodInfo> GetGodotMethodList()
	{
		return new List<MethodInfo>(34)
		{
			new MethodInfo(MethodName.SetupCharacter, new PropertyInfo(Variant.Type.Nil, "", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false), MethodFlags.Normal, null, null),
			new MethodInfo(MethodName._Process, new PropertyInfo(Variant.Type.Nil, "", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false), MethodFlags.Normal, new List<PropertyInfo>
			{
				new PropertyInfo(Variant.Type.Float, "delta", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false)
			}, null),
			new MethodInfo(MethodName.FlipHSpriteTo, new PropertyInfo(Variant.Type.Nil, "", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false), MethodFlags.Normal, new List<PropertyInfo>
			{
				new PropertyInfo(Variant.Type.Bool, "FlipH", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false)
			}, null),
			new MethodInfo(MethodName.SelectClothing, new PropertyInfo(Variant.Type.Nil, "", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false), MethodFlags.Normal, new List<PropertyInfo>
			{
				new PropertyInfo(Variant.Type.Int, "i", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false)
			}, null),
			new MethodInfo(MethodName.Grabbed, new PropertyInfo(Variant.Type.Nil, "", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false), MethodFlags.Normal, new List<PropertyInfo>
			{
				new PropertyInfo(Variant.Type.Bool, "isGrabbed", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false)
			}, null),
			new MethodInfo(MethodName.StartDangle, new PropertyInfo(Variant.Type.Nil, "", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false), MethodFlags.Normal, null, null),
			new MethodInfo(MethodName.UpdateDangle, new PropertyInfo(Variant.Type.Nil, "", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false), MethodFlags.Normal, new List<PropertyInfo>
			{
				new PropertyInfo(Variant.Type.Float, "delta", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false)
			}, null),
			new MethodInfo(MethodName.StopDangle, new PropertyInfo(Variant.Type.Nil, "", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false), MethodFlags.Normal, null, null),
			new MethodInfo(MethodName.BeginThrow, new PropertyInfo(Variant.Type.Nil, "", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false), MethodFlags.Normal, new List<PropertyInfo>
			{
				new PropertyInfo(Variant.Type.Vector2, "throwVelocity", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false)
			}, null),
			new MethodInfo(MethodName.ApplyBounceRotation, new PropertyInfo(Variant.Type.Nil, "", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false), MethodFlags.Normal, new List<PropertyInfo>
			{
				new PropertyInfo(Variant.Type.Float, "newXVelocity", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false)
			}, null),
			new MethodInfo(MethodName.UpdateThrowRotation, new PropertyInfo(Variant.Type.Nil, "", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false), MethodFlags.Normal, new List<PropertyInfo>
			{
				new PropertyInfo(Variant.Type.Float, "delta", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false)
			}, null),
			new MethodInfo(MethodName.CancelThrowRotation, new PropertyInfo(Variant.Type.Nil, "", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false), MethodFlags.Normal, null, null),
			new MethodInfo(MethodName.BeginLand, new PropertyInfo(Variant.Type.Nil, "", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false), MethodFlags.Normal, null, null),
			new MethodInfo(MethodName.TryQueueThrowReaction, new PropertyInfo(Variant.Type.Nil, "", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false), MethodFlags.Normal, null, null),
			new MethodInfo(MethodName.TryQueueLandReaction, new PropertyInfo(Variant.Type.Nil, "", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false), MethodFlags.Normal, null, null),
			new MethodInfo(MethodName.TryQueueThrowLandReaction, new PropertyInfo(Variant.Type.Nil, "", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false), MethodFlags.Normal, new List<PropertyInfo>
			{
				new PropertyInfo(Variant.Type.Float, "chancePercent", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false)
			}, null),
			new MethodInfo(MethodName.MasterAnimationHandler, new PropertyInfo(Variant.Type.Nil, "", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false), MethodFlags.Normal, new List<PropertyInfo>
			{
				new PropertyInfo(Variant.Type.String, "newAnimation", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false),
				new PropertyInfo(Variant.Type.Bool, "reverse", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false)
			}, null),
			new MethodInfo(MethodName.ForceMainBodyState, new PropertyInfo(Variant.Type.Nil, "", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false), MethodFlags.Normal, new List<PropertyInfo>
			{
				new PropertyInfo(Variant.Type.Int, "newState", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false),
				new PropertyInfo(Variant.Type.String, "newAnimation", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false),
				new PropertyInfo(Variant.Type.Bool, "reverse", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false)
			}, null),
			new MethodInfo(MethodName.ForceMainBodyState, new PropertyInfo(Variant.Type.Nil, "", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false), MethodFlags.Normal, new List<PropertyInfo>
			{
				new PropertyInfo(Variant.Type.Int, "newState", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false),
				new PropertyInfo(Variant.Type.String, "newAnimation", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false),
				new PropertyInfo(Variant.Type.Float, "newTime", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false),
				new PropertyInfo(Variant.Type.Bool, "reverse", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false)
			}, null),
			new MethodInfo(MethodName.ForceMainBodyStateTransition, new PropertyInfo(Variant.Type.Nil, "", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false), MethodFlags.Normal, new List<PropertyInfo>
			{
				new PropertyInfo(Variant.Type.String, "newAnimation", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false),
				new PropertyInfo(Variant.Type.Float, "newTime", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false)
			}, null),
			new MethodInfo(MethodName.MainBodyTimerTimeOut, new PropertyInfo(Variant.Type.Nil, "", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false), MethodFlags.Normal, null, null),
			new MethodInfo(MethodName.StartBlinkTimer, new PropertyInfo(Variant.Type.Nil, "", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false), MethodFlags.Normal, null, null),
			new MethodInfo(MethodName.FaceBlinkTimerTimeOut, new PropertyInfo(Variant.Type.Nil, "", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false), MethodFlags.Normal, null, null),
			new MethodInfo(MethodName.Blink, new PropertyInfo(Variant.Type.Nil, "", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false), MethodFlags.Normal, null, null),
			new MethodInfo(MethodName.OnBlinkFinished, new PropertyInfo(Variant.Type.Nil, "", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false), MethodFlags.Normal, null, null),
			new MethodInfo(MethodName.BeginSit, new PropertyInfo(Variant.Type.Nil, "", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false), MethodFlags.Normal, null, null),
			new MethodInfo(MethodName.SitTransitionFinish, new PropertyInfo(Variant.Type.Nil, "", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false), MethodFlags.Normal, new List<PropertyInfo>
			{
				new PropertyInfo(Variant.Type.Bool, "GetUp", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false)
			}, null),
			new MethodInfo(MethodName.AddTag, new PropertyInfo(Variant.Type.Nil, "", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false), MethodFlags.Normal, new List<PropertyInfo>
			{
				new PropertyInfo(Variant.Type.Object, "tag", PropertyHint.None, "", PropertyUsageFlags.Default, new StringName("Resource"), exported: false)
			}, null),
			new MethodInfo(MethodName.CheckStateOverridesMain, new PropertyInfo(Variant.Type.Nil, "", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false), MethodFlags.Normal, null, null),
			new MethodInfo(MethodName.CheckStateOverridesSub, new PropertyInfo(Variant.Type.Nil, "", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false), MethodFlags.Normal, null, null),
			new MethodInfo(MethodName.RemoveTag, new PropertyInfo(Variant.Type.Nil, "", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false), MethodFlags.Normal, new List<PropertyInfo>
			{
				new PropertyInfo(Variant.Type.Object, "tag", PropertyHint.None, "", PropertyUsageFlags.Default, new StringName("Resource"), exported: false),
				new PropertyInfo(Variant.Type.Bool, "tickDown", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false)
			}, null),
			new MethodInfo(MethodName.GetTag, new PropertyInfo(Variant.Type.Object, "", PropertyHint.None, "", PropertyUsageFlags.Default, new StringName("Resource"), exported: false), MethodFlags.Normal, new List<PropertyInfo>
			{
				new PropertyInfo(Variant.Type.String, "tagName", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false)
			}, null),
			new MethodInfo(MethodName.CheckAttachedBodies, new PropertyInfo(Variant.Type.Nil, "", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false), MethodFlags.Normal, null, null),
			new MethodInfo(MethodName.UpdateTags, new PropertyInfo(Variant.Type.Nil, "", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false), MethodFlags.Normal, new List<PropertyInfo>
			{
				new PropertyInfo(Variant.Type.Float, "delta", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false)
			}, null)
		};
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override bool InvokeGodotClassMethod(in godot_string_name method, NativeVariantPtrArgs args, out godot_variant ret)
	{
		if (method == MethodName.SetupCharacter && args.Count == 0)
		{
			SetupCharacter();
			ret = default(godot_variant);
			return true;
		}
		if (method == MethodName._Process && args.Count == 1)
		{
			_Process(VariantUtils.ConvertTo<double>(in args[0]));
			ret = default(godot_variant);
			return true;
		}
		if (method == MethodName.FlipHSpriteTo && args.Count == 1)
		{
			FlipHSpriteTo(VariantUtils.ConvertTo<bool>(in args[0]));
			ret = default(godot_variant);
			return true;
		}
		if (method == MethodName.SelectClothing && args.Count == 1)
		{
			SelectClothing(VariantUtils.ConvertTo<int>(in args[0]));
			ret = default(godot_variant);
			return true;
		}
		if (method == MethodName.Grabbed && args.Count == 1)
		{
			Grabbed(VariantUtils.ConvertTo<bool>(in args[0]));
			ret = default(godot_variant);
			return true;
		}
		if (method == MethodName.StartDangle && args.Count == 0)
		{
			StartDangle();
			ret = default(godot_variant);
			return true;
		}
		if (method == MethodName.UpdateDangle && args.Count == 1)
		{
			UpdateDangle(VariantUtils.ConvertTo<double>(in args[0]));
			ret = default(godot_variant);
			return true;
		}
		if (method == MethodName.StopDangle && args.Count == 0)
		{
			StopDangle();
			ret = default(godot_variant);
			return true;
		}
		if (method == MethodName.BeginThrow && args.Count == 1)
		{
			BeginThrow(VariantUtils.ConvertTo<Vector2>(in args[0]));
			ret = default(godot_variant);
			return true;
		}
		if (method == MethodName.ApplyBounceRotation && args.Count == 1)
		{
			ApplyBounceRotation(VariantUtils.ConvertTo<float>(in args[0]));
			ret = default(godot_variant);
			return true;
		}
		if (method == MethodName.UpdateThrowRotation && args.Count == 1)
		{
			UpdateThrowRotation(VariantUtils.ConvertTo<double>(in args[0]));
			ret = default(godot_variant);
			return true;
		}
		if (method == MethodName.CancelThrowRotation && args.Count == 0)
		{
			CancelThrowRotation();
			ret = default(godot_variant);
			return true;
		}
		if (method == MethodName.BeginLand && args.Count == 0)
		{
			BeginLand();
			ret = default(godot_variant);
			return true;
		}
		if (method == MethodName.TryQueueThrowReaction && args.Count == 0)
		{
			TryQueueThrowReaction();
			ret = default(godot_variant);
			return true;
		}
		if (method == MethodName.TryQueueLandReaction && args.Count == 0)
		{
			TryQueueLandReaction();
			ret = default(godot_variant);
			return true;
		}
		if (method == MethodName.TryQueueThrowLandReaction && args.Count == 1)
		{
			TryQueueThrowLandReaction(VariantUtils.ConvertTo<float>(in args[0]));
			ret = default(godot_variant);
			return true;
		}
		if (method == MethodName.MasterAnimationHandler && args.Count == 2)
		{
			MasterAnimationHandler(VariantUtils.ConvertTo<string>(in args[0]), VariantUtils.ConvertTo<bool>(in args[1]));
			ret = default(godot_variant);
			return true;
		}
		if (method == MethodName.ForceMainBodyState && args.Count == 3)
		{
			ForceMainBodyState(VariantUtils.ConvertTo<MainBodyStates>(in args[0]), VariantUtils.ConvertTo<string>(in args[1]), VariantUtils.ConvertTo<bool>(in args[2]));
			ret = default(godot_variant);
			return true;
		}
		if (method == MethodName.ForceMainBodyState && args.Count == 4)
		{
			ForceMainBodyState(VariantUtils.ConvertTo<MainBodyStates>(in args[0]), VariantUtils.ConvertTo<string>(in args[1]), VariantUtils.ConvertTo<float>(in args[2]), VariantUtils.ConvertTo<bool>(in args[3]));
			ret = default(godot_variant);
			return true;
		}
		if (method == MethodName.ForceMainBodyStateTransition && args.Count == 2)
		{
			ForceMainBodyStateTransition(VariantUtils.ConvertTo<string>(in args[0]), VariantUtils.ConvertTo<float>(in args[1]));
			ret = default(godot_variant);
			return true;
		}
		if (method == MethodName.MainBodyTimerTimeOut && args.Count == 0)
		{
			MainBodyTimerTimeOut();
			ret = default(godot_variant);
			return true;
		}
		if (method == MethodName.StartBlinkTimer && args.Count == 0)
		{
			StartBlinkTimer();
			ret = default(godot_variant);
			return true;
		}
		if (method == MethodName.FaceBlinkTimerTimeOut && args.Count == 0)
		{
			FaceBlinkTimerTimeOut();
			ret = default(godot_variant);
			return true;
		}
		if (method == MethodName.Blink && args.Count == 0)
		{
			Blink();
			ret = default(godot_variant);
			return true;
		}
		if (method == MethodName.OnBlinkFinished && args.Count == 0)
		{
			OnBlinkFinished();
			ret = default(godot_variant);
			return true;
		}
		if (method == MethodName.BeginSit && args.Count == 0)
		{
			BeginSit();
			ret = default(godot_variant);
			return true;
		}
		if (method == MethodName.SitTransitionFinish && args.Count == 1)
		{
			SitTransitionFinish(VariantUtils.ConvertTo<bool>(in args[0]));
			ret = default(godot_variant);
			return true;
		}
		if (method == MethodName.AddTag && args.Count == 1)
		{
			AddTag(VariantUtils.ConvertTo<TagDataRes>(in args[0]));
			ret = default(godot_variant);
			return true;
		}
		if (method == MethodName.CheckStateOverridesMain && args.Count == 0)
		{
			CheckStateOverridesMain();
			ret = default(godot_variant);
			return true;
		}
		if (method == MethodName.CheckStateOverridesSub && args.Count == 0)
		{
			CheckStateOverridesSub();
			ret = default(godot_variant);
			return true;
		}
		if (method == MethodName.RemoveTag && args.Count == 2)
		{
			RemoveTag(VariantUtils.ConvertTo<TagDataRes>(in args[0]), VariantUtils.ConvertTo<bool>(in args[1]));
			ret = default(godot_variant);
			return true;
		}
		if (method == MethodName.GetTag && args.Count == 1)
		{
			TagDataRes from = GetTag(VariantUtils.ConvertTo<string>(in args[0]));
			ret = VariantUtils.CreateFrom(in from);
			return true;
		}
		if (method == MethodName.CheckAttachedBodies && args.Count == 0)
		{
			CheckAttachedBodies();
			ret = default(godot_variant);
			return true;
		}
		if (method == MethodName.UpdateTags && args.Count == 1)
		{
			UpdateTags(VariantUtils.ConvertTo<double>(in args[0]));
			ret = default(godot_variant);
			return true;
		}
		return base.InvokeGodotClassMethod(in method, args, out ret);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override bool HasGodotClassMethod(in godot_string_name method)
	{
		if (method == MethodName.SetupCharacter)
		{
			return true;
		}
		if (method == MethodName._Process)
		{
			return true;
		}
		if (method == MethodName.FlipHSpriteTo)
		{
			return true;
		}
		if (method == MethodName.SelectClothing)
		{
			return true;
		}
		if (method == MethodName.Grabbed)
		{
			return true;
		}
		if (method == MethodName.StartDangle)
		{
			return true;
		}
		if (method == MethodName.UpdateDangle)
		{
			return true;
		}
		if (method == MethodName.StopDangle)
		{
			return true;
		}
		if (method == MethodName.BeginThrow)
		{
			return true;
		}
		if (method == MethodName.ApplyBounceRotation)
		{
			return true;
		}
		if (method == MethodName.UpdateThrowRotation)
		{
			return true;
		}
		if (method == MethodName.CancelThrowRotation)
		{
			return true;
		}
		if (method == MethodName.BeginLand)
		{
			return true;
		}
		if (method == MethodName.TryQueueThrowReaction)
		{
			return true;
		}
		if (method == MethodName.TryQueueLandReaction)
		{
			return true;
		}
		if (method == MethodName.TryQueueThrowLandReaction)
		{
			return true;
		}
		if (method == MethodName.MasterAnimationHandler)
		{
			return true;
		}
		if (method == MethodName.ForceMainBodyState)
		{
			return true;
		}
		if (method == MethodName.ForceMainBodyStateTransition)
		{
			return true;
		}
		if (method == MethodName.MainBodyTimerTimeOut)
		{
			return true;
		}
		if (method == MethodName.StartBlinkTimer)
		{
			return true;
		}
		if (method == MethodName.FaceBlinkTimerTimeOut)
		{
			return true;
		}
		if (method == MethodName.Blink)
		{
			return true;
		}
		if (method == MethodName.OnBlinkFinished)
		{
			return true;
		}
		if (method == MethodName.BeginSit)
		{
			return true;
		}
		if (method == MethodName.SitTransitionFinish)
		{
			return true;
		}
		if (method == MethodName.AddTag)
		{
			return true;
		}
		if (method == MethodName.CheckStateOverridesMain)
		{
			return true;
		}
		if (method == MethodName.CheckStateOverridesSub)
		{
			return true;
		}
		if (method == MethodName.RemoveTag)
		{
			return true;
		}
		if (method == MethodName.GetTag)
		{
			return true;
		}
		if (method == MethodName.CheckAttachedBodies)
		{
			return true;
		}
		if (method == MethodName.UpdateTags)
		{
			return true;
		}
		return base.HasGodotClassMethod(in method);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override bool SetGodotClassPropertyValue(in godot_string_name name, in godot_variant value)
	{
		if (name == PropertyName.characterInformation)
		{
			characterInformation = VariantUtils.ConvertTo<CharacterInfoDataRes>(in value);
			return true;
		}
		if (name == PropertyName.spriteParentController)
		{
			spriteParentController = VariantUtils.ConvertTo<SpriteParentController>(in value);
			return true;
		}
		if (name == PropertyName.TimerParent)
		{
			TimerParent = VariantUtils.ConvertTo<Node>(in value);
			return true;
		}
		if (name == PropertyName.mainBodyTimer)
		{
			mainBodyTimer = VariantUtils.ConvertTo<Timer>(in value);
			return true;
		}
		if (name == PropertyName.faceBlinkTimer)
		{
			faceBlinkTimer = VariantUtils.ConvertTo<Timer>(in value);
			return true;
		}
		if (name == PropertyName.petMainBodyState)
		{
			petMainBodyState = VariantUtils.ConvertTo<MainBodyStates>(in value);
			return true;
		}
		if (name == PropertyName.SelectedClothingState)
		{
			SelectedClothingState = VariantUtils.ConvertTo<string>(in value);
			return true;
		}
		if (name == PropertyName.SelectedClothingIndex)
		{
			SelectedClothingIndex = VariantUtils.ConvertTo<int>(in value);
			return true;
		}
		if (name == PropertyName.trueSize)
		{
			trueSize = VariantUtils.ConvertTo<Vector2I>(in value);
			return true;
		}
		if (name == PropertyName.existingTags)
		{
			existingTags = VariantUtils.ConvertToArray<TagDataRes>(in value);
			return true;
		}
		if (name == PropertyName.MainBody)
		{
			MainBody = VariantUtils.ConvertTo<AnimatedSprite2D>(in value);
			return true;
		}
		if (name == PropertyName.FaceBody)
		{
			FaceBody = VariantUtils.ConvertTo<AnimatedSprite2D>(in value);
			return true;
		}
		if (name == PropertyName.attachedBodies)
		{
			attachedBodies = VariantUtils.ConvertToDictionary<AnimatedSprite2D, CharAnimDataRes>(in value);
			return true;
		}
		if (name == PropertyName.allowBlink)
		{
			allowBlink = VariantUtils.ConvertTo<bool>(in value);
			return true;
		}
		if (name == PropertyName.heldAnimation)
		{
			heldAnimation = VariantUtils.ConvertTo<string>(in value);
			return true;
		}
		if (name == PropertyName._checkingStateOverrides)
		{
			_checkingStateOverrides = VariantUtils.ConvertTo<bool>(in value);
			return true;
		}
		if (name == PropertyName.dandleOffset)
		{
			dandleOffset = VariantUtils.ConvertTo<Vector2>(in value);
			return true;
		}
		if (name == PropertyName.dandleVelocity)
		{
			dandleVelocity = VariantUtils.ConvertTo<Vector2>(in value);
			return true;
		}
		if (name == PropertyName.isDangling)
		{
			isDangling = VariantUtils.ConvertTo<bool>(in value);
			return true;
		}
		if (name == PropertyName.throwRotation)
		{
			throwRotation = VariantUtils.ConvertTo<float>(in value);
			return true;
		}
		if (name == PropertyName.throwRotationVelocity)
		{
			throwRotationVelocity = VariantUtils.ConvertTo<float>(in value);
			return true;
		}
		if (name == PropertyName.throwWasHard)
		{
			throwWasHard = VariantUtils.ConvertTo<bool>(in value);
			return true;
		}
		if (name == PropertyName.spriteParentOriginalPos)
		{
			spriteParentOriginalPos = VariantUtils.ConvertTo<Vector2>(in value);
			return true;
		}
		if (name == PropertyName.lastMousePos)
		{
			lastMousePos = VariantUtils.ConvertTo<Vector2>(in value);
			return true;
		}
		if (name == PropertyName.mouseVelocity)
		{
			mouseVelocity = VariantUtils.ConvertTo<Vector2>(in value);
			return true;
		}
		return base.SetGodotClassPropertyValue(in name, in value);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override bool GetGodotClassPropertyValue(in godot_string_name name, out godot_variant value)
	{
		if (name == PropertyName.characterInformation)
		{
			value = VariantUtils.CreateFrom(in characterInformation);
			return true;
		}
		if (name == PropertyName.spriteParentController)
		{
			value = VariantUtils.CreateFrom(in spriteParentController);
			return true;
		}
		if (name == PropertyName.TimerParent)
		{
			value = VariantUtils.CreateFrom(in TimerParent);
			return true;
		}
		if (name == PropertyName.mainBodyTimer)
		{
			value = VariantUtils.CreateFrom(in mainBodyTimer);
			return true;
		}
		if (name == PropertyName.faceBlinkTimer)
		{
			value = VariantUtils.CreateFrom(in faceBlinkTimer);
			return true;
		}
		if (name == PropertyName.petMainBodyState)
		{
			value = VariantUtils.CreateFrom(in petMainBodyState);
			return true;
		}
		if (name == PropertyName.SelectedClothingState)
		{
			value = VariantUtils.CreateFrom(in SelectedClothingState);
			return true;
		}
		if (name == PropertyName.SelectedClothingIndex)
		{
			value = VariantUtils.CreateFrom(in SelectedClothingIndex);
			return true;
		}
		if (name == PropertyName.trueSize)
		{
			value = VariantUtils.CreateFrom(in trueSize);
			return true;
		}
		if (name == PropertyName.existingTags)
		{
			value = VariantUtils.CreateFromArray(existingTags);
			return true;
		}
		if (name == PropertyName.MainBody)
		{
			value = VariantUtils.CreateFrom(in MainBody);
			return true;
		}
		if (name == PropertyName.FaceBody)
		{
			value = VariantUtils.CreateFrom(in FaceBody);
			return true;
		}
		if (name == PropertyName.attachedBodies)
		{
			value = VariantUtils.CreateFromDictionary(attachedBodies);
			return true;
		}
		if (name == PropertyName.allowBlink)
		{
			value = VariantUtils.CreateFrom(in allowBlink);
			return true;
		}
		if (name == PropertyName.heldAnimation)
		{
			value = VariantUtils.CreateFrom(in heldAnimation);
			return true;
		}
		if (name == PropertyName._checkingStateOverrides)
		{
			value = VariantUtils.CreateFrom(in _checkingStateOverrides);
			return true;
		}
		if (name == PropertyName.dandleOffset)
		{
			value = VariantUtils.CreateFrom(in dandleOffset);
			return true;
		}
		if (name == PropertyName.dandleVelocity)
		{
			value = VariantUtils.CreateFrom(in dandleVelocity);
			return true;
		}
		if (name == PropertyName.isDangling)
		{
			value = VariantUtils.CreateFrom(in isDangling);
			return true;
		}
		if (name == PropertyName.throwRotation)
		{
			value = VariantUtils.CreateFrom(in throwRotation);
			return true;
		}
		if (name == PropertyName.throwRotationVelocity)
		{
			value = VariantUtils.CreateFrom(in throwRotationVelocity);
			return true;
		}
		if (name == PropertyName.throwWasHard)
		{
			value = VariantUtils.CreateFrom(in throwWasHard);
			return true;
		}
		if (name == PropertyName.spriteParentOriginalPos)
		{
			value = VariantUtils.CreateFrom(in spriteParentOriginalPos);
			return true;
		}
		if (name == PropertyName.lastMousePos)
		{
			value = VariantUtils.CreateFrom(in lastMousePos);
			return true;
		}
		if (name == PropertyName.mouseVelocity)
		{
			value = VariantUtils.CreateFrom(in mouseVelocity);
			return true;
		}
		return base.GetGodotClassPropertyValue(in name, out value);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	internal static List<PropertyInfo> GetGodotPropertyList()
	{
		return new List<PropertyInfo>
		{
			new PropertyInfo(Variant.Type.Object, PropertyName.characterInformation, PropertyHint.ResourceType, "CharacterInfoDataRes", PropertyUsageFlags.Default | PropertyUsageFlags.ScriptVariable, exported: true),
			new PropertyInfo(Variant.Type.Object, PropertyName.spriteParentController, PropertyHint.NodeType, "Node2D", PropertyUsageFlags.Default | PropertyUsageFlags.ScriptVariable, exported: true),
			new PropertyInfo(Variant.Type.Object, PropertyName.TimerParent, PropertyHint.NodeType, "Node", PropertyUsageFlags.Default | PropertyUsageFlags.ScriptVariable, exported: true),
			new PropertyInfo(Variant.Type.Object, PropertyName.mainBodyTimer, PropertyHint.NodeType, "Timer", PropertyUsageFlags.Default | PropertyUsageFlags.ScriptVariable, exported: true),
			new PropertyInfo(Variant.Type.Object, PropertyName.faceBlinkTimer, PropertyHint.NodeType, "Timer", PropertyUsageFlags.Default | PropertyUsageFlags.ScriptVariable, exported: true),
			new PropertyInfo(Variant.Type.Int, PropertyName.petMainBodyState, PropertyHint.None, "", PropertyUsageFlags.ScriptVariable, exported: false),
			new PropertyInfo(Variant.Type.String, PropertyName.SelectedClothingState, PropertyHint.None, "", PropertyUsageFlags.ScriptVariable, exported: false),
			new PropertyInfo(Variant.Type.Int, PropertyName.SelectedClothingIndex, PropertyHint.None, "", PropertyUsageFlags.ScriptVariable, exported: false),
			new PropertyInfo(Variant.Type.Vector2I, PropertyName.trueSize, PropertyHint.None, "", PropertyUsageFlags.ScriptVariable, exported: false),
			new PropertyInfo(Variant.Type.Array, PropertyName.existingTags, PropertyHint.None, "", PropertyUsageFlags.ScriptVariable, exported: false),
			new PropertyInfo(Variant.Type.Object, PropertyName.MainBody, PropertyHint.None, "", PropertyUsageFlags.ScriptVariable, exported: false),
			new PropertyInfo(Variant.Type.Object, PropertyName.FaceBody, PropertyHint.None, "", PropertyUsageFlags.ScriptVariable, exported: false),
			new PropertyInfo(Variant.Type.Dictionary, PropertyName.attachedBodies, PropertyHint.None, "", PropertyUsageFlags.ScriptVariable, exported: false),
			new PropertyInfo(Variant.Type.Bool, PropertyName.allowBlink, PropertyHint.None, "", PropertyUsageFlags.ScriptVariable, exported: false),
			new PropertyInfo(Variant.Type.String, PropertyName.heldAnimation, PropertyHint.None, "", PropertyUsageFlags.ScriptVariable, exported: false),
			new PropertyInfo(Variant.Type.Bool, PropertyName._checkingStateOverrides, PropertyHint.None, "", PropertyUsageFlags.ScriptVariable, exported: false),
			new PropertyInfo(Variant.Type.Vector2, PropertyName.dandleOffset, PropertyHint.None, "", PropertyUsageFlags.ScriptVariable, exported: false),
			new PropertyInfo(Variant.Type.Vector2, PropertyName.dandleVelocity, PropertyHint.None, "", PropertyUsageFlags.ScriptVariable, exported: false),
			new PropertyInfo(Variant.Type.Bool, PropertyName.isDangling, PropertyHint.None, "", PropertyUsageFlags.ScriptVariable, exported: false),
			new PropertyInfo(Variant.Type.Float, PropertyName.throwRotation, PropertyHint.None, "", PropertyUsageFlags.ScriptVariable, exported: false),
			new PropertyInfo(Variant.Type.Float, PropertyName.throwRotationVelocity, PropertyHint.None, "", PropertyUsageFlags.ScriptVariable, exported: false),
			new PropertyInfo(Variant.Type.Bool, PropertyName.throwWasHard, PropertyHint.None, "", PropertyUsageFlags.ScriptVariable, exported: false),
			new PropertyInfo(Variant.Type.Vector2, PropertyName.spriteParentOriginalPos, PropertyHint.None, "", PropertyUsageFlags.ScriptVariable, exported: false),
			new PropertyInfo(Variant.Type.Vector2, PropertyName.lastMousePos, PropertyHint.None, "", PropertyUsageFlags.ScriptVariable, exported: false),
			new PropertyInfo(Variant.Type.Vector2, PropertyName.mouseVelocity, PropertyHint.None, "", PropertyUsageFlags.ScriptVariable, exported: false)
		};
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override void SaveGodotObjectData(GodotSerializationInfo info)
	{
		base.SaveGodotObjectData(info);
		info.AddProperty(PropertyName.characterInformation, Variant.From(in characterInformation));
		info.AddProperty(PropertyName.spriteParentController, Variant.From(in spriteParentController));
		info.AddProperty(PropertyName.TimerParent, Variant.From(in TimerParent));
		info.AddProperty(PropertyName.mainBodyTimer, Variant.From(in mainBodyTimer));
		info.AddProperty(PropertyName.faceBlinkTimer, Variant.From(in faceBlinkTimer));
		info.AddProperty(PropertyName.petMainBodyState, Variant.From(in petMainBodyState));
		info.AddProperty(PropertyName.SelectedClothingState, Variant.From(in SelectedClothingState));
		info.AddProperty(PropertyName.SelectedClothingIndex, Variant.From(in SelectedClothingIndex));
		info.AddProperty(PropertyName.trueSize, Variant.From(in trueSize));
		info.AddProperty(PropertyName.existingTags, Variant.CreateFrom(existingTags));
		info.AddProperty(PropertyName.MainBody, Variant.From(in MainBody));
		info.AddProperty(PropertyName.FaceBody, Variant.From(in FaceBody));
		info.AddProperty(PropertyName.attachedBodies, Variant.CreateFrom(attachedBodies));
		info.AddProperty(PropertyName.allowBlink, Variant.From(in allowBlink));
		info.AddProperty(PropertyName.heldAnimation, Variant.From(in heldAnimation));
		info.AddProperty(PropertyName._checkingStateOverrides, Variant.From(in _checkingStateOverrides));
		info.AddProperty(PropertyName.dandleOffset, Variant.From(in dandleOffset));
		info.AddProperty(PropertyName.dandleVelocity, Variant.From(in dandleVelocity));
		info.AddProperty(PropertyName.isDangling, Variant.From(in isDangling));
		info.AddProperty(PropertyName.throwRotation, Variant.From(in throwRotation));
		info.AddProperty(PropertyName.throwRotationVelocity, Variant.From(in throwRotationVelocity));
		info.AddProperty(PropertyName.throwWasHard, Variant.From(in throwWasHard));
		info.AddProperty(PropertyName.spriteParentOriginalPos, Variant.From(in spriteParentOriginalPos));
		info.AddProperty(PropertyName.lastMousePos, Variant.From(in lastMousePos));
		info.AddProperty(PropertyName.mouseVelocity, Variant.From(in mouseVelocity));
		info.AddSignalEventDelegate(SignalName.Walking, backing_Walking);
		info.AddSignalEventDelegate(SignalName.FinishedWalking, backing_FinishedWalking);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override void RestoreGodotObjectData(GodotSerializationInfo info)
	{
		base.RestoreGodotObjectData(info);
		if (info.TryGetProperty(PropertyName.characterInformation, out var value))
		{
			characterInformation = value.As<CharacterInfoDataRes>();
		}
		if (info.TryGetProperty(PropertyName.spriteParentController, out var value2))
		{
			spriteParentController = value2.As<SpriteParentController>();
		}
		if (info.TryGetProperty(PropertyName.TimerParent, out var value3))
		{
			TimerParent = value3.As<Node>();
		}
		if (info.TryGetProperty(PropertyName.mainBodyTimer, out var value4))
		{
			mainBodyTimer = value4.As<Timer>();
		}
		if (info.TryGetProperty(PropertyName.faceBlinkTimer, out var value5))
		{
			faceBlinkTimer = value5.As<Timer>();
		}
		if (info.TryGetProperty(PropertyName.petMainBodyState, out var value6))
		{
			petMainBodyState = value6.As<MainBodyStates>();
		}
		if (info.TryGetProperty(PropertyName.SelectedClothingState, out var value7))
		{
			SelectedClothingState = value7.As<string>();
		}
		if (info.TryGetProperty(PropertyName.SelectedClothingIndex, out var value8))
		{
			SelectedClothingIndex = value8.As<int>();
		}
		if (info.TryGetProperty(PropertyName.trueSize, out var value9))
		{
			trueSize = value9.As<Vector2I>();
		}
		if (info.TryGetProperty(PropertyName.existingTags, out var value10))
		{
			existingTags = value10.AsGodotArray<TagDataRes>();
		}
		if (info.TryGetProperty(PropertyName.MainBody, out var value11))
		{
			MainBody = value11.As<AnimatedSprite2D>();
		}
		if (info.TryGetProperty(PropertyName.FaceBody, out var value12))
		{
			FaceBody = value12.As<AnimatedSprite2D>();
		}
		if (info.TryGetProperty(PropertyName.attachedBodies, out var value13))
		{
			attachedBodies = value13.AsGodotDictionary<AnimatedSprite2D, CharAnimDataRes>();
		}
		if (info.TryGetProperty(PropertyName.allowBlink, out var value14))
		{
			allowBlink = value14.As<bool>();
		}
		if (info.TryGetProperty(PropertyName.heldAnimation, out var value15))
		{
			heldAnimation = value15.As<string>();
		}
		if (info.TryGetProperty(PropertyName._checkingStateOverrides, out var value16))
		{
			_checkingStateOverrides = value16.As<bool>();
		}
		if (info.TryGetProperty(PropertyName.dandleOffset, out var value17))
		{
			dandleOffset = value17.As<Vector2>();
		}
		if (info.TryGetProperty(PropertyName.dandleVelocity, out var value18))
		{
			dandleVelocity = value18.As<Vector2>();
		}
		if (info.TryGetProperty(PropertyName.isDangling, out var value19))
		{
			isDangling = value19.As<bool>();
		}
		if (info.TryGetProperty(PropertyName.throwRotation, out var value20))
		{
			throwRotation = value20.As<float>();
		}
		if (info.TryGetProperty(PropertyName.throwRotationVelocity, out var value21))
		{
			throwRotationVelocity = value21.As<float>();
		}
		if (info.TryGetProperty(PropertyName.throwWasHard, out var value22))
		{
			throwWasHard = value22.As<bool>();
		}
		if (info.TryGetProperty(PropertyName.spriteParentOriginalPos, out var value23))
		{
			spriteParentOriginalPos = value23.As<Vector2>();
		}
		if (info.TryGetProperty(PropertyName.lastMousePos, out var value24))
		{
			lastMousePos = value24.As<Vector2>();
		}
		if (info.TryGetProperty(PropertyName.mouseVelocity, out var value25))
		{
			mouseVelocity = value25.As<Vector2>();
		}
		if (info.TryGetSignalEventDelegate<WalkingEventHandler>(SignalName.Walking, out var value26))
		{
			backing_Walking = value26;
		}
		if (info.TryGetSignalEventDelegate<FinishedWalkingEventHandler>(SignalName.FinishedWalking, out var value27))
		{
			backing_FinishedWalking = value27;
		}
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	internal static List<MethodInfo> GetGodotSignalList()
	{
		return new List<MethodInfo>(2)
		{
			new MethodInfo(SignalName.Walking, new PropertyInfo(Variant.Type.Nil, "", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false), MethodFlags.Normal, null, null),
			new MethodInfo(SignalName.FinishedWalking, new PropertyInfo(Variant.Type.Nil, "", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false), MethodFlags.Normal, null, null)
		};
	}

	protected void EmitSignalWalking()
	{
		EmitSignal(SignalName.Walking);
	}

	protected void EmitSignalFinishedWalking()
	{
		EmitSignal(SignalName.FinishedWalking);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override void RaiseGodotClassSignalCallbacks(in godot_string_name signal, NativeVariantPtrArgs args)
	{
		if (signal == SignalName.Walking && args.Count == 0)
		{
			backing_Walking?.Invoke();
		}
		else if (signal == SignalName.FinishedWalking && args.Count == 0)
		{
			backing_FinishedWalking?.Invoke();
		}
		else
		{
			base.RaiseGodotClassSignalCallbacks(in signal, args);
		}
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override bool HasGodotClassSignal(in godot_string_name signal)
	{
		if (signal == SignalName.Walking)
		{
			return true;
		}
		if (signal == SignalName.FinishedWalking)
		{
			return true;
		}
		return base.HasGodotClassSignal(in signal);
	}
}
