using System.Collections.Generic;
using System.ComponentModel;
using Godot;
using Godot.Bridge;
using Godot.NativeInterop;

[GlobalClass]
[ScriptPath("res://Scripts/SubMenus/TerminalMenu/TerminalAdventure/TA_ItemDataRes.cs")]
public class TA_ItemDataRes : Resource
{
	public new class MethodName : Resource.MethodName
	{
	}

	public new class PropertyName : Resource.PropertyName
	{
		public static readonly StringName ItemName = "ItemName";

		public static readonly StringName Description = "Description";

		public static readonly StringName UseDescription = "UseDescription";

		public static readonly StringName Takeable = "Takeable";

		public static readonly StringName GiveKey = "GiveKey";

		public static readonly StringName HiddenUntilKey = "HiddenUntilKey";

		public static readonly StringName RequiredVisibilityKey = "RequiredVisibilityKey";

		public static readonly StringName ExaminePopup = "ExaminePopup";

		public static readonly StringName UsePopup = "UsePopup";

		public static readonly StringName GameOverOnPickup = "GameOverOnPickup";

		public static readonly StringName GameOverOnUse = "GameOverOnUse";

		public static readonly StringName ExaminePopupID = "ExaminePopupID";

		public static readonly StringName UsePopupID = "UsePopupID";
	}

	public new class SignalName : Resource.SignalName
	{
	}

	[Export(PropertyHint.None, "")]
	public string ItemName = "";

	[Export(PropertyHint.MultilineText, "")]
	public string Description = "";

	[Export(PropertyHint.MultilineText, "")]
	public string UseDescription = "";

	[Export(PropertyHint.None, "")]
	public bool Takeable = true;

	[Export(PropertyHint.None, "")]
	public string GiveKey = "";

	[ExportGroup("Visibility", "")]
	[Export(PropertyHint.None, "")]
	public bool HiddenUntilKey;

	[Export(PropertyHint.None, "")]
	public string RequiredVisibilityKey = "";

	[ExportGroup("", "")]
	[Export(PropertyHint.None, "")]
	public AttachDataRes ExaminePopup;

	[Export(PropertyHint.None, "")]
	public AttachDataRes UsePopup;

	[ExportGroup("Game Over Toggles", "")]
	[Export(PropertyHint.None, "")]
	public bool GameOverOnPickup;

	[Export(PropertyHint.None, "")]
	public bool GameOverOnUse;

	[ExportSubgroup("SceneID Override", "")]
	[Export(PropertyHint.None, "")]
	public string ExaminePopupID = "";

	[Export(PropertyHint.None, "")]
	public string UsePopupID = "";

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override bool SetGodotClassPropertyValue(in godot_string_name name, in godot_variant value)
	{
		if (name == PropertyName.ItemName)
		{
			ItemName = VariantUtils.ConvertTo<string>(in value);
			return true;
		}
		if (name == PropertyName.Description)
		{
			Description = VariantUtils.ConvertTo<string>(in value);
			return true;
		}
		if (name == PropertyName.UseDescription)
		{
			UseDescription = VariantUtils.ConvertTo<string>(in value);
			return true;
		}
		if (name == PropertyName.Takeable)
		{
			Takeable = VariantUtils.ConvertTo<bool>(in value);
			return true;
		}
		if (name == PropertyName.GiveKey)
		{
			GiveKey = VariantUtils.ConvertTo<string>(in value);
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
		if (name == PropertyName.ExaminePopup)
		{
			ExaminePopup = VariantUtils.ConvertTo<AttachDataRes>(in value);
			return true;
		}
		if (name == PropertyName.UsePopup)
		{
			UsePopup = VariantUtils.ConvertTo<AttachDataRes>(in value);
			return true;
		}
		if (name == PropertyName.GameOverOnPickup)
		{
			GameOverOnPickup = VariantUtils.ConvertTo<bool>(in value);
			return true;
		}
		if (name == PropertyName.GameOverOnUse)
		{
			GameOverOnUse = VariantUtils.ConvertTo<bool>(in value);
			return true;
		}
		if (name == PropertyName.ExaminePopupID)
		{
			ExaminePopupID = VariantUtils.ConvertTo<string>(in value);
			return true;
		}
		if (name == PropertyName.UsePopupID)
		{
			UsePopupID = VariantUtils.ConvertTo<string>(in value);
			return true;
		}
		return base.SetGodotClassPropertyValue(in name, in value);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override bool GetGodotClassPropertyValue(in godot_string_name name, out godot_variant value)
	{
		if (name == PropertyName.ItemName)
		{
			value = VariantUtils.CreateFrom(in ItemName);
			return true;
		}
		if (name == PropertyName.Description)
		{
			value = VariantUtils.CreateFrom(in Description);
			return true;
		}
		if (name == PropertyName.UseDescription)
		{
			value = VariantUtils.CreateFrom(in UseDescription);
			return true;
		}
		if (name == PropertyName.Takeable)
		{
			value = VariantUtils.CreateFrom(in Takeable);
			return true;
		}
		if (name == PropertyName.GiveKey)
		{
			value = VariantUtils.CreateFrom(in GiveKey);
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
		if (name == PropertyName.ExaminePopup)
		{
			value = VariantUtils.CreateFrom(in ExaminePopup);
			return true;
		}
		if (name == PropertyName.UsePopup)
		{
			value = VariantUtils.CreateFrom(in UsePopup);
			return true;
		}
		if (name == PropertyName.GameOverOnPickup)
		{
			value = VariantUtils.CreateFrom(in GameOverOnPickup);
			return true;
		}
		if (name == PropertyName.GameOverOnUse)
		{
			value = VariantUtils.CreateFrom(in GameOverOnUse);
			return true;
		}
		if (name == PropertyName.ExaminePopupID)
		{
			value = VariantUtils.CreateFrom(in ExaminePopupID);
			return true;
		}
		if (name == PropertyName.UsePopupID)
		{
			value = VariantUtils.CreateFrom(in UsePopupID);
			return true;
		}
		return base.GetGodotClassPropertyValue(in name, out value);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	internal static List<PropertyInfo> GetGodotPropertyList()
	{
		return new List<PropertyInfo>
		{
			new PropertyInfo(Variant.Type.String, PropertyName.ItemName, PropertyHint.None, "", PropertyUsageFlags.Default | PropertyUsageFlags.ScriptVariable, exported: true),
			new PropertyInfo(Variant.Type.String, PropertyName.Description, PropertyHint.MultilineText, "", PropertyUsageFlags.Default | PropertyUsageFlags.ScriptVariable, exported: true),
			new PropertyInfo(Variant.Type.String, PropertyName.UseDescription, PropertyHint.MultilineText, "", PropertyUsageFlags.Default | PropertyUsageFlags.ScriptVariable, exported: true),
			new PropertyInfo(Variant.Type.Bool, PropertyName.Takeable, PropertyHint.None, "", PropertyUsageFlags.Default | PropertyUsageFlags.ScriptVariable, exported: true),
			new PropertyInfo(Variant.Type.String, PropertyName.GiveKey, PropertyHint.None, "", PropertyUsageFlags.Default | PropertyUsageFlags.ScriptVariable, exported: true),
			new PropertyInfo(Variant.Type.Nil, "Visibility", PropertyHint.None, "", PropertyUsageFlags.Group, exported: true),
			new PropertyInfo(Variant.Type.Bool, PropertyName.HiddenUntilKey, PropertyHint.None, "", PropertyUsageFlags.Default | PropertyUsageFlags.ScriptVariable, exported: true),
			new PropertyInfo(Variant.Type.String, PropertyName.RequiredVisibilityKey, PropertyHint.None, "", PropertyUsageFlags.Default | PropertyUsageFlags.ScriptVariable, exported: true),
			new PropertyInfo(Variant.Type.Nil, "", PropertyHint.None, "", PropertyUsageFlags.Group, exported: true),
			new PropertyInfo(Variant.Type.Object, PropertyName.ExaminePopup, PropertyHint.ResourceType, "AttachDataRes", PropertyUsageFlags.Default | PropertyUsageFlags.ScriptVariable, exported: true),
			new PropertyInfo(Variant.Type.Object, PropertyName.UsePopup, PropertyHint.ResourceType, "AttachDataRes", PropertyUsageFlags.Default | PropertyUsageFlags.ScriptVariable, exported: true),
			new PropertyInfo(Variant.Type.Nil, "Game Over Toggles", PropertyHint.None, "", PropertyUsageFlags.Group, exported: true),
			new PropertyInfo(Variant.Type.Bool, PropertyName.GameOverOnPickup, PropertyHint.None, "", PropertyUsageFlags.Default | PropertyUsageFlags.ScriptVariable, exported: true),
			new PropertyInfo(Variant.Type.Bool, PropertyName.GameOverOnUse, PropertyHint.None, "", PropertyUsageFlags.Default | PropertyUsageFlags.ScriptVariable, exported: true),
			new PropertyInfo(Variant.Type.Nil, "SceneID Override", PropertyHint.None, "", PropertyUsageFlags.Subgroup, exported: true),
			new PropertyInfo(Variant.Type.String, PropertyName.ExaminePopupID, PropertyHint.None, "", PropertyUsageFlags.Default | PropertyUsageFlags.ScriptVariable, exported: true),
			new PropertyInfo(Variant.Type.String, PropertyName.UsePopupID, PropertyHint.None, "", PropertyUsageFlags.Default | PropertyUsageFlags.ScriptVariable, exported: true)
		};
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override void SaveGodotObjectData(GodotSerializationInfo info)
	{
		base.SaveGodotObjectData(info);
		info.AddProperty(PropertyName.ItemName, Variant.From(in ItemName));
		info.AddProperty(PropertyName.Description, Variant.From(in Description));
		info.AddProperty(PropertyName.UseDescription, Variant.From(in UseDescription));
		info.AddProperty(PropertyName.Takeable, Variant.From(in Takeable));
		info.AddProperty(PropertyName.GiveKey, Variant.From(in GiveKey));
		info.AddProperty(PropertyName.HiddenUntilKey, Variant.From(in HiddenUntilKey));
		info.AddProperty(PropertyName.RequiredVisibilityKey, Variant.From(in RequiredVisibilityKey));
		info.AddProperty(PropertyName.ExaminePopup, Variant.From(in ExaminePopup));
		info.AddProperty(PropertyName.UsePopup, Variant.From(in UsePopup));
		info.AddProperty(PropertyName.GameOverOnPickup, Variant.From(in GameOverOnPickup));
		info.AddProperty(PropertyName.GameOverOnUse, Variant.From(in GameOverOnUse));
		info.AddProperty(PropertyName.ExaminePopupID, Variant.From(in ExaminePopupID));
		info.AddProperty(PropertyName.UsePopupID, Variant.From(in UsePopupID));
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override void RestoreGodotObjectData(GodotSerializationInfo info)
	{
		base.RestoreGodotObjectData(info);
		if (info.TryGetProperty(PropertyName.ItemName, out var value))
		{
			ItemName = value.As<string>();
		}
		if (info.TryGetProperty(PropertyName.Description, out var value2))
		{
			Description = value2.As<string>();
		}
		if (info.TryGetProperty(PropertyName.UseDescription, out var value3))
		{
			UseDescription = value3.As<string>();
		}
		if (info.TryGetProperty(PropertyName.Takeable, out var value4))
		{
			Takeable = value4.As<bool>();
		}
		if (info.TryGetProperty(PropertyName.GiveKey, out var value5))
		{
			GiveKey = value5.As<string>();
		}
		if (info.TryGetProperty(PropertyName.HiddenUntilKey, out var value6))
		{
			HiddenUntilKey = value6.As<bool>();
		}
		if (info.TryGetProperty(PropertyName.RequiredVisibilityKey, out var value7))
		{
			RequiredVisibilityKey = value7.As<string>();
		}
		if (info.TryGetProperty(PropertyName.ExaminePopup, out var value8))
		{
			ExaminePopup = value8.As<AttachDataRes>();
		}
		if (info.TryGetProperty(PropertyName.UsePopup, out var value9))
		{
			UsePopup = value9.As<AttachDataRes>();
		}
		if (info.TryGetProperty(PropertyName.GameOverOnPickup, out var value10))
		{
			GameOverOnPickup = value10.As<bool>();
		}
		if (info.TryGetProperty(PropertyName.GameOverOnUse, out var value11))
		{
			GameOverOnUse = value11.As<bool>();
		}
		if (info.TryGetProperty(PropertyName.ExaminePopupID, out var value12))
		{
			ExaminePopupID = value12.As<string>();
		}
		if (info.TryGetProperty(PropertyName.UsePopupID, out var value13))
		{
			UsePopupID = value13.As<string>();
		}
	}
}
