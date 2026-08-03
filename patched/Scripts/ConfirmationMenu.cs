using System;
using System.Collections.Generic;
using System.ComponentModel;
using Godot;
using Godot.Bridge;
using Godot.Collections;
using Godot.NativeInterop;

[ScriptPath("res://Scripts/SubMenus/ConfirmationMenu/ConfirmationMenu.cs")]
public partial class ConfirmationMenu : Window
{
    [Signal]
    public delegate void ConfirmedEventHandler();

    [Signal]
    public delegate void DenyEventHandler();

    [Export]
    public RichTextLabel label;

    public bool UnpauseOnClose = true;

    public override void _Ready()
    {
        // On mobile, the built-in scene buttons (Confirm/Deny) work via
        // emulate_mouse_from_touch. Don't add extra buttons that interfere
        // with dialogs that have text input (like name entry).
        ProcessMode = ProcessModeEnum.Always;
    }

    public override void _Process(double delta)
    {
        if (Input.IsActionJustPressed("PauseGame"))
        {
            // On mobile, MENU button can close this dialog
            if (Main._isMobile)
                ConfirmClose();
            else
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
}
