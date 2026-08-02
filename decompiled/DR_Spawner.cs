using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Godot;
using Godot.Bridge;
using Godot.Collections;
using Godot.NativeInterop;

[ScriptPath("res://Scripts/Minigames/DinoRunner/DR_Spawner.cs")]
public class DR_Spawner : Node2D
{
	public new class MethodName : Node2D.MethodName
	{
		public static readonly StringName StartSpawner = "StartSpawner";

		public static readonly StringName StopSpawner = "StopSpawner";

		public static readonly StringName OnSpawnTimerTimeout = "OnSpawnTimerTimeout";

		public static readonly StringName SpawnProp = "SpawnProp";

		public static readonly StringName GetWeightedRandomProp = "GetWeightedRandomProp";

		public static readonly StringName GetPropFrameSize = "GetPropFrameSize";
	}

	public new class PropertyName : Node2D.PropertyName
	{
		public static readonly StringName gameHandler = "gameHandler";

		public static readonly StringName spawnTimer = "spawnTimer";

		public static readonly StringName spawnHolder = "spawnHolder";

		public static readonly StringName spawnMarker = "spawnMarker";

		public static readonly StringName flyingSpawnMarker = "flyingSpawnMarker";

		public static readonly StringName spawnIntervalRange = "spawnIntervalRange";

		public static readonly StringName groupSpacing = "groupSpacing";

		public static readonly StringName possibleProps = "possibleProps";

		public static readonly StringName propObject = "propObject";
	}

	public new class SignalName : Node2D.SignalName
	{
	}

	[Export(PropertyHint.None, "")]
	public DR_GameHandler gameHandler;

	[Export(PropertyHint.None, "")]
	public Timer spawnTimer;

	[Export(PropertyHint.None, "")]
	public Node2D spawnHolder;

	[Export(PropertyHint.None, "")]
	public Marker2D spawnMarker;

	[Export(PropertyHint.None, "")]
	public Marker2D flyingSpawnMarker;

	[Export(PropertyHint.None, "")]
	public Vector2 spawnIntervalRange = new Vector2(2f, 4.5f);

	[Export(PropertyHint.None, "")]
	public float groupSpacing;

	[Export(PropertyHint.None, "")]
	public Array<DR_PropDataRes> possibleProps;

	[Export(PropertyHint.None, "")]
	private PackedScene propObject;

	public void StartSpawner()
	{
		spawnTimer.WaitTime = GD.RandRange(spawnIntervalRange.X, spawnIntervalRange.Y);
		spawnTimer.Start();
	}

	public void StopSpawner()
	{
		spawnTimer.Stop();
	}

	private void OnSpawnTimerTimeout()
	{
		SpawnProp();
		float weight = Mathf.InverseLerp(1f, gameHandler.maxSpeed, gameHandler.gameSpeed);
		float num = Mathf.Lerp(spawnIntervalRange.Y, spawnIntervalRange.X, weight);
		spawnTimer.WaitTime = num;
	}

	private void SpawnProp()
	{
		if (propObject == null || possibleProps == null || possibleProps.Count == 0)
		{
			return;
		}
		DR_PropDataRes weightedRandomProp = GetWeightedRandomProp();
		if (weightedRandomProp == null)
		{
			return;
		}
		Vector2 propFrameSize = GetPropFrameSize(weightedRandomProp);
		if (propFrameSize == Vector2.Zero)
		{
			GD.PrintErr("DR_Spawner: selected DR_PropDataRes '" + weightedRandomProp.ResourceName + "' has neither propAnimation nor propTexture. Skipping spawn.");
			return;
		}
		Marker2D obj = ((weightedRandomProp.isFlying && flyingSpawnMarker != null) ? flyingSpawnMarker : spawnMarker);
		int num = ((weightedRandomProp.maxGrouping <= 1) ? 1 : GD.RandRange(1, weightedRandomProp.maxGrouping));
		float num2 = propFrameSize.Y * weightedRandomProp.propScale.Y;
		float num3 = propFrameSize.X * weightedRandomProp.propScale.X;
		float y = obj.GlobalPosition.Y - num2 / 2f;
		float num4 = obj.GlobalPosition.X;
		for (int i = 0; i < num; i++)
		{
			DR_PropHandler dR_PropHandler = propObject.Instantiate<DR_PropHandler>(PackedScene.GenEditState.Disabled);
			dR_PropHandler.PropTexture = weightedRandomProp.propTexture;
			dR_PropHandler.GlobalPosition = new Vector2(num4, y);
			dR_PropHandler.gameHandler = gameHandler;
			dR_PropHandler.propData = weightedRandomProp;
			spawnHolder.AddChild(dR_PropHandler, forceReadableName: false, InternalMode.Disabled);
			num4 += num3 / 2f + groupSpacing;
		}
	}

	private DR_PropDataRes GetWeightedRandomProp()
	{
		float num = possibleProps.Sum((DR_PropDataRes p) => p.spawnWeight);
		float num2 = (float)GD.RandRange(0.0, num);
		float num3 = 0f;
		foreach (DR_PropDataRes possibleProp in possibleProps)
		{
			num3 += possibleProp.spawnWeight;
			if (num2 <= num3)
			{
				return possibleProp;
			}
		}
		return possibleProps.LastOrDefault();
	}

	private Vector2 GetPropFrameSize(DR_PropDataRes data)
	{
		if (data.propAnimation != null)
		{
			string text = data.propAnimation.GetAnimationNames()[0];
			if (data.propAnimation.GetFrameCount(text) > 0)
			{
				Texture2D frameTexture = data.propAnimation.GetFrameTexture(text, 0);
				if (frameTexture != null)
				{
					return new Vector2(frameTexture.GetWidth(), frameTexture.GetHeight());
				}
			}
		}
		if (data.propTexture != null)
		{
			return new Vector2(data.propTexture.GetWidth(), data.propTexture.GetHeight());
		}
		return Vector2.Zero;
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	internal static List<MethodInfo> GetGodotMethodList()
	{
		return new List<MethodInfo>(6)
		{
			new MethodInfo(MethodName.StartSpawner, new PropertyInfo(Variant.Type.Nil, "", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false), MethodFlags.Normal, null, null),
			new MethodInfo(MethodName.StopSpawner, new PropertyInfo(Variant.Type.Nil, "", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false), MethodFlags.Normal, null, null),
			new MethodInfo(MethodName.OnSpawnTimerTimeout, new PropertyInfo(Variant.Type.Nil, "", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false), MethodFlags.Normal, null, null),
			new MethodInfo(MethodName.SpawnProp, new PropertyInfo(Variant.Type.Nil, "", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false), MethodFlags.Normal, null, null),
			new MethodInfo(MethodName.GetWeightedRandomProp, new PropertyInfo(Variant.Type.Object, "", PropertyHint.None, "", PropertyUsageFlags.Default, new StringName("Resource"), exported: false), MethodFlags.Normal, null, null),
			new MethodInfo(MethodName.GetPropFrameSize, new PropertyInfo(Variant.Type.Vector2, "", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false), MethodFlags.Normal, new List<PropertyInfo>
			{
				new PropertyInfo(Variant.Type.Object, "data", PropertyHint.None, "", PropertyUsageFlags.Default, new StringName("Resource"), exported: false)
			}, null)
		};
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override bool InvokeGodotClassMethod(in godot_string_name method, NativeVariantPtrArgs args, out godot_variant ret)
	{
		if (method == MethodName.StartSpawner && args.Count == 0)
		{
			StartSpawner();
			ret = default(godot_variant);
			return true;
		}
		if (method == MethodName.StopSpawner && args.Count == 0)
		{
			StopSpawner();
			ret = default(godot_variant);
			return true;
		}
		if (method == MethodName.OnSpawnTimerTimeout && args.Count == 0)
		{
			OnSpawnTimerTimeout();
			ret = default(godot_variant);
			return true;
		}
		if (method == MethodName.SpawnProp && args.Count == 0)
		{
			SpawnProp();
			ret = default(godot_variant);
			return true;
		}
		if (method == MethodName.GetWeightedRandomProp && args.Count == 0)
		{
			DR_PropDataRes from = GetWeightedRandomProp();
			ret = VariantUtils.CreateFrom(in from);
			return true;
		}
		if (method == MethodName.GetPropFrameSize && args.Count == 1)
		{
			Vector2 from2 = GetPropFrameSize(VariantUtils.ConvertTo<DR_PropDataRes>(in args[0]));
			ret = VariantUtils.CreateFrom(in from2);
			return true;
		}
		return base.InvokeGodotClassMethod(in method, args, out ret);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override bool HasGodotClassMethod(in godot_string_name method)
	{
		if (method == MethodName.StartSpawner)
		{
			return true;
		}
		if (method == MethodName.StopSpawner)
		{
			return true;
		}
		if (method == MethodName.OnSpawnTimerTimeout)
		{
			return true;
		}
		if (method == MethodName.SpawnProp)
		{
			return true;
		}
		if (method == MethodName.GetWeightedRandomProp)
		{
			return true;
		}
		if (method == MethodName.GetPropFrameSize)
		{
			return true;
		}
		return base.HasGodotClassMethod(in method);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override bool SetGodotClassPropertyValue(in godot_string_name name, in godot_variant value)
	{
		if (name == PropertyName.gameHandler)
		{
			gameHandler = VariantUtils.ConvertTo<DR_GameHandler>(in value);
			return true;
		}
		if (name == PropertyName.spawnTimer)
		{
			spawnTimer = VariantUtils.ConvertTo<Timer>(in value);
			return true;
		}
		if (name == PropertyName.spawnHolder)
		{
			spawnHolder = VariantUtils.ConvertTo<Node2D>(in value);
			return true;
		}
		if (name == PropertyName.spawnMarker)
		{
			spawnMarker = VariantUtils.ConvertTo<Marker2D>(in value);
			return true;
		}
		if (name == PropertyName.flyingSpawnMarker)
		{
			flyingSpawnMarker = VariantUtils.ConvertTo<Marker2D>(in value);
			return true;
		}
		if (name == PropertyName.spawnIntervalRange)
		{
			spawnIntervalRange = VariantUtils.ConvertTo<Vector2>(in value);
			return true;
		}
		if (name == PropertyName.groupSpacing)
		{
			groupSpacing = VariantUtils.ConvertTo<float>(in value);
			return true;
		}
		if (name == PropertyName.possibleProps)
		{
			possibleProps = VariantUtils.ConvertToArray<DR_PropDataRes>(in value);
			return true;
		}
		if (name == PropertyName.propObject)
		{
			propObject = VariantUtils.ConvertTo<PackedScene>(in value);
			return true;
		}
		return base.SetGodotClassPropertyValue(in name, in value);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override bool GetGodotClassPropertyValue(in godot_string_name name, out godot_variant value)
	{
		if (name == PropertyName.gameHandler)
		{
			value = VariantUtils.CreateFrom(in gameHandler);
			return true;
		}
		if (name == PropertyName.spawnTimer)
		{
			value = VariantUtils.CreateFrom(in spawnTimer);
			return true;
		}
		if (name == PropertyName.spawnHolder)
		{
			value = VariantUtils.CreateFrom(in spawnHolder);
			return true;
		}
		if (name == PropertyName.spawnMarker)
		{
			value = VariantUtils.CreateFrom(in spawnMarker);
			return true;
		}
		if (name == PropertyName.flyingSpawnMarker)
		{
			value = VariantUtils.CreateFrom(in flyingSpawnMarker);
			return true;
		}
		if (name == PropertyName.spawnIntervalRange)
		{
			value = VariantUtils.CreateFrom(in spawnIntervalRange);
			return true;
		}
		if (name == PropertyName.groupSpacing)
		{
			value = VariantUtils.CreateFrom(in groupSpacing);
			return true;
		}
		if (name == PropertyName.possibleProps)
		{
			value = VariantUtils.CreateFromArray(possibleProps);
			return true;
		}
		if (name == PropertyName.propObject)
		{
			value = VariantUtils.CreateFrom(in propObject);
			return true;
		}
		return base.GetGodotClassPropertyValue(in name, out value);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	internal static List<PropertyInfo> GetGodotPropertyList()
	{
		return new List<PropertyInfo>
		{
			new PropertyInfo(Variant.Type.Object, PropertyName.gameHandler, PropertyHint.NodeType, "Window", PropertyUsageFlags.Default | PropertyUsageFlags.ScriptVariable, exported: true),
			new PropertyInfo(Variant.Type.Object, PropertyName.spawnTimer, PropertyHint.NodeType, "Timer", PropertyUsageFlags.Default | PropertyUsageFlags.ScriptVariable, exported: true),
			new PropertyInfo(Variant.Type.Object, PropertyName.spawnHolder, PropertyHint.NodeType, "Node2D", PropertyUsageFlags.Default | PropertyUsageFlags.ScriptVariable, exported: true),
			new PropertyInfo(Variant.Type.Object, PropertyName.spawnMarker, PropertyHint.NodeType, "Marker2D", PropertyUsageFlags.Default | PropertyUsageFlags.ScriptVariable, exported: true),
			new PropertyInfo(Variant.Type.Object, PropertyName.flyingSpawnMarker, PropertyHint.NodeType, "Marker2D", PropertyUsageFlags.Default | PropertyUsageFlags.ScriptVariable, exported: true),
			new PropertyInfo(Variant.Type.Vector2, PropertyName.spawnIntervalRange, PropertyHint.None, "", PropertyUsageFlags.Default | PropertyUsageFlags.ScriptVariable, exported: true),
			new PropertyInfo(Variant.Type.Float, PropertyName.groupSpacing, PropertyHint.None, "", PropertyUsageFlags.Default | PropertyUsageFlags.ScriptVariable, exported: true),
			new PropertyInfo(Variant.Type.Array, PropertyName.possibleProps, PropertyHint.TypeString, "24/17:DR_PropDataRes", PropertyUsageFlags.Default | PropertyUsageFlags.ScriptVariable, exported: true),
			new PropertyInfo(Variant.Type.Object, PropertyName.propObject, PropertyHint.ResourceType, "PackedScene", PropertyUsageFlags.Default | PropertyUsageFlags.ScriptVariable, exported: true)
		};
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override void SaveGodotObjectData(GodotSerializationInfo info)
	{
		base.SaveGodotObjectData(info);
		info.AddProperty(PropertyName.gameHandler, Variant.From(in gameHandler));
		info.AddProperty(PropertyName.spawnTimer, Variant.From(in spawnTimer));
		info.AddProperty(PropertyName.spawnHolder, Variant.From(in spawnHolder));
		info.AddProperty(PropertyName.spawnMarker, Variant.From(in spawnMarker));
		info.AddProperty(PropertyName.flyingSpawnMarker, Variant.From(in flyingSpawnMarker));
		info.AddProperty(PropertyName.spawnIntervalRange, Variant.From(in spawnIntervalRange));
		info.AddProperty(PropertyName.groupSpacing, Variant.From(in groupSpacing));
		info.AddProperty(PropertyName.possibleProps, Variant.CreateFrom(possibleProps));
		info.AddProperty(PropertyName.propObject, Variant.From(in propObject));
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override void RestoreGodotObjectData(GodotSerializationInfo info)
	{
		base.RestoreGodotObjectData(info);
		if (info.TryGetProperty(PropertyName.gameHandler, out var value))
		{
			gameHandler = value.As<DR_GameHandler>();
		}
		if (info.TryGetProperty(PropertyName.spawnTimer, out var value2))
		{
			spawnTimer = value2.As<Timer>();
		}
		if (info.TryGetProperty(PropertyName.spawnHolder, out var value3))
		{
			spawnHolder = value3.As<Node2D>();
		}
		if (info.TryGetProperty(PropertyName.spawnMarker, out var value4))
		{
			spawnMarker = value4.As<Marker2D>();
		}
		if (info.TryGetProperty(PropertyName.flyingSpawnMarker, out var value5))
		{
			flyingSpawnMarker = value5.As<Marker2D>();
		}
		if (info.TryGetProperty(PropertyName.spawnIntervalRange, out var value6))
		{
			spawnIntervalRange = value6.As<Vector2>();
		}
		if (info.TryGetProperty(PropertyName.groupSpacing, out var value7))
		{
			groupSpacing = value7.As<float>();
		}
		if (info.TryGetProperty(PropertyName.possibleProps, out var value8))
		{
			possibleProps = value8.AsGodotArray<DR_PropDataRes>();
		}
		if (info.TryGetProperty(PropertyName.propObject, out var value9))
		{
			propObject = value9.As<PackedScene>();
		}
	}
}
