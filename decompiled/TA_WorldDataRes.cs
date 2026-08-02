using System.Collections.Generic;
using System.ComponentModel;
using Godot;
using Godot.Bridge;
using Godot.Collections;
using Godot.NativeInterop;

[GlobalClass]
[Tool]
[ScriptPath("res://Scripts/SubMenus/TerminalMenu/TerminalAdventure/TA_WorldDataRes.cs")]
public class TA_WorldDataRes : Resource
{
	public new class MethodName : Resource.MethodName
	{
		public static readonly StringName generateItemID = "generateItemID";
	}

	public new class PropertyName : Resource.PropertyName
	{
		public static readonly StringName itemID = "itemID";

		public static readonly StringName Name = "Name";

		public static readonly StringName _itemID = "_itemID";

		public static readonly StringName _name = "_name";

		public static readonly StringName StartRoom = "StartRoom";

		public static readonly StringName IntroText = "IntroText";

		public static readonly StringName mapData = "mapData";

		public static readonly StringName Exits = "Exits";
	}

	public new class SignalName : Resource.SignalName
	{
	}

	public string _itemID;

	public string _name = "EMPTY TITLE";

	[Export(PropertyHint.None, "")]
	public TA_RoomDataRes StartRoom;

	[Export(PropertyHint.MultilineText, "")]
	public string IntroText = "";

	[Export(PropertyHint.None, "")]
	public AttachDataRes mapData;

	[Export(PropertyHint.None, "")]
	public Array<TA_ExitDataRes> Exits = new Array<TA_ExitDataRes>();

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

	public TA_WorldDataRes()
	{
		generateItemID();
	}

	private void generateItemID()
	{
		if (Name != "EMPTY TITLE" && Name != "")
		{
			_itemID = Name;
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
		if (name == PropertyName.Name)
		{
			Name = VariantUtils.ConvertTo<string>(in value);
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
		if (name == PropertyName.StartRoom)
		{
			StartRoom = VariantUtils.ConvertTo<TA_RoomDataRes>(in value);
			return true;
		}
		if (name == PropertyName.IntroText)
		{
			IntroText = VariantUtils.ConvertTo<string>(in value);
			return true;
		}
		if (name == PropertyName.mapData)
		{
			mapData = VariantUtils.ConvertTo<AttachDataRes>(in value);
			return true;
		}
		if (name == PropertyName.Exits)
		{
			Exits = VariantUtils.ConvertToArray<TA_ExitDataRes>(in value);
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
		if (name == PropertyName.Name)
		{
			from = Name;
			value = VariantUtils.CreateFrom(in from);
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
		if (name == PropertyName.StartRoom)
		{
			value = VariantUtils.CreateFrom(in StartRoom);
			return true;
		}
		if (name == PropertyName.IntroText)
		{
			value = VariantUtils.CreateFrom(in IntroText);
			return true;
		}
		if (name == PropertyName.mapData)
		{
			value = VariantUtils.CreateFrom(in mapData);
			return true;
		}
		if (name == PropertyName.Exits)
		{
			value = VariantUtils.CreateFromArray(Exits);
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
			new PropertyInfo(Variant.Type.String, PropertyName.Name, PropertyHint.None, "", PropertyUsageFlags.Default | PropertyUsageFlags.ScriptVariable, exported: true),
			new PropertyInfo(Variant.Type.Object, PropertyName.StartRoom, PropertyHint.ResourceType, "TA_RoomDataRes", PropertyUsageFlags.Default | PropertyUsageFlags.ScriptVariable, exported: true),
			new PropertyInfo(Variant.Type.String, PropertyName.IntroText, PropertyHint.MultilineText, "", PropertyUsageFlags.Default | PropertyUsageFlags.ScriptVariable, exported: true),
			new PropertyInfo(Variant.Type.Object, PropertyName.mapData, PropertyHint.ResourceType, "AttachDataRes", PropertyUsageFlags.Default | PropertyUsageFlags.ScriptVariable, exported: true),
			new PropertyInfo(Variant.Type.Array, PropertyName.Exits, PropertyHint.TypeString, "24/17:TA_ExitDataRes", PropertyUsageFlags.Default | PropertyUsageFlags.ScriptVariable, exported: true)
		};
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override void SaveGodotObjectData(GodotSerializationInfo info)
	{
		base.SaveGodotObjectData(info);
		StringName name = PropertyName.itemID;
		string from = itemID;
		info.AddProperty(name, Variant.From(in from));
		StringName name2 = PropertyName.Name;
		from = Name;
		info.AddProperty(name2, Variant.From(in from));
		info.AddProperty(PropertyName._itemID, Variant.From(in _itemID));
		info.AddProperty(PropertyName._name, Variant.From(in _name));
		info.AddProperty(PropertyName.StartRoom, Variant.From(in StartRoom));
		info.AddProperty(PropertyName.IntroText, Variant.From(in IntroText));
		info.AddProperty(PropertyName.mapData, Variant.From(in mapData));
		info.AddProperty(PropertyName.Exits, Variant.CreateFrom(Exits));
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override void RestoreGodotObjectData(GodotSerializationInfo info)
	{
		base.RestoreGodotObjectData(info);
		if (info.TryGetProperty(PropertyName.itemID, out var value))
		{
			itemID = value.As<string>();
		}
		if (info.TryGetProperty(PropertyName.Name, out var value2))
		{
			Name = value2.As<string>();
		}
		if (info.TryGetProperty(PropertyName._itemID, out var value3))
		{
			_itemID = value3.As<string>();
		}
		if (info.TryGetProperty(PropertyName._name, out var value4))
		{
			_name = value4.As<string>();
		}
		if (info.TryGetProperty(PropertyName.StartRoom, out var value5))
		{
			StartRoom = value5.As<TA_RoomDataRes>();
		}
		if (info.TryGetProperty(PropertyName.IntroText, out var value6))
		{
			IntroText = value6.As<string>();
		}
		if (info.TryGetProperty(PropertyName.mapData, out var value7))
		{
			mapData = value7.As<AttachDataRes>();
		}
		if (info.TryGetProperty(PropertyName.Exits, out var value8))
		{
			Exits = value8.AsGodotArray<TA_ExitDataRes>();
		}
	}
}
