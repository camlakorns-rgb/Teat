using Godot;
using System;

// On-screen mobile control bar (top-right corner of the screen, NOT on Byte).
// Each button synthesizes the same input action the desktop keyboard binds, so
// every game feature (pause menu, terminal, sit, clothes, screen-lock, magnifier,
// despawn) becomes reachable by touch. Created only on mobile (Main._Ready).
//
// Buttons:
//   MENU   hold -> PauseGame
//   TERM   hold -> Terminal
//   SIT    tap  -> Sit
//   OUTFIT tap  -> Clothing_Up,  long-press -> Clothing_Down
//   LOCK   hold -> Screen_Lock   (faked touch on Byte so the hit-test passes)
//   AWAY   hold -> Despawn       (faked touch on Byte), long-press -> move all items to Byte
//   ZOOM   tap  -> Magnifier
public partial class MobileUI : CanvasLayer
{
    public static MobileUI Instance;
    private VBoxContainer _box;

    public override void _Ready()
    {
        Instance = this;
        Layer = 100;
        ProcessMode = ProcessModeEnum.Always;

        _box = new VBoxContainer();
        _box.SetAnchorsPreset(Control.LayoutPreset.TopRight);
        _box.Position = new Vector2(-8, 48);
        _box.GrowHorizontal = Control.GrowDirection.Begin;
        _box.AddThemeConstantOverride("separation", 6);
        AddChild(_box);

        AddHoldButton("MENU", "PauseGame");
        AddHoldButton("TERM", "Terminal");
        AddTapButton("SIT", "Sit");
        AddOutfitButton();
        AddHoldByteButton("LOCK", "Screen_Lock", null);
        AddHoldByteButton("AWAY", "Despawn", () => Main.Instance.RepositionAllItemsToMouseScreen());
        AddTapButton("ZOOM", "Magnifier");
    }

    public override void _Process(double delta)
    {
        Main m = Main.Instance;
        if (m == null)
        {
            return;
        }
        bool occupied = m.Pause != null || m.Terminal != null || (m.spawnedMinigames != null && m.spawnedMinigames.Count > 0);
        Visible = !occupied;
    }

    public static bool IsPointInUI(Vector2I pos)
    {
        if (Instance == null || !Instance.Visible || Instance._box == null)
        {
            return false;
        }
        foreach (Node child in Instance._box.GetChildren())
        {
            if (child is Control c && c.GetGlobalRect().HasPoint(pos))
            {
                return true;
            }
        }
        return false;
    }

    private Button MakeButton(string label)
    {
        Button b = new Button();
        b.Text = label;
        b.CustomMinimumSize = new Vector2(92, 44);
        b.AddThemeFontSizeOverride("font_size", 13);
        StyleBoxFlat normal = new StyleBoxFlat();
        normal.BgColor = new Color(0.08f, 0.08f, 0.14f, 0.72f);
        normal.BorderColor = new Color(1f, 1f, 1f, 0.25f);
        normal.SetBorderWidthAll(1);
        normal.SetCornerRadiusAll(8);
        b.AddThemeStyleboxOverride("normal", normal);
        StyleBoxFlat hover = (StyleBoxFlat)normal.Duplicate();
        hover.BgColor = new Color(0.18f, 0.18f, 0.28f, 0.82f);
        b.AddThemeStyleboxOverride("hover", hover);
        StyleBoxFlat pressed = (StyleBoxFlat)normal.Duplicate();
        pressed.BgColor = new Color(0.3f, 0.3f, 0.45f, 0.9f);
        b.AddThemeStyleboxOverride("pressed", pressed);
        return b;
    }

    private void AddHoldButton(string label, string action)
    {
        Button b = MakeButton(label);
        b.ButtonDown += () => Input.ActionPress(action);
        b.ButtonUp += () => Input.ActionRelease(action);
        _box.AddChild(b);
    }

    private void AddTapButton(string label, string action)
    {
        Button b = MakeButton(label);
        b.ButtonDown += () =>
        {
            Input.ActionPress(action);
            Callable.From(() => Input.ActionRelease(action)).CallDeferred();
        };
        _box.AddChild(b);
    }

    private void AddOutfitButton()
    {
        Button b = MakeButton("OUTFIT");
        bool longFired = false;
        b.ButtonDown += () =>
        {
            longFired = false;
            Input.ActionPress("Clothing_Up");
            Callable.From(() => Input.ActionRelease("Clothing_Up")).CallDeferred();
            SceneTreeTimer t = GetTree().CreateTimer(0.6);
            t.Timeout += () =>
            {
                if (b.ButtonPressed && !longFired)
                {
                    longFired = true;
                    Input.ActionPress("Clothing_Down");
                    Callable.From(() => Input.ActionRelease("Clothing_Down")).CallDeferred();
                }
            };
        };
        _box.AddChild(b);
    }

    private void AddHoldByteButton(string label, string action, Action onLongPress)
    {
        Button b = MakeButton(label);
        bool longFired = false;
        b.ButtonDown += () =>
        {
            longFired = false;
            Main.Instance.MobileActionOnByte(action, true);
            if (onLongPress != null)
            {
                SceneTreeTimer t = GetTree().CreateTimer(0.6);
                t.Timeout += () =>
                {
                    if (b.ButtonPressed && !longFired)
                    {
                        longFired = true;
                        Main.Instance.MobileActionOnByte(action, false);
                        onLongPress();
                    }
                };
            }
        };
        b.ButtonUp += () =>
        {
            if (!longFired)
            {
                Main.Instance.MobileActionOnByte(action, false);
            }
        };
        _box.AddChild(b);
    }

    // Shared helper for on-screen buttons inside minigame / terminal windows.
    public static Button MakeGameButton(string label, Control.LayoutPreset anchor, Vector2 pos, Vector2 size)
    {
        Button b = new Button();
        b.Text = label;
        b.SetAnchorsPreset(anchor);
        b.Position = pos;
        b.Size = size;
        b.AddThemeFontSizeOverride("font_size", 18);
        StyleBoxFlat sb = new StyleBoxFlat();
        sb.BgColor = new Color(0.1f, 0.1f, 0.16f, 0.78f);
        sb.BorderColor = new Color(1f, 1f, 1f, 0.3f);
        sb.SetBorderWidthAll(1);
        sb.SetCornerRadiusAll(10);
        b.AddThemeStyleboxOverride("normal", sb);
        return b;
    }
}
