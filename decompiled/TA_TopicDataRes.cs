using System.Collections.Generic;
using System.ComponentModel;
using Godot;
using Godot.Bridge;
using Godot.NativeInterop;

[GlobalClass]
[ScriptPath("res://Scripts/SubMenus/TerminalMenu/TerminalAdventure/TA_TopicDataRes.cs")]
public class TA_TopicDataRes : Resource
{
	public new class MethodName : Resource.MethodName
	{
	}

	public new class PropertyName : Resource.PropertyName
	{
		public static readonly StringName Keywords = "Keywords";

		public static readonly StringName Response = "Response";

		public static readonly StringName GameOver = "GameOver";

		public static readonly StringName GiveKey = "GiveKey";

		public static readonly StringName GiveItem = "GiveItem";

		public static readonly StringName GiveItemID = "GiveItemID";

		public static readonly StringName ScenePopup = "ScenePopup";

		public static readonly StringName ScenePopupID = "ScenePopupID";
	}

	public new class SignalName : Resource.SignalName
	{
	}

	[Export(PropertyHint.None, "")]
	public string Keywords = "";

	[Export(PropertyHint.MultilineText, "")]
	public string Response = "";

	[Export(PropertyHint.None, "")]
	public bool GameOver;

	[Export(PropertyHint.None, "")]
	public string GiveKey = "";

	[Export(PropertyHint.None, "")]
	public ItemDataRes GiveItem;

	[ExportSubgroup("ItemID Override", "")]
	[Export(PropertyHint.None, "")]
	public string GiveItemID = "";

	[ExportGroup("", "")]
	[Export(PropertyHint.None, "")]
	public AttachDataRes ScenePopup;

	[ExportSubgroup("SceneID Override", "")]
	[Export(PropertyHint.None, "")]
	public string ScenePopupID = "";

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override bool SetGodotClassPropertyValue(in godot_string_name name, in godot_variant value)
	{
		if (name == PropertyName.Keywords)
		{
			Keywords = VariantUtils.ConvertTo<string>(in value);
			return true;
		}
		if (name == PropertyName.Response)
		{
			Response = VariantUtils.ConvertTo<string>(in value);
			return true;
		}
		if (name == PropertyName.GameOver)
		{
			GameOver = VariantUtils.ConvertTo<bool>(in value);
			return true;
		}
		if (name == PropertyName.GiveKey)
		{
			GiveKey = VariantUtils.ConvertTo<string>(in value);
			return true;
		}
		if (name == PropertyName.GiveItem)
		{
			GiveItem = VariantUtils.ConvertTo<ItemDataRes>(in value);
			return true;
		}
		if (name == PropertyName.GiveItemID)
		{
			GiveItemID = VariantUtils.ConvertTo<string>(in value);
			return true;
		}
		if (name == PropertyName.ScenePopup)
		{
			ScenePopup = VariantUtils.ConvertTo<AttachDataRes>(in value);
			return true;
		}
		if (name == PropertyName.ScenePopupID)
		{
			ScenePopupID = VariantUtils.ConvertTo<string>(in value);
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
		if (name == PropertyName.Response)
		{
			value = VariantUtils.CreateFrom(in Response);
			return true;
		}
		if (name == PropertyName.GameOver)
		{
			value = VariantUtils.CreateFrom(in GameOver);
			return true;
		}
		if (name == PropertyName.GiveKey)
		{
			value = VariantUtils.CreateFrom(in GiveKey);
			return true;
		}
		if (name == PropertyName.GiveItem)
		{
			value = VariantUtils.CreateFrom(in GiveItem);
			return true;
		}
		if (name == PropertyName.GiveItemID)
		{
			value = VariantUtils.CreateFrom(in GiveItemID);
			return true;
		}
		if (name == PropertyName.ScenePopup)
		{
			value = VariantUtils.CreateFrom(in ScenePopup);
			return true;
		}
		if (name == PropertyName.ScenePopupID)
		{
			value = VariantUtils.CreateFrom(in ScenePopupID);
			return true;
		}
		return base.GetGodotClassPropertyValue(in name, out value);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	internal static List<PropertyInfo> GetGodotPropertyList()
	{
		return new List<PropertyInfo>
		{
			new PropertyInfo(Variant.Type.String, PropertyName.Keywords, PropertyHint.None, "", PropertyUsageFlags.Default | PropertyUsageFlags.ScriptVariable, exported: true),
			new PropertyInfo(Variant.Type.String, PropertyName.Response, PropertyHint.MultilineText, "", PropertyUsageFlags.Default | PropertyUsageFlags.ScriptVariable, exported: true),
			new PropertyInfo(Variant.Type.Bool, PropertyName.GameOver, PropertyHint.None, "", PropertyUsageFlags.Default | PropertyUsageFlags.ScriptVariable, exported: true),
			new PropertyInfo(Variant.Type.String, PropertyName.GiveKey, PropertyHint.None, "", PropertyUsageFlags.Default | PropertyUsageFlags.ScriptVariable, exported: true),
			new PropertyInfo(Variant.Type.Object, PropertyName.GiveItem, PropertyHint.ResourceType, "ItemDataRes", PropertyUsageFlags.Default | PropertyUsageFlags.ScriptVariable, exported: true),
			new PropertyInfo(Variant.Type.Nil, "ItemID Override", PropertyHint.None, "", PropertyUsageFlags.Subgroup, exported: true),
			new PropertyInfo(Variant.Type.String, PropertyName.GiveItemID, PropertyHint.None, "", PropertyUsageFlags.Default | PropertyUsageFlags.ScriptVariable, exported: true),
			new PropertyInfo(Variant.Type.Nil, "", PropertyHint.None, "", PropertyUsageFlags.Group, exported: true),
			new PropertyInfo(Variant.Type.Object, PropertyName.ScenePopup, PropertyHint.ResourceType, "AttachDataRes", PropertyUsageFlags.Default | PropertyUsageFlags.ScriptVariable, exported: true),
			new PropertyInfo(Variant.Type.Nil, "SceneID Override", PropertyHint.None, "", PropertyUsageFlags.Subgroup, exported: true),
			new PropertyInfo(Variant.Type.String, PropertyName.ScenePopupID, PropertyHint.None, "", PropertyUsageFlags.Default | PropertyUsageFlags.ScriptVariable, exported: true)
		};
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override void SaveGodotObjectData(GodotSerializationInfo info)
	{
		base.SaveGodotObjectData(info);
		info.AddProperty(PropertyName.Keywords, Variant.From(in Keywords));
		info.AddProperty(PropertyName.Response, Variant.From(in Response));
		info.AddProperty(PropertyName.GameOver, Variant.From(in GameOver));
		info.AddProperty(PropertyName.GiveKey, Variant.From(in GiveKey));
		info.AddProperty(PropertyName.GiveItem, Variant.From(in GiveItem));
		info.AddProperty(PropertyName.GiveItemID, Variant.From(in GiveItemID));
		info.AddProperty(PropertyName.ScenePopup, Variant.From(in ScenePopup));
		info.AddProperty(PropertyName.ScenePopupID, Variant.From(in ScenePopupID));
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override void RestoreGodotObjectData(GodotSerializationInfo info)
	{
		base.RestoreGodotObjectData(info);
		if (info.TryGetProperty(PropertyName.Keywords, out var value))
		{
			Keywords = value.As<string>();
		}
		if (info.TryGetProperty(PropertyName.Response, out var value2))
		{
			Response = value2.As<string>();
		}
		if (info.TryGetProperty(PropertyName.GameOver, out var value3))
		{
			GameOver = value3.As<bool>();
		}
		if (info.TryGetProperty(PropertyName.GiveKey, out var value4))
		{
			GiveKey = value4.As<string>();
		}
		if (info.TryGetProperty(PropertyName.GiveItem, out var value5))
		{
			GiveItem = value5.As<ItemDataRes>();
		}
		if (info.TryGetProperty(PropertyName.GiveItemID, out var value6))
		{
			GiveItemID = value6.As<string>();
		}
		if (info.TryGetProperty(PropertyName.ScenePopup, out var value7))
		{
			ScenePopup = value7.As<AttachDataRes>();
		}
		if (info.TryGetProperty(PropertyName.ScenePopupID, out var value8))
		{
			ScenePopupID = value8.As<string>();
		}
	}
}
