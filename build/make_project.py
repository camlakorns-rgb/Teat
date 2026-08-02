#!/usr/bin/env python3
"""v9 build: assemble proj/ from stripped sources + v8 patched files + v9 mobile fixes."""
import os, shutil, sys

ROOT = '/home/user/.cache/v9'
STRIPPED = f'{ROOT}/stripped'
PATCHED_V8 = '/home/user/.cache/teat/patched/Scripts'
SRC = f'{ROOT}/src'
PROJ = f'{ROOT}/proj'

# res://Scripts subfolder mapping (from the v8 APK assets/Scripts listing)
MAPPING = {
    'AttachmentObject.cs': 'AttachmentScripts', 'AttachObjWindow.cs': 'AttachmentScripts',
    'AttachSpriteController.cs': 'AttachmentScripts', 'AttachTextController.cs': 'AttachmentScripts',
    'ResourceCache.cs': 'Cache',
    'ActorCharacter.cs': 'CharacterScripts', 'ActorSpriteController.cs': 'CharacterScripts',
    'ActorWindow.cs': 'CharacterScripts', 'AnimDataRes.cs': 'CharacterScripts',
    'Character.cs': 'CharacterScripts', 'SpriteParentController.cs': 'CharacterScripts',
    'AiItemDataRes.cs': 'DataResources', 'AttachDataRes.cs': 'DataResources',
    'CharacterInfoDataRes.cs': 'DataResources', 'CharAnimDataRes.cs': 'DataResources',
    'CombinationDataRes.cs': 'DataResources', 'ConvoDataRes.cs': 'DataResources',
    'DialogueDataRes.cs': 'DataResources', 'GalleryDataRes.cs': 'DataResources',
    'ItemDataRes.cs': 'DataResources', 'ScreenDataHandler.cs': 'DataResources',
    'TagDataRes.cs': 'DataResources', 'TagOverrideDataRes.cs': 'DataResources',
    'TerminalEEDataRes.cs': 'DataResources',
    'SignalEventBus.cs': 'Globals',
    'ItemObjectHandler.cs': 'ItemScripts', 'ItemSpriteController.cs': 'ItemScripts',
    'ItemWindow.cs': 'ItemScripts',
    'CatchHerGameLogic.cs': 'Minigames/CatchHer',
    'DR_Dino.cs': 'Minigames/DinoRunner', 'DR_GameHandler.cs': 'Minigames/DinoRunner',
    'DR_GroundHandler.cs': 'Minigames/DinoRunner', 'DR_PropDataRes.cs': 'Minigames/DinoRunner',
    'DR_PropHandler.cs': 'Minigames/DinoRunner', 'DR_Spawner.cs': 'Minigames/DinoRunner',
    'FloatingText.cs': 'Minigames/Helpers',
    'LI_Clicker.cs': 'Minigames/LovenseIdle', 'LI_GameHandler.cs': 'Minigames/LovenseIdle',
    'SG_GameHandler.cs': 'Minigames/SnakeGame',
    'MinigameBase.cs': 'Minigames',
    'SaveHandler.cs': 'SaveAndLoad',
    'ConfirmationMenu.cs': 'SubMenus/ConfirmationMenu',
    'GalleryDisplay.cs': 'SubMenus/GalleryMenu', 'GalleryHandler.cs': 'SubMenus/GalleryMenu',
    'GalleryItem.cs': 'SubMenus/GalleryMenu', 'GalleryPieceDataRes.cs': 'SubMenus/GalleryMenu',
    'GalleryPieceHandler.cs': 'SubMenus/GalleryMenu', 'GalleryWindow.cs': 'SubMenus/GalleryMenu',
    'PieceDisplayHandler.cs': 'SubMenus/GalleryMenu', 'URLLinker.cs': 'SubMenus/GalleryMenu',
    'LoadingPopup.cs': 'SubMenus/LoadingPopup',
    'MagnifierWindow.cs': 'SubMenus/MagnifierMenu',
    'NineSplitButton.cs': 'SubMenus/PauseMenu', 'PauseMenu.cs': 'SubMenus/PauseMenu',
    'CombinationUIHandler.cs': 'SubMenus/RecipeBook', 'RecipeMenuHandler.cs': 'SubMenus/RecipeBook',
    'SettingsMenu.cs': 'SubMenus/SettingsMenu',
    'TA_ExitDataRes.cs': 'SubMenus/TerminalMenu/TerminalAdventure',
    'TA_ItemDataRes.cs': 'SubMenus/TerminalMenu/TerminalAdventure',
    'TA_NPCDataRes.cs': 'SubMenus/TerminalMenu/TerminalAdventure',
    'TA_RoomDataRes.cs': 'SubMenus/TerminalMenu/TerminalAdventure',
    'TA_TopicDataRes.cs': 'SubMenus/TerminalMenu/TerminalAdventure',
    'TA_TradeDataRes.cs': 'SubMenus/TerminalMenu/TerminalAdventure',
    'TA_WorldDataRes.cs': 'SubMenus/TerminalMenu/TerminalAdventure',
    'TerminalAdventure.cs': 'SubMenus/TerminalMenu/TerminalAdventure',
    'TAsk_AskDataRes.cs': 'SubMenus/TerminalMenu/TerminalAsk',
    'TAsk_EntryDataRes.cs': 'SubMenus/TerminalMenu/TerminalAsk',
    'TerminalAsk.cs': 'SubMenus/TerminalMenu/TerminalAsk',
    'TerminalHandler.cs': 'SubMenus/TerminalMenu', 'TerminalWindow.cs': 'SubMenus/TerminalMenu',
    'DEBUG_SimSpawnItems.cs': 'Tool', 'TimeSpinBox.cs': 'Tool',
    'Main.cs': '.', 'ModManifest.cs': '.', 'PowerThrottling.cs': '.', 'WeightGroup.cs': '.',
}

def fail(msg):
    print(f'FATAL: {msg}')
    sys.exit(1)

def normalize(path):
    """Convert leading tabs to 4 spaces (repo patched files mix tabs/spaces)."""
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
            fail(f'[{tag}] pattern not found in {path}:\n{old[:200]}')
        if src.count(old) != 1:
            fail(f'[{tag}] pattern matches {src.count(old)}x in {path}')
        src = src.replace(old, new)
        print(f'  ✓ {tag} ({path.split("/")[-1]})')
    with open(path, 'w', encoding='utf-8') as f:
        f.write(src)

# ---------------------------------------------------------------- assemble tree
if os.path.exists(SRC):
    shutil.rmtree(SRC)
for fn, sub in MAPPING.items():
    dst = os.path.join(SRC, 'Scripts', sub, fn)
    os.makedirs(os.path.dirname(dst), exist_ok=True)
    shutil.copy(os.path.join(STRIPPED, fn), dst)

# overlay v8 patched files (they include prior mobile fixes)
shutil.copy(f'{PATCHED_V8}/Main.cs', f'{SRC}/Scripts/Main.cs')
shutil.copy(f'{PATCHED_V8}/AttachObjWindow.cs', f'{SRC}/Scripts/AttachmentScripts/AttachObjWindow.cs')

# ---------------------------------------------------------------- v9 patches: Main.cs
main_cs = f'{SRC}/Scripts/Main.cs'
normalize(main_cs)
patch_file(main_cs, [
    # P1: fix infinite recursion in MobileMousePos
    ('''    public Vector2I MobileMousePos()
    {
        if (_isMobile)
        {
            if (_lastTouchPos != Vector2I.Zero)
                return _lastTouchPos;
        }
        return MobileMousePos();
    }''',
     '''    public Vector2I MobileMousePos()
    {
        if (_isMobile)
        {
            if (_lastTouchPos != Vector2I.Zero)
                return _lastTouchPos;
        }
        return DisplayServer.MouseGetPosition();
    }''',
     'P1 MobileMousePos recursion fix'),

    # P2: FollowMouse mobile branch (move Node2D Position, not window)
    ('''    private void FollowMouse(bool Unlocked = false)
    {
        Vector2I mousePos = MobileMousePos();
        if (!Unlocked)
        {
            mainWindow.Position = new Vector2I(screenDataHandler.ClampAcrossAllScreensX((int)((float)mousePos.X + mouseOffset.X), mainCharacter.trueSize.X), screenDataHandler.taskbarPos);
            return;
        }''',
     '''    private void FollowMouse(bool Unlocked = false)
    {
        Vector2I mousePos = MobileMousePos();
        if (_isMobile)
        {
            Vector2I screenSize = DisplayServer.ScreenGetSize(screenDataHandler.screenIndex);
            Vector2 newPos = (Vector2)mousePos + mouseOffset;
            newPos.X = Mathf.Clamp(newPos.X, 0f, (float)(screenSize.X - mainCharacter.trueSize.X));
            newPos.Y = Mathf.Clamp(newPos.Y, 0f, (float)(screenSize.Y - mainCharacter.trueSize.Y));
            Position = newPos;
            return;
        }
        if (!Unlocked)
        {
            mainWindow.Position = new Vector2I(screenDataHandler.ClampAcrossAllScreensX((int)((float)mousePos.X + mouseOffset.X), mainCharacter.trueSize.X), screenDataHandler.taskbarPos);
            return;
        }''',
     'P2 FollowMouse mobile branch'),

    # P3: Walk mobile branch (use Position instead of mainWindow.Position)
    ('''    private void Walk(double delta)
    {
        Vector2I position = mainWindow.Position;''',
     '''    private void Walk(double delta)
    {
        Vector2I position = (_isMobile) ? (Vector2I)Position : mainWindow.Position;''',
     'P3a Walk read Position'),

    ('''            position.X = num;
            mainWindow.Position = new Vector2I(screenDataHandler.ClampAcrossAllScreensX(position.X, mainCharacter.trueSize.X), screenDataHandler.ClampAcrossAllScreensY(position.Y));
        }
        else
        {
            int effectiveLeftX = screenDataHandler.EffectiveLeftX;''',
     '''            position.X = num;
            if (_isMobile) Position = new Vector2(position.X, Position.Y);
            else mainWindow.Position = new Vector2I(screenDataHandler.ClampAcrossAllScreensX(position.X, mainCharacter.trueSize.X), screenDataHandler.ClampAcrossAllScreensY(position.Y));
        }
        else
        {
            int effectiveLeftX = screenDataHandler.EffectiveLeftX;''',
     'P3b Walk write Position (target)'),

    ('''            position.X = num3;
            mainWindow.Position = new Vector2I(screenDataHandler.ClampAcrossAllScreensX(position.X, mainCharacter.trueSize.X), screenDataHandler.ClampAcrossAllScreensY(position.Y));
        }
    }''',
     '''            position.X = num3;
            if (_isMobile) Position = new Vector2(position.X, Position.Y);
            else mainWindow.Position = new Vector2I(screenDataHandler.ClampAcrossAllScreensX(position.X, mainCharacter.trueSize.X), screenDataHandler.ClampAcrossAllScreensY(position.Y));
        }
    }''',
     'P3c Walk write Position (wander)'),

    # P4: throw / gravity / ground snap mobile branch
    ('''        else if (isThrown)
        {
            windowVelocity.Y += 1400f * (float)delta;
            windowVelocity *= Mathf.Pow(0.995f, (float)delta * windowAirResist);
            Vector2I position = new Vector2I(mainWindow.Position.X + Mathf.RoundToInt(windowVelocity.X * (float)delta), mainWindow.Position.Y + Mathf.RoundToInt(windowVelocity.Y * (float)delta));
            int num = screenDataHandler.ClampAcrossAllScreensX(position.X, mainCharacter.trueSize.X);
            if (num != position.X)
            {
                windowVelocity.X *= windowBounceDamping;
                position.X = num;
                mainCharacter.ApplyBounceRotation(windowVelocity.X);
            }
            if (position.Y >= screenDataHandler.taskbarPos)
            {
                position.Y = screenDataHandler.taskbarPos;
                isThrown = false;
                mainWindow.Position = position;
                mainCharacter.BeginLand();
                windowVelocity = Vector2.Zero;
                mainCharacter.CancelThrowRotation();
                CheckLandingInteraction(mainCharacter.throwWasHard);
            }
            mainWindow.Position = position;
            mainCharacter.UpdateThrowRotation(delta);
        }
        else if (mainWindow.Position.Y < screenDataHandler.taskbarPos)
        {
            mainWindow.Position += new Vector2I(0, Mathf.RoundToInt(980f * (float)delta));
        }
        else
        {
            mainWindow.Position = new Vector2I(mainWindow.Position.X, screenDataHandler.taskbarPos);
        }''',
     '''        else if (isThrown)
        {
            windowVelocity.Y += 1400f * (float)delta;
            windowVelocity *= Mathf.Pow(0.995f, (float)delta * windowAirResist);
            if (_isMobile)
            {
                Vector2I screenSize = DisplayServer.ScreenGetSize(screenDataHandler.screenIndex);
                int groundY = screenSize.Y - mainCharacter.trueSize.Y;
                Vector2 pos = Position + windowVelocity * (float)delta;
                if (pos.X < 0f)
                {
                    pos.X = 0f;
                    windowVelocity.X *= windowBounceDamping;
                    mainCharacter.ApplyBounceRotation(windowVelocity.X);
                }
                else if (pos.X > (float)(screenSize.X - mainCharacter.trueSize.X))
                {
                    pos.X = screenSize.X - mainCharacter.trueSize.X;
                    windowVelocity.X *= windowBounceDamping;
                    mainCharacter.ApplyBounceRotation(windowVelocity.X);
                }
                if (pos.Y >= (float)groundY)
                {
                    pos.Y = groundY;
                    isThrown = false;
                    windowVelocity = Vector2.Zero;
                    mainCharacter.CancelThrowRotation();
                    mainCharacter.BeginLand();
                    CheckLandingInteraction(mainCharacter.throwWasHard);
                }
                Position = pos;
                mainCharacter.UpdateThrowRotation(delta);
            }
            else
            {
                Vector2I position = new Vector2I(mainWindow.Position.X + Mathf.RoundToInt(windowVelocity.X * (float)delta), mainWindow.Position.Y + Mathf.RoundToInt(windowVelocity.Y * (float)delta));
                int num = screenDataHandler.ClampAcrossAllScreensX(position.X, mainCharacter.trueSize.X);
                if (num != position.X)
                {
                    windowVelocity.X *= windowBounceDamping;
                    position.X = num;
                    mainCharacter.ApplyBounceRotation(windowVelocity.X);
                }
                if (position.Y >= screenDataHandler.taskbarPos)
                {
                    position.Y = screenDataHandler.taskbarPos;
                    isThrown = false;
                    mainWindow.Position = position;
                    mainCharacter.BeginLand();
                    windowVelocity = Vector2.Zero;
                    mainCharacter.CancelThrowRotation();
                    CheckLandingInteraction(mainCharacter.throwWasHard);
                }
                mainWindow.Position = position;
                mainCharacter.UpdateThrowRotation(delta);
            }
        }
        else if (_isMobile)
        {
            Vector2I screenSize = DisplayServer.ScreenGetSize(screenDataHandler.screenIndex);
            int groundY = screenSize.Y - mainCharacter.trueSize.Y;
            if (Position.Y < (float)groundY)
            {
                Position += new Vector2(0f, 980f * (float)delta);
                if (Position.Y > (float)groundY)
                {
                    Position = new Vector2(Position.X, groundY);
                }
            }
            else
            {
                Position = new Vector2(Position.X, groundY);
            }
        }
        else
        {
            if (mainWindow.Position.Y < screenDataHandler.taskbarPos)
            {
                mainWindow.Position += new Vector2I(0, Mathf.RoundToInt(980f * (float)delta));
            }
            else
            {
                mainWindow.Position = new Vector2I(mainWindow.Position.X, screenDataHandler.taskbarPos);
            }
        }''',
     'P4 throw/gravity mobile branch'),

    # P5: GetThinnerCollisionBox full width on mobile (33% -> 100%)
    ('''        float num = 0.33f;
        if (_isMobile)
        {
            int w = Mathf.RoundToInt((float)mainCharacter.trueSize.X * num);
            int x = Mathf.RoundToInt(Position.X + (mainCharacter.trueSize.X - w) / 2f);
            int y = Mathf.RoundToInt(Position.Y);
            return new Rect2I(new Vector2I(x, y), new Vector2I(w, mainCharacter.trueSize.Y));
        }''',
     '''        float num = 0.33f;
        if (_isMobile)
        {
            int w = mainCharacter.trueSize.X;
            int x = Mathf.RoundToInt(Position.X);
            int y = Mathf.RoundToInt(Position.Y);
            return new Rect2I(new Vector2I(x, y), new Vector2I(w, mainCharacter.trueSize.Y));
        }''',
     'P5 GetThinnerCollisionBox full width'),
])

# ---------------------------------------------------------------- v9 patches: Character.cs
char_cs = f'{SRC}/Scripts/CharacterScripts/Character.cs'
normalize(char_cs)
patch_file(char_cs, [
    ('''        lastMousePos = DisplayServer.MouseGetPosition();''',
     '''        lastMousePos = Main.Instance.MobileMousePos();''',
     'C1 StartDangle MobileMousePos'),
    ('''            Vector2 vector = DisplayServer.MouseGetPosition();''',
     '''            Vector2 vector = Main.Instance.MobileMousePos();''',
     'C2 UpdateDangle MobileMousePos'),
])

# ---------------------------------------------------------------- v9 patches: AttachObjWindow.cs
att_cs = f'{SRC}/Scripts/AttachmentScripts/AttachObjWindow.cs'
normalize(att_cs)
with open(att_cs, encoding='utf-8') as f:
    att = f.read()

# A1: bubble (TEXT) MousePassthrough on mobile - patch the TEXT case in SetupAttachmentWindow
old_text_case = '''        case AttachDataRes.AttachmentType.TEXT:
            attachObject.SetupTextAttachment(passedText);
            break;'''
new_text_case = '''        case AttachDataRes.AttachmentType.TEXT:
            attachObject.SetupTextAttachment(passedText);
            if (Main._isMobile)
            {
                base.MousePassthrough = true;
            }
            break;'''
if old_text_case not in att:
    fail('A1 TEXT case anchor not found')
att = att.replace(old_text_case, new_text_case, 1)
# A2: FollowParent mobile branch (position above Main's head)
old_follow = '''        Vector2 attachmentMargin = attachObject.attachedItemInformation.attachmentMargin;
        Vector2I size = parentWindow.Size;
        Vector2I trueSize = attachObject.trueSize;
        List<AttachObjWindow> list = new List<AttachObjWindow>();'''
new_follow = '''        Vector2 attachmentMargin = attachObject.attachedItemInformation.attachmentMargin;
        Vector2I size = parentWindow.Size;
        Vector2I trueSize = attachObject.trueSize;
        if (Main._isMobile && parentWindow == Main.Instance.mainWindow)
        {
            // On mobile the main window is fullscreen at (0,0); position the bubble
            // relative to Byte's actual Position, centered above her head.
            Vector2I mainPos = (Vector2I)Main.Instance.Position;
            Vector2I mainSize = Main.Instance.mainCharacter.trueSize;
            int bx = mainPos.X + mainSize.X / 2 - trueSize.X / 2 + (int)attachmentMargin.X;
            int by = mainPos.Y - trueSize.Y - 20 + (int)attachmentMargin.Y;
            if (by < 0) by = 0;
            base.Position = new Vector2I(bx, by);
            base.MousePassthrough = true;
            return;
        }
        List<AttachObjWindow> list = new List<AttachObjWindow>();'''
if old_follow not in att:
    fail('A2 FollowParent anchor not found')
att = att.replace(old_follow, new_follow, 1)
with open(att_cs, 'w', encoding='utf-8') as f:
    f.write(att)
print('  ✓ A1/A2 AttachObjWindow bubble (anchor verified)')

print('\nAll v9 patches applied.')
