using Godot;
using System;
using System.Collections.Generic;

public partial class MobileUI : CanvasLayer
{
    public static MobileUI Instance;
    private VBoxContainer _box;
    private Panel _spawnPanel;
    private readonly List<string> _managedActions = new List<string>
    {
        "PauseGame", "Terminal", "Sit", "Clothing_Up", "Clothing_Down"
    };

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

        AddTapActionButton("MENU", "PauseGame");
        AddTapActionButton("TERM", "Terminal");
        AddTapActionButton("SIT", "Sit");
        AddOutfitButton();
        AddSpawnButton();
    }

    public override void _Process(double delta)
    {
        Main m = Main.Instance;
        if (m == null) return;
        bool occupied = m.Pause != null || m.Terminal != null || (m.spawnedMinigames != null && m.spawnedMinigames.Count > 0) || (m.Magnifier != null && GodotObject.IsInstanceValid(m.Magnifier));
        bool spawnOpen = _spawnPanel != null && GodotObject.IsInstanceValid(_spawnPanel);

        if (occupied && _box != null && _box.Visible)
        {
            foreach (string a in _managedActions)
            {
                if (Input.IsActionPressed(a))
                    Input.ActionRelease(a);
            }
        }

        // Show/hide bar: hide when pause/terminal/minigame open, but keep visible when only spawn menu open? Actually hide bar when spawn open to avoid clutter
        if (_box != null)
            _box.Visible = !occupied && !spawnOpen;
        
        // Spawn panel visibility stays as is (don't auto-hide)
        // It will be freed only via CLOSE button
    }

    public static bool IsPointInUI(Vector2I pos)
    {
        if (Instance == null) return false;
        // Check main button box
        if (Instance._box != null && Instance._box.Visible)
        {
            if (IsPointInControlRecursive(Instance._box, pos))
                return true;
        }
        // Check spawn panel
        if (Instance._spawnPanel != null && GodotObject.IsInstanceValid(Instance._spawnPanel) && Instance._spawnPanel.Visible)
        {
            if (IsPointInControlRecursive(Instance._spawnPanel, pos))
                return true;
        }
        return false;
    }

    private static bool IsPointInControlRecursive(Control ctrl, Vector2I pos)
    {
        if (ctrl == null || !ctrl.Visible) return false;
        if (ctrl.GetGlobalRect().HasPoint(pos))
            return true;
        foreach (Node child in ctrl.GetChildren())
        {
            if (child is Control c && IsPointInControlRecursive(c, pos))
                return true;
        }
        return false;
    }

    private Button MakeButton(string label)
    {
        Button b = new Button();
        b.Text = label;
        b.CustomMinimumSize = new Vector2(100, 50);
        b.AddThemeFontSizeOverride("font_size", 14);
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
        b.MouseFilter = Control.MouseFilterEnum.Stop;
        b.MouseDefaultCursorShape = Control.CursorShape.PointingHand;
        return b;
    }

    private void AddTapActionButton(string label, string action)
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

    private void AddSpawnButton()
    {
        Button b = MakeButton("SPAWN");
        b.ButtonDown += () => ToggleSpawnMenu();
        _box.AddChild(b);
    }

    private void ToggleSpawnMenu()
    {
        if (_spawnPanel != null && GodotObject.IsInstanceValid(_spawnPanel))
        {
            _spawnPanel.QueueFree();
            _spawnPanel = null;
            return;
        }
        CreateSpawnMenu();
    }

    private void CreateSpawnMenu()
    {
        Panel panel = new Panel();
        panel.Name = "SpawnMenu";
        panel.SetAnchorsPreset(Control.LayoutPreset.FullRect);
        panel.OffsetLeft = 16;
        panel.OffsetTop = 16;
        panel.OffsetRight = -16;
        panel.OffsetBottom = -16;
        StyleBoxFlat bg = new StyleBoxFlat();
        bg.BgColor = new Color(0.05f, 0.05f, 0.08f, 0.92f);
        bg.SetCornerRadiusAll(12);
        bg.SetBorderWidthAll(1);
        bg.BorderColor = new Color(1f, 1f, 1f, 0.2f);
        panel.AddThemeStyleboxOverride("panel", bg);
        panel.ProcessMode = ProcessModeEnum.Always;
        panel.MouseFilter = Control.MouseFilterEnum.Stop;
        AddChild(panel);
        _spawnPanel = panel;

        VBoxContainer vbox = new VBoxContainer();
        vbox.SetAnchorsPreset(Control.LayoutPreset.FullRect);
        vbox.OffsetLeft = 12;
        vbox.OffsetTop = 12;
        vbox.OffsetRight = -12;
        vbox.OffsetBottom = -12;
        vbox.AddThemeConstantOverride("separation", 8);
        vbox.MouseFilter = Control.MouseFilterEnum.Pass;
        panel.AddChild(vbox);

        HBoxContainer header = new HBoxContainer();
        header.MouseFilter = Control.MouseFilterEnum.Pass;
        Label title = new Label();
        title.Text = "Spawn Items / Actors (Tap to spawn at Byte)";
        title.AddThemeFontSizeOverride("font_size", 16);
        title.MouseFilter = Control.MouseFilterEnum.Ignore;
        header.AddChild(title);
        Control spacer = new Control();
        spacer.CustomMinimumSize = new Vector2(20, 0);
        spacer.SizeFlagsHorizontal = Control.SizeFlags.Expand;
        spacer.MouseFilter = Control.MouseFilterEnum.Ignore;
        header.AddChild(spacer);
        Button close = MakeButton("CLOSE");
        close.CustomMinimumSize = new Vector2(80, 40);
        close.Pressed += () =>
        {
            if (_spawnPanel != null)
            {
                _spawnPanel.QueueFree();
                _spawnPanel = null;
            }
        };
        header.AddChild(close);
        vbox.AddChild(header);

        HBoxContainer quickRow = new HBoxContainer();
        quickRow.AddThemeConstantOverride("separation", 8);
        quickRow.MouseFilter = Control.MouseFilterEnum.Pass;
        Button randItem = MakeButton("Random Item");
        randItem.CustomMinimumSize = new Vector2(0, 40);
        randItem.SizeFlagsHorizontal = Control.SizeFlags.Expand;
        randItem.Pressed += () =>
        {
            Main m = Main.Instance;
            if (m != null) m.OnSpawnerTimeout();
        };
        quickRow.AddChild(randItem);
        Button randActor = MakeButton("Random Actor");
        randActor.CustomMinimumSize = new Vector2(0, 40);
        randActor.SizeFlagsHorizontal = Control.SizeFlags.Expand;
        randActor.Pressed += () =>
        {
            Main m = Main.Instance;
            if (m != null) m.OnSpawnerActorTimeout();
        };
        quickRow.AddChild(randActor);
        Button clearItems = MakeButton("Clear Items");
        clearItems.CustomMinimumSize = new Vector2(0, 40);
        clearItems.SizeFlagsHorizontal = Control.SizeFlags.Expand;
        clearItems.Pressed += () =>
        {
            Main m = Main.Instance;
            if (m != null)
            {
                var copy = new Godot.Collections.Array<ItemWindow>(m.spawnedItems);
                foreach (var it in copy)
                {
                    if (GodotObject.IsInstanceValid(it))
                        it.QueueFree();
                }
                m.spawnedItems.Clear();
            }
        };
        quickRow.AddChild(clearItems);
        vbox.AddChild(quickRow);

        ScrollContainer scroll = new ScrollContainer();
        scroll.SizeFlagsVertical = Control.SizeFlags.Expand;
        scroll.SizeFlagsHorizontal = Control.SizeFlags.Expand;
        scroll.HorizontalScrollMode = ScrollContainer.ScrollMode.Disabled;
        scroll.MouseFilter = Control.MouseFilterEnum.Pass;
        vbox.AddChild(scroll);

        GridContainer grid = new GridContainer();
        grid.Columns = 2;
        grid.AddThemeConstantOverride("h_separation", 8);
        grid.AddThemeConstantOverride("v_separation", 6);
        grid.MouseFilter = Control.MouseFilterEnum.Pass;
        grid.SizeFlagsHorizontal = Control.SizeFlags.Expand;
        scroll.AddChild(grid);

        Main main = Main.Instance;
        if (main == null)
        {
            Label lbl = new Label();
            lbl.Text = "Main not ready";
            grid.AddChild(lbl);
            return;
        }

        if (!ResourceCache.resourcesLoaded.ContainsKey(ResourceCache.ResourceTyping.ITEM) || ResourceCache.resourcesLoaded[ResourceCache.ResourceTyping.ITEM].Count == 0)
        {
            Label lbl = new Label();
            lbl.Text = "Items not loaded yet, retrying in 1s...";
            grid.AddChild(lbl);
            GetTree().CreateTimer(1.0).Timeout += () =>
            {
                if (_spawnPanel != null && GodotObject.IsInstanceValid(_spawnPanel))
                {
                    _spawnPanel.QueueFree();
                    _spawnPanel = null;
                    CreateSpawnMenu();
                }
            };
            return;
        }

        var items = ResourceCache.resourcesLoaded[ResourceCache.ResourceTyping.ITEM];
        foreach (string key in items.Keys)
        {
            if (items[key] is ItemDataRes itemRes)
            {
                Button ib = new Button();
                string display = itemRes.itemID;
                if (display.Length > 22) display = display.Substring(0, 22);
                ib.Text = display;
                ib.TooltipText = itemRes.itemID;
                ib.CustomMinimumSize = new Vector2(0, 38);
                ib.AddThemeFontSizeOverride("font_size", 11);
                ib.MouseFilter = Control.MouseFilterEnum.Stop;
                string captured = key;
                ib.Pressed += () =>
                {
                    Main m2 = Main.Instance;
                    if (m2 != null && ResourceCache.resourcesLoaded[ResourceCache.ResourceTyping.ITEM].ContainsKey(captured))
                    {
                        var res = ResourceCache.resourcesLoaded[ResourceCache.ResourceTyping.ITEM][captured] as ItemDataRes;
                        Vector2I pos = new Vector2I((int)(m2.Position.X + m2.mainCharacter.trueSize.X/2), (int)(m2.Position.Y));
                        m2.CallItemSpawn(res, pos);
                    }
                };
                grid.AddChild(ib);
            }
        }

        Label actorLabel = new Label();
        actorLabel.Text = "--- Actors ---";
        actorLabel.AddThemeFontSizeOverride("font_size", 14);
        actorLabel.MouseFilter = Control.MouseFilterEnum.Ignore;
        grid.AddChild(actorLabel);
        Label dummy = new Label();
        dummy.MouseFilter = Control.MouseFilterEnum.Ignore;
        grid.AddChild(dummy);

        if (ResourceCache.resourcesLoaded.ContainsKey(ResourceCache.ResourceTyping.CHARACTER))
        {
            var actors = ResourceCache.resourcesLoaded[ResourceCache.ResourceTyping.CHARACTER];
            foreach (string key in actors.Keys)
            {
                if (actors[key] is CharacterInfoDataRes charRes)
                {
                    Button ab = new Button();
                    string display = charRes.Name;
                    if (display.Length > 20) display = display.Substring(0, 20);
                    ab.Text = display;
                    ab.TooltipText = charRes._itemID;
                    ab.CustomMinimumSize = new Vector2(0, 38);
                    ab.AddThemeFontSizeOverride("font_size", 11);
                    ab.MouseFilter = Control.MouseFilterEnum.Stop;
                    string captured = key;
                    ab.Pressed += () =>
                    {
                        Main m2 = Main.Instance;
                        if (m2 != null && ResourceCache.resourcesLoaded[ResourceCache.ResourceTyping.CHARACTER].ContainsKey(captured))
                        {
                            var res = ResourceCache.resourcesLoaded[ResourceCache.ResourceTyping.CHARACTER][captured] as CharacterInfoDataRes;
                            m2.CallActorSpawn(res);
                        }
                    };
                    grid.AddChild(ab);
                }
            }
        }
    }

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
        b.MouseFilter = Control.MouseFilterEnum.Stop;
        return b;
    }
}
