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
        // Add a mobile-friendly close button (top-right corner)
        if (Main._isMobile)
        {
            Button closeBtn = new Button();
            closeBtn.Text = "X";
            closeBtn.CustomMinimumSize = new Vector2(60, 60);
            closeBtn.Position = new Vector2(Size.X - 70, 10);
            closeBtn.AddThemeFontSizeOverride("font_size", 24);
            StyleBoxFlat sb = new StyleBoxFlat();
            sb.BgColor = new Color(0.8f, 0.2f, 0.2f, 0.9f);
            sb.SetCornerRadiusAll(8);
            closeBtn.AddThemeStyleboxOverride("normal", sb);
            closeBtn.Pressed += () => OnClose();
            AddChild(closeBtn);

            // Also make the whole dialog tappable to dismiss (like desktop click-to-close)
            ProcessMode = ProcessModeEnum.Always;
        }
    }

    public override void _Process(double delta)
    {
        if (Input.IsActionJustPressed("PauseGame"))
        {
            OnClose();
        }
    }

    public override void _Input(InputEvent @event)
    {
        // On mobile, tapping anywhere on the confirmation dismisses it
        if (Main._isMobile && @event is InputEventScreenTouch touch && touch.Pressed)
        {
            // Check if the touch is inside this window
            Rect2I windowRect = new Rect2I(Position, (Vector2I)Size);
            if (windowRect.HasPoint((Vector2I)touch.Position))
            {
                // If there's a Confirm button, clicking in the lower half confirms
                // If in the upper portion or no button, just dismiss
                if (touch.Position.Y > Position.Y + Size.Y / 2)
                    ConfirmClose();
                else
                    OnClose();
            }
        }
        base._Input(@event);
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
