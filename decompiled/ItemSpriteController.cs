using System.Collections.Generic;
using System.ComponentModel;
using Godot;
using Godot.Bridge;
using Godot.NativeInterop;

[ScriptPath("res://Scripts/ItemScripts/ItemSpriteController.cs")]
public class ItemSpriteController : Node2D
{
	public new class MethodName : Node2D.MethodName
	{
		public static readonly StringName setupItemSprites = "setupItemSprites";
	}

	public new class PropertyName : Node2D.PropertyName
	{
	}

	public new class SignalName : Node2D.SignalName
	{
	}

	public void setupItemSprites(ItemObjectHandler parent)
	{
		base.Material = (ShaderMaterial)base.Material.Duplicate();
		foreach (SpriteFrames itemAnimation in parent.itemInformation.ItemAnimations)
		{
			AnimatedSprite2D animatedSprite2D = new AnimatedSprite2D();
			animatedSprite2D.SpriteFrames = itemAnimation;
			animatedSprite2D.TextureFilter = TextureFilterEnum.Nearest;
			animatedSprite2D.UseParentMaterial = true;
			animatedSprite2D.Play(itemAnimation.GetAnimationNames()[0]);
			AddChild(animatedSprite2D, forceReadableName: false, InternalMode.Disabled);
		}
		if (GetChild(0) is AnimatedSprite2D animatedSprite2D2)
		{
			base.Scale = parent.itemInformation.itemScale * Main.Instance.settingItemScaler;
			base.Position = animatedSprite2D2.SpriteFrames.GetFrameTexture(animatedSprite2D2.Animation, 0).GetSize() / 2f * base.Scale;
			base.Position += parent.itemInformation.itemOffset * base.Scale;
		}
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	internal static List<MethodInfo> GetGodotMethodList()
	{
		return new List<MethodInfo>(1)
		{
			new MethodInfo(MethodName.setupItemSprites, new PropertyInfo(Variant.Type.Nil, "", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false), MethodFlags.Normal, new List<PropertyInfo>
			{
				new PropertyInfo(Variant.Type.Object, "parent", PropertyHint.None, "", PropertyUsageFlags.Default, new StringName("Node2D"), exported: false)
			}, null)
		};
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override bool InvokeGodotClassMethod(in godot_string_name method, NativeVariantPtrArgs args, out godot_variant ret)
	{
		if (method == MethodName.setupItemSprites && args.Count == 1)
		{
			setupItemSprites(VariantUtils.ConvertTo<ItemObjectHandler>(in args[0]));
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
