using System.Collections.Generic;
using System.ComponentModel;
using Godot;
using Godot.Bridge;
using Godot.Collections;
using Godot.NativeInterop;

[GlobalClass]
[Tool]
[ScriptPath("res://Scripts/DataResources/AttachDataRes.cs")]
public class AttachDataRes : Resource
{
	public enum AttachmentType
	{
		UNTYPED,
		IMAGE,
		TEXT,
		OVERRIDE,
		RANDOM_CLICKED_WINDOW
	}

	public new class MethodName : Resource.MethodName
	{
		public static readonly StringName generateItemID = "generateItemID";
	}

	public new class PropertyName : Resource.PropertyName
	{
		public static readonly StringName itemID = "itemID";

		public static readonly StringName Name = "Name";

		public static readonly StringName itemVariation = "itemVariation";

		public static readonly StringName _itemID = "_itemID";

		public static readonly StringName _name = "_name";

		public static readonly StringName attachmentTyping = "attachmentTyping";

		public static readonly StringName attachmentScale = "attachmentScale";

		public static readonly StringName attachmentMargin = "attachmentMargin";

		public static readonly StringName attachmentAppeanceWeight = "attachmentAppeanceWeight";

		public static readonly StringName attachmentImageSize = "attachmentImageSize";

		public static readonly StringName attachmentAnimations = "attachmentAnimations";

		public static readonly StringName isKillOnRandomTimer = "isKillOnRandomTimer";

		public static readonly StringName randomKillOnTime = "randomKillOnTime";

		public static readonly StringName dialogueStack = "dialogueStack";

		public static readonly StringName attachmentStack = "attachmentStack";

		public static readonly StringName TagPair = "TagPair";

		public static readonly StringName RequiredTags = "RequiredTags";

		public static readonly StringName ChainedOverride = "ChainedOverride";

		public static readonly StringName chanceOfItem = "chanceOfItem";

		public static readonly StringName possibleItems = "possibleItems";

		public static readonly StringName overrideTaskbar = "overrideTaskbar";

		public static readonly StringName excludePopup = "excludePopup";

		public static readonly StringName movesActorAttachment = "movesActorAttachment";

		public static readonly StringName movementDuration = "movementDuration";

		public static readonly StringName ForcedIdleTimeRange = "ForcedIdleTimeRange";

		public static readonly StringName ForcedWalkTimeRange = "ForcedWalkTimeRange";

		public static readonly StringName ForcedWalkSpeed = "ForcedWalkSpeed";

		public static readonly StringName popupURL = "popupURL";

		public static readonly StringName taggedKinks = "taggedKinks";
	}

	public new class SignalName : Resource.SignalName
	{
	}

	public string _itemID;

	public string _name = "EMPTY TITLE";

	[Export(PropertyHint.None, "")]
	public AttachmentType attachmentTyping = AttachmentType.IMAGE;

	[Export(PropertyHint.None, "")]
	public Vector2 attachmentScale = new Vector2I(1, 1);

	[Export(PropertyHint.None, "")]
	public Vector2 attachmentMargin = new Vector2I(0, 0);

	[Export(PropertyHint.None, "")]
	public float attachmentAppeanceWeight = 25f;

	[ExportGroup("IMAGE INFORMATION", "")]
	[Export(PropertyHint.None, "")]
	public Vector2I attachmentImageSize = new Vector2I(256, 256);

	[Export(PropertyHint.None, "")]
	public Array<SpriteFrames> attachmentAnimations = new Array<SpriteFrames>();

	[Export(PropertyHint.None, "")]
	public bool isKillOnRandomTimer;

	[Export(PropertyHint.None, "")]
	public Vector2 randomKillOnTime = new Vector2(1f, 5f);

	[ExportGroup("DIALOGUE INFORMATION", "")]
	[Export(PropertyHint.None, "")]
	public Godot.Collections.Dictionary<int, DialogueDataRes> dialogueStack = new Godot.Collections.Dictionary<int, DialogueDataRes>();

	[ExportGroup("ATTACH INFORMATION", "")]
	[Export(PropertyHint.None, "")]
	public Godot.Collections.Dictionary<int, AttachDataRes> attachmentStack = new Godot.Collections.Dictionary<int, AttachDataRes>();

	[ExportGroup("TAGGING SYSTEM", "")]
	[Export(PropertyHint.None, "")]
	public Array<TagDataRes> TagPair = new Array<TagDataRes>();

	[ExportSubgroup("ATTACHMENT REQUIRED TAGS", "")]
	[Export(PropertyHint.None, "")]
	public Array<TagDataRes> RequiredTags = new Array<TagDataRes>();

	[ExportGroup("OVERRIDE CHAIN", "")]
	[Export(PropertyHint.None, "")]
	public AttachDataRes ChainedOverride;

	[ExportGroup("SPAWN CHAIN", "")]
	[Export(PropertyHint.None, "")]
	public float chanceOfItem = 10f;

	[Export(PropertyHint.None, "")]
	public Array<ItemDataRes> possibleItems = new Array<ItemDataRes>();

	[ExportGroup("SPECIAL FLAGS", "")]
	[Export(PropertyHint.None, "")]
	public bool overrideTaskbar;

	[Export(PropertyHint.None, "")]
	public bool excludePopup;

	[Export(PropertyHint.None, "")]
	public bool movesActorAttachment;

	[ExportSubgroup("Moving Override", "")]
	[Export(PropertyHint.None, "")]
	public float movementDuration = 15f;

	[Export(PropertyHint.None, "")]
	public Vector2 ForcedIdleTimeRange = new Vector2(1.5f, 4f);

	[Export(PropertyHint.None, "")]
	public Vector2 ForcedWalkTimeRange = new Vector2(2f, 6f);

	[Export(PropertyHint.None, "")]
	public float ForcedWalkSpeed = 220f;

	[ExportSubgroup("URL Popup", "")]
	[Export(PropertyHint.None, "")]
	public string popupURL = "";

	[ExportGroup("Kink Settings", "")]
	[Export(PropertyHint.None, "")]
	public Array<SaveHandler.Kinks> taggedKinks = new Array<SaveHandler.Kinks>();

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

	[Export(PropertyHint.None, "")]
	public int itemVariation { get; set; }

	public AttachDataRes()
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
		if (name == PropertyName.Name)
		{
			Name = VariantUtils.ConvertTo<string>(in value);
			return true;
		}
		if (name == PropertyName.itemVariation)
		{
			itemVariation = VariantUtils.ConvertTo<int>(in value);
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
		if (name == PropertyName.attachmentTyping)
		{
			attachmentTyping = VariantUtils.ConvertTo<AttachmentType>(in value);
			return true;
		}
		if (name == PropertyName.attachmentScale)
		{
			attachmentScale = VariantUtils.ConvertTo<Vector2>(in value);
			return true;
		}
		if (name == PropertyName.attachmentMargin)
		{
			attachmentMargin = VariantUtils.ConvertTo<Vector2>(in value);
			return true;
		}
		if (name == PropertyName.attachmentAppeanceWeight)
		{
			attachmentAppeanceWeight = VariantUtils.ConvertTo<float>(in value);
			return true;
		}
		if (name == PropertyName.attachmentImageSize)
		{
			attachmentImageSize = VariantUtils.ConvertTo<Vector2I>(in value);
			return true;
		}
		if (name == PropertyName.attachmentAnimations)
		{
			attachmentAnimations = VariantUtils.ConvertToArray<SpriteFrames>(in value);
			return true;
		}
		if (name == PropertyName.isKillOnRandomTimer)
		{
			isKillOnRandomTimer = VariantUtils.ConvertTo<bool>(in value);
			return true;
		}
		if (name == PropertyName.randomKillOnTime)
		{
			randomKillOnTime = VariantUtils.ConvertTo<Vector2>(in value);
			return true;
		}
		if (name == PropertyName.dialogueStack)
		{
			dialogueStack = VariantUtils.ConvertToDictionary<int, DialogueDataRes>(in value);
			return true;
		}
		if (name == PropertyName.attachmentStack)
		{
			attachmentStack = VariantUtils.ConvertToDictionary<int, AttachDataRes>(in value);
			return true;
		}
		if (name == PropertyName.TagPair)
		{
			TagPair = VariantUtils.ConvertToArray<TagDataRes>(in value);
			return true;
		}
		if (name == PropertyName.RequiredTags)
		{
			RequiredTags = VariantUtils.ConvertToArray<TagDataRes>(in value);
			return true;
		}
		if (name == PropertyName.ChainedOverride)
		{
			ChainedOverride = VariantUtils.ConvertTo<AttachDataRes>(in value);
			return true;
		}
		if (name == PropertyName.chanceOfItem)
		{
			chanceOfItem = VariantUtils.ConvertTo<float>(in value);
			return true;
		}
		if (name == PropertyName.possibleItems)
		{
			possibleItems = VariantUtils.ConvertToArray<ItemDataRes>(in value);
			return true;
		}
		if (name == PropertyName.overrideTaskbar)
		{
			overrideTaskbar = VariantUtils.ConvertTo<bool>(in value);
			return true;
		}
		if (name == PropertyName.excludePopup)
		{
			excludePopup = VariantUtils.ConvertTo<bool>(in value);
			return true;
		}
		if (name == PropertyName.movesActorAttachment)
		{
			movesActorAttachment = VariantUtils.ConvertTo<bool>(in value);
			return true;
		}
		if (name == PropertyName.movementDuration)
		{
			movementDuration = VariantUtils.ConvertTo<float>(in value);
			return true;
		}
		if (name == PropertyName.ForcedIdleTimeRange)
		{
			ForcedIdleTimeRange = VariantUtils.ConvertTo<Vector2>(in value);
			return true;
		}
		if (name == PropertyName.ForcedWalkTimeRange)
		{
			ForcedWalkTimeRange = VariantUtils.ConvertTo<Vector2>(in value);
			return true;
		}
		if (name == PropertyName.ForcedWalkSpeed)
		{
			ForcedWalkSpeed = VariantUtils.ConvertTo<float>(in value);
			return true;
		}
		if (name == PropertyName.popupURL)
		{
			popupURL = VariantUtils.ConvertTo<string>(in value);
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
		if (name == PropertyName.itemVariation)
		{
			int from2 = itemVariation;
			value = VariantUtils.CreateFrom(in from2);
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
		if (name == PropertyName.attachmentTyping)
		{
			value = VariantUtils.CreateFrom(in attachmentTyping);
			return true;
		}
		if (name == PropertyName.attachmentScale)
		{
			value = VariantUtils.CreateFrom(in attachmentScale);
			return true;
		}
		if (name == PropertyName.attachmentMargin)
		{
			value = VariantUtils.CreateFrom(in attachmentMargin);
			return true;
		}
		if (name == PropertyName.attachmentAppeanceWeight)
		{
			value = VariantUtils.CreateFrom(in attachmentAppeanceWeight);
			return true;
		}
		if (name == PropertyName.attachmentImageSize)
		{
			value = VariantUtils.CreateFrom(in attachmentImageSize);
			return true;
		}
		if (name == PropertyName.attachmentAnimations)
		{
			value = VariantUtils.CreateFromArray(attachmentAnimations);
			return true;
		}
		if (name == PropertyName.isKillOnRandomTimer)
		{
			value = VariantUtils.CreateFrom(in isKillOnRandomTimer);
			return true;
		}
		if (name == PropertyName.randomKillOnTime)
		{
			value = VariantUtils.CreateFrom(in randomKillOnTime);
			return true;
		}
		if (name == PropertyName.dialogueStack)
		{
			value = VariantUtils.CreateFromDictionary(dialogueStack);
			return true;
		}
		if (name == PropertyName.attachmentStack)
		{
			value = VariantUtils.CreateFromDictionary(attachmentStack);
			return true;
		}
		if (name == PropertyName.TagPair)
		{
			value = VariantUtils.CreateFromArray(TagPair);
			return true;
		}
		if (name == PropertyName.RequiredTags)
		{
			value = VariantUtils.CreateFromArray(RequiredTags);
			return true;
		}
		if (name == PropertyName.ChainedOverride)
		{
			value = VariantUtils.CreateFrom(in ChainedOverride);
			return true;
		}
		if (name == PropertyName.chanceOfItem)
		{
			value = VariantUtils.CreateFrom(in chanceOfItem);
			return true;
		}
		if (name == PropertyName.possibleItems)
		{
			value = VariantUtils.CreateFromArray(possibleItems);
			return true;
		}
		if (name == PropertyName.overrideTaskbar)
		{
			value = VariantUtils.CreateFrom(in overrideTaskbar);
			return true;
		}
		if (name == PropertyName.excludePopup)
		{
			value = VariantUtils.CreateFrom(in excludePopup);
			return true;
		}
		if (name == PropertyName.movesActorAttachment)
		{
			value = VariantUtils.CreateFrom(in movesActorAttachment);
			return true;
		}
		if (name == PropertyName.movementDuration)
		{
			value = VariantUtils.CreateFrom(in movementDuration);
			return true;
		}
		if (name == PropertyName.ForcedIdleTimeRange)
		{
			value = VariantUtils.CreateFrom(in ForcedIdleTimeRange);
			return true;
		}
		if (name == PropertyName.ForcedWalkTimeRange)
		{
			value = VariantUtils.CreateFrom(in ForcedWalkTimeRange);
			return true;
		}
		if (name == PropertyName.ForcedWalkSpeed)
		{
			value = VariantUtils.CreateFrom(in ForcedWalkSpeed);
			return true;
		}
		if (name == PropertyName.popupURL)
		{
			value = VariantUtils.CreateFrom(in popupURL);
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
			new PropertyInfo(Variant.Type.String, PropertyName._itemID, PropertyHint.None, "", PropertyUsageFlags.ScriptVariable, exported: false),
			new PropertyInfo(Variant.Type.String, PropertyName._name, PropertyHint.None, "", PropertyUsageFlags.ScriptVariable, exported: false),
			new PropertyInfo(Variant.Type.String, PropertyName.itemID, PropertyHint.None, "", PropertyUsageFlags.Default | PropertyUsageFlags.ScriptVariable, exported: true),
			new PropertyInfo(Variant.Type.Nil, "Core Properties", PropertyHint.None, "", PropertyUsageFlags.Group, exported: true),
			new PropertyInfo(Variant.Type.String, PropertyName.Name, PropertyHint.None, "", PropertyUsageFlags.Default | PropertyUsageFlags.ScriptVariable, exported: true),
			new PropertyInfo(Variant.Type.Int, PropertyName.itemVariation, PropertyHint.None, "", PropertyUsageFlags.Default | PropertyUsageFlags.ScriptVariable, exported: true),
			new PropertyInfo(Variant.Type.Int, PropertyName.attachmentTyping, PropertyHint.Enum, "UNTYPED,IMAGE,TEXT,OVERRIDE,RANDOM_CLICKED_WINDOW", PropertyUsageFlags.Default | PropertyUsageFlags.ScriptVariable, exported: true),
			new PropertyInfo(Variant.Type.Vector2, PropertyName.attachmentScale, PropertyHint.None, "", PropertyUsageFlags.Default | PropertyUsageFlags.ScriptVariable, exported: true),
			new PropertyInfo(Variant.Type.Vector2, PropertyName.attachmentMargin, PropertyHint.None, "", PropertyUsageFlags.Default | PropertyUsageFlags.ScriptVariable, exported: true),
			new PropertyInfo(Variant.Type.Float, PropertyName.attachmentAppeanceWeight, PropertyHint.None, "", PropertyUsageFlags.Default | PropertyUsageFlags.ScriptVariable, exported: true),
			new PropertyInfo(Variant.Type.Nil, "IMAGE INFORMATION", PropertyHint.None, "", PropertyUsageFlags.Group, exported: true),
			new PropertyInfo(Variant.Type.Vector2I, PropertyName.attachmentImageSize, PropertyHint.None, "", PropertyUsageFlags.Default | PropertyUsageFlags.ScriptVariable, exported: true),
			new PropertyInfo(Variant.Type.Array, PropertyName.attachmentAnimations, PropertyHint.TypeString, "24/17:SpriteFrames", PropertyUsageFlags.Default | PropertyUsageFlags.ScriptVariable, exported: true),
			new PropertyInfo(Variant.Type.Bool, PropertyName.isKillOnRandomTimer, PropertyHint.None, "", PropertyUsageFlags.Default | PropertyUsageFlags.ScriptVariable, exported: true),
			new PropertyInfo(Variant.Type.Vector2, PropertyName.randomKillOnTime, PropertyHint.None, "", PropertyUsageFlags.Default | PropertyUsageFlags.ScriptVariable, exported: true),
			new PropertyInfo(Variant.Type.Nil, "DIALOGUE INFORMATION", PropertyHint.None, "", PropertyUsageFlags.Group, exported: true),
			new PropertyInfo(Variant.Type.Dictionary, PropertyName.dialogueStack, PropertyHint.TypeString, "2/0:;24/17:DialogueDataRes", PropertyUsageFlags.Default | PropertyUsageFlags.ScriptVariable, exported: true),
			new PropertyInfo(Variant.Type.Nil, "ATTACH INFORMATION", PropertyHint.None, "", PropertyUsageFlags.Group, exported: true),
			new PropertyInfo(Variant.Type.Dictionary, PropertyName.attachmentStack, PropertyHint.TypeString, "2/0:;24/17:AttachDataRes", PropertyUsageFlags.Default | PropertyUsageFlags.ScriptVariable, exported: true),
			new PropertyInfo(Variant.Type.Nil, "TAGGING SYSTEM", PropertyHint.None, "", PropertyUsageFlags.Group, exported: true),
			new PropertyInfo(Variant.Type.Array, PropertyName.TagPair, PropertyHint.TypeString, "24/17:TagDataRes", PropertyUsageFlags.Default | PropertyUsageFlags.ScriptVariable, exported: true),
			new PropertyInfo(Variant.Type.Nil, "ATTACHMENT REQUIRED TAGS", PropertyHint.None, "", PropertyUsageFlags.Subgroup, exported: true),
			new PropertyInfo(Variant.Type.Array, PropertyName.RequiredTags, PropertyHint.TypeString, "24/17:TagDataRes", PropertyUsageFlags.Default | PropertyUsageFlags.ScriptVariable, exported: true),
			new PropertyInfo(Variant.Type.Nil, "OVERRIDE CHAIN", PropertyHint.None, "", PropertyUsageFlags.Group, exported: true),
			new PropertyInfo(Variant.Type.Object, PropertyName.ChainedOverride, PropertyHint.ResourceType, "AttachDataRes", PropertyUsageFlags.Default | PropertyUsageFlags.ScriptVariable, exported: true),
			new PropertyInfo(Variant.Type.Nil, "SPAWN CHAIN", PropertyHint.None, "", PropertyUsageFlags.Group, exported: true),
			new PropertyInfo(Variant.Type.Float, PropertyName.chanceOfItem, PropertyHint.None, "", PropertyUsageFlags.Default | PropertyUsageFlags.ScriptVariable, exported: true),
			new PropertyInfo(Variant.Type.Array, PropertyName.possibleItems, PropertyHint.TypeString, "24/17:ItemDataRes", PropertyUsageFlags.Default | PropertyUsageFlags.ScriptVariable, exported: true),
			new PropertyInfo(Variant.Type.Nil, "SPECIAL FLAGS", PropertyHint.None, "", PropertyUsageFlags.Group, exported: true),
			new PropertyInfo(Variant.Type.Bool, PropertyName.overrideTaskbar, PropertyHint.None, "", PropertyUsageFlags.Default | PropertyUsageFlags.ScriptVariable, exported: true),
			new PropertyInfo(Variant.Type.Bool, PropertyName.excludePopup, PropertyHint.None, "", PropertyUsageFlags.Default | PropertyUsageFlags.ScriptVariable, exported: true),
			new PropertyInfo(Variant.Type.Bool, PropertyName.movesActorAttachment, PropertyHint.None, "", PropertyUsageFlags.Default | PropertyUsageFlags.ScriptVariable, exported: true),
			new PropertyInfo(Variant.Type.Nil, "Moving Override", PropertyHint.None, "", PropertyUsageFlags.Subgroup, exported: true),
			new PropertyInfo(Variant.Type.Float, PropertyName.movementDuration, PropertyHint.None, "", PropertyUsageFlags.Default | PropertyUsageFlags.ScriptVariable, exported: true),
			new PropertyInfo(Variant.Type.Vector2, PropertyName.ForcedIdleTimeRange, PropertyHint.None, "", PropertyUsageFlags.Default | PropertyUsageFlags.ScriptVariable, exported: true),
			new PropertyInfo(Variant.Type.Vector2, PropertyName.ForcedWalkTimeRange, PropertyHint.None, "", PropertyUsageFlags.Default | PropertyUsageFlags.ScriptVariable, exported: true),
			new PropertyInfo(Variant.Type.Float, PropertyName.ForcedWalkSpeed, PropertyHint.None, "", PropertyUsageFlags.Default | PropertyUsageFlags.ScriptVariable, exported: true),
			new PropertyInfo(Variant.Type.Nil, "URL Popup", PropertyHint.None, "", PropertyUsageFlags.Subgroup, exported: true),
			new PropertyInfo(Variant.Type.String, PropertyName.popupURL, PropertyHint.None, "", PropertyUsageFlags.Default | PropertyUsageFlags.ScriptVariable, exported: true),
			new PropertyInfo(Variant.Type.Nil, "Kink Settings", PropertyHint.None, "", PropertyUsageFlags.Group, exported: true),
			new PropertyInfo(Variant.Type.Array, PropertyName.taggedKinks, PropertyHint.TypeString, "2/2:UNTYPED,CUCKING", PropertyUsageFlags.Default | PropertyUsageFlags.ScriptVariable, exported: true)
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
		StringName name3 = PropertyName.itemVariation;
		int from2 = itemVariation;
		info.AddProperty(name3, Variant.From(in from2));
		info.AddProperty(PropertyName._itemID, Variant.From(in _itemID));
		info.AddProperty(PropertyName._name, Variant.From(in _name));
		info.AddProperty(PropertyName.attachmentTyping, Variant.From(in attachmentTyping));
		info.AddProperty(PropertyName.attachmentScale, Variant.From(in attachmentScale));
		info.AddProperty(PropertyName.attachmentMargin, Variant.From(in attachmentMargin));
		info.AddProperty(PropertyName.attachmentAppeanceWeight, Variant.From(in attachmentAppeanceWeight));
		info.AddProperty(PropertyName.attachmentImageSize, Variant.From(in attachmentImageSize));
		info.AddProperty(PropertyName.attachmentAnimations, Variant.CreateFrom(attachmentAnimations));
		info.AddProperty(PropertyName.isKillOnRandomTimer, Variant.From(in isKillOnRandomTimer));
		info.AddProperty(PropertyName.randomKillOnTime, Variant.From(in randomKillOnTime));
		info.AddProperty(PropertyName.dialogueStack, Variant.CreateFrom(dialogueStack));
		info.AddProperty(PropertyName.attachmentStack, Variant.CreateFrom(attachmentStack));
		info.AddProperty(PropertyName.TagPair, Variant.CreateFrom(TagPair));
		info.AddProperty(PropertyName.RequiredTags, Variant.CreateFrom(RequiredTags));
		info.AddProperty(PropertyName.ChainedOverride, Variant.From(in ChainedOverride));
		info.AddProperty(PropertyName.chanceOfItem, Variant.From(in chanceOfItem));
		info.AddProperty(PropertyName.possibleItems, Variant.CreateFrom(possibleItems));
		info.AddProperty(PropertyName.overrideTaskbar, Variant.From(in overrideTaskbar));
		info.AddProperty(PropertyName.excludePopup, Variant.From(in excludePopup));
		info.AddProperty(PropertyName.movesActorAttachment, Variant.From(in movesActorAttachment));
		info.AddProperty(PropertyName.movementDuration, Variant.From(in movementDuration));
		info.AddProperty(PropertyName.ForcedIdleTimeRange, Variant.From(in ForcedIdleTimeRange));
		info.AddProperty(PropertyName.ForcedWalkTimeRange, Variant.From(in ForcedWalkTimeRange));
		info.AddProperty(PropertyName.ForcedWalkSpeed, Variant.From(in ForcedWalkSpeed));
		info.AddProperty(PropertyName.popupURL, Variant.From(in popupURL));
		info.AddProperty(PropertyName.taggedKinks, Variant.CreateFrom(taggedKinks));
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
		if (info.TryGetProperty(PropertyName.itemVariation, out var value3))
		{
			itemVariation = value3.As<int>();
		}
		if (info.TryGetProperty(PropertyName._itemID, out var value4))
		{
			_itemID = value4.As<string>();
		}
		if (info.TryGetProperty(PropertyName._name, out var value5))
		{
			_name = value5.As<string>();
		}
		if (info.TryGetProperty(PropertyName.attachmentTyping, out var value6))
		{
			attachmentTyping = value6.As<AttachmentType>();
		}
		if (info.TryGetProperty(PropertyName.attachmentScale, out var value7))
		{
			attachmentScale = value7.As<Vector2>();
		}
		if (info.TryGetProperty(PropertyName.attachmentMargin, out var value8))
		{
			attachmentMargin = value8.As<Vector2>();
		}
		if (info.TryGetProperty(PropertyName.attachmentAppeanceWeight, out var value9))
		{
			attachmentAppeanceWeight = value9.As<float>();
		}
		if (info.TryGetProperty(PropertyName.attachmentImageSize, out var value10))
		{
			attachmentImageSize = value10.As<Vector2I>();
		}
		if (info.TryGetProperty(PropertyName.attachmentAnimations, out var value11))
		{
			attachmentAnimations = value11.AsGodotArray<SpriteFrames>();
		}
		if (info.TryGetProperty(PropertyName.isKillOnRandomTimer, out var value12))
		{
			isKillOnRandomTimer = value12.As<bool>();
		}
		if (info.TryGetProperty(PropertyName.randomKillOnTime, out var value13))
		{
			randomKillOnTime = value13.As<Vector2>();
		}
		if (info.TryGetProperty(PropertyName.dialogueStack, out var value14))
		{
			dialogueStack = value14.AsGodotDictionary<int, DialogueDataRes>();
		}
		if (info.TryGetProperty(PropertyName.attachmentStack, out var value15))
		{
			attachmentStack = value15.AsGodotDictionary<int, AttachDataRes>();
		}
		if (info.TryGetProperty(PropertyName.TagPair, out var value16))
		{
			TagPair = value16.AsGodotArray<TagDataRes>();
		}
		if (info.TryGetProperty(PropertyName.RequiredTags, out var value17))
		{
			RequiredTags = value17.AsGodotArray<TagDataRes>();
		}
		if (info.TryGetProperty(PropertyName.ChainedOverride, out var value18))
		{
			ChainedOverride = value18.As<AttachDataRes>();
		}
		if (info.TryGetProperty(PropertyName.chanceOfItem, out var value19))
		{
			chanceOfItem = value19.As<float>();
		}
		if (info.TryGetProperty(PropertyName.possibleItems, out var value20))
		{
			possibleItems = value20.AsGodotArray<ItemDataRes>();
		}
		if (info.TryGetProperty(PropertyName.overrideTaskbar, out var value21))
		{
			overrideTaskbar = value21.As<bool>();
		}
		if (info.TryGetProperty(PropertyName.excludePopup, out var value22))
		{
			excludePopup = value22.As<bool>();
		}
		if (info.TryGetProperty(PropertyName.movesActorAttachment, out var value23))
		{
			movesActorAttachment = value23.As<bool>();
		}
		if (info.TryGetProperty(PropertyName.movementDuration, out var value24))
		{
			movementDuration = value24.As<float>();
		}
		if (info.TryGetProperty(PropertyName.ForcedIdleTimeRange, out var value25))
		{
			ForcedIdleTimeRange = value25.As<Vector2>();
		}
		if (info.TryGetProperty(PropertyName.ForcedWalkTimeRange, out var value26))
		{
			ForcedWalkTimeRange = value26.As<Vector2>();
		}
		if (info.TryGetProperty(PropertyName.ForcedWalkSpeed, out var value27))
		{
			ForcedWalkSpeed = value27.As<float>();
		}
		if (info.TryGetProperty(PropertyName.popupURL, out var value28))
		{
			popupURL = value28.As<string>();
		}
		if (info.TryGetProperty(PropertyName.taggedKinks, out var value29))
		{
			taggedKinks = value29.AsGodotArray<SaveHandler.Kinks>();
		}
	}
}
