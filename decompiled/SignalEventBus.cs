using System;
using System.Collections.Generic;
using System.ComponentModel;
using Godot;
using Godot.Bridge;
using Godot.NativeInterop;

[GlobalClass]
[Tool]
[ScriptPath("res://Scripts/Globals/SignalEventBus.cs")]
public class SignalEventBus : Node
{
	[Signal]
	public delegate void ItemUsedOnMainActorEventHandler(ItemDataRes item, ItemDataRes.ItemTask itemTask);

	[Signal]
	public delegate void ItemUsedOnSubActorEventHandler(ItemDataRes item, ItemDataRes.ItemTask itemTask);

	[Signal]
	public delegate void AttachmentStartEventHandler(AttachDataRes.AttachmentType attachmentType);

	[Signal]
	public delegate void AttachmentEndEventHandler(AttachDataRes.AttachmentType attachmentType);

	public new class MethodName : Node.MethodName
	{
		public new static readonly StringName _EnterTree = "_EnterTree";

		public new static readonly StringName _ExitTree = "_ExitTree";
	}

	public new class PropertyName : Node.PropertyName
	{
	}

	public new class SignalName : Node.SignalName
	{
		public static readonly StringName ItemUsedOnMainActor = "ItemUsedOnMainActor";

		public static readonly StringName ItemUsedOnSubActor = "ItemUsedOnSubActor";

		public static readonly StringName AttachmentStart = "AttachmentStart";

		public static readonly StringName AttachmentEnd = "AttachmentEnd";
	}

	private ItemUsedOnMainActorEventHandler backing_ItemUsedOnMainActor;

	private ItemUsedOnSubActorEventHandler backing_ItemUsedOnSubActor;

	private AttachmentStartEventHandler backing_AttachmentStart;

	private AttachmentEndEventHandler backing_AttachmentEnd;

	public static SignalEventBus Instance { get; private set; }

	public event ItemUsedOnMainActorEventHandler ItemUsedOnMainActor
	{
		add
		{
			backing_ItemUsedOnMainActor = (ItemUsedOnMainActorEventHandler)Delegate.Combine(backing_ItemUsedOnMainActor, value);
		}
		remove
		{
			backing_ItemUsedOnMainActor = (ItemUsedOnMainActorEventHandler)Delegate.Remove(backing_ItemUsedOnMainActor, value);
		}
	}

	public event ItemUsedOnSubActorEventHandler ItemUsedOnSubActor
	{
		add
		{
			backing_ItemUsedOnSubActor = (ItemUsedOnSubActorEventHandler)Delegate.Combine(backing_ItemUsedOnSubActor, value);
		}
		remove
		{
			backing_ItemUsedOnSubActor = (ItemUsedOnSubActorEventHandler)Delegate.Remove(backing_ItemUsedOnSubActor, value);
		}
	}

	public event AttachmentStartEventHandler AttachmentStart
	{
		add
		{
			backing_AttachmentStart = (AttachmentStartEventHandler)Delegate.Combine(backing_AttachmentStart, value);
		}
		remove
		{
			backing_AttachmentStart = (AttachmentStartEventHandler)Delegate.Remove(backing_AttachmentStart, value);
		}
	}

	public event AttachmentEndEventHandler AttachmentEnd
	{
		add
		{
			backing_AttachmentEnd = (AttachmentEndEventHandler)Delegate.Combine(backing_AttachmentEnd, value);
		}
		remove
		{
			backing_AttachmentEnd = (AttachmentEndEventHandler)Delegate.Remove(backing_AttachmentEnd, value);
		}
	}

	public override void _EnterTree()
	{
		Instance = this;
	}

	public override void _ExitTree()
	{
		if (Instance == this)
		{
			Instance = null;
		}
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	internal static List<MethodInfo> GetGodotMethodList()
	{
		return new List<MethodInfo>(2)
		{
			new MethodInfo(MethodName._EnterTree, new PropertyInfo(Variant.Type.Nil, "", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false), MethodFlags.Normal, null, null),
			new MethodInfo(MethodName._ExitTree, new PropertyInfo(Variant.Type.Nil, "", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false), MethodFlags.Normal, null, null)
		};
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override bool InvokeGodotClassMethod(in godot_string_name method, NativeVariantPtrArgs args, out godot_variant ret)
	{
		if (method == MethodName._EnterTree && args.Count == 0)
		{
			_EnterTree();
			ret = default(godot_variant);
			return true;
		}
		if (method == MethodName._ExitTree && args.Count == 0)
		{
			_ExitTree();
			ret = default(godot_variant);
			return true;
		}
		return base.InvokeGodotClassMethod(in method, args, out ret);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override bool HasGodotClassMethod(in godot_string_name method)
	{
		if (method == MethodName._EnterTree)
		{
			return true;
		}
		if (method == MethodName._ExitTree)
		{
			return true;
		}
		return base.HasGodotClassMethod(in method);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override void SaveGodotObjectData(GodotSerializationInfo info)
	{
		base.SaveGodotObjectData(info);
		info.AddSignalEventDelegate(SignalName.ItemUsedOnMainActor, backing_ItemUsedOnMainActor);
		info.AddSignalEventDelegate(SignalName.ItemUsedOnSubActor, backing_ItemUsedOnSubActor);
		info.AddSignalEventDelegate(SignalName.AttachmentStart, backing_AttachmentStart);
		info.AddSignalEventDelegate(SignalName.AttachmentEnd, backing_AttachmentEnd);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override void RestoreGodotObjectData(GodotSerializationInfo info)
	{
		base.RestoreGodotObjectData(info);
		if (info.TryGetSignalEventDelegate<ItemUsedOnMainActorEventHandler>(SignalName.ItemUsedOnMainActor, out var value))
		{
			backing_ItemUsedOnMainActor = value;
		}
		if (info.TryGetSignalEventDelegate<ItemUsedOnSubActorEventHandler>(SignalName.ItemUsedOnSubActor, out var value2))
		{
			backing_ItemUsedOnSubActor = value2;
		}
		if (info.TryGetSignalEventDelegate<AttachmentStartEventHandler>(SignalName.AttachmentStart, out var value3))
		{
			backing_AttachmentStart = value3;
		}
		if (info.TryGetSignalEventDelegate<AttachmentEndEventHandler>(SignalName.AttachmentEnd, out var value4))
		{
			backing_AttachmentEnd = value4;
		}
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	internal static List<MethodInfo> GetGodotSignalList()
	{
		return new List<MethodInfo>(4)
		{
			new MethodInfo(SignalName.ItemUsedOnMainActor, new PropertyInfo(Variant.Type.Nil, "", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false), MethodFlags.Normal, new List<PropertyInfo>
			{
				new PropertyInfo(Variant.Type.Object, "item", PropertyHint.None, "", PropertyUsageFlags.Default, new StringName("Resource"), exported: false),
				new PropertyInfo(Variant.Type.Int, "itemTask", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false)
			}, null),
			new MethodInfo(SignalName.ItemUsedOnSubActor, new PropertyInfo(Variant.Type.Nil, "", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false), MethodFlags.Normal, new List<PropertyInfo>
			{
				new PropertyInfo(Variant.Type.Object, "item", PropertyHint.None, "", PropertyUsageFlags.Default, new StringName("Resource"), exported: false),
				new PropertyInfo(Variant.Type.Int, "itemTask", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false)
			}, null),
			new MethodInfo(SignalName.AttachmentStart, new PropertyInfo(Variant.Type.Nil, "", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false), MethodFlags.Normal, new List<PropertyInfo>
			{
				new PropertyInfo(Variant.Type.Int, "attachmentType", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false)
			}, null),
			new MethodInfo(SignalName.AttachmentEnd, new PropertyInfo(Variant.Type.Nil, "", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false), MethodFlags.Normal, new List<PropertyInfo>
			{
				new PropertyInfo(Variant.Type.Int, "attachmentType", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false)
			}, null)
		};
	}

	protected void EmitSignalItemUsedOnMainActor(ItemDataRes item, ItemDataRes.ItemTask itemTask)
	{
		EmitSignal(SignalName.ItemUsedOnMainActor, item, (int)itemTask);
	}

	protected void EmitSignalItemUsedOnSubActor(ItemDataRes item, ItemDataRes.ItemTask itemTask)
	{
		EmitSignal(SignalName.ItemUsedOnSubActor, item, (int)itemTask);
	}

	protected void EmitSignalAttachmentStart(AttachDataRes.AttachmentType attachmentType)
	{
		EmitSignal(SignalName.AttachmentStart, (int)attachmentType);
	}

	protected void EmitSignalAttachmentEnd(AttachDataRes.AttachmentType attachmentType)
	{
		EmitSignal(SignalName.AttachmentEnd, (int)attachmentType);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override void RaiseGodotClassSignalCallbacks(in godot_string_name signal, NativeVariantPtrArgs args)
	{
		if (signal == SignalName.ItemUsedOnMainActor && args.Count == 2)
		{
			backing_ItemUsedOnMainActor?.Invoke(VariantUtils.ConvertTo<ItemDataRes>(in args[0]), VariantUtils.ConvertTo<ItemDataRes.ItemTask>(in args[1]));
		}
		else if (signal == SignalName.ItemUsedOnSubActor && args.Count == 2)
		{
			backing_ItemUsedOnSubActor?.Invoke(VariantUtils.ConvertTo<ItemDataRes>(in args[0]), VariantUtils.ConvertTo<ItemDataRes.ItemTask>(in args[1]));
		}
		else if (signal == SignalName.AttachmentStart && args.Count == 1)
		{
			backing_AttachmentStart?.Invoke(VariantUtils.ConvertTo<AttachDataRes.AttachmentType>(in args[0]));
		}
		else if (signal == SignalName.AttachmentEnd && args.Count == 1)
		{
			backing_AttachmentEnd?.Invoke(VariantUtils.ConvertTo<AttachDataRes.AttachmentType>(in args[0]));
		}
		else
		{
			base.RaiseGodotClassSignalCallbacks(in signal, args);
		}
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override bool HasGodotClassSignal(in godot_string_name signal)
	{
		if (signal == SignalName.ItemUsedOnMainActor)
		{
			return true;
		}
		if (signal == SignalName.ItemUsedOnSubActor)
		{
			return true;
		}
		if (signal == SignalName.AttachmentStart)
		{
			return true;
		}
		if (signal == SignalName.AttachmentEnd)
		{
			return true;
		}
		return base.HasGodotClassSignal(in signal);
	}
}
