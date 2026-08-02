using System.Collections.Generic;
using System.ComponentModel;
using Godot;
using Godot.Bridge;
using Godot.Collections;
using Godot.NativeInterop;

[GlobalClass]
[Tool]
[ScriptPath("res://Scripts/DataResources/TerminalEEDataRes.cs")]
public class TerminalEEDataRes : Resource
{
	public new class MethodName : Resource.MethodName
	{
	}

	public new class PropertyName : Resource.PropertyName
	{
		public static readonly StringName EEName = "EEName";

		public static readonly StringName possibleStrings = "possibleStrings";
	}

	public new class SignalName : Resource.SignalName
	{
	}

	[Export(PropertyHint.None, "")]
	public string EEName = "INVALID";

	[Export(PropertyHint.MultilineText, "")]
	public Array<string> possibleStrings = new Array<string>();

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override bool SetGodotClassPropertyValue(in godot_string_name name, in godot_variant value)
	{
		if (name == PropertyName.EEName)
		{
			EEName = VariantUtils.ConvertTo<string>(in value);
			return true;
		}
		if (name == PropertyName.possibleStrings)
		{
			possibleStrings = VariantUtils.ConvertToArray<string>(in value);
			return true;
		}
		return base.SetGodotClassPropertyValue(in name, in value);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override bool GetGodotClassPropertyValue(in godot_string_name name, out godot_variant value)
	{
		if (name == PropertyName.EEName)
		{
			value = VariantUtils.CreateFrom(in EEName);
			return true;
		}
		if (name == PropertyName.possibleStrings)
		{
			value = VariantUtils.CreateFromArray(possibleStrings);
			return true;
		}
		return base.GetGodotClassPropertyValue(in name, out value);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	internal static List<PropertyInfo> GetGodotPropertyList()
	{
		return new List<PropertyInfo>
		{
			new PropertyInfo(Variant.Type.String, PropertyName.EEName, PropertyHint.None, "", PropertyUsageFlags.Default | PropertyUsageFlags.ScriptVariable, exported: true),
			new PropertyInfo(Variant.Type.Array, PropertyName.possibleStrings, PropertyHint.TypeString, "4/0:", PropertyUsageFlags.Default | PropertyUsageFlags.ScriptVariable, exported: true)
		};
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override void SaveGodotObjectData(GodotSerializationInfo info)
	{
		base.SaveGodotObjectData(info);
		info.AddProperty(PropertyName.EEName, Variant.From(in EEName));
		info.AddProperty(PropertyName.possibleStrings, Variant.CreateFrom(possibleStrings));
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override void RestoreGodotObjectData(GodotSerializationInfo info)
	{
		base.RestoreGodotObjectData(info);
		if (info.TryGetProperty(PropertyName.EEName, out var value))
		{
			EEName = value.As<string>();
		}
		if (info.TryGetProperty(PropertyName.possibleStrings, out var value2))
		{
			possibleStrings = value2.AsGodotArray<string>();
		}
	}
}
