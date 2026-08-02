# HANDOFF4 — Byte Desktop Pet → Android APK (Session 4)

**Date:** 2026-08-02
**Previous:** HANDOFF (GDRE recovery failed) → Handoff2 (original PCK, got past 25%) → HANDOFF3 (merged .NET assemblies into PCK, v3–v8 APKs)
**Repo:** https://github.com/camlakorns-rgb/Teat (LFS: `*.apk *.pck *.zip *.dll`)

---

## TL;DR

**v9 (`Byte-Launcher-v9-fullphysics.apk`, 110 MB, signed, in this repo) is the first APK that actually contains the mobile fixes.** Sessions 1–3's "fixes" (drag, walk, throw, bubble) were *documented in HANDOFF3 but never compiled into any shipped APK* — the shipped DLLs were stale/partial builds. Root causes found and fixed this session:

1. **v8's PCK DLL was built from a project named `GodotPlugins.Game`** (a misnamed launcher project) and contained only PART of the mobile patches (GetThinnerCollisionBox mobile branch) — `FollowMouse`, `Walk`, and throw/gravity still used `mainWindow.Position` on mobile → **"stuck in middle"** (the Android window can't move, only the Node2D `Position` can).
2. **v8's loose `assets/.godot/mono/publish/arm64/DesktopPets.dll` was a stale v6-era build** (no mobile branch at all) — a leftover from an older export.
3. **`MobileMousePos()` had infinite recursion** (`return MobileMousePos();` instead of `return DisplayServer.MouseGetPosition();`) in both DLLs — latent crash.
4. **The repo's `patched/` sources did not match HANDOFF3's documented mobile code** — the doc described intent, the code never implemented it. The bubble fix (`AttachObjWindow.FollowParent` mobile branch) was never written at all.

## What v9 actually changes (vs v8)

| # | File | Change |
|---|------|--------|
| P1 | Main.cs | `MobileMousePos()` recursion → `DisplayServer.MouseGetPosition()` fallback |
| P2 | Main.cs | `FollowMouse()`: on mobile moves `Position` (Node2D) clamped to screen, not `mainWindow.Position` |
| P3 | Main.cs | `Walk()`: reads/writes `Position` on mobile (both auto-walk-to-item and idle wander) |
| P4 | Main.cs | `_Process` throw physics: on mobile applies gravity 1400, bounce, `groundY = screenSize.Y - trueSize.Y`, instant ground snap + `BeginLand()` + `CheckLandingInteraction()`; desktop path unchanged |
| P5 | Main.cs | `GetThinnerCollisionBox()`: 100% width on mobile (was 33% — too small to grab) |
| C1/C2 | Character.cs | `StartDangle`/`UpdateDangle` use `Main.Instance.MobileMousePos()` (dangle velocity from touch) |
| A1 | AttachObjWindow.cs | TEXT (talk bubble) sets `MousePassthrough = true` on mobile — bubble no longer draggable |
| A2 | AttachObjWindow.cs | `FollowParent()` mobile branch: bubble positioned centered above Byte's head from `Main.Instance.Position`, no left-side flip, clamped ≥ 0 |

Build marker: `public static readonly string V9_BUILD = "V9_MOBILE_BUILD_MARKER";` in Main.cs.
**VERIFY BEFORE INSTALLING:** the marker string must be present (UTF-16LE) in `DesktopPets.dll` inside the APK's `assets/assets.sparsepck` AND `assets/.godot/mono/publish/arm64/` (both were replaced with the same v9 build this session).

## Verified on this APK

- `apksigner verify` → v2+v3 OK (same `byte.keystore`, so `pm install -r` upgrade works over v8)
- `zipalign -c 4` OK, `unzip -t` OK
- `assets/project.binary` = the known-good patched file (md5 `a33cb5892eb650a5b26a9541ba70b9ad`, main_scene `Scenes/Main.tscn`, features `["4.6","C#","GL Compatibility"]`, transparent off, etc2_astc on) — identical bytes in APK and inside PCK
- Merged PCK (66 MB, 3517 files) loads in Godot 4.6.2: `load_resource_pack` OK, `load("res://Scenes/Main.tscn")` → PackedScene OK (no broken references — original Byte.pck resources preserved)
- Size 109.9 MB (v8 was 110 MB) — slimmed: 13 `*.a` static libs removed, only `libc++_shared.so` + `libgodot_android.so` deflated, everything else stored like v8

## How v9 was built (reproducible recipe)

Everything in `/home/user/.cache/` (excluded from workspace snapshot — re-download per URLs in HANDOFF.md/Handoff2):

```
# 1. Toolchain
godot 4.6.2 mono editor, export templates tpz (android_debug.apk into
  $XDG_DATA_HOME/godot/export_templates/4.6.2.stable.mono/), dotnet SDK 9.0.316,
  build-tools r35 (zipalign/apksigner), ilspycmd 8.2.0.7535 (dotnet tool, run with DOTNET_ROLL_FORWARD=Major),
  GDRE_tools v2.6.3 (PCK extract + create), git-lfs 3.5.1
# 2. Source
build/strip_generated.py  decompiled/ -> stripped/ (removes MethodName/PropertyName/SignalName,
  EditorBrowsable glue, backing fields, event accessors, EmitSignalX methods; adds `partial`)
build/make_project.py     stripped/ + v8 patched + v9 patches -> src/Scripts/** (res:// layout)
# 3. Build C#
proj/ (project.godot, DesktopPets.csproj net9.0 Godot.NET.Sdk/4.6.2, sln, icon.svg,
  export_presets.cfg Android non-gradle arm64)  -> dotnet build -c Debug (needs 4 GB swapfile)
# 4. Export launcher
godot --headless --path proj --export-debug Android out/Byte-Launcher.apk
  (needs editor_settings-4.6.tres: android_sdk_path=fake sdk with real build-tools,
   java_sdk_path=/usr/lib/jvm/jdk-11, debug_keystore=byte.keystore; fake platform-tools/adb)
# 5. Merged PCK
gdre --extract "0.5.2 LIVE/Byte.pck" -> pck_src/
cp <v8 patched project.binary> pck_src/project.binary     # 12787 B, md5 a33cb589...
cp <export APK's assets/.godot/mono/publish/arm64/*> pck_src/.godot/mono/publish/arm64/
gdre --pck-create pck_src --pck-version=3 --pck-engine-version=4.6.2 --output=Byte-v9.pck
# 6. Assemble (build/assemble_apk.py)
extract launcher apk; rm lib/*.a (13 files); replace assets/assets.sparsepck <- Byte-v9.pck;
replace assets/project.binary <- patched; rezip (STORE: resources.arsc, sparsepck,
lib mono .so/jar/dex; DEFLATE: libc++_shared.so, libgodot_android.so, rest);
zipalign -f 4; apksigner sign --ks byte.keystore --ks-pass pass:bytepass --ks-key-alias byte (JDK11)
```

Key gotchas re-learned: PCK v3 needs GDRE (not hand-rolled parsers — RSRC section); dotnet tool install needs an SDK (install with dotnet9, run with `DOTNET_ROLL_FORWARD=Major`); non-gradle mono export is fine but `gradle_build/compress_native_libraries` must be false; export templates go under `$XDG_DATA_HOME/godot/export_templates/` when XDG is set; `out/` dir must exist before export; C# string markers are UTF-16LE in the DLL (`grep -a` misses them).

## Install / test (on the Pixel 7)

```bash
adb push Byte-Launcher-v9-fullphysics.apk /sdcard/Download/
su -c "cp /sdcard/Download/Byte-Launcher-v9-fullphysics.apk /data/local/tmp/byte.apk"
su -c "pm uninstall com.desktop.byte"      # first time only / signature change
su -c "pm install /data/local/tmp/byte.apk"
logcat -d | grep -iE "godot|mono|fatal" | tail -60
```

**Expected v9 behavior:**
- Spawns bottom-center ON the ground (Position = screen/2, screen - trueSize)
- Touch anywhere on her → grab; drag in ANY direction → she follows the finger (full-width grab box)
- Release with upward/downward flick → she flies (dangle animation), falls with gravity, bounces at screen edges, lands, continues walking
- Tap → pet + talk bubble centered ABOVE her head; bubble follows her when she moves; touching the bubble does not drag/move it
- No stack overflow from `MobileMousePos` (fixed recursion)

**If still stuck in middle:** check logcat for `Position` values; suspect `ScreenDataHandler.UpdateScreenInfo` / `taskbarPos` math — on mobile everything must go through the P4 mobile branch which uses `screenSize.Y - trueSize.Y` directly (no taskbar).

## Known issues / next steps (v10 candidates)

1. **`_Input` drag synthesis presses "Move" even if the touch started off-Byte** (InputEventScreenDrag → ActionPress("Move") unconditionally). Gated by `GetThinnerCollisionBox().HasPoint()` in MovePet, so movement is safe, but other `Move`-action consumers (item pickup, actor interactions) may react to stray drags. Consider tracking `_touchStartedOnByte` per pointer.
2. **Pause menu / EULA buttons** rely on `emulate_mouse_from_touch` — if "Accept" is hard to tap, add explicit touch→click mapping for `ConfirmationMenu`.
3. **`walkTargetItem` / item pickup paths** use `mainWindow.Position` in some item code — grep remaining `mainWindow.Position` uses in Main.cs/ItemWindow.cs that run on mobile (item drag-to-desktop interactions are desktop-only by design; verify none matter on mobile).
4. **Screen orientation** — game is portrait-ish; `screen/immersive_mode=true` set; test landscape.
5. **Performance** — debug export (no trimming). A release export with `<PublishTrimmed>` may be smaller/faster but risks breaking reflection-based systems (SaveHandler etc.) — test carefully.

## Repo layout (updated this session)

- `Byte-Launcher-v3..v8-*.apk` (LFS) — previous; **note v3==v4==v5 bytes (identical LFS oid)**; v8's shipped DLL was stale (see above)
- `Byte-Launcher-v9-fullphysics.apk` (LFS) — **INSTALL THIS ONE**
- `patched/Scripts/Main.cs`, `Character.cs`, `AttachObjWindow.cs` — v9 sources (post-strip, with all mobile patches)
- `build/` — strip_generated.py, make_project.py, assemble_apk.py, pcktool.py (rebuild tooling)
- `decompiled/` — raw ILSpy output of original DesktopPets.dll
- `byte.keystore` — JKS, alias `byte`, pass `bytepass`
- `HANDOFF.md`, `Handoff2.md`, `HANDOFF3.md`, `HANDOFF4.md`, `README.md`

## Legal

Personal-use port only, per the developers' permission. Do not publish the APK or source publicly. 18+ content.
