using System.Collections.Generic;
using System.ComponentModel;
using Godot;
using Godot.Bridge;
using Godot.Collections;
using Godot.NativeInterop;

[GlobalClass]
[Tool]
[ScriptPath("res://Scripts/DataResources/ItemDataRes.cs")]
public class ItemDataRes : Resource
{
	public enum ItemTask
	{
		UNTYPED = 0,
		RUN_ANIMATION = 1,
		RUN_ATTACH = 2,
		RUN_DIALOGUE = 3,
		SPAWN_ENEMY = 4,
		SPAWN_ITEM = 5,
		SPAWN_POPUP = 6,
		SPAWN_MINIGAME = 10,
		DESPAWN_SUB_ACTOR = 7,
		AGGRO_SUB_ACTOR = 8,
		ENEMY_SUB_ACTOR = 9
	}

	public new class MethodName : Resource.MethodName
	{
		public static readonly StringName generateItemID = "generateItemID";
	}

	public new class PropertyName : Resource.PropertyName
	{
		public static readonly StringName itemID = "itemID";

		public static readonly StringName Name = "Name";

		public static readonly StringName itemDescription = "itemDescription";

		public static readonly StringName itemVariation = "itemVariation";

		public static readonly StringName _itemID = "_itemID";

		public static readonly StringName _name = "_name";

		public static readonly StringName ItemAnimations = "ItemAnimations";

		public static readonly StringName itemSize = "itemSize";

		public static readonly StringName itemScale = "itemScale";

		public static readonly StringName itemOffset = "itemOffset";

		public static readonly StringName itemSpawnWeight = "itemSpawnWeight";

		public static readonly StringName itemPopupWeight = "itemPopupWeight";

		public static readonly StringName itemTasks = "itemTasks";

		public static readonly StringName isReusable = "isReusable";

		public static readonly StringName isUsableDroppedOn = "isUsableDroppedOn";

		public static readonly StringName AnimationTies = "AnimationTies";

		public static readonly StringName RandomTimerAmount = "RandomTimerAmount";

		public static readonly StringName attachedData = "attachedData";

		public static readonly StringName RandomAmountToAttach = "RandomAmountToAttach";

		public static readonly StringName dialogueStack = "dialogueStack";

		public static readonly StringName RandomDialogueAssignment = "RandomDialogueAssignment";

		public static readonly StringName possibleSpawnedActors = "possibleSpawnedActors";

		public static readonly StringName spawningItem = "spawningItem";

		public static readonly StringName ItemAmountToSpawn = "ItemAmountToSpawn";

		public static readonly StringName popupData = "popupData";

		public static readonly StringName RandomAmountToSpawn = "RandomAmountToSpawn";

		public static readonly StringName maxDuplicates = "maxDuplicates";

		public static readonly StringName minigameID = "minigameID";

		public static readonly StringName possibleCombinations = "possibleCombinations";

		public static readonly StringName TagPair = "TagPair";

		public static readonly StringName possiblePickUpDialogue = "possiblePickUpDialogue";

		public static readonly StringName PerchentChanceOfDialogue = "PerchentChanceOfDialogue";

		public static readonly StringName NontargetablePickup = "NontargetablePickup";

		public static readonly StringName NoPassivePickup = "NoPassivePickup";

		public static readonly StringName possibleUsableAIs = "possibleUsableAIs";

		public static readonly StringName taggedKinks = "taggedKinks";
	}

	public new class SignalName : Resource.SignalName
	{
	}

	public string _itemID;

	public string _name = "EMPTY TITLE";

	[Export(PropertyHint.None, "")]
	public Array<SpriteFrames> ItemAnimations = new Array<SpriteFrames>();

	[Export(PropertyHint.None, "")]
	public Vector2I itemSize = new Vector2I(256, 256);

	[Export(PropertyHint.None, "")]
	public Vector2 itemScale = new Vector2I(1, 1);

	[Export(PropertyHint.None, "")]
	public Vector2 itemOffset = new Vector2I(0, 0);

	[ExportSubgroup("Spawn Weights", "")]
	[Export(PropertyHint.None, "")]
	public double itemSpawnWeight;

	[Export(PropertyHint.None, "")]
	public double itemPopupWeight;

	[ExportSubgroup("", "")]
	[Export(PropertyHint.None, "")]
	public Array<ItemTask> itemTasks = new Array<ItemTask>();

	[Export(PropertyHint.None, "")]
	public bool isReusable;

	[Export(PropertyHint.None, "")]
	public bool isUsableDroppedOn;

	[ExportSubgroup("TASK - Actor Animation Variables", "")]
	[Export(PropertyHint.None, "")]
	public Array<string> AnimationTies = new Array<string>();

	[Export(PropertyHint.None, "")]
	public Vector2 RandomTimerAmount;

	[ExportSubgroup("TASK - Attachment Variables", "")]
	[Export(PropertyHint.None, "")]
	public Array<AttachDataRes> attachedData = new Array<AttachDataRes>();

	[Export(PropertyHint.None, "")]
	public int RandomAmountToAttach = -1;

	[ExportSubgroup("TASK - Dialogue Variables", "")]
	[Export(PropertyHint.None, "")]
	public Array<DialogueDataRes> dialogueStack = new Array<DialogueDataRes>();

	[Export(PropertyHint.None, "")]
	public bool RandomDialogueAssignment;

	[ExportSubgroup("TASK - Actor Spawn Variables", "")]
	[Export(PropertyHint.None, "")]
	public Array<CharacterInfoDataRes> possibleSpawnedActors = new Array<CharacterInfoDataRes>();

	[ExportSubgroup("TASK - Item Spawn Variables", "")]
	[Export(PropertyHint.None, "")]
	public Godot.Collections.Dictionary<ItemDataRes, float> spawningItem = new Godot.Collections.Dictionary<ItemDataRes, float>();

	[Export(PropertyHint.None, "")]
	public int ItemAmountToSpawn = 1;

	[ExportSubgroup("TASK - Popup Variables", "")]
	[Export(PropertyHint.None, "")]
	public Array<AttachDataRes> popupData = new Array<AttachDataRes>();

	[Export(PropertyHint.None, "")]
	public int RandomAmountToSpawn = -1;

	[Export(PropertyHint.None, "")]
	public int maxDuplicates = 3;

	[ExportSubgroup("TASK - Minigame Variables", "")]
	[Export(PropertyHint.None, "")]
	public string minigameID;

	[ExportSubgroup("Possible Combinations", "")]
	[Export(PropertyHint.None, "")]
	public Array<CombinationDataRes> possibleCombinations = new Array<CombinationDataRes>();

	[ExportSubgroup("Tagging System", "")]
	[Export(PropertyHint.None, "")]
	public Array<TagDataRes> TagPair = new Array<TagDataRes>();

	[ExportSubgroup("Pickup Dialogue", "")]
	[Export(PropertyHint.None, "")]
	public Array<DialogueDataRes> possiblePickUpDialogue = new Array<DialogueDataRes>();

	[Export(PropertyHint.None, "")]
	public float PerchentChanceOfDialogue = 25f;

	[ExportGroup("AI Settings", "")]
	[Export(PropertyHint.None, "")]
	public bool NontargetablePickup;

	[Export(PropertyHint.None, "")]
	public bool NoPassivePickup;

	[Export(PropertyHint.None, "")]
	public Array<AiItemDataRes> possibleUsableAIs = new Array<AiItemDataRes>();

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

	[Export(PropertyHint.MultilineText, "")]
	public string itemDescription { get; set; }

	[Export(PropertyHint.None, "")]
	public int itemVariation { get; set; }

	public ItemDataRes()
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
		if (name == PropertyName.itemDescription)
		{
			itemDescription = VariantUtils.ConvertTo<string>(in value);
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
		if (name == PropertyName.ItemAnimations)
		{
			ItemAnimations = VariantUtils.ConvertToArray<SpriteFrames>(in value);
			return true;
		}
		if (name == PropertyName.itemSize)
		{
			itemSize = VariantUtils.ConvertTo<Vector2I>(in value);
			return true;
		}
		if (name == PropertyName.itemScale)
		{
			itemScale = VariantUtils.ConvertTo<Vector2>(in value);
			return true;
		}
		if (name == PropertyName.itemOffset)
		{
			itemOffset = VariantUtils.ConvertTo<Vector2>(in value);
			return true;
		}
		if (name == PropertyName.itemSpawnWeight)
		{
			itemSpawnWeight = VariantUtils.ConvertTo<double>(in value);
			return true;
		}
		if (name == PropertyName.itemPopupWeight)
		{
			itemPopupWeight = VariantUtils.ConvertTo<double>(in value);
			return true;
		}
		if (name == PropertyName.itemTasks)
		{
			itemTasks = VariantUtils.ConvertToArray<ItemTask>(in value);
			return true;
		}
		if (name == PropertyName.isReusable)
		{
			isReusable = VariantUtils.ConvertTo<bool>(in value);
			return true;
		}
		if (name == PropertyName.isUsableDroppedOn)
		{
			isUsableDroppedOn = VariantUtils.ConvertTo<bool>(in value);
			return true;
		}
		if (name == PropertyName.AnimationTies)
		{
			AnimationTies = VariantUtils.ConvertToArray<string>(in value);
			return true;
		}
		if (name == PropertyName.RandomTimerAmount)
		{
			RandomTimerAmount = VariantUtils.ConvertTo<Vector2>(in value);
			return true;
		}
		if (name == PropertyName.attachedData)
		{
			attachedData = VariantUtils.ConvertToArray<AttachDataRes>(in value);
			return true;
		}
		if (name == PropertyName.RandomAmountToAttach)
		{
			RandomAmountToAttach = VariantUtils.ConvertTo<int>(in value);
			return true;
		}
		if (name == PropertyName.dialogueStack)
		{
			dialogueStack = VariantUtils.ConvertToArray<DialogueDataRes>(in value);
			return true;
		}
		if (name == PropertyName.RandomDialogueAssignment)
		{
			RandomDialogueAssignment = VariantUtils.ConvertTo<bool>(in value);
			return true;
		}
		if (name == PropertyName.possibleSpawnedActors)
		{
			possibleSpawnedActors = VariantUtils.ConvertToArray<CharacterInfoDataRes>(in value);
			return true;
		}
		if (name == PropertyName.spawningItem)
		{
			spawningItem = VariantUtils.ConvertToDictionary<ItemDataRes, float>(in value);
			return true;
		}
		if (name == PropertyName.ItemAmountToSpawn)
		{
			ItemAmountToSpawn = VariantUtils.ConvertTo<int>(in value);
			return true;
		}
		if (name == PropertyName.popupData)
		{
			popupData = VariantUtils.ConvertToArray<AttachDataRes>(in value);
			return true;
		}
		if (name == PropertyName.RandomAmountToSpawn)
		{
			RandomAmountToSpawn = VariantUtils.ConvertTo<int>(in value);
			return true;
		}
		if (name == PropertyName.maxDuplicates)
		{
			maxDuplicates = VariantUtils.ConvertTo<int>(in value);
			return true;
		}
		if (name == PropertyName.minigameID)
		{
			minigameID = VariantUtils.ConvertTo<string>(in value);
			return true;
		}
		if (name == PropertyName.possibleCombinations)
		{
			possibleCombinations = VariantUtils.ConvertToArray<CombinationDataRes>(in value);
			return true;
		}
		if (name == PropertyName.TagPair)
		{
			TagPair = VariantUtils.ConvertToArray<TagDataRes>(in value);
			return true;
		}
		if (name == PropertyName.possiblePickUpDialogue)
		{
			possiblePickUpDialogue = VariantUtils.ConvertToArray<DialogueDataRes>(in value);
			return true;
		}
		if (name == PropertyName.PerchentChanceOfDialogue)
		{
			PerchentChanceOfDialogue = VariantUtils.ConvertTo<float>(in value);
			return true;
		}
		if (name == PropertyName.NontargetablePickup)
		{
			NontargetablePickup = VariantUtils.ConvertTo<bool>(in value);
			return true;
		}
		if (name == PropertyName.NoPassivePickup)
		{
			NoPassivePickup = VariantUtils.ConvertTo<bool>(in value);
			return true;
		}
		if (name == PropertyName.possibleUsableAIs)
		{
			possibleUsableAIs = VariantUtils.ConvertToArray<AiItemDataRes>(in value);
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
		if (name == PropertyName.itemDescription)
		{
			from = itemDescription;
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
		if (name == PropertyName.ItemAnimations)
		{
			value = VariantUtils.CreateFromArray(ItemAnimations);
			return true;
		}
		if (name == PropertyName.itemSize)
		{
			value = VariantUtils.CreateFrom(in itemSize);
			return true;
		}
		if (name == PropertyName.itemScale)
		{
			value = VariantUtils.CreateFrom(in itemScale);
			return true;
		}
		if (name == PropertyName.itemOffset)
		{
			value = VariantUtils.CreateFrom(in itemOffset);
			return true;
		}
		if (name == PropertyName.itemSpawnWeight)
		{
			value = VariantUtils.CreateFrom(in itemSpawnWeight);
			return true;
		}
		if (name == PropertyName.itemPopupWeight)
		{
			value = VariantUtils.CreateFrom(in itemPopupWeight);
			return true;
		}
		if (name == PropertyName.itemTasks)
		{
			value = VariantUtils.CreateFromArray(itemTasks);
			return true;
		}
		if (name == PropertyName.isReusable)
		{
			value = VariantUtils.CreateFrom(in isReusable);
			return true;
		}
		if (name == PropertyName.isUsableDroppedOn)
		{
			value = VariantUtils.CreateFrom(in isUsableDroppedOn);
			return true;
		}
		if (name == PropertyName.AnimationTies)
		{
			value = VariantUtils.CreateFromArray(AnimationTies);
			return true;
		}
		if (name == PropertyName.RandomTimerAmount)
		{
			value = VariantUtils.CreateFrom(in RandomTimerAmount);
			return true;
		}
		if (name == PropertyName.attachedData)
		{
			value = VariantUtils.CreateFromArray(attachedData);
			return true;
		}
		if (name == PropertyName.RandomAmountToAttach)
		{
			value = VariantUtils.CreateFrom(in RandomAmountToAttach);
			return true;
		}
		if (name == PropertyName.dialogueStack)
		{
			value = VariantUtils.CreateFromArray(dialogueStack);
			return true;
		}
		if (name == PropertyName.RandomDialogueAssignment)
		{
			value = VariantUtils.CreateFrom(in RandomDialogueAssignment);
			return true;
		}
		if (name == PropertyName.possibleSpawnedActors)
		{
			value = VariantUtils.CreateFromArray(possibleSpawnedActors);
			return true;
		}
		if (name == PropertyName.spawningItem)
		{
			value = VariantUtils.CreateFromDictionary(spawningItem);
			return true;
		}
		if (name == PropertyName.ItemAmountToSpawn)
		{
			value = VariantUtils.CreateFrom(in ItemAmountToSpawn);
			return true;
		}
		if (name == PropertyName.popupData)
		{
			value = VariantUtils.CreateFromArray(popupData);
			return true;
		}
		if (name == PropertyName.RandomAmountToSpawn)
		{
			value = VariantUtils.CreateFrom(in RandomAmountToSpawn);
			return true;
		}
		if (name == PropertyName.maxDuplicates)
		{
			value = VariantUtils.CreateFrom(in maxDuplicates);
			return true;
		}
		if (name == PropertyName.minigameID)
		{
			value = VariantUtils.CreateFrom(in minigameID);
			return true;
		}
		if (name == PropertyName.possibleCombinations)
		{
			value = VariantUtils.CreateFromArray(possibleCombinations);
			return true;
		}
		if (name == PropertyName.TagPair)
		{
			value = VariantUtils.CreateFromArray(TagPair);
			return true;
		}
		if (name == PropertyName.possiblePickUpDialogue)
		{
			value = VariantUtils.CreateFromArray(possiblePickUpDialogue);
			return true;
		}
		if (name == PropertyName.PerchentChanceOfDialogue)
		{
			value = VariantUtils.CreateFrom(in PerchentChanceOfDialogue);
			return true;
		}
		if (name == PropertyName.NontargetablePickup)
		{
			value = VariantUtils.CreateFrom(in NontargetablePickup);
			return true;
		}
		if (name == PropertyName.NoPassivePickup)
		{
			value = VariantUtils.CreateFrom(in NoPassivePickup);
			return true;
		}
		if (name == PropertyName.possibleUsableAIs)
		{
			value = VariantUtils.CreateFromArray(possibleUsableAIs);
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
			new PropertyInfo(Variant.Type.String, PropertyName.itemDescription, PropertyHint.MultilineText, "", PropertyUsageFlags.Default | PropertyUsageFlags.ScriptVariable, exported: true),
			new PropertyInfo(Variant.Type.Int, PropertyName.itemVariation, PropertyHint.None, "", PropertyUsageFlags.Default | PropertyUsageFlags.ScriptVariable, exported: true),
			new PropertyInfo(Variant.Type.Array, PropertyName.ItemAnimations, PropertyHint.TypeString, "24/17:SpriteFrames", PropertyUsageFlags.Default | PropertyUsageFlags.ScriptVariable, exported: true),
			new PropertyInfo(Variant.Type.Vector2I, PropertyName.itemSize, PropertyHint.None, "", PropertyUsageFlags.Default | PropertyUsageFlags.ScriptVariable, exported: true),
			new PropertyInfo(Variant.Type.Vector2, PropertyName.itemScale, PropertyHint.None, "", PropertyUsageFlags.Default | PropertyUsageFlags.ScriptVariable, exported: true),
			new PropertyInfo(Variant.Type.Vector2, PropertyName.itemOffset, PropertyHint.None, "", PropertyUsageFlags.Default | PropertyUsageFlags.ScriptVariable, exported: true),
			new PropertyInfo(Variant.Type.Nil, "Spawn Weights", PropertyHint.None, "", PropertyUsageFlags.Subgroup, exported: true),
			new PropertyInfo(Variant.Type.Float, PropertyName.itemSpawnWeight, PropertyHint.None, "", PropertyUsageFlags.Default | PropertyUsageFlags.ScriptVariable, exported: true),
			new PropertyInfo(Variant.Type.Float, PropertyName.itemPopupWeight, PropertyHint.None, "", PropertyUsageFlags.Default | PropertyUsageFlags.ScriptVariable, exported: true),
			new PropertyInfo(Variant.Type.Nil, "", PropertyHint.None, "", PropertyUsageFlags.Subgroup, exported: true),
			new PropertyInfo(Variant.Type.Array, PropertyName.itemTasks, PropertyHint.TypeString, "2/2:UNTYPED:0,RUN_ANIMATION:1,RUN_ATTACH:2,RUN_DIALOGUE:3,SPAWN_ENEMY:4,SPAWN_ITEM:5,SPAWN_POPUP:6,SPAWN_MINIGAME:10,DESPAWN_SUB_ACTOR:7,AGGRO_SUB_ACTOR:8,ENEMY_SUB_ACTOR:9", PropertyUsageFlags.Default | PropertyUsageFlags.ScriptVariable, exported: true),
			new PropertyInfo(Variant.Type.Bool, PropertyName.isReusable, PropertyHint.None, "", PropertyUsageFlags.Default | PropertyUsageFlags.ScriptVariable, exported: true),
			new PropertyInfo(Variant.Type.Bool, PropertyName.isUsableDroppedOn, PropertyHint.None, "", PropertyUsageFlags.Default | PropertyUsageFlags.ScriptVariable, exported: true),
			new PropertyInfo(Variant.Type.Nil, "TASK - Actor Animation Variables", PropertyHint.None, "", PropertyUsageFlags.Subgroup, exported: true),
			new PropertyInfo(Variant.Type.Array, PropertyName.AnimationTies, PropertyHint.TypeString, "4/0:", PropertyUsageFlags.Default | PropertyUsageFlags.ScriptVariable, exported: true),
			new PropertyInfo(Variant.Type.Vector2, PropertyName.RandomTimerAmount, PropertyHint.None, "", PropertyUsageFlags.Default | PropertyUsageFlags.ScriptVariable, exported: true),
			new PropertyInfo(Variant.Type.Nil, "TASK - Attachment Variables", PropertyHint.None, "", PropertyUsageFlags.Subgroup, exported: true),
			new PropertyInfo(Variant.Type.Array, PropertyName.attachedData, PropertyHint.TypeString, "24/17:AttachDataRes", PropertyUsageFlags.Default | PropertyUsageFlags.ScriptVariable, exported: true),
			new PropertyInfo(Variant.Type.Int, PropertyName.RandomAmountToAttach, PropertyHint.None, "", PropertyUsageFlags.Default | PropertyUsageFlags.ScriptVariable, exported: true),
			new PropertyInfo(Variant.Type.Nil, "TASK - Dialogue Variables", PropertyHint.None, "", PropertyUsageFlags.Subgroup, exported: true),
			new PropertyInfo(Variant.Type.Array, PropertyName.dialogueStack, PropertyHint.TypeString, "24/17:DialogueDataRes", PropertyUsageFlags.Default | PropertyUsageFlags.ScriptVariable, exported: true),
			new PropertyInfo(Variant.Type.Bool, PropertyName.RandomDialogueAssignment, PropertyHint.None, "", PropertyUsageFlags.Default | PropertyUsageFlags.ScriptVariable, exported: true),
			new PropertyInfo(Variant.Type.Nil, "TASK - Actor Spawn Variables", PropertyHint.None, "", PropertyUsageFlags.Subgroup, exported: true),
			new PropertyInfo(Variant.Type.Array, PropertyName.possibleSpawnedActors, PropertyHint.TypeString, "24/17:CharacterInfoDataRes", PropertyUsageFlags.Default | PropertyUsageFlags.ScriptVariable, exported: true),
			new PropertyInfo(Variant.Type.Nil, "TASK - Item Spawn Variables", PropertyHint.None, "", PropertyUsageFlags.Subgroup, exported: true),
			new PropertyInfo(Variant.Type.Dictionary, PropertyName.spawningItem, PropertyHint.TypeString, "24/17:ItemDataRes;3/0:", PropertyUsageFlags.Default | PropertyUsageFlags.ScriptVariable, exported: true),
			new PropertyInfo(Variant.Type.Int, PropertyName.ItemAmountToSpawn, PropertyHint.None, "", PropertyUsageFlags.Default | PropertyUsageFlags.ScriptVariable, exported: true),
			new PropertyInfo(Variant.Type.Nil, "TASK - Popup Variables", PropertyHint.None, "", PropertyUsageFlags.Subgroup, exported: true),
			new PropertyInfo(Variant.Type.Array, PropertyName.popupData, PropertyHint.TypeString, "24/17:AttachDataRes", PropertyUsageFlags.Default | PropertyUsageFlags.ScriptVariable, exported: true),
			new PropertyInfo(Variant.Type.Int, PropertyName.RandomAmountToSpawn, PropertyHint.None, "", PropertyUsageFlags.Default | PropertyUsageFlags.ScriptVariable, exported: true),
			new PropertyInfo(Variant.Type.Int, PropertyName.maxDuplicates, PropertyHint.None, "", PropertyUsageFlags.Default | PropertyUsageFlags.ScriptVariable, exported: true),
			new PropertyInfo(Variant.Type.Nil, "TASK - Minigame Variables", PropertyHint.None, "", PropertyUsageFlags.Subgroup, exported: true),
			new PropertyInfo(Variant.Type.String, PropertyName.minigameID, PropertyHint.None, "", PropertyUsageFlags.Default | PropertyUsageFlags.ScriptVariable, exported: true),
			new PropertyInfo(Variant.Type.Nil, "Possible Combinations", PropertyHint.None, "", PropertyUsageFlags.Subgroup, exported: true),
			new PropertyInfo(Variant.Type.Array, PropertyName.possibleCombinations, PropertyHint.TypeString, "24/17:CombinationDataRes", PropertyUsageFlags.Default | PropertyUsageFlags.ScriptVariable, exported: true),
			new PropertyInfo(Variant.Type.Nil, "Tagging System", PropertyHint.None, "", PropertyUsageFlags.Subgroup, exported: true),
			new PropertyInfo(Variant.Type.Array, PropertyName.TagPair, PropertyHint.TypeString, "24/17:TagDataRes", PropertyUsageFlags.Default | PropertyUsageFlags.ScriptVariable, exported: true),
			new PropertyInfo(Variant.Type.Nil, "Pickup Dialogue", PropertyHint.None, "", PropertyUsageFlags.Subgroup, exported: true),
			new PropertyInfo(Variant.Type.Array, PropertyName.possiblePickUpDialogue, PropertyHint.TypeString, "24/17:DialogueDataRes", PropertyUsageFlags.Default | PropertyUsageFlags.ScriptVariable, exported: true),
			new PropertyInfo(Variant.Type.Float, PropertyName.PerchentChanceOfDialogue, PropertyHint.None, "", PropertyUsageFlags.Default | PropertyUsageFlags.ScriptVariable, exported: true),
			new PropertyInfo(Variant.Type.Nil, "AI Settings", PropertyHint.None, "", PropertyUsageFlags.Group, exported: true),
			new PropertyInfo(Variant.Type.Bool, PropertyName.NontargetablePickup, PropertyHint.None, "", PropertyUsageFlags.Default | PropertyUsageFlags.ScriptVariable, exported: true),
			new PropertyInfo(Variant.Type.Bool, PropertyName.NoPassivePickup, PropertyHint.None, "", PropertyUsageFlags.Default | PropertyUsageFlags.ScriptVariable, exported: true),
			new PropertyInfo(Variant.Type.Array, PropertyName.possibleUsableAIs, PropertyHint.TypeString, "24/17:AiItemDataRes", PropertyUsageFlags.Default | PropertyUsageFlags.ScriptVariable, exported: true),
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
		StringName name3 = PropertyName.itemDescription;
		from = itemDescription;
		info.AddProperty(name3, Variant.From(in from));
		StringName name4 = PropertyName.itemVariation;
		int from2 = itemVariation;
		info.AddProperty(name4, Variant.From(in from2));
		info.AddProperty(PropertyName._itemID, Variant.From(in _itemID));
		info.AddProperty(PropertyName._name, Variant.From(in _name));
		info.AddProperty(PropertyName.ItemAnimations, Variant.CreateFrom(ItemAnimations));
		info.AddProperty(PropertyName.itemSize, Variant.From(in itemSize));
		info.AddProperty(PropertyName.itemScale, Variant.From(in itemScale));
		info.AddProperty(PropertyName.itemOffset, Variant.From(in itemOffset));
		info.AddProperty(PropertyName.itemSpawnWeight, Variant.From(in itemSpawnWeight));
		info.AddProperty(PropertyName.itemPopupWeight, Variant.From(in itemPopupWeight));
		info.AddProperty(PropertyName.itemTasks, Variant.CreateFrom(itemTasks));
		info.AddProperty(PropertyName.isReusable, Variant.From(in isReusable));
		info.AddProperty(PropertyName.isUsableDroppedOn, Variant.From(in isUsableDroppedOn));
		info.AddProperty(PropertyName.AnimationTies, Variant.CreateFrom(AnimationTies));
		info.AddProperty(PropertyName.RandomTimerAmount, Variant.From(in RandomTimerAmount));
		info.AddProperty(PropertyName.attachedData, Variant.CreateFrom(attachedData));
		info.AddProperty(PropertyName.RandomAmountToAttach, Variant.From(in RandomAmountToAttach));
		info.AddProperty(PropertyName.dialogueStack, Variant.CreateFrom(dialogueStack));
		info.AddProperty(PropertyName.RandomDialogueAssignment, Variant.From(in RandomDialogueAssignment));
		info.AddProperty(PropertyName.possibleSpawnedActors, Variant.CreateFrom(possibleSpawnedActors));
		info.AddProperty(PropertyName.spawningItem, Variant.CreateFrom(spawningItem));
		info.AddProperty(PropertyName.ItemAmountToSpawn, Variant.From(in ItemAmountToSpawn));
		info.AddProperty(PropertyName.popupData, Variant.CreateFrom(popupData));
		info.AddProperty(PropertyName.RandomAmountToSpawn, Variant.From(in RandomAmountToSpawn));
		info.AddProperty(PropertyName.maxDuplicates, Variant.From(in maxDuplicates));
		info.AddProperty(PropertyName.minigameID, Variant.From(in minigameID));
		info.AddProperty(PropertyName.possibleCombinations, Variant.CreateFrom(possibleCombinations));
		info.AddProperty(PropertyName.TagPair, Variant.CreateFrom(TagPair));
		info.AddProperty(PropertyName.possiblePickUpDialogue, Variant.CreateFrom(possiblePickUpDialogue));
		info.AddProperty(PropertyName.PerchentChanceOfDialogue, Variant.From(in PerchentChanceOfDialogue));
		info.AddProperty(PropertyName.NontargetablePickup, Variant.From(in NontargetablePickup));
		info.AddProperty(PropertyName.NoPassivePickup, Variant.From(in NoPassivePickup));
		info.AddProperty(PropertyName.possibleUsableAIs, Variant.CreateFrom(possibleUsableAIs));
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
		if (info.TryGetProperty(PropertyName.itemDescription, out var value3))
		{
			itemDescription = value3.As<string>();
		}
		if (info.TryGetProperty(PropertyName.itemVariation, out var value4))
		{
			itemVariation = value4.As<int>();
		}
		if (info.TryGetProperty(PropertyName._itemID, out var value5))
		{
			_itemID = value5.As<string>();
		}
		if (info.TryGetProperty(PropertyName._name, out var value6))
		{
			_name = value6.As<string>();
		}
		if (info.TryGetProperty(PropertyName.ItemAnimations, out var value7))
		{
			ItemAnimations = value7.AsGodotArray<SpriteFrames>();
		}
		if (info.TryGetProperty(PropertyName.itemSize, out var value8))
		{
			itemSize = value8.As<Vector2I>();
		}
		if (info.TryGetProperty(PropertyName.itemScale, out var value9))
		{
			itemScale = value9.As<Vector2>();
		}
		if (info.TryGetProperty(PropertyName.itemOffset, out var value10))
		{
			itemOffset = value10.As<Vector2>();
		}
		if (info.TryGetProperty(PropertyName.itemSpawnWeight, out var value11))
		{
			itemSpawnWeight = value11.As<double>();
		}
		if (info.TryGetProperty(PropertyName.itemPopupWeight, out var value12))
		{
			itemPopupWeight = value12.As<double>();
		}
		if (info.TryGetProperty(PropertyName.itemTasks, out var value13))
		{
			itemTasks = value13.AsGodotArray<ItemTask>();
		}
		if (info.TryGetProperty(PropertyName.isReusable, out var value14))
		{
			isReusable = value14.As<bool>();
		}
		if (info.TryGetProperty(PropertyName.isUsableDroppedOn, out var value15))
		{
			isUsableDroppedOn = value15.As<bool>();
		}
		if (info.TryGetProperty(PropertyName.AnimationTies, out var value16))
		{
			AnimationTies = value16.AsGodotArray<string>();
		}
		if (info.TryGetProperty(PropertyName.RandomTimerAmount, out var value17))
		{
			RandomTimerAmount = value17.As<Vector2>();
		}
		if (info.TryGetProperty(PropertyName.attachedData, out var value18))
		{
			attachedData = value18.AsGodotArray<AttachDataRes>();
		}
		if (info.TryGetProperty(PropertyName.RandomAmountToAttach, out var value19))
		{
			RandomAmountToAttach = value19.As<int>();
		}
		if (info.TryGetProperty(PropertyName.dialogueStack, out var value20))
		{
			dialogueStack = value20.AsGodotArray<DialogueDataRes>();
		}
		if (info.TryGetProperty(PropertyName.RandomDialogueAssignment, out var value21))
		{
			RandomDialogueAssignment = value21.As<bool>();
		}
		if (info.TryGetProperty(PropertyName.possibleSpawnedActors, out var value22))
		{
			possibleSpawnedActors = value22.AsGodotArray<CharacterInfoDataRes>();
		}
		if (info.TryGetProperty(PropertyName.spawningItem, out var value23))
		{
			spawningItem = value23.AsGodotDictionary<ItemDataRes, float>();
		}
		if (info.TryGetProperty(PropertyName.ItemAmountToSpawn, out var value24))
		{
			ItemAmountToSpawn = value24.As<int>();
		}
		if (info.TryGetProperty(PropertyName.popupData, out var value25))
		{
			popupData = value25.AsGodotArray<AttachDataRes>();
		}
		if (info.TryGetProperty(PropertyName.RandomAmountToSpawn, out var value26))
		{
			RandomAmountToSpawn = value26.As<int>();
		}
		if (info.TryGetProperty(PropertyName.maxDuplicates, out var value27))
		{
			maxDuplicates = value27.As<int>();
		}
		if (info.TryGetProperty(PropertyName.minigameID, out var value28))
		{
			minigameID = value28.As<string>();
		}
		if (info.TryGetProperty(PropertyName.possibleCombinations, out var value29))
		{
			possibleCombinations = value29.AsGodotArray<CombinationDataRes>();
		}
		if (info.TryGetProperty(PropertyName.TagPair, out var value30))
		{
			TagPair = value30.AsGodotArray<TagDataRes>();
		}
		if (info.TryGetProperty(PropertyName.possiblePickUpDialogue, out var value31))
		{
			possiblePickUpDialogue = value31.AsGodotArray<DialogueDataRes>();
		}
		if (info.TryGetProperty(PropertyName.PerchentChanceOfDialogue, out var value32))
		{
			PerchentChanceOfDialogue = value32.As<float>();
		}
		if (info.TryGetProperty(PropertyName.NontargetablePickup, out var value33))
		{
			NontargetablePickup = value33.As<bool>();
		}
		if (info.TryGetProperty(PropertyName.NoPassivePickup, out var value34))
		{
			NoPassivePickup = value34.As<bool>();
		}
		if (info.TryGetProperty(PropertyName.possibleUsableAIs, out var value35))
		{
			possibleUsableAIs = value35.AsGodotArray<AiItemDataRes>();
		}
		if (info.TryGetProperty(PropertyName.taggedKinks, out var value36))
		{
			taggedKinks = value36.AsGodotArray<SaveHandler.Kinks>();
		}
	}
}
