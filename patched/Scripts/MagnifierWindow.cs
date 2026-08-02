using System.Collections.Generic;
using System.ComponentModel;
using Godot;
using Godot.Bridge;
using Godot.NativeInterop;

[ScriptPath("res://Scripts/SubMenus/MagnifierMenu/MagnifierWindow.cs")]
public partial class MagnifierWindow : Window
{

    private ShaderMaterial _material;

    private Sprite2D _lens;

    private ViewportTexture _rootTexture;

    private Window _currentParent;

    private Window _cachedRoot;

    private float _magnification = 2f;

    private const float MagMin = 1.5f;

    private const float MagMax = 8f;

    private Button _closeButton;
    private Button _zoomInButton;
    private Button _zoomOutButton;
    private Vector2 _mobilePos = new Vector2(-1f, -1f);
    private bool _mobileControlsCreated;

    public override void _Input(InputEvent @event)
    {
        if (!Main._isMobile)
        {
            return;
        }
        if (@event is InputEventScreenTouch touch)
        {
            if (touch.Pressed)
            {
                _mobilePos = touch.Position;
            }
        }
        else if (@event is InputEventScreenDrag drag)
        {
            _mobilePos = drag.Position;
        }
        base._Input(@event);
    }

    private void EnsureMobileControls()
    {
        if (_mobileControlsCreated)
        {
            return;
        }
        _mobileControlsCreated = true;
        _closeButton = MobileUI.MakeGameButton("CLOSE", Control.LayoutPreset.TopRight, new Vector2(-116, 10), new Vector2(104, 48));
        AddChild(_closeButton);
        _closeButton.Pressed += () => Main.Instance.CloseMagnifier();
        _zoomInButton = MobileUI.MakeGameButton("+", Control.LayoutPreset.BottomRight, new Vector2(-120, -70), new Vector2(54, 54));
        AddChild(_zoomInButton);
        _zoomInButton.Pressed += () => AdjustMagnification(0.25f);
        _zoomOutButton = MobileUI.MakeGameButton("-", Control.LayoutPreset.BottomRight, new Vector2(-60, -70), new Vector2(54, 54));
        AddChild(_zoomOutButton);
        _zoomOutButton.Pressed += () => AdjustMagnification(-0.25f);
    }

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
        if (Main._isMobile)
        {
            EnsureMobileControls();
            if (_mobilePos.X < 0f)
            {
                _mobilePos = (Vector2)(DisplayServer.ScreenGetSize() / 2);
            }
            vector = _mobilePos;
            Vector2I sz = DisplayServer.ScreenGetSize();
            base.Position = new Vector2I(
                Mathf.Clamp(Mathf.RoundToInt(vector.X - base.Size.X / 2f), 0, Mathf.Max(0, sz.X - base.Size.X)),
                Mathf.Clamp(Mathf.RoundToInt(vector.Y - base.Size.Y / 2f), 0, Mathf.Max(0, sz.Y - base.Size.Y)));
        }
        else
        {
            base.Position = (Vector2I)(vector - base.Size / 2);
        }
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

}
