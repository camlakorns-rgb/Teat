using System.Collections.Generic;
using System.ComponentModel;
using Godot;
using Godot.Bridge;
using Godot.NativeInterop;

[ScriptPath("res://Scripts/Minigames/DinoRunner/DR_Dino.cs")]
public class DR_Dino : CharacterBody2D
{
	public new class MethodName : CharacterBody2D.MethodName
	{
		public new static readonly StringName _PhysicsProcess = "_PhysicsProcess";

		public static readonly StringName OnAreaEntered = "OnAreaEntered";

		public static readonly StringName Die = "Die";
	}

	public new class PropertyName : CharacterBody2D.PropertyName
	{
		public static readonly StringName gameHandler = "gameHandler";

		public static readonly StringName parent = "parent";

		public static readonly StringName standingShape = "standingShape";

		public static readonly StringName duckingShape = "duckingShape";

		public static readonly StringName jumpVelocity = "jumpVelocity";

		public static readonly StringName sprite = "sprite";
	}

	public new class SignalName : CharacterBody2D.SignalName
	{
	}

	[Export(PropertyHint.None, "")]
	private DR_GameHandler gameHandler;

	[Export(PropertyHint.None, "")]
	private Node2D parent;

	[Export(PropertyHint.None, "")]
	private CollisionShape2D standingShape;

	[Export(PropertyHint.None, "")]
	private CollisionShape2D duckingShape;

	[Export(PropertyHint.None, "")]
	private float jumpVelocity = -500f;

	[Export(PropertyHint.None, "")]
	private AnimatedSprite2D sprite;

	public override void _PhysicsProcess(double delta)
	{
		if (!IsOnFloor())
		{
			base.Velocity += GetGravity() * (float)delta;
			if (sprite.Animation != (StringName)"Jump")
			{
				gameHandler.jumpSound.Play();
				sprite.Play("Jump");
			}
		}
		if (Input.IsActionJustPressed("DR_Jump") && IsOnFloor())
		{
			base.Velocity = new Vector2(base.Velocity.X, jumpVelocity);
		}
		if (IsOnFloor())
		{
			if (Input.IsActionPressed("DR_Duck"))
			{
				standingShape.Disabled = true;
				duckingShape.Disabled = false;
				if (sprite.Animation != (StringName)"Duck")
				{
					sprite.Play("Duck");
				}
			}
			else
			{
				standingShape.Disabled = false;
				duckingShape.Disabled = true;
				if (sprite.Animation != (StringName)"Walk")
				{
					sprite.Play("Walk");
				}
			}
		}
		MoveAndSlide();
	}

	private void OnAreaEntered(Area2D area)
	{
		if (!(area is DR_PropHandler dR_PropHandler))
		{
			return;
		}
		switch (dR_PropHandler.propData.propFunction)
		{
		case DR_PropDataRes.PropFunction.HAZARD:
			gameHandler.calculateScore();
			CallDeferred("Die");
			break;
		case DR_PropDataRes.PropFunction.SPAWN_ITEM:
			gameHandler.SpawnByteItem(dR_PropHandler.propData.spawnID);
			break;
		case DR_PropDataRes.PropFunction.SPAWN_DIALOGUE:
			gameHandler.SpawnDialogue(dR_PropHandler.propData.functionDialogue);
			break;
		case DR_PropDataRes.PropFunction.SPAWN_ACTOR:
			gameHandler.SpawnActor(dR_PropHandler.propData.spawnID);
			break;
		case DR_PropDataRes.PropFunction.SPAWN_POPUP_TAGGED:
			gameHandler.SpawnPopup(dR_PropHandler.propData.spawnID);
			break;
		case DR_PropDataRes.PropFunction.SPAWN_POPUP_RANDOM:
			gameHandler.SpawnRandomPopup();
			break;
		}
		if (dR_PropHandler.propData.propFunction != 0)
		{
			gameHandler.scoreBoonSound.Play();
			gameHandler.scoreAccumulator += dR_PropHandler.propData.scoreBoon;
			int num = 50;
			if (dR_PropHandler.propData.scoreBoon != 0f)
			{
				gameHandler.SpawnFloatingText("+" + dR_PropHandler.propData.scoreBoon, base.Position + new Vector2(50f, num));
				num += 25;
			}
			DR_PropDataRes.PropFunction propFunction = dR_PropHandler.propData.propFunction;
			if (propFunction == DR_PropDataRes.PropFunction.SPAWN_ITEM || (uint)(propFunction - 5) <= 1u)
			{
				gameHandler.SpawnFloatingText("+Prize!", base.Position + new Vector2(50f, num));
			}
			dR_PropHandler.QueueFree();
		}
	}

	public void Die()
	{
		gameHandler.GameRunning = false;
		sprite.Play("Die");
		base.Position = gameHandler.cachedDinoPosition;
		gameHandler.GameStop.Visible = true;
		gameHandler.deathSound.Play();
		gameHandler.SaveMinigame(gameHandler.highScore);
		foreach (Node child in gameHandler.spawnedProps.GetChildren())
		{
			child.QueueFree();
		}
		parent.ProcessMode = ProcessModeEnum.Disabled;
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	internal static List<MethodInfo> GetGodotMethodList()
	{
		return new List<MethodInfo>(3)
		{
			new MethodInfo(MethodName._PhysicsProcess, new PropertyInfo(Variant.Type.Nil, "", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false), MethodFlags.Normal, new List<PropertyInfo>
			{
				new PropertyInfo(Variant.Type.Float, "delta", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false)
			}, null),
			new MethodInfo(MethodName.OnAreaEntered, new PropertyInfo(Variant.Type.Nil, "", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false), MethodFlags.Normal, new List<PropertyInfo>
			{
				new PropertyInfo(Variant.Type.Object, "area", PropertyHint.None, "", PropertyUsageFlags.Default, new StringName("Area2D"), exported: false)
			}, null),
			new MethodInfo(MethodName.Die, new PropertyInfo(Variant.Type.Nil, "", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false), MethodFlags.Normal, null, null)
		};
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override bool InvokeGodotClassMethod(in godot_string_name method, NativeVariantPtrArgs args, out godot_variant ret)
	{
		if (method == MethodName._PhysicsProcess && args.Count == 1)
		{
			_PhysicsProcess(VariantUtils.ConvertTo<double>(in args[0]));
			ret = default(godot_variant);
			return true;
		}
		if (method == MethodName.OnAreaEntered && args.Count == 1)
		{
			OnAreaEntered(VariantUtils.ConvertTo<Area2D>(in args[0]));
			ret = default(godot_variant);
			return true;
		}
		if (method == MethodName.Die && args.Count == 0)
		{
			Die();
			ret = default(godot_variant);
			return true;
		}
		return base.InvokeGodotClassMethod(in method, args, out ret);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override bool HasGodotClassMethod(in godot_string_name method)
	{
		if (method == MethodName._PhysicsProcess)
		{
			return true;
		}
		if (method == MethodName.OnAreaEntered)
		{
			return true;
		}
		if (method == MethodName.Die)
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
		if (name == PropertyName.parent)
		{
			parent = VariantUtils.ConvertTo<Node2D>(in value);
			return true;
		}
		if (name == PropertyName.standingShape)
		{
			standingShape = VariantUtils.ConvertTo<CollisionShape2D>(in value);
			return true;
		}
		if (name == PropertyName.duckingShape)
		{
			duckingShape = VariantUtils.ConvertTo<CollisionShape2D>(in value);
			return true;
		}
		if (name == PropertyName.jumpVelocity)
		{
			jumpVelocity = VariantUtils.ConvertTo<float>(in value);
			return true;
		}
		if (name == PropertyName.sprite)
		{
			sprite = VariantUtils.ConvertTo<AnimatedSprite2D>(in value);
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
		if (name == PropertyName.parent)
		{
			value = VariantUtils.CreateFrom(in parent);
			return true;
		}
		if (name == PropertyName.standingShape)
		{
			value = VariantUtils.CreateFrom(in standingShape);
			return true;
		}
		if (name == PropertyName.duckingShape)
		{
			value = VariantUtils.CreateFrom(in duckingShape);
			return true;
		}
		if (name == PropertyName.jumpVelocity)
		{
			value = VariantUtils.CreateFrom(in jumpVelocity);
			return true;
		}
		if (name == PropertyName.sprite)
		{
			value = VariantUtils.CreateFrom(in sprite);
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
			new PropertyInfo(Variant.Type.Object, PropertyName.parent, PropertyHint.NodeType, "Node2D", PropertyUsageFlags.Default | PropertyUsageFlags.ScriptVariable, exported: true),
			new PropertyInfo(Variant.Type.Object, PropertyName.standingShape, PropertyHint.NodeType, "CollisionShape2D", PropertyUsageFlags.Default | PropertyUsageFlags.ScriptVariable, exported: true),
			new PropertyInfo(Variant.Type.Object, PropertyName.duckingShape, PropertyHint.NodeType, "CollisionShape2D", PropertyUsageFlags.Default | PropertyUsageFlags.ScriptVariable, exported: true),
			new PropertyInfo(Variant.Type.Float, PropertyName.jumpVelocity, PropertyHint.None, "", PropertyUsageFlags.Default | PropertyUsageFlags.ScriptVariable, exported: true),
			new PropertyInfo(Variant.Type.Object, PropertyName.sprite, PropertyHint.NodeType, "AnimatedSprite2D", PropertyUsageFlags.Default | PropertyUsageFlags.ScriptVariable, exported: true)
		};
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override void SaveGodotObjectData(GodotSerializationInfo info)
	{
		base.SaveGodotObjectData(info);
		info.AddProperty(PropertyName.gameHandler, Variant.From(in gameHandler));
		info.AddProperty(PropertyName.parent, Variant.From(in parent));
		info.AddProperty(PropertyName.standingShape, Variant.From(in standingShape));
		info.AddProperty(PropertyName.duckingShape, Variant.From(in duckingShape));
		info.AddProperty(PropertyName.jumpVelocity, Variant.From(in jumpVelocity));
		info.AddProperty(PropertyName.sprite, Variant.From(in sprite));
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override void RestoreGodotObjectData(GodotSerializationInfo info)
	{
		base.RestoreGodotObjectData(info);
		if (info.TryGetProperty(PropertyName.gameHandler, out var value))
		{
			gameHandler = value.As<DR_GameHandler>();
		}
		if (info.TryGetProperty(PropertyName.parent, out var value2))
		{
			parent = value2.As<Node2D>();
		}
		if (info.TryGetProperty(PropertyName.standingShape, out var value3))
		{
			standingShape = value3.As<CollisionShape2D>();
		}
		if (info.TryGetProperty(PropertyName.duckingShape, out var value4))
		{
			duckingShape = value4.As<CollisionShape2D>();
		}
		if (info.TryGetProperty(PropertyName.jumpVelocity, out var value5))
		{
			jumpVelocity = value5.As<float>();
		}
		if (info.TryGetProperty(PropertyName.sprite, out var value6))
		{
			sprite = value6.As<AnimatedSprite2D>();
		}
	}
}
