using System.Collections.Generic;
using System.ComponentModel;
using Godot;
using Godot.Bridge;
using Godot.Collections;
using Godot.NativeInterop;

[GlobalClass]
[ScriptPath("res://Scripts/DataResources/GalleryDataRes.cs")]
public class GalleryDataRes : Resource
{
	public new class MethodName : Resource.MethodName
	{
		public static readonly StringName generateItemID = "generateItemID";
	}

	public new class PropertyName : Resource.PropertyName
	{
		public static readonly StringName itemID = "itemID";

		public static readonly StringName icon = "icon";

		public static readonly StringName Name = "Name";

		public static readonly StringName itemDescriptor = "itemDescriptor";

		public static readonly StringName itemVariation = "itemVariation";

		public static readonly StringName galleryOrder = "galleryOrder";

		public static readonly StringName itemCGs = "itemCGs";

		public static readonly StringName _itemID = "_itemID";

		public static readonly StringName _name = "_name";
	}

	public new class SignalName : Resource.SignalName
	{
	}

	public string _itemID;

	public string _name = "EMPTY TITLE";

	[Export(PropertyHint.None, "")]
	public string itemID
	{
		get
		{
			generateItemID();
			return _itemID;
		}
		set
		{
			generateItemID();
		}
	}

	[ExportGroup("Core Properties", "")]
	[Export(PropertyHint.None, "")]
	public Texture2D icon { get; set; }

	[Export(PropertyHint.None, "")]
	public string Name
	{
		get
		{
			return _name;
		}
		set
		{
			_name = value;
			generateItemID();
		}
	}

	[Export(PropertyHint.None, "")]
	public string itemDescriptor { get; set; } = "EMPTY DESCRIPTOR";


	[Export(PropertyHint.None, "")]
	public int itemVariation { get; set; }

	[Export(PropertyHint.None, "")]
	public int galleryOrder { get; set; }

	[ExportGroup("Item CG Info", "")]
	[Export(PropertyHint.None, "")]
	public Array<GalleryPieceDataRes> itemCGs { get; set; }

	public GalleryDataRes()
	{
		generateItemID();
	}

	private void generateItemID()
	{
		if (Name != "EMPTY TITLE" && Name != "")
		{
			_itemID = Name.Replace(" ", "_") + "_" + itemVariation;
		}
		else
		{
			_itemID = "INVALID_ID: Please Set Name";
		}
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	internal static List<MethodInfo> GetGodotMethodList()
	{
		return new List<MethodInfo>(1)
		{
			new MethodInfo(MethodName.generateItemID, new PropertyInfo(Variant.Type.Nil, "", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false), MethodFlags.Normal, null, null)
		};
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override bool InvokeGodotClassMethod(in godot_string_name method, NativeVariantPtrArgs args, out godot_variant ret)
	{
		if (method == MethodName.generateItemID && args.Count == 0)
		{
			generateItemID();
			ret = default(godot_variant);
			return true;
		}
		return base.InvokeGodotClassMethod(in method, args, out ret);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override bool HasGodotClassMethod(in godot_string_name method)
	{
		if (method == MethodName.generateItemID)
		{
			return true;
		}
		return base.HasGodotClassMethod(in method);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override bool SetGodotClassPropertyValue(in godot_string_name name, in godot_variant value)
	{
		if (name == PropertyName.itemID)
		{
			itemID = VariantUtils.ConvertTo<string>(in value);
			return true;
		}
		if (name == PropertyName.icon)
		{
			icon = VariantUtils.ConvertTo<Texture2D>(in value);
			return true;
		}
		if (name == PropertyName.Name)
		{
			Name = VariantUtils.ConvertTo<string>(in value);
			return true;
		}
		if (name == PropertyName.itemDescriptor)
		{
			itemDescriptor = VariantUtils.ConvertTo<string>(in value);
			return true;
		}
		if (name == PropertyName.itemVariation)
		{
			itemVariation = VariantUtils.ConvertTo<int>(in value);
			return true;
		}
		if (name == PropertyName.galleryOrder)
		{
			galleryOrder = VariantUtils.ConvertTo<int>(in value);
			return true;
		}
		if (name == PropertyName.itemCGs)
		{
			itemCGs = VariantUtils.ConvertToArray<GalleryPieceDataRes>(in value);
			return true;
		}
		if (name == PropertyName._itemID)
		{
			_itemID = VariantUtils.ConvertTo<string>(in value);
			return true;
		}
		if (name == PropertyName._name)
		{
			_name = VariantUtils.ConvertTo<string>(in value);
			return true;
		}
		return base.SetGodotClassPropertyValue(in name, in value);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override bool GetGodotClassPropertyValue(in godot_string_name name, out godot_variant value)
	{
		string from;
		if (name == PropertyName.itemID)
		{
			from = itemID;
			value = VariantUtils.CreateFrom(in from);
			return true;
		}
		if (name == PropertyName.icon)
		{
			Texture2D from2 = icon;
			value = VariantUtils.CreateFrom(in from2);
			return true;
		}
		if (name == PropertyName.Name)
		{
			from = Name;
			value = VariantUtils.CreateFrom(in from);
			return true;
		}
		if (name == PropertyName.itemDescriptor)
		{
			from = itemDescriptor;
			value = VariantUtils.CreateFrom(in from);
			return true;
		}
		int from3;
		if (name == PropertyName.itemVariation)
		{
			from3 = itemVariation;
			value = VariantUtils.CreateFrom(in from3);
			return true;
		}
		if (name == PropertyName.galleryOrder)
		{
			from3 = galleryOrder;
			value = VariantUtils.CreateFrom(in from3);
			return true;
		}
		if (name == PropertyName.itemCGs)
		{
			value = VariantUtils.CreateFromArray(itemCGs);
			return true;
		}
		if (name == PropertyName._itemID)
		{
			value = VariantUtils.CreateFrom(in _itemID);
			return true;
		}
		if (name == PropertyName._name)
		{
			value = VariantUtils.CreateFrom(in _name);
			return true;
		}
		return base.GetGodotClassPropertyValue(in name, out value);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	internal static List<PropertyInfo> GetGodotPropertyList()
	{
		return new List<PropertyInfo>
		{
			new PropertyInfo(Variant.Type.String, PropertyName._itemID, PropertyHint.None, "", PropertyUsageFlags.ScriptVariable, exported: false),
			new PropertyInfo(Variant.Type.String, PropertyName._name, PropertyHint.None, "", PropertyUsageFlags.ScriptVariable, exported: false),
			new PropertyInfo(Variant.Type.String, PropertyName.itemID, PropertyHint.None, "", PropertyUsageFlags.Default | PropertyUsageFlags.ScriptVariable, exported: true),
			new PropertyInfo(Variant.Type.Nil, "Core Properties", PropertyHint.None, "", PropertyUsageFlags.Group, exported: true),
			new PropertyInfo(Variant.Type.Object, PropertyName.icon, PropertyHint.ResourceType, "Texture2D", PropertyUsageFlags.Default | PropertyUsageFlags.ScriptVariable, exported: true),
			new PropertyInfo(Variant.Type.String, PropertyName.Name, PropertyHint.None, "", PropertyUsageFlags.Default | PropertyUsageFlags.ScriptVariable, exported: true),
			new PropertyInfo(Variant.Type.String, PropertyName.itemDescriptor, PropertyHint.None, "", PropertyUsageFlags.Default | PropertyUsageFlags.ScriptVariable, exported: true),
			new PropertyInfo(Variant.Type.Int, PropertyName.itemVariation, PropertyHint.None, "", PropertyUsageFlags.Default | PropertyUsageFlags.ScriptVariable, exported: true),
			new PropertyInfo(Variant.Type.Int, PropertyName.galleryOrder, PropertyHint.None, "", PropertyUsageFlags.Default | PropertyUsageFlags.ScriptVariable, exported: true),
			new PropertyInfo(Variant.Type.Nil, "Item CG Info", PropertyHint.None, "", PropertyUsageFlags.Group, exported: true),
			new PropertyInfo(Variant.Type.Array, PropertyName.itemCGs, PropertyHint.TypeString, "24/17:GalleryPieceDataRes", PropertyUsageFlags.Default | PropertyUsageFlags.ScriptVariable, exported: true)
		};
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override void SaveGodotObjectData(GodotSerializationInfo info)
	{
		base.SaveGodotObjectData(info);
		StringName name = PropertyName.itemID;
		string from = itemID;
		info.AddProperty(name, Variant.From(in from));
		StringName name2 = PropertyName.icon;
		Texture2D from2 = icon;
		info.AddProperty(name2, Variant.From(in from2));
		StringName name3 = PropertyName.Name;
		from = Name;
		info.AddProperty(name3, Variant.From(in from));
		StringName name4 = PropertyName.itemDescriptor;
		from = itemDescriptor;
		info.AddProperty(name4, Variant.From(in from));
		StringName name5 = PropertyName.itemVariation;
		int from3 = itemVariation;
		info.AddProperty(name5, Variant.From(in from3));
		StringName name6 = PropertyName.galleryOrder;
		from3 = galleryOrder;
		info.AddProperty(name6, Variant.From(in from3));
		info.AddProperty(PropertyName.itemCGs, Variant.CreateFrom(itemCGs));
		info.AddProperty(PropertyName._itemID, Variant.From(in _itemID));
		info.AddProperty(PropertyName._name, Variant.From(in _name));
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override void RestoreGodotObjectData(GodotSerializationInfo info)
	{
		base.RestoreGodotObjectData(info);
		if (info.TryGetProperty(PropertyName.itemID, out var value))
		{
			itemID = value.As<string>();
		}
		if (info.TryGetProperty(PropertyName.icon, out var value2))
		{
			icon = value2.As<Texture2D>();
		}
		if (info.TryGetProperty(PropertyName.Name, out var value3))
		{
			Name = value3.As<string>();
		}
		if (info.TryGetProperty(PropertyName.itemDescriptor, out var value4))
		{
			itemDescriptor = value4.As<string>();
		}
		if (info.TryGetProperty(PropertyName.itemVariation, out var value5))
		{
			itemVariation = value5.As<int>();
		}
		if (info.TryGetProperty(PropertyName.galleryOrder, out var value6))
		{
			galleryOrder = value6.As<int>();
		}
		if (info.TryGetProperty(PropertyName.itemCGs, out var value7))
		{
			itemCGs = value7.AsGodotArray<GalleryPieceDataRes>();
		}
		if (info.TryGetProperty(PropertyName._itemID, out var value8))
		{
			_itemID = value8.As<string>();
		}
		if (info.TryGetProperty(PropertyName._name, out var value9))
		{
			_name = value9.As<string>();
		}
	}
}
