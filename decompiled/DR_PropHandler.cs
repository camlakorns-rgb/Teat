using System.Collections.Generic;
using System.ComponentModel;
using Godot;
using Godot.Bridge;
using Godot.NativeInterop;

[ScriptPath("res://Scripts/Minigames/DinoRunner/DR_PropHandler.cs")]
public class DR_PropHandler : Area2D
{
	public new class MethodName : Area2D.MethodName
	{
		public new static readonly StringName _Ready = "_Ready";

		public new static readonly StringName _Process = "_Process";

		public static readonly StringName UpdateCollisionShape = "UpdateCollisionShape";

		public static readonly StringName GetVisualSize = "GetVisualSize";
	}

	public new class PropertyName : Area2D.PropertyName
	{
		public static readonly StringName PropTexture = "PropTexture";

		public static readonly StringName gameHandler = "gameHandler";

		public static readonly StringName propData = "propData";

		public static readonly StringName speed = "speed";

		public static readonly StringName _sprite = "_sprite";

		public static readonly StringName _animatedSprite = "_animatedSprite";

		public static readonly StringName _collisionShape = "_collisionShape";
	}

	public new class SignalName : Area2D.SignalName
	{
	}

	[Export(PropertyHint.None, "")]
	public DR_GameHandler gameHandler;

	[Export(PropertyHint.None, "")]
	public DR_PropDataRes propData;

	[Export(PropertyHint.None, "")]
	public float speed = 300f;

	[Export(PropertyHint.None, "")]
	private Sprite2D _sprite;

	[Export(PropertyHint.None, "")]
	private AnimatedSprite2D _animatedSprite;

	[Export(PropertyHint.None, "")]
	private CollisionShape2D _collisionShape;

	[Export(PropertyHint.None, "")]
	public Texture2D PropTexture { get; set; }

	public override void _Ready()
	{
		bool num = propData != null && propData.propAnimation != null;
		_ = PropTexture;
		if (num)
		{
			_animatedSprite.Visible = true;
			_sprite.Visible = false;
			_animatedSprite.SpriteFrames = propData.propAnimation;
			_animatedSprite.Scale = propData.propScale;
			_animatedSprite.Play(propData.propAnimation.GetAnimationNames()[0]);
		}
		else
		{
			if (PropTexture == null)
			{
				GD.PrintErr($"DR_PropHandler: '{base.Name}' has no propAnimation or PropTexture assigned. Freeing.");
				QueueFree();
				return;
			}
			_animatedSprite.Visible = false;
			_sprite.Visible = true;
			_sprite.Texture = PropTexture;
			if (propData != null)
			{
				_sprite.Scale = propData.propScale;
			}
		}
		UpdateCollisionShape();
	}

	public override void _Process(double delta)
	{
		base.Position = new Vector2(base.Position.X - speed * gameHandler.gameSpeed * (float)delta, base.Position.Y);
		if (base.Position.X < -100f)
		{
			QueueFree();
		}
	}

	private void UpdateCollisionShape()
	{
		Vector2 visualSize = GetVisualSize();
		if (!(visualSize == Vector2.Zero))
		{
			CapsuleShape2D capsuleShape2D = new CapsuleShape2D();
			capsuleShape2D.Radius = visualSize.X / 4f;
			capsuleShape2D.Height = visualSize.Y;
			_collisionShape.Shape = capsuleShape2D;
		}
	}

	private Vector2 GetVisualSize()
	{
		if (propData != null && propData.propAnimation != null)
		{
			string text = propData.propAnimation.GetAnimationNames()[0];
			if (propData.propAnimation.GetFrameCount(text) > 0)
			{
				Texture2D frameTexture = propData.propAnimation.GetFrameTexture(text, 0);
				if (frameTexture != null)
				{
					return frameTexture.GetSize() * propData.propScale;
				}
			}
		}
		if (_sprite.Texture != null)
		{
			return _sprite.Texture.GetSize() * (propData?.propScale ?? Vector2.One);
		}
		return Vector2.Zero;
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	internal static List<MethodInfo> GetGodotMethodList()
	{
		return new List<MethodInfo>(4)
		{
			new MethodInfo(MethodName._Ready, new PropertyInfo(Variant.Type.Nil, "", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false), MethodFlags.Normal, null, null),
			new MethodInfo(MethodName._Process, new PropertyInfo(Variant.Type.Nil, "", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false), MethodFlags.Normal, new List<PropertyInfo>
			{
				new PropertyInfo(Variant.Type.Float, "delta", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false)
			}, null),
			new MethodInfo(MethodName.UpdateCollisionShape, new PropertyInfo(Variant.Type.Nil, "", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false), MethodFlags.Normal, null, null),
			new MethodInfo(MethodName.GetVisualSize, new PropertyInfo(Variant.Type.Vector2, "", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false), MethodFlags.Normal, null, null)
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
		if (method == MethodName._Process && args.Count == 1)
		{
			_Process(VariantUtils.ConvertTo<double>(in args[0]));
			ret = default(godot_variant);
			return true;
		}
		if (method == MethodName.UpdateCollisionShape && args.Count == 0)
		{
			UpdateCollisionShape();
			ret = default(godot_variant);
			return true;
		}
		if (method == MethodName.GetVisualSize && args.Count == 0)
		{
			Vector2 from = GetVisualSize();
			ret = VariantUtils.CreateFrom(in from);
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
		if (method == MethodName._Process)
		{
			return true;
		}
		if (method == MethodName.UpdateCollisionShape)
		{
			return true;
		}
		if (method == MethodName.GetVisualSize)
		{
			return true;
		}
		return base.HasGodotClassMethod(in method);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override bool SetGodotClassPropertyValue(in godot_string_name name, in godot_variant value)
	{
		if (name == PropertyName.PropTexture)
		{
			PropTexture = VariantUtils.ConvertTo<Texture2D>(in value);
			return true;
		}
		if (name == PropertyName.gameHandler)
		{
			gameHandler = VariantUtils.ConvertTo<DR_GameHandler>(in value);
			return true;
		}
		if (name == PropertyName.propData)
		{
			propData = VariantUtils.ConvertTo<DR_PropDataRes>(in value);
			return true;
		}
		if (name == PropertyName.speed)
		{
			speed = VariantUtils.ConvertTo<float>(in value);
			return true;
		}
		if (name == PropertyName._sprite)
		{
			_sprite = VariantUtils.ConvertTo<Sprite2D>(in value);
			return true;
		}
		if (name == PropertyName._animatedSprite)
		{
			_animatedSprite = VariantUtils.ConvertTo<AnimatedSprite2D>(in value);
			return true;
		}
		if (name == PropertyName._collisionShape)
		{
			_collisionShape = VariantUtils.ConvertTo<CollisionShape2D>(in value);
			return true;
		}
		return base.SetGodotClassPropertyValue(in name, in value);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override bool GetGodotClassPropertyValue(in godot_string_name name, out godot_variant value)
	{
		if (name == PropertyName.PropTexture)
		{
			Texture2D from = PropTexture;
			value = VariantUtils.CreateFrom(in from);
			return true;
		}
		if (name == PropertyName.gameHandler)
		{
			value = VariantUtils.CreateFrom(in gameHandler);
			return true;
		}
		if (name == PropertyName.propData)
		{
			value = VariantUtils.CreateFrom(in propData);
			return true;
		}
		if (name == PropertyName.speed)
		{
			value = VariantUtils.CreateFrom(in speed);
			return true;
		}
		if (name == PropertyName._sprite)
		{
			value = VariantUtils.CreateFrom(in _sprite);
			return true;
		}
		if (name == PropertyName._animatedSprite)
		{
			value = VariantUtils.CreateFrom(in _animatedSprite);
			return true;
		}
		if (name == PropertyName._collisionShape)
		{
			value = VariantUtils.CreateFrom(in _collisionShape);
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
			new PropertyInfo(Variant.Type.Object, PropertyName.propData, PropertyHint.ResourceType, "DR_PropDataRes", PropertyUsageFlags.Default | PropertyUsageFlags.ScriptVariable, exported: true),
			new PropertyInfo(Variant.Type.Object, PropertyName.PropTexture, PropertyHint.ResourceType, "Texture2D", PropertyUsageFlags.Default | PropertyUsageFlags.ScriptVariable, exported: true),
			new PropertyInfo(Variant.Type.Float, PropertyName.speed, PropertyHint.None, "", PropertyUsageFlags.Default | PropertyUsageFlags.ScriptVariable, exported: true),
			new PropertyInfo(Variant.Type.Object, PropertyName._sprite, PropertyHint.NodeType, "Sprite2D", PropertyUsageFlags.Default | PropertyUsageFlags.ScriptVariable, exported: true),
			new PropertyInfo(Variant.Type.Object, PropertyName._animatedSprite, PropertyHint.NodeType, "AnimatedSprite2D", PropertyUsageFlags.Default | PropertyUsageFlags.ScriptVariable, exported: true),
			new PropertyInfo(Variant.Type.Object, PropertyName._collisionShape, PropertyHint.NodeType, "CollisionShape2D", PropertyUsageFlags.Default | PropertyUsageFlags.ScriptVariable, exported: true)
		};
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override void SaveGodotObjectData(GodotSerializationInfo info)
	{
		base.SaveGodotObjectData(info);
		StringName propTexture = PropertyName.PropTexture;
		Texture2D from = PropTexture;
		info.AddProperty(propTexture, Variant.From(in from));
		info.AddProperty(PropertyName.gameHandler, Variant.From(in gameHandler));
		info.AddProperty(PropertyName.propData, Variant.From(in propData));
		info.AddProperty(PropertyName.speed, Variant.From(in speed));
		info.AddProperty(PropertyName._sprite, Variant.From(in _sprite));
		info.AddProperty(PropertyName._animatedSprite, Variant.From(in _animatedSprite));
		info.AddProperty(PropertyName._collisionShape, Variant.From(in _collisionShape));
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override void RestoreGodotObjectData(GodotSerializationInfo info)
	{
		base.RestoreGodotObjectData(info);
		if (info.TryGetProperty(PropertyName.PropTexture, out var value))
		{
			PropTexture = value.As<Texture2D>();
		}
		if (info.TryGetProperty(PropertyName.gameHandler, out var value2))
		{
			gameHandler = value2.As<DR_GameHandler>();
		}
		if (info.TryGetProperty(PropertyName.propData, out var value3))
		{
			propData = value3.As<DR_PropDataRes>();
		}
		if (info.TryGetProperty(PropertyName.speed, out var value4))
		{
			speed = value4.As<float>();
		}
		if (info.TryGetProperty(PropertyName._sprite, out var value5))
		{
			_sprite = value5.As<Sprite2D>();
		}
		if (info.TryGetProperty(PropertyName._animatedSprite, out var value6))
		{
			_animatedSprite = value6.As<AnimatedSprite2D>();
		}
		if (info.TryGetProperty(PropertyName._collisionShape, out var value7))
		{
			_collisionShape = value7.As<CollisionShape2D>();
		}
	}
}
