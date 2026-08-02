using System;
using System.Collections.Generic;
using System.ComponentModel;
using Godot;
using Godot.Bridge;
using Godot.NativeInterop;

[ScriptPath("res://Scripts/SubMenus/ConfirmationMenu/ConfirmationMenu.cs")]
public class ConfirmationMenu : Window
{
	[Signal]
	public delegate void ConfirmedEventHandler();

	[Signal]
	public delegate void DenyEventHandler();

	public new class MethodName : Window.MethodName
	{
		public new static readonly StringName _Process = "_Process";

		public static readonly StringName ConfirmClose = "ConfirmClose";

		public static readonly StringName OnClose = "OnClose";

		public static readonly StringName SetCharacterName = "SetCharacterName";
	}

	public new class PropertyName : Window.PropertyName
	{
		public static readonly StringName label = "label";

		public static readonly StringName UnpauseOnClose = "UnpauseOnClose";
	}

	public new class SignalName : Window.SignalName
	{
		public static readonly StringName Confirmed = "Confirmed";

		public static readonly StringName Deny = "Deny";
	}

	[Export(PropertyHint.None, "")]
	public RichTextLabel label;

	public bool UnpauseOnClose = true;

	private ConfirmedEventHandler backing_Confirmed;

	private DenyEventHandler backing_Deny;

	public event ConfirmedEventHandler Confirmed
	{
		add
		{
			backing_Confirmed = (ConfirmedEventHandler)Delegate.Combine(backing_Confirmed, value);
		}
		remove
		{
			backing_Confirmed = (ConfirmedEventHandler)Delegate.Remove(backing_Confirmed, value);
		}
	}

	public event DenyEventHandler Deny
	{
		add
		{
			backing_Deny = (DenyEventHandler)Delegate.Combine(backing_Deny, value);
		}
		remove
		{
			backing_Deny = (DenyEventHandler)Delegate.Remove(backing_Deny, value);
		}
	}

	public override void _Process(double delta)
	{
		if (Input.IsActionJustPressed("PauseGame"))
		{
			OnClose();
		}
	}

	public void ConfirmClose()
	{
		if (UnpauseOnClose)
		{
			GetTree().Paused = false;
		}
		EmitSignal(SignalName.Confirmed);
		Main.Instance.mainWindow.GrabFocus();
		QueueFree();
	}

	public void OnClose()
	{
		if (UnpauseOnClose)
		{
			GetTree().Paused = false;
		}
		EmitSignal(SignalName.Deny);
		Main.Instance.mainWindow.GrabFocus();
		QueueFree();
	}

	public void SetCharacterName(string newText)
	{
		Main.Instance.userInfoName = newText;
		Main.Instance.saveHandler.SaveSettings();
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	internal static List<MethodInfo> GetGodotMethodList()
	{
		return new List<MethodInfo>(4)
		{
			new MethodInfo(MethodName._Process, new PropertyInfo(Variant.Type.Nil, "", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false), MethodFlags.Normal, new List<PropertyInfo>
			{
				new PropertyInfo(Variant.Type.Float, "delta", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false)
			}, null),
			new MethodInfo(MethodName.ConfirmClose, new PropertyInfo(Variant.Type.Nil, "", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false), MethodFlags.Normal, null, null),
			new MethodInfo(MethodName.OnClose, new PropertyInfo(Variant.Type.Nil, "", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false), MethodFlags.Normal, null, null),
			new MethodInfo(MethodName.SetCharacterName, new PropertyInfo(Variant.Type.Nil, "", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false), MethodFlags.Normal, new List<PropertyInfo>
			{
				new PropertyInfo(Variant.Type.String, "newText", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false)
			}, null)
		};
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override bool InvokeGodotClassMethod(in godot_string_name method, NativeVariantPtrArgs args, out godot_variant ret)
	{
		if (method == MethodName._Process && args.Count == 1)
		{
			_Process(VariantUtils.ConvertTo<double>(in args[0]));
			ret = default(godot_variant);
			return true;
		}
		if (method == MethodName.ConfirmClose && args.Count == 0)
		{
			ConfirmClose();
			ret = default(godot_variant);
			return true;
		}
		if (method == MethodName.OnClose && args.Count == 0)
		{
			OnClose();
			ret = default(godot_variant);
			return true;
		}
		if (method == MethodName.SetCharacterName && args.Count == 1)
		{
			SetCharacterName(VariantUtils.ConvertTo<string>(in args[0]));
			ret = default(godot_variant);
			return true;
		}
		return base.InvokeGodotClassMethod(in method, args, out ret);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override bool HasGodotClassMethod(in godot_string_name method)
	{
		if (method == MethodName._Process)
		{
			return true;
		}
		if (method == MethodName.ConfirmClose)
		{
			return true;
		}
		if (method == MethodName.OnClose)
		{
			return true;
		}
		if (method == MethodName.SetCharacterName)
		{
			return true;
		}
		return base.HasGodotClassMethod(in method);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override bool SetGodotClassPropertyValue(in godot_string_name name, in godot_variant value)
	{
		if (name == PropertyName.label)
		{
			label = VariantUtils.ConvertTo<RichTextLabel>(in value);
			return true;
		}
		if (name == PropertyName.UnpauseOnClose)
		{
			UnpauseOnClose = VariantUtils.ConvertTo<bool>(in value);
			return true;
		}
		return base.SetGodotClassPropertyValue(in name, in value);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override bool GetGodotClassPropertyValue(in godot_string_name name, out godot_variant value)
	{
		if (name == PropertyName.label)
		{
			value = VariantUtils.CreateFrom(in label);
			return true;
		}
		if (name == PropertyName.UnpauseOnClose)
		{
			value = VariantUtils.CreateFrom(in UnpauseOnClose);
			return true;
		}
		return base.GetGodotClassPropertyValue(in name, out value);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	internal static List<PropertyInfo> GetGodotPropertyList()
	{
		return new List<PropertyInfo>
		{
			new PropertyInfo(Variant.Type.Object, PropertyName.label, PropertyHint.NodeType, "RichTextLabel", PropertyUsageFlags.Default | PropertyUsageFlags.ScriptVariable, exported: true),
			new PropertyInfo(Variant.Type.Bool, PropertyName.UnpauseOnClose, PropertyHint.None, "", PropertyUsageFlags.ScriptVariable, exported: false)
		};
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override void SaveGodotObjectData(GodotSerializationInfo info)
	{
		base.SaveGodotObjectData(info);
		info.AddProperty(PropertyName.label, Variant.From(in label));
		info.AddProperty(PropertyName.UnpauseOnClose, Variant.From(in UnpauseOnClose));
		info.AddSignalEventDelegate(SignalName.Confirmed, backing_Confirmed);
		info.AddSignalEventDelegate(SignalName.Deny, backing_Deny);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override void RestoreGodotObjectData(GodotSerializationInfo info)
	{
		base.RestoreGodotObjectData(info);
		if (info.TryGetProperty(PropertyName.label, out var value))
		{
			label = value.As<RichTextLabel>();
		}
		if (info.TryGetProperty(PropertyName.UnpauseOnClose, out var value2))
		{
			UnpauseOnClose = value2.As<bool>();
		}
		if (info.TryGetSignalEventDelegate<ConfirmedEventHandler>(SignalName.Confirmed, out var value3))
		{
			backing_Confirmed = value3;
		}
		if (info.TryGetSignalEventDelegate<DenyEventHandler>(SignalName.Deny, out var value4))
		{
			backing_Deny = value4;
		}
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	internal static List<MethodInfo> GetGodotSignalList()
	{
		return new List<MethodInfo>(2)
		{
			new MethodInfo(SignalName.Confirmed, new PropertyInfo(Variant.Type.Nil, "", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false), MethodFlags.Normal, null, null),
			new MethodInfo(SignalName.Deny, new PropertyInfo(Variant.Type.Nil, "", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false), MethodFlags.Normal, null, null)
		};
	}

	protected void EmitSignalConfirmed()
	{
		EmitSignal(SignalName.Confirmed);
	}

	protected void EmitSignalDeny()
	{
		EmitSignal(SignalName.Deny);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override void RaiseGodotClassSignalCallbacks(in godot_string_name signal, NativeVariantPtrArgs args)
	{
		if (signal == SignalName.Confirmed && args.Count == 0)
		{
			backing_Confirmed?.Invoke();
		}
		else if (signal == SignalName.Deny && args.Count == 0)
		{
			backing_Deny?.Invoke();
		}
		else
		{
			base.RaiseGodotClassSignalCallbacks(in signal, args);
		}
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override bool HasGodotClassSignal(in godot_string_name signal)
	{
		if (signal == SignalName.Confirmed)
		{
			return true;
		}
		if (signal == SignalName.Deny)
		{
			return true;
		}
		return base.HasGodotClassSignal(in signal);
	}
}
