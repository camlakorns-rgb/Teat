using System.Collections.Generic;
using System.ComponentModel;
using Godot;
using Godot.Bridge;
using Godot.Collections;
using Godot.NativeInterop;

[ScriptPath("res://Scripts/SubMenus/GalleryMenu/GalleryDisplay.cs")]
public class GalleryDisplay : Window
{
	public new class MethodName : Window.MethodName
	{
		public static readonly StringName OpenGalleryPiece = "OpenGalleryPiece";

		public static readonly StringName CloseGalleryPiece = "CloseGalleryPiece";
	}

	public new class PropertyName : Window.PropertyName
	{
		public static readonly StringName handler = "handler";

		public static readonly StringName parent = "parent";

		public static readonly StringName _backgroundWindows = "_backgroundWindows";
	}

	public new class SignalName : Window.SignalName
	{
	}

	[Export(PropertyHint.None, "")]
	private PieceDisplayHandler handler;

	public GalleryHandler parent;

	private Array<Window> _backgroundWindows = new Array<Window>();

	public void OpenGalleryPiece(GalleryPieceDataRes pieceData)
	{
		GrabFocus();
		int currentScreen = Main.Instance.mainWindow.CurrentScreen;
		Vector2I vector2I = DisplayServer.ScreenGetSize(currentScreen);
		Vector2I vector2I2 = DisplayServer.ScreenGetPosition(currentScreen);
		base.CurrentScreen = currentScreen;
		CallDeferred("set_position", vector2I2);
		CallDeferred("set_size", vector2I);
		_backgroundWindows.Clear();
		foreach (Node item in GetTree().GetNodesInGroup("WindowTag"))
		{
			if (item is Window window && window != this)
			{
				window.MousePassthrough = true;
				window.ProcessMode = ProcessModeEnum.Disabled;
				_backgroundWindows.Add(window);
			}
		}
		handler.OpenGalleryPiece(pieceData, Main.Instance.IsBlacklisted(pieceData.taggedKinks));
	}

	public void CloseGalleryPiece()
	{
		foreach (Window backgroundWindow in _backgroundWindows)
		{
			if (GodotObject.IsInstanceValid(backgroundWindow))
			{
				if (backgroundWindow.IsInGroup("AlwaysActive"))
				{
					backgroundWindow.ProcessMode = ProcessModeEnum.Always;
				}
				else
				{
					backgroundWindow.ProcessMode = ProcessModeEnum.Inherit;
				}
				backgroundWindow.MousePassthrough = false;
			}
		}
		_backgroundWindows.Clear();
		parent.spawnedGallery = null;
		Main.Instance.mainWindow.GrabFocus();
		QueueFree();
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	internal static List<MethodInfo> GetGodotMethodList()
	{
		return new List<MethodInfo>(2)
		{
			new MethodInfo(MethodName.OpenGalleryPiece, new PropertyInfo(Variant.Type.Nil, "", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false), MethodFlags.Normal, new List<PropertyInfo>
			{
				new PropertyInfo(Variant.Type.Object, "pieceData", PropertyHint.None, "", PropertyUsageFlags.Default, new StringName("Resource"), exported: false)
			}, null),
			new MethodInfo(MethodName.CloseGalleryPiece, new PropertyInfo(Variant.Type.Nil, "", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false), MethodFlags.Normal, null, null)
		};
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override bool InvokeGodotClassMethod(in godot_string_name method, NativeVariantPtrArgs args, out godot_variant ret)
	{
		if (method == MethodName.OpenGalleryPiece && args.Count == 1)
		{
			OpenGalleryPiece(VariantUtils.ConvertTo<GalleryPieceDataRes>(in args[0]));
			ret = default(godot_variant);
			return true;
		}
		if (method == MethodName.CloseGalleryPiece && args.Count == 0)
		{
			CloseGalleryPiece();
			ret = default(godot_variant);
			return true;
		}
		return base.InvokeGodotClassMethod(in method, args, out ret);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override bool HasGodotClassMethod(in godot_string_name method)
	{
		if (method == MethodName.OpenGalleryPiece)
		{
			return true;
		}
		if (method == MethodName.CloseGalleryPiece)
		{
			return true;
		}
		return base.HasGodotClassMethod(in method);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override bool SetGodotClassPropertyValue(in godot_string_name name, in godot_variant value)
	{
		if (name == PropertyName.handler)
		{
			handler = VariantUtils.ConvertTo<PieceDisplayHandler>(in value);
			return true;
		}
		if (name == PropertyName.parent)
		{
			parent = VariantUtils.ConvertTo<GalleryHandler>(in value);
			return true;
		}
		if (name == PropertyName._backgroundWindows)
		{
			_backgroundWindows = VariantUtils.ConvertToArray<Window>(in value);
			return true;
		}
		return base.SetGodotClassPropertyValue(in name, in value);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override bool GetGodotClassPropertyValue(in godot_string_name name, out godot_variant value)
	{
		if (name == PropertyName.handler)
		{
			value = VariantUtils.CreateFrom(in handler);
			return true;
		}
		if (name == PropertyName.parent)
		{
			value = VariantUtils.CreateFrom(in parent);
			return true;
		}
		if (name == PropertyName._backgroundWindows)
		{
			value = VariantUtils.CreateFromArray(_backgroundWindows);
			return true;
		}
		return base.GetGodotClassPropertyValue(in name, out value);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	internal static List<PropertyInfo> GetGodotPropertyList()
	{
		return new List<PropertyInfo>
		{
			new PropertyInfo(Variant.Type.Object, PropertyName.handler, PropertyHint.NodeType, "Control", PropertyUsageFlags.Default | PropertyUsageFlags.ScriptVariable, exported: true),
			new PropertyInfo(Variant.Type.Object, PropertyName.parent, PropertyHint.None, "", PropertyUsageFlags.ScriptVariable, exported: false),
			new PropertyInfo(Variant.Type.Array, PropertyName._backgroundWindows, PropertyHint.None, "", PropertyUsageFlags.ScriptVariable, exported: false)
		};
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override void SaveGodotObjectData(GodotSerializationInfo info)
	{
		base.SaveGodotObjectData(info);
		info.AddProperty(PropertyName.handler, Variant.From(in handler));
		info.AddProperty(PropertyName.parent, Variant.From(in parent));
		info.AddProperty(PropertyName._backgroundWindows, Variant.CreateFrom(_backgroundWindows));
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override void RestoreGodotObjectData(GodotSerializationInfo info)
	{
		base.RestoreGodotObjectData(info);
		if (info.TryGetProperty(PropertyName.handler, out var value))
		{
			handler = value.As<PieceDisplayHandler>();
		}
		if (info.TryGetProperty(PropertyName.parent, out var value2))
		{
			parent = value2.As<GalleryHandler>();
		}
		if (info.TryGetProperty(PropertyName._backgroundWindows, out var value3))
		{
			_backgroundWindows = value3.AsGodotArray<Window>();
		}
	}
}
