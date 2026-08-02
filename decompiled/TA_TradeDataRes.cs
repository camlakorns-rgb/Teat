using System.Collections.Generic;
using System.ComponentModel;
using Godot;
using Godot.Bridge;
using Godot.NativeInterop;

[GlobalClass]
[ScriptPath("res://Scripts/SubMenus/TerminalMenu/TerminalAdventure/TA_TradeDataRes.cs")]
public class TA_TradeDataRes : Resource
{
	public new class MethodName : Resource.MethodName
	{
	}

	public new class PropertyName : Resource.PropertyName
	{
		public static readonly StringName WantedItem = "WantedItem";

		public static readonly StringName TradeRewardItem = "TradeRewardItem";

		public static readonly StringName TradeRewardInfo = "TradeRewardInfo";

		public static readonly StringName TradeDialogue = "TradeDialogue";

		public static readonly StringName GiveKey = "GiveKey";

		public static readonly StringName GiveItem = "GiveItem";

		public static readonly StringName GameOver = "GameOver";

		public static readonly StringName GiveItemID = "GiveItemID";

		public static readonly StringName ScenePopup = "ScenePopup";

		public static readonly StringName ScenePopupID = "ScenePopupID";
	}

	public new class SignalName : Resource.SignalName
	{
	}

	[Export(PropertyHint.None, "")]
	public TA_ItemDataRes WantedItem;

	[Export(PropertyHint.None, "")]
	public TA_ItemDataRes TradeRewardItem;

	[Export(PropertyHint.MultilineText, "")]
	public string TradeRewardInfo = "";

	[Export(PropertyHint.MultilineText, "")]
	public string TradeDialogue = "A fair trade. Here, take {reward} in exchange for the {item}.";

	[Export(PropertyHint.None, "")]
	public string GiveKey = "";

	[Export(PropertyHint.None, "")]
	public ItemDataRes GiveItem;

	[Export(PropertyHint.None, "")]
	public bool GameOver;

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
		if (name == PropertyName.WantedItem)
		{
			WantedItem = VariantUtils.ConvertTo<TA_ItemDataRes>(in value);
			return true;
		}
		if (name == PropertyName.TradeRewardItem)
		{
			TradeRewardItem = VariantUtils.ConvertTo<TA_ItemDataRes>(in value);
			return true;
		}
		if (name == PropertyName.TradeRewardInfo)
		{
			TradeRewardInfo = VariantUtils.ConvertTo<string>(in value);
			return true;
		}
		if (name == PropertyName.TradeDialogue)
		{
			TradeDialogue = VariantUtils.ConvertTo<string>(in value);
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
		if (name == PropertyName.GameOver)
		{
			GameOver = VariantUtils.ConvertTo<bool>(in value);
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
		if (name == PropertyName.WantedItem)
		{
			value = VariantUtils.CreateFrom(in WantedItem);
			return true;
		}
		if (name == PropertyName.TradeRewardItem)
		{
			value = VariantUtils.CreateFrom(in TradeRewardItem);
			return true;
		}
		if (name == PropertyName.TradeRewardInfo)
		{
			value = VariantUtils.CreateFrom(in TradeRewardInfo);
			return true;
		}
		if (name == PropertyName.TradeDialogue)
		{
			value = VariantUtils.CreateFrom(in TradeDialogue);
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
		if (name == PropertyName.GameOver)
		{
			value = VariantUtils.CreateFrom(in GameOver);
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
			new PropertyInfo(Variant.Type.Object, PropertyName.WantedItem, PropertyHint.ResourceType, "TA_ItemDataRes", PropertyUsageFlags.Default | PropertyUsageFlags.ScriptVariable, exported: true),
			new PropertyInfo(Variant.Type.Object, PropertyName.TradeRewardItem, PropertyHint.ResourceType, "TA_ItemDataRes", PropertyUsageFlags.Default | PropertyUsageFlags.ScriptVariable, exported: true),
			new PropertyInfo(Variant.Type.String, PropertyName.TradeRewardInfo, PropertyHint.MultilineText, "", PropertyUsageFlags.Default | PropertyUsageFlags.ScriptVariable, exported: true),
			new PropertyInfo(Variant.Type.String, PropertyName.TradeDialogue, PropertyHint.MultilineText, "", PropertyUsageFlags.Default | PropertyUsageFlags.ScriptVariable, exported: true),
			new PropertyInfo(Variant.Type.String, PropertyName.GiveKey, PropertyHint.None, "", PropertyUsageFlags.Default | PropertyUsageFlags.ScriptVariable, exported: true),
			new PropertyInfo(Variant.Type.Object, PropertyName.GiveItem, PropertyHint.ResourceType, "ItemDataRes", PropertyUsageFlags.Default | PropertyUsageFlags.ScriptVariable, exported: true),
			new PropertyInfo(Variant.Type.Bool, PropertyName.GameOver, PropertyHint.None, "", PropertyUsageFlags.Default | PropertyUsageFlags.ScriptVariable, exported: true),
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
		info.AddProperty(PropertyName.WantedItem, Variant.From(in WantedItem));
		info.AddProperty(PropertyName.TradeRewardItem, Variant.From(in TradeRewardItem));
		info.AddProperty(PropertyName.TradeRewardInfo, Variant.From(in TradeRewardInfo));
		info.AddProperty(PropertyName.TradeDialogue, Variant.From(in TradeDialogue));
		info.AddProperty(PropertyName.GiveKey, Variant.From(in GiveKey));
		info.AddProperty(PropertyName.GiveItem, Variant.From(in GiveItem));
		info.AddProperty(PropertyName.GameOver, Variant.From(in GameOver));
		info.AddProperty(PropertyName.GiveItemID, Variant.From(in GiveItemID));
		info.AddProperty(PropertyName.ScenePopup, Variant.From(in ScenePopup));
		info.AddProperty(PropertyName.ScenePopupID, Variant.From(in ScenePopupID));
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override void RestoreGodotObjectData(GodotSerializationInfo info)
	{
		base.RestoreGodotObjectData(info);
		if (info.TryGetProperty(PropertyName.WantedItem, out var value))
		{
			WantedItem = value.As<TA_ItemDataRes>();
		}
		if (info.TryGetProperty(PropertyName.TradeRewardItem, out var value2))
		{
			TradeRewardItem = value2.As<TA_ItemDataRes>();
		}
		if (info.TryGetProperty(PropertyName.TradeRewardInfo, out var value3))
		{
			TradeRewardInfo = value3.As<string>();
		}
		if (info.TryGetProperty(PropertyName.TradeDialogue, out var value4))
		{
			TradeDialogue = value4.As<string>();
		}
		if (info.TryGetProperty(PropertyName.GiveKey, out var value5))
		{
			GiveKey = value5.As<string>();
		}
		if (info.TryGetProperty(PropertyName.GiveItem, out var value6))
		{
			GiveItem = value6.As<ItemDataRes>();
		}
		if (info.TryGetProperty(PropertyName.GameOver, out var value7))
		{
			GameOver = value7.As<bool>();
		}
		if (info.TryGetProperty(PropertyName.GiveItemID, out var value8))
		{
			GiveItemID = value8.As<string>();
		}
		if (info.TryGetProperty(PropertyName.ScenePopup, out var value9))
		{
			ScenePopup = value9.As<AttachDataRes>();
		}
		if (info.TryGetProperty(PropertyName.ScenePopupID, out var value10))
		{
			ScenePopupID = value10.As<string>();
		}
	}
}
