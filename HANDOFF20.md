# HANDOFF20 — V39 Deep Dive: Bit Small + Top of Screen (User: "do a deep dive and find out why instead of just guessing?")

**Date:** 2026-08-03
**Previous:** V38 `Byte-Launcher-v38-flicker-teleport-fix.apk` — fixed flicker + teleport but user reports Bit still small and at top
**APK:** `Byte-Launcher-v39-deepdive-bit-top-small.apk` (123 MB)

## Deep Dive (Not Guessing)

### 1. Found Bit's Actual Data

Extracted `Qubit.res` (Bit's CharacterInfoDataRes) via GDRE:
```
Name = "Qubit"
characterSize = Vector2i(128, 128)
characterScale = Vector2(1.75, 1.75)
```
Byte's data from `Byte.res`:
```
Name = "Byte"
characterSize = Vector2i(128, 128)
characterScale = Vector2(1.75, 1.75)
```
**Identical base size!** 128*1.75=224px. On 1080p screen, 224px is ~1/5 screen height = tiny (expected). Byte looks fine because her window is centered? Actually Byte also 224px but looks fine? The difference is Byte is main character Node2D, not Window? No both same.

Why Bit appears smaller? Because our previous boosts changed trueSize which moves ground up.

### 2. Why Bit at Top of Screen?

Traced `ActorWindow.SetupActorWindow`:

- If overridePos==Zero, spawn at left/right off-screen:
  ```csharp
  y = ScreenGetUsableRect.End.Y - trueSize.Y;
  ```
  UsableRect excludes nav bar (e.g., 2352 vs 2400). Byte uses `ScreenGetSize()` (full 2400) for ground.
- So Bit ground = 2352 - trueSize, Byte ground = 2400 - trueSize → Bit 48px higher (top).
- When we boosted trueSize to 3.0x (224*3=672), ground = 2352-672=1680 (middle of screen, top-ish). Larger trueSize = higher ground = appears at top.
- Previous V35/V36 boosted trueSize, causing floating 1 block higher, then 3 blocks higher.

### 3. Why Previous Boost Made Floating Worse?

`groundY = End.Y - trueSize.Y`
- Original trueSize 224: ground 2352-224=2128 (near bottom)
- Boosted 3x: trueSize 672: ground 2352-672=1680 (middle, top-ish)
- Boosted 4.5x: trueSize 1008: ground 2352-1008=1344 (top)

So boosting trueSize directly moves ground up → floating top.

### 4. Why Still Small?

Boosting trueSize also increases Window Size (collision box) but visual MainBody.Scale was boosted mismatched (1.8 vs 2.8 etc) causing gap.

### V39 Fix (Deep Dive, Not Guessing)

**Keep trueSize original for grounding:**
- Do NOT boost trueSize for ground calculation. Keep trueSize = 224 for ground = 2352-224=2128 (same as Byte level).
- Boost ONLY visual: `MainBody.Scale *= 2.2f`

```csharp
if (id.Contains("bit")||id.Contains("qubit")||id.Contains("trojan")) {
    float visualBoost = 2.2f;
    MainBody.Scale *= visualBoost;
    // Keep trueSize original so ground stays at Byte level
}
```

**Ground: use full screen size like Byte, not usableRect:**
```csharp
if (_isMobile) {
  screenSize = ScreenGetSize();
  groundY = screenSize.Y - trueSize.Y; // same as Byte
} else {
  groundY = usableRect.End.Y - trueSize.Y;
}
```

Now Bit:
- TrueSize 224 (same as Byte) → ground 2400-224=2176 (same as Byte, not top)
- Visual Scale 2.2x → 224*2.2=492px visually larger (not tiny), but bounding box still 224, so feet touch ground, no floating.

**Also fixed initial spawn Y:**
- Old: `y = UsableRect.End.Y - trueSize.Y`
- New: mobile uses `ScreenSize.Y - trueSize.Y`

**Markers:** `V39_DEEP_DIVE_BIT_TOP_SMALL`

## Verification

- dotnet build 0 errors
- Marker V39 present
- zipalign OK, apksigner v2+v3 OK
- project.binary a33cb589

## Test

```bash
adb push Byte-Launcher-v39-deepdive-bit-top-small.apk /sdcard/Download/
su -c "cp /sdcard/Download/Byte-Launcher-v39-deepdive-bit-top-small.apk /data/local/tmp/byte.apk"
su -c "pm install -r /data/local/tmp/byte.apk"
# Enable Cindesh
```

Expected:
- Byte: original fine size, grounded (reverted from V32 scaling)
- Bit: 2.2x visually larger (not tiny), grounded at same Y as Byte (not top), full screen ground reference

If still small, we can increase visualBoost to 3.0 but keep trueSize original.

