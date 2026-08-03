# HANDOFF18 — V37 Fix Bit Higher and Smaller + Revert Byte

**Date:** 2026-08-03
**Previous:** V36 `Byte-Launcher-v36-revert-byte-fix-bit-final.apk` — Bit boost 3.0x same factor, ground no extra. User: "bit is higher and might be smaller" + "revert byte back"
**APK:** `Byte-Launcher-v37-bit-higher-smaller-fix.apk` (123 MB)

## User Report V36

- Bit higher (floating) and smaller
- Revert Byte back (was fine)

## Analysis

- V36 ground for actors was End - trueSize (no extra). With boost 3.0x, trueSize large, ground = End - large = high up → appears floating higher (was 1 block higher in V35 with extra offset, now in V36 still higher due to large trueSize)
- Also used usableRect End for ground? In V36 we reverted walk ground to original usableRect End - trueSize, but Byte uses full ScreenGetSize. Usable rect excludes nav bar → higher than Byte's ground → Bit appears higher than Byte.
- Still small: 3.0x may still be small vs Byte if Bit base size small. Need 4.5x.

## V37 Fixes

**ActorWindow:**
- Boost increased to 4.5x for Bit/Qubit/Trojan (was 3.0x): `trueSize *= 4.5`, `Scale *= 4.5` same factor → no gap, much larger.
- Ground: use full screen size on mobile, not usable rect, to match Byte ground:
  ```csharp
  if (_isMobile) {
    screenSize = ScreenGetSize(screenIndex);
    groundY = screenSize.Y - trueSize.Y;
  } else {
    groundY = cachedActorScreenRect.End.Y - trueSize.Y;
  }
  ```
- Ensures Bit and Byte share same ground reference.

**Main.cs:**
- Byte ground reverted to original `screenSize.Y - trueSize.Y` (no +8%) per user request "revert byte back".

**Marker:** `V37_BIT_HIGHER_SMALLER` + same boost.

## Test

```bash
adb push Byte-Launcher-v37-bit-higher-smaller-fix.apk /sdcard/Download/
su -c "cp /sdcard/Download/Byte-Launcher-v37-bit-higher-smaller-fix.apk /data/local/tmp/byte.apk"
su -c "pm install -r /data/local/tmp/byte.apk"
```

Expected:
- Byte: original fine size, grounded (reverted)
- Bit: 4.5x larger (not small), grounded same level as Byte (not higher), using full screen ground

