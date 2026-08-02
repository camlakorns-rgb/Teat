using System.Collections.Generic;
using System.ComponentModel;
using Godot;
using Godot.Bridge;
using Godot.NativeInterop;

[ScriptPath("res://Scripts/SubMenus/TerminalMenu/TerminalWindow.cs")]
public partial class TerminalWindow : Window
{

    private Button _closeButton;

    public override void _Process(double delta)
    {
        if (Input.IsActionJustPressed("PauseGame"))
        {
            OnClose();
        }
        if (Main._isMobile && _closeButton == null)
        {
            _closeButton = MobileUI.MakeGameButton("CLOSE", Control.LayoutPreset.TopRight, new Vector2(-116, 10), new Vector2(104, 48));
            AddChild(_closeButton);
            _closeButton.Pressed += OnClose;
        }
    }

    public void OnClose()
    {
        Main.Instance.Terminal = null;
        Main.Instance.mainWindow.GrabFocus();
        QueueFree();
    }

}
