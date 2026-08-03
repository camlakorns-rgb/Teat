# HANDOFF14 — V33 Revert Scale, Fix Bit Floating (User Clarification)

**Date:** 2026-08-03
**Previous:** V32 `Byte-Launcher-v32-scale-float-fix.apk` — scaled Byte 2.5x to fix tiny, but user says: "you scaled byte who was fine it was bit that was tiny and floating"
**APK:** `Byte-Launcher-v33-revert-scale-fix-bit.apk` (123 MB, signed, LFS)
**User clarification:** Bit is not a typo, it's a separate character (Byte's BF) from Cindesh Mode. Byte was fine size, Bit was tiny and floating.

From F95 thread:
> Cindesh Mode adds two new items (Flowers and Flowers Trojan) and two new Actors (Bit and Trojan). Bit by lore is Byte's BF and cuck.

## V32 Problem

V32 forced:
```csharp
settingSpriteScaler = 2.5f
settingItemScaler = 1.8f
settingUIScaler = 1.3f
```
This scaled **all** characters (Byte and Bit) because trueSize uses same scaler. Byte was already fine, became too big. Bit still tiny? Actually Bit's base characterSize may be smaller so still tiny even at 2.5x, and floating due to sprite offset.

## V33 Fixes (Revert + Targeted)

**Revert Main.cs scaling:**
- Removed V32 block that forced 2.5/1.8/1.3.
- Back to original: `trueSize = characterSize * characterScale * settingSpriteScaler` with saved scaler (default 1.0). Byte returns to original fine size.

**Keep V31 item visible fix:**
- ItemWindow: `CreateMobileRenderer()` deferred creation (fixes invisible items when GetTree null), AddChild before Setup in Main.cs.

**Fix Bit floating (ActorWindow):**
- In `ActorWindow.SetupActorWindow`, on mobile:
  ```csharp
  if (characterActor.spriteParentController != null)
      characterActor.spriteParentController.Position = Vector2.Zero;
  ```
- Also kept Character.cs offset reset for Byte (harmless, prevents floating).
- Ensures Bit's sprite sits at base.Position (ground), not offset above.

**Markers:** `V33_REVERT_SCALE_FIX_BIT`

## Verification

- dotnet build 0 errors
- Marker V33 present
- zipalign OK, apksigner v2+v3 OK
- project.binary a33cb589

## Test

```bash
adb push Byte-Launcher-v33-revert-scale-fix-bit.apk /sdcard/Download/
su -c "cp /sdcard/Download/Byte-Launcher-v33-revert-scale-fix-bit.apk /data/local/tmp/byte.apk"
su -c "pm install -r /data/local/tmp/byte.apk"
# Enable Cindesh Mode to spawn Bit:
# TERM -> type `enable_cindesh_mode` -> enter
# Then force spawn: `force_spawn_actor` -> look for Bit ID, or wait for random spawn
```

Expected:
- Byte original size, grounded, no flicker, no black screen, items visible
- Bit (blue BF) when spawned: not tiny (original intended size), grounded not floating, walks to Byte, has cuck animation when Byte fucked

## Next

If Bit still tiny, may need per-actor scale boost for Bit/Trojan only (check CharacterInfoDataRes for Bit's characterScale).
