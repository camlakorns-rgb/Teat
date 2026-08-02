# HANDOFF6 — Byte Desktop Pet → Android APK (Session 6): v11 BUGFIX PASS

**Date:** 2026-08-03
**Previous:** v10 (`Byte-Launcher-v10-mobileui.apk`) added the full touch port. User report on v10:
- terminal ("thermal") can only be entered ONCE, then locked out
- menu only "sort of works"
- sit works ✓
- lock/away unclear — **no items spawn at all**
- zoom buggy — **can't unzoom**
- (items not tested — none spawned)

**Repo:** https://github.com/camlakorns-rgb/Teat · **v11:** `Byte-Launcher-v11-bugfix.apk` (110 MB, signed, same keystore → `pm install -r` upgrade)

---

## Root causes found (by reading the scripts again, as requested)

### 1. Terminal lockout + menu "sort of works" → STUCK INPUT ACTIONS
v10's MENU/TERM buttons were **hold-buttons**. The bar auto-hides while the terminal/pause menu is open — so the finger's `ButtonUp` was **lost while the button was invisible**, leaving `Terminal` / `PauseGame` actions *pressed forever*. `Input.IsActionJustPressed()` needs a press→release edge, so after closing the terminal the next press never registered → **"enter once, locked out"**.

**Fix:** all bar buttons are now **tap-based** (press + one-frame-deferred release) — every action they trigger (`PauseGame`, `Terminal`, `Screen_Lock`, `Magnifier`, `Sit`, `Clothing_*`) is `IsActionJustPressed`-driven, so a tap is the exact edge. Plus a safety net: `MobileUI._Process` force-releases every managed action whenever the bar hides.

### 2. Can't close the terminal → **CLOSE button added**
The desktop closes the terminal with the Pause key; on mobile the bar is hidden and there was no other way out. **`TerminalWindow` now has a CLOSE button (top-right)** wired to `OnClose()` — exit and re-enter freely.

### 3. Zoom buggy / can't unzoom → magnifier reworked
- The magnifier followed `DisplayServer.MouseGetPosition()` — **no mouse on Android** → it sat at the top-left corner, static, magnifying nothing useful.
- The magnifier window covers the screen, so the ZOOM bar button was unreachable → **no way to close it**.
- **Fix:** `MagnifierWindow` tracks touch itself (`_Input`), follows the last touch (clamped on-screen), and gets a **CLOSE** button (top-right) + **+ / − zoom buttons** (bottom-right). Close → `Main.CloseMagnifier()` (new, resets `_magnifierActive` so the toggle stays consistent).

### 4. No items spawning → two real bugs
- **Spawn position:** desktop spawns items at `y = usableRect.Y - spawnMargin.Y` (above the top of the screen, they "fall in"). On Android usableRect.Y = 0, and `spawnMargin` from the scene = `(256, 0)` → items spawn at **y=0**, which should be fine… but the desktop code path has extra failure modes, so v11 **spawns items at the top edge explicitly on mobile** (clamped x/y, always on-screen) and keeps the desktop path untouched.
- **No self-healing:** if the first `OnSpawnerTimeout` hit any missing-resource state (slow pack load on mobile), the exception silently killed that spawn and the retry was 30–120 s away.
- **Fix:** `_Ready` on mobile **starts both spawner timers explicitly** (4 s / 20 s) if the scene's autostart didn't; `OnSpawnerTimeout` guards for an empty `ITEM` resource table and **retries in 5 s** with a logcat message instead of silently dying.

### 5. Item drag was actually dead in v10 (would have been the next report)
v10 routed item touches through `Main._Input`, but **touches that land on an embedded Window are delivered to that window's viewport — `Main._Input` never sees them.** So `_mobileTouchTarget = Item` never happened.
**Fix:** `ItemWindow` now handles its own touch input (`ItemWindow._Input`): touch-down on the item → grab (with the same pickup-dialogue chance as desktop); drag → move (clamped); release → **throw on flick** (velocity > 260) / **use on Byte** (`UseOnMainActor`) / **use on NPC** (`UseOnOtherActor`) / **combine** (`CombineItem`). Coordinate-space safe: movement uses grab-point deltas only.

### 6. Tap-NPC was also dead (same window-routing reason)
**Fix:** `ActorWindow._Input` on mobile: touch-down on an NPC → press `Pet` + set `_lastTouchPos` to the actor's center (so `PopActor`'s hit-test passes) → **enemies pop on tap**, exactly like desktop click. Companions keep desktop behavior (Despawn-only).

### 7. AWAY (Despawn) did nothing while Byte is visible
Desktop's `Despawn` handler only exists in the **hidden** state (it dismisses override/transform attachments). So the AWAY button appeared dead.
**Fix:** `Main.ToggleDespawnMobile()` — tap AWAY while Byte is visible → **hides her** (with dialogue + save); tap again → desktop behavior restores her. Long-press AWAY still moves all items to her.

### 8. Dino Runner never started
`IsMouseOver()` checked `DisplayServer.MouseGetPosition()` (no mouse on Android) → the first JUMP couldn't start the game.
**Fix:** `IsMouseOver()` returns `true` on mobile (the JUMP button already sits inside the game window).

---

## Verification (all passed)
- `dotnet build` 0 errors; marker `V11_BUGFIX_BUILD` **confirmed in BOTH** the loose DLL and the PCK DLL inside the shipped APK
- `apksigner` v2+v3 OK; `zipalign -c 4` OK; `unzip -t` OK; `project.binary` md5 `a33cb589…` (known-good)
- Merged PCK (66 MB, 3517 files) loads in Godot 4.6.2: `load_resource_pack` OK, `Scenes/Main.tscn` → PackedScene OK
- 109.9 MB (same slim layout as v9/v10)

## Install (Pixel 7)
```bash
adb push Byte-Launcher-v11-bugfix.apk /sdcard/Download/
su -c "cp /sdcard/Download/Byte-Launcher-v11-bugfix.apk /data/local/tmp/byte.apk"
su -c "pm install -r /data/local/tmp/byte.apk"
logcat -d | grep -iE "godot|mono|fatal|Spawner" | tail -60
```

## What to test on v11
1. **Items**: within ~5–30 s an item should drop from the top edge. Drag it around; flick to throw; drop on Byte (use), drop on another item (combine), drop on an NPC.
2. **Terminal**: TERM → use it → CLOSE → TERM again (repeatable, no lockout). SEND works.
3. **Menu**: MENU → pause → Resume → MENU again.
4. **Zoom**: ZOOM → magnifier follows your finger → +/− zoom → CLOSE.
5. **NPCs**: when an enemy spawns, tap it → pops. Companions follow Byte.
6. **AWAY**: tap hides Byte (bubble says goodbye), tap again brings her back; long-press = items to her.
7. **LOCK**: tap → lock dialogue (no visible movement on a single screen, but dialogue confirms).
8. If items STILL don't spawn, `logcat | grep Spawner` will now say why (resource guard message) — send it over.

## Changed files (patched/Scripts, all v11 state)
`Main.cs` (timers, spawn guard, mobile spawn pos, CloseMagnifier, ToggleDespawnMobile) · `MobileUI.cs` (tap buttons + release-on-hide) · `ItemWindow.cs` (own touch handling) · `ActorWindow.cs` (tap→Pet) · `TerminalWindow.cs` (CLOSE) · `MagnifierWindow.cs` (touch-follow, CLOSE, ±) · `DR_GameHandler.cs` (IsMouseOver) — plus build/ `make_project.py` (now overlays current patched state, no re-patching) and `v11_patch.py` (new).

## Known remaining gaps (v12 candidates)
- Snake is GDScript-driven; D-pad injects `ui_*` + raw key events — verify on device.
- Pause/settings/gallery submenus: confirm all buttons tappable (emulated mouse); if any are small, direct touch wiring.
- Terminal `ask_mode`/adventure text entry relies on the soft keyboard.
- If items still don't spawn with the guard message showing, the next step is dumping `ResourceCache.resourcesLoaded` contents at runtime.
