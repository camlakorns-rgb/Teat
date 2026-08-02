using System.Collections.Generic;
using System.ComponentModel;
using Godot;
using Godot.Bridge;
using Godot.Collections;
using Godot.NativeInterop;

[GlobalClass]
[Tool]
[ScriptPath("res://Scripts/DataResources/ConvoDataRes.cs")]
public class ConvoDataRes : Resource
{
	public new class MethodName : Resource.MethodName
	{
	}

	public new class PropertyName : Resource.PropertyName
	{
		public static readonly StringName convoName = "convoName";

		public static readonly StringName Weight = "Weight";

		public static readonly StringName convoStack = "convoStack";

		public static readonly StringName RequiredTags = "RequiredTags";

		public static readonly StringName taggedKinks = "taggedKinks";
	}

	public new class SignalName : Resource.SignalName
	{
	}

	[Export(PropertyHint.None, "")]
	private string convoName = "Unnamed Convo";

	[Export(PropertyHint.None, "")]
	public float Weight = 25f;

	[Export(PropertyHint.None, "")]
	public Array<DialogueDataRes> convoStack = new Array<DialogueDataRes>();

	[ExportGroup("Tag Specific", "")]
	[Export(PropertyHint.None, "")]
	public Array<TagDataRes> RequiredTags = new Array<TagDataRes>();

	[ExportGroup("Kink Settings", "")]
	[Export(PropertyHint.None, "")]
	public Array<SaveHandler.Kinks> taggedKinks = new Array<SaveHandler.Kinks>();

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override bool SetGodotClassPropertyValue(in godot_string_name name, in godot_variant value)
	{
		if (name == PropertyName.convoName)
		{
			convoName = VariantUtils.ConvertTo<string>(in value);
			return true;
		}
		if (name == PropertyName.Weight)
		{
			Weight = VariantUtils.ConvertTo<float>(in value);
			return true;
		}
		if (name == PropertyName.convoStack)
		{
			convoStack = VariantUtils.ConvertToArray<DialogueDataRes>(in value);
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
		if (name == PropertyName.convoName)
		{
			value = VariantUtils.CreateFrom(in convoName);
			return true;
		}
		if (name == PropertyName.Weight)
		{
			value = VariantUtils.CreateFrom(in Weight);
			return true;
		}
		if (name == PropertyName.convoStack)
		{
			value = VariantUtils.CreateFromArray(convoStack);
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
			new PropertyInfo(Variant.Type.String, PropertyName.convoName, PropertyHint.None, "", PropertyUsageFlags.Default | PropertyUsageFlags.ScriptVariable, exported: true),
			new PropertyInfo(Variant.Type.Float, PropertyName.Weight, PropertyHint.None, "", PropertyUsageFlags.Default | PropertyUsageFlags.ScriptVariable, exported: true),
			new PropertyInfo(Variant.Type.Array, PropertyName.convoStack, PropertyHint.TypeString, "24/17:DialogueDataRes", PropertyUsageFlags.Default | PropertyUsageFlags.ScriptVariable, exported: true),
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
		info.AddProperty(PropertyName.convoName, Variant.From(in convoName));
		info.AddProperty(PropertyName.Weight, Variant.From(in Weight));
		info.AddProperty(PropertyName.convoStack, Variant.CreateFrom(convoStack));
		info.AddProperty(PropertyName.RequiredTags, Variant.CreateFrom(RequiredTags));
		info.AddProperty(PropertyName.taggedKinks, Variant.CreateFrom(taggedKinks));
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override void RestoreGodotObjectData(GodotSerializationInfo info)
	{
		base.RestoreGodotObjectData(info);
		if (info.TryGetProperty(PropertyName.convoName, out var value))
		{
			convoName = value.As<string>();
		}
		if (info.TryGetProperty(PropertyName.Weight, out var value2))
		{
			Weight = value2.As<float>();
		}
		if (info.TryGetProperty(PropertyName.convoStack, out var value3))
		{
			convoStack = value3.AsGodotArray<DialogueDataRes>();
		}
		if (info.TryGetProperty(PropertyName.RequiredTags, out var value4))
		{
			RequiredTags = value4.AsGodotArray<TagDataRes>();
		}
		if (info.TryGetProperty(PropertyName.taggedKinks, out var value5))
		{
			taggedKinks = value5.AsGodotArray<SaveHandler.Kinks>();
		}
	}
}
