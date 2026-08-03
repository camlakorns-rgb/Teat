# HANDOFF12 — V31 Item Visible Fix

**Date:** 2026-08-03
**Previous:** V30 `Byte-Launcher-v30-blackscreen-fix.apk` — fixed black screen, but items spawned invisible (user report: "no black screen the items spawn but I can't see them").
**APK:** `Byte-Launcher-v31-item-visible-fix.apk` (123 MB, signed, LFS, same keystore)

## Root Cause V30 → Invisible Items

ItemWindow mobile renderer creation happened in `SetupItemWindow()` **before** the Window node was added to SceneTree (`GetTree() == null`).

In `OnSpawnerTimeout` and `CallItemSpawn`:
```csharp
var w = Instantiate();
w.SetupItemWindow(); // GetTree null here → try { GetTree().Root.AddChild } fails, catch silently
AddChild(w);
```

The `try { AddChild } catch {}` swallowed the failure, so `_mobileSpriteRoot` never created → items logically spawned (IsPointOnAnyItem true) but no visible sprite.

## V31 Fixes

1. **ItemWindow: deferred renderer creation**
   - New method `CreateMobileRenderer()` that checks `GetTree()==null` → `CallDeferred(CreateMobileRenderer)` to retry.
   - Setup now just calls `CreateMobileRenderer()`; method creates `_mobileSprite` copying SpriteFrames/Scale/Anim from source, creates `_mobileSpriteRoot` Node2D at `(Vector2)base.Position`, adds to `GetTree().Root`.
   - Unconditional per-frame sync already in V30 kept: Position, Visible=IsActiveForMobile, SpriteFrames/Anim/Frame/Scale/Flip.

2. **Main.cs: AddChild before Setup**
   - `CallItemSpawn`: now `AddChild` first, then `SetupItemWindow`.
   - `OnSpawnerTimeout`: same — `AddChild` before `Setup`.
   - Ensures `GetTree()` valid when Setup tries to create renderer, but deferred method also handles race.

3. **Markers:** `V31_ITEM_VISIBLE_FIX` present in DLL.

## Verification

- dotnet build 0 errors, 1 warning
- Marker `V31_ITEM_VISIBLE_FIX` in loose and PCK DLLs
- zipalign OK, apksigner v2+v3 OK, unzip OK
- project.binary `a33cb589` (Scenes/Main.tscn, GL Compatibility)

## Test

```bash
adb push Byte-Launcher-v31-item-visible-fix.apk /sdcard/Download/
su -c "cp /sdcard/Download/Byte-Launcher-v31-item-visible-fix.apk /data/local/tmp/byte.apk"
su -c "pm install -r /data/local/tmp/byte.apk"
```

Expected:
- No black screen (V30 fix kept)
- Items **visible**: 2 items drop from top, fall, rest on ground, you can see sprites, drag them, throw, use on Byte, combine.
- NPCs visible, no flicker.
- Logcat: TryLoadResources success, ITEM count >0.

