# HANDOFF26 — V45 Bit Shorter Than Byte, Floating Fix, No Blur, Spawn Menu, Remove Lock/Zoom/Away

**Date:** 2026-08-03
**Previous:** V44 `Byte-Launcher-v44-huge-match-anim.apk` — Bit 2.6x huge, user: "hes floating a bit now and also hes too tall he basically should be a bit short than byte also hes burry for some reason Also make a menu that I can spawn items also remove lock zoom and away these aren't useful"
**APK:** `Byte-Launcher-v45-bit-shorter-spawn-menu.apk` (123 MB)

## User Report V44

- Bit floating a bit now
- Too tall, should be a bit shorter than Byte
- Blurry for some reason
- Make menu to spawn items
- Remove lock, zoom, away buttons

## Fixes

**Bit size + blurry:**
- Previous 2.6x upscaling caused blurry (linear filter). User wants Bit slightly shorter than Byte.
- New: `visualBoost = 0.9f` (90% of Byte) → shorter than Byte, close to 1.0 so not blurry (downscale 10% minimal blur)
- Previously 5.5x huge was blurry due to large upscaling.

**Floating a bit:**
- Ground offset increased from 22% to 30% push down:
  ```csharp
  groundY = screenH - trueSize + 30%*trueSize
  y = same
  ```
  Pushes down more to fix floating.

**Spawn menu + remove buttons:**
- MobileUI: removed LOCK, ZOOM, AWAY buttons (were using Screen_Lock, Despawn, Magnifier actions)
- Kept MENU, TERM, SIT, OUTFIT
- Added SPAWN button → opens full-screen panel:
  - Header with CLOSE
  - Quick row: Random Item, Random Actor, Clear Items
  - Scroll Grid 2 columns listing all ItemDataRes from ResourceCache (itemID) and CharacterInfoDataRes (Name)
  - Each button spawns at Byte's position via CallItemSpawn / CallActorSpawn
  - IsPointInUI now includes spawn panel so touches don't pass through to Byte

**Marker:** `V45_BIT_SHORTER_GROUNDED_SPAWN_MENU`

## Test

```bash
adb push Byte-Launcher-v45-bit-shorter-spawn-menu.apk /sdcard/Download/
su -c "cp /sdcard/Download/Byte-Launcher-v45-bit-shorter-spawn-menu.apk /data/local/tmp/byte.apk"
su -c "pm install -r /data/local/tmp/byte.apk"
```

Expected:
- Bit 0.9x Byte (slightly shorter, not tall, not blurry)
- Grounded (not floating a bit) with 30% push down
- Mobile bar: MENU, TERM, SIT, OUTFIT, SPAWN (no LOCK/ZOOM/AWAY)
- SPAWN opens menu to spawn any item/actor

