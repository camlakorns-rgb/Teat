using System.Collections.Generic;
using System.ComponentModel;
using Godot;
using Godot.Bridge;
using Godot.NativeInterop;

[ScriptPath("res://Scripts/Minigames/Helpers/FloatingText.cs")]
public class FloatingText : Node2D
{
	public new class MethodName : Node2D.MethodName
	{
		public new static readonly StringName _Ready = "_Ready";

		public new static readonly StringName _Process = "_Process";

		public static readonly StringName Destroy = "Destroy";
	}

	public new class PropertyName : Node2D.PropertyName
	{
		public static readonly StringName Text = "Text";

		public static readonly StringName _velocity = "_velocity";

		public static readonly StringName _gravity = "_gravity";

		public static readonly StringName _mass = "_mass";
	}

	public new class SignalName : Node2D.SignalName
	{
	}

	private Vector2 _velocity = new Vector2(50f, -100f);

	private Vector2 _gravity = new Vector2(0f, 1f);

	private float _mass = 200f;

	public string Text
	{
		get
		{
			return GetNode<RichTextLabel>("Label").Text;
		}
		set
		{
			GetNode<RichTextLabel>("Label").Text = value;
		}
	}

	public override void _Ready()
	{
		Tween tween = CreateTween();
		tween.SetParallel();
		tween.TweenProperty(this, "modulate:a", 0f, 0.30000001192092896).SetTrans(Tween.TransitionType.Linear).SetEase(Tween.EaseType.Out)
			.SetDelay(0.699999988079071);
		tween.TweenProperty(this, "scale", new Vector2(1f, 1f), 0.30000001192092896).From(Vector2.Zero).SetTrans(Tween.TransitionType.Quart)
			.SetEase(Tween.EaseType.Out);
		tween.TweenProperty(this, "scale", new Vector2(0.4f, 0.4f), 1.0).SetTrans(Tween.TransitionType.Linear).SetEase(Tween.EaseType.Out)
			.SetDelay(0.6000000238418579);
		tween.TweenCallback(Callable.From(Destroy)).SetDelay(1.0);
	}

	public override void _Process(double delta)
	{
		float num = (float)delta;
		_velocity += _gravity * _mass * num;
		base.Position += _velocity * num;
	}

	private void Destroy()
	{
		QueueFree();
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	internal static List<MethodInfo> GetGodotMethodList()
	{
		return new List<MethodInfo>(3)
		{
			new MethodInfo(MethodName._Ready, new PropertyInfo(Variant.Type.Nil, "", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false), MethodFlags.Normal, null, null),
			new MethodInfo(MethodName._Process, new PropertyInfo(Variant.Type.Nil, "", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false), MethodFlags.Normal, new List<PropertyInfo>
			{
				new PropertyInfo(Variant.Type.Float, "delta", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false)
			}, null),
			new MethodInfo(MethodName.Destroy, new PropertyInfo(Variant.Type.Nil, "", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false), MethodFlags.Normal, null, null)
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
		if (method == MethodName.Destroy && args.Count == 0)
		{
			Destroy();
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
		if (method == MethodName._Process)
		{
			return true;
		}
		if (method == MethodName.Destroy)
		{
			return true;
		}
		return base.HasGodotClassMethod(in method);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override bool SetGodotClassPropertyValue(in godot_string_name name, in godot_variant value)
	{
		if (name == PropertyName.Text)
		{
			Text = VariantUtils.ConvertTo<string>(in value);
			return true;
		}
		if (name == PropertyName._velocity)
		{
			_velocity = VariantUtils.ConvertTo<Vector2>(in value);
			return true;
		}
		if (name == PropertyName._gravity)
		{
			_gravity = VariantUtils.ConvertTo<Vector2>(in value);
			return true;
		}
		if (name == PropertyName._mass)
		{
			_mass = VariantUtils.ConvertTo<float>(in value);
			return true;
		}
		return base.SetGodotClassPropertyValue(in name, in value);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override bool GetGodotClassPropertyValue(in godot_string_name name, out godot_variant value)
	{
		if (name == PropertyName.Text)
		{
			string from = Text;
			value = VariantUtils.CreateFrom(in from);
			return true;
		}
		if (name == PropertyName._velocity)
		{
			value = VariantUtils.CreateFrom(in _velocity);
			return true;
		}
		if (name == PropertyName._gravity)
		{
			value = VariantUtils.CreateFrom(in _gravity);
			return true;
		}
		if (name == PropertyName._mass)
		{
			value = VariantUtils.CreateFrom(in _mass);
			return true;
		}
		return base.GetGodotClassPropertyValue(in name, out value);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	internal static List<PropertyInfo> GetGodotPropertyList()
	{
		return new List<PropertyInfo>
		{
			new PropertyInfo(Variant.Type.Vector2, PropertyName._velocity, PropertyHint.None, "", PropertyUsageFlags.ScriptVariable, exported: false),
			new PropertyInfo(Variant.Type.Vector2, PropertyName._gravity, PropertyHint.None, "", PropertyUsageFlags.ScriptVariable, exported: false),
			new PropertyInfo(Variant.Type.Float, PropertyName._mass, PropertyHint.None, "", PropertyUsageFlags.ScriptVariable, exported: false),
			new PropertyInfo(Variant.Type.String, PropertyName.Text, PropertyHint.None, "", PropertyUsageFlags.ScriptVariable, exported: false)
		};
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override void SaveGodotObjectData(GodotSerializationInfo info)
	{
		base.SaveGodotObjectData(info);
		StringName text = PropertyName.Text;
		string from = Text;
		info.AddProperty(text, Variant.From(in from));
		info.AddProperty(PropertyName._velocity, Variant.From(in _velocity));
		info.AddProperty(PropertyName._gravity, Variant.From(in _gravity));
		info.AddProperty(PropertyName._mass, Variant.From(in _mass));
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override void RestoreGodotObjectData(GodotSerializationInfo info)
	{
		base.RestoreGodotObjectData(info);
		if (info.TryGetProperty(PropertyName.Text, out var value))
		{
			Text = value.As<string>();
		}
		if (info.TryGetProperty(PropertyName._velocity, out var value2))
		{
			_velocity = value2.As<Vector2>();
		}
		if (info.TryGetProperty(PropertyName._gravity, out var value3))
		{
			_gravity = value3.As<Vector2>();
		}
		if (info.TryGetProperty(PropertyName._mass, out var value4))
		{
			_mass = value4.As<float>();
		}
	}
}
