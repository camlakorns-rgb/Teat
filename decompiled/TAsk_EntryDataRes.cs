using System.Collections.Generic;
using System.ComponentModel;
using Godot;
using Godot.Bridge;
using Godot.Collections;
using Godot.NativeInterop;

[GlobalClass]
[Tool]
[ScriptPath("res://Scripts/SubMenus/TerminalMenu/TerminalAsk/TAsk_EntryDataRes.cs")]
public class TAsk_EntryDataRes : Resource
{
	public enum CodeTies
	{
		UNTYPED,
		QUIT
	}

	public new class MethodName : Resource.MethodName
	{
	}

	public new class PropertyName : Resource.PropertyName
	{
		public static readonly StringName Keywords = "Keywords";

		public static readonly StringName DisplayHint = "DisplayHint";

		public static readonly StringName Convo = "Convo";

		public static readonly StringName AltConvo = "AltConvo";

		public static readonly StringName dialogueTask = "dialogueTask";
	}

	public new class SignalName : Resource.SignalName
	{
	}

	[Export(PropertyHint.None, "Pipe-separated match keywords. Example: handler|fen|master")]
	public string Keywords = "";

	[Export(PropertyHint.None, "If left blank the entry is silently matchable but won't appear in HELP.")]
	public string DisplayHint = "";

	[Export(PropertyHint.None, "")]
	public ConvoDataRes Convo;

	[Export(PropertyHint.None, "")]
	public Godot.Collections.Dictionary<string, ConvoDataRes> AltConvo = new Godot.Collections.Dictionary<string, ConvoDataRes>();

	[Export(PropertyHint.None, "")]
	public CodeTies dialogueTask;

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override bool SetGodotClassPropertyValue(in godot_string_name name, in godot_variant value)
	{
		if (name == PropertyName.Keywords)
		{
			Keywords = VariantUtils.ConvertTo<string>(in value);
			return true;
		}
		if (name == PropertyName.DisplayHint)
		{
			DisplayHint = VariantUtils.ConvertTo<string>(in value);
			return true;
		}
		if (name == PropertyName.Convo)
		{
			Convo = VariantUtils.ConvertTo<ConvoDataRes>(in value);
			return true;
		}
		if (name == PropertyName.AltConvo)
		{
			AltConvo = VariantUtils.ConvertToDictionary<string, ConvoDataRes>(in value);
			return true;
		}
		if (name == PropertyName.dialogueTask)
		{
			dialogueTask = VariantUtils.ConvertTo<CodeTies>(in value);
			return true;
		}
		return base.SetGodotClassPropertyValue(in name, in value);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override bool GetGodotClassPropertyValue(in godot_string_name name, out godot_variant value)
	{
		if (name == PropertyName.Keywords)
		{
			value = VariantUtils.CreateFrom(in Keywords);
			return true;
		}
		if (name == PropertyName.DisplayHint)
		{
			value = VariantUtils.CreateFrom(in DisplayHint);
			return true;
		}
		if (name == PropertyName.Convo)
		{
			value = VariantUtils.CreateFrom(in Convo);
			return true;
		}
		if (name == PropertyName.AltConvo)
		{
			value = VariantUtils.CreateFromDictionary(AltConvo);
			return true;
		}
		if (name == PropertyName.dialogueTask)
		{
			value = VariantUtils.CreateFrom(in dialogueTask);
			return true;
		}
		return base.GetGodotClassPropertyValue(in name, out value);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	internal static List<PropertyInfo> GetGodotPropertyList()
	{
		return new List<PropertyInfo>
		{
			new PropertyInfo(Variant.Type.String, PropertyName.Keywords, PropertyHint.None, "Pipe-separated match keywords. Example: handler|fen|master", PropertyUsageFlags.Default | PropertyUsageFlags.ScriptVariable, exported: true),
			new PropertyInfo(Variant.Type.String, PropertyName.DisplayHint, PropertyHint.None, "If left blank the entry is silently matchable but won't appear in HELP.", PropertyUsageFlags.Default | PropertyUsageFlags.ScriptVariable, exported: true),
			new PropertyInfo(Variant.Type.Object, PropertyName.Convo, PropertyHint.ResourceType, "ConvoDataRes", PropertyUsageFlags.Default | PropertyUsageFlags.ScriptVariable, exported: true),
			new PropertyInfo(Variant.Type.Dictionary, PropertyName.AltConvo, PropertyHint.TypeString, "4/0:;24/17:ConvoDataRes", PropertyUsageFlags.Default | PropertyUsageFlags.ScriptVariable, exported: true),
			new PropertyInfo(Variant.Type.Int, PropertyName.dialogueTask, PropertyHint.Enum, "UNTYPED,QUIT", PropertyUsageFlags.Default | PropertyUsageFlags.ScriptVariable, exported: true)
		};
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override void SaveGodotObjectData(GodotSerializationInfo info)
	{
		base.SaveGodotObjectData(info);
		info.AddProperty(PropertyName.Keywords, Variant.From(in Keywords));
		info.AddProperty(PropertyName.DisplayHint, Variant.From(in DisplayHint));
		info.AddProperty(PropertyName.Convo, Variant.From(in Convo));
		info.AddProperty(PropertyName.AltConvo, Variant.CreateFrom(AltConvo));
		info.AddProperty(PropertyName.dialogueTask, Variant.From(in dialogueTask));
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override void RestoreGodotObjectData(GodotSerializationInfo info)
	{
		base.RestoreGodotObjectData(info);
		if (info.TryGetProperty(PropertyName.Keywords, out var value))
		{
			Keywords = value.As<string>();
		}
		if (info.TryGetProperty(PropertyName.DisplayHint, out var value2))
		{
			DisplayHint = value2.As<string>();
		}
		if (info.TryGetProperty(PropertyName.Convo, out var value3))
		{
			Convo = value3.As<ConvoDataRes>();
		}
		if (info.TryGetProperty(PropertyName.AltConvo, out var value4))
		{
			AltConvo = value4.AsGodotDictionary<string, ConvoDataRes>();
		}
		if (info.TryGetProperty(PropertyName.dialogueTask, out var value5))
		{
			dialogueTask = value5.As<CodeTies>();
		}
	}
}
