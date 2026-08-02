using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text.RegularExpressions;
using Godot;
using Godot.Bridge;
using Godot.NativeInterop;

[GlobalClass]
[Tool]
[ScriptPath("res://Scripts/Tool/TimeSpinBox.cs")]
public class TimeSpinBox : LineEdit
{
	[Signal]
	public delegate void ValueCommittedEventHandler(int totalSeconds);

	public new class MethodName : LineEdit.MethodName
	{
		public new static readonly StringName _Ready = "_Ready";

		public new static readonly StringName _PhysicsProcess = "_PhysicsProcess";

		public static readonly StringName OnTextChanged = "OnTextChanged";

		public static readonly StringName OnTextSubmitted = "OnTextSubmitted";

		public static readonly StringName OnFocusExited = "OnFocusExited";

		public static readonly StringName CommitFromText = "CommitFromText";

		public static readonly StringName FormatTime = "FormatTime";

		public static readonly StringName StartStopwatch = "StartStopwatch";

		public static readonly StringName StopStopwatch = "StopStopwatch";

		public static readonly StringName ResetStopwatch = "ResetStopwatch";
	}

	public new class PropertyName : LineEdit.PropertyName
	{
		public static readonly StringName TotalSeconds = "TotalSeconds";

		public static readonly StringName _running = "_running";

		public static readonly StringName _secondsAccumulator = "_secondsAccumulator";
	}

	public new class SignalName : LineEdit.SignalName
	{
		public static readonly StringName ValueCommitted = "ValueCommitted";
	}

	private bool _running;

	private double _secondsAccumulator;

	private ValueCommittedEventHandler backing_ValueCommitted;

	[Export(PropertyHint.None, "")]
	public int TotalSeconds { get; private set; }

	public event ValueCommittedEventHandler ValueCommitted
	{
		add
		{
			backing_ValueCommitted = (ValueCommittedEventHandler)Delegate.Combine(backing_ValueCommitted, value);
		}
		remove
		{
			backing_ValueCommitted = (ValueCommittedEventHandler)Delegate.Remove(backing_ValueCommitted, value);
		}
	}

	public override void _Ready()
	{
		base.FocusMode = FocusModeEnum.All;
		base.Editable = true;
		base.TextSubmitted += OnTextSubmitted;
		base.FocusExited += OnFocusExited;
		base.TextChanged += OnTextChanged;
	}

	public override void _PhysicsProcess(double delta)
	{
		if (!_running || TotalSeconds <= 0)
		{
			return;
		}
		_secondsAccumulator += delta;
		if (_secondsAccumulator >= 1.0)
		{
			int num = (int)_secondsAccumulator;
			TotalSeconds -= num;
			_secondsAccumulator -= num;
			if (TotalSeconds <= 0)
			{
				TotalSeconds = 0;
				_running = false;
			}
			base.Text = FormatTime(TotalSeconds);
			base.TooltipText = $"{TotalSeconds} seconds";
			EmitSignal("ValueCommitted", TotalSeconds);
		}
	}

	private void OnTextChanged(string newText)
	{
		string text = Regex.Replace(newText, "[^\\d]", "");
		if (text != newText)
		{
			base.Text = text;
			base.CaretColumn = text.Length;
		}
	}

	private void OnTextSubmitted(string text)
	{
		CommitFromText(text);
		ReleaseFocus();
	}

	private void OnFocusExited()
	{
		CommitFromText(base.Text);
	}

	private void CommitFromText(string text)
	{
		if (!int.TryParse(text, out var result))
		{
			result = TotalSeconds;
		}
		TotalSeconds = Math.Max(0, result);
		base.Text = FormatTime(TotalSeconds);
		EmitSignal("ValueCommitted", TotalSeconds);
	}

	private string FormatTime(int totalSeconds)
	{
		int num = totalSeconds / 3600;
		int num2 = totalSeconds % 3600 / 60;
		int value = totalSeconds % 60;
		if (num <= 0)
		{
			if (num2 <= 0)
			{
				return $"{value}";
			}
			return $"{num2}:{value:D2}";
		}
		return $"{num}.{num2:D2}:{value:D2}";
	}

	public void StartStopwatch()
	{
		if (TotalSeconds > 0)
		{
			base.FocusMode = FocusModeEnum.None;
			base.Editable = false;
			_running = true;
		}
	}

	public void StopStopwatch()
	{
		base.FocusMode = FocusModeEnum.All;
		base.Editable = true;
		_running = false;
	}

	public void ResetStopwatch(int seconds = 0)
	{
		TotalSeconds = Math.Max(0, seconds);
		_running = false;
		base.Text = FormatTime(TotalSeconds);
		base.TooltipText = $"{TotalSeconds} seconds";
		EmitSignal("ValueCommitted", TotalSeconds);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	internal static List<MethodInfo> GetGodotMethodList()
	{
		return new List<MethodInfo>(10)
		{
			new MethodInfo(MethodName._Ready, new PropertyInfo(Variant.Type.Nil, "", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false), MethodFlags.Normal, null, null),
			new MethodInfo(MethodName._PhysicsProcess, new PropertyInfo(Variant.Type.Nil, "", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false), MethodFlags.Normal, new List<PropertyInfo>
			{
				new PropertyInfo(Variant.Type.Float, "delta", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false)
			}, null),
			new MethodInfo(MethodName.OnTextChanged, new PropertyInfo(Variant.Type.Nil, "", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false), MethodFlags.Normal, new List<PropertyInfo>
			{
				new PropertyInfo(Variant.Type.String, "newText", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false)
			}, null),
			new MethodInfo(MethodName.OnTextSubmitted, new PropertyInfo(Variant.Type.Nil, "", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false), MethodFlags.Normal, new List<PropertyInfo>
			{
				new PropertyInfo(Variant.Type.String, "text", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false)
			}, null),
			new MethodInfo(MethodName.OnFocusExited, new PropertyInfo(Variant.Type.Nil, "", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false), MethodFlags.Normal, null, null),
			new MethodInfo(MethodName.CommitFromText, new PropertyInfo(Variant.Type.Nil, "", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false), MethodFlags.Normal, new List<PropertyInfo>
			{
				new PropertyInfo(Variant.Type.String, "text", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false)
			}, null),
			new MethodInfo(MethodName.FormatTime, new PropertyInfo(Variant.Type.String, "", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false), MethodFlags.Normal, new List<PropertyInfo>
			{
				new PropertyInfo(Variant.Type.Int, "totalSeconds", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false)
			}, null),
			new MethodInfo(MethodName.StartStopwatch, new PropertyInfo(Variant.Type.Nil, "", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false), MethodFlags.Normal, null, null),
			new MethodInfo(MethodName.StopStopwatch, new PropertyInfo(Variant.Type.Nil, "", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false), MethodFlags.Normal, null, null),
			new MethodInfo(MethodName.ResetStopwatch, new PropertyInfo(Variant.Type.Nil, "", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false), MethodFlags.Normal, new List<PropertyInfo>
			{
				new PropertyInfo(Variant.Type.Int, "seconds", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false)
			}, null)
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
		if (method == MethodName._PhysicsProcess && args.Count == 1)
		{
			_PhysicsProcess(VariantUtils.ConvertTo<double>(in args[0]));
			ret = default(godot_variant);
			return true;
		}
		if (method == MethodName.OnTextChanged && args.Count == 1)
		{
			OnTextChanged(VariantUtils.ConvertTo<string>(in args[0]));
			ret = default(godot_variant);
			return true;
		}
		if (method == MethodName.OnTextSubmitted && args.Count == 1)
		{
			OnTextSubmitted(VariantUtils.ConvertTo<string>(in args[0]));
			ret = default(godot_variant);
			return true;
		}
		if (method == MethodName.OnFocusExited && args.Count == 0)
		{
			OnFocusExited();
			ret = default(godot_variant);
			return true;
		}
		if (method == MethodName.CommitFromText && args.Count == 1)
		{
			CommitFromText(VariantUtils.ConvertTo<string>(in args[0]));
			ret = default(godot_variant);
			return true;
		}
		if (method == MethodName.FormatTime && args.Count == 1)
		{
			string from = FormatTime(VariantUtils.ConvertTo<int>(in args[0]));
			ret = VariantUtils.CreateFrom(in from);
			return true;
		}
		if (method == MethodName.StartStopwatch && args.Count == 0)
		{
			StartStopwatch();
			ret = default(godot_variant);
			return true;
		}
		if (method == MethodName.StopStopwatch && args.Count == 0)
		{
			StopStopwatch();
			ret = default(godot_variant);
			return true;
		}
		if (method == MethodName.ResetStopwatch && args.Count == 1)
		{
			ResetStopwatch(VariantUtils.ConvertTo<int>(in args[0]));
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
		if (method == MethodName._PhysicsProcess)
		{
			return true;
		}
		if (method == MethodName.OnTextChanged)
		{
			return true;
		}
		if (method == MethodName.OnTextSubmitted)
		{
			return true;
		}
		if (method == MethodName.OnFocusExited)
		{
			return true;
		}
		if (method == MethodName.CommitFromText)
		{
			return true;
		}
		if (method == MethodName.FormatTime)
		{
			return true;
		}
		if (method == MethodName.StartStopwatch)
		{
			return true;
		}
		if (method == MethodName.StopStopwatch)
		{
			return true;
		}
		if (method == MethodName.ResetStopwatch)
		{
			return true;
		}
		return base.HasGodotClassMethod(in method);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override bool SetGodotClassPropertyValue(in godot_string_name name, in godot_variant value)
	{
		if (name == PropertyName.TotalSeconds)
		{
			TotalSeconds = VariantUtils.ConvertTo<int>(in value);
			return true;
		}
		if (name == PropertyName._running)
		{
			_running = VariantUtils.ConvertTo<bool>(in value);
			return true;
		}
		if (name == PropertyName._secondsAccumulator)
		{
			_secondsAccumulator = VariantUtils.ConvertTo<double>(in value);
			return true;
		}
		return base.SetGodotClassPropertyValue(in name, in value);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override bool GetGodotClassPropertyValue(in godot_string_name name, out godot_variant value)
	{
		if (name == PropertyName.TotalSeconds)
		{
			int from = TotalSeconds;
			value = VariantUtils.CreateFrom(in from);
			return true;
		}
		if (name == PropertyName._running)
		{
			value = VariantUtils.CreateFrom(in _running);
			return true;
		}
		if (name == PropertyName._secondsAccumulator)
		{
			value = VariantUtils.CreateFrom(in _secondsAccumulator);
			return true;
		}
		return base.GetGodotClassPropertyValue(in name, out value);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	internal static List<PropertyInfo> GetGodotPropertyList()
	{
		return new List<PropertyInfo>
		{
			new PropertyInfo(Variant.Type.Int, PropertyName.TotalSeconds, PropertyHint.None, "", PropertyUsageFlags.Default | PropertyUsageFlags.ScriptVariable, exported: true),
			new PropertyInfo(Variant.Type.Bool, PropertyName._running, PropertyHint.None, "", PropertyUsageFlags.ScriptVariable, exported: false),
			new PropertyInfo(Variant.Type.Float, PropertyName._secondsAccumulator, PropertyHint.None, "", PropertyUsageFlags.ScriptVariable, exported: false)
		};
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override void SaveGodotObjectData(GodotSerializationInfo info)
	{
		base.SaveGodotObjectData(info);
		StringName totalSeconds = PropertyName.TotalSeconds;
		int from = TotalSeconds;
		info.AddProperty(totalSeconds, Variant.From(in from));
		info.AddProperty(PropertyName._running, Variant.From(in _running));
		info.AddProperty(PropertyName._secondsAccumulator, Variant.From(in _secondsAccumulator));
		info.AddSignalEventDelegate(SignalName.ValueCommitted, backing_ValueCommitted);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override void RestoreGodotObjectData(GodotSerializationInfo info)
	{
		base.RestoreGodotObjectData(info);
		if (info.TryGetProperty(PropertyName.TotalSeconds, out var value))
		{
			TotalSeconds = value.As<int>();
		}
		if (info.TryGetProperty(PropertyName._running, out var value2))
		{
			_running = value2.As<bool>();
		}
		if (info.TryGetProperty(PropertyName._secondsAccumulator, out var value3))
		{
			_secondsAccumulator = value3.As<double>();
		}
		if (info.TryGetSignalEventDelegate<ValueCommittedEventHandler>(SignalName.ValueCommitted, out var value4))
		{
			backing_ValueCommitted = value4;
		}
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	internal static List<MethodInfo> GetGodotSignalList()
	{
		return new List<MethodInfo>(1)
		{
			new MethodInfo(SignalName.ValueCommitted, new PropertyInfo(Variant.Type.Nil, "", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false), MethodFlags.Normal, new List<PropertyInfo>
			{
				new PropertyInfo(Variant.Type.Int, "totalSeconds", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false)
			}, null)
		};
	}

	protected void EmitSignalValueCommitted(int totalSeconds)
	{
		EmitSignal(SignalName.ValueCommitted, totalSeconds);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override void RaiseGodotClassSignalCallbacks(in godot_string_name signal, NativeVariantPtrArgs args)
	{
		if (signal == SignalName.ValueCommitted && args.Count == 1)
		{
			backing_ValueCommitted?.Invoke(VariantUtils.ConvertTo<int>(in args[0]));
		}
		else
		{
			base.RaiseGodotClassSignalCallbacks(in signal, args);
		}
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override bool HasGodotClassSignal(in godot_string_name signal)
	{
		if (signal == SignalName.ValueCommitted)
		{
			return true;
		}
		return base.HasGodotClassSignal(in signal);
	}
}
