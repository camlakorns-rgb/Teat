# HANDOFF19 — V38 Flicker + Sex Scene Teleport Fix (User: Drive video most stable, but other chars flicker, Byte teleports right on sex scene)

**Date:** 2026-08-03
**Previous:** V37 `Byte-Launcher-v37-bit-higher-smaller-fix.apk` — Bit 4.5x boost, full screen ground. User: "this is the most stable built the only problem is the fact the other characters flicker and only byte is not flickering oh also when she enters a sex scene she teleports to the right and when the scene ends she teleports back"
**APK:** `Byte-Launcher-v38-flicker-teleport-fix.apk` (123 MB)

## Issues

1. **Other characters flicker, only Byte not:** ActorWindow mobile renderer should avoid flicker by Visible=false, but code paths set base.Visible=true:
   - `HandleEnemyAI`: `if (!base.Visible) base.Visible = true;`
   - `SyncCachesToScreen`: `if (base.Visible != shouldShow) base.Visible = shouldShow;`
   On mobile, setting Window Visible true creates fullscreen opaque surface → flicker/black overlay.

2. **Sex scene teleport to right then back:** In `AttachObjWindow`:
   - `LayerOntoParent` for OVERRIDE uses `attachmentMargin.X` → offset to right on mobile
   - `HandleForcedActorMovement` moves `Main.Instance.Position` during forced walk (sex scene walk) → Byte (invisible) moves right, then `StopForcedMovement` restores `_savedBytePosX` → teleport back.

## V38 Fixes

**ActorWindow flicker:**
```csharp
if (!base.Visible) base.Visible = true; // becomes
if (!base.Visible && !Main._isMobile) base.Visible = true;

if (base.Visible != shouldShow) base.Visible = shouldShow;
// becomes
if (base.Visible != shouldShow && !Main._isMobile) base.Visible = shouldShow;
```
- Keeps Window invisible on mobile, only mobile sprite visible → no flicker.

**AttachObjWindow teleport:**
- `LayerOntoParent`: ignore margin on mobile for OVERRIDE:
  ```csharp
  int marginX = Main._isMobile ? 0 : (int)attachmentMargin.X;
  int marginY = Main._isMobile ? 0 : (int)attachmentMargin.Y;
  ```
- `HandleForcedActorMovement`: on mobile don't move Byte:
  ```csharp
  if (Main._isMobile && parentWindow == Main.Instance.mainWindow) {
    // Keep stationary, was causing teleport right
  } else {
    parentWindow.Position = ...
  }
  ```

**Keeps V37 fixes:**
- Bit 4.5x boost same factor for trueSize and Scale, full screen ground for actors to match Byte
- Byte reverted to original size/ground
- Items visible via deferred CreateMobileRenderer

**Marker:** `V38_FLICKER_TELEPORT_FIX`

## Test

```bash
adb push Byte-Launcher-v38-flicker-teleport-fix.apk /sdcard/Download/
su -c "cp /sdcard/Download/Byte-Launcher-v38-flicker-teleport-fix.apk /data/local/tmp/byte.apk"
su -c "pm install -r /data/local/tmp/byte.apk"
```

Expected:
- No flicker for companions/enemies/Bit/Trojan (only Byte previously stable, now all stable)
- Sex scene (e.g., use red orb item): Byte stays in place, attachment centered above her, no teleport right, after scene ends she remains at same X (no teleport back)
- Bit 4.5x size, grounded

