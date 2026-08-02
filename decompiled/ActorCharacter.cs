using System;
using System.Collections.Generic;
using System.ComponentModel;
using Godot;
using Godot.Bridge;
using Godot.NativeInterop;

[ScriptPath("res://Scripts/CharacterScripts/ActorCharacter.cs")]
public class ActorCharacter : Node2D
{
	public enum MainBodyStates
	{
		Idle,
		Walk,
		Forced_Animation,
		Transition
	}

	public new class MethodName : Node2D.MethodName
	{
		public static readonly StringName SetupActor = "SetupActor";

		public static readonly StringName FlipHSpriteTo = "FlipHSpriteTo";

		public static readonly StringName ForceMainBodyState = "ForceMainBodyState";

		public static readonly StringName ForceMainBodyStateTransition = "ForceMainBodyStateTransition";

		public static readonly StringName MainBodyTimerTimeOut = "MainBodyTimerTimeOut";
	}

	public new class PropertyName : Node2D.PropertyName
	{
		public static readonly StringName characterInformation = "characterInformation";

		public static readonly StringName spriteParentController = "spriteParentController";

		public static readonly StringName TimerParent = "TimerParent";

		public static readonly StringName mainBodyTimer = "mainBodyTimer";

		public static readonly StringName glitchEffect = "glitchEffect";

		public static readonly StringName petMainBodyState = "petMainBodyState";

		public static readonly StringName allowBlink = "allowBlink";

		public static readonly StringName heldAnimation = "heldAnimation";

		public static readonly StringName MainBody = "MainBody";

		public static readonly StringName trueSize = "trueSize";
	}

	public new class SignalName : Node2D.SignalName
	{
	}

	[Export(PropertyHint.None, "")]
	public CharacterInfoDataRes characterInformation;

	[Export(PropertyHint.None, "")]
	public ActorSpriteController spriteParentController;

	[Export(PropertyHint.None, "")]
	public Node TimerParent;

	[Export(PropertyHint.None, "")]
	public Timer mainBodyTimer;

	[Export(PropertyHint.None, "")]
	public Control glitchEffect;

	public MainBodyStates petMainBodyState;

	private Action _animationTransitionHandler;

	private Action _animationEndingHandler;

	private bool allowBlink;

	private string heldAnimation;

	public AnimatedSprite2D MainBody;

	public Vector2I trueSize;

	public void SetupActor()
	{
		spriteParentController.setupCharacterSprites(this);
		if (characterInformation.AITyping == CharacterInfoDataRes.AITypes.COMPANION)
		{
			mainBodyTimer.Start();
		}
		if (!characterInformation.Glitchy)
		{
			glitchEffect.QueueFree();
		}
	}

	public void FlipHSpriteTo(bool FlipH)
	{
		foreach (AnimatedSprite2D child in spriteParentController.GetChildren())
		{
			child.FlipH = !FlipH;
		}
	}

	public void ForceMainBodyState(MainBodyStates newState, string newAnimation, float newTime = 0f)
	{
		petMainBodyState = newState;
		MainBody.Play(newAnimation);
		if (newTime == 0f)
		{
			StringName animation = MainBody.Animation;
			SpriteFrames spriteFrames = MainBody.SpriteFrames;
			int frameCount = spriteFrames.GetFrameCount(animation);
			double animationSpeed = spriteFrames.GetAnimationSpeed(animation);
			double num = (double)frameCount / animationSpeed;
			GD.Print(num);
			mainBodyTimer.WaitTime = num;
			mainBodyTimer.Start();
		}
		else
		{
			mainBodyTimer.WaitTime = newTime;
			mainBodyTimer.Start();
		}
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
		MainBody.Play(newAnimation + "_Transition");
		_animationTransitionHandler = delegate
		{
			MainBody.AnimationLooped -= _animationTransitionHandler;
			MainBody.AnimationFinished -= _animationTransitionHandler;
			_animationTransitionHandler = null;
			MainBody.Play(newAnimation);
			mainBodyTimer.WaitTime = newTime;
			mainBodyTimer.Start();
		};
		MainBody.AnimationLooped += _animationTransitionHandler;
		MainBody.AnimationFinished += _animationTransitionHandler;
	}

	public void MainBodyTimerTimeOut()
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
			MainBody.PlayBackwards(heldAnimation + "_Transition");
			_animationEndingHandler = delegate
			{
				MainBody.AnimationLooped -= _animationEndingHandler;
				MainBody.AnimationFinished -= _animationEndingHandler;
				_animationEndingHandler = null;
				heldAnimation = "";
				petMainBodyState = MainBodyStates.Idle;
				MainBody.Play(petMainBodyState.ToString());
				mainBodyTimer.WaitTime = 5.0;
				mainBodyTimer.Start();
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
				MainBody.AnimationLooped -= _animationEndingHandler;
				MainBody.AnimationFinished -= _animationEndingHandler;
				_animationEndingHandler = null;
				petMainBodyState = MainBodyStates.Idle;
				MainBody.Play(petMainBodyState.ToString());
				mainBodyTimer.WaitTime = 5.0;
				mainBodyTimer.Start();
			};
			MainBody.AnimationLooped += _animationEndingHandler;
			MainBody.AnimationFinished += _animationEndingHandler;
		}
		else
		{
			mainBodyTimer.Start();
		}
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	internal static List<MethodInfo> GetGodotMethodList()
	{
		return new List<MethodInfo>(5)
		{
			new MethodInfo(MethodName.SetupActor, new PropertyInfo(Variant.Type.Nil, "", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false), MethodFlags.Normal, null, null),
			new MethodInfo(MethodName.FlipHSpriteTo, new PropertyInfo(Variant.Type.Nil, "", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false), MethodFlags.Normal, new List<PropertyInfo>
			{
				new PropertyInfo(Variant.Type.Bool, "FlipH", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false)
			}, null),
			new MethodInfo(MethodName.ForceMainBodyState, new PropertyInfo(Variant.Type.Nil, "", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false), MethodFlags.Normal, new List<PropertyInfo>
			{
				new PropertyInfo(Variant.Type.Int, "newState", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false),
				new PropertyInfo(Variant.Type.String, "newAnimation", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false),
				new PropertyInfo(Variant.Type.Float, "newTime", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false)
			}, null),
			new MethodInfo(MethodName.ForceMainBodyStateTransition, new PropertyInfo(Variant.Type.Nil, "", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false), MethodFlags.Normal, new List<PropertyInfo>
			{
				new PropertyInfo(Variant.Type.String, "newAnimation", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false),
				new PropertyInfo(Variant.Type.Float, "newTime", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false)
			}, null),
			new MethodInfo(MethodName.MainBodyTimerTimeOut, new PropertyInfo(Variant.Type.Nil, "", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false), MethodFlags.Normal, null, null)
		};
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override bool InvokeGodotClassMethod(in godot_string_name method, NativeVariantPtrArgs args, out godot_variant ret)
	{
		if (method == MethodName.SetupActor && args.Count == 0)
		{
			SetupActor();
			ret = default(godot_variant);
			return true;
		}
		if (method == MethodName.FlipHSpriteTo && args.Count == 1)
		{
			FlipHSpriteTo(VariantUtils.ConvertTo<bool>(in args[0]));
			ret = default(godot_variant);
			return true;
		}
		if (method == MethodName.ForceMainBodyState && args.Count == 3)
		{
			ForceMainBodyState(VariantUtils.ConvertTo<MainBodyStates>(in args[0]), VariantUtils.ConvertTo<string>(in args[1]), VariantUtils.ConvertTo<float>(in args[2]));
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
		return base.InvokeGodotClassMethod(in method, args, out ret);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override bool HasGodotClassMethod(in godot_string_name method)
	{
		if (method == MethodName.SetupActor)
		{
			return true;
		}
		if (method == MethodName.FlipHSpriteTo)
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
			spriteParentController = VariantUtils.ConvertTo<ActorSpriteController>(in value);
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
		if (name == PropertyName.glitchEffect)
		{
			glitchEffect = VariantUtils.ConvertTo<Control>(in value);
			return true;
		}
		if (name == PropertyName.petMainBodyState)
		{
			petMainBodyState = VariantUtils.ConvertTo<MainBodyStates>(in value);
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
		if (name == PropertyName.MainBody)
		{
			MainBody = VariantUtils.ConvertTo<AnimatedSprite2D>(in value);
			return true;
		}
		if (name == PropertyName.trueSize)
		{
			trueSize = VariantUtils.ConvertTo<Vector2I>(in value);
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
		if (name == PropertyName.glitchEffect)
		{
			value = VariantUtils.CreateFrom(in glitchEffect);
			return true;
		}
		if (name == PropertyName.petMainBodyState)
		{
			value = VariantUtils.CreateFrom(in petMainBodyState);
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
		if (name == PropertyName.MainBody)
		{
			value = VariantUtils.CreateFrom(in MainBody);
			return true;
		}
		if (name == PropertyName.trueSize)
		{
			value = VariantUtils.CreateFrom(in trueSize);
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
			new PropertyInfo(Variant.Type.Object, PropertyName.glitchEffect, PropertyHint.NodeType, "Control", PropertyUsageFlags.Default | PropertyUsageFlags.ScriptVariable, exported: true),
			new PropertyInfo(Variant.Type.Int, PropertyName.petMainBodyState, PropertyHint.None, "", PropertyUsageFlags.ScriptVariable, exported: false),
			new PropertyInfo(Variant.Type.Bool, PropertyName.allowBlink, PropertyHint.None, "", PropertyUsageFlags.ScriptVariable, exported: false),
			new PropertyInfo(Variant.Type.String, PropertyName.heldAnimation, PropertyHint.None, "", PropertyUsageFlags.ScriptVariable, exported: false),
			new PropertyInfo(Variant.Type.Object, PropertyName.MainBody, PropertyHint.None, "", PropertyUsageFlags.ScriptVariable, exported: false),
			new PropertyInfo(Variant.Type.Vector2I, PropertyName.trueSize, PropertyHint.None, "", PropertyUsageFlags.ScriptVariable, exported: false)
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
		info.AddProperty(PropertyName.glitchEffect, Variant.From(in glitchEffect));
		info.AddProperty(PropertyName.petMainBodyState, Variant.From(in petMainBodyState));
		info.AddProperty(PropertyName.allowBlink, Variant.From(in allowBlink));
		info.AddProperty(PropertyName.heldAnimation, Variant.From(in heldAnimation));
		info.AddProperty(PropertyName.MainBody, Variant.From(in MainBody));
		info.AddProperty(PropertyName.trueSize, Variant.From(in trueSize));
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
			spriteParentController = value2.As<ActorSpriteController>();
		}
		if (info.TryGetProperty(PropertyName.TimerParent, out var value3))
		{
			TimerParent = value3.As<Node>();
		}
		if (info.TryGetProperty(PropertyName.mainBodyTimer, out var value4))
		{
			mainBodyTimer = value4.As<Timer>();
		}
		if (info.TryGetProperty(PropertyName.glitchEffect, out var value5))
		{
			glitchEffect = value5.As<Control>();
		}
		if (info.TryGetProperty(PropertyName.petMainBodyState, out var value6))
		{
			petMainBodyState = value6.As<MainBodyStates>();
		}
		if (info.TryGetProperty(PropertyName.allowBlink, out var value7))
		{
			allowBlink = value7.As<bool>();
		}
		if (info.TryGetProperty(PropertyName.heldAnimation, out var value8))
		{
			heldAnimation = value8.As<string>();
		}
		if (info.TryGetProperty(PropertyName.MainBody, out var value9))
		{
			MainBody = value9.As<AnimatedSprite2D>();
		}
		if (info.TryGetProperty(PropertyName.trueSize, out var value10))
		{
			trueSize = value10.As<Vector2I>();
		}
	}
}
