using System.Collections.Generic;
using System.ComponentModel;
using Godot;
using Godot.Bridge;
using Godot.NativeInterop;

[ScriptPath("res://Scripts/Minigames/SnakeGame/SG_GameHandler.cs")]
public partial class SG_GameHandler : MinigameBase
{

    private bool _mobileControlsCreated;

    private void EnsureMobileControls()
    {
        if (_mobileControlsCreated)
        {
            return;
        }
        _mobileControlsCreated = true;
        AddDpadButton("UP", Key.Up, "ui_up", new Vector2(110, -168));
        AddDpadButton("LEFT", Key.Left, "ui_left", new Vector2(24, -102));
        AddDpadButton("DOWN", Key.Down, "ui_down", new Vector2(110, -102));
        AddDpadButton("RIGHT", Key.Right, "ui_right", new Vector2(196, -102));
    }

    private void AddDpadButton(string label, Key key, string action, Vector2 pos)
    {
        Button b = MobileUI.MakeGameButton(label, Control.LayoutPreset.BottomLeft, pos, new Vector2(72, 60));
        b.ButtonDown += () =>
        {
            Input.ActionPress(action);
            Input.ParseInputEvent(new InputEventKey { Keycode = key, Pressed = true });
        };
        b.ButtonUp += () =>
        {
            Input.ActionRelease(action);
            Input.ParseInputEvent(new InputEventKey { Keycode = key, Pressed = false });
        };
        AddChild(b);
    }

    public override void _Ready()
    {
        base._Ready();
        if (Main._isMobile)
        {
            EnsureMobileControls();
        }
    }

}
