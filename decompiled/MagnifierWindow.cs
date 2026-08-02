using System.Collections.Generic;
using System.ComponentModel;
using Godot;
using Godot.Bridge;
using Godot.NativeInterop;

[ScriptPath("res://Scripts/SubMenus/MagnifierMenu/MagnifierWindow.cs")]
public class MagnifierWindow : Window
{
	public new class MethodName : Window.MethodName
	{
		public new static readonly StringName _Ready = "_Ready";

		public static readonly StringName ReparentToRoot = "ReparentToRoot";

		public new static readonly StringName _Process = "_Process";

		public static readonly StringName AdjustMagnification = "AdjustMagnification";

		public static readonly StringName SetParentWindow = "SetParentWindow";

		public static readonly StringName OnParentExiting = "OnParentExiting";

		public static readonly StringName GetWindowUnderCursor = "GetWindowUnderCursor";

		public static readonly StringName FindWindowUnderCursor = "FindWindowUnderCursor";
	}

	public new class PropertyName : Window.PropertyName
	{
		public static readonly StringName _material = "_material";

		public static readonly StringName _lens = "_lens";

		public static readonly StringName _rootTexture = "_rootTexture";

		public static readonly StringName _currentParent = "_currentParent";

		public static readonly StringName _cachedRoot = "_cachedRoot";

		public static readonly StringName _magnification = "_magnification";
	}

	public new class SignalName : Window.SignalName
	{
	}

	private ShaderMaterial _material;

	private Sprite2D _lens;

	private ViewportTexture _rootTexture;

	private Window _currentParent;

	private Window _cachedRoot;

	private float _magnification = 2f;

	private const float MagMin = 1.5f;

	private const float MagMax = 8f;

	public override void _Ready()
	{
		_lens = GetNode<Sprite2D>("LensSprite");
		_material = _lens.Material as ShaderMaterial;
		_lens.Centered = false;
		_lens.Position = Vector2.Zero;
		_lens.Scale = base.Size / _lens.Texture.GetSize();
		_lens.SelfModulate = new Color(0f, 0f, 0f, 0f);
		_lens.Visible = false;
		base.Visible = false;
		CallDeferred(MethodName.ReparentToRoot);
	}

	private void ReparentToRoot()
	{
		Window window = (_cachedRoot = GetTree().Root);
		Node parent = GetParent();
		if (parent != null && GodotObject.IsInstanceValid(parent))
		{
			parent.RemoveChild(this);
		}
		window.AddChild(this, forceReadableName: false, InternalMode.Disabled);
		_currentParent = window;
		_rootTexture = window.GetViewport().GetTexture();
		_material.SetShaderParameter("window_texture", _rootTexture);
		_material.SetShaderParameter("window_uv_offset", Vector2.Zero);
		_material.SetShaderParameter("window_uv_scale", Vector2.One);
		base.Visible = true;
		_lens.Visible = false;
	}

	public override void _Process(double delta)
	{
		if (_currentParent == null || !GodotObject.IsInstanceValid(_currentParent))
		{
			CallDeferred(MethodName.ReparentToRoot);
			_lens.Visible = false;
			return;
		}
		Vector2 vector = DisplayServer.MouseGetPosition();
		base.Position = (Vector2I)(vector - base.Size / 2);
		Window windowUnderCursor = GetWindowUnderCursor(vector);
		ViewportTexture viewportTexture;
		Vector2 vector2;
		Vector2 vector3;
		if (windowUnderCursor != null)
		{
			if (!GodotObject.IsInstanceValid(windowUnderCursor))
			{
				_lens.Visible = false;
				return;
			}
			Viewport viewport = windowUnderCursor.GetViewport();
			if (viewport == null)
			{
				_lens.Visible = false;
				return;
			}
			viewportTexture = viewport.GetTexture();
			vector2 = windowUnderCursor.Position;
			vector3 = windowUnderCursor.Size;
			SetParentWindow(windowUnderCursor);
		}
		else
		{
			if (!new Rect2I(Main.Instance.mainWindow.Position, Main.Instance.mainWindow.Size).HasPoint(DisplayServer.MouseGetPosition()))
			{
				_lens.Visible = false;
				return;
			}
			viewportTexture = _rootTexture;
			vector2 = GetTree().Root.Position;
			vector3 = GetTree().Root.Size;
			SetParentWindow(GetTree().Root);
		}
		_lens.Visible = true;
		Vector2 vector4 = (base.Position - vector2) / vector3;
		Vector2 vector5 = base.Size / vector3;
		_material.SetShaderParameter("window_texture", viewportTexture);
		_material.SetShaderParameter("window_uv_offset", vector4);
		_material.SetShaderParameter("window_uv_scale", vector5);
		GrabFocus();
	}

	public void AdjustMagnification(float delta)
	{
		_magnification = Mathf.Clamp(_magnification + delta, 1.5f, 8f);
		_material.SetShaderParameter("magnification", _magnification);
	}

	private void SetParentWindow(Window newParent)
	{
		if (_currentParent != newParent && GodotObject.IsInstanceValid(newParent))
		{
			if (_currentParent != null && GodotObject.IsInstanceValid(_currentParent) && _currentParent != GetTree().Root && _currentParent.IsConnected(Node.SignalName.TreeExiting, Callable.From(OnParentExiting)))
			{
				_currentParent.Disconnect(Node.SignalName.TreeExiting, Callable.From(OnParentExiting));
			}
			Node parent = GetParent();
			if (parent != null && GodotObject.IsInstanceValid(parent))
			{
				parent.RemoveChild(this);
			}
			newParent.AddChild(this, forceReadableName: false, InternalMode.Disabled);
			_currentParent = newParent;
			if (newParent != GetTree().Root)
			{
				newParent.Connect(Node.SignalName.TreeExiting, Callable.From(OnParentExiting));
			}
		}
	}

	private void OnParentExiting()
	{
		Node parent = GetParent();
		if (parent != null && GodotObject.IsInstanceValid(parent))
		{
			parent.RemoveChild(this);
		}
		_cachedRoot.CallDeferred(Node.MethodName.AddChild, this);
		_currentParent = _cachedRoot;
		_lens.Visible = false;
	}

	private Window GetWindowUnderCursor(Vector2 mousePos)
	{
		return FindWindowUnderCursor(GetTree().Root, mousePos);
	}

	private Window FindWindowUnderCursor(Node node, Vector2 mousePos)
	{
		if (!GodotObject.IsInstanceValid(node))
		{
			return null;
		}
		Window window = null;
		foreach (Node child in node.GetChildren())
		{
			if (GodotObject.IsInstanceValid(child))
			{
				Window window2 = FindWindowUnderCursor(child, mousePos);
				if (window2 != null)
				{
					window = window2;
				}
			}
		}
		if (window != null)
		{
			return window;
		}
		if (node is Window window3 && window3 != this && window3 != GetTree().Root && window3.Visible && new Rect2(window3.Position, window3.Size).HasPoint(mousePos))
		{
			return window3;
		}
		return null;
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	internal static List<MethodInfo> GetGodotMethodList()
	{
		return new List<MethodInfo>(8)
		{
			new MethodInfo(MethodName._Ready, new PropertyInfo(Variant.Type.Nil, "", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false), MethodFlags.Normal, null, null),
			new MethodInfo(MethodName.ReparentToRoot, new PropertyInfo(Variant.Type.Nil, "", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false), MethodFlags.Normal, null, null),
			new MethodInfo(MethodName._Process, new PropertyInfo(Variant.Type.Nil, "", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false), MethodFlags.Normal, new List<PropertyInfo>
			{
				new PropertyInfo(Variant.Type.Float, "delta", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false)
			}, null),
			new MethodInfo(MethodName.AdjustMagnification, new PropertyInfo(Variant.Type.Nil, "", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false), MethodFlags.Normal, new List<PropertyInfo>
			{
				new PropertyInfo(Variant.Type.Float, "delta", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false)
			}, null),
			new MethodInfo(MethodName.SetParentWindow, new PropertyInfo(Variant.Type.Nil, "", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false), MethodFlags.Normal, new List<PropertyInfo>
			{
				new PropertyInfo(Variant.Type.Object, "newParent", PropertyHint.None, "", PropertyUsageFlags.Default, new StringName("Window"), exported: false)
			}, null),
			new MethodInfo(MethodName.OnParentExiting, new PropertyInfo(Variant.Type.Nil, "", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false), MethodFlags.Normal, null, null),
			new MethodInfo(MethodName.GetWindowUnderCursor, new PropertyInfo(Variant.Type.Object, "", PropertyHint.None, "", PropertyUsageFlags.Default, new StringName("Window"), exported: false), MethodFlags.Normal, new List<PropertyInfo>
			{
				new PropertyInfo(Variant.Type.Vector2, "mousePos", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false)
			}, null),
			new MethodInfo(MethodName.FindWindowUnderCursor, new PropertyInfo(Variant.Type.Object, "", PropertyHint.None, "", PropertyUsageFlags.Default, new StringName("Window"), exported: false), MethodFlags.Normal, new List<PropertyInfo>
			{
				new PropertyInfo(Variant.Type.Object, "node", PropertyHint.None, "", PropertyUsageFlags.Default, new StringName("Node"), exported: false),
				new PropertyInfo(Variant.Type.Vector2, "mousePos", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false)
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
		if (method == MethodName.ReparentToRoot && args.Count == 0)
		{
			ReparentToRoot();
			ret = default(godot_variant);
			return true;
		}
		if (method == MethodName._Process && args.Count == 1)
		{
			_Process(VariantUtils.ConvertTo<double>(in args[0]));
			ret = default(godot_variant);
			return true;
		}
		if (method == MethodName.AdjustMagnification && args.Count == 1)
		{
			AdjustMagnification(VariantUtils.ConvertTo<float>(in args[0]));
			ret = default(godot_variant);
			return true;
		}
		if (method == MethodName.SetParentWindow && args.Count == 1)
		{
			SetParentWindow(VariantUtils.ConvertTo<Window>(in args[0]));
			ret = default(godot_variant);
			return true;
		}
		if (method == MethodName.OnParentExiting && args.Count == 0)
		{
			OnParentExiting();
			ret = default(godot_variant);
			return true;
		}
		if (method == MethodName.GetWindowUnderCursor && args.Count == 1)
		{
			Window from = GetWindowUnderCursor(VariantUtils.ConvertTo<Vector2>(in args[0]));
			ret = VariantUtils.CreateFrom(in from);
			return true;
		}
		if (method == MethodName.FindWindowUnderCursor && args.Count == 2)
		{
			Window from2 = FindWindowUnderCursor(VariantUtils.ConvertTo<Node>(in args[0]), VariantUtils.ConvertTo<Vector2>(in args[1]));
			ret = VariantUtils.CreateFrom(in from2);
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
		if (method == MethodName.ReparentToRoot)
		{
			return true;
		}
		if (method == MethodName._Process)
		{
			return true;
		}
		if (method == MethodName.AdjustMagnification)
		{
			return true;
		}
		if (method == MethodName.SetParentWindow)
		{
			return true;
		}
		if (method == MethodName.OnParentExiting)
		{
			return true;
		}
		if (method == MethodName.GetWindowUnderCursor)
		{
			return true;
		}
		if (method == MethodName.FindWindowUnderCursor)
		{
			return true;
		}
		return base.HasGodotClassMethod(in method);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override bool SetGodotClassPropertyValue(in godot_string_name name, in godot_variant value)
	{
		if (name == PropertyName._material)
		{
			_material = VariantUtils.ConvertTo<ShaderMaterial>(in value);
			return true;
		}
		if (name == PropertyName._lens)
		{
			_lens = VariantUtils.ConvertTo<Sprite2D>(in value);
			return true;
		}
		if (name == PropertyName._rootTexture)
		{
			_rootTexture = VariantUtils.ConvertTo<ViewportTexture>(in value);
			return true;
		}
		if (name == PropertyName._currentParent)
		{
			_currentParent = VariantUtils.ConvertTo<Window>(in value);
			return true;
		}
		if (name == PropertyName._cachedRoot)
		{
			_cachedRoot = VariantUtils.ConvertTo<Window>(in value);
			return true;
		}
		if (name == PropertyName._magnification)
		{
			_magnification = VariantUtils.ConvertTo<float>(in value);
			return true;
		}
		return base.SetGodotClassPropertyValue(in name, in value);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override bool GetGodotClassPropertyValue(in godot_string_name name, out godot_variant value)
	{
		if (name == PropertyName._material)
		{
			value = VariantUtils.CreateFrom(in _material);
			return true;
		}
		if (name == PropertyName._lens)
		{
			value = VariantUtils.CreateFrom(in _lens);
			return true;
		}
		if (name == PropertyName._rootTexture)
		{
			value = VariantUtils.CreateFrom(in _rootTexture);
			return true;
		}
		if (name == PropertyName._currentParent)
		{
			value = VariantUtils.CreateFrom(in _currentParent);
			return true;
		}
		if (name == PropertyName._cachedRoot)
		{
			value = VariantUtils.CreateFrom(in _cachedRoot);
			return true;
		}
		if (name == PropertyName._magnification)
		{
			value = VariantUtils.CreateFrom(in _magnification);
			return true;
		}
		return base.GetGodotClassPropertyValue(in name, out value);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	internal static List<PropertyInfo> GetGodotPropertyList()
	{
		return new List<PropertyInfo>
		{
			new PropertyInfo(Variant.Type.Object, PropertyName._material, PropertyHint.None, "", PropertyUsageFlags.ScriptVariable, exported: false),
			new PropertyInfo(Variant.Type.Object, PropertyName._lens, PropertyHint.None, "", PropertyUsageFlags.ScriptVariable, exported: false),
			new PropertyInfo(Variant.Type.Object, PropertyName._rootTexture, PropertyHint.None, "", PropertyUsageFlags.ScriptVariable, exported: false),
			new PropertyInfo(Variant.Type.Object, PropertyName._currentParent, PropertyHint.None, "", PropertyUsageFlags.ScriptVariable, exported: false),
			new PropertyInfo(Variant.Type.Object, PropertyName._cachedRoot, PropertyHint.None, "", PropertyUsageFlags.ScriptVariable, exported: false),
			new PropertyInfo(Variant.Type.Float, PropertyName._magnification, PropertyHint.None, "", PropertyUsageFlags.ScriptVariable, exported: false)
		};
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override void SaveGodotObjectData(GodotSerializationInfo info)
	{
		base.SaveGodotObjectData(info);
		info.AddProperty(PropertyName._material, Variant.From(in _material));
		info.AddProperty(PropertyName._lens, Variant.From(in _lens));
		info.AddProperty(PropertyName._rootTexture, Variant.From(in _rootTexture));
		info.AddProperty(PropertyName._currentParent, Variant.From(in _currentParent));
		info.AddProperty(PropertyName._cachedRoot, Variant.From(in _cachedRoot));
		info.AddProperty(PropertyName._magnification, Variant.From(in _magnification));
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override void RestoreGodotObjectData(GodotSerializationInfo info)
	{
		base.RestoreGodotObjectData(info);
		if (info.TryGetProperty(PropertyName._material, out var value))
		{
			_material = value.As<ShaderMaterial>();
		}
		if (info.TryGetProperty(PropertyName._lens, out var value2))
		{
			_lens = value2.As<Sprite2D>();
		}
		if (info.TryGetProperty(PropertyName._rootTexture, out var value3))
		{
			_rootTexture = value3.As<ViewportTexture>();
		}
		if (info.TryGetProperty(PropertyName._currentParent, out var value4))
		{
			_currentParent = value4.As<Window>();
		}
		if (info.TryGetProperty(PropertyName._cachedRoot, out var value5))
		{
			_cachedRoot = value5.As<Window>();
		}
		if (info.TryGetProperty(PropertyName._magnification, out var value6))
		{
			_magnification = value6.As<float>();
		}
	}
}
