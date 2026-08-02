using System.Collections.Generic;
using System.ComponentModel;
using Godot;
using Godot.Bridge;
using Godot.NativeInterop;

[ScriptPath("res://Scripts/AttachmentScripts/AttachSpriteController.cs")]
public class AttachSpriteController : Node2D
{
	public new class MethodName : Node2D.MethodName
	{
		public static readonly StringName setupItemSprites = "setupItemSprites";

		public static readonly StringName CheckAndSetupRandomKillTimer = "CheckAndSetupRandomKillTimer";

		public static readonly StringName FlipImage = "FlipImage";
	}

	public new class PropertyName : Node2D.PropertyName
	{
	}

	public new class SignalName : Node2D.SignalName
	{
	}

	public void setupItemSprites(AttachmentObject parent)
	{
		foreach (SpriteFrames attachmentAnimation in parent.attachedItemInformation.attachmentAnimations)
		{
			AnimatedSprite2D animatedSprite2D = new AnimatedSprite2D();
			animatedSprite2D.SpriteFrames = attachmentAnimation;
			animatedSprite2D.TextureFilter = TextureFilterEnum.Nearest;
			animatedSprite2D.Play(attachmentAnimation.GetAnimationNames()[0]);
			AddChild(animatedSprite2D, forceReadableName: false, InternalMode.Disabled);
		}
		if (!(GetChild(0) is AnimatedSprite2D animatedSprite2D2))
		{
			return;
		}
		base.Scale = parent.attachedItemInformation.attachmentScale * Main.Instance.settingSpriteScaler;
		base.Position = animatedSprite2D2.SpriteFrames.GetFrameTexture(animatedSprite2D2.Animation, 0).GetSize() / 2f * base.Scale;
		switch (parent.attachedItemInformation.attachmentTyping)
		{
		case AttachDataRes.AttachmentType.RANDOM_CLICKED_WINDOW:
			CheckAndSetupRandomKillTimer(parent);
			parent.GetParent<AttachObjWindow>().mainBody = animatedSprite2D2;
			return;
		case AttachDataRes.AttachmentType.OVERRIDE:
			CheckAndSetupRandomKillTimer(parent);
			animatedSprite2D2.AnimationFinished += parent.GetParent<AttachObjWindow>().OnAnimationTimeout;
			animatedSprite2D2.AnimationLooped += parent.GetParent<AttachObjWindow>().OnAnimationTimeout;
			animatedSprite2D2.FrameChanged += parent.GetParent<AttachObjWindow>().PlayDialogueOnFrame;
			break;
		default:
			if (!CheckAndSetupRandomKillTimer(parent))
			{
				animatedSprite2D2.AnimationFinished += parent.GetParent<AttachObjWindow>().OnAnimationTimeout;
				animatedSprite2D2.AnimationLooped += parent.GetParent<AttachObjWindow>().OnAnimationTimeout;
			}
			break;
		}
		parent.GetParent<AttachObjWindow>().mainBody = animatedSprite2D2;
	}

	private bool CheckAndSetupRandomKillTimer(AttachmentObject parent)
	{
		if (!parent.attachedItemInformation.isKillOnRandomTimer)
		{
			return false;
		}
		Timer timer = new Timer();
		timer.WaitTime = GD.RandRange(parent.attachedItemInformation.randomKillOnTime.X, parent.attachedItemInformation.randomKillOnTime.Y);
		timer.Timeout += parent.GetParent<AttachObjWindow>().OnAnimationTimeout;
		parent.AddChild(timer, forceReadableName: false, InternalMode.Disabled);
		return true;
	}

	public void FlipImage(bool isLeft)
	{
		foreach (AnimatedSprite2D child in GetChildren())
		{
			child.FlipH = isLeft;
		}
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	internal static List<MethodInfo> GetGodotMethodList()
	{
		return new List<MethodInfo>(3)
		{
			new MethodInfo(MethodName.setupItemSprites, new PropertyInfo(Variant.Type.Nil, "", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false), MethodFlags.Normal, new List<PropertyInfo>
			{
				new PropertyInfo(Variant.Type.Object, "parent", PropertyHint.None, "", PropertyUsageFlags.Default, new StringName("Node2D"), exported: false)
			}, null),
			new MethodInfo(MethodName.CheckAndSetupRandomKillTimer, new PropertyInfo(Variant.Type.Bool, "", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false), MethodFlags.Normal, new List<PropertyInfo>
			{
				new PropertyInfo(Variant.Type.Object, "parent", PropertyHint.None, "", PropertyUsageFlags.Default, new StringName("Node2D"), exported: false)
			}, null),
			new MethodInfo(MethodName.FlipImage, new PropertyInfo(Variant.Type.Nil, "", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false), MethodFlags.Normal, new List<PropertyInfo>
			{
				new PropertyInfo(Variant.Type.Bool, "isLeft", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false)
			}, null)
		};
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override bool InvokeGodotClassMethod(in godot_string_name method, NativeVariantPtrArgs args, out godot_variant ret)
	{
		if (method == MethodName.setupItemSprites && args.Count == 1)
		{
			setupItemSprites(VariantUtils.ConvertTo<AttachmentObject>(in args[0]));
			ret = default(godot_variant);
			return true;
		}
		if (method == MethodName.CheckAndSetupRandomKillTimer && args.Count == 1)
		{
			bool from = CheckAndSetupRandomKillTimer(VariantUtils.ConvertTo<AttachmentObject>(in args[0]));
			ret = VariantUtils.CreateFrom(in from);
			return true;
		}
		if (method == MethodName.FlipImage && args.Count == 1)
		{
			FlipImage(VariantUtils.ConvertTo<bool>(in args[0]));
			ret = default(godot_variant);
			return true;
		}
		return base.InvokeGodotClassMethod(in method, args, out ret);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override bool HasGodotClassMethod(in godot_string_name method)
	{
		if (method == MethodName.setupItemSprites)
		{
			return true;
		}
		if (method == MethodName.CheckAndSetupRandomKillTimer)
		{
			return true;
		}
		if (method == MethodName.FlipImage)
		{
			return true;
		}
		return base.HasGodotClassMethod(in method);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override void SaveGodotObjectData(GodotSerializationInfo info)
	{
		base.SaveGodotObjectData(info);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override void RestoreGodotObjectData(GodotSerializationInfo info)
	{
		base.RestoreGodotObjectData(info);
	}
}
