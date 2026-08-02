using System.Collections.Generic;
using System.ComponentModel;
using Godot;
using Godot.Bridge;
using Godot.Collections;
using Godot.NativeInterop;

[GlobalClass]
[Tool]
[ScriptPath("res://Scripts/SubMenus/TerminalMenu/TerminalAsk/TAsk_AskDataRes.cs")]
public class TAsk_AskDataRes : Resource
{
	public new class MethodName : Resource.MethodName
	{
	}

	public new class PropertyName : Resource.PropertyName
	{
		public static readonly StringName askingCharacter = "askingCharacter";

		public static readonly StringName Entries = "Entries";

		public static readonly StringName NoMatchDialogue = "NoMatchDialogue";

		public static readonly StringName EnterText = "EnterText";

		public static readonly StringName ExitText = "ExitText";
	}

	public new class SignalName : Resource.SignalName
	{
	}

	[Export(PropertyHint.None, "")]
	public CharacterInfoDataRes askingCharacter;

	[Export(PropertyHint.None, "")]
	public Array<TAsk_EntryDataRes> Entries = new Array<TAsk_EntryDataRes>();

	[Export(PropertyHint.None, "")]
	public Array<DialogueDataRes> NoMatchDialogue = new Array<DialogueDataRes>();

	[ExportGroup("Intro Outro Text", "")]
	[Export(PropertyHint.MultilineText, "")]
	public string EnterText = "  [color=cyan]-- ASK MODE -- Ask what you want. Type QUIT to leave. Type HELP to list topics. --[/color]";

	[Export(PropertyHint.MultilineText, "")]
	public string ExitText = "  [color=cyan]-- LEFT ASK MODE --[/color]";

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override bool SetGodotClassPropertyValue(in godot_string_name name, in godot_variant value)
	{
		if (name == PropertyName.askingCharacter)
		{
			askingCharacter = VariantUtils.ConvertTo<CharacterInfoDataRes>(in value);
			return true;
		}
		if (name == PropertyName.Entries)
		{
			Entries = VariantUtils.ConvertToArray<TAsk_EntryDataRes>(in value);
			return true;
		}
		if (name == PropertyName.NoMatchDialogue)
		{
			NoMatchDialogue = VariantUtils.ConvertToArray<DialogueDataRes>(in value);
			return true;
		}
		if (name == PropertyName.EnterText)
		{
			EnterText = VariantUtils.ConvertTo<string>(in value);
			return true;
		}
		if (name == PropertyName.ExitText)
		{
			ExitText = VariantUtils.ConvertTo<string>(in value);
			return true;
		}
		return base.SetGodotClassPropertyValue(in name, in value);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override bool GetGodotClassPropertyValue(in godot_string_name name, out godot_variant value)
	{
		if (name == PropertyName.askingCharacter)
		{
			value = VariantUtils.CreateFrom(in askingCharacter);
			return true;
		}
		if (name == PropertyName.Entries)
		{
			value = VariantUtils.CreateFromArray(Entries);
			return true;
		}
		if (name == PropertyName.NoMatchDialogue)
		{
			value = VariantUtils.CreateFromArray(NoMatchDialogue);
			return true;
		}
		if (name == PropertyName.EnterText)
		{
			value = VariantUtils.CreateFrom(in EnterText);
			return true;
		}
		if (name == PropertyName.ExitText)
		{
			value = VariantUtils.CreateFrom(in ExitText);
			return true;
		}
		return base.GetGodotClassPropertyValue(in name, out value);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	internal static List<PropertyInfo> GetGodotPropertyList()
	{
		return new List<PropertyInfo>
		{
			new PropertyInfo(Variant.Type.Object, PropertyName.askingCharacter, PropertyHint.ResourceType, "CharacterInfoDataRes", PropertyUsageFlags.Default | PropertyUsageFlags.ScriptVariable, exported: true),
			new PropertyInfo(Variant.Type.Array, PropertyName.Entries, PropertyHint.TypeString, "24/17:TAsk_EntryDataRes", PropertyUsageFlags.Default | PropertyUsageFlags.ScriptVariable, exported: true),
			new PropertyInfo(Variant.Type.Array, PropertyName.NoMatchDialogue, PropertyHint.TypeString, "24/17:DialogueDataRes", PropertyUsageFlags.Default | PropertyUsageFlags.ScriptVariable, exported: true),
			new PropertyInfo(Variant.Type.Nil, "Intro Outro Text", PropertyHint.None, "", PropertyUsageFlags.Group, exported: true),
			new PropertyInfo(Variant.Type.String, PropertyName.EnterText, PropertyHint.MultilineText, "", PropertyUsageFlags.Default | PropertyUsageFlags.ScriptVariable, exported: true),
			new PropertyInfo(Variant.Type.String, PropertyName.ExitText, PropertyHint.MultilineText, "", PropertyUsageFlags.Default | PropertyUsageFlags.ScriptVariable, exported: true)
		};
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override void SaveGodotObjectData(GodotSerializationInfo info)
	{
		base.SaveGodotObjectData(info);
		info.AddProperty(PropertyName.askingCharacter, Variant.From(in askingCharacter));
		info.AddProperty(PropertyName.Entries, Variant.CreateFrom(Entries));
		info.AddProperty(PropertyName.NoMatchDialogue, Variant.CreateFrom(NoMatchDialogue));
		info.AddProperty(PropertyName.EnterText, Variant.From(in EnterText));
		info.AddProperty(PropertyName.ExitText, Variant.From(in ExitText));
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override void RestoreGodotObjectData(GodotSerializationInfo info)
	{
		base.RestoreGodotObjectData(info);
		if (info.TryGetProperty(PropertyName.askingCharacter, out var value))
		{
			askingCharacter = value.As<CharacterInfoDataRes>();
		}
		if (info.TryGetProperty(PropertyName.Entries, out var value2))
		{
			Entries = value2.AsGodotArray<TAsk_EntryDataRes>();
		}
		if (info.TryGetProperty(PropertyName.NoMatchDialogue, out var value3))
		{
			NoMatchDialogue = value3.AsGodotArray<DialogueDataRes>();
		}
		if (info.TryGetProperty(PropertyName.EnterText, out var value4))
		{
			EnterText = value4.As<string>();
		}
		if (info.TryGetProperty(PropertyName.ExitText, out var value5))
		{
			ExitText = value5.As<string>();
		}
	}
}
