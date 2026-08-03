# HANDOFF27 — V46 Bit Tiny Floating Again + Spawn Menu Disappears Cant Drag

**Date:** 2026-08-03
**Previous:** V45 `Byte-Launcher-v45-bit-shorter-spawn-menu.apk` — Bit 0.9x shorter than Byte, ground 30%, spawn menu (MENU,TERM,SIT,OUTFIT,SPAWN) remove LOCK/ZOOM/AWAY. User: "now hes back to being tiny and floating again also when I tap on the spawn menu every menu just disappears and I acnt drag or anything"
**APK:** `Byte-Launcher-v46-bit-tiny-float-spawn-fix.apk` (123 MB)

## Issues V45

- Bit tiny and floating again: 0.9x too small (201px) vs Byte 224px, still tiny on 1080p. Need larger but slightly shorter than Byte.
- Spawn menu disappears when tapped and can't drag.

## Root Causes

**Bit tiny/floating:**
- 0.9x makes Bit 201px vs Byte 224px, even tinier than original tiny. User said Bit should be a bit shorter than Byte, but both need to be larger than tiny to be visible on 1080p.
- Ground +30% push down helped but 0.9x trueSize small means ground higher? Actually trueSize 201, ground 2400-201+60=2259, still near bottom but visual small.

**Spawn menu disappears:**
- IsPointInUI only checked direct children of box and panel, not recursively. Buttons inside GridContainer inside ScrollContainer were not detected as UI → touch passed to Main._Input → drag Byte → menu maybe freed?
- _Process set _box.Visible = !occupied && !spawnOpen, and _spawnPanel.Visible = spawnOpen || !occupied — when spawnOpen true, box hidden, panel visible. But when tapping panel, if IsPointInUI returned false (because recursive check failed), Main would treat as drag on Byte, not UI, and maybe close menu?
- Also Panel MouseFilter default Stop may block child buttons.

## V46 Fixes

**Bit size + floating:**
- Byte: settingSpriteScaler 1.6x on mobile (from V46 fix_v46.py) → Byte 224*1.6=358px, not tiny, original fine but larger for visibility
- Bit: 1.4x (shorter than Byte 1.6x) → 224*1.4=313px, slightly shorter than Byte 358px, not tiny, not blurry (close to 1.0)
- Ground: +30% push down for both to fix floating a bit

**Spawn menu disappearing + can't drag:**
- Rewrote MobileUI.cs:
  - IsPointInUI now recursive: IsPointInControlRecursive checks all descendants
  - Panel MouseFilter = Stop, children Stop, to capture input
  - _Process: _box.Visible = !occupied && !spawnOpen, _spawnPanel stays visible when spawnOpen, no auto-hide based on occupied
  - Buttons have MouseFilter Stop and PointingHand cursor
  - Grid buttons inside ScrollContainer now correctly detected as UI → won't drag Byte through menu
  - CLOSE button only way to close, tapping item spawns but keeps menu open (so not disappearing)

**Markers:** `V46_BIT_TINY_FLOATING_SPAWN_MENU_FIX`

## Test

```bash
adb push Byte-Launcher-v46-bit-tiny-float-spawn-fix.apk /sdcard/Download/
su -c "cp /sdcard/Download/Byte-Launcher-v46-bit-tiny-float-spawn-fix.apk /data/local/tmp/byte.apk"
su -c "pm install -r /data/local/tmp/byte.apk"
```

Expected:
- Byte 1.6x (not tiny), grounded
- Bit 1.4x (a bit shorter than Byte, not tiny), grounded (30% push down)
- Not blurry (close to 1x)
- Spawn menu: tap SPAWN → menu stays open, tap item buttons → spawns at Byte, menu stays, tap CLOSE to close, after close you can drag Byte again

