#!/usr/bin/env python3
"""v11 patches: bugfix pass on v10.
Fixes:
 - stuck-action lockout (hold buttons -> tap buttons; force-release on bar hide)
 - terminal: CLOSE button (couldn't exit -> "enter once then locked out")
 - magnifier: touch-follow, CLOSE + zoom buttons ("can't unzoom", "buggy")
 - items: own-window touch handling (v10 relied on Main._Input which never
   receives touches routed to subwindows -> item drag was dead)
 - actors: own-window touch -> Pet (tap enemy to pop)
 - item/NPC spawning: resource guard + retry, mobile spawn at top edge
   (desktop spawned at negative Y; if fall logic stalls -> invisible items)
 - timers started explicitly on mobile in _Ready (autostart safety)
 - AWAY: hide Byte when visible / bring her back when hidden (desktop Despawn
   only acts in the hidden state -> button seemed dead)
 - dino runner: IsMouseOver always true on mobile (no mouse -> game never started)
"""
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
    # M1: start spawner timers explicitly on mobile
    ('''        if (_isMobile)
        {
            AddChild(new MobileUI());
        }''',
     '''        if (_isMobile)
        {
            AddChild(new MobileUI());
            if (spawnerTimer != null && spawnerTimer.IsStopped())
            {
                spawnerTimer.WaitTime = 4f;
                spawnerTimer.Start();
            }
            if (spawnerActorTimer != null && spawnerActorTimer.IsStopped())
            {
                spawnerActorTimer.WaitTime = 20f;
                spawnerActorTimer.Start();
            }
        }''',
     'M1 start spawner timers on mobile'),

    # M2: resource guard in item spawner (quick retry instead of silent death)
    ('''            spawnerTimer.WaitTime = GD.RandRange(spawnerTimeRange.X, spawnerTimeRange.Y);
            spawnerTimer.Start();
            if (spawnedItems.Count >= maxItems)
            {
                return;
            }''',
     '''            spawnerTimer.WaitTime = GD.RandRange(spawnerTimeRange.X, spawnerTimeRange.Y);
            spawnerTimer.Start();
            if (spawnedItems.Count >= maxItems)
            {
                return;
            }
            if (!ResourceCache.resourcesLoaded.ContainsKey(ResourceCache.ResourceTyping.ITEM) || ResourceCache.resourcesLoaded[ResourceCache.ResourceTyping.ITEM].Count == 0)
            {
                GD.PrintErr("[Spawner] ITEM resources not ready yet - retrying in 5s");
                spawnerTimer.WaitTime = 5f;
                spawnerTimer.Start();
                return;
            }''',
     'M2 item resource guard'),

    # M3: mobile spawn position (top edge, on-screen) vs desktop (above top)
    ('''            if (x == -1 && y == -1)
            {
                int num = (int)GD.RandRange((float)screenDataHandler.EffectiveLeftX + spawnMargin.X, (float)screenDataHandler.EffectiveRightX - spawnMargin.X);
                int screenCount = DisplayServer.GetScreenCount();
                int y2 = DisplayServer.ScreenGetUsableRect(screenDataHandler.screenIndex).Position.Y;
                for (int i = 0; i < screenCount; i++)
                {
                    Rect2I rect2I = DisplayServer.ScreenGetUsableRect(i);
                    if (num >= rect2I.Position.X && num < rect2I.Position.X + rect2I.Size.X)
                    {
                        y2 = rect2I.Position.Y;
                        break;
                    }
                }
                itemWindow.Position = new Vector2I(num, y2 - Mathf.RoundToInt(spawnMargin.Y));
            }''',
     '''            if (x == -1 && y == -1)
            {
                int num = (int)GD.RandRange((float)screenDataHandler.EffectiveLeftX + spawnMargin.X, (float)screenDataHandler.EffectiveRightX - spawnMargin.X);
                if (Main._isMobile)
                {
                    Vector2I sz = DisplayServer.ScreenGetSize(screenDataHandler.screenIndex);
                    num = Mathf.Clamp(num, 0, Mathf.Max(0, sz.X - itemWindow.Size.X));
                    int yTop = Mathf.Max(DisplayServer.ScreenGetUsableRect(screenDataHandler.screenIndex).Position.Y, 0);
                    itemWindow.Position = new Vector2I(num, yTop);
                }
                else
                {
                    int screenCount = DisplayServer.GetScreenCount();
                    int y2 = DisplayServer.ScreenGetUsableRect(screenDataHandler.screenIndex).Position.Y;
                    for (int i = 0; i < screenCount; i++)
                    {
                        Rect2I rect2I = DisplayServer.ScreenGetUsableRect(i);
                        if (num >= rect2I.Position.X && num < rect2I.Position.X + rect2I.Size.X)
                        {
                            y2 = rect2I.Position.Y;
                            break;
                        }
                    }
                    itemWindow.Position = new Vector2I(num, y2 - Mathf.RoundToInt(spawnMargin.Y));
                }
            }''',
     'M3 mobile spawn position'),

    # M4: CloseMagnifier + ToggleDespawnMobile after MobileActionOnByte
    ('''    public void MobileActionOnByte(string action, bool pressed)
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
     '''    public void MobileActionOnByte(string action, bool pressed)
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
    }

    public void CloseMagnifier()
    {
        _magnifierActive = false;
        if (Magnifier != null && GodotObject.IsInstanceValid(Magnifier))
        {
            Magnifier.QueueFree();
        }
        Magnifier = null;
    }

    public void ToggleDespawnMobile()
    {
        if (!_isMobile)
        {
            return;
        }
        ClearAllAttachments();
        dialogueStack.Clear();
        dialogueStack.Add(mainCharacter.characterInformation.responseTexts[CharacterInfoDataRes.ResponseToSituation.IN_CONVO]);
        PopDialogueInStack(skipTimer: true);
        if (mainCharacter.Visible)
        {
            mainCharacter.Visible = false;
        }
        else
        {
            mainCharacter.ForceMainBodyState(Character.MainBodyStates.Forced_Animation, "Pet", 0.5f);
        }
        saveHandler.SaveSettings();
    }''',
     'M4 CloseMagnifier + ToggleDespawnMobile'),
])

# ================================================================ ItemWindow.cs
item_cs = f'{SRC}/Scripts/ItemScripts/ItemWindow.cs'
normalize(item_cs)
with open(item_cs, encoding='utf-8') as f:
    item_src = f.read()

start_anchor = '    private bool _mobileHeld;\n    private Vector2I _mobileOffset;'
end_anchor = '    private void FollowMouse()'
if start_anchor not in item_src:
    fail('[I1] start anchor not found in ItemWindow.cs')
if end_anchor not in item_src:
    fail('[I1] end anchor not found in ItemWindow.cs')
i = item_src.index(start_anchor)
j = item_src.index(end_anchor, i + len(start_anchor))
new_block = '''    private bool _mobileHeld;
    private Vector2I _mobileGrabLocal;
    private Vector2I _mobileGrabWindowPos;
    private Vector2I _mobileLastLocal;
    private Vector2 _mobileVelocity;
    private double _mobileLastTime;

    public override void _Input(InputEvent @event)
    {
        if (!Main._isMobile || !isSetup || CurrentlyPickedUp)
        {
            return;
        }
        if (@event is InputEventScreenTouch touch)
        {
            if (touch.Pressed)
            {
                if (Main.Instance.SomethingHasBeenGrabbed)
                {
                    return;
                }
                _mobileHeld = true;
                Main.Instance.SomethingHasBeenGrabbed = true;
                Main.Instance.mainWindow.AlwaysOnTop = false;
                _mobileGrabLocal = (Vector2I)touch.Position;
                _mobileGrabWindowPos = base.Position;
                _mobileLastLocal = _mobileGrabLocal;
                _mobileVelocity = Vector2.Zero;
                _mobileLastTime = Time.GetTicksMsec() / 1000.0;
                UpdateCombinationShaders(enable: true);
                if (itemObject.itemInformation.possiblePickUpDialogue.Count() > 0 && Main.Instance.mainCharacter.Visible && (float)GD.RandRange(0, 100) < itemObject.itemInformation.PerchentChanceOfDialogue)
                {
                    DialogueDataRes dialogueDataRes = Main.Instance.PickDialogue(itemObject.itemInformation.possiblePickUpDialogue);
                    if (dialogueDataRes != null)
                    {
                        Main.Instance.dialogueStack.Add(dialogueDataRes);
                        Main.Instance.PopDialogueInStack(skipTimer: true);
                    }
                }
            }
            else if (_mobileHeld)
            {
                ReleaseMobileItem();
            }
        }
        else if (@event is InputEventScreenDrag drag && _mobileHeld)
        {
            Vector2I local = (Vector2I)drag.Position;
            double now = Time.GetTicksMsec() / 1000.0;
            double dt = Mathf.Max((float)(now - _mobileLastTime), 0.001f);
            _mobileVelocity = ((Vector2)(local - _mobileLastLocal)) / (float)dt;
            _mobileLastLocal = local;
            _mobileLastTime = now;
            ApplyMobilePosition();
        }
        base._Input(@event);
    }

    private Vector2I ClampMobilePos(Vector2I pos)
    {
        Vector2I screenSize = DisplayServer.ScreenGetSize(Main.Instance.screenDataHandler.screenIndex);
        pos.X = Mathf.Clamp(pos.X, 0, Mathf.Max(0, screenSize.X - itemObject.trueSize.X));
        pos.Y = Mathf.Clamp(pos.Y, 0, Mathf.Max(0, screenSize.Y - itemObject.trueSize.Y));
        return pos;
    }

    private void ApplyMobilePosition()
    {
        base.Position = ClampMobilePos(_mobileGrabWindowPos + (_mobileLastLocal - _mobileGrabLocal));
    }

    private void ReleaseMobileItem()
    {
        _mobileHeld = false;
        Main m = Main.Instance;
        if (m == null)
        {
            return;
        }
        m.SomethingHasBeenGrabbed = false;
        m.mainWindow.AlwaysOnTop = true;
        UpdateCombinationShaders(enable: false);
        ApplyMobilePosition();
        if (m.settingWindowThrowPhysics && _mobileVelocity.Length() > 260f)
        {
            itemWindowVelocity = _mobileVelocity * m.mouseVelocityScaler;
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

    private void ProcessMobileItem(double delta)
    {
        if (_mobileHeld)
        {
            ApplyMobilePosition();
        }
    }

'''
item_src = item_src[:i] + new_block + item_src[j:]
with open(item_cs, 'w', encoding='utf-8') as f:
    f.write(item_src)
print('  ✓ I1 ItemWindow self-contained touch handling')

# ================================================================ ActorWindow.cs
actor_cs = f'{SRC}/Scripts/CharacterScripts/ActorWindow.cs'
normalize(actor_cs)
patch_file(actor_cs, [
    ('''    private void PopActor()''',
     '''    public override void _Input(InputEvent @event)
    {
        if (Main._isMobile && !inUseByAttachment && @event is InputEventScreenTouch touch)
        {
            if (touch.Pressed && !Main.Instance.SomethingHasBeenGrabbed)
            {
                Main._lastTouchPos = new Rect2I(base.Position, base.Size).GetCenter();
                if (!Input.IsActionPressed("Pet"))
                {
                    Input.ActionPress("Pet");
                }
            }
            else if (!touch.Pressed)
            {
                if (Input.IsActionPressed("Pet"))
                {
                    Input.ActionRelease("Pet");
                }
            }
        }
        base._Input(@event);
    }

    private void PopActor()''',
     'A1 actor mobile tap'),
])

# ================================================================ TerminalWindow.cs
tw_cs = f'{SRC}/Scripts/SubMenus/TerminalMenu/TerminalWindow.cs'
normalize(tw_cs)
patch_file(tw_cs, [
    ('''    public override void _Process(double delta)
    {
        if (Input.IsActionJustPressed("PauseGame"))
        {
            OnClose();
        }
    }''',
     '''    private Button _closeButton;

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
    }''',
     'T1 terminal CLOSE button'),
])

# ================================================================ MagnifierWindow.cs
mw_cs = f'{SRC}/Scripts/SubMenus/MagnifierMenu/MagnifierWindow.cs'
normalize(mw_cs)
patch_file(mw_cs, [
    ('''    public override void _Ready()''',
     '''    private Button _closeButton;
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

    public override void _Ready()''',
     'G1 magnifier mobile fields/controls'),

    ('''        Vector2 vector = DisplayServer.MouseGetPosition();
        base.Position = (Vector2I)(vector - base.Size / 2);''',
     '''        Vector2 vector = DisplayServer.MouseGetPosition();
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
        }''',
     'G2 magnifier mobile follow'),
])

# ================================================================ DR_GameHandler.cs
dr_cs = f'{SRC}/Scripts/Minigames/DinoRunner/DR_GameHandler.cs'
normalize(dr_cs)
patch_file(dr_cs, [
    ('''    private bool IsMouseOver()
    {
        return new Rect2(base.Position, base.Size).HasPoint(DisplayServer.MouseGetPosition());
    }''',
     '''    private bool IsMouseOver()
    {
        if (Main._isMobile)
        {
            return true;
        }
        return new Rect2(base.Position, base.Size).HasPoint(DisplayServer.MouseGetPosition());
    }''',
     'D1 dino IsMouseOver mobile'),
])

# ================================================================ sync to proj
if os.path.exists(f'{PROJ}/Scripts'):
    shutil.rmtree(f'{PROJ}/Scripts')
shutil.copytree(f'{SRC}/Scripts', f'{PROJ}/Scripts')
print('proj/Scripts synced')

print('\nAll v11 patches applied.')
