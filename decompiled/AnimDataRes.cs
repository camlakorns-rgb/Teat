using System.Collections.Generic;
using System.ComponentModel;
using Godot;
using Godot.Bridge;
using Godot.Collections;
using Godot.NativeInterop;

[Tool]
[GlobalClass]
[ScriptPath("res://Scripts/CharacterScripts/AnimDataRes.cs")]
public class AnimDataRes : Resource
{
	public new class MethodName : Resource.MethodName
	{
	}

	public new class PropertyName : Resource.PropertyName
	{
		public static readonly StringName animationName = "animationName";

		public static readonly StringName animationAppeanceWeight = "animationAppeanceWeight";

		public static readonly StringName hasTransition = "hasTransition";

		public static readonly StringName randomTime = "randomTime";

		public static readonly StringName RequiredTags = "RequiredTags";

		public static readonly StringName taggedKinks = "taggedKinks";
	}

	public new class SignalName : Resource.SignalName
	{
	}

	[Export(PropertyHint.None, "")]
	public string animationName = "INVALID_ANIMATION";

	[Export(PropertyHint.None, "")]
	public float animationAppeanceWeight = 25f;

	[Export(PropertyHint.None, "")]
	public bool hasTransition;

	[Export(PropertyHint.None, "")]
	public Vector2 randomTime = new Vector2(1f, 2f);

	[ExportGroup("Tag Specific", "")]
	[Export(PropertyHint.None, "")]
	public Array<TagDataRes> RequiredTags = new Array<TagDataRes>();

	[ExportGroup("Kink Settings", "")]
	[Export(PropertyHint.None, "")]
	public Array<SaveHandler.Kinks> taggedKinks = new Array<SaveHandler.Kinks>();

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override bool SetGodotClassPropertyValue(in godot_string_name name, in godot_variant value)
	{
		if (name == PropertyName.animationName)
		{
			animationName = VariantUtils.ConvertTo<string>(in value);
			return true;
		}
		if (name == PropertyName.animationAppeanceWeight)
		{
			animationAppeanceWeight = VariantUtils.ConvertTo<float>(in value);
			return true;
		}
		if (name == PropertyName.hasTransition)
		{
			hasTransition = VariantUtils.ConvertTo<bool>(in value);
			return true;
		}
		if (name == PropertyName.randomTime)
		{
			randomTime = VariantUtils.ConvertTo<Vector2>(in value);
			return true;
		}
		if (name == PropertyName.RequiredTags)
		{
			RequiredTags = VariantUtils.ConvertToArray<TagDataRes>(in value);
			return true;
		}
		if (name == PropertyName.taggedKinks)
		{
			taggedKinks = VariantUtils.ConvertToArray<SaveHandler.Kinks>(in value);
			return true;
		}
		return base.SetGodotClassPropertyValue(in name, in value);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override bool GetGodotClassPropertyValue(in godot_string_name name, out godot_variant value)
	{
		if (name == PropertyName.animationName)
		{
			value = VariantUtils.CreateFrom(in animationName);
			return true;
		}
		if (name == PropertyName.animationAppeanceWeight)
		{
			value = VariantUtils.CreateFrom(in animationAppeanceWeight);
			return true;
		}
		if (name == PropertyName.hasTransition)
		{
			value = VariantUtils.CreateFrom(in hasTransition);
			return true;
		}
		if (name == PropertyName.randomTime)
		{
			value = VariantUtils.CreateFrom(in randomTime);
			return true;
		}
		if (name == PropertyName.RequiredTags)
		{
			value = VariantUtils.CreateFromArray(RequiredTags);
			return true;
		}
		if (name == PropertyName.taggedKinks)
		{
			value = VariantUtils.CreateFromArray(taggedKinks);
			return true;
		}
		return base.GetGodotClassPropertyValue(in name, out value);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	internal static List<PropertyInfo> GetGodotPropertyList()
	{
		return new List<PropertyInfo>
		{
			new PropertyInfo(Variant.Type.String, PropertyName.animationName, PropertyHint.None, "", PropertyUsageFlags.Default | PropertyUsageFlags.ScriptVariable, exported: true),
			new PropertyInfo(Variant.Type.Float, PropertyName.animationAppeanceWeight, PropertyHint.None, "", PropertyUsageFlags.Default | PropertyUsageFlags.ScriptVariable, exported: true),
			new PropertyInfo(Variant.Type.Bool, PropertyName.hasTransition, PropertyHint.None, "", PropertyUsageFlags.Default | PropertyUsageFlags.ScriptVariable, exported: true),
			new PropertyInfo(Variant.Type.Vector2, PropertyName.randomTime, PropertyHint.None, "", PropertyUsageFlags.Default | PropertyUsageFlags.ScriptVariable, exported: true),
			new PropertyInfo(Variant.Type.Nil, "Tag Specific", PropertyHint.None, "", PropertyUsageFlags.Group, exported: true),
			new PropertyInfo(Variant.Type.Array, PropertyName.RequiredTags, PropertyHint.TypeString, "24/17:TagDataRes", PropertyUsageFlags.Default | PropertyUsageFlags.ScriptVariable, exported: true),
			new PropertyInfo(Variant.Type.Nil, "Kink Settings", PropertyHint.None, "", PropertyUsageFlags.Group, exported: true),
			new PropertyInfo(Variant.Type.Array, PropertyName.taggedKinks, PropertyHint.TypeString, "2/2:UNTYPED,CUCKING", PropertyUsageFlags.Default | PropertyUsageFlags.ScriptVariable, exported: true)
		};
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override void SaveGodotObjectData(GodotSerializationInfo info)
	{
		base.SaveGodotObjectData(info);
		info.AddProperty(PropertyName.animationName, Variant.From(in animationName));
		info.AddProperty(PropertyName.animationAppeanceWeight, Variant.From(in animationAppeanceWeight));
		info.AddProperty(PropertyName.hasTransition, Variant.From(in hasTransition));
		info.AddProperty(PropertyName.randomTime, Variant.From(in randomTime));
		info.AddProperty(PropertyName.RequiredTags, Variant.CreateFrom(RequiredTags));
		info.AddProperty(PropertyName.taggedKinks, Variant.CreateFrom(taggedKinks));
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override void RestoreGodotObjectData(GodotSerializationInfo info)
	{
		base.RestoreGodotObjectData(info);
		if (info.TryGetProperty(PropertyName.animationName, out var value))
		{
			animationName = value.As<string>();
		}
		if (info.TryGetProperty(PropertyName.animationAppeanceWeight, out var value2))
		{
			animationAppeanceWeight = value2.As<float>();
		}
		if (info.TryGetProperty(PropertyName.hasTransition, out var value3))
		{
			hasTransition = value3.As<bool>();
		}
		if (info.TryGetProperty(PropertyName.randomTime, out var value4))
		{
			randomTime = value4.As<Vector2>();
		}
		if (info.TryGetProperty(PropertyName.RequiredTags, out var value5))
		{
			RequiredTags = value5.AsGodotArray<TagDataRes>();
		}
		if (info.TryGetProperty(PropertyName.taggedKinks, out var value6))
		{
			taggedKinks = value6.AsGodotArray<SaveHandler.Kinks>();
		}
	}
}
