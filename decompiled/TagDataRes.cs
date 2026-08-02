using System.Collections.Generic;
using System.ComponentModel;
using Godot;
using Godot.Bridge;
using Godot.NativeInterop;

[GlobalClass]
[Tool]
[ScriptPath("res://Scripts/DataResources/TagDataRes.cs")]
public class TagDataRes : Resource
{
	public enum actionEnum
	{
		ADD,
		REMOVE,
		REQUIRED
	}

	public new class MethodName : Resource.MethodName
	{
	}

	public new class PropertyName : Resource.PropertyName
	{
		public static readonly StringName tagName = "tagName";

		public static readonly StringName tagAmount = "tagAmount";

		public static readonly StringName tagDuration = "tagDuration";

		public static readonly StringName tagAction = "tagAction";

		public static readonly StringName savedTag = "savedTag";

		public static readonly StringName tagOriginalDuration = "tagOriginalDuration";
	}

	public new class SignalName : Resource.SignalName
	{
	}

	[Export(PropertyHint.None, "")]
	public string tagName = "Unassigned Name";

	[Export(PropertyHint.None, "")]
	public int tagAmount;

	[Export(PropertyHint.None, "")]
	public float tagDuration = 60f;

	[Export(PropertyHint.None, "")]
	public actionEnum tagAction;

	[Export(PropertyHint.None, "")]
	public bool savedTag;

	public float tagOriginalDuration;

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override bool SetGodotClassPropertyValue(in godot_string_name name, in godot_variant value)
	{
		if (name == PropertyName.tagName)
		{
			tagName = VariantUtils.ConvertTo<string>(in value);
			return true;
		}
		if (name == PropertyName.tagAmount)
		{
			tagAmount = VariantUtils.ConvertTo<int>(in value);
			return true;
		}
		if (name == PropertyName.tagDuration)
		{
			tagDuration = VariantUtils.ConvertTo<float>(in value);
			return true;
		}
		if (name == PropertyName.tagAction)
		{
			tagAction = VariantUtils.ConvertTo<actionEnum>(in value);
			return true;
		}
		if (name == PropertyName.savedTag)
		{
			savedTag = VariantUtils.ConvertTo<bool>(in value);
			return true;
		}
		if (name == PropertyName.tagOriginalDuration)
		{
			tagOriginalDuration = VariantUtils.ConvertTo<float>(in value);
			return true;
		}
		return base.SetGodotClassPropertyValue(in name, in value);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override bool GetGodotClassPropertyValue(in godot_string_name name, out godot_variant value)
	{
		if (name == PropertyName.tagName)
		{
			value = VariantUtils.CreateFrom(in tagName);
			return true;
		}
		if (name == PropertyName.tagAmount)
		{
			value = VariantUtils.CreateFrom(in tagAmount);
			return true;
		}
		if (name == PropertyName.tagDuration)
		{
			value = VariantUtils.CreateFrom(in tagDuration);
			return true;
		}
		if (name == PropertyName.tagAction)
		{
			value = VariantUtils.CreateFrom(in tagAction);
			return true;
		}
		if (name == PropertyName.savedTag)
		{
			value = VariantUtils.CreateFrom(in savedTag);
			return true;
		}
		if (name == PropertyName.tagOriginalDuration)
		{
			value = VariantUtils.CreateFrom(in tagOriginalDuration);
			return true;
		}
		return base.GetGodotClassPropertyValue(in name, out value);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	internal static List<PropertyInfo> GetGodotPropertyList()
	{
		return new List<PropertyInfo>
		{
			new PropertyInfo(Variant.Type.String, PropertyName.tagName, PropertyHint.None, "", PropertyUsageFlags.Default | PropertyUsageFlags.ScriptVariable, exported: true),
			new PropertyInfo(Variant.Type.Int, PropertyName.tagAmount, PropertyHint.None, "", PropertyUsageFlags.Default | PropertyUsageFlags.ScriptVariable, exported: true),
			new PropertyInfo(Variant.Type.Float, PropertyName.tagDuration, PropertyHint.None, "", PropertyUsageFlags.Default | PropertyUsageFlags.ScriptVariable, exported: true),
			new PropertyInfo(Variant.Type.Int, PropertyName.tagAction, PropertyHint.Enum, "ADD,REMOVE,REQUIRED", PropertyUsageFlags.Default | PropertyUsageFlags.ScriptVariable, exported: true),
			new PropertyInfo(Variant.Type.Bool, PropertyName.savedTag, PropertyHint.None, "", PropertyUsageFlags.Default | PropertyUsageFlags.ScriptVariable, exported: true),
			new PropertyInfo(Variant.Type.Float, PropertyName.tagOriginalDuration, PropertyHint.None, "", PropertyUsageFlags.ScriptVariable, exported: false)
		};
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override void SaveGodotObjectData(GodotSerializationInfo info)
	{
		base.SaveGodotObjectData(info);
		info.AddProperty(PropertyName.tagName, Variant.From(in tagName));
		info.AddProperty(PropertyName.tagAmount, Variant.From(in tagAmount));
		info.AddProperty(PropertyName.tagDuration, Variant.From(in tagDuration));
		info.AddProperty(PropertyName.tagAction, Variant.From(in tagAction));
		info.AddProperty(PropertyName.savedTag, Variant.From(in savedTag));
		info.AddProperty(PropertyName.tagOriginalDuration, Variant.From(in tagOriginalDuration));
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override void RestoreGodotObjectData(GodotSerializationInfo info)
	{
		base.RestoreGodotObjectData(info);
		if (info.TryGetProperty(PropertyName.tagName, out var value))
		{
			tagName = value.As<string>();
		}
		if (info.TryGetProperty(PropertyName.tagAmount, out var value2))
		{
			tagAmount = value2.As<int>();
		}
		if (info.TryGetProperty(PropertyName.tagDuration, out var value3))
		{
			tagDuration = value3.As<float>();
		}
		if (info.TryGetProperty(PropertyName.tagAction, out var value4))
		{
			tagAction = value4.As<actionEnum>();
		}
		if (info.TryGetProperty(PropertyName.savedTag, out var value5))
		{
			savedTag = value5.As<bool>();
		}
		if (info.TryGetProperty(PropertyName.tagOriginalDuration, out var value6))
		{
			tagOriginalDuration = value6.As<float>();
		}
	}
}
