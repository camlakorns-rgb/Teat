using System;
using System.Collections.Generic;
using System.ComponentModel;
using Godot;
using Godot.Bridge;
using Godot.NativeInterop;

[ScriptPath("res://Scripts/Minigames/LovenseIdle/LI_Clicker.cs")]
public class LI_Clicker : Panel
{
	[Signal]
	public delegate void ClickerPressedEventHandler();

	public new class MethodName : Panel.MethodName
	{
		public new static readonly StringName _Ready = "_Ready";

		public new static readonly StringName _Process = "_Process";

		public static readonly StringName OnButtonPressed = "OnButtonPressed";

		public static readonly StringName ApplyClick = "ApplyClick";

		public static readonly StringName UpdateFloatStats = "UpdateFloatStats";

		public static readonly StringName UpdateLabel = "UpdateLabel";
	}

	public new class PropertyName : Panel.PropertyName
	{
		public static readonly StringName backgroundSprite = "backgroundSprite";

		public static readonly StringName ButtonController = "ButtonController";

		public static readonly StringName HeartsPerSecond = "HeartsPerSecond";

		public static readonly StringName currentHeartsPerSecond = "currentHeartsPerSecond";

		public static readonly StringName decayGracePeriod = "decayGracePeriod";

		public static readonly StringName decayRate = "decayRate";

		public static readonly StringName graceTimeRemaining = "graceTimeRemaining";
	}

	public new class SignalName : Panel.SignalName
	{
		public static readonly StringName ClickerPressed = "ClickerPressed";
	}

	[Export(PropertyHint.None, "")]
	public Sprite2D backgroundSprite;

	[Export(PropertyHint.None, "")]
	public Control ButtonController;

	[Export(PropertyHint.None, "")]
	public RichTextLabel HeartsPerSecond;

	public float currentHeartsPerSecond;

	public float decayGracePeriod;

	public float decayRate;

	private float graceTimeRemaining;

	private ClickerPressedEventHandler backing_ClickerPressed;

	public event ClickerPressedEventHandler ClickerPressed
	{
		add
		{
			backing_ClickerPressed = (ClickerPressedEventHandler)Delegate.Combine(backing_ClickerPressed, value);
		}
		remove
		{
			backing_ClickerPressed = (ClickerPressedEventHandler)Delegate.Remove(backing_ClickerPressed, value);
		}
	}

	public override void _Ready()
	{
		UpdateLabel();
	}

	public override void _Process(double delta)
	{
		if (!(currentHeartsPerSecond <= 0f))
		{
			if (graceTimeRemaining > 0f)
			{
				graceTimeRemaining -= (float)delta;
				return;
			}
			currentHeartsPerSecond = Mathf.Max(0f, currentHeartsPerSecond - decayRate * (float)delta);
			UpdateLabel();
		}
	}

	private void OnButtonPressed()
	{
		EmitSignal(SignalName.ClickerPressed);
	}

	public void ApplyClick(float powerCapValue)
	{
		currentHeartsPerSecond = powerCapValue;
		graceTimeRemaining = decayGracePeriod;
		UpdateLabel();
	}

	public void UpdateFloatStats(float newGracePeriod, float newDecayRate)
	{
		decayGracePeriod = newGracePeriod;
		decayRate = newDecayRate;
	}

	private void UpdateLabel()
	{
		if (HeartsPerSecond != null)
		{
			HeartsPerSecond.Text = $"{currentHeartsPerSecond:0.0}/s";
		}
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	internal static List<MethodInfo> GetGodotMethodList()
	{
		return new List<MethodInfo>(6)
		{
			new MethodInfo(MethodName._Ready, new PropertyInfo(Variant.Type.Nil, "", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false), MethodFlags.Normal, null, null),
			new MethodInfo(MethodName._Process, new PropertyInfo(Variant.Type.Nil, "", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false), MethodFlags.Normal, new List<PropertyInfo>
			{
				new PropertyInfo(Variant.Type.Float, "delta", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false)
			}, null),
			new MethodInfo(MethodName.OnButtonPressed, new PropertyInfo(Variant.Type.Nil, "", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false), MethodFlags.Normal, null, null),
			new MethodInfo(MethodName.ApplyClick, new PropertyInfo(Variant.Type.Nil, "", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false), MethodFlags.Normal, new List<PropertyInfo>
			{
				new PropertyInfo(Variant.Type.Float, "powerCapValue", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false)
			}, null),
			new MethodInfo(MethodName.UpdateFloatStats, new PropertyInfo(Variant.Type.Nil, "", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false), MethodFlags.Normal, new List<PropertyInfo>
			{
				new PropertyInfo(Variant.Type.Float, "newGracePeriod", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false),
				new PropertyInfo(Variant.Type.Float, "newDecayRate", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false)
			}, null),
			new MethodInfo(MethodName.UpdateLabel, new PropertyInfo(Variant.Type.Nil, "", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false), MethodFlags.Normal, null, null)
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
		if (method == MethodName.OnButtonPressed && args.Count == 0)
		{
			OnButtonPressed();
			ret = default(godot_variant);
			return true;
		}
		if (method == MethodName.ApplyClick && args.Count == 1)
		{
			ApplyClick(VariantUtils.ConvertTo<float>(in args[0]));
			ret = default(godot_variant);
			return true;
		}
		if (method == MethodName.UpdateFloatStats && args.Count == 2)
		{
			UpdateFloatStats(VariantUtils.ConvertTo<float>(in args[0]), VariantUtils.ConvertTo<float>(in args[1]));
			ret = default(godot_variant);
			return true;
		}
		if (method == MethodName.UpdateLabel && args.Count == 0)
		{
			UpdateLabel();
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
		if (method == MethodName.OnButtonPressed)
		{
			return true;
		}
		if (method == MethodName.ApplyClick)
		{
			return true;
		}
		if (method == MethodName.UpdateFloatStats)
		{
			return true;
		}
		if (method == MethodName.UpdateLabel)
		{
			return true;
		}
		return base.HasGodotClassMethod(in method);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override bool SetGodotClassPropertyValue(in godot_string_name name, in godot_variant value)
	{
		if (name == PropertyName.backgroundSprite)
		{
			backgroundSprite = VariantUtils.ConvertTo<Sprite2D>(in value);
			return true;
		}
		if (name == PropertyName.ButtonController)
		{
			ButtonController = VariantUtils.ConvertTo<Control>(in value);
			return true;
		}
		if (name == PropertyName.HeartsPerSecond)
		{
			HeartsPerSecond = VariantUtils.ConvertTo<RichTextLabel>(in value);
			return true;
		}
		if (name == PropertyName.currentHeartsPerSecond)
		{
			currentHeartsPerSecond = VariantUtils.ConvertTo<float>(in value);
			return true;
		}
		if (name == PropertyName.decayGracePeriod)
		{
			decayGracePeriod = VariantUtils.ConvertTo<float>(in value);
			return true;
		}
		if (name == PropertyName.decayRate)
		{
			decayRate = VariantUtils.ConvertTo<float>(in value);
			return true;
		}
		if (name == PropertyName.graceTimeRemaining)
		{
			graceTimeRemaining = VariantUtils.ConvertTo<float>(in value);
			return true;
		}
		return base.SetGodotClassPropertyValue(in name, in value);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override bool GetGodotClassPropertyValue(in godot_string_name name, out godot_variant value)
	{
		if (name == PropertyName.backgroundSprite)
		{
			value = VariantUtils.CreateFrom(in backgroundSprite);
			return true;
		}
		if (name == PropertyName.ButtonController)
		{
			value = VariantUtils.CreateFrom(in ButtonController);
			return true;
		}
		if (name == PropertyName.HeartsPerSecond)
		{
			value = VariantUtils.CreateFrom(in HeartsPerSecond);
			return true;
		}
		if (name == PropertyName.currentHeartsPerSecond)
		{
			value = VariantUtils.CreateFrom(in currentHeartsPerSecond);
			return true;
		}
		if (name == PropertyName.decayGracePeriod)
		{
			value = VariantUtils.CreateFrom(in decayGracePeriod);
			return true;
		}
		if (name == PropertyName.decayRate)
		{
			value = VariantUtils.CreateFrom(in decayRate);
			return true;
		}
		if (name == PropertyName.graceTimeRemaining)
		{
			value = VariantUtils.CreateFrom(in graceTimeRemaining);
			return true;
		}
		return base.GetGodotClassPropertyValue(in name, out value);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	internal static List<PropertyInfo> GetGodotPropertyList()
	{
		return new List<PropertyInfo>
		{
			new PropertyInfo(Variant.Type.Object, PropertyName.backgroundSprite, PropertyHint.NodeType, "Sprite2D", PropertyUsageFlags.Default | PropertyUsageFlags.ScriptVariable, exported: true),
			new PropertyInfo(Variant.Type.Object, PropertyName.ButtonController, PropertyHint.NodeType, "Control", PropertyUsageFlags.Default | PropertyUsageFlags.ScriptVariable, exported: true),
			new PropertyInfo(Variant.Type.Object, PropertyName.HeartsPerSecond, PropertyHint.NodeType, "RichTextLabel", PropertyUsageFlags.Default | PropertyUsageFlags.ScriptVariable, exported: true),
			new PropertyInfo(Variant.Type.Float, PropertyName.currentHeartsPerSecond, PropertyHint.None, "", PropertyUsageFlags.ScriptVariable, exported: false),
			new PropertyInfo(Variant.Type.Float, PropertyName.decayGracePeriod, PropertyHint.None, "", PropertyUsageFlags.ScriptVariable, exported: false),
			new PropertyInfo(Variant.Type.Float, PropertyName.decayRate, PropertyHint.None, "", PropertyUsageFlags.ScriptVariable, exported: false),
			new PropertyInfo(Variant.Type.Float, PropertyName.graceTimeRemaining, PropertyHint.None, "", PropertyUsageFlags.ScriptVariable, exported: false)
		};
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override void SaveGodotObjectData(GodotSerializationInfo info)
	{
		base.SaveGodotObjectData(info);
		info.AddProperty(PropertyName.backgroundSprite, Variant.From(in backgroundSprite));
		info.AddProperty(PropertyName.ButtonController, Variant.From(in ButtonController));
		info.AddProperty(PropertyName.HeartsPerSecond, Variant.From(in HeartsPerSecond));
		info.AddProperty(PropertyName.currentHeartsPerSecond, Variant.From(in currentHeartsPerSecond));
		info.AddProperty(PropertyName.decayGracePeriod, Variant.From(in decayGracePeriod));
		info.AddProperty(PropertyName.decayRate, Variant.From(in decayRate));
		info.AddProperty(PropertyName.graceTimeRemaining, Variant.From(in graceTimeRemaining));
		info.AddSignalEventDelegate(SignalName.ClickerPressed, backing_ClickerPressed);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override void RestoreGodotObjectData(GodotSerializationInfo info)
	{
		base.RestoreGodotObjectData(info);
		if (info.TryGetProperty(PropertyName.backgroundSprite, out var value))
		{
			backgroundSprite = value.As<Sprite2D>();
		}
		if (info.TryGetProperty(PropertyName.ButtonController, out var value2))
		{
			ButtonController = value2.As<Control>();
		}
		if (info.TryGetProperty(PropertyName.HeartsPerSecond, out var value3))
		{
			HeartsPerSecond = value3.As<RichTextLabel>();
		}
		if (info.TryGetProperty(PropertyName.currentHeartsPerSecond, out var value4))
		{
			currentHeartsPerSecond = value4.As<float>();
		}
		if (info.TryGetProperty(PropertyName.decayGracePeriod, out var value5))
		{
			decayGracePeriod = value5.As<float>();
		}
		if (info.TryGetProperty(PropertyName.decayRate, out var value6))
		{
			decayRate = value6.As<float>();
		}
		if (info.TryGetProperty(PropertyName.graceTimeRemaining, out var value7))
		{
			graceTimeRemaining = value7.As<float>();
		}
		if (info.TryGetSignalEventDelegate<ClickerPressedEventHandler>(SignalName.ClickerPressed, out var value8))
		{
			backing_ClickerPressed = value8;
		}
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	internal static List<MethodInfo> GetGodotSignalList()
	{
		return new List<MethodInfo>(1)
		{
			new MethodInfo(SignalName.ClickerPressed, new PropertyInfo(Variant.Type.Nil, "", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false), MethodFlags.Normal, null, null)
		};
	}

	protected void EmitSignalClickerPressed()
	{
		EmitSignal(SignalName.ClickerPressed);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override void RaiseGodotClassSignalCallbacks(in godot_string_name signal, NativeVariantPtrArgs args)
	{
		if (signal == SignalName.ClickerPressed && args.Count == 0)
		{
			backing_ClickerPressed?.Invoke();
		}
		else
		{
			base.RaiseGodotClassSignalCallbacks(in signal, args);
		}
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override bool HasGodotClassSignal(in godot_string_name signal)
	{
		if (signal == SignalName.ClickerPressed)
		{
			return true;
		}
		return base.HasGodotClassSignal(in signal);
	}
}
