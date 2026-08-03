#!/usr/bin/env python3
"""v11 build step 1: assemble proj/ from stripped decompiled sources + overlay the
current repo patched/ files (which already contain all v9/v10 mobile patches)."""
import os, shutil, sys

ROOT = '/home/user/.cache/v9'
STRIPPED = f'{ROOT}/stripped'
PATCHED = '/home/user/teat/repo/patched/Scripts'
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

# files in repo/patched/Scripts (current mobile state) -> target subfolder
PATCHED_TARGETS = {
    'Main.cs': '.',
    'Character.cs': 'CharacterScripts',
    'AttachObjWindow.cs': 'AttachmentScripts',
    'ItemWindow.cs': 'ItemScripts',
    'ActorWindow.cs': 'CharacterScripts',
    'DR_GameHandler.cs': 'Minigames/DinoRunner',
    'SG_GameHandler.cs': 'Minigames/SnakeGame',
    'CatchHerGameLogic.cs': 'Minigames/CatchHer',
    'TerminalHandler.cs': 'SubMenus/TerminalMenu',
    'MobileUI.cs': '.',
    'ConfirmationMenu.cs': 'SubMenus/ConfirmationMenu',
    'ActorWindow.cs': 'CharacterScripts',
    'ScreenDataHandler.cs': 'DataResources',
    'TerminalAdventure.cs': 'SubMenus/TerminalMenu/TerminalAdventure',
}

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

# ---------------------------------------------------------------- assemble tree
if os.path.exists(SRC):
    shutil.rmtree(SRC)
for fn, sub in MAPPING.items():
    dst = os.path.join(SRC, 'Scripts', sub, fn)
    os.makedirs(os.path.dirname(dst), exist_ok=True)
    shutil.copy(os.path.join(STRIPPED, fn), dst)

# overlay current patched (v9+v10 mobile state) files
for fn, sub in PATCHED_TARGETS.items():
    dst = os.path.join(SRC, 'Scripts', sub, fn)
    os.makedirs(os.path.dirname(dst), exist_ok=True)
    shutil.copy(os.path.join(PATCHED, fn), dst)
    normalize(dst)
    print(f'overlaid {fn}')

print('\nsrc assembled (stripped + patched overlay).')
