using System.Collections.Generic;
using System.ComponentModel;
using Godot;
using Godot.Bridge;
using Godot.Collections;
using Godot.NativeInterop;

[GlobalClass]
[Tool]
[ScriptPath("res://Scripts/DataResources/CharAnimDataRes.cs")]
public class CharAnimDataRes : Resource
{
	public enum AnimationTyping
	{
		UNTYPED,
		MAIN_BODY,
		FACE_BODY,
		ATTACHED_BODY
	}

	public new class MethodName : Resource.MethodName
	{
	}

	public new class PropertyName : Resource.PropertyName
	{
		public static readonly StringName Name = "Name";

		public static readonly StringName animationType = "animationType";

		public static readonly StringName CharacterAnimationData = "CharacterAnimationData";

		public static readonly StringName DefaultAnimation = "DefaultAnimation";

		public static readonly StringName ZLayer = "ZLayer";

		public static readonly StringName RandomStateTimer = "RandomStateTimer";

		public static readonly StringName defaultVisibility = "defaultVisibility";

		public static readonly StringName whitelistedTags = "whitelistedTags";

		public static readonly StringName blacklistedTags = "blacklistedTags";

		public static readonly StringName spriteSpecificOffset = "spriteSpecificOffset";
	}

	public new class SignalName : Resource.SignalName
	{
	}

	[Export(PropertyHint.None, "")]
	public string Name = "UNNAMED";

	[Export(PropertyHint.None, "")]
	public AnimationTyping animationType;

	[Export(PropertyHint.None, "")]
	public SpriteFrames CharacterAnimationData;

	[Export(PropertyHint.None, "")]
	public string DefaultAnimation = "INVALID_DEFAULT_ANIMATION";

	[Export(PropertyHint.None, "")]
	public int ZLayer;

	[ExportGroup("Random Timer", "")]
	[Export(PropertyHint.None, "")]
	public Vector2 RandomStateTimer = new Vector2(0f, 1f);

	[ExportGroup("Attachment Tags", "")]
	[Export(PropertyHint.None, "")]
	public bool defaultVisibility;

	[Export(PropertyHint.None, "")]
	public Array<TagDataRes> whitelistedTags = new Array<TagDataRes>();

	[Export(PropertyHint.None, "")]
	public Array<TagDataRes> blacklistedTags = new Array<TagDataRes>();

	[ExportGroup("Sprite Information", "")]
	[Export(PropertyHint.None, "")]
	public Vector2 spriteSpecificOffset = new Vector2I(0, 0);

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override bool SetGodotClassPropertyValue(in godot_string_name name, in godot_variant value)
	{
		if (name == PropertyName.Name)
		{
			Name = VariantUtils.ConvertTo<string>(in value);
			return true;
		}
		if (name == PropertyName.animationType)
		{
			animationType = VariantUtils.ConvertTo<AnimationTyping>(in value);
			return true;
		}
		if (name == PropertyName.CharacterAnimationData)
		{
			CharacterAnimationData = VariantUtils.ConvertTo<SpriteFrames>(in value);
			return true;
		}
		if (name == PropertyName.DefaultAnimation)
		{
			DefaultAnimation = VariantUtils.ConvertTo<string>(in value);
			return true;
		}
		if (name == PropertyName.ZLayer)
		{
			ZLayer = VariantUtils.ConvertTo<int>(in value);
			return true;
		}
		if (name == PropertyName.RandomStateTimer)
		{
			RandomStateTimer = VariantUtils.ConvertTo<Vector2>(in value);
			return true;
		}
		if (name == PropertyName.defaultVisibility)
		{
			defaultVisibility = VariantUtils.ConvertTo<bool>(in value);
			return true;
		}
		if (name == PropertyName.whitelistedTags)
		{
			whitelistedTags = VariantUtils.ConvertToArray<TagDataRes>(in value);
			return true;
		}
		if (name == PropertyName.blacklistedTags)
		{
			blacklistedTags = VariantUtils.ConvertToArray<TagDataRes>(in value);
			return true;
		}
		if (name == PropertyName.spriteSpecificOffset)
		{
			spriteSpecificOffset = VariantUtils.ConvertTo<Vector2>(in value);
			return true;
		}
		return base.SetGodotClassPropertyValue(in name, in value);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override bool GetGodotClassPropertyValue(in godot_string_name name, out godot_variant value)
	{
		if (name == PropertyName.Name)
		{
			value = VariantUtils.CreateFrom(in Name);
			return true;
		}
		if (name == PropertyName.animationType)
		{
			value = VariantUtils.CreateFrom(in animationType);
			return true;
		}
		if (name == PropertyName.CharacterAnimationData)
		{
			value = VariantUtils.CreateFrom(in CharacterAnimationData);
			return true;
		}
		if (name == PropertyName.DefaultAnimation)
		{
			value = VariantUtils.CreateFrom(in DefaultAnimation);
			return true;
		}
		if (name == PropertyName.ZLayer)
		{
			value = VariantUtils.CreateFrom(in ZLayer);
			return true;
		}
		if (name == PropertyName.RandomStateTimer)
		{
			value = VariantUtils.CreateFrom(in RandomStateTimer);
			return true;
		}
		if (name == PropertyName.defaultVisibility)
		{
			value = VariantUtils.CreateFrom(in defaultVisibility);
			return true;
		}
		if (name == PropertyName.whitelistedTags)
		{
			value = VariantUtils.CreateFromArray(whitelistedTags);
			return true;
		}
		if (name == PropertyName.blacklistedTags)
		{
			value = VariantUtils.CreateFromArray(blacklistedTags);
			return true;
		}
		if (name == PropertyName.spriteSpecificOffset)
		{
			value = VariantUtils.CreateFrom(in spriteSpecificOffset);
			return true;
		}
		return base.GetGodotClassPropertyValue(in name, out value);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	internal static List<PropertyInfo> GetGodotPropertyList()
	{
		return new List<PropertyInfo>
		{
			new PropertyInfo(Variant.Type.String, PropertyName.Name, PropertyHint.None, "", PropertyUsageFlags.Default | PropertyUsageFlags.ScriptVariable, exported: true),
			new PropertyInfo(Variant.Type.Int, PropertyName.animationType, PropertyHint.Enum, "UNTYPED,MAIN_BODY,FACE_BODY,ATTACHED_BODY", PropertyUsageFlags.Default | PropertyUsageFlags.ScriptVariable, exported: true),
			new PropertyInfo(Variant.Type.Object, PropertyName.CharacterAnimationData, PropertyHint.ResourceType, "SpriteFrames", PropertyUsageFlags.Default | PropertyUsageFlags.ScriptVariable, exported: true),
			new PropertyInfo(Variant.Type.String, PropertyName.DefaultAnimation, PropertyHint.None, "", PropertyUsageFlags.Default | PropertyUsageFlags.ScriptVariable, exported: true),
			new PropertyInfo(Variant.Type.Int, PropertyName.ZLayer, PropertyHint.None, "", PropertyUsageFlags.Default | PropertyUsageFlags.ScriptVariable, exported: true),
			new PropertyInfo(Variant.Type.Nil, "Random Timer", PropertyHint.None, "", PropertyUsageFlags.Group, exported: true),
			new PropertyInfo(Variant.Type.Vector2, PropertyName.RandomStateTimer, PropertyHint.None, "", PropertyUsageFlags.Default | PropertyUsageFlags.ScriptVariable, exported: true),
			new PropertyInfo(Variant.Type.Nil, "Attachment Tags", PropertyHint.None, "", PropertyUsageFlags.Group, exported: true),
			new PropertyInfo(Variant.Type.Bool, PropertyName.defaultVisibility, PropertyHint.None, "", PropertyUsageFlags.Default | PropertyUsageFlags.ScriptVariable, exported: true),
			new PropertyInfo(Variant.Type.Array, PropertyName.whitelistedTags, PropertyHint.TypeString, "24/17:TagDataRes", PropertyUsageFlags.Default | PropertyUsageFlags.ScriptVariable, exported: true),
			new PropertyInfo(Variant.Type.Array, PropertyName.blacklistedTags, PropertyHint.TypeString, "24/17:TagDataRes", PropertyUsageFlags.Default | PropertyUsageFlags.ScriptVariable, exported: true),
			new PropertyInfo(Variant.Type.Nil, "Sprite Information", PropertyHint.None, "", PropertyUsageFlags.Group, exported: true),
			new PropertyInfo(Variant.Type.Vector2, PropertyName.spriteSpecificOffset, PropertyHint.None, "", PropertyUsageFlags.Default | PropertyUsageFlags.ScriptVariable, exported: true)
		};
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override void SaveGodotObjectData(GodotSerializationInfo info)
	{
		base.SaveGodotObjectData(info);
		info.AddProperty(PropertyName.Name, Variant.From(in Name));
		info.AddProperty(PropertyName.animationType, Variant.From(in animationType));
		info.AddProperty(PropertyName.CharacterAnimationData, Variant.From(in CharacterAnimationData));
		info.AddProperty(PropertyName.DefaultAnimation, Variant.From(in DefaultAnimation));
		info.AddProperty(PropertyName.ZLayer, Variant.From(in ZLayer));
		info.AddProperty(PropertyName.RandomStateTimer, Variant.From(in RandomStateTimer));
		info.AddProperty(PropertyName.defaultVisibility, Variant.From(in defaultVisibility));
		info.AddProperty(PropertyName.whitelistedTags, Variant.CreateFrom(whitelistedTags));
		info.AddProperty(PropertyName.blacklistedTags, Variant.CreateFrom(blacklistedTags));
		info.AddProperty(PropertyName.spriteSpecificOffset, Variant.From(in spriteSpecificOffset));
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override void RestoreGodotObjectData(GodotSerializationInfo info)
	{
		base.RestoreGodotObjectData(info);
		if (info.TryGetProperty(PropertyName.Name, out var value))
		{
			Name = value.As<string>();
		}
		if (info.TryGetProperty(PropertyName.animationType, out var value2))
		{
			animationType = value2.As<AnimationTyping>();
		}
		if (info.TryGetProperty(PropertyName.CharacterAnimationData, out var value3))
		{
			CharacterAnimationData = value3.As<SpriteFrames>();
		}
		if (info.TryGetProperty(PropertyName.DefaultAnimation, out var value4))
		{
			DefaultAnimation = value4.As<string>();
		}
		if (info.TryGetProperty(PropertyName.ZLayer, out var value5))
		{
			ZLayer = value5.As<int>();
		}
		if (info.TryGetProperty(PropertyName.RandomStateTimer, out var value6))
		{
			RandomStateTimer = value6.As<Vector2>();
		}
		if (info.TryGetProperty(PropertyName.defaultVisibility, out var value7))
		{
			defaultVisibility = value7.As<bool>();
		}
		if (info.TryGetProperty(PropertyName.whitelistedTags, out var value8))
		{
			whitelistedTags = value8.AsGodotArray<TagDataRes>();
		}
		if (info.TryGetProperty(PropertyName.blacklistedTags, out var value9))
		{
			blacklistedTags = value9.AsGodotArray<TagDataRes>();
		}
		if (info.TryGetProperty(PropertyName.spriteSpecificOffset, out var value10))
		{
			spriteSpecificOffset = value10.As<Vector2>();
		}
	}
}
