# HANDOFF21 — V40 Fix Bit Still Small + One Block High + Walks

**Date:** 2026-08-03
**Previous:** V39 deep dive — Qubit 128x128 scale 1.75 same as Byte (224px), previous trueSize boost moved ground up causing top. V39 kept trueSize original for ground and boosted visual 2.2x. User: "bit is still small but hes one block high instead of on top of the screen also I've noticed he can also walk"
**APK:** `Byte-Launcher-v40-bit-small-oneblock-fix.apk` (123 MB)

## Current State V39

- Bit not at top anymore, now one block high (progress from top)
- Still small (2.2x not enough)
- Can walk (expected, companion AI)

## V40 Fixes

- Visual boost increased from 2.2x to 3.2x → larger, not small
- Ground offset added 22% push down to fix one block high:
  ```csharp
  groundY = screenH - trueSize + 0.22*trueSize
  spawnY = same +0.22
  ```
  Pushes Bit down to ground level, same as Byte (full screen reference, not usableRect)

- Byte reverted to original (no scaling, original ground)

**Marker:** `V40_BIT_SMALL_ONE_BLOCK_HIGH`

## Test

```bash
adb push Byte-Launcher-v40-bit-small-oneblock-fix.apk /sdcard/Download/
su -c "cp /sdcard/Download/Byte-Launcher-v40-bit-small-oneblock-fix.apk /data/local/tmp/byte.apk"
su -c "pm install -r /data/local/tmp/byte.apk"
```

Expected:
- Byte original size, grounded
- Bit 3.2x larger (not small), grounded (not one block high), walks to Byte correctly

