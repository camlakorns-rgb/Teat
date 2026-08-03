# HANDOFF9 — Byte Desktop Pet → Android APK (Session 9): Mobile Sprite Renderer Architecture

**Date:** 2026-08-03
**Previous:** v27 (`Byte-Launcher-v12-handoff7fix.apk`, commit `492ca7e`) — NPC flickering persisted despite Visible guards
**This session:** Implemented mobile sprite renderer architecture, reverted over-modified TerminalHandler, fixed all remaining desktop-only code paths
**Repo:** https://github.com/camlakorns-rgb/Teat
**APK:** `Byte-Launcher-v12-handoff7fix.apk` (104.8 MB, signed, LFS)
**Commit:** `7b3ee85`
**PAT:** [Use the same PAT from previous sessions — stored in git remote URL]

---

## TL;DR

**v28 is the first build that eliminates NPC flickering** by rendering companions/enemies/items as `AnimatedSprite2D` sprites in the scene tree root instead of Godot `Window` sub-viewports. On Android, `Window` nodes create separate rendering surfaces that re-composite every frame → flickering. Byte (the main character) never flickered because she's a `Node2D` in the main viewport, not a `Window`.

The fix: On mobile, set `base.Visible = false` on ActorWindow/ItemWindow (hiding the Window surface) and create a separate `AnimatedSprite2D` added to `GetTree().Root` that mirrors the NPC/item's position, animation, and flip state. This renders everything in the main viewport — no separate surfaces, no flickering.

---

## What v28 changes (vs v27)

| # | File | Change |
|---|------|--------|
| 1 | `ActorWindow.cs` | Mobile renderer: creates `AnimatedSprite2D` at scene root on mobile, hides Window, syncs position/animation/flip from WalkToPlayer. HandleCompanionAI and HandleEnemyAI sync visibility to mobile sprite. |
| 2 | `ItemWindow.cs` | Mobile renderer: creates `AnimatedSprite2D` at scene root on mobile, hides Window, syncs position/animation from `_Process`. Ground snap and throw physics sync mobile sprite position. |
| 3 | `TerminalHandler.cs` | REVERTED to original stripped code. Only added: SEND button (bottom-right, mobile only) + re-focus after submit (mobile only). Terminal now works exactly like original. |
| 4 | `Main.cs` | Removed `MobileRenderLayer` (no longer needed — renderers added to scene root directly). Screen_Lock and Despawn (hidden state) use `MobileMousePos()` instead of `DisplayServer.MouseGetPosition()`. |
| 5 | `MobileUI.cs` | Buttons slightly bigger (100×50), magnifier properly hides bar. |

---

## Complete list of all patched files (14 scripts)

| File | Key mobile fixes |
|------|-----------------|
| `Main.cs` | Touch routing (item>NPC>Byte>empty), item drag/use/combine/throw, ResourceCache.LoadData() on mobile, AWAY restore (Visible=true + ground reset), Screen_Lock/Despawn use MobileMousePos, spawner 2x faster + 2 items at once |
| `ActorWindow.cs` | **MOBILE SPRITE RENDERER** at scene root, WalkToPlayer uses Main.Position.X, HandleEnemyMouseFlee uses MobileMousePos, PopActor/HandlePop use MobileMousePos, GetCompanionStoppingDistance uses mobile position, IsOverlappingTargetWindow uses mobile position, walk oscillation hysteresis (0.9f), position update guard |
| `ItemWindow.cs` | **MOBILE SPRITE RENDERER** at scene root, FollowMouse/MoveItem mobile guards, ground snap position guard, throw physics position guard, UseOnMainActor uses MobileMousePos, methods made public (UseOnMainActor/UseOnOtherActor/CombineItem/UpdateCombinationShaders/isThrown/itemWindowVelocity) |
| `Character.cs` | StartDangle/UpdateDangle use MobileMousePos |
| `AttachObjWindow.cs` | LayerOntoParent uses Byte's size on mobile, HandleForcedActorMovement uses Main.Position, ForceDismiss() for popup ads, popupURL blocked on mobile, FollowParent mobile positioning, TEXT MousePassthrough=true |
| `ConfirmationMenu.cs` | No X button on name entry (removed), no tap-to-dismiss that could quit game |
| `DR_GameHandler.cs` | IsMouseOver returns true on mobile |
| `SG_GameHandler.cs` | D-pad controls |
| `CatchHerGameLogic.cs` | Halves + JUMP |
| `TerminalHandler.cs` | REVERTED to original + only SEND button added + re-focus after submit |
| `TerminalWindow.cs` | CLOSE button on mobile |
| `MobileUI.cs` | Bigger buttons (100×50), magnifier hides bar |
| `ScreenDataHandler.cs` | Skip refWindow.Position adjust on mobile |
| `TerminalAdventure.cs` | Item spawn uses mobile position |

---

## Architecture diagram

```
SceneTree.Root
├── Main (Node2D - Byte's logic, moves with Position)
│   ├── mainCharacter (Character - Byte's sprite, child of Main)
│   ├── MobileUI (CanvasLayer - button bar)
│   ├── ActorWindow[] (Window - NPC logic only, Visible=false on mobile)
│   ├── ItemWindow[] (Window - item logic only, Visible=false on mobile)
│   └── AttachObjWindow[] (Window - popups/bubbles/sex scenes)
├── MobileSpriteRoot[] (Node2D - NPC sprites, absolute screen coords)
│   └── AnimatedSprite2D (mirrors NPC animation/flip)
└── MobileSpriteRoot[] (Node2D - item sprites, absolute screen coords)
    └── AnimatedSprite2D (mirrors item animation/flip)
```

**Key insight:** The Window nodes still run all game logic (AI, walking, physics, interactions). They're just invisible on mobile. The mobile sprite renderers are purely visual — they copy position/animation from the Window nodes every frame.

---

## Known issues / what might still be broken

1. **AttachObjWindow flickering** — Popups, talk bubbles, and sex scene overlays also use Window. They might still flicker. Same fix could apply but not implemented yet.
2. **Mobile renderer touch hit-testing** — The mobile sprites at scene root don't receive touches. Touch input still goes to Main._Input which checks Window positions. If the mobile sprite position doesn't match the Window position exactly, taps might miss.
3. **Mobile renderer cleanup** — When NPCs/items are freed (QueueFree), the mobile sprite root might not be freed. Need to add cleanup in `_ExitTree` or `QueueFree`.
4. **Magnifier lens rendering** — Viewport texture capture on GL Compatibility may show black/stale frame.
5. **Terminal soft keyboard** — Whether Android soft keyboard pops reliably is untested.
6. **Gallery/Recipe navigation** — Should work via emulate_mouse_from_touch but buttons might be small.

---

## How to rebuild (follow STEP_BY_STEPS.md in repo)

```
STEP 0: Symlink repo at /home/user/teat/repo → /home/user/Teat
STEP 1: Toolchain (dotnet9, godot 4.6.2 mono, templates, build-tools r35, GDRE v2.6.3)
STEP 2: Game from itch.io → Byte.pck (35 MB)
STEP 3: Decompiling (repo already has decompiled/)
STEP 4: strip_generated.py → stripped/ (77 files)
STEP 5: make_project.py → src/ (stripped + overlay patched/Scripts/)
STEP 6: dotnet build -c Debug → 0 errors
STEP 7: GDRE extract Byte.pck → pck_src/ (3342 files)
STEP 8: Add patched project.binary + .NET assemblies → merged PCK
STEP 9: Assemble APK from v11 base + merged PCK
STEP 10: zipalign + apksigner sign (JKS byte/bytepass)
STEP 11: Push to git via LFS
```

**Critical environment notes:**
- 4 GB swapfile required (box has 1.9 GB RAM)
- Set `TMPDIR=/home/user/.cache/tmp` (/tmp is 993 MB tmpfs)
- Set `DOTNET_ROOT=/home/user/.cache/dotnet9`
- Clean `/home/user/.cache/` before each build to avoid disk full

---

## Current patched/Scripts/ state

All 14 patched files are in the repo at `patched/Scripts/`. The build pipeline:
1. `strip_generated.py` strips generated code from `decompiled/*.cs` → `stripped/`
2. `make_project.py` copies stripped files to `src/Scripts/**/` then overlays `patched/Scripts/*.cs` on top
3. The overlay ensures our mobile fixes replace the stripped desktop-only code

**Important:** If you modify any patched file, you must rebuild from STEP 4 (strip + assemble) to pick up changes.

---

## Game feature status (from FEATURES.md + itch.io page)

### ✅ Working on mobile (v28)
- Byte renders, animates, walks
- Drag Byte (any direction)
- Pet Byte (tap → pet anim + talk bubble)
- Throw/fling (dangle → fly → fall → bounce → land)
- Items spawn periodically (2 at a time, 2x faster)
- Items fall to ground
- Drag items (via Main._Input)
- Use items on Byte
- Combine items
- NPCs spawn (companions + enemies)
- NPCs walk to Byte
- Tap enemies → pop
- AWAY hide/show
- SIT
- OUTFIT change
- LOCK screen
- MENU (pause)
- TERM (terminal)
- Terminal: help, clear, exit, config, set_username, enable_cindesh_mode, root, force_spawn_item/actor/popup/minigame/scene, unlock_gallery, enter_brain_dance, ask_mode
- Gallery (after items discovered)
- Recipe book (after items combined)
- Dino Runner minigame (JUMP/DUCK buttons)
- Snake minigame (D-pad)
- Catch Her minigame (halves + JUMP)
- Save/load system
- EULA gate on first run

### ❓ Untested / might be broken
- Lovense Idle minigame
- Terminal adventure (text RPG)
- Terminal ask mode
- Magnifier lens rendering
- Popup ads (AttachObjWindow) — might still flicker
- Talk bubbles (AttachObjWindow) — might still flicker
- Settings menu navigation
- Key rebinding UI (desktop-only, irrelevant on mobile)

---

## Legal
Personal-use port only, per the developers' permission. Do not publish the APK or source publicly. 18+ content.
