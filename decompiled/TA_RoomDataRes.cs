using System.Collections.Generic;
using System.ComponentModel;
using Godot;
using Godot.Bridge;
using Godot.Collections;
using Godot.NativeInterop;

[GlobalClass]
[Tool]
[ScriptPath("res://Scripts/SubMenus/TerminalMenu/TerminalAdventure/TA_RoomDataRes.cs")]
public class TA_RoomDataRes : Resource
{
	public new class MethodName : Resource.MethodName
	{
	}

	public new class PropertyName : Resource.PropertyName
	{
		public static readonly StringName RoomName = "RoomName";

		public static readonly StringName Description = "Description";

		public static readonly StringName Hint = "Hint";

		public static readonly StringName requiredKey = "requiredKey";

		public static readonly StringName lockedDescription = "lockedDescription";

		public static readonly StringName HiddenUntilKey = "HiddenUntilKey";

		public static readonly StringName RequiredVisibilityKey = "RequiredVisibilityKey";

		public static readonly StringName Exits = "Exits";

		public static readonly StringName Items = "Items";

		public static readonly StringName NPCs = "NPCs";

		public static readonly StringName ScenePopup = "ScenePopup";

		public static readonly StringName ScenePopupID = "ScenePopupID";
	}

	public new class SignalName : Resource.SignalName
	{
	}

	[Export(PropertyHint.None, "")]
	public string RoomName = "";

	[Export(PropertyHint.MultilineText, "")]
	public string Description = "";

	[Export(PropertyHint.MultilineText, "")]
	public string Hint = "";

	[Export(PropertyHint.None, "")]
	public string requiredKey = "";

	[Export(PropertyHint.None, "")]
	public string lockedDescription = "This room is locked because you are missing {KEY}";

	[ExportGroup("Visibility", "")]
	[Export(PropertyHint.None, "")]
	public bool HiddenUntilKey;

	[Export(PropertyHint.None, "")]
	public string RequiredVisibilityKey = "";

	[ExportGroup("", "")]
	public Godot.Collections.Dictionary<string, TA_RoomDataRes> Exits = new Godot.Collections.Dictionary<string, TA_RoomDataRes>();

	[Export(PropertyHint.None, "")]
	public Array<TA_ItemDataRes> Items = new Array<TA_ItemDataRes>();

	[Export(PropertyHint.None, "")]
	public Array<TA_NPCDataRes> NPCs = new Array<TA_NPCDataRes>();

	[Export(PropertyHint.None, "")]
	public AttachDataRes ScenePopup;

	[ExportSubgroup("SceneID Override", "")]
	[Export(PropertyHint.None, "")]
	public string ScenePopupID = "";

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override bool SetGodotClassPropertyValue(in godot_string_name name, in godot_variant value)
	{
		if (name == PropertyName.RoomName)
		{
			RoomName = VariantUtils.ConvertTo<string>(in value);
			return true;
		}
		if (name == PropertyName.Description)
		{
			Description = VariantUtils.ConvertTo<string>(in value);
			return true;
		}
		if (name == PropertyName.Hint)
		{
			Hint = VariantUtils.ConvertTo<string>(in value);
			return true;
		}
		if (name == PropertyName.requiredKey)
		{
			requiredKey = VariantUtils.ConvertTo<string>(in value);
			return true;
		}
		if (name == PropertyName.lockedDescription)
		{
			lockedDescription = VariantUtils.ConvertTo<string>(in value);
			return true;
		}
		if (name == PropertyName.HiddenUntilKey)
		{
			HiddenUntilKey = VariantUtils.ConvertTo<bool>(in value);
			return true;
		}
		if (name == PropertyName.RequiredVisibilityKey)
		{
			RequiredVisibilityKey = VariantUtils.ConvertTo<string>(in value);
			return true;
		}
		if (name == PropertyName.Exits)
		{
			Exits = VariantUtils.ConvertToDictionary<string, TA_RoomDataRes>(in value);
			return true;
		}
		if (name == PropertyName.Items)
		{
			Items = VariantUtils.ConvertToArray<TA_ItemDataRes>(in value);
			return true;
		}
		if (name == PropertyName.NPCs)
		{
			NPCs = VariantUtils.ConvertToArray<TA_NPCDataRes>(in value);
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
		if (name == PropertyName.RoomName)
		{
			value = VariantUtils.CreateFrom(in RoomName);
			return true;
		}
		if (name == PropertyName.Description)
		{
			value = VariantUtils.CreateFrom(in Description);
			return true;
		}
		if (name == PropertyName.Hint)
		{
			value = VariantUtils.CreateFrom(in Hint);
			return true;
		}
		if (name == PropertyName.requiredKey)
		{
			value = VariantUtils.CreateFrom(in requiredKey);
			return true;
		}
		if (name == PropertyName.lockedDescription)
		{
			value = VariantUtils.CreateFrom(in lockedDescription);
			return true;
		}
		if (name == PropertyName.HiddenUntilKey)
		{
			value = VariantUtils.CreateFrom(in HiddenUntilKey);
			return true;
		}
		if (name == PropertyName.RequiredVisibilityKey)
		{
			value = VariantUtils.CreateFrom(in RequiredVisibilityKey);
			return true;
		}
		if (name == PropertyName.Exits)
		{
			value = VariantUtils.CreateFromDictionary(Exits);
			return true;
		}
		if (name == PropertyName.Items)
		{
			value = VariantUtils.CreateFromArray(Items);
			return true;
		}
		if (name == PropertyName.NPCs)
		{
			value = VariantUtils.CreateFromArray(NPCs);
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
			new PropertyInfo(Variant.Type.String, PropertyName.RoomName, PropertyHint.None, "", PropertyUsageFlags.Default | PropertyUsageFlags.ScriptVariable, exported: true),
			new PropertyInfo(Variant.Type.String, PropertyName.Description, PropertyHint.MultilineText, "", PropertyUsageFlags.Default | PropertyUsageFlags.ScriptVariable, exported: true),
			new PropertyInfo(Variant.Type.String, PropertyName.Hint, PropertyHint.MultilineText, "", PropertyUsageFlags.Default | PropertyUsageFlags.ScriptVariable, exported: true),
			new PropertyInfo(Variant.Type.String, PropertyName.requiredKey, PropertyHint.None, "", PropertyUsageFlags.Default | PropertyUsageFlags.ScriptVariable, exported: true),
			new PropertyInfo(Variant.Type.String, PropertyName.lockedDescription, PropertyHint.None, "", PropertyUsageFlags.Default | PropertyUsageFlags.ScriptVariable, exported: true),
			new PropertyInfo(Variant.Type.Nil, "Visibility", PropertyHint.None, "", PropertyUsageFlags.Group, exported: true),
			new PropertyInfo(Variant.Type.Bool, PropertyName.HiddenUntilKey, PropertyHint.None, "", PropertyUsageFlags.Default | PropertyUsageFlags.ScriptVariable, exported: true),
			new PropertyInfo(Variant.Type.String, PropertyName.RequiredVisibilityKey, PropertyHint.None, "", PropertyUsageFlags.Default | PropertyUsageFlags.ScriptVariable, exported: true),
			new PropertyInfo(Variant.Type.Nil, "", PropertyHint.None, "", PropertyUsageFlags.Group, exported: true),
			new PropertyInfo(Variant.Type.Dictionary, PropertyName.Exits, PropertyHint.None, "", PropertyUsageFlags.ScriptVariable, exported: false),
			new PropertyInfo(Variant.Type.Array, PropertyName.Items, PropertyHint.TypeString, "24/17:TA_ItemDataRes", PropertyUsageFlags.Default | PropertyUsageFlags.ScriptVariable, exported: true),
			new PropertyInfo(Variant.Type.Array, PropertyName.NPCs, PropertyHint.TypeString, "24/17:TA_NPCDataRes", PropertyUsageFlags.Default | PropertyUsageFlags.ScriptVariable, exported: true),
			new PropertyInfo(Variant.Type.Object, PropertyName.ScenePopup, PropertyHint.ResourceType, "AttachDataRes", PropertyUsageFlags.Default | PropertyUsageFlags.ScriptVariable, exported: true),
			new PropertyInfo(Variant.Type.Nil, "SceneID Override", PropertyHint.None, "", PropertyUsageFlags.Subgroup, exported: true),
			new PropertyInfo(Variant.Type.String, PropertyName.ScenePopupID, PropertyHint.None, "", PropertyUsageFlags.Default | PropertyUsageFlags.ScriptVariable, exported: true)
		};
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override void SaveGodotObjectData(GodotSerializationInfo info)
	{
		base.SaveGodotObjectData(info);
		info.AddProperty(PropertyName.RoomName, Variant.From(in RoomName));
		info.AddProperty(PropertyName.Description, Variant.From(in Description));
		info.AddProperty(PropertyName.Hint, Variant.From(in Hint));
		info.AddProperty(PropertyName.requiredKey, Variant.From(in requiredKey));
		info.AddProperty(PropertyName.lockedDescription, Variant.From(in lockedDescription));
		info.AddProperty(PropertyName.HiddenUntilKey, Variant.From(in HiddenUntilKey));
		info.AddProperty(PropertyName.RequiredVisibilityKey, Variant.From(in RequiredVisibilityKey));
		info.AddProperty(PropertyName.Exits, Variant.CreateFrom(Exits));
		info.AddProperty(PropertyName.Items, Variant.CreateFrom(Items));
		info.AddProperty(PropertyName.NPCs, Variant.CreateFrom(NPCs));
		info.AddProperty(PropertyName.ScenePopup, Variant.From(in ScenePopup));
		info.AddProperty(PropertyName.ScenePopupID, Variant.From(in ScenePopupID));
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override void RestoreGodotObjectData(GodotSerializationInfo info)
	{
		base.RestoreGodotObjectData(info);
		if (info.TryGetProperty(PropertyName.RoomName, out var value))
		{
			RoomName = value.As<string>();
		}
		if (info.TryGetProperty(PropertyName.Description, out var value2))
		{
			Description = value2.As<string>();
		}
		if (info.TryGetProperty(PropertyName.Hint, out var value3))
		{
			Hint = value3.As<string>();
		}
		if (info.TryGetProperty(PropertyName.requiredKey, out var value4))
		{
			requiredKey = value4.As<string>();
		}
		if (info.TryGetProperty(PropertyName.lockedDescription, out var value5))
		{
			lockedDescription = value5.As<string>();
		}
		if (info.TryGetProperty(PropertyName.HiddenUntilKey, out var value6))
		{
			HiddenUntilKey = value6.As<bool>();
		}
		if (info.TryGetProperty(PropertyName.RequiredVisibilityKey, out var value7))
		{
			RequiredVisibilityKey = value7.As<string>();
		}
		if (info.TryGetProperty(PropertyName.Exits, out var value8))
		{
			Exits = value8.AsGodotDictionary<string, TA_RoomDataRes>();
		}
		if (info.TryGetProperty(PropertyName.Items, out var value9))
		{
			Items = value9.AsGodotArray<TA_ItemDataRes>();
		}
		if (info.TryGetProperty(PropertyName.NPCs, out var value10))
		{
			NPCs = value10.AsGodotArray<TA_NPCDataRes>();
		}
		if (info.TryGetProperty(PropertyName.ScenePopup, out var value11))
		{
			ScenePopup = value11.As<AttachDataRes>();
		}
		if (info.TryGetProperty(PropertyName.ScenePopupID, out var value12))
		{
			ScenePopupID = value12.As<string>();
		}
	}
}
