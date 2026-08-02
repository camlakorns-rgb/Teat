using System.Collections.Generic;
using System.ComponentModel;
using Godot;
using Godot.Bridge;
using Godot.Collections;
using Godot.NativeInterop;

[GlobalClass]
[Tool]
[ScriptPath("res://Scripts/DataResources/CharacterInfoDataRes.cs")]
public class CharacterInfoDataRes : Resource
{
	public enum ResponseToSituation
	{
		UNTYPED,
		COMPANION_LIMIT,
		ITEM_LIMIT,
		LOCKED_TO_MONITOR,
		UNLOCKED_FROM_MONITOR,
		COMPANION_KILLED,
		IN_CONVO,
		IN_OVERRIDE,
		THROWN_SOFT,
		THROWN_HARD,
		LAND_SOFT,
		LAND_HARD,
		MINIGAME_FAIL,
		MINIGAME_OK,
		MINIGAME_GOOD,
		MINIGAME_GREAT,
		MINIGAME_PERFECT
	}

	public enum AITypes
	{
		UNTYPED,
		ENEMY,
		COMPANION
	}

	public enum EnemySubTypes
	{
		ENEMY_BASIC,
		ENEMY_SUMMONER
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

		public static readonly StringName characterAnimationLayers = "characterAnimationLayers";

		public static readonly StringName ClothingStates = "ClothingStates";

		public static readonly StringName characterSize = "characterSize";

		public static readonly StringName characterScale = "characterScale";

		public static readonly StringName characterOffset = "characterOffset";

		public static readonly StringName characterColor = "characterColor";

		public static readonly StringName interactionTexts = "interactionTexts";

		public static readonly StringName randomTexts = "randomTexts";

		public static readonly StringName randomDialogueTimer = "randomDialogueTimer";

		public static readonly StringName randomAnimations = "randomAnimations";

		public static readonly StringName randomAnimationTimer = "randomAnimationTimer";

		public static readonly StringName firstTimeStartupMessage = "firstTimeStartupMessage";

		public static readonly StringName welcomeMessages = "welcomeMessages";

		public static readonly StringName flipSpriteH = "flipSpriteH";

		public static readonly StringName WalkSpeed = "WalkSpeed";

		public static readonly StringName responseTexts = "responseTexts";

		public static readonly StringName possibleStateOverride = "possibleStateOverride";

		public static readonly StringName AITyping = "AITyping";

		public static readonly StringName actorSpawnWeight = "actorSpawnWeight";

		public static readonly StringName overrideAnimation = "overrideAnimation";

		public static readonly StringName spawnsItems = "spawnsItems";

		public static readonly StringName itemSpawnRate = "itemSpawnRate";

		public static readonly StringName Glitchy = "Glitchy";

		public static readonly StringName isAggroActor = "isAggroActor";

		public static readonly StringName aggroTimerRange = "aggroTimerRange";

		public static readonly StringName SpeicalAnimations = "SpeicalAnimations";

		public static readonly StringName UnBlockable = "UnBlockable";

		public static readonly StringName AvoidMouse = "AvoidMouse";

		public static readonly StringName enemySubType = "enemySubType";

		public static readonly StringName taggedKinks = "taggedKinks";
	}

	public new class SignalName : Resource.SignalName
	{
	}

	public string _itemID;

	public string _name = "EMPTY TITLE";

	[ExportSubgroup("Sprite Information", "")]
	[Export(PropertyHint.None, "")]
	public Array<CharAnimDataRes> characterAnimationLayers = new Array<CharAnimDataRes>();

	[Export(PropertyHint.None, "")]
	public Array<string> ClothingStates = new Array<string> { "naked" };

	[Export(PropertyHint.None, "")]
	public Vector2I characterSize = new Vector2I(256, 256);

	[Export(PropertyHint.None, "")]
	public Vector2 characterScale = new Vector2I(1, 1);

	[Export(PropertyHint.None, "")]
	public Vector2 characterOffset = new Vector2I(0, 0);

	[ExportSubgroup("Dialogue Information", "")]
	[Export(PropertyHint.None, "")]
	public Color characterColor = new Color("ffffff");

	[Export(PropertyHint.None, "")]
	public Array<DialogueDataRes> interactionTexts = new Array<DialogueDataRes>();

	[Export(PropertyHint.None, "")]
	public Array<ConvoDataRes> randomTexts = new Array<ConvoDataRes>();

	[Export(PropertyHint.None, "")]
	public Vector2 randomDialogueTimer = new Vector2(30f, 60f);

	[ExportSubgroup("Animation Information", "")]
	[Export(PropertyHint.None, "")]
	public Array<AnimDataRes> randomAnimations = new Array<AnimDataRes>();

	[Export(PropertyHint.None, "")]
	public Vector2 randomAnimationTimer = new Vector2(30f, 60f);

	[ExportGroup("Main Actor Specific Settings", "")]
	[Export(PropertyHint.None, "")]
	public DialogueDataRes firstTimeStartupMessage;

	[Export(PropertyHint.None, "")]
	public Array<DialogueDataRes> welcomeMessages = new Array<DialogueDataRes>();

	[ExportGroup("Shared Settings", "")]
	[Export(PropertyHint.None, "")]
	public bool flipSpriteH;

	[Export(PropertyHint.None, "")]
	public float WalkSpeed = 250f;

	[Export(PropertyHint.None, "")]
	public Godot.Collections.Dictionary<ResponseToSituation, DialogueDataRes> responseTexts = new Godot.Collections.Dictionary<ResponseToSituation, DialogueDataRes>();

	[Export(PropertyHint.None, "")]
	public Array<TagOverrideDataRes> possibleStateOverride = new Array<TagOverrideDataRes>();

	[ExportGroup("AI Settings", "")]
	[Export(PropertyHint.None, "")]
	public AITypes AITyping = AITypes.ENEMY;

	[Export(PropertyHint.None, "")]
	public double actorSpawnWeight = 25.0;

	[ExportSubgroup("Shared Settings", "")]
	[Export(PropertyHint.None, "")]
	public Array<AttachDataRes> overrideAnimation = new Array<AttachDataRes>();

	[Export(PropertyHint.None, "")]
	public bool spawnsItems;

	[Export(PropertyHint.None, "")]
	public Vector2 itemSpawnRate = new Vector2(30f, 60f);

	[Export(PropertyHint.None, "")]
	public bool Glitchy;

	[ExportSubgroup("Companion Settings", "")]
	[Export(PropertyHint.None, "")]
	public bool isAggroActor;

	[Export(PropertyHint.None, "")]
	public Vector2 aggroTimerRange = new Vector2(8f, 20f);

	[Export(PropertyHint.None, "")]
	public AnimDataRes SpeicalAnimations;

	[ExportSubgroup("Enemy Settings", "")]
	[Export(PropertyHint.None, "")]
	public bool UnBlockable;

	[Export(PropertyHint.None, "")]
	public bool AvoidMouse;

	[Export(PropertyHint.None, "")]
	public EnemySubTypes enemySubType;

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

	public CharacterInfoDataRes()
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
		if (name == PropertyName.characterAnimationLayers)
		{
			characterAnimationLayers = VariantUtils.ConvertToArray<CharAnimDataRes>(in value);
			return true;
		}
		if (name == PropertyName.ClothingStates)
		{
			ClothingStates = VariantUtils.ConvertToArray<string>(in value);
			return true;
		}
		if (name == PropertyName.characterSize)
		{
			characterSize = VariantUtils.ConvertTo<Vector2I>(in value);
			return true;
		}
		if (name == PropertyName.characterScale)
		{
			characterScale = VariantUtils.ConvertTo<Vector2>(in value);
			return true;
		}
		if (name == PropertyName.characterOffset)
		{
			characterOffset = VariantUtils.ConvertTo<Vector2>(in value);
			return true;
		}
		if (name == PropertyName.characterColor)
		{
			characterColor = VariantUtils.ConvertTo<Color>(in value);
			return true;
		}
		if (name == PropertyName.interactionTexts)
		{
			interactionTexts = VariantUtils.ConvertToArray<DialogueDataRes>(in value);
			return true;
		}
		if (name == PropertyName.randomTexts)
		{
			randomTexts = VariantUtils.ConvertToArray<ConvoDataRes>(in value);
			return true;
		}
		if (name == PropertyName.randomDialogueTimer)
		{
			randomDialogueTimer = VariantUtils.ConvertTo<Vector2>(in value);
			return true;
		}
		if (name == PropertyName.randomAnimations)
		{
			randomAnimations = VariantUtils.ConvertToArray<AnimDataRes>(in value);
			return true;
		}
		if (name == PropertyName.randomAnimationTimer)
		{
			randomAnimationTimer = VariantUtils.ConvertTo<Vector2>(in value);
			return true;
		}
		if (name == PropertyName.firstTimeStartupMessage)
		{
			firstTimeStartupMessage = VariantUtils.ConvertTo<DialogueDataRes>(in value);
			return true;
		}
		if (name == PropertyName.welcomeMessages)
		{
			welcomeMessages = VariantUtils.ConvertToArray<DialogueDataRes>(in value);
			return true;
		}
		if (name == PropertyName.flipSpriteH)
		{
			flipSpriteH = VariantUtils.ConvertTo<bool>(in value);
			return true;
		}
		if (name == PropertyName.WalkSpeed)
		{
			WalkSpeed = VariantUtils.ConvertTo<float>(in value);
			return true;
		}
		if (name == PropertyName.responseTexts)
		{
			responseTexts = VariantUtils.ConvertToDictionary<ResponseToSituation, DialogueDataRes>(in value);
			return true;
		}
		if (name == PropertyName.possibleStateOverride)
		{
			possibleStateOverride = VariantUtils.ConvertToArray<TagOverrideDataRes>(in value);
			return true;
		}
		if (name == PropertyName.AITyping)
		{
			AITyping = VariantUtils.ConvertTo<AITypes>(in value);
			return true;
		}
		if (name == PropertyName.actorSpawnWeight)
		{
			actorSpawnWeight = VariantUtils.ConvertTo<double>(in value);
			return true;
		}
		if (name == PropertyName.overrideAnimation)
		{
			overrideAnimation = VariantUtils.ConvertToArray<AttachDataRes>(in value);
			return true;
		}
		if (name == PropertyName.spawnsItems)
		{
			spawnsItems = VariantUtils.ConvertTo<bool>(in value);
			return true;
		}
		if (name == PropertyName.itemSpawnRate)
		{
			itemSpawnRate = VariantUtils.ConvertTo<Vector2>(in value);
			return true;
		}
		if (name == PropertyName.Glitchy)
		{
			Glitchy = VariantUtils.ConvertTo<bool>(in value);
			return true;
		}
		if (name == PropertyName.isAggroActor)
		{
			isAggroActor = VariantUtils.ConvertTo<bool>(in value);
			return true;
		}
		if (name == PropertyName.aggroTimerRange)
		{
			aggroTimerRange = VariantUtils.ConvertTo<Vector2>(in value);
			return true;
		}
		if (name == PropertyName.SpeicalAnimations)
		{
			SpeicalAnimations = VariantUtils.ConvertTo<AnimDataRes>(in value);
			return true;
		}
		if (name == PropertyName.UnBlockable)
		{
			UnBlockable = VariantUtils.ConvertTo<bool>(in value);
			return true;
		}
		if (name == PropertyName.AvoidMouse)
		{
			AvoidMouse = VariantUtils.ConvertTo<bool>(in value);
			return true;
		}
		if (name == PropertyName.enemySubType)
		{
			enemySubType = VariantUtils.ConvertTo<EnemySubTypes>(in value);
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
		if (name == PropertyName.characterAnimationLayers)
		{
			value = VariantUtils.CreateFromArray(characterAnimationLayers);
			return true;
		}
		if (name == PropertyName.ClothingStates)
		{
			value = VariantUtils.CreateFromArray(ClothingStates);
			return true;
		}
		if (name == PropertyName.characterSize)
		{
			value = VariantUtils.CreateFrom(in characterSize);
			return true;
		}
		if (name == PropertyName.characterScale)
		{
			value = VariantUtils.CreateFrom(in characterScale);
			return true;
		}
		if (name == PropertyName.characterOffset)
		{
			value = VariantUtils.CreateFrom(in characterOffset);
			return true;
		}
		if (name == PropertyName.characterColor)
		{
			value = VariantUtils.CreateFrom(in characterColor);
			return true;
		}
		if (name == PropertyName.interactionTexts)
		{
			value = VariantUtils.CreateFromArray(interactionTexts);
			return true;
		}
		if (name == PropertyName.randomTexts)
		{
			value = VariantUtils.CreateFromArray(randomTexts);
			return true;
		}
		if (name == PropertyName.randomDialogueTimer)
		{
			value = VariantUtils.CreateFrom(in randomDialogueTimer);
			return true;
		}
		if (name == PropertyName.randomAnimations)
		{
			value = VariantUtils.CreateFromArray(randomAnimations);
			return true;
		}
		if (name == PropertyName.randomAnimationTimer)
		{
			value = VariantUtils.CreateFrom(in randomAnimationTimer);
			return true;
		}
		if (name == PropertyName.firstTimeStartupMessage)
		{
			value = VariantUtils.CreateFrom(in firstTimeStartupMessage);
			return true;
		}
		if (name == PropertyName.welcomeMessages)
		{
			value = VariantUtils.CreateFromArray(welcomeMessages);
			return true;
		}
		if (name == PropertyName.flipSpriteH)
		{
			value = VariantUtils.CreateFrom(in flipSpriteH);
			return true;
		}
		if (name == PropertyName.WalkSpeed)
		{
			value = VariantUtils.CreateFrom(in WalkSpeed);
			return true;
		}
		if (name == PropertyName.responseTexts)
		{
			value = VariantUtils.CreateFromDictionary(responseTexts);
			return true;
		}
		if (name == PropertyName.possibleStateOverride)
		{
			value = VariantUtils.CreateFromArray(possibleStateOverride);
			return true;
		}
		if (name == PropertyName.AITyping)
		{
			value = VariantUtils.CreateFrom(in AITyping);
			return true;
		}
		if (name == PropertyName.actorSpawnWeight)
		{
			value = VariantUtils.CreateFrom(in actorSpawnWeight);
			return true;
		}
		if (name == PropertyName.overrideAnimation)
		{
			value = VariantUtils.CreateFromArray(overrideAnimation);
			return true;
		}
		if (name == PropertyName.spawnsItems)
		{
			value = VariantUtils.CreateFrom(in spawnsItems);
			return true;
		}
		if (name == PropertyName.itemSpawnRate)
		{
			value = VariantUtils.CreateFrom(in itemSpawnRate);
			return true;
		}
		if (name == PropertyName.Glitchy)
		{
			value = VariantUtils.CreateFrom(in Glitchy);
			return true;
		}
		if (name == PropertyName.isAggroActor)
		{
			value = VariantUtils.CreateFrom(in isAggroActor);
			return true;
		}
		if (name == PropertyName.aggroTimerRange)
		{
			value = VariantUtils.CreateFrom(in aggroTimerRange);
			return true;
		}
		if (name == PropertyName.SpeicalAnimations)
		{
			value = VariantUtils.CreateFrom(in SpeicalAnimations);
			return true;
		}
		if (name == PropertyName.UnBlockable)
		{
			value = VariantUtils.CreateFrom(in UnBlockable);
			return true;
		}
		if (name == PropertyName.AvoidMouse)
		{
			value = VariantUtils.CreateFrom(in AvoidMouse);
			return true;
		}
		if (name == PropertyName.enemySubType)
		{
			value = VariantUtils.CreateFrom(in enemySubType);
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
			new PropertyInfo(Variant.Type.Nil, "Sprite Information", PropertyHint.None, "", PropertyUsageFlags.Subgroup, exported: true),
			new PropertyInfo(Variant.Type.Array, PropertyName.characterAnimationLayers, PropertyHint.TypeString, "24/17:CharAnimDataRes", PropertyUsageFlags.Default | PropertyUsageFlags.ScriptVariable, exported: true),
			new PropertyInfo(Variant.Type.Array, PropertyName.ClothingStates, PropertyHint.TypeString, "4/0:", PropertyUsageFlags.Default | PropertyUsageFlags.ScriptVariable, exported: true),
			new PropertyInfo(Variant.Type.Vector2I, PropertyName.characterSize, PropertyHint.None, "", PropertyUsageFlags.Default | PropertyUsageFlags.ScriptVariable, exported: true),
			new PropertyInfo(Variant.Type.Vector2, PropertyName.characterScale, PropertyHint.None, "", PropertyUsageFlags.Default | PropertyUsageFlags.ScriptVariable, exported: true),
			new PropertyInfo(Variant.Type.Vector2, PropertyName.characterOffset, PropertyHint.None, "", PropertyUsageFlags.Default | PropertyUsageFlags.ScriptVariable, exported: true),
			new PropertyInfo(Variant.Type.Nil, "Dialogue Information", PropertyHint.None, "", PropertyUsageFlags.Subgroup, exported: true),
			new PropertyInfo(Variant.Type.Color, PropertyName.characterColor, PropertyHint.None, "", PropertyUsageFlags.Default | PropertyUsageFlags.ScriptVariable, exported: true),
			new PropertyInfo(Variant.Type.Array, PropertyName.interactionTexts, PropertyHint.TypeString, "24/17:DialogueDataRes", PropertyUsageFlags.Default | PropertyUsageFlags.ScriptVariable, exported: true),
			new PropertyInfo(Variant.Type.Array, PropertyName.randomTexts, PropertyHint.TypeString, "24/17:ConvoDataRes", PropertyUsageFlags.Default | PropertyUsageFlags.ScriptVariable, exported: true),
			new PropertyInfo(Variant.Type.Vector2, PropertyName.randomDialogueTimer, PropertyHint.None, "", PropertyUsageFlags.Default | PropertyUsageFlags.ScriptVariable, exported: true),
			new PropertyInfo(Variant.Type.Nil, "Animation Information", PropertyHint.None, "", PropertyUsageFlags.Subgroup, exported: true),
			new PropertyInfo(Variant.Type.Array, PropertyName.randomAnimations, PropertyHint.TypeString, "24/17:AnimDataRes", PropertyUsageFlags.Default | PropertyUsageFlags.ScriptVariable, exported: true),
			new PropertyInfo(Variant.Type.Vector2, PropertyName.randomAnimationTimer, PropertyHint.None, "", PropertyUsageFlags.Default | PropertyUsageFlags.ScriptVariable, exported: true),
			new PropertyInfo(Variant.Type.Nil, "Main Actor Specific Settings", PropertyHint.None, "", PropertyUsageFlags.Group, exported: true),
			new PropertyInfo(Variant.Type.Object, PropertyName.firstTimeStartupMessage, PropertyHint.ResourceType, "DialogueDataRes", PropertyUsageFlags.Default | PropertyUsageFlags.ScriptVariable, exported: true),
			new PropertyInfo(Variant.Type.Array, PropertyName.welcomeMessages, PropertyHint.TypeString, "24/17:DialogueDataRes", PropertyUsageFlags.Default | PropertyUsageFlags.ScriptVariable, exported: true),
			new PropertyInfo(Variant.Type.Nil, "Shared Settings", PropertyHint.None, "", PropertyUsageFlags.Group, exported: true),
			new PropertyInfo(Variant.Type.Bool, PropertyName.flipSpriteH, PropertyHint.None, "", PropertyUsageFlags.Default | PropertyUsageFlags.ScriptVariable, exported: true),
			new PropertyInfo(Variant.Type.Float, PropertyName.WalkSpeed, PropertyHint.None, "", PropertyUsageFlags.Default | PropertyUsageFlags.ScriptVariable, exported: true),
			new PropertyInfo(Variant.Type.Dictionary, PropertyName.responseTexts, PropertyHint.TypeString, "2/2:UNTYPED,COMPANION_LIMIT,ITEM_LIMIT,LOCKED_TO_MONITOR,UNLOCKED_FROM_MONITOR,COMPANION_KILLED,IN_CONVO,IN_OVERRIDE,THROWN_SOFT,THROWN_HARD,LAND_SOFT,LAND_HARD,MINIGAME_FAIL,MINIGAME_OK,MINIGAME_GOOD,MINIGAME_GREAT,MINIGAME_PERFECT;24/17:DialogueDataRes", PropertyUsageFlags.Default | PropertyUsageFlags.ScriptVariable, exported: true),
			new PropertyInfo(Variant.Type.Array, PropertyName.possibleStateOverride, PropertyHint.TypeString, "24/17:TagOverrideDataRes", PropertyUsageFlags.Default | PropertyUsageFlags.ScriptVariable, exported: true),
			new PropertyInfo(Variant.Type.Nil, "AI Settings", PropertyHint.None, "", PropertyUsageFlags.Group, exported: true),
			new PropertyInfo(Variant.Type.Int, PropertyName.AITyping, PropertyHint.Enum, "UNTYPED,ENEMY,COMPANION", PropertyUsageFlags.Default | PropertyUsageFlags.ScriptVariable, exported: true),
			new PropertyInfo(Variant.Type.Float, PropertyName.actorSpawnWeight, PropertyHint.None, "", PropertyUsageFlags.Default | PropertyUsageFlags.ScriptVariable, exported: true),
			new PropertyInfo(Variant.Type.Nil, "Shared Settings", PropertyHint.None, "", PropertyUsageFlags.Subgroup, exported: true),
			new PropertyInfo(Variant.Type.Array, PropertyName.overrideAnimation, PropertyHint.TypeString, "24/17:AttachDataRes", PropertyUsageFlags.Default | PropertyUsageFlags.ScriptVariable, exported: true),
			new PropertyInfo(Variant.Type.Bool, PropertyName.spawnsItems, PropertyHint.None, "", PropertyUsageFlags.Default | PropertyUsageFlags.ScriptVariable, exported: true),
			new PropertyInfo(Variant.Type.Vector2, PropertyName.itemSpawnRate, PropertyHint.None, "", PropertyUsageFlags.Default | PropertyUsageFlags.ScriptVariable, exported: true),
			new PropertyInfo(Variant.Type.Bool, PropertyName.Glitchy, PropertyHint.None, "", PropertyUsageFlags.Default | PropertyUsageFlags.ScriptVariable, exported: true),
			new PropertyInfo(Variant.Type.Nil, "Companion Settings", PropertyHint.None, "", PropertyUsageFlags.Subgroup, exported: true),
			new PropertyInfo(Variant.Type.Bool, PropertyName.isAggroActor, PropertyHint.None, "", PropertyUsageFlags.Default | PropertyUsageFlags.ScriptVariable, exported: true),
			new PropertyInfo(Variant.Type.Vector2, PropertyName.aggroTimerRange, PropertyHint.None, "", PropertyUsageFlags.Default | PropertyUsageFlags.ScriptVariable, exported: true),
			new PropertyInfo(Variant.Type.Object, PropertyName.SpeicalAnimations, PropertyHint.ResourceType, "AnimDataRes", PropertyUsageFlags.Default | PropertyUsageFlags.ScriptVariable, exported: true),
			new PropertyInfo(Variant.Type.Nil, "Enemy Settings", PropertyHint.None, "", PropertyUsageFlags.Subgroup, exported: true),
			new PropertyInfo(Variant.Type.Bool, PropertyName.UnBlockable, PropertyHint.None, "", PropertyUsageFlags.Default | PropertyUsageFlags.ScriptVariable, exported: true),
			new PropertyInfo(Variant.Type.Bool, PropertyName.AvoidMouse, PropertyHint.None, "", PropertyUsageFlags.Default | PropertyUsageFlags.ScriptVariable, exported: true),
			new PropertyInfo(Variant.Type.Int, PropertyName.enemySubType, PropertyHint.Enum, "ENEMY_BASIC,ENEMY_SUMMONER", PropertyUsageFlags.Default | PropertyUsageFlags.ScriptVariable, exported: true),
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
		info.AddProperty(PropertyName.characterAnimationLayers, Variant.CreateFrom(characterAnimationLayers));
		info.AddProperty(PropertyName.ClothingStates, Variant.CreateFrom(ClothingStates));
		info.AddProperty(PropertyName.characterSize, Variant.From(in characterSize));
		info.AddProperty(PropertyName.characterScale, Variant.From(in characterScale));
		info.AddProperty(PropertyName.characterOffset, Variant.From(in characterOffset));
		info.AddProperty(PropertyName.characterColor, Variant.From(in characterColor));
		info.AddProperty(PropertyName.interactionTexts, Variant.CreateFrom(interactionTexts));
		info.AddProperty(PropertyName.randomTexts, Variant.CreateFrom(randomTexts));
		info.AddProperty(PropertyName.randomDialogueTimer, Variant.From(in randomDialogueTimer));
		info.AddProperty(PropertyName.randomAnimations, Variant.CreateFrom(randomAnimations));
		info.AddProperty(PropertyName.randomAnimationTimer, Variant.From(in randomAnimationTimer));
		info.AddProperty(PropertyName.firstTimeStartupMessage, Variant.From(in firstTimeStartupMessage));
		info.AddProperty(PropertyName.welcomeMessages, Variant.CreateFrom(welcomeMessages));
		info.AddProperty(PropertyName.flipSpriteH, Variant.From(in flipSpriteH));
		info.AddProperty(PropertyName.WalkSpeed, Variant.From(in WalkSpeed));
		info.AddProperty(PropertyName.responseTexts, Variant.CreateFrom(responseTexts));
		info.AddProperty(PropertyName.possibleStateOverride, Variant.CreateFrom(possibleStateOverride));
		info.AddProperty(PropertyName.AITyping, Variant.From(in AITyping));
		info.AddProperty(PropertyName.actorSpawnWeight, Variant.From(in actorSpawnWeight));
		info.AddProperty(PropertyName.overrideAnimation, Variant.CreateFrom(overrideAnimation));
		info.AddProperty(PropertyName.spawnsItems, Variant.From(in spawnsItems));
		info.AddProperty(PropertyName.itemSpawnRate, Variant.From(in itemSpawnRate));
		info.AddProperty(PropertyName.Glitchy, Variant.From(in Glitchy));
		info.AddProperty(PropertyName.isAggroActor, Variant.From(in isAggroActor));
		info.AddProperty(PropertyName.aggroTimerRange, Variant.From(in aggroTimerRange));
		info.AddProperty(PropertyName.SpeicalAnimations, Variant.From(in SpeicalAnimations));
		info.AddProperty(PropertyName.UnBlockable, Variant.From(in UnBlockable));
		info.AddProperty(PropertyName.AvoidMouse, Variant.From(in AvoidMouse));
		info.AddProperty(PropertyName.enemySubType, Variant.From(in enemySubType));
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
		if (info.TryGetProperty(PropertyName.characterAnimationLayers, out var value6))
		{
			characterAnimationLayers = value6.AsGodotArray<CharAnimDataRes>();
		}
		if (info.TryGetProperty(PropertyName.ClothingStates, out var value7))
		{
			ClothingStates = value7.AsGodotArray<string>();
		}
		if (info.TryGetProperty(PropertyName.characterSize, out var value8))
		{
			characterSize = value8.As<Vector2I>();
		}
		if (info.TryGetProperty(PropertyName.characterScale, out var value9))
		{
			characterScale = value9.As<Vector2>();
		}
		if (info.TryGetProperty(PropertyName.characterOffset, out var value10))
		{
			characterOffset = value10.As<Vector2>();
		}
		if (info.TryGetProperty(PropertyName.characterColor, out var value11))
		{
			characterColor = value11.As<Color>();
		}
		if (info.TryGetProperty(PropertyName.interactionTexts, out var value12))
		{
			interactionTexts = value12.AsGodotArray<DialogueDataRes>();
		}
		if (info.TryGetProperty(PropertyName.randomTexts, out var value13))
		{
			randomTexts = value13.AsGodotArray<ConvoDataRes>();
		}
		if (info.TryGetProperty(PropertyName.randomDialogueTimer, out var value14))
		{
			randomDialogueTimer = value14.As<Vector2>();
		}
		if (info.TryGetProperty(PropertyName.randomAnimations, out var value15))
		{
			randomAnimations = value15.AsGodotArray<AnimDataRes>();
		}
		if (info.TryGetProperty(PropertyName.randomAnimationTimer, out var value16))
		{
			randomAnimationTimer = value16.As<Vector2>();
		}
		if (info.TryGetProperty(PropertyName.firstTimeStartupMessage, out var value17))
		{
			firstTimeStartupMessage = value17.As<DialogueDataRes>();
		}
		if (info.TryGetProperty(PropertyName.welcomeMessages, out var value18))
		{
			welcomeMessages = value18.AsGodotArray<DialogueDataRes>();
		}
		if (info.TryGetProperty(PropertyName.flipSpriteH, out var value19))
		{
			flipSpriteH = value19.As<bool>();
		}
		if (info.TryGetProperty(PropertyName.WalkSpeed, out var value20))
		{
			WalkSpeed = value20.As<float>();
		}
		if (info.TryGetProperty(PropertyName.responseTexts, out var value21))
		{
			responseTexts = value21.AsGodotDictionary<ResponseToSituation, DialogueDataRes>();
		}
		if (info.TryGetProperty(PropertyName.possibleStateOverride, out var value22))
		{
			possibleStateOverride = value22.AsGodotArray<TagOverrideDataRes>();
		}
		if (info.TryGetProperty(PropertyName.AITyping, out var value23))
		{
			AITyping = value23.As<AITypes>();
		}
		if (info.TryGetProperty(PropertyName.actorSpawnWeight, out var value24))
		{
			actorSpawnWeight = value24.As<double>();
		}
		if (info.TryGetProperty(PropertyName.overrideAnimation, out var value25))
		{
			overrideAnimation = value25.AsGodotArray<AttachDataRes>();
		}
		if (info.TryGetProperty(PropertyName.spawnsItems, out var value26))
		{
			spawnsItems = value26.As<bool>();
		}
		if (info.TryGetProperty(PropertyName.itemSpawnRate, out var value27))
		{
			itemSpawnRate = value27.As<Vector2>();
		}
		if (info.TryGetProperty(PropertyName.Glitchy, out var value28))
		{
			Glitchy = value28.As<bool>();
		}
		if (info.TryGetProperty(PropertyName.isAggroActor, out var value29))
		{
			isAggroActor = value29.As<bool>();
		}
		if (info.TryGetProperty(PropertyName.aggroTimerRange, out var value30))
		{
			aggroTimerRange = value30.As<Vector2>();
		}
		if (info.TryGetProperty(PropertyName.SpeicalAnimations, out var value31))
		{
			SpeicalAnimations = value31.As<AnimDataRes>();
		}
		if (info.TryGetProperty(PropertyName.UnBlockable, out var value32))
		{
			UnBlockable = value32.As<bool>();
		}
		if (info.TryGetProperty(PropertyName.AvoidMouse, out var value33))
		{
			AvoidMouse = value33.As<bool>();
		}
		if (info.TryGetProperty(PropertyName.enemySubType, out var value34))
		{
			enemySubType = value34.As<EnemySubTypes>();
		}
		if (info.TryGetProperty(PropertyName.taggedKinks, out var value35))
		{
			taggedKinks = value35.AsGodotArray<SaveHandler.Kinks>();
		}
	}
}
