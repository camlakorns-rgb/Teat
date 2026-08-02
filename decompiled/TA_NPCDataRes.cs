using System.Collections.Generic;
using System.ComponentModel;
using Godot;
using Godot.Bridge;
using Godot.Collections;
using Godot.NativeInterop;

[GlobalClass]
[ScriptPath("res://Scripts/SubMenus/TerminalMenu/TerminalAdventure/TA_NPCDataRes.cs")]
public class TA_NPCDataRes : Resource
{
	public new class MethodName : Resource.MethodName
	{
	}

	public new class PropertyName : Resource.PropertyName
	{
		public static readonly StringName NPCName = "NPCName";

		public static readonly StringName Description = "Description";

		public static readonly StringName GreetingDialogue = "GreetingDialogue";

		public static readonly StringName RefuseItemDialogue = "RefuseItemDialogue";

		public static readonly StringName HeldItems = "HeldItems";

		public static readonly StringName Trades = "Trades";

		public static readonly StringName RejectedItems = "RejectedItems";

		public static readonly StringName AskTopics = "AskTopics";

		public static readonly StringName KeyOrNPCPresentDialogue = "KeyOrNPCPresentDialogue";

		public static readonly StringName UnknownTopicDialogue = "UnknownTopicDialogue";

		public static readonly StringName ScenePopup = "ScenePopup";

		public static readonly StringName ScenePopupID = "ScenePopupID";

		public static readonly StringName HiddenUntilKey = "HiddenUntilKey";

		public static readonly StringName RequiredVisibilityKey = "RequiredVisibilityKey";

		public static readonly StringName Recruitable = "Recruitable";

		public static readonly StringName RecruitableKey = "RecruitableKey";

		public static readonly StringName PartyDialogue = "PartyDialogue";

		public static readonly StringName taggedKinks = "taggedKinks";
	}

	public new class SignalName : Resource.SignalName
	{
	}

	[Export(PropertyHint.None, "")]
	public string NPCName = "";

	[Export(PropertyHint.MultilineText, "")]
	public string Description = "";

	[Export(PropertyHint.MultilineText, "")]
	public string GreetingDialogue = "";

	[Export(PropertyHint.MultilineText, "")]
	public string RefuseItemDialogue = "I have no use for the {item}.";

	[Export(PropertyHint.None, "")]
	public Array<TA_ItemDataRes> HeldItems = new Array<TA_ItemDataRes>();

	[Export(PropertyHint.None, "")]
	public Array<TA_TradeDataRes> Trades = new Array<TA_TradeDataRes>();

	[Export(PropertyHint.MultilineText, "")]
	public Godot.Collections.Dictionary<TA_ItemDataRes, string> RejectedItems = new Godot.Collections.Dictionary<TA_ItemDataRes, string>();

	[Export(PropertyHint.None, "")]
	public Array<TA_TopicDataRes> AskTopics = new Array<TA_TopicDataRes>();

	[Export(PropertyHint.None, "")]
	public Godot.Collections.Dictionary<string, TA_TopicDataRes> KeyOrNPCPresentDialogue = new Godot.Collections.Dictionary<string, TA_TopicDataRes>();

	[Export(PropertyHint.MultilineText, "")]
	public string UnknownTopicDialogue = "I wouldn't know anything about that.";

	[Export(PropertyHint.None, "")]
	public AttachDataRes ScenePopup;

	[ExportSubgroup("SceneID Override", "")]
	[Export(PropertyHint.None, "")]
	public string ScenePopupID = "";

	[ExportGroup("Visibility", "")]
	[Export(PropertyHint.None, "")]
	public bool HiddenUntilKey;

	[Export(PropertyHint.None, "")]
	public string RequiredVisibilityKey = "";

	[ExportGroup("Recruit Settings", "")]
	[Export(PropertyHint.None, "")]
	public bool Recruitable;

	[Export(PropertyHint.None, "")]
	public string RecruitableKey = "";

	[Export(PropertyHint.None, "")]
	public Array<TA_TopicDataRes> PartyDialogue = new Array<TA_TopicDataRes>();

	[ExportGroup("Kink Settings", "")]
	[Export(PropertyHint.None, "")]
	public Array<SaveHandler.Kinks> taggedKinks = new Array<SaveHandler.Kinks>();

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override bool SetGodotClassPropertyValue(in godot_string_name name, in godot_variant value)
	{
		if (name == PropertyName.NPCName)
		{
			NPCName = VariantUtils.ConvertTo<string>(in value);
			return true;
		}
		if (name == PropertyName.Description)
		{
			Description = VariantUtils.ConvertTo<string>(in value);
			return true;
		}
		if (name == PropertyName.GreetingDialogue)
		{
			GreetingDialogue = VariantUtils.ConvertTo<string>(in value);
			return true;
		}
		if (name == PropertyName.RefuseItemDialogue)
		{
			RefuseItemDialogue = VariantUtils.ConvertTo<string>(in value);
			return true;
		}
		if (name == PropertyName.HeldItems)
		{
			HeldItems = VariantUtils.ConvertToArray<TA_ItemDataRes>(in value);
			return true;
		}
		if (name == PropertyName.Trades)
		{
			Trades = VariantUtils.ConvertToArray<TA_TradeDataRes>(in value);
			return true;
		}
		if (name == PropertyName.RejectedItems)
		{
			RejectedItems = VariantUtils.ConvertToDictionary<TA_ItemDataRes, string>(in value);
			return true;
		}
		if (name == PropertyName.AskTopics)
		{
			AskTopics = VariantUtils.ConvertToArray<TA_TopicDataRes>(in value);
			return true;
		}
		if (name == PropertyName.KeyOrNPCPresentDialogue)
		{
			KeyOrNPCPresentDialogue = VariantUtils.ConvertToDictionary<string, TA_TopicDataRes>(in value);
			return true;
		}
		if (name == PropertyName.UnknownTopicDialogue)
		{
			UnknownTopicDialogue = VariantUtils.ConvertTo<string>(in value);
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
		if (name == PropertyName.Recruitable)
		{
			Recruitable = VariantUtils.ConvertTo<bool>(in value);
			return true;
		}
		if (name == PropertyName.RecruitableKey)
		{
			RecruitableKey = VariantUtils.ConvertTo<string>(in value);
			return true;
		}
		if (name == PropertyName.PartyDialogue)
		{
			PartyDialogue = VariantUtils.ConvertToArray<TA_TopicDataRes>(in value);
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
		if (name == PropertyName.NPCName)
		{
			value = VariantUtils.CreateFrom(in NPCName);
			return true;
		}
		if (name == PropertyName.Description)
		{
			value = VariantUtils.CreateFrom(in Description);
			return true;
		}
		if (name == PropertyName.GreetingDialogue)
		{
			value = VariantUtils.CreateFrom(in GreetingDialogue);
			return true;
		}
		if (name == PropertyName.RefuseItemDialogue)
		{
			value = VariantUtils.CreateFrom(in RefuseItemDialogue);
			return true;
		}
		if (name == PropertyName.HeldItems)
		{
			value = VariantUtils.CreateFromArray(HeldItems);
			return true;
		}
		if (name == PropertyName.Trades)
		{
			value = VariantUtils.CreateFromArray(Trades);
			return true;
		}
		if (name == PropertyName.RejectedItems)
		{
			value = VariantUtils.CreateFromDictionary(RejectedItems);
			return true;
		}
		if (name == PropertyName.AskTopics)
		{
			value = VariantUtils.CreateFromArray(AskTopics);
			return true;
		}
		if (name == PropertyName.KeyOrNPCPresentDialogue)
		{
			value = VariantUtils.CreateFromDictionary(KeyOrNPCPresentDialogue);
			return true;
		}
		if (name == PropertyName.UnknownTopicDialogue)
		{
			value = VariantUtils.CreateFrom(in UnknownTopicDialogue);
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
		if (name == PropertyName.Recruitable)
		{
			value = VariantUtils.CreateFrom(in Recruitable);
			return true;
		}
		if (name == PropertyName.RecruitableKey)
		{
			value = VariantUtils.CreateFrom(in RecruitableKey);
			return true;
		}
		if (name == PropertyName.PartyDialogue)
		{
			value = VariantUtils.CreateFromArray(PartyDialogue);
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
			new PropertyInfo(Variant.Type.String, PropertyName.NPCName, PropertyHint.None, "", PropertyUsageFlags.Default | PropertyUsageFlags.ScriptVariable, exported: true),
			new PropertyInfo(Variant.Type.String, PropertyName.Description, PropertyHint.MultilineText, "", PropertyUsageFlags.Default | PropertyUsageFlags.ScriptVariable, exported: true),
			new PropertyInfo(Variant.Type.String, PropertyName.GreetingDialogue, PropertyHint.MultilineText, "", PropertyUsageFlags.Default | PropertyUsageFlags.ScriptVariable, exported: true),
			new PropertyInfo(Variant.Type.String, PropertyName.RefuseItemDialogue, PropertyHint.MultilineText, "", PropertyUsageFlags.Default | PropertyUsageFlags.ScriptVariable, exported: true),
			new PropertyInfo(Variant.Type.Array, PropertyName.HeldItems, PropertyHint.TypeString, "24/17:TA_ItemDataRes", PropertyUsageFlags.Default | PropertyUsageFlags.ScriptVariable, exported: true),
			new PropertyInfo(Variant.Type.Array, PropertyName.Trades, PropertyHint.TypeString, "24/17:TA_TradeDataRes", PropertyUsageFlags.Default | PropertyUsageFlags.ScriptVariable, exported: true),
			new PropertyInfo(Variant.Type.Dictionary, PropertyName.RejectedItems, PropertyHint.TypeString, "24/17:TA_ItemDataRes;4/0:", PropertyUsageFlags.Default | PropertyUsageFlags.ScriptVariable, exported: true),
			new PropertyInfo(Variant.Type.Array, PropertyName.AskTopics, PropertyHint.TypeString, "24/17:TA_TopicDataRes", PropertyUsageFlags.Default | PropertyUsageFlags.ScriptVariable, exported: true),
			new PropertyInfo(Variant.Type.Dictionary, PropertyName.KeyOrNPCPresentDialogue, PropertyHint.TypeString, "4/0:;24/17:TA_TopicDataRes", PropertyUsageFlags.Default | PropertyUsageFlags.ScriptVariable, exported: true),
			new PropertyInfo(Variant.Type.String, PropertyName.UnknownTopicDialogue, PropertyHint.MultilineText, "", PropertyUsageFlags.Default | PropertyUsageFlags.ScriptVariable, exported: true),
			new PropertyInfo(Variant.Type.Object, PropertyName.ScenePopup, PropertyHint.ResourceType, "AttachDataRes", PropertyUsageFlags.Default | PropertyUsageFlags.ScriptVariable, exported: true),
			new PropertyInfo(Variant.Type.Nil, "SceneID Override", PropertyHint.None, "", PropertyUsageFlags.Subgroup, exported: true),
			new PropertyInfo(Variant.Type.String, PropertyName.ScenePopupID, PropertyHint.None, "", PropertyUsageFlags.Default | PropertyUsageFlags.ScriptVariable, exported: true),
			new PropertyInfo(Variant.Type.Nil, "Visibility", PropertyHint.None, "", PropertyUsageFlags.Group, exported: true),
			new PropertyInfo(Variant.Type.Bool, PropertyName.HiddenUntilKey, PropertyHint.None, "", PropertyUsageFlags.Default | PropertyUsageFlags.ScriptVariable, exported: true),
			new PropertyInfo(Variant.Type.String, PropertyName.RequiredVisibilityKey, PropertyHint.None, "", PropertyUsageFlags.Default | PropertyUsageFlags.ScriptVariable, exported: true),
			new PropertyInfo(Variant.Type.Nil, "Recruit Settings", PropertyHint.None, "", PropertyUsageFlags.Group, exported: true),
			new PropertyInfo(Variant.Type.Bool, PropertyName.Recruitable, PropertyHint.None, "", PropertyUsageFlags.Default | PropertyUsageFlags.ScriptVariable, exported: true),
			new PropertyInfo(Variant.Type.String, PropertyName.RecruitableKey, PropertyHint.None, "", PropertyUsageFlags.Default | PropertyUsageFlags.ScriptVariable, exported: true),
			new PropertyInfo(Variant.Type.Array, PropertyName.PartyDialogue, PropertyHint.TypeString, "24/17:TA_TopicDataRes", PropertyUsageFlags.Default | PropertyUsageFlags.ScriptVariable, exported: true),
			new PropertyInfo(Variant.Type.Nil, "Kink Settings", PropertyHint.None, "", PropertyUsageFlags.Group, exported: true),
			new PropertyInfo(Variant.Type.Array, PropertyName.taggedKinks, PropertyHint.TypeString, "2/2:UNTYPED,CUCKING", PropertyUsageFlags.Default | PropertyUsageFlags.ScriptVariable, exported: true)
		};
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override void SaveGodotObjectData(GodotSerializationInfo info)
	{
		base.SaveGodotObjectData(info);
		info.AddProperty(PropertyName.NPCName, Variant.From(in NPCName));
		info.AddProperty(PropertyName.Description, Variant.From(in Description));
		info.AddProperty(PropertyName.GreetingDialogue, Variant.From(in GreetingDialogue));
		info.AddProperty(PropertyName.RefuseItemDialogue, Variant.From(in RefuseItemDialogue));
		info.AddProperty(PropertyName.HeldItems, Variant.CreateFrom(HeldItems));
		info.AddProperty(PropertyName.Trades, Variant.CreateFrom(Trades));
		info.AddProperty(PropertyName.RejectedItems, Variant.CreateFrom(RejectedItems));
		info.AddProperty(PropertyName.AskTopics, Variant.CreateFrom(AskTopics));
		info.AddProperty(PropertyName.KeyOrNPCPresentDialogue, Variant.CreateFrom(KeyOrNPCPresentDialogue));
		info.AddProperty(PropertyName.UnknownTopicDialogue, Variant.From(in UnknownTopicDialogue));
		info.AddProperty(PropertyName.ScenePopup, Variant.From(in ScenePopup));
		info.AddProperty(PropertyName.ScenePopupID, Variant.From(in ScenePopupID));
		info.AddProperty(PropertyName.HiddenUntilKey, Variant.From(in HiddenUntilKey));
		info.AddProperty(PropertyName.RequiredVisibilityKey, Variant.From(in RequiredVisibilityKey));
		info.AddProperty(PropertyName.Recruitable, Variant.From(in Recruitable));
		info.AddProperty(PropertyName.RecruitableKey, Variant.From(in RecruitableKey));
		info.AddProperty(PropertyName.PartyDialogue, Variant.CreateFrom(PartyDialogue));
		info.AddProperty(PropertyName.taggedKinks, Variant.CreateFrom(taggedKinks));
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override void RestoreGodotObjectData(GodotSerializationInfo info)
	{
		base.RestoreGodotObjectData(info);
		if (info.TryGetProperty(PropertyName.NPCName, out var value))
		{
			NPCName = value.As<string>();
		}
		if (info.TryGetProperty(PropertyName.Description, out var value2))
		{
			Description = value2.As<string>();
		}
		if (info.TryGetProperty(PropertyName.GreetingDialogue, out var value3))
		{
			GreetingDialogue = value3.As<string>();
		}
		if (info.TryGetProperty(PropertyName.RefuseItemDialogue, out var value4))
		{
			RefuseItemDialogue = value4.As<string>();
		}
		if (info.TryGetProperty(PropertyName.HeldItems, out var value5))
		{
			HeldItems = value5.AsGodotArray<TA_ItemDataRes>();
		}
		if (info.TryGetProperty(PropertyName.Trades, out var value6))
		{
			Trades = value6.AsGodotArray<TA_TradeDataRes>();
		}
		if (info.TryGetProperty(PropertyName.RejectedItems, out var value7))
		{
			RejectedItems = value7.AsGodotDictionary<TA_ItemDataRes, string>();
		}
		if (info.TryGetProperty(PropertyName.AskTopics, out var value8))
		{
			AskTopics = value8.AsGodotArray<TA_TopicDataRes>();
		}
		if (info.TryGetProperty(PropertyName.KeyOrNPCPresentDialogue, out var value9))
		{
			KeyOrNPCPresentDialogue = value9.AsGodotDictionary<string, TA_TopicDataRes>();
		}
		if (info.TryGetProperty(PropertyName.UnknownTopicDialogue, out var value10))
		{
			UnknownTopicDialogue = value10.As<string>();
		}
		if (info.TryGetProperty(PropertyName.ScenePopup, out var value11))
		{
			ScenePopup = value11.As<AttachDataRes>();
		}
		if (info.TryGetProperty(PropertyName.ScenePopupID, out var value12))
		{
			ScenePopupID = value12.As<string>();
		}
		if (info.TryGetProperty(PropertyName.HiddenUntilKey, out var value13))
		{
			HiddenUntilKey = value13.As<bool>();
		}
		if (info.TryGetProperty(PropertyName.RequiredVisibilityKey, out var value14))
		{
			RequiredVisibilityKey = value14.As<string>();
		}
		if (info.TryGetProperty(PropertyName.Recruitable, out var value15))
		{
			Recruitable = value15.As<bool>();
		}
		if (info.TryGetProperty(PropertyName.RecruitableKey, out var value16))
		{
			RecruitableKey = value16.As<string>();
		}
		if (info.TryGetProperty(PropertyName.PartyDialogue, out var value17))
		{
			PartyDialogue = value17.AsGodotArray<TA_TopicDataRes>();
		}
		if (info.TryGetProperty(PropertyName.taggedKinks, out var value18))
		{
			taggedKinks = value18.AsGodotArray<SaveHandler.Kinks>();
		}
	}
}
