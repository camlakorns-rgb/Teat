using System.Collections.Generic;
using System.ComponentModel;
using Godot;
using Godot.Bridge;
using Godot.NativeInterop;

[ScriptPath("res://Scripts/CharacterScripts/ActorSpriteController.cs")]
public class ActorSpriteController : Node2D
{
	public new class MethodName : Node2D.MethodName
	{
		public static readonly StringName setupCharacterSprites = "setupCharacterSprites";
	}

	public new class PropertyName : Node2D.PropertyName
	{
		public static readonly StringName shader = "shader";

		public static readonly StringName animatedSprite2D = "animatedSprite2D";
	}

	public new class SignalName : Node2D.SignalName
	{
	}

	[Export(PropertyHint.None, "")]
	private ShaderMaterial shader;

	[Export(PropertyHint.None, "")]
	private PackedScene animatedSprite2D;

	public void setupCharacterSprites(ActorCharacter parent)
	{
		foreach (CharAnimDataRes characterAnimationLayer in parent.characterInformation.characterAnimationLayers)
		{
			AnimatedSprite2D animatedSprite2D = this.animatedSprite2D.Instantiate<AnimatedSprite2D>(PackedScene.GenEditState.Disabled);
			animatedSprite2D.SpriteFrames = characterAnimationLayer.CharacterAnimationData;
			animatedSprite2D.Offset = characterAnimationLayer.spriteSpecificOffset;
			animatedSprite2D.TextureFilter = TextureFilterEnum.Nearest;
			animatedSprite2D.Play(characterAnimationLayer.DefaultAnimation);
			AddChild(animatedSprite2D, forceReadableName: false, InternalMode.Disabled);
			if (characterAnimationLayer.animationType == CharAnimDataRes.AnimationTyping.MAIN_BODY)
			{
				parent.MainBody = animatedSprite2D;
			}
			ShaderMaterial shaderMaterial = (shaderMaterial = (ShaderMaterial)shader.Duplicate());
			shaderMaterial.ResourceLocalToScene = true;
			animatedSprite2D.Material = shaderMaterial;
		}
		if (GetChild(0) is AnimatedSprite2D animatedSprite2D2)
		{
			base.Scale = parent.characterInformation.characterScale * Main.Instance.settingSpriteScaler;
			base.Position = animatedSprite2D2.SpriteFrames.GetFrameTexture(animatedSprite2D2.Animation, 0).GetSize() / 2f * base.Scale;
			base.Position += parent.characterInformation.characterOffset * base.Scale;
		}
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	internal static List<MethodInfo> GetGodotMethodList()
	{
		return new List<MethodInfo>(1)
		{
			new MethodInfo(MethodName.setupCharacterSprites, new PropertyInfo(Variant.Type.Nil, "", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false), MethodFlags.Normal, new List<PropertyInfo>
			{
				new PropertyInfo(Variant.Type.Object, "parent", PropertyHint.None, "", PropertyUsageFlags.Default, new StringName("Node2D"), exported: false)
			}, null)
		};
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override bool InvokeGodotClassMethod(in godot_string_name method, NativeVariantPtrArgs args, out godot_variant ret)
	{
		if (method == MethodName.setupCharacterSprites && args.Count == 1)
		{
			setupCharacterSprites(VariantUtils.ConvertTo<ActorCharacter>(in args[0]));
			ret = default(godot_variant);
			return true;
		}
		return base.InvokeGodotClassMethod(in method, args, out ret);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override bool HasGodotClassMethod(in godot_string_name method)
	{
		if (method == MethodName.setupCharacterSprites)
		{
			return true;
		}
		return base.HasGodotClassMethod(in method);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override bool SetGodotClassPropertyValue(in godot_string_name name, in godot_variant value)
	{
		if (name == PropertyName.shader)
		{
			shader = VariantUtils.ConvertTo<ShaderMaterial>(in value);
			return true;
		}
		if (name == PropertyName.animatedSprite2D)
		{
			animatedSprite2D = VariantUtils.ConvertTo<PackedScene>(in value);
			return true;
		}
		return base.SetGodotClassPropertyValue(in name, in value);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override bool GetGodotClassPropertyValue(in godot_string_name name, out godot_variant value)
	{
		if (name == PropertyName.shader)
		{
			value = VariantUtils.CreateFrom(in shader);
			return true;
		}
		if (name == PropertyName.animatedSprite2D)
		{
			value = VariantUtils.CreateFrom(in animatedSprite2D);
			return true;
		}
		return base.GetGodotClassPropertyValue(in name, out value);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	internal static List<PropertyInfo> GetGodotPropertyList()
	{
		return new List<PropertyInfo>
		{
			new PropertyInfo(Variant.Type.Object, PropertyName.shader, PropertyHint.ResourceType, "ShaderMaterial", PropertyUsageFlags.Default | PropertyUsageFlags.ScriptVariable, exported: true),
			new PropertyInfo(Variant.Type.Object, PropertyName.animatedSprite2D, PropertyHint.ResourceType, "PackedScene", PropertyUsageFlags.Default | PropertyUsageFlags.ScriptVariable, exported: true)
		};
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override void SaveGodotObjectData(GodotSerializationInfo info)
	{
		base.SaveGodotObjectData(info);
		info.AddProperty(PropertyName.shader, Variant.From(in shader));
		info.AddProperty(PropertyName.animatedSprite2D, Variant.From(in animatedSprite2D));
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override void RestoreGodotObjectData(GodotSerializationInfo info)
	{
		base.RestoreGodotObjectData(info);
		if (info.TryGetProperty(PropertyName.shader, out var value))
		{
			shader = value.As<ShaderMaterial>();
		}
		if (info.TryGetProperty(PropertyName.animatedSprite2D, out var value2))
		{
			animatedSprite2D = value2.As<PackedScene>();
		}
	}
}
