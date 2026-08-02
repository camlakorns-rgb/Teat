using System.Collections.Generic;
using System.ComponentModel;
using Godot;
using Godot.Bridge;
using Godot.Collections;
using Godot.NativeInterop;

[GlobalClass]
[Tool]
[ScriptPath("res://Scripts/SubMenus/GalleryMenu/GalleryPieceDataRes.cs")]
public class GalleryPieceDataRes : Resource
{
	public new class MethodName : Resource.MethodName
	{
	}

	public new class PropertyName : Resource.PropertyName
	{
		public static readonly StringName icon = "icon";

		public static readonly StringName requiredKey = "requiredKey";

		public static readonly StringName pieceDescriptor = "pieceDescriptor";

		public static readonly StringName pieceTexture = "pieceTexture";

		public static readonly StringName pieceFrames = "pieceFrames";

		public static readonly StringName altPieceDescriptor = "altPieceDescriptor";

		public static readonly StringName altPieceTexture = "altPieceTexture";

		public static readonly StringName altpieceFrames = "altpieceFrames";

		public static readonly StringName taggedKinks = "taggedKinks";
	}

	public new class SignalName : Resource.SignalName
	{
	}

	[Export(PropertyHint.None, "")]
	public Array<SaveHandler.Kinks> taggedKinks = new Array<SaveHandler.Kinks>();

	[ExportGroup("Core Properties", "")]
	[Export(PropertyHint.None, "")]
	public Texture2D icon { get; set; }

	[Export(PropertyHint.None, "")]
	public string requiredKey { get; set; } = "NO KEY";


	[Export(PropertyHint.None, "")]
	public string pieceDescriptor { get; set; } = "EMPTY DESCRIPTOR";


	[Export(PropertyHint.None, "")]
	public Texture2D pieceTexture { get; set; }

	[Export(PropertyHint.None, "")]
	public SpriteFrames pieceFrames { get; set; }

	[Export(PropertyHint.None, "")]
	public string altPieceDescriptor { get; set; } = "EMPTY DESCRIPTOR";


	[Export(PropertyHint.None, "")]
	public Texture2D altPieceTexture { get; set; }

	[Export(PropertyHint.None, "")]
	public SpriteFrames altpieceFrames { get; set; }

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override bool SetGodotClassPropertyValue(in godot_string_name name, in godot_variant value)
	{
		if (name == PropertyName.icon)
		{
			icon = VariantUtils.ConvertTo<Texture2D>(in value);
			return true;
		}
		if (name == PropertyName.requiredKey)
		{
			requiredKey = VariantUtils.ConvertTo<string>(in value);
			return true;
		}
		if (name == PropertyName.pieceDescriptor)
		{
			pieceDescriptor = VariantUtils.ConvertTo<string>(in value);
			return true;
		}
		if (name == PropertyName.pieceTexture)
		{
			pieceTexture = VariantUtils.ConvertTo<Texture2D>(in value);
			return true;
		}
		if (name == PropertyName.pieceFrames)
		{
			pieceFrames = VariantUtils.ConvertTo<SpriteFrames>(in value);
			return true;
		}
		if (name == PropertyName.altPieceDescriptor)
		{
			altPieceDescriptor = VariantUtils.ConvertTo<string>(in value);
			return true;
		}
		if (name == PropertyName.altPieceTexture)
		{
			altPieceTexture = VariantUtils.ConvertTo<Texture2D>(in value);
			return true;
		}
		if (name == PropertyName.altpieceFrames)
		{
			altpieceFrames = VariantUtils.ConvertTo<SpriteFrames>(in value);
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
		Texture2D from;
		if (name == PropertyName.icon)
		{
			from = icon;
			value = VariantUtils.CreateFrom(in from);
			return true;
		}
		string from2;
		if (name == PropertyName.requiredKey)
		{
			from2 = requiredKey;
			value = VariantUtils.CreateFrom(in from2);
			return true;
		}
		if (name == PropertyName.pieceDescriptor)
		{
			from2 = pieceDescriptor;
			value = VariantUtils.CreateFrom(in from2);
			return true;
		}
		if (name == PropertyName.pieceTexture)
		{
			from = pieceTexture;
			value = VariantUtils.CreateFrom(in from);
			return true;
		}
		SpriteFrames from3;
		if (name == PropertyName.pieceFrames)
		{
			from3 = pieceFrames;
			value = VariantUtils.CreateFrom(in from3);
			return true;
		}
		if (name == PropertyName.altPieceDescriptor)
		{
			from2 = altPieceDescriptor;
			value = VariantUtils.CreateFrom(in from2);
			return true;
		}
		if (name == PropertyName.altPieceTexture)
		{
			from = altPieceTexture;
			value = VariantUtils.CreateFrom(in from);
			return true;
		}
		if (name == PropertyName.altpieceFrames)
		{
			from3 = altpieceFrames;
			value = VariantUtils.CreateFrom(in from3);
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
			new PropertyInfo(Variant.Type.Nil, "Core Properties", PropertyHint.None, "", PropertyUsageFlags.Group, exported: true),
			new PropertyInfo(Variant.Type.Object, PropertyName.icon, PropertyHint.ResourceType, "Texture2D", PropertyUsageFlags.Default | PropertyUsageFlags.ScriptVariable, exported: true),
			new PropertyInfo(Variant.Type.String, PropertyName.requiredKey, PropertyHint.None, "", PropertyUsageFlags.Default | PropertyUsageFlags.ScriptVariable, exported: true),
			new PropertyInfo(Variant.Type.String, PropertyName.pieceDescriptor, PropertyHint.None, "", PropertyUsageFlags.Default | PropertyUsageFlags.ScriptVariable, exported: true),
			new PropertyInfo(Variant.Type.Object, PropertyName.pieceTexture, PropertyHint.ResourceType, "Texture2D", PropertyUsageFlags.Default | PropertyUsageFlags.ScriptVariable, exported: true),
			new PropertyInfo(Variant.Type.Object, PropertyName.pieceFrames, PropertyHint.ResourceType, "SpriteFrames", PropertyUsageFlags.Default | PropertyUsageFlags.ScriptVariable, exported: true),
			new PropertyInfo(Variant.Type.Array, PropertyName.taggedKinks, PropertyHint.TypeString, "2/2:UNTYPED,CUCKING", PropertyUsageFlags.Default | PropertyUsageFlags.ScriptVariable, exported: true),
			new PropertyInfo(Variant.Type.String, PropertyName.altPieceDescriptor, PropertyHint.None, "", PropertyUsageFlags.Default | PropertyUsageFlags.ScriptVariable, exported: true),
			new PropertyInfo(Variant.Type.Object, PropertyName.altPieceTexture, PropertyHint.ResourceType, "Texture2D", PropertyUsageFlags.Default | PropertyUsageFlags.ScriptVariable, exported: true),
			new PropertyInfo(Variant.Type.Object, PropertyName.altpieceFrames, PropertyHint.ResourceType, "SpriteFrames", PropertyUsageFlags.Default | PropertyUsageFlags.ScriptVariable, exported: true)
		};
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override void SaveGodotObjectData(GodotSerializationInfo info)
	{
		base.SaveGodotObjectData(info);
		StringName name = PropertyName.icon;
		Texture2D from = icon;
		info.AddProperty(name, Variant.From(in from));
		StringName name2 = PropertyName.requiredKey;
		string from2 = requiredKey;
		info.AddProperty(name2, Variant.From(in from2));
		StringName name3 = PropertyName.pieceDescriptor;
		from2 = pieceDescriptor;
		info.AddProperty(name3, Variant.From(in from2));
		StringName name4 = PropertyName.pieceTexture;
		from = pieceTexture;
		info.AddProperty(name4, Variant.From(in from));
		StringName name5 = PropertyName.pieceFrames;
		SpriteFrames from3 = pieceFrames;
		info.AddProperty(name5, Variant.From(in from3));
		StringName name6 = PropertyName.altPieceDescriptor;
		from2 = altPieceDescriptor;
		info.AddProperty(name6, Variant.From(in from2));
		StringName name7 = PropertyName.altPieceTexture;
		from = altPieceTexture;
		info.AddProperty(name7, Variant.From(in from));
		StringName name8 = PropertyName.altpieceFrames;
		from3 = altpieceFrames;
		info.AddProperty(name8, Variant.From(in from3));
		info.AddProperty(PropertyName.taggedKinks, Variant.CreateFrom(taggedKinks));
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override void RestoreGodotObjectData(GodotSerializationInfo info)
	{
		base.RestoreGodotObjectData(info);
		if (info.TryGetProperty(PropertyName.icon, out var value))
		{
			icon = value.As<Texture2D>();
		}
		if (info.TryGetProperty(PropertyName.requiredKey, out var value2))
		{
			requiredKey = value2.As<string>();
		}
		if (info.TryGetProperty(PropertyName.pieceDescriptor, out var value3))
		{
			pieceDescriptor = value3.As<string>();
		}
		if (info.TryGetProperty(PropertyName.pieceTexture, out var value4))
		{
			pieceTexture = value4.As<Texture2D>();
		}
		if (info.TryGetProperty(PropertyName.pieceFrames, out var value5))
		{
			pieceFrames = value5.As<SpriteFrames>();
		}
		if (info.TryGetProperty(PropertyName.altPieceDescriptor, out var value6))
		{
			altPieceDescriptor = value6.As<string>();
		}
		if (info.TryGetProperty(PropertyName.altPieceTexture, out var value7))
		{
			altPieceTexture = value7.As<Texture2D>();
		}
		if (info.TryGetProperty(PropertyName.altpieceFrames, out var value8))
		{
			altpieceFrames = value8.As<SpriteFrames>();
		}
		if (info.TryGetProperty(PropertyName.taggedKinks, out var value9))
		{
			taggedKinks = value9.AsGodotArray<SaveHandler.Kinks>();
		}
	}
}
