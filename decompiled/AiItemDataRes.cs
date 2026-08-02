using System.Collections.Generic;
using System.ComponentModel;
using Godot;
using Godot.Bridge;
using Godot.Collections;
using Godot.NativeInterop;

[GlobalClass]
[Tool]
[ScriptPath("res://Scripts/DataResources/AiItemDataRes.cs")]
public class AiItemDataRes : Resource
{
	public new class MethodName : Resource.MethodName
	{
	}

	public new class PropertyName : Resource.PropertyName
	{
		public static readonly StringName targetActorsID = "targetActorsID";

		public static readonly StringName itemTasks = "itemTasks";

		public static readonly StringName animationName = "animationName";

		public static readonly StringName attachedSubData = "attachedSubData";

		public static readonly StringName RandomAmountToAttach = "RandomAmountToAttach";

		public static readonly StringName dialogueSubStack = "dialogueSubStack";

		public static readonly StringName RandomDialogueSubAssignment = "RandomDialogueSubAssignment";

		public static readonly StringName possibleSpawnedSubActors = "possibleSpawnedSubActors";

		public static readonly StringName spawningItem = "spawningItem";

		public static readonly StringName ItemAmountToSpawn = "ItemAmountToSpawn";

		public static readonly StringName aggroAnimations = "aggroAnimations";
	}

	public new class SignalName : Resource.SignalName
	{
	}

	[Export(PropertyHint.None, "")]
	public string targetActorsID;

	[Export(PropertyHint.None, "")]
	public Array<ItemDataRes.ItemTask> itemTasks = new Array<ItemDataRes.ItemTask>();

	[ExportSubgroup("TASK - Animation Variables", "")]
	[Export(PropertyHint.None, "")]
	public string animationName;

	[ExportSubgroup("TASK - Attachment Variables", "")]
	[Export(PropertyHint.None, "")]
	public Array<AttachDataRes> attachedSubData = new Array<AttachDataRes>();

	[Export(PropertyHint.None, "")]
	public int RandomAmountToAttach = -1;

	[ExportSubgroup("TASK - Dialogue Variables", "")]
	[Export(PropertyHint.None, "")]
	public Array<DialogueDataRes> dialogueSubStack = new Array<DialogueDataRes>();

	[Export(PropertyHint.None, "")]
	public bool RandomDialogueSubAssignment;

	[ExportSubgroup("TASK - Actor Spawn Variables", "")]
	[Export(PropertyHint.None, "")]
	public Array<CharacterInfoDataRes> possibleSpawnedSubActors = new Array<CharacterInfoDataRes>();

	[ExportSubgroup("TASK - Item Spawn Variables", "")]
	[Export(PropertyHint.None, "")]
	public Godot.Collections.Dictionary<ItemDataRes, float> spawningItem = new Godot.Collections.Dictionary<ItemDataRes, float>();

	[Export(PropertyHint.None, "")]
	public int ItemAmountToSpawn = 1;

	[ExportSubgroup("TASK - Actor Aggro Animations", "")]
	[Export(PropertyHint.None, "")]
	public Array<AttachDataRes> aggroAnimations = new Array<AttachDataRes>();

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override bool SetGodotClassPropertyValue(in godot_string_name name, in godot_variant value)
	{
		if (name == PropertyName.targetActorsID)
		{
			targetActorsID = VariantUtils.ConvertTo<string>(in value);
			return true;
		}
		if (name == PropertyName.itemTasks)
		{
			itemTasks = VariantUtils.ConvertToArray<ItemDataRes.ItemTask>(in value);
			return true;
		}
		if (name == PropertyName.animationName)
		{
			animationName = VariantUtils.ConvertTo<string>(in value);
			return true;
		}
		if (name == PropertyName.attachedSubData)
		{
			attachedSubData = VariantUtils.ConvertToArray<AttachDataRes>(in value);
			return true;
		}
		if (name == PropertyName.RandomAmountToAttach)
		{
			RandomAmountToAttach = VariantUtils.ConvertTo<int>(in value);
			return true;
		}
		if (name == PropertyName.dialogueSubStack)
		{
			dialogueSubStack = VariantUtils.ConvertToArray<DialogueDataRes>(in value);
			return true;
		}
		if (name == PropertyName.RandomDialogueSubAssignment)
		{
			RandomDialogueSubAssignment = VariantUtils.ConvertTo<bool>(in value);
			return true;
		}
		if (name == PropertyName.possibleSpawnedSubActors)
		{
			possibleSpawnedSubActors = VariantUtils.ConvertToArray<CharacterInfoDataRes>(in value);
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
		if (name == PropertyName.aggroAnimations)
		{
			aggroAnimations = VariantUtils.ConvertToArray<AttachDataRes>(in value);
			return true;
		}
		return base.SetGodotClassPropertyValue(in name, in value);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override bool GetGodotClassPropertyValue(in godot_string_name name, out godot_variant value)
	{
		if (name == PropertyName.targetActorsID)
		{
			value = VariantUtils.CreateFrom(in targetActorsID);
			return true;
		}
		if (name == PropertyName.itemTasks)
		{
			value = VariantUtils.CreateFromArray(itemTasks);
			return true;
		}
		if (name == PropertyName.animationName)
		{
			value = VariantUtils.CreateFrom(in animationName);
			return true;
		}
		if (name == PropertyName.attachedSubData)
		{
			value = VariantUtils.CreateFromArray(attachedSubData);
			return true;
		}
		if (name == PropertyName.RandomAmountToAttach)
		{
			value = VariantUtils.CreateFrom(in RandomAmountToAttach);
			return true;
		}
		if (name == PropertyName.dialogueSubStack)
		{
			value = VariantUtils.CreateFromArray(dialogueSubStack);
			return true;
		}
		if (name == PropertyName.RandomDialogueSubAssignment)
		{
			value = VariantUtils.CreateFrom(in RandomDialogueSubAssignment);
			return true;
		}
		if (name == PropertyName.possibleSpawnedSubActors)
		{
			value = VariantUtils.CreateFromArray(possibleSpawnedSubActors);
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
		if (name == PropertyName.aggroAnimations)
		{
			value = VariantUtils.CreateFromArray(aggroAnimations);
			return true;
		}
		return base.GetGodotClassPropertyValue(in name, out value);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	internal static List<PropertyInfo> GetGodotPropertyList()
	{
		return new List<PropertyInfo>
		{
			new PropertyInfo(Variant.Type.String, PropertyName.targetActorsID, PropertyHint.None, "", PropertyUsageFlags.Default | PropertyUsageFlags.ScriptVariable, exported: true),
			new PropertyInfo(Variant.Type.Array, PropertyName.itemTasks, PropertyHint.TypeString, "2/2:UNTYPED:0,RUN_ANIMATION:1,RUN_ATTACH:2,RUN_DIALOGUE:3,SPAWN_ENEMY:4,SPAWN_ITEM:5,SPAWN_POPUP:6,SPAWN_MINIGAME:10,DESPAWN_SUB_ACTOR:7,AGGRO_SUB_ACTOR:8,ENEMY_SUB_ACTOR:9", PropertyUsageFlags.Default | PropertyUsageFlags.ScriptVariable, exported: true),
			new PropertyInfo(Variant.Type.Nil, "TASK - Animation Variables", PropertyHint.None, "", PropertyUsageFlags.Subgroup, exported: true),
			new PropertyInfo(Variant.Type.String, PropertyName.animationName, PropertyHint.None, "", PropertyUsageFlags.Default | PropertyUsageFlags.ScriptVariable, exported: true),
			new PropertyInfo(Variant.Type.Nil, "TASK - Attachment Variables", PropertyHint.None, "", PropertyUsageFlags.Subgroup, exported: true),
			new PropertyInfo(Variant.Type.Array, PropertyName.attachedSubData, PropertyHint.TypeString, "24/17:AttachDataRes", PropertyUsageFlags.Default | PropertyUsageFlags.ScriptVariable, exported: true),
			new PropertyInfo(Variant.Type.Int, PropertyName.RandomAmountToAttach, PropertyHint.None, "", PropertyUsageFlags.Default | PropertyUsageFlags.ScriptVariable, exported: true),
			new PropertyInfo(Variant.Type.Nil, "TASK - Dialogue Variables", PropertyHint.None, "", PropertyUsageFlags.Subgroup, exported: true),
			new PropertyInfo(Variant.Type.Array, PropertyName.dialogueSubStack, PropertyHint.TypeString, "24/17:DialogueDataRes", PropertyUsageFlags.Default | PropertyUsageFlags.ScriptVariable, exported: true),
			new PropertyInfo(Variant.Type.Bool, PropertyName.RandomDialogueSubAssignment, PropertyHint.None, "", PropertyUsageFlags.Default | PropertyUsageFlags.ScriptVariable, exported: true),
			new PropertyInfo(Variant.Type.Nil, "TASK - Actor Spawn Variables", PropertyHint.None, "", PropertyUsageFlags.Subgroup, exported: true),
			new PropertyInfo(Variant.Type.Array, PropertyName.possibleSpawnedSubActors, PropertyHint.TypeString, "24/17:CharacterInfoDataRes", PropertyUsageFlags.Default | PropertyUsageFlags.ScriptVariable, exported: true),
			new PropertyInfo(Variant.Type.Nil, "TASK - Item Spawn Variables", PropertyHint.None, "", PropertyUsageFlags.Subgroup, exported: true),
			new PropertyInfo(Variant.Type.Dictionary, PropertyName.spawningItem, PropertyHint.TypeString, "24/17:ItemDataRes;3/0:", PropertyUsageFlags.Default | PropertyUsageFlags.ScriptVariable, exported: true),
			new PropertyInfo(Variant.Type.Int, PropertyName.ItemAmountToSpawn, PropertyHint.None, "", PropertyUsageFlags.Default | PropertyUsageFlags.ScriptVariable, exported: true),
			new PropertyInfo(Variant.Type.Nil, "TASK - Actor Aggro Animations", PropertyHint.None, "", PropertyUsageFlags.Subgroup, exported: true),
			new PropertyInfo(Variant.Type.Array, PropertyName.aggroAnimations, PropertyHint.TypeString, "24/17:AttachDataRes", PropertyUsageFlags.Default | PropertyUsageFlags.ScriptVariable, exported: true)
		};
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override void SaveGodotObjectData(GodotSerializationInfo info)
	{
		base.SaveGodotObjectData(info);
		info.AddProperty(PropertyName.targetActorsID, Variant.From(in targetActorsID));
		info.AddProperty(PropertyName.itemTasks, Variant.CreateFrom(itemTasks));
		info.AddProperty(PropertyName.animationName, Variant.From(in animationName));
		info.AddProperty(PropertyName.attachedSubData, Variant.CreateFrom(attachedSubData));
		info.AddProperty(PropertyName.RandomAmountToAttach, Variant.From(in RandomAmountToAttach));
		info.AddProperty(PropertyName.dialogueSubStack, Variant.CreateFrom(dialogueSubStack));
		info.AddProperty(PropertyName.RandomDialogueSubAssignment, Variant.From(in RandomDialogueSubAssignment));
		info.AddProperty(PropertyName.possibleSpawnedSubActors, Variant.CreateFrom(possibleSpawnedSubActors));
		info.AddProperty(PropertyName.spawningItem, Variant.CreateFrom(spawningItem));
		info.AddProperty(PropertyName.ItemAmountToSpawn, Variant.From(in ItemAmountToSpawn));
		info.AddProperty(PropertyName.aggroAnimations, Variant.CreateFrom(aggroAnimations));
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override void RestoreGodotObjectData(GodotSerializationInfo info)
	{
		base.RestoreGodotObjectData(info);
		if (info.TryGetProperty(PropertyName.targetActorsID, out var value))
		{
			targetActorsID = value.As<string>();
		}
		if (info.TryGetProperty(PropertyName.itemTasks, out var value2))
		{
			itemTasks = value2.AsGodotArray<ItemDataRes.ItemTask>();
		}
		if (info.TryGetProperty(PropertyName.animationName, out var value3))
		{
			animationName = value3.As<string>();
		}
		if (info.TryGetProperty(PropertyName.attachedSubData, out var value4))
		{
			attachedSubData = value4.AsGodotArray<AttachDataRes>();
		}
		if (info.TryGetProperty(PropertyName.RandomAmountToAttach, out var value5))
		{
			RandomAmountToAttach = value5.As<int>();
		}
		if (info.TryGetProperty(PropertyName.dialogueSubStack, out var value6))
		{
			dialogueSubStack = value6.AsGodotArray<DialogueDataRes>();
		}
		if (info.TryGetProperty(PropertyName.RandomDialogueSubAssignment, out var value7))
		{
			RandomDialogueSubAssignment = value7.As<bool>();
		}
		if (info.TryGetProperty(PropertyName.possibleSpawnedSubActors, out var value8))
		{
			possibleSpawnedSubActors = value8.AsGodotArray<CharacterInfoDataRes>();
		}
		if (info.TryGetProperty(PropertyName.spawningItem, out var value9))
		{
			spawningItem = value9.AsGodotDictionary<ItemDataRes, float>();
		}
		if (info.TryGetProperty(PropertyName.ItemAmountToSpawn, out var value10))
		{
			ItemAmountToSpawn = value10.As<int>();
		}
		if (info.TryGetProperty(PropertyName.aggroAnimations, out var value11))
		{
			aggroAnimations = value11.AsGodotArray<AttachDataRes>();
		}
	}
}
