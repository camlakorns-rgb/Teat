# HANDOFF17 — V36 Revert Byte Back, Fix Bit Tiny + Floating 3 Blocks High

**Date:** 2026-08-03
**Previous:** V35 `Byte-Launcher-v35-float-small-fix.apk` — boosted Bit 2.8x but mismatched trueSize vs visual scale + extra ground offset caused floating 3 blocks high. User: "now bit is still small and hes floating 3 block high also revert byte back"
**APK:** `Byte-Launcher-v36-revert-byte-fix-bit-final.apk` (123 MB)

## User Report V35

- Bit still small
- Floating 3 blocks high
- Revert Byte back (was fine before V32 scaling)

## Analysis V35 Failure

- Bit boost: `trueSize *= 2.8`, `MainBody.Scale *= 2.2` — mismatch: bounding box 2.8x, visual 2.2x → gap at bottom = (2.8-2.2)*orig = 0.6*orig → appears floating
- Ground: `groundY = End - trueSize + 15% trueSize` → with boosted trueSize, ground moves up by `-trueSize +15%` = `-0.85*boosted` → higher than original, floating more = 3 blocks high
- Byte: V35 had ground +8% offset added to fix floating one block, but that made Byte float? Revert needed.

## V36 Fixes

**Revert Byte:**
- Main.cs mobile ground: back to original `groundY = screenSize.Y - trueSize.Y` (no +8%)
- Character.cs: back to original `spriteParentOriginalPos = spriteParentController.Position` (no zeroing) — keeps designed foot offset

**Fix Bit (same factor for size and visual, grounded):**
```csharp
float boost = 3.0f;
trueSize = (Vector2I)(trueSize * boost);
MainBody.Scale *= boost; // same factor
```
- Ground: `newPos = (ClampX, End.Y - trueSize.Y)` (no extra +15%) → bottom of boosted box touches bottom, visual fills box → grounded, not floating
- Boost 3.0x → not tiny

**Markers:** `V36_REVERT_BYTE_FIX_BIT_FINAL`

## Test

```bash
adb push Byte-Launcher-v36-revert-byte-fix-bit-final.apk /sdcard/Download/
su -c "cp /sdcard/Download/Byte-Launcher-v36-revert-byte-fix-bit-final.apk /data/local/tmp/byte.apk"
su -c "pm install -r /data/local/tmp/byte.apk"
# Enable Cindesh to see Bit
```

Expected:
- Byte: original fine size, grounded (reverted), no floating
- Bit: 3x larger (not tiny), grounded (not 3 blocks high), walks to Byte

If still floating, need to inspect actual spriteParentController offset value from scene — may need to keep original offset but adjust ground by -10px.

