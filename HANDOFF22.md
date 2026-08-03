# HANDOFF22 — V41 Fix Bit Floating One Block + Small + Kiss Duplicate

**Date:** 2026-08-03
**Previous:** V40 `Byte-Launcher-v40-bit-small-oneblock-fix.apk` — Bit 3.2x + ground +22%. User: "bit is still floating one block and small oh I've noticed this too when I put byte on him she should kiss him and theres an animation but he stayed there when the animation happens and theres two bits the small one and the one in the animation"
**APK:** `Byte-Launcher-v41-bit-float-kiss-fix.apk` (123 MB)

## Issues

1. Bit still floating one block (V40 +22% not enough)
2. Still small (3.2x not enough, Qubit 224px base)
3. Kiss animation duplicate: when putting Byte on Bit, small Bit remains + animation Bit → two Bits

## Deep Dive Duplicate

`ActorWindow._Process` had:
```csharp
_mobileSpriteRoot.Visible = IsActiveForMobile && characterActor.Visible;
```
This overwrote `HandleCompanionAI` which sets `Visible = shouldShow` where `shouldShow=false` when `inUseByAttachment=true` (during kiss/sex). So mobile sprite stayed visible during animation → duplicate.

## V41 Fixes

**Visual size:** 3.2x → 4.2x for Bit/Trojan/Qubit
```csharp
float visualBoost = 4.2f;
MainBody.Scale *= visualBoost;
```
TrueSize kept original for ground (not boosted) → ground stays at Byte level, not top.

**Ground:** +22% → +35% push down
```csharp
groundY = screenH - trueSize + 0.35*trueSize // was 0.22
y = same
```
Pushes Bit down to fix one block high.

**Duplicate kiss:** hide mobile sprite when inUse or inUseByAttachment
```csharp
_mobileSpriteRoot.Visible = IsActiveForMobile && characterActor.Visible && !inUse && !inUseByAttachment;
```

**Marker:** `V41_BIT_SMALL_FLOAT_KISS_DUP`

## Test

```bash
adb push Byte-Launcher-v41-bit-float-kiss-fix.apk /sdcard/Download/
su -c "cp /sdcard/Download/Byte-Launcher-v41-bit-float-kiss-fix.apk /data/local/tmp/byte.apk"
su -c "pm install -r /data/local/tmp/byte.apk"
```

Expected:
- Bit 4.2x larger (not small), grounded (not one block high, +35% offset)
- When putting Byte on Bit for kiss: small Bit hides, only animation Bit visible (no duplicate)

