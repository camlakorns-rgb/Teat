# HANDOFF5 — Byte Desktop Pet → Android APK (Session 5): FULL FEATURE PORT

**Date:** 2026-08-03
**Previous:** HANDOFF4 shipped v9 (`Byte-Launcher-v9-fullphysics.apk`) — drag/throw/walk/ground-snap/bubble all working on device.
**Repo:** https://github.com/camlakorns-rgb/Teat (LFS: `*.apk *.pck *.zip *.dll`)

---

## TL;DR

**v10 (`Byte-Launcher-v10-mobileui.apk`, 110 MB, signed, in this repo) ports the remaining game features to touch.** User approved "build all" with the button bar placed in the **top-right corner of the screen (not on Byte)**.

Feature inventory before this session: `FEATURES.md` (27 features in code, ~9 reachable on mobile). After v10, **all of them are reachable**:

### What v10 adds

**1. On-screen button bar (top-right, semi-transparent, auto-hides while pause/terminal/minigames open)** — `Scripts/MobileUI.cs` (new):
| Button | Action |
|---|---|
| MENU (hold) | PauseGame → full Pause menu (Settings / Recipe book / Gallery / Guide / Quit) |
| TERM (hold) | Terminal window (commands, Adventure RPG, Ask mode) |
| SIT (tap) | Sit / stand |
| OUTFIT (tap) | Clothing_Up (next outfit) — long-press = Clothing_Down |
| LOCK (hold) | Screen_Lock (faked touch on Byte so hit-test passes) |
| AWAY (hold) | Despawn — long-press = move all items to Byte's screen |
| ZOOM (tap) | Magnifier lens |

**2. Touch routing rewrite (`Main._Input`)** — a touch-down now targets ONE thing (topmost): **item > NPC > Byte > empty**:
- Item → item window handles grab/drag/use (below)
- NPC → synthesizes Pet (enemy NPCs pop, exactly like desktop click)
- Byte → Move + Pet (drag, as before)
- Empty → nothing; **long-press (0.5 s) on empty space opens the Pause menu**
- Touches on the button bar are excluded (`MobileUI.IsPointInUI` guard)

**3. Items fully touchable (`ItemWindow.ProcessMobileItem`)**:
- Tap & drag an item → it follows your finger (clamped to screen)
- Release with a flick (velocity > 260) → item flies, bounces, lands (same physics as Byte)
- Release over Byte → `UseOnMainActor` (food/toys/outfits…)
- Release over an NPC → `UseOnOtherActor` (AI items)
- Release over another item → `CombineItem` (recipe discovery)
- Tap-to-delete (desktop Pet-click on item) disabled on mobile to avoid accidents

**4. NPCs**: tap enemy/aggro NPC → pops (desktop behavior). Companion walk-target now uses Byte's real `Position` on mobile (they follow her instead of walking to the left edge). Pop hit-tests use `MobileMousePos()`.

**5. Byte attachments (hats/outfits)** — `AttachObjWindow.LayerOntoParent` uses `Main.Position` as the base on mobile (was window 0,0 → hats rendered at top-left).

**6. Minigames touch controls**:
- **Dino Runner**: JUMP + DUCK buttons (bottom-left) → DR_Jump/DR_Duck actions
- **Snake**: on-screen D-pad (presses `ui_*` actions AND injects raw key events via `Input.ParseInputEvent` for GDScript-driven input)
- **Catch Her**: hold left half = A, right half = D, JUMP button = Space
- **Lovense Idle**: already button-based → works via touch emulation

**7. Terminal**: SEND button (bottom-right) next to the input trap → same `HandleSubmit` path as Enter; soft keyboard pops when the terminal opens (`_inputTrap.GrabFocus()`).

### Build marker
`public static readonly string V10_BUILD = "V10_MOBILEUI_BUILD";` in Main.cs.
**Verified present (UTF-16LE) in BOTH the loose `assets/.godot/mono/publish/arm64/DesktopPets.dll` and the PCK DLL inside `Byte-Launcher-v10-mobileui.apk`.**

---

## Verification (all passed on this APK)

- `apksigner verify` → v2+v3 OK (same `byte.keystore` → `pm install -r` upgrade over v9)
- `zipalign -c 4` OK, `unzip -t` OK
- `assets/project.binary` md5 `a33cb589…` (known-good patched, main_scene `Scenes/Main.tscn`)
- Merged PCK (66 MB, 3517 files) loads in Godot 4.6.2: `load_resource_pack` OK, `load("res://Scenes/Main.tscn")` OK
- `dotnet build`: 0 errors; `V10_MOBILEUI_BUILD` in both shipped DLLs
- Size 109.9 MB (same slim layout as v9: 13 `*.a` removed, 2 big `.so` deflated, rest stored)

## Files changed (all in `patched/Scripts/`, plus new `MobileUI.cs`)

`Main.cs` (touch routing, helpers, MobileUI creation, GetThinnerCollisionBox public, RepositionAllItemsToMouseScreen public) · `ItemWindow.cs` (ProcessMobileItem) · `ActorWindow.cs` (mobile walk target + pop hit-test) · `AttachObjWindow.cs` (LayerOntoParent mobile base) · `DR_GameHandler.cs` (JUMP/DUCK) · `SG_GameHandler.cs` (D-pad) · `CatchHerGameLogic.cs` (halves + JUMP) · `TerminalHandler.cs` (SEND) · `MobileUI.cs` (new).

Build tooling in `build/`: `make_project.py`, `v10_patch.py` (new — all v10 patches, strict anchors), `strip_generated.py`, `assemble_apk.py`, `pcktool.py`.

## Install / test (Pixel 7)

```bash
adb push Byte-Launcher-v10-mobileui.apk /sdcard/Download/
su -c "cp /sdcard/Download/Byte-Launcher-v10-mobileui.apk /data/local/tmp/byte.apk"
su -c "pm install -r /data/local/tmp/byte.apk"
logcat -d | grep -iE "godot|mono|fatal" | tail -60
```

**Test checklist:**
1. Button bar visible top-right; MENU opens Pause (settings/gallery/recipes inside), TERM opens terminal (SEND button bottom-right, soft keyboard pops)
2. Drag items: pick up, drag, drop on Byte (use), drop on item (combine → recipe toast?), flick to throw
3. Tap an enemy NPC → pops; companions walk toward Byte
4. OUTFIT tap cycles clothes; long-press reverses; SIT; LOCK; ZOOM (magnifier); AWAY despawns her
5. Long-press empty area → Pause menu
6. Minigames: use an item that launches one (or terminal `root` → `force_spawn_minigame`), test JUMP/DUCK, D-pad, Catch Her halves
7. Bar auto-hides while pause/terminal/minigame open, returns after

## Known issues / notes

1. **PauseMenu closing via MENU button while paused**: PauseMenu's own Resume button works (GUI input is not paused). If the MENU button doesn't close it, that's expected desktop behavior — use Resume.
2. **Snake (WIP)**: input is GDScript-driven; we inject both `ui_*` actions and raw key events — if the GDScript uses its own custom action names, the D-pad may not map. Low priority (WIP minigame).
3. **`emulate_mouse_from_touch` still drives** buttons/menus; if any pause-menu button is hard to tap, add explicit touch handling there later.
4. **Item overlap order**: touch targets topmost = last spawned; if an item sits exactly on Byte, touching that spot grabs the item, not Byte (drag the item away first). Same as desktop stacking.
5. **Catch Her**: after game-over, tap restarts via emulated mouse click; left/right flags also set on that tap — player may move immediately (minor).
6. **Terminal SEND** uses the invisible full-rect input trap; the soft keyboard's own "enter/done" key also works if it sends Enter.

## Next steps (v11 candidates)

- On-screen mini-controls for terminal `ask_mode`/adventure (pure text — keyboard is fine)
- Settings key-rebinding UI is desktop-only (irrelevant on touch) — hide on mobile
- Test gallery/recipe menus on device; add touch close-buttons if windows' X is small
- If emulated-mouse button taps feel laggy, switch PauseMenu/Settings to direct touch signal wiring
