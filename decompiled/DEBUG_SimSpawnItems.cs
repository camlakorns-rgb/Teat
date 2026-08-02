using System.Collections.Generic;
using System.ComponentModel;
using Godot;
using Godot.Bridge;
using Godot.Collections;
using Godot.NativeInterop;

[ScriptPath("res://Scripts/Tool/DEBUG_SimSpawnItems.cs")]
public class DEBUG_SimSpawnItems : Node
{
	public enum Targets
	{
		CACHE,
		ANIMATIONS,
		DIALOGUE,
		OVERRIDES
	}

	public new class MethodName : Node.MethodName
	{
		public new static readonly StringName _Ready = "_Ready";

		public static readonly StringName GetDummyTag = "GetDummyTag";

		public static readonly StringName TagsValid = "TagsValid";

		public static readonly StringName SimulateLootTable = "SimulateLootTable";

		public static readonly StringName SimulateAnimations = "SimulateAnimations";

		public static readonly StringName SimulateDialogue = "SimulateDialogue";

		public static readonly StringName SimulateOverrides = "SimulateOverrides";
	}

	public new class PropertyName : Node.PropertyName
	{
		public static readonly StringName simulationCount = "simulationCount";

		public static readonly StringName targets = "targets";

		public static readonly StringName simulationTarget = "simulationTarget";

		public static readonly StringName dummyTagList = "dummyTagList";

		public static readonly StringName animationCache = "animationCache";

		public static readonly StringName dialogueCache = "dialogueCache";

		public static readonly StringName overrideCache = "overrideCache";
	}

	public new class SignalName : Node.SignalName
	{
	}

	[Export(PropertyHint.None, "")]
	public int simulationCount = 100;

	[Export(PropertyHint.None, "")]
	public Targets targets;

	[ExportGroup("Resource Cache", "")]
	[Export(PropertyHint.None, "")]
	public ResourceCache.ResourceTyping simulationTarget = ResourceCache.ResourceTyping.ITEM;

	[ExportGroup("Custom Caches", "")]
	[Export(PropertyHint.None, "")]
	public Array<TagDataRes> dummyTagList = new Array<TagDataRes>();

	[ExportSubgroup("Animations", "")]
	[Export(PropertyHint.None, "")]
	public Array<AnimDataRes> animationCache = new Array<AnimDataRes>();

	[ExportSubgroup("Dialogue", "")]
	[Export(PropertyHint.None, "")]
	public Array<DialogueDataRes> dialogueCache = new Array<DialogueDataRes>();

	[ExportSubgroup("Overrides", "")]
	[Export(PropertyHint.None, "")]
	public Array<AttachDataRes> overrideCache = new Array<AttachDataRes>();

	public override void _Ready()
	{
		switch (targets)
		{
		case Targets.CACHE:
			CallDeferred("SimulateLootTable");
			break;
		case Targets.ANIMATIONS:
			CallDeferred("SimulateAnimations");
			break;
		case Targets.DIALOGUE:
			CallDeferred("SimulateDialogue");
			break;
		case Targets.OVERRIDES:
			CallDeferred("SimulateOverrides");
			break;
		}
	}

	private TagDataRes GetDummyTag(string tagName)
	{
		foreach (TagDataRes dummyTag in dummyTagList)
		{
			if (dummyTag.tagName == tagName)
			{
				return dummyTag;
			}
		}
		return null;
	}

	private bool TagsValid(Array<TagDataRes> requiredTags)
	{
		if (requiredTags == null || requiredTags.Count == 0)
		{
			return true;
		}
		foreach (TagDataRes requiredTag in requiredTags)
		{
			TagDataRes dummyTag = GetDummyTag(requiredTag.tagName);
			if (dummyTag == null || (requiredTag.tagAmount > 0 && dummyTag.tagAmount < requiredTag.tagAmount))
			{
				return false;
			}
		}
		return true;
	}

	private void SimulateLootTable()
	{
		Godot.Collections.Dictionary<string, int> dictionary = new Godot.Collections.Dictionary<string, int>();
		for (int i = 0; i < simulationCount; i++)
		{
			ICollection<string> keys = ResourceCache.resourcesLoaded[simulationTarget].Keys;
			WeightGroup<string> weightGroup = new WeightGroup<string>();
			foreach (string item2 in keys)
			{
				switch (simulationTarget)
				{
				case ResourceCache.ResourceTyping.ITEM:
					if (ResourceCache.resourcesLoaded[simulationTarget][item2] is ItemDataRes itemDataRes)
					{
						weightGroup.Add(item2, itemDataRes.itemSpawnWeight);
					}
					break;
				case ResourceCache.ResourceTyping.CHARACTER:
					if (ResourceCache.resourcesLoaded[simulationTarget][item2] is CharacterInfoDataRes characterInfoDataRes)
					{
						weightGroup.Add(item2, characterInfoDataRes.actorSpawnWeight);
					}
					break;
				}
			}
			string item = weightGroup.GetItem(GD.RandRange(0, 10000));
			if (dictionary.ContainsKey(item))
			{
				dictionary[item]++;
			}
			else
			{
				dictionary.Add(item, 1);
			}
		}
		GD.Print(dictionary);
		GetTree().Quit();
	}

	private void SimulateAnimations()
	{
		Godot.Collections.Dictionary<string, int> dictionary = new Godot.Collections.Dictionary<string, int>();
		for (int i = 0; i < simulationCount; i++)
		{
			WeightGroup<AnimDataRes> weightGroup = new WeightGroup<AnimDataRes>();
			foreach (AnimDataRes item in animationCache)
			{
				if (!(item.animationName == "INVALID_ANIMATION") && TagsValid(item.RequiredTags))
				{
					weightGroup.Add(item, item.animationAppeanceWeight);
				}
			}
			string animationName = weightGroup.GetItem(GD.RandRange(0, 10000)).animationName;
			if (dictionary.ContainsKey(animationName))
			{
				dictionary[animationName]++;
			}
			else
			{
				dictionary.Add(animationName, 1);
			}
		}
		GD.Print(dictionary);
		GetTree().Quit();
	}

	private void SimulateDialogue()
	{
		Godot.Collections.Dictionary<string, int> dictionary = new Godot.Collections.Dictionary<string, int>();
		for (int i = 0; i < simulationCount; i++)
		{
			WeightGroup<DialogueDataRes> weightGroup = new WeightGroup<DialogueDataRes>();
			foreach (DialogueDataRes item2 in dialogueCache)
			{
				if (TagsValid(item2.RequiredTags))
				{
					weightGroup.Add(item2, item2.dialogueAppeanceWeight);
				}
			}
			DialogueDataRes item = weightGroup.GetItem(GD.RandRange(0, 10000));
			string key = ((!string.IsNullOrEmpty(item.speakingActorID)) ? (item.speakingActorID + ": " + item.Dialogue?.Left(32)) : (item.Dialogue?.Left(48) ?? "NULL"));
			if (dictionary.ContainsKey(key))
			{
				dictionary[key]++;
			}
			else
			{
				dictionary.Add(key, 1);
			}
		}
		GD.Print(dictionary);
		GetTree().Quit();
	}

	private void SimulateOverrides()
	{
		Godot.Collections.Dictionary<string, int> dictionary = new Godot.Collections.Dictionary<string, int>();
		for (int i = 0; i < simulationCount; i++)
		{
			WeightGroup<AttachDataRes> weightGroup = new WeightGroup<AttachDataRes>();
			foreach (AttachDataRes item2 in overrideCache)
			{
				if (TagsValid(item2.RequiredTags))
				{
					weightGroup.Add(item2, item2.attachmentAppeanceWeight);
				}
			}
			AttachDataRes item = weightGroup.GetItem(GD.RandRange(0, 10000));
			string key = ((!string.IsNullOrEmpty(item.ResourcePath)) ? item.ResourcePath.GetFile() : $"{item.attachmentTyping}_{overrideCache.IndexOf(item)}");
			if (dictionary.ContainsKey(key))
			{
				dictionary[key]++;
			}
			else
			{
				dictionary.Add(key, 1);
			}
		}
		GD.Print(dictionary);
		GetTree().Quit();
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	internal static List<MethodInfo> GetGodotMethodList()
	{
		return new List<MethodInfo>(7)
		{
			new MethodInfo(MethodName._Ready, new PropertyInfo(Variant.Type.Nil, "", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false), MethodFlags.Normal, null, null),
			new MethodInfo(MethodName.GetDummyTag, new PropertyInfo(Variant.Type.Object, "", PropertyHint.None, "", PropertyUsageFlags.Default, new StringName("Resource"), exported: false), MethodFlags.Normal, new List<PropertyInfo>
			{
				new PropertyInfo(Variant.Type.String, "tagName", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false)
			}, null),
			new MethodInfo(MethodName.TagsValid, new PropertyInfo(Variant.Type.Bool, "", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false), MethodFlags.Normal, new List<PropertyInfo>
			{
				new PropertyInfo(Variant.Type.Array, "requiredTags", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false)
			}, null),
			new MethodInfo(MethodName.SimulateLootTable, new PropertyInfo(Variant.Type.Nil, "", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false), MethodFlags.Normal, null, null),
			new MethodInfo(MethodName.SimulateAnimations, new PropertyInfo(Variant.Type.Nil, "", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false), MethodFlags.Normal, null, null),
			new MethodInfo(MethodName.SimulateDialogue, new PropertyInfo(Variant.Type.Nil, "", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false), MethodFlags.Normal, null, null),
			new MethodInfo(MethodName.SimulateOverrides, new PropertyInfo(Variant.Type.Nil, "", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false), MethodFlags.Normal, null, null)
		};
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override bool InvokeGodotClassMethod(in godot_string_name method, NativeVariantPtrArgs args, out godot_variant ret)
	{
		if (method == MethodName._Ready && args.Count == 0)
		{
			_Ready();
			ret = default(godot_variant);
			return true;
		}
		if (method == MethodName.GetDummyTag && args.Count == 1)
		{
			TagDataRes from = GetDummyTag(VariantUtils.ConvertTo<string>(in args[0]));
			ret = VariantUtils.CreateFrom(in from);
			return true;
		}
		if (method == MethodName.TagsValid && args.Count == 1)
		{
			bool from2 = TagsValid(VariantUtils.ConvertToArray<TagDataRes>(in args[0]));
			ret = VariantUtils.CreateFrom(in from2);
			return true;
		}
		if (method == MethodName.SimulateLootTable && args.Count == 0)
		{
			SimulateLootTable();
			ret = default(godot_variant);
			return true;
		}
		if (method == MethodName.SimulateAnimations && args.Count == 0)
		{
			SimulateAnimations();
			ret = default(godot_variant);
			return true;
		}
		if (method == MethodName.SimulateDialogue && args.Count == 0)
		{
			SimulateDialogue();
			ret = default(godot_variant);
			return true;
		}
		if (method == MethodName.SimulateOverrides && args.Count == 0)
		{
			SimulateOverrides();
			ret = default(godot_variant);
			return true;
		}
		return base.InvokeGodotClassMethod(in method, args, out ret);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override bool HasGodotClassMethod(in godot_string_name method)
	{
		if (method == MethodName._Ready)
		{
			return true;
		}
		if (method == MethodName.GetDummyTag)
		{
			return true;
		}
		if (method == MethodName.TagsValid)
		{
			return true;
		}
		if (method == MethodName.SimulateLootTable)
		{
			return true;
		}
		if (method == MethodName.SimulateAnimations)
		{
			return true;
		}
		if (method == MethodName.SimulateDialogue)
		{
			return true;
		}
		if (method == MethodName.SimulateOverrides)
		{
			return true;
		}
		return base.HasGodotClassMethod(in method);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override bool SetGodotClassPropertyValue(in godot_string_name name, in godot_variant value)
	{
		if (name == PropertyName.simulationCount)
		{
			simulationCount = VariantUtils.ConvertTo<int>(in value);
			return true;
		}
		if (name == PropertyName.targets)
		{
			targets = VariantUtils.ConvertTo<Targets>(in value);
			return true;
		}
		if (name == PropertyName.simulationTarget)
		{
			simulationTarget = VariantUtils.ConvertTo<ResourceCache.ResourceTyping>(in value);
			return true;
		}
		if (name == PropertyName.dummyTagList)
		{
			dummyTagList = VariantUtils.ConvertToArray<TagDataRes>(in value);
			return true;
		}
		if (name == PropertyName.animationCache)
		{
			animationCache = VariantUtils.ConvertToArray<AnimDataRes>(in value);
			return true;
		}
		if (name == PropertyName.dialogueCache)
		{
			dialogueCache = VariantUtils.ConvertToArray<DialogueDataRes>(in value);
			return true;
		}
		if (name == PropertyName.overrideCache)
		{
			overrideCache = VariantUtils.ConvertToArray<AttachDataRes>(in value);
			return true;
		}
		return base.SetGodotClassPropertyValue(in name, in value);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override bool GetGodotClassPropertyValue(in godot_string_name name, out godot_variant value)
	{
		if (name == PropertyName.simulationCount)
		{
			value = VariantUtils.CreateFrom(in simulationCount);
			return true;
		}
		if (name == PropertyName.targets)
		{
			value = VariantUtils.CreateFrom(in targets);
			return true;
		}
		if (name == PropertyName.simulationTarget)
		{
			value = VariantUtils.CreateFrom(in simulationTarget);
			return true;
		}
		if (name == PropertyName.dummyTagList)
		{
			value = VariantUtils.CreateFromArray(dummyTagList);
			return true;
		}
		if (name == PropertyName.animationCache)
		{
			value = VariantUtils.CreateFromArray(animationCache);
			return true;
		}
		if (name == PropertyName.dialogueCache)
		{
			value = VariantUtils.CreateFromArray(dialogueCache);
			return true;
		}
		if (name == PropertyName.overrideCache)
		{
			value = VariantUtils.CreateFromArray(overrideCache);
			return true;
		}
		return base.GetGodotClassPropertyValue(in name, out value);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	internal static List<PropertyInfo> GetGodotPropertyList()
	{
		return new List<PropertyInfo>
		{
			new PropertyInfo(Variant.Type.Int, PropertyName.simulationCount, PropertyHint.None, "", PropertyUsageFlags.Default | PropertyUsageFlags.ScriptVariable, exported: true),
			new PropertyInfo(Variant.Type.Int, PropertyName.targets, PropertyHint.Enum, "CACHE,ANIMATIONS,DIALOGUE,OVERRIDES", PropertyUsageFlags.Default | PropertyUsageFlags.ScriptVariable, exported: true),
			new PropertyInfo(Variant.Type.Nil, "Resource Cache", PropertyHint.None, "", PropertyUsageFlags.Group, exported: true),
			new PropertyInfo(Variant.Type.Int, PropertyName.simulationTarget, PropertyHint.Enum, "UNTYPED,CHARACTER,ITEM,GALLERY,SPAM,BRAINDACE_WORLDS,ASK_CHARACTERS,H_SCENES", PropertyUsageFlags.Default | PropertyUsageFlags.ScriptVariable, exported: true),
			new PropertyInfo(Variant.Type.Nil, "Custom Caches", PropertyHint.None, "", PropertyUsageFlags.Group, exported: true),
			new PropertyInfo(Variant.Type.Array, PropertyName.dummyTagList, PropertyHint.TypeString, "24/17:TagDataRes", PropertyUsageFlags.Default | PropertyUsageFlags.ScriptVariable, exported: true),
			new PropertyInfo(Variant.Type.Nil, "Animations", PropertyHint.None, "", PropertyUsageFlags.Subgroup, exported: true),
			new PropertyInfo(Variant.Type.Array, PropertyName.animationCache, PropertyHint.TypeString, "24/17:AnimDataRes", PropertyUsageFlags.Default | PropertyUsageFlags.ScriptVariable, exported: true),
			new PropertyInfo(Variant.Type.Nil, "Dialogue", PropertyHint.None, "", PropertyUsageFlags.Subgroup, exported: true),
			new PropertyInfo(Variant.Type.Array, PropertyName.dialogueCache, PropertyHint.TypeString, "24/17:DialogueDataRes", PropertyUsageFlags.Default | PropertyUsageFlags.ScriptVariable, exported: true),
			new PropertyInfo(Variant.Type.Nil, "Overrides", PropertyHint.None, "", PropertyUsageFlags.Subgroup, exported: true),
			new PropertyInfo(Variant.Type.Array, PropertyName.overrideCache, PropertyHint.TypeString, "24/17:AttachDataRes", PropertyUsageFlags.Default | PropertyUsageFlags.ScriptVariable, exported: true)
		};
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override void SaveGodotObjectData(GodotSerializationInfo info)
	{
		base.SaveGodotObjectData(info);
		info.AddProperty(PropertyName.simulationCount, Variant.From(in simulationCount));
		info.AddProperty(PropertyName.targets, Variant.From(in targets));
		info.AddProperty(PropertyName.simulationTarget, Variant.From(in simulationTarget));
		info.AddProperty(PropertyName.dummyTagList, Variant.CreateFrom(dummyTagList));
		info.AddProperty(PropertyName.animationCache, Variant.CreateFrom(animationCache));
		info.AddProperty(PropertyName.dialogueCache, Variant.CreateFrom(dialogueCache));
		info.AddProperty(PropertyName.overrideCache, Variant.CreateFrom(overrideCache));
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override void RestoreGodotObjectData(GodotSerializationInfo info)
	{
		base.RestoreGodotObjectData(info);
		if (info.TryGetProperty(PropertyName.simulationCount, out var value))
		{
			simulationCount = value.As<int>();
		}
		if (info.TryGetProperty(PropertyName.targets, out var value2))
		{
			targets = value2.As<Targets>();
		}
		if (info.TryGetProperty(PropertyName.simulationTarget, out var value3))
		{
			simulationTarget = value3.As<ResourceCache.ResourceTyping>();
		}
		if (info.TryGetProperty(PropertyName.dummyTagList, out var value4))
		{
			dummyTagList = value4.AsGodotArray<TagDataRes>();
		}
		if (info.TryGetProperty(PropertyName.animationCache, out var value5))
		{
			animationCache = value5.AsGodotArray<AnimDataRes>();
		}
		if (info.TryGetProperty(PropertyName.dialogueCache, out var value6))
		{
			dialogueCache = value6.AsGodotArray<DialogueDataRes>();
		}
		if (info.TryGetProperty(PropertyName.overrideCache, out var value7))
		{
			overrideCache = value7.AsGodotArray<AttachDataRes>();
		}
	}
}
