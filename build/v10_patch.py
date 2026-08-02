#!/usr/bin/env python3
"""v10 patches: full mobile feature port (button bar, items, NPCs, minigames, terminal).
Run AFTER build/make_project.py has produced /home/user/.cache/v9/src (v9 state)."""
import os, sys, shutil

ROOT = '/home/user/.cache/v9'
SRC = f'{ROOT}/src'
PROJ = f'{ROOT}/proj'
REPO = '/home/user/teat/repo'

def fail(msg):
    print(f'FATAL: {msg}')
    sys.exit(1)

def normalize(path):
    with open(path, encoding='utf-8') as f:
        lines = f.readlines()
    out = []
    for l in lines:
        n = 0
        while n < len(l) and l[n] == '\t':
            n += 1
        out.append('    ' * n + l[n:])
    with open(path, 'w', encoding='utf-8') as f:
        f.writelines(out)

def patch_file(path, replacements):
    with open(path, encoding='utf-8') as f:
        src = f.read()
    for old, new, tag in replacements:
        if old not in src:
            fail(f'[{tag}] pattern not found in {path}:\n{old[:250]}')
        if src.count(old) != 1:
            fail(f'[{tag}] pattern matches {src.count(old)}x in {path}')
        src = src.replace(old, new)
        print(f'  ✓ {tag} ({os.path.basename(path)})')
    with open(path, 'w', encoding='utf-8') as f:
        f.write(src)

# ================================================================ Main.cs
main_cs = f'{SRC}/Scripts/Main.cs'
normalize(main_cs)
patch_file(main_cs, [
    # M1: new touch-state fields + build marker
    ('''    public static Vector2I _lastTouchPos = new Vector2I(0,0);''',
     '''    public static Vector2I _lastTouchPos = new Vector2I(0,0);
    public static readonly string V10_BUILD = "V10_MOBILEUI_BUILD";
    public enum MobileTouchTargetKind { None, Byte, Item, Actor }
    public int _mobileTouchIndex = -1;
    public Vector2I _mobileTouchStart;
    public double _mobileTouchStartTime;
    public bool _mobileTouchMoved;
    public MobileTouchTargetKind _mobileTouchTarget = MobileTouchTargetKind.None;
    public Vector2 _mobileDragVelocity;
    private Vector2I _prevTouchPos;
    private double _lastDragTime;
    private Vector2I _savedTouchPos;''',
     'M1 touch state fields'),

    # M2: rewrite mobile _Input (target routing: item/actor/byte/none + long-press pause)
    ('''    public override void _Input(InputEvent @event)
    {
        if (_isMobile)
        {
            if (@event is InputEventScreenTouch touch)
            {
                _lastTouchPos = (Vector2I)touch.Position;
                bool hasPoint = false;
                try { hasPoint = GetThinnerCollisionBox().HasPoint(_lastTouchPos); } catch { hasPoint = true; }
                if (touch.Pressed)
                {
                    if (hasPoint)
                    {
                        if (!Input.IsActionPressed("Move"))
                            Input.ActionPress("Move");
                        if (!Input.IsActionPressed("Pet"))
                            Input.ActionPress("Pet");
                    }
                }
                else
                {
                    if (Input.IsActionPressed("Move"))
                        Input.ActionRelease("Move");
                    if (Input.IsActionPressed("Pet"))
                        Input.ActionRelease("Pet");
                }
            }
            else if (@event is InputEventScreenDrag drag)
            {
                _lastTouchPos = (Vector2I)drag.Position;
                if (!Input.IsActionPressed("Move"))
                    Input.ActionPress("Move");
            }
        }
        base._Input(@event);
    }''',
     '''    public override void _Input(InputEvent @event)
    {
        if (_isMobile)
        {
            if (@event is InputEventScreenTouch touch)
            {
                Vector2I pos = (Vector2I)touch.Position;
                _lastTouchPos = pos;
                if (touch.Pressed)
                {
                    if (MobileUI.IsPointInUI(pos))
                    {
                        return;
                    }
                    _mobileTouchIndex = touch.Index;
                    _mobileTouchStart = pos;
                    _mobileTouchStartTime = Time.GetTicksMsec() / 1000.0;
                    _mobileTouchMoved = false;
                    _prevTouchPos = pos;
                    _mobileDragVelocity = Vector2.Zero;
                    _lastDragTime = _mobileTouchStartTime;
                    if (IsPointOnAnyItem(pos))
                        _mobileTouchTarget = MobileTouchTargetKind.Item;
                    else if (IsPointOnAnyActor(pos))
                        _mobileTouchTarget = MobileTouchTargetKind.Actor;
                    else if (GetThinnerCollisionBox().HasPoint(pos))
                        _mobileTouchTarget = MobileTouchTargetKind.Byte;
                    else
                        _mobileTouchTarget = MobileTouchTargetKind.None;
                    if (_mobileTouchTarget == MobileTouchTargetKind.Actor)
                        Input.ActionPress("Pet");
                    if (_mobileTouchTarget == MobileTouchTargetKind.Byte)
                    {
                        Input.ActionPress("Move");
                        Input.ActionPress("Pet");
                    }
                }
                else if (touch.Index == _mobileTouchIndex)
                {
                    double duration = Time.GetTicksMsec() / 1000.0 - _mobileTouchStartTime;
                    if (_mobileTouchTarget == MobileTouchTargetKind.Actor)
                        Input.ActionRelease("Pet");
                    if (_mobileTouchTarget == MobileTouchTargetKind.Byte)
                    {
                        Input.ActionRelease("Move");
                        Input.ActionRelease("Pet");
                    }
                    if (_mobileTouchTarget == MobileTouchTargetKind.None && duration >= 0.5 && !_mobileTouchMoved)
                    {
                        Input.ActionPress("PauseGame");
                        Callable.From(() => Input.ActionRelease("PauseGame")).CallDeferred();
                    }
                    _mobileTouchTarget = MobileTouchTargetKind.None;
                    _mobileTouchIndex = -1;
                }
            }
            else if (@event is InputEventScreenDrag drag)
            {
                if (drag.Index == _mobileTouchIndex)
                {
                    Vector2I pos = (Vector2I)drag.Position;
                    _lastTouchPos = pos;
                    if (pos.DistanceTo(_mobileTouchStart) > 24f)
                        _mobileTouchMoved = true;
                    double now = Time.GetTicksMsec() / 1000.0;
                    double dt = Math.Max(now - _lastDragTime, 0.001);
                    _mobileDragVelocity = (pos - _prevTouchPos) / (float)dt;
                    _prevTouchPos = pos;
                    _lastDragTime = now;
                    if (_mobileTouchTarget == MobileTouchTargetKind.Byte && !Input.IsActionPressed("Move"))
                        Input.ActionPress("Move");
                }
            }
            return;
        }
        base._Input(@event);
    }''',
     'M2 touch target routing'),

    # M3: helpers after MobileMousePos
    ('''    public Vector2I MobileMousePos()
    {
        if (_isMobile)
        {
            if (_lastTouchPos != Vector2I.Zero)
                return _lastTouchPos;
        }
        return DisplayServer.MouseGetPosition();
    }''',
     '''    public Vector2I MobileMousePos()
    {
        if (_isMobile)
        {
            if (_lastTouchPos != Vector2I.Zero)
                return _lastTouchPos;
        }
        return DisplayServer.MouseGetPosition();
    }

    public bool IsPointOnAnyItem(Vector2I p)
    {
        for (int i = spawnedItems.Count - 1; i >= 0; i--)
        {
            ItemWindow w = spawnedItems[i];
            if (GodotObject.IsInstanceValid(w) && w.Visible && new Rect2I(w.Position, w.Size).HasPoint(p))
                return true;
        }
        return false;
    }

    public bool IsPointOnAnyActor(Vector2I p)
    {
        for (int i = spawnedActors.Count - 1; i >= 0; i--)
        {
            ActorWindow w = spawnedActors[i];
            if (GodotObject.IsInstanceValid(w) && w.Visible && new Rect2I(w.Position, w.Size).HasPoint(p))
                return true;
        }
        return false;
    }

    // Fake a touch at Byte's center so hit-tested actions (Screen_Lock, Despawn)
    // work when triggered from the on-screen button bar.
    public void MobileActionOnByte(string action, bool pressed)
    {
        if (pressed)
        {
            _savedTouchPos = _lastTouchPos;
            _lastTouchPos = (Vector2I)(Position + (Vector2)mainCharacter.trueSize * 0.5f);
            Input.ActionPress(action);
        }
        else
        {
            Input.ActionRelease(action);
            _lastTouchPos = _savedTouchPos;
        }
    }''',
     'M3 touch helpers'),

    # M4: create MobileUI in _Ready
    ('''        Callable.From(delegate
        {
            BootDialogue(!settingEULA);
        }).CallDeferred();''',
     '''        if (_isMobile)
        {
            AddChild(new MobileUI());
        }
        Callable.From(delegate
        {
            BootDialogue(!settingEULA);
        }).CallDeferred();''',
     'M4 create MobileUI'),

    # M5: collision box public (ItemWindow uses it)
    ('''    private Rect2I GetThinnerCollisionBox()''',
     '''    public Rect2I GetThinnerCollisionBox()''',
     'M5 collision box public'),

    # M6: Vector2I/float division fix in drag velocity
    ('''                    _mobileDragVelocity = (pos - _prevTouchPos) / (float)dt;''',
     '''                    _mobileDragVelocity = (Vector2)(pos - _prevTouchPos) / (float)dt;''',
     'M6 drag velocity cast'),

    # M7: RepositionAllItemsToMouseScreen public (MobileUI long-press AWAY)
    ('''    private void RepositionAllItemsToMouseScreen()''',
     '''    public void RepositionAllItemsToMouseScreen()''',
     'M7 reposition public'),
])

# ================================================================ ItemWindow.cs
item_cs = f'{SRC}/Scripts/ItemScripts/ItemWindow.cs'
normalize(item_cs)
patch_file(item_cs, [
    # I1: mobile branch in _Process
    ('''        if (!isSetup)
        {
            return;
        }
        if (CurrentlyPickedUp)''',
     '''        if (!isSetup)
        {
            return;
        }
        if (Main._isMobile)
        {
            if (CurrentlyPickedUp)
            {
                base.Visible = false;
                base.MousePassthrough = true;
                return;
            }
            ProcessMobileItem(delta);
        }
        if (CurrentlyPickedUp)''',
     'I1 mobile branch in _Process'),

    # I2: mobile grab/drag/use/combine/throw
    ('''    private void FollowMouse()''',
     '''    private bool _mobileHeld;
    private Vector2I _mobileOffset;

    private bool IsMobileTouchOnThisItem(Vector2I startPos)
    {
        return new Rect2I(base.Position, base.Size).HasPoint(startPos);
    }

    private void ProcessMobileItem(double delta)
    {
        Main m = Main.Instance;
        if (m == null)
        {
            return;
        }
        bool isActive = m._mobileTouchIndex >= 0 && m._mobileTouchTarget == Main.MobileTouchTargetKind.Item && IsMobileTouchOnThisItem(m._mobileTouchStart);
        if (isActive)
        {
            if (!_mobileHeld)
            {
                _mobileHeld = true;
                m.SomethingHasBeenGrabbed = true;
                m.mainWindow.AlwaysOnTop = false;
                _mobileOffset = (Vector2I)base.Position - Main._lastTouchPos;
                UpdateCombinationShaders(enable: true);
            }
            Vector2I screenSize = DisplayServer.ScreenGetSize(m.screenDataHandler.screenIndex);
            Vector2I newPos = Main._lastTouchPos + _mobileOffset;
            newPos.X = Mathf.Clamp(newPos.X, 0, screenSize.X - itemObject.trueSize.X);
            newPos.Y = Mathf.Clamp(newPos.Y, 0, screenSize.Y - itemObject.trueSize.Y);
            base.Position = newPos;
        }
        else if (_mobileHeld)
        {
            _mobileHeld = false;
            m.SomethingHasBeenGrabbed = false;
            m.mainWindow.AlwaysOnTop = true;
            UpdateCombinationShaders(enable: false);
            if (m.settingWindowThrowPhysics && m._mobileDragVelocity.Length() > 260f)
            {
                itemWindowVelocity = m._mobileDragVelocity * m.mouseVelocityScaler;
                itemWindowVelocity.Y = Mathf.Clamp(itemWindowVelocity.Y, -1600f, float.MaxValue);
                isThrown = true;
            }
            Rect2I thisRect = new Rect2I(base.Position, base.Size);
            if (m.mainCharacter.Visible && m.GetThinnerCollisionBox().Intersects(thisRect))
            {
                UseOnMainActor();
                if (!itemObject.itemInformation.isReusable)
                {
                    m.TweenGrabRelease();
                    m.spawnedItems.Remove(this);
                    CallDeferred("queue_free");
                }
                return;
            }
            bool usedOnActor = false;
            if (itemObject.itemInformation.possibleUsableAIs.Count() > 0)
            {
                foreach (ActorWindow spawnedActor in m.spawnedActors)
                {
                    if (!GodotObject.IsInstanceValid(spawnedActor))
                    {
                        continue;
                    }
                    AiItemDataRes aiItemDataRes = null;
                    foreach (AiItemDataRes possibleUsableAI in itemObject.itemInformation.possibleUsableAIs)
                    {
                        if (possibleUsableAI.targetActorsID == spawnedActor.characterActor.characterInformation._itemID)
                        {
                            aiItemDataRes = possibleUsableAI;
                            break;
                        }
                    }
                    if (aiItemDataRes == null)
                    {
                        continue;
                    }
                    if (new Rect2I(spawnedActor.Position, spawnedActor.Size).Intersects(thisRect) && spawnedActor.Visible)
                    {
                        UseOnOtherActor(spawnedActor, aiItemDataRes);
                        usedOnActor = true;
                        if (!itemObject.itemInformation.isReusable)
                        {
                            m.TweenGrabRelease();
                            m.spawnedItems.Remove(this);
                            CallDeferred("queue_free");
                        }
                        break;
                    }
                }
            }
            if (!usedOnActor)
            {
                CombineItem(thisRect);
            }
        }
    }

    private void FollowMouse()''',
     'I2 mobile item interactions'),

    # I3: skip desktop "click item to delete" on mobile (accidental taps)
    ('''    private void PopItem()
    {
        if (Input.IsActionJustPressed("Pet") && !selected && new Rect2I(base.Position, base.Size).HasPoint(DisplayServer.MouseGetPosition()))''',
     '''    private void PopItem()
    {
        if (Main._isMobile)
        {
            return;
        }
        if (Input.IsActionJustPressed("Pet") && !selected && new Rect2I(base.Position, base.Size).HasPoint(DisplayServer.MouseGetPosition()))''',
     'I3 PopItem mobile guard'),
])

# ================================================================ ActorWindow.cs
actor_cs = f'{SRC}/Scripts/CharacterScripts/ActorWindow.cs'
normalize(actor_cs)
patch_file(actor_cs, [
    ('''            WalkToPlayer(delta, (targetWindow != null) ? targetWindow.Position.X : Main.Instance.mainWindow.Position.X, chaseStoppingDistance);''',
     '''            WalkToPlayer(delta, (targetWindow != null) ? targetWindow.Position.X : (Main._isMobile ? (int)Main.Instance.Position.X : Main.Instance.mainWindow.Position.X), chaseStoppingDistance);''',
     'A1 walk target X mobile'),
    ('''        if (!cachedThinBox.HasPoint(DisplayServer.MouseGetPosition()))''',
     '''        if (!cachedThinBox.HasPoint(Main._isMobile ? Main.Instance.MobileMousePos() : DisplayServer.MouseGetPosition()))''',
     'A2 pop hit-test mobile'),
    ('''                float num = (float)Main.Instance.mainWindow.Position.X + (float)Main.Instance.mainCharacter.trueSize.X / 2f;''',
     '''                float num = (float)(Main._isMobile ? (int)Main.Instance.Position.X : Main.Instance.mainWindow.Position.X) + (float)Main.Instance.mainCharacter.trueSize.X / 2f;''',
     'A3 follow-center X mobile'),
    ('''        if (Mathf.Abs(walkX - (float)Main.Instance.mainWindow.Position.X) >= num4 && characterActor.MainBody.Animation != (StringName)"Walk")''',
     '''        if (Mathf.Abs(walkX - (float)(Main._isMobile ? (int)Main.Instance.Position.X : Main.Instance.mainWindow.Position.X)) >= num4 && characterActor.MainBody.Animation != (StringName)"Walk")''',
     'A4 follow-distance X mobile'),
])

# ================================================================ AttachObjWindow.cs
att_cs = f'{SRC}/Scripts/AttachmentScripts/AttachObjWindow.cs'
normalize(att_cs)
patch_file(att_cs, [
    ('''        Vector2I position = parentWindow.Position;''',
     '''        Vector2I position = (Main._isMobile && parentWindow == Main.Instance.mainWindow) ? (Vector2I)Main.Instance.Position : parentWindow.Position;''',
     'B1 LayerOntoParent mobile base pos'),
])

# ================================================================ DR_GameHandler.cs (Dino Runner)
dr_cs = f'{SRC}/Scripts/Minigames/DinoRunner/DR_GameHandler.cs'
normalize(dr_cs)
patch_file(dr_cs, [
    ('''    public override void _Ready()
    {
        base._Ready();''',
     '''    private bool _mobileButtonsCreated;

    private void EnsureMobileButtons()
    {
        if (_mobileButtonsCreated)
        {
            return;
        }
        _mobileButtonsCreated = true;
        Button jump = MobileUI.MakeGameButton("JUMP", Control.LayoutPreset.BottomLeft, new Vector2(24, -176), new Vector2(150, 64));
        jump.ButtonDown += () => Input.ActionPress("DR_Jump");
        jump.ButtonUp += () => Input.ActionRelease("DR_Jump");
        AddChild(jump);
        Button duck = MobileUI.MakeGameButton("DUCK", Control.LayoutPreset.BottomLeft, new Vector2(24, -100), new Vector2(150, 64));
        duck.ButtonDown += () => Input.ActionPress("DR_Duck");
        duck.ButtonUp += () => Input.ActionRelease("DR_Duck");
        AddChild(duck);
    }

    public override void _Ready()
    {
        base._Ready();
        if (Main._isMobile)
        {
            EnsureMobileButtons();
        }''',
     'D1 dino runner mobile buttons'),
])

# ================================================================ SG_GameHandler.cs (Snake)
sg_cs = f'{SRC}/Scripts/Minigames/SnakeGame/SG_GameHandler.cs'
normalize(sg_cs)
patch_file(sg_cs, [
    ('''    public override void _Ready()
    {
        base._Ready();''',
     '''    private bool _mobileControlsCreated;

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
        }''',
     'S1 snake mobile dpad'),
])

# ================================================================ CatchHerGameLogic.cs
ch_cs = f'{SRC}/Scripts/Minigames/CatchHer/CatchHerGameLogic.cs'
normalize(ch_cs)
patch_file(ch_cs, [
    # C1: mobile fields + buttons (insert before _Ready)
    ('''    public override void _Ready()''',
     '''    private bool _mobileControlsCreated;
    private bool _mobileLeft;
    private bool _mobileRight;
    private bool _mobileJump;
    private Button _jumpButton;

    private void EnsureMobileControls()
    {
        if (_mobileControlsCreated)
        {
            return;
        }
        _mobileControlsCreated = true;
        _jumpButton = MobileUI.MakeGameButton("JUMP", Control.LayoutPreset.CenterBottom, new Vector2(-80, -120), new Vector2(160, 80));
        _jumpButton.ButtonDown += () => _mobileJump = true;
        _jumpButton.ButtonUp += () => _mobileJump = false;
        AddChild(_jumpButton);
    }

    public override void _Ready()''',
     'C1 catch-her mobile fields'),

    # C2: hook EnsureMobileControls in _Process
    ('''    public override void _Process(double delta)
    {
        switch (currentState)''',
     '''    public override void _Process(double delta)
    {
        if (Main._isMobile)
        {
            EnsureMobileControls();
        }
        switch (currentState)''',
     'C2 catch-her _Process hook'),

    # C3: touch halves in _Input
    ('''    public override void _Input(InputEvent ev)
    {
        if (currentState == GameState.WaitClick && ev is InputEventMouseButton inputEventMouseButton && inputEventMouseButton.ButtonIndex == MouseButton.Left && inputEventMouseButton.Pressed)''',
     '''    public override void _Input(InputEvent ev)
    {
        if (Main._isMobile && ev is InputEventScreenTouch touchEvent)
        {
            Vector2 tpos = touchEvent.Position;
            bool onJump = _jumpButton != null && _jumpButton.GetGlobalRect().HasPoint(tpos);
            bool leftHalf = !onJump && tpos.X < (float)GetViewportRect().Size.X / 2f;
            if (touchEvent.Pressed)
            {
                if (onJump)
                {
                    _mobileJump = true;
                }
                else if (leftHalf)
                {
                    _mobileLeft = true;
                }
                else
                {
                    _mobileRight = true;
                }
            }
            else
            {
                if (onJump)
                {
                    _mobileJump = false;
                }
                else if (leftHalf)
                {
                    _mobileLeft = false;
                }
                else
                {
                    _mobileRight = false;
                }
            }
        }
        if (currentState == GameState.WaitClick && ev is InputEventMouseButton inputEventMouseButton && inputEventMouseButton.ButtonIndex == MouseButton.Left && inputEventMouseButton.Pressed)''',
     'C3 catch-her touch input'),

    # C4: UpdatePlayer mobile keys
    ('''        if (Input.IsKeyPressed(Key.D))
        {
            num += 1f;
        }
        if (Input.IsKeyPressed(Key.A))
        {
            num -= 1f;
        }
        playerVelocity.X = num * 280f;
        if (PlayerScene.IsOnFloor() && (Input.IsKeyPressed(Key.Space) || Input.IsKeyPressed(Key.W)))
        {
            playerVelocity.Y = -920f;
        }''',
     '''        if (Input.IsKeyPressed(Key.D) || (Main._isMobile && _mobileRight))
        {
            num += 1f;
        }
        if (Input.IsKeyPressed(Key.A) || (Main._isMobile && _mobileLeft))
        {
            num -= 1f;
        }
        playerVelocity.X = num * 280f;
        if (PlayerScene.IsOnFloor() && (Input.IsKeyPressed(Key.Space) || Input.IsKeyPressed(Key.W) || (Main._isMobile && _mobileJump)))
        {
            playerVelocity.Y = -920f;
        }''',
     'C4 catch-her mobile movement'),
])

# ================================================================ TerminalHandler.cs
th_cs = f'{SRC}/Scripts/SubMenus/TerminalMenu/TerminalHandler.cs'
normalize(th_cs)
patch_file(th_cs, [
    ('''        _inputTrap.GrabFocus();
        if (OS.HasFeature("editor"))''',
     '''        _inputTrap.GrabFocus();
        if (Main._isMobile)
        {
            Button send = MobileUI.MakeGameButton("SEND", Control.LayoutPreset.BottomRight, new Vector2(-110, -60), new Vector2(100, 48));
            AddChild(send);
            send.Pressed += () =>
            {
                HandleSubmit(_inputTrap.Text);
                _inputTrap.GrabFocus();
            };
        }
        if (OS.HasFeature("editor"))''',
     'T1 terminal SEND button'),
])

# ================================================================ copy MobileUI into project
shutil.copy(f'{REPO}/patched/Scripts/MobileUI.cs', f'{SRC}/Scripts/MobileUI.cs')
print('  ✓ MobileUI.cs copied')

# ================================================================ sync to proj
if os.path.exists(f'{PROJ}/Scripts'):
    shutil.rmtree(f'{PROJ}/Scripts')
shutil.copytree(f'{SRC}/Scripts', f'{PROJ}/Scripts')
print('proj/Scripts synced')

print('\nAll v10 patches applied.')
