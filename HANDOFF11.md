# HANDOFF11 — Byte Desktop Pet → Android APK (Session 11): Black Screen & Item Spawn Fix (V30)

**Date:** 2026-08-03
**Previous:** v29 (HANDOFF10, `Byte-Launcher-v12-handoff7fix.apk`, commit `51576ab`) — fixed visibility logic but introduced black screen.
**This session:** Fixed black screen (Window Visible=true overlay) and items not spawning (ResourceCache retry + mobile renderer sync). Rebuilt and signed APK.
**Repo:** https://github.com/camlakorns-rgb/Teat
**APK:** `Byte-Launcher-v30-blackscreen-fix.apk` (123 MB, signed, LFS, same keystore byte/bytepass)
**Markers:** `V30_BLACKSCREEN_ITEMSPAWN_FIX` and `V30_MOBILE_RENDERER_VISIBLE_FALSE` verified in DLL inside PCK and loose.

---

## User Report (v29 / v12 handoff7fix)

- Items are not spawning
- Now it's on a black screen

## Root Cause Analysis

### 1. Black Screen — `base.Visible = true` on Android Window nodes

HANDOFF10 fixed the `w.Visible` filter bug by setting `base.Visible = true` for active items/NPCs and hiding internal sprites. 

**Problem:** On Android, Godot `Window` nodes are **fullscreen** surfaces. If `Visible=true`, each item/NPC window covers the entire screen with its own viewport that clears to black (opaque). When several items spawn, you get a stack of black fullscreen windows covering Byte → black screen.

**V28 architecture (which worked, no black screen):**
- `ItemWindow` and `ActorWindow` `Visible=false` on mobile → no Window surface rendered
- Separate `Node2D` + `AnimatedSprite2D` added to `SceneTree.Root` mirrors position/animation/flip → renders in main viewport, no flickering, no black overlay.

**V29 regression:** Set `Visible=true` to make `IsPointOnAnyItem` checks pass → reintroduced black overlay.

**V30 fix:** Revert to `Visible=false` on mobile (no black overlay) but **make logic checks not rely on Visible**.

- `ItemWindow`: `Transparent=true`, `TransparentBg=true`, `Visible=false`, `_mobileSpriteRoot.Visible = IsActiveForMobile`
- `ActorWindow`: same, plus `isSetup` flag to track logical existence
- `AttachObjWindow` (bubbles/popups): set `Transparent` and `TransparentBg` true on mobile to avoid black background covering screen.

### 2. Items Not Spawning — Three Interacting Bugs

**A. ResourceCache.LoadData() never completing:**
- `Main._Ready` called `CallDeferred("LoadData")` only once if `Instance != null`. If autoload not ready yet, `Instance` is null → LoadData never called → `resourcesLoaded[ITEM]` stays empty → spawner prints `[Spawner] ITEM not ready - retrying` forever → no items.
- **Fix V30:** New robust `TryLoadResources()` that retries every 0.6s up to 20 times, checks if ITEM table empty, and calls LoadData each time. Also `OnSpawnerTimeout` now re-triggers LoadData if table empty.

**B. Mobile renderer desync & invisibility:**
- In V29, `_mobileSpriteRoot.Position` sync only happened in limited branches (isThrown or at end). Falling items updated `base.Position` but mobile sprite not synced every frame → invisible or stuck at (0,0).
- `_mobileSpriteRoot.Visible` logic was `base.Visible && !CurrentlyPickedUp` → false because `base.Visible` was false in V28 (and intended to be false) → mobile sprites invisible even when they should be visible.
- **Fix:** Unconditional per-frame sync in `_Process`:
  ```csharp
  _mobileSpriteRoot.Position = base.Position
  _mobileSpriteRoot.Visible = IsActiveForMobile
  // + copy SpriteFrames, Animation, Frame, Scale, Position, Flip
  ```
  And setup creates root with `Visible=true`.

**C. Logic filters using Visible:**
- `IsPointOnAnyItem`, `IsPointOnAnyActor`, `FindItemAtPoint`, `TrySetTargetToItem`, `CheckLandingInteraction` all filtered by `w.Visible`. On mobile Visible=false → items treated as non-existent for:
  - Touch hit-testing (can't drag)
  - Auto-walk targeting (Byte never walks to items)
  - Landing interaction (dropping Byte on item does nothing)
- **Fix:** Helpers:
  ```csharp
  public bool IsItemLogicallyVisible(ItemWindow w) => _isMobile ? w.IsActiveForMobile : w.Visible;
  public bool IsActorLogicallyVisible(ActorWindow w) => _isMobile ? w.IsActiveForMobile : w.Visible;
  ```
  And updated all queries to use them. Added to `ItemWindow`: `public bool IsSetup => isSetup; public bool IsActiveForMobile => isSetup && !CurrentlyPickedUp;` and to `ActorWindow`: `isSetup` flag + `IsActiveForMobile`.

### 3. Additional Fixes

- **AttachObjWindow flicker/black:** Set Transparent true on mobile, added `_ExitTree` cleanup (already existed in Item/Actor, added guard try-catch for Root add).
- **Build verification:** Marker strings present in both loose and PCK DLLs.

---

## File Changes (V30 vs V29)

| File | Change |
|------|--------|
| `patched/Scripts/Main.cs` | Added `V30_BUILD` markers, `TryLoadResources()` with retry timer, `IsItemLogicallyVisible` / `IsActorLogicallyVisible` helpers, rewrote `IsPointOnAnyItem`, `IsPointOnAnyActor`, `FindItemAtPoint`, fixed LINQ filters in `TrySetTargetToItem` and `CheckLandingInteraction`, spawner now retriggers LoadData on empty. |
| `patched/Scripts/ItemWindow.cs` | `Transparent=true`, `TransparentBg=true`, `Visible=false` on mobile, added `IsSetup` / `IsActiveForMobile` properties, unconditional sync of mobile sprite root every frame, `Visible = IsActiveForMobile` for root, hide root when picked up, try-catch on Root add. |
| `patched/Scripts/ActorWindow.cs` | Added `isSetup` flag, `IsSetup` / `IsActiveForMobile` properties, `Transparent`/`TransparentBg` + `Visible=false` on mobile, unconditional sync, fixed visibility logic, cleanup in `_ExitTree`. |
| `patched/Scripts/AttachObjWindow.cs` | `Transparent`/`TransparentBg` true on mobile in `DelayedSetupFlag`. |
| `Byte-Launcher-v30-blackscreen-fix.apk` | Rebuilt 123 MB, zipalign 4, apksigner v2+v3 OK, contains new DLL + merged PCK (64 MB, 3517 files). |

---

## Build Verification (V30)

- `dotnet build` 0 errors, 1 warning (unused field)
- Marker `V30_BLACKSCREEN_ITEMSPAWN_FIX` found UTF-16LE in:
  - `assets/.godot/mono/publish/arm64/DesktopPets.dll` (loose)
  - `assets/assets.sparsepck` → `.godot/mono/publish/arm64/DesktopPets.dll` inside PCK (verified via GDRE extract)
- `zipalign -c 4` OK
- `unzip -t` OK
- `apksigner verify` v2 true, v3 true, same keystore `byte`/`bytepass` → `pm install -r` upgrade OK
- `project.binary` md5 `a33cb5892eb650a5b26a9541ba70b9ad` (known-good: `Scenes/Main.tscn`, `GL Compatibility`, autoloads preserved)
- Size 123 MB (175 DLLs in loose folder, 64 MB PCK)

---

## Install / Test (Pixel 7)

```bash
adb push Byte-Launcher-v30-blackscreen-fix.apk /sdcard/Download/
su -c "cp /sdcard/Download/Byte-Launcher-v30-blackscreen-fix.apk /data/local/tmp/byte.apk"
su -c "pm install -r /data/local/tmp/byte.apk"
logcat -d | grep -iE "godot|mono|fatal|Spawner|ResourceCache|Mobile" | tail -80
```

### Checklist

1. **No black screen** — Byte renders on launch, background visible, not black.
2. **Items spawn** — within 4-20s, 2 items drop from top edge, fall to ground, visible as sprites.
3. **Item drag** — touch item → it follows finger (clamped), release with flick → flies/bounces/lands.
4. **Item use** — drop item on Byte → `UseOnMainActor` (food/toys work), drop on another item → `CombineItem` (recipe toast), drop on NPC → `UseOnOtherActor`.
5. **NPCs spawn** — companions/enemies walk to Byte (mobile sprite at root, no flicker, no black overlay).
6. **Touch Byte** — drag in any direction → she follows, release with flick → fling/fall/bounce.
7. **Button bar top-right** — MENU, TERM, SIT, OUTFIT, LOCK, AWAY, ZOOM all tap-based, auto-hide when pause/terminal/minigame open.
8. **Terminal** — TERM → CLOSE button exists, SEND bottom-right, commands `force_spawn_item /ls` lists IDs (ResourceCache loaded).
9. **Gallery/Recipes** — after discovering items, pause → gallery/recipe lists fill (proves ResourceCache).
10. **Magnifier** — ZOOM → lens follows finger, +/− buttons, CLOSE.
11. **AWAY** — tap hides Byte with dialogue, tap again restores at ground level.
12. **Logcat** — should show `[Mobile] TryLoadResources attempt 0 - calling LoadData` then `[Mobile] ResourceCache already loaded: ITEM=...` and no infinite Spawner retry.

If items still not spawning, send `logcat | grep Spawner|ResourceCache`.

---

## Build Pipeline (followed, updated)

```
1. Toolchain: dotnet9, godot 4.6.2 mono, templates, build-tools r35, GDRE 2.6.3, git-lfs
2. Game zip from itch.io → Byte.pck 35 MB
3. Decompile DLL (repo already has decompiled/ 77 files)
4. strip_generated.py → stripped/
5. make_project.py → src/ (stripped + overlay patched/Scripts V30)
6. proj/: DesktopPets.csproj (Godot.NET.Sdk/4.6.2, net9.0) + Scripts/ copy
7. dotnet build -c Debug → DesktopPets.dll 682K with V30 markers
8. GDRE extract Byte.pck → pck_src/ 3342 files
   GDRE extract previous merged PCK (v10) → pck_v10/ 3517 files (contains .godot/mono/publish/arm64)
   Replace DesktopPets.dll in pck_v10 with new DLL, keep project.binary a33cb589 (Scenes/Main.tscn, GL Compatibility)
   GDRE pck-create → Byte-v30.pck 64 MB
9. Assemble APK: base = v10 APK (engine), replace assets/assets.sparsepck with Byte-v30.pck,
   replace assets/project.binary with a33cb589, replace loose DLLs with 175 assemblies (new DLL)
   Remove *.a libs, rezip (STORED for sparsepck, resources.arsc, mono; DEFLATED for libgodot), zipalign 4
10. apksigner sign JKS byte/bytepass v2+v3
11. Verify + LFS push
```

**Env notes:** 4 GB swapfile required, TMPDIR=/home/user/.cache/tmp, DOTNET_ROOT=/home/user/.cache/dotnet9, HOME=/home/user/.cache/home.

---

## Next Steps / Known Gaps

- AttachObjWindow bubbles still use Window (may flicker on some devices) — could also port to mobile sprite root if needed.
- Magnifier lens uses ViewportTexture — may show black on GL Compatibility; fallback could be simple zoom.
- Terminal soft keyboard reliability untested on this device build.
- Snake/Dino/CatchHer minigame D-pads already in place.

---

## Legal

Personal-use port only, per developers' permission. Do not publish APK or source publicly. 18+ content.
