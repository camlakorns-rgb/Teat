using System.Collections.Generic;
using System.ComponentModel;
using Godot;
using Godot.Bridge;
using Godot.Collections;
using Godot.NativeInterop;

[GlobalClass]
[Tool]
[ScriptPath("res://Scripts/DataResources/TagOverrideDataRes.cs")]
public class TagOverrideDataRes : Resource
{
	public new class MethodName : Resource.MethodName
	{
	}

	public new class PropertyName : Resource.PropertyName
	{
		public static readonly StringName requiredTags = "requiredTags";

		public static readonly StringName possibleOverrides = "possibleOverrides";
	}

	public new class SignalName : Resource.SignalName
	{
	}

	[Export(PropertyHint.None, "")]
	public TagDataRes requiredTags;

	[Export(PropertyHint.None, "")]
	public Array<AttachDataRes> possibleOverrides;

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override bool SetGodotClassPropertyValue(in godot_string_name name, in godot_variant value)
	{
		if (name == PropertyName.requiredTags)
		{
			requiredTags = VariantUtils.ConvertTo<TagDataRes>(in value);
			return true;
		}
		if (name == PropertyName.possibleOverrides)
		{
			possibleOverrides = VariantUtils.ConvertToArray<AttachDataRes>(in value);
			return true;
		}
		return base.SetGodotClassPropertyValue(in name, in value);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override bool GetGodotClassPropertyValue(in godot_string_name name, out godot_variant value)
	{
		if (name == PropertyName.requiredTags)
		{
			value = VariantUtils.CreateFrom(in requiredTags);
			return true;
		}
		if (name == PropertyName.possibleOverrides)
		{
			value = VariantUtils.CreateFromArray(possibleOverrides);
			return true;
		}
		return base.GetGodotClassPropertyValue(in name, out value);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	internal static List<PropertyInfo> GetGodotPropertyList()
	{
		return new List<PropertyInfo>
		{
			new PropertyInfo(Variant.Type.Object, PropertyName.requiredTags, PropertyHint.ResourceType, "TagDataRes", PropertyUsageFlags.Default | PropertyUsageFlags.ScriptVariable, exported: true),
			new PropertyInfo(Variant.Type.Array, PropertyName.possibleOverrides, PropertyHint.TypeString, "24/17:AttachDataRes", PropertyUsageFlags.Default | PropertyUsageFlags.ScriptVariable, exported: true)
		};
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override void SaveGodotObjectData(GodotSerializationInfo info)
	{
		base.SaveGodotObjectData(info);
		info.AddProperty(PropertyName.requiredTags, Variant.From(in requiredTags));
		info.AddProperty(PropertyName.possibleOverrides, Variant.CreateFrom(possibleOverrides));
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override void RestoreGodotObjectData(GodotSerializationInfo info)
	{
		base.RestoreGodotObjectData(info);
		if (info.TryGetProperty(PropertyName.requiredTags, out var value))
		{
			requiredTags = value.As<TagDataRes>();
		}
		if (info.TryGetProperty(PropertyName.possibleOverrides, out var value2))
		{
			possibleOverrides = value2.AsGodotArray<AttachDataRes>();
		}
	}
}
