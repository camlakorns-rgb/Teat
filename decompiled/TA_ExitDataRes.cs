using System.Collections.Generic;
using System.ComponentModel;
using Godot;
using Godot.Bridge;
using Godot.NativeInterop;

[GlobalClass]
[ScriptPath("res://Scripts/SubMenus/TerminalMenu/TerminalAdventure/TA_ExitDataRes.cs")]
public class TA_ExitDataRes : Resource
{
	public new class MethodName : Resource.MethodName
	{
	}

	public new class PropertyName : Resource.PropertyName
	{
		public static readonly StringName From = "From";

		public static readonly StringName Label = "Label";

		public static readonly StringName To = "To";
	}

	public new class SignalName : Resource.SignalName
	{
	}

	[Export(PropertyHint.None, "")]
	public TA_RoomDataRes From;

	[Export(PropertyHint.None, "")]
	public string Label = "";

	[Export(PropertyHint.None, "")]
	public TA_RoomDataRes To;

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override bool SetGodotClassPropertyValue(in godot_string_name name, in godot_variant value)
	{
		if (name == PropertyName.From)
		{
			From = VariantUtils.ConvertTo<TA_RoomDataRes>(in value);
			return true;
		}
		if (name == PropertyName.Label)
		{
			Label = VariantUtils.ConvertTo<string>(in value);
			return true;
		}
		if (name == PropertyName.To)
		{
			To = VariantUtils.ConvertTo<TA_RoomDataRes>(in value);
			return true;
		}
		return base.SetGodotClassPropertyValue(in name, in value);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override bool GetGodotClassPropertyValue(in godot_string_name name, out godot_variant value)
	{
		if (name == PropertyName.From)
		{
			value = VariantUtils.CreateFrom(in From);
			return true;
		}
		if (name == PropertyName.Label)
		{
			value = VariantUtils.CreateFrom(in Label);
			return true;
		}
		if (name == PropertyName.To)
		{
			value = VariantUtils.CreateFrom(in To);
			return true;
		}
		return base.GetGodotClassPropertyValue(in name, out value);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	internal static List<PropertyInfo> GetGodotPropertyList()
	{
		return new List<PropertyInfo>
		{
			new PropertyInfo(Variant.Type.Object, PropertyName.From, PropertyHint.ResourceType, "TA_RoomDataRes", PropertyUsageFlags.Default | PropertyUsageFlags.ScriptVariable, exported: true),
			new PropertyInfo(Variant.Type.String, PropertyName.Label, PropertyHint.None, "", PropertyUsageFlags.Default | PropertyUsageFlags.ScriptVariable, exported: true),
			new PropertyInfo(Variant.Type.Object, PropertyName.To, PropertyHint.ResourceType, "TA_RoomDataRes", PropertyUsageFlags.Default | PropertyUsageFlags.ScriptVariable, exported: true)
		};
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override void SaveGodotObjectData(GodotSerializationInfo info)
	{
		base.SaveGodotObjectData(info);
		info.AddProperty(PropertyName.From, Variant.From(in From));
		info.AddProperty(PropertyName.Label, Variant.From(in Label));
		info.AddProperty(PropertyName.To, Variant.From(in To));
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override void RestoreGodotObjectData(GodotSerializationInfo info)
	{
		base.RestoreGodotObjectData(info);
		if (info.TryGetProperty(PropertyName.From, out var value))
		{
			From = value.As<TA_RoomDataRes>();
		}
		if (info.TryGetProperty(PropertyName.Label, out var value2))
		{
			Label = value2.As<string>();
		}
		if (info.TryGetProperty(PropertyName.To, out var value3))
		{
			To = value3.As<TA_RoomDataRes>();
		}
	}
}
