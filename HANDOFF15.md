# HANDOFF15 — V34 Fix Byte Floating + Bit Tiny & Floating (User: "now byte is floating and bit is even smaller")

**Date:** 2026-08-03
**Previous:** V33 `Byte-Launcher-v33-revert-scale-fix-bit.apk` — reverted V32 scaling (Byte was fine), fixed Bit floating via offset reset. User report: "Alright now byte is floating and bit is even smaller Oh and hes still floating"
**APK:** `Byte-Launcher-v34-fix-bit-tiny-float.apk` (123 MB)

## Analysis of V33 Failure

- V32 scaled all characters 2.5x → Byte too big
- V33 reverted scaling to 1.0 → Byte back to original fine size, but Bit (which was tiny) became **even smaller** (1.0 vs 2.5)
- Both still floating: offset reset `spriteParentController.Position = Zero` removed original offset that was needed to align feet to ground box. Setting to zero made them float above ground.

## V34 Fixes

**Revert Character.cs offset reset:**
- Removed V32/V33 `Position = Vector2.Zero` in Character.cs, back to original:
  ```csharp
  spriteParentOriginalPos = spriteParentController.Position;
  ```
- Keeps original designed foot alignment for Byte.

**Revert ActorWindow offset reset, add per-actor boost:**
- Removed zeroing of `spriteParentController.Position` in ActorWindow.
- Added targeted boost only for Bit/Trojan/Qubit:
  ```csharp
  characterActor.trueSize = characterSize * scale * settingSpriteScaler;
  if (_isMobile && (id.Contains("bit")||id.Contains("qubit")||id.Contains("trojan")||name.Contains(...))) {
    trueSize = (Vector2I)(trueSize * 1.8f);
    MainBody.Scale *= 1.5f;
  }
  ```
- Bit now 1.8x larger than original tiny size, but Byte stays original size.

**Grounding:**
- Kept V31 invisible item fix (CreateMobileRenderer deferred, AddChild before Setup)
- Main.cs _Process mobile ground logic already falls to groundY = screenSize.Y - trueSize.Y each frame. No extra offset.

**Markers:** `V34_FIX_FLOAT_TINY_BIT`

## Verification

- dotnet build 0 errors
- Marker V34 present
- zipalign OK, apksigner v2+v3 OK
- project.binary a33cb589

## Test

```bash
adb push Byte-Launcher-v34-fix-bit-tiny-float.apk /sdcard/Download/
su -c "cp /sdcard/Download/Byte-Launcher-v34-fix-bit-tiny-float.apk /data/local/tmp/byte.apk"
su -c "pm install -r /data/local/tmp/byte.apk"
```

Expected after V34:
- Byte: original fine size, standing on ground (not floating), no flicker, no black screen, items visible
- Bit (enable_cindesh_mode): 1.8x larger than before (not tiny), grounded not floating, walks to Byte, cuck anim works

If Byte still floating, need to adjust groundY offset slightly (e.g., groundY + 10px) or inspect spriteParentController original offset value from scene.

