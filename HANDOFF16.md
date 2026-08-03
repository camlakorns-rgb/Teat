# HANDOFF16 — V35 Fix Floating One Block Higher + Still Small

**Date:** 2026-08-03
**Previous:** V34 `Byte-Launcher-v34-fix-bit-tiny-float.apk` — reverted scaling, boosted Bit 1.8x but user says: "now hes floating one block higher also he's still small"
**APK:** `Byte-Launcher-v35-float-small-fix.apk` (123 MB)

## Issues V34

- Bit still small (1.8x not enough)
- Floating one block higher: both Byte and Bit appear floating above ground.

Root cause:
- Ground calculation `groundY = screenH - trueSize` places bottom of bounding box at screen bottom. If trueSize increased (1.8x), groundY moves UP (higher) → appears to float higher.
- For Byte, previous offset reset removed foot alignment; after reverting, still floating slightly due to sprite padding.

## V35 Fixes

**ActorWindow (Bit/Trojan):**
- Boost increased: `trueSize *= 2.8f`, `MainBody.Scale *= 2.2f` (was 1.8/1.5) → Bit much larger, not small.
- Ground fix: `groundY = End.Y - trueSize.Y + Round(trueSize.Y * 0.15f)` → pushes 15% lower to compensate for floating (moves down).

**Main.cs (Byte):**
- Ground fix: `groundY = screenH - trueSize.Y + Round(trueSize.Y * 0.08f)` → pushes 8% lower to sit on ground, not one block above.

**Markers:** `V35_FLOAT_SMALL_FIX`

## Test

```bash
adb push Byte-Launcher-v35-float-small-fix.apk /sdcard/Download/
su -c "cp /sdcard/Download/Byte-Launcher-v35-float-small-fix.apk /data/local/tmp/byte.apk"
su -c "pm install -r /data/local/tmp/byte.apk"
```

Expected:
- Byte grounded (not floating one block higher), original fine size
- Bit 2.8x larger (not tiny), grounded with 15% offset lower, walks correctly

