# HANDOFF13 — V32 Scale & Floating Fix

**Date:** 2026-08-03
**Previous:** V31 `Byte-Launcher-v31-item-visible-fix.apk` — fixed invisible items (deferred renderer, AddChild before Setup). User report: "bit isnt flickering anymore but hes tiny and hes floating"
**APK:** `Byte-Launcher-v32-scale-float-fix.apk` (123 MB, signed, LFS)

## User Report V31

- No black screen ✓
- No flickering ✓
- Items spawn ✓ but...
- Byte tiny and floating

## Root Causes

### Tiny
- Pixel 7 screen 1080x2400. Byte trueSize = characterSize * characterScale * settingSpriteScaler. settingSpriteScaler default 1.0 → ~256-400px tall on 2400px screen = tiny (~1/6 screen).
- Desktop window size = trueSize, so Byte fills window. On mobile, main viewport is full screen, so same pixel size looks tiny.

### Floating
- `Character.cs` Setup: `spriteParentOriginalPos = spriteParentController.Position` where Position may be non-zero from scene (e.g., offset for idle). `Position` is Byte's Node2D ground position, but sprite offset adds floating gap.
- Also saved scaler was small, trueSize small, groundY = screenH - trueSize = near bottom but visual sprite may have transparent padding below feet.

## V32 Fixes

**Main.cs:**
- On mobile, after `saveHandler.AttemptLoad()`:
  ```csharp
  if (_isMobile) {
    if (settingSpriteScaler < 2.0) settingSpriteScaler = 2.5;
    if (settingItemScaler < 1.5) settingItemScaler = 1.8;
    if (settingUIScaler < 1.2) settingUIScaler = 1.3;
    SaveSettings();
  }
  ```
- Recalc trueSize after adjustment.

**Character.cs:**
- In `SetupCharacter()`, on mobile reset sprite offset:
  ```csharp
  if (Main._isMobile) spriteParentController.Position = Vector2.Zero;
  spriteParentOriginalPos = spriteParentController.Position;
  ```
- Ensures sprite sits exactly at Node2D position, not offset.

**Markers:** `V32_SCALE_FLOAT_FIX` present.

## Verification

- dotnet build 0 errors
- Marker V32 present in DLL
- zipalign OK, apksigner v2+v3 OK
- project.binary same a33cb589

## Test

```bash
adb push Byte-Launcher-v32-scale-float-fix.apk /sdcard/Download/
su -c "cp /sdcard/Download/Byte-Launcher-v32-scale-float-fix.apk /data/local/tmp/byte.apk"
su -c "pm install -r /data/local/tmp/byte.apk"
```

Expected:
- Byte ~2.5x larger, not tiny
- Standing on ground, not floating (falls to groundY each frame)
- Items 1.8x larger, UI 1.3x
- No flicker, no black screen, items visible, drag/throw works

If still floating, check if character has extra bottom padding in SpriteFrames — may need additional Y offset adjustment.

