# HANDOFF10 — Byte Desktop Pet → Android APK (Session 10): Item Spawning & Mobile Renderer Root-Cause Fix

**Date:** 2026-08-03
**Previous:** v28 (`Byte-Launcher-v12-handoff7fix.apk`, commit `ce42bdd`)
**This session:** Fixed item & NPC non-spawning, non-visibility, and non-interactivity caused by `base.Visible = false` and desynced mobile sprite renderers. Rebuilt and signed APK, verified with LFS.
**Repo:** https://github.com/camlakorns-rgb/Teat
**APK:** `Byte-Launcher-v12-handoff7fix.apk` (123 MB, signed, LFS)
**Marker:** `V29_ITEM_SPAWN_RENDERER_FIX_BUILD`

---

## Root Cause Analysis (Why items were not spawning / visible / interactive in v28)

1. **`base.Visible = false` broke 30+ game logic checks:**
   In v28, `SetupItemWindow` and `SetupActorWindow` set `base.Visible = false` on mobile to hide Godot sub-window surfaces. However, throughout `Main.cs` and game handlers:
   - `IsPointOnAnyItem(p)` checks `if (w.Visible && ...)`
   - `FindItemAtPoint(pos)` checks `if (w.Visible && ...)`
   - Spawner, dialogue, collision, and combination routines filter items/actors by `item.Visible` / `actor.Visible`.
   Because `base.Visible` was `false`, `Main.cs` treated ALL items and actors as invisible/non-existent. They could not be touched, dragged, used, or interacted with.

2. **Mobile Sprite Renderer Desync:**
   - In `ItemWindow.cs`, `_mobileSpriteRoot` was initialized once at `base.Position` inside `SetupItemWindow()` when `base.Position` was `(0,0)`. When `Main.cs` assigned the actual spawn position, `_mobileSpriteRoot` was not updated.
   - During item fall and resting states, `_mobileSpriteRoot.Position` was never synced in `_Process` (it was only synced during `isThrown`).
   - `_mobileSprite` was never configured with proper scale (`itemObject.itemInformation.itemScale * settingItemScaler`), offset, or animation playback.
   - `_Process` contained `if (!base.Visible) base.Visible = true;` every frame, contradicting `base.Visible = false`.

3. **Memory Leaks on Node Removal:**
   - `_mobileSpriteRoot` nodes added to `GetTree().Root` were not freed when `ItemWindow` or `ActorWindow` were queue_freed, leaving dead orphaned nodes.

---

## Architectural Fix Implemented (v29)

1. **`base.Visible = true` when active:**
   - Active items and actors keep `base.Visible = true` so all 30+ `w.Visible` checks in `Main.cs` work natively without broken edge cases.
   - To prevent Android sub-window surface compositing flicker, internal sprites (`itemObject.spriteParentController` and `characterActor.MainBody`) are set to `Visible = false` on mobile.

2. **Unconditional Per-Frame Mobile Renderer Sync:**
   - In `_Process(double delta)` of `ItemWindow.cs` and `ActorWindow.cs`, `_mobileSpriteRoot.Position` is synced to `(Vector2)base.Position` every frame (spawn, fall, rest, drag, walk, idle, pop, throw).
   - `_mobileSprite` copies `SpriteFrames`, `Animation`, `Frame`, `Scale`, `Position`, `FlipH`, and `FlipV` from source sprites every frame.
   - Visibility is synced: `_mobileSpriteRoot.Visible = base.Visible && !CurrentlyPickedUp` (and `characterActor.Visible`).

3. **Clean Lifecycle Cleanup:**
   - Added `_ExitTree()` overrides to `ItemWindow.cs` and `ActorWindow.cs` to automatically `QueueFree()` `_mobileSpriteRoot` when items/actors are destroyed or consumed.

---

## Build Verification

- **zipalign:** `ALIGNED-OK` (4-byte alignment verified)
- **zip integrity:** `ZIP-OK`
- **apksigner:** Verified using v2 & v3 schemes (`Byte` keystore / `bytepass`)
- **Build marker:** `V29_ITEM_SPAWN_RENDERER_FIX_BUILD` verified present (UTF-16LE) in BOTH loose `DesktopPets.dll` and `assets/assets.sparsepck` PCK.

---

## File Changes in Commit

| File | Changes |
|---|---|
| `patched/Scripts/ItemWindow.cs` | Kept `base.Visible = true`, hide internal sprite on mobile, unconditional per-frame `_mobileSpriteRoot` sync (position, frames, anim, scale, flip), `_ExitTree` cleanup |
| `patched/Scripts/ActorWindow.cs` | Kept `base.Visible = true`, hide internal sprite on mobile, unconditional per-frame `_mobileSpriteRoot` sync, `_ExitTree` cleanup |
| `patched/Scripts/Main.cs` | Added `V29_ITEM_SPAWN_RENDERER_FIX_BUILD` marker, updated `IsPointOnAnyItem` and `FindItemAtPoint` |
| `Byte-Launcher-v12-handoff7fix.apk` | Rebuilt and signed APK |
| `HANDOFF10.md` | Handoff documentation |
