using System.Collections.Generic;
using System.ComponentModel;
using Godot;
using Godot.Bridge;
using Godot.Collections;
using Godot.NativeInterop;

[GlobalClass]
[Tool]
[ScriptPath("res://Scripts/DataResources/DialogueDataRes.cs")]
public class DialogueDataRes : Resource
{
	public new class MethodName : Resource.MethodName
	{
	}

	public new class PropertyName : Resource.PropertyName
	{
		public static readonly StringName Dialogue = "Dialogue";

		public static readonly StringName dialogueAppeanceWeight = "dialogueAppeanceWeight";

		public static readonly StringName overrideColor = "overrideColor";

		public static readonly StringName MinimumTextSizePerLine = "MinimumTextSizePerLine";

		public static readonly StringName TypingSpeed = "TypingSpeed";

		public static readonly StringName TextHalfLife = "TextHalfLife";

		public static readonly StringName BubbleMargin = "BubbleMargin";

		public static readonly StringName fontFile = "fontFile";

		public static readonly StringName audioStreamUID = "audioStreamUID";

		public static readonly StringName speakingActorID = "speakingActorID";

		public static readonly StringName RequiredTags = "RequiredTags";

		public static readonly StringName taggedKinks = "taggedKinks";
	}

	public new class SignalName : Resource.SignalName
	{
	}

	[Export(PropertyHint.MultilineText, "")]
	public string Dialogue;

	[Export(PropertyHint.None, "")]
	public float dialogueAppeanceWeight = 25f;

	[ExportGroup("Bubble Specific", "")]
	[Export(PropertyHint.None, "")]
	public Color overrideColor = new Color("ffffff");

	[Export(PropertyHint.None, "")]
	public float MinimumTextSizePerLine = 300f;

	[Export(PropertyHint.None, "")]
	public float TypingSpeed = 30f;

	[Export(PropertyHint.None, "")]
	public Vector2 TextHalfLife = new Vector2(3f, 6f);

	[Export(PropertyHint.None, "")]
	public Vector2 BubbleMargin = new Vector2(-32f, -100f);

	[ExportGroup("Text Specific", "")]
	[Export(PropertyHint.None, "")]
	public Font fontFile;

	[Export(PropertyHint.None, "")]
	public string audioStreamUID = "uid://dptaameluxgg6";

	[ExportGroup("Actor Specific", "")]
	[Export(PropertyHint.None, "")]
	public string speakingActorID;

	[ExportGroup("Tag Specific", "")]
	[Export(PropertyHint.None, "")]
	public Array<TagDataRes> RequiredTags = new Array<TagDataRes>();

	[ExportGroup("Kink Settings", "")]
	[Export(PropertyHint.None, "")]
	public Array<SaveHandler.Kinks> taggedKinks = new Array<SaveHandler.Kinks>();

	public DialogueDataRes()
	{
	}

	public DialogueDataRes(string _Dialogue, string _target, Color _color)
	{
		Dialogue = _Dialogue;
		speakingActorID = _target;
		overrideColor = _color;
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override bool SetGodotClassPropertyValue(in godot_string_name name, in godot_variant value)
	{
		if (name == PropertyName.Dialogue)
		{
			Dialogue = VariantUtils.ConvertTo<string>(in value);
			return true;
		}
		if (name == PropertyName.dialogueAppeanceWeight)
		{
			dialogueAppeanceWeight = VariantUtils.ConvertTo<float>(in value);
			return true;
		}
		if (name == PropertyName.overrideColor)
		{
			overrideColor = VariantUtils.ConvertTo<Color>(in value);
			return true;
		}
		if (name == PropertyName.MinimumTextSizePerLine)
		{
			MinimumTextSizePerLine = VariantUtils.ConvertTo<float>(in value);
			return true;
		}
		if (name == PropertyName.TypingSpeed)
		{
			TypingSpeed = VariantUtils.ConvertTo<float>(in value);
			return true;
		}
		if (name == PropertyName.TextHalfLife)
		{
			TextHalfLife = VariantUtils.ConvertTo<Vector2>(in value);
			return true;
		}
		if (name == PropertyName.BubbleMargin)
		{
			BubbleMargin = VariantUtils.ConvertTo<Vector2>(in value);
			return true;
		}
		if (name == PropertyName.fontFile)
		{
			fontFile = VariantUtils.ConvertTo<Font>(in value);
			return true;
		}
		if (name == PropertyName.audioStreamUID)
		{
			audioStreamUID = VariantUtils.ConvertTo<string>(in value);
			return true;
		}
		if (name == PropertyName.speakingActorID)
		{
			speakingActorID = VariantUtils.ConvertTo<string>(in value);
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
		if (name == PropertyName.Dialogue)
		{
			value = VariantUtils.CreateFrom(in Dialogue);
			return true;
		}
		if (name == PropertyName.dialogueAppeanceWeight)
		{
			value = VariantUtils.CreateFrom(in dialogueAppeanceWeight);
			return true;
		}
		if (name == PropertyName.overrideColor)
		{
			value = VariantUtils.CreateFrom(in overrideColor);
			return true;
		}
		if (name == PropertyName.MinimumTextSizePerLine)
		{
			value = VariantUtils.CreateFrom(in MinimumTextSizePerLine);
			return true;
		}
		if (name == PropertyName.TypingSpeed)
		{
			value = VariantUtils.CreateFrom(in TypingSpeed);
			return true;
		}
		if (name == PropertyName.TextHalfLife)
		{
			value = VariantUtils.CreateFrom(in TextHalfLife);
			return true;
		}
		if (name == PropertyName.BubbleMargin)
		{
			value = VariantUtils.CreateFrom(in BubbleMargin);
			return true;
		}
		if (name == PropertyName.fontFile)
		{
			value = VariantUtils.CreateFrom(in fontFile);
			return true;
		}
		if (name == PropertyName.audioStreamUID)
		{
			value = VariantUtils.CreateFrom(in audioStreamUID);
			return true;
		}
		if (name == PropertyName.speakingActorID)
		{
			value = VariantUtils.CreateFrom(in speakingActorID);
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
			new PropertyInfo(Variant.Type.String, PropertyName.Dialogue, PropertyHint.MultilineText, "", PropertyUsageFlags.Default | PropertyUsageFlags.ScriptVariable, exported: true),
			new PropertyInfo(Variant.Type.Float, PropertyName.dialogueAppeanceWeight, PropertyHint.None, "", PropertyUsageFlags.Default | PropertyUsageFlags.ScriptVariable, exported: true),
			new PropertyInfo(Variant.Type.Nil, "Bubble Specific", PropertyHint.None, "", PropertyUsageFlags.Group, exported: true),
			new PropertyInfo(Variant.Type.Color, PropertyName.overrideColor, PropertyHint.None, "", PropertyUsageFlags.Default | PropertyUsageFlags.ScriptVariable, exported: true),
			new PropertyInfo(Variant.Type.Float, PropertyName.MinimumTextSizePerLine, PropertyHint.None, "", PropertyUsageFlags.Default | PropertyUsageFlags.ScriptVariable, exported: true),
			new PropertyInfo(Variant.Type.Float, PropertyName.TypingSpeed, PropertyHint.None, "", PropertyUsageFlags.Default | PropertyUsageFlags.ScriptVariable, exported: true),
			new PropertyInfo(Variant.Type.Vector2, PropertyName.TextHalfLife, PropertyHint.None, "", PropertyUsageFlags.Default | PropertyUsageFlags.ScriptVariable, exported: true),
			new PropertyInfo(Variant.Type.Vector2, PropertyName.BubbleMargin, PropertyHint.None, "", PropertyUsageFlags.Default | PropertyUsageFlags.ScriptVariable, exported: true),
			new PropertyInfo(Variant.Type.Nil, "Text Specific", PropertyHint.None, "", PropertyUsageFlags.Group, exported: true),
			new PropertyInfo(Variant.Type.Object, PropertyName.fontFile, PropertyHint.ResourceType, "Font", PropertyUsageFlags.Default | PropertyUsageFlags.ScriptVariable, exported: true),
			new PropertyInfo(Variant.Type.String, PropertyName.audioStreamUID, PropertyHint.None, "", PropertyUsageFlags.Default | PropertyUsageFlags.ScriptVariable, exported: true),
			new PropertyInfo(Variant.Type.Nil, "Actor Specific", PropertyHint.None, "", PropertyUsageFlags.Group, exported: true),
			new PropertyInfo(Variant.Type.String, PropertyName.speakingActorID, PropertyHint.None, "", PropertyUsageFlags.Default | PropertyUsageFlags.ScriptVariable, exported: true),
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
		info.AddProperty(PropertyName.Dialogue, Variant.From(in Dialogue));
		info.AddProperty(PropertyName.dialogueAppeanceWeight, Variant.From(in dialogueAppeanceWeight));
		info.AddProperty(PropertyName.overrideColor, Variant.From(in overrideColor));
		info.AddProperty(PropertyName.MinimumTextSizePerLine, Variant.From(in MinimumTextSizePerLine));
		info.AddProperty(PropertyName.TypingSpeed, Variant.From(in TypingSpeed));
		info.AddProperty(PropertyName.TextHalfLife, Variant.From(in TextHalfLife));
		info.AddProperty(PropertyName.BubbleMargin, Variant.From(in BubbleMargin));
		info.AddProperty(PropertyName.fontFile, Variant.From(in fontFile));
		info.AddProperty(PropertyName.audioStreamUID, Variant.From(in audioStreamUID));
		info.AddProperty(PropertyName.speakingActorID, Variant.From(in speakingActorID));
		info.AddProperty(PropertyName.RequiredTags, Variant.CreateFrom(RequiredTags));
		info.AddProperty(PropertyName.taggedKinks, Variant.CreateFrom(taggedKinks));
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override void RestoreGodotObjectData(GodotSerializationInfo info)
	{
		base.RestoreGodotObjectData(info);
		if (info.TryGetProperty(PropertyName.Dialogue, out var value))
		{
			Dialogue = value.As<string>();
		}
		if (info.TryGetProperty(PropertyName.dialogueAppeanceWeight, out var value2))
		{
			dialogueAppeanceWeight = value2.As<float>();
		}
		if (info.TryGetProperty(PropertyName.overrideColor, out var value3))
		{
			overrideColor = value3.As<Color>();
		}
		if (info.TryGetProperty(PropertyName.MinimumTextSizePerLine, out var value4))
		{
			MinimumTextSizePerLine = value4.As<float>();
		}
		if (info.TryGetProperty(PropertyName.TypingSpeed, out var value5))
		{
			TypingSpeed = value5.As<float>();
		}
		if (info.TryGetProperty(PropertyName.TextHalfLife, out var value6))
		{
			TextHalfLife = value6.As<Vector2>();
		}
		if (info.TryGetProperty(PropertyName.BubbleMargin, out var value7))
		{
			BubbleMargin = value7.As<Vector2>();
		}
		if (info.TryGetProperty(PropertyName.fontFile, out var value8))
		{
			fontFile = value8.As<Font>();
		}
		if (info.TryGetProperty(PropertyName.audioStreamUID, out var value9))
		{
			audioStreamUID = value9.As<string>();
		}
		if (info.TryGetProperty(PropertyName.speakingActorID, out var value10))
		{
			speakingActorID = value10.As<string>();
		}
		if (info.TryGetProperty(PropertyName.RequiredTags, out var value11))
		{
			RequiredTags = value11.AsGodotArray<TagDataRes>();
		}
		if (info.TryGetProperty(PropertyName.taggedKinks, out var value12))
		{
			taggedKinks = value12.AsGodotArray<SaveHandler.Kinks>();
		}
	}
}
