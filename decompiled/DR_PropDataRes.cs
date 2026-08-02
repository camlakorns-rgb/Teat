using System.Collections.Generic;
using System.ComponentModel;
using Godot;
using Godot.Bridge;
using Godot.NativeInterop;

[GlobalClass]
[Tool]
[ScriptPath("res://Scripts/Minigames/DinoRunner/DR_PropDataRes.cs")]
public class DR_PropDataRes : Resource
{
	public enum PropFunction
	{
		HAZARD,
		EDIT_SCORE,
		SPAWN_ITEM,
		SPAWN_ACTOR,
		SPAWN_DIALOGUE,
		SPAWN_POPUP_TAGGED,
		SPAWN_POPUP_RANDOM
	}

	public new class MethodName : Resource.MethodName
	{
	}

	public new class PropertyName : Resource.PropertyName
	{
		public static readonly StringName propAnimation = "propAnimation";

		public static readonly StringName propTexture = "propTexture";

		public static readonly StringName propScale = "propScale";

		public static readonly StringName spawnWeight = "spawnWeight";

		public static readonly StringName propFunction = "propFunction";

		public static readonly StringName scoreBoon = "scoreBoon";

		public static readonly StringName spawnID = "spawnID";

		public static readonly StringName functionDialogue = "functionDialogue";

		public static readonly StringName isFlying = "isFlying";

		public static readonly StringName maxGrouping = "maxGrouping";
	}

	public new class SignalName : Resource.SignalName
	{
	}

	[Export(PropertyHint.None, "")]
	public SpriteFrames propAnimation;

	[Export(PropertyHint.None, "")]
	public Texture2D propTexture;

	[Export(PropertyHint.None, "")]
	public Vector2 propScale = new Vector2(1f, 1f);

	[Export(PropertyHint.None, "")]
	public float spawnWeight = 10f;

	[Export(PropertyHint.None, "")]
	public PropFunction propFunction;

	[ExportGroup("Prop Function Variables", "")]
	[Export(PropertyHint.None, "")]
	public float scoreBoon;

	[Export(PropertyHint.None, "")]
	public string spawnID;

	[Export(PropertyHint.None, "")]
	public DialogueDataRes functionDialogue;

	[ExportGroup("", "")]
	[Export(PropertyHint.None, "")]
	public bool isFlying;

	[Export(PropertyHint.None, "")]
	public int maxGrouping = 1;

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override bool SetGodotClassPropertyValue(in godot_string_name name, in godot_variant value)
	{
		if (name == PropertyName.propAnimation)
		{
			propAnimation = VariantUtils.ConvertTo<SpriteFrames>(in value);
			return true;
		}
		if (name == PropertyName.propTexture)
		{
			propTexture = VariantUtils.ConvertTo<Texture2D>(in value);
			return true;
		}
		if (name == PropertyName.propScale)
		{
			propScale = VariantUtils.ConvertTo<Vector2>(in value);
			return true;
		}
		if (name == PropertyName.spawnWeight)
		{
			spawnWeight = VariantUtils.ConvertTo<float>(in value);
			return true;
		}
		if (name == PropertyName.propFunction)
		{
			propFunction = VariantUtils.ConvertTo<PropFunction>(in value);
			return true;
		}
		if (name == PropertyName.scoreBoon)
		{
			scoreBoon = VariantUtils.ConvertTo<float>(in value);
			return true;
		}
		if (name == PropertyName.spawnID)
		{
			spawnID = VariantUtils.ConvertTo<string>(in value);
			return true;
		}
		if (name == PropertyName.functionDialogue)
		{
			functionDialogue = VariantUtils.ConvertTo<DialogueDataRes>(in value);
			return true;
		}
		if (name == PropertyName.isFlying)
		{
			isFlying = VariantUtils.ConvertTo<bool>(in value);
			return true;
		}
		if (name == PropertyName.maxGrouping)
		{
			maxGrouping = VariantUtils.ConvertTo<int>(in value);
			return true;
		}
		return base.SetGodotClassPropertyValue(in name, in value);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override bool GetGodotClassPropertyValue(in godot_string_name name, out godot_variant value)
	{
		if (name == PropertyName.propAnimation)
		{
			value = VariantUtils.CreateFrom(in propAnimation);
			return true;
		}
		if (name == PropertyName.propTexture)
		{
			value = VariantUtils.CreateFrom(in propTexture);
			return true;
		}
		if (name == PropertyName.propScale)
		{
			value = VariantUtils.CreateFrom(in propScale);
			return true;
		}
		if (name == PropertyName.spawnWeight)
		{
			value = VariantUtils.CreateFrom(in spawnWeight);
			return true;
		}
		if (name == PropertyName.propFunction)
		{
			value = VariantUtils.CreateFrom(in propFunction);
			return true;
		}
		if (name == PropertyName.scoreBoon)
		{
			value = VariantUtils.CreateFrom(in scoreBoon);
			return true;
		}
		if (name == PropertyName.spawnID)
		{
			value = VariantUtils.CreateFrom(in spawnID);
			return true;
		}
		if (name == PropertyName.functionDialogue)
		{
			value = VariantUtils.CreateFrom(in functionDialogue);
			return true;
		}
		if (name == PropertyName.isFlying)
		{
			value = VariantUtils.CreateFrom(in isFlying);
			return true;
		}
		if (name == PropertyName.maxGrouping)
		{
			value = VariantUtils.CreateFrom(in maxGrouping);
			return true;
		}
		return base.GetGodotClassPropertyValue(in name, out value);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	internal static List<PropertyInfo> GetGodotPropertyList()
	{
		return new List<PropertyInfo>
		{
			new PropertyInfo(Variant.Type.Object, PropertyName.propAnimation, PropertyHint.ResourceType, "SpriteFrames", PropertyUsageFlags.Default | PropertyUsageFlags.ScriptVariable, exported: true),
			new PropertyInfo(Variant.Type.Object, PropertyName.propTexture, PropertyHint.ResourceType, "Texture2D", PropertyUsageFlags.Default | PropertyUsageFlags.ScriptVariable, exported: true),
			new PropertyInfo(Variant.Type.Vector2, PropertyName.propScale, PropertyHint.None, "", PropertyUsageFlags.Default | PropertyUsageFlags.ScriptVariable, exported: true),
			new PropertyInfo(Variant.Type.Float, PropertyName.spawnWeight, PropertyHint.None, "", PropertyUsageFlags.Default | PropertyUsageFlags.ScriptVariable, exported: true),
			new PropertyInfo(Variant.Type.Int, PropertyName.propFunction, PropertyHint.Enum, "HAZARD,EDIT_SCORE,SPAWN_ITEM,SPAWN_ACTOR,SPAWN_DIALOGUE,SPAWN_POPUP_TAGGED,SPAWN_POPUP_RANDOM", PropertyUsageFlags.Default | PropertyUsageFlags.ScriptVariable, exported: true),
			new PropertyInfo(Variant.Type.Nil, "Prop Function Variables", PropertyHint.None, "", PropertyUsageFlags.Group, exported: true),
			new PropertyInfo(Variant.Type.Float, PropertyName.scoreBoon, PropertyHint.None, "", PropertyUsageFlags.Default | PropertyUsageFlags.ScriptVariable, exported: true),
			new PropertyInfo(Variant.Type.String, PropertyName.spawnID, PropertyHint.None, "", PropertyUsageFlags.Default | PropertyUsageFlags.ScriptVariable, exported: true),
			new PropertyInfo(Variant.Type.Object, PropertyName.functionDialogue, PropertyHint.ResourceType, "DialogueDataRes", PropertyUsageFlags.Default | PropertyUsageFlags.ScriptVariable, exported: true),
			new PropertyInfo(Variant.Type.Nil, "", PropertyHint.None, "", PropertyUsageFlags.Group, exported: true),
			new PropertyInfo(Variant.Type.Bool, PropertyName.isFlying, PropertyHint.None, "", PropertyUsageFlags.Default | PropertyUsageFlags.ScriptVariable, exported: true),
			new PropertyInfo(Variant.Type.Int, PropertyName.maxGrouping, PropertyHint.None, "", PropertyUsageFlags.Default | PropertyUsageFlags.ScriptVariable, exported: true)
		};
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override void SaveGodotObjectData(GodotSerializationInfo info)
	{
		base.SaveGodotObjectData(info);
		info.AddProperty(PropertyName.propAnimation, Variant.From(in propAnimation));
		info.AddProperty(PropertyName.propTexture, Variant.From(in propTexture));
		info.AddProperty(PropertyName.propScale, Variant.From(in propScale));
		info.AddProperty(PropertyName.spawnWeight, Variant.From(in spawnWeight));
		info.AddProperty(PropertyName.propFunction, Variant.From(in propFunction));
		info.AddProperty(PropertyName.scoreBoon, Variant.From(in scoreBoon));
		info.AddProperty(PropertyName.spawnID, Variant.From(in spawnID));
		info.AddProperty(PropertyName.functionDialogue, Variant.From(in functionDialogue));
		info.AddProperty(PropertyName.isFlying, Variant.From(in isFlying));
		info.AddProperty(PropertyName.maxGrouping, Variant.From(in maxGrouping));
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override void RestoreGodotObjectData(GodotSerializationInfo info)
	{
		base.RestoreGodotObjectData(info);
		if (info.TryGetProperty(PropertyName.propAnimation, out var value))
		{
			propAnimation = value.As<SpriteFrames>();
		}
		if (info.TryGetProperty(PropertyName.propTexture, out var value2))
		{
			propTexture = value2.As<Texture2D>();
		}
		if (info.TryGetProperty(PropertyName.propScale, out var value3))
		{
			propScale = value3.As<Vector2>();
		}
		if (info.TryGetProperty(PropertyName.spawnWeight, out var value4))
		{
			spawnWeight = value4.As<float>();
		}
		if (info.TryGetProperty(PropertyName.propFunction, out var value5))
		{
			propFunction = value5.As<PropFunction>();
		}
		if (info.TryGetProperty(PropertyName.scoreBoon, out var value6))
		{
			scoreBoon = value6.As<float>();
		}
		if (info.TryGetProperty(PropertyName.spawnID, out var value7))
		{
			spawnID = value7.As<string>();
		}
		if (info.TryGetProperty(PropertyName.functionDialogue, out var value8))
		{
			functionDialogue = value8.As<DialogueDataRes>();
		}
		if (info.TryGetProperty(PropertyName.isFlying, out var value9))
		{
			isFlying = value9.As<bool>();
		}
		if (info.TryGetProperty(PropertyName.maxGrouping, out var value10))
		{
			maxGrouping = value10.As<int>();
		}
	}
}
