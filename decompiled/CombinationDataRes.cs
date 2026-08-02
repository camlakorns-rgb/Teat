using System.Collections.Generic;
using System.ComponentModel;
using Godot;
using Godot.Bridge;
using Godot.NativeInterop;

[GlobalClass]
[Tool]
[ScriptPath("res://Scripts/DataResources/CombinationDataRes.cs")]
public class CombinationDataRes : Resource
{
	public new class MethodName : Resource.MethodName
	{
	}

	public new class PropertyName : Resource.PropertyName
	{
		public static readonly StringName outputItem = "outputItem";

		public static readonly StringName requiredItem = "requiredItem";

		public static readonly StringName outputItemPath = "outputItemPath";

		public static readonly StringName requiredItemPath = "requiredItemPath";
	}

	public new class SignalName : Resource.SignalName
	{
	}

	[Export(PropertyHint.File, "*.tres")]
	public string outputItemPath;

	[Export(PropertyHint.File, "*.tres")]
	public string requiredItemPath;

	public ItemDataRes outputItem
	{
		get
		{
			if (string.IsNullOrEmpty(outputItemPath) || (!OS.HasFeature("substar") && !OS.HasFeature("editor") && outputItemPath.Contains("SubStar")))
			{
				return null;
			}
			return GD.Load<ItemDataRes>(outputItemPath);
		}
	}

	public ItemDataRes requiredItem
	{
		get
		{
			if (string.IsNullOrEmpty(requiredItemPath) || (!OS.HasFeature("substar") && !OS.HasFeature("editor") && requiredItemPath.Contains("SubStar")))
			{
				return null;
			}
			return GD.Load<ItemDataRes>(requiredItemPath);
		}
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override bool SetGodotClassPropertyValue(in godot_string_name name, in godot_variant value)
	{
		if (name == PropertyName.outputItemPath)
		{
			outputItemPath = VariantUtils.ConvertTo<string>(in value);
			return true;
		}
		if (name == PropertyName.requiredItemPath)
		{
			requiredItemPath = VariantUtils.ConvertTo<string>(in value);
			return true;
		}
		return base.SetGodotClassPropertyValue(in name, in value);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override bool GetGodotClassPropertyValue(in godot_string_name name, out godot_variant value)
	{
		ItemDataRes from;
		if (name == PropertyName.outputItem)
		{
			from = outputItem;
			value = VariantUtils.CreateFrom(in from);
			return true;
		}
		if (name == PropertyName.requiredItem)
		{
			from = requiredItem;
			value = VariantUtils.CreateFrom(in from);
			return true;
		}
		if (name == PropertyName.outputItemPath)
		{
			value = VariantUtils.CreateFrom(in outputItemPath);
			return true;
		}
		if (name == PropertyName.requiredItemPath)
		{
			value = VariantUtils.CreateFrom(in requiredItemPath);
			return true;
		}
		return base.GetGodotClassPropertyValue(in name, out value);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	internal static List<PropertyInfo> GetGodotPropertyList()
	{
		return new List<PropertyInfo>
		{
			new PropertyInfo(Variant.Type.String, PropertyName.outputItemPath, PropertyHint.File, "*.tres", PropertyUsageFlags.Default | PropertyUsageFlags.ScriptVariable, exported: true),
			new PropertyInfo(Variant.Type.String, PropertyName.requiredItemPath, PropertyHint.File, "*.tres", PropertyUsageFlags.Default | PropertyUsageFlags.ScriptVariable, exported: true),
			new PropertyInfo(Variant.Type.Object, PropertyName.outputItem, PropertyHint.None, "", PropertyUsageFlags.ScriptVariable, exported: false),
			new PropertyInfo(Variant.Type.Object, PropertyName.requiredItem, PropertyHint.None, "", PropertyUsageFlags.ScriptVariable, exported: false)
		};
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override void SaveGodotObjectData(GodotSerializationInfo info)
	{
		base.SaveGodotObjectData(info);
		info.AddProperty(PropertyName.outputItemPath, Variant.From(in outputItemPath));
		info.AddProperty(PropertyName.requiredItemPath, Variant.From(in requiredItemPath));
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override void RestoreGodotObjectData(GodotSerializationInfo info)
	{
		base.RestoreGodotObjectData(info);
		if (info.TryGetProperty(PropertyName.outputItemPath, out var value))
		{
			outputItemPath = value.As<string>();
		}
		if (info.TryGetProperty(PropertyName.requiredItemPath, out var value2))
		{
			requiredItemPath = value2.As<string>();
		}
	}
}
