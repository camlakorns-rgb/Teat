# HANDOFF — Byte Desktop Pet → Android APK

**Last updated:** 2026-08-02 · **Start a new chat and paste this file (or summarize it) + the file list below.**

---

## 1. Goal & status (TL;DR)

Port **Byte – Desktop Pet 0.5.2** (NSFW Godot 4.6.2 **C#/mono** desktop pet by Cindesh & Fen, from itch.io) to an Android APK for the user's **Pixel 7** (rooted, Termux).

**Status: the game's code is decompiled, recompiled for Android, and packaged as an APK. It runs on the phone. Remaining work: (a) the user must install the LATEST APK and (b) fix any game-code crashes / touch controls that surface.**

- ✅ Game (C# DLL) decompiled to source → recompiled for Android arm64 (.NET 9)
- ✅ APK boots, GL renderer fixed (GL Compatibility, not Forward Plus)
- ✅ Game pack now embedded as the **main pack** (proper Godot layout) — no "not found" screen should appear
- ✅ Touch → mouse emulation enabled + custom drag-to-grab patch in `Main.cs`
- ❌ **The latest APK (77 MB, "game-first") has NOT been tested by the user yet.** The user's phone currently shows the OLD launcher's "Byte.pck not found" screen → they are running a STALE build. Next step is: install `byte/Byte-Launcher.apk` (the 77 MB one) and report what happens.
- ❓ After that: game-code layer issues likely (desktop-only window calls in `Main._Ready` are already guarded for mobile; interactions use `DisplayServer.MouseGetPosition()` which is patched to touch).

## 2. File inventory (`/home/user/byte/` — persists across chats)

| File | Purpose |
|---|---|
| `Byte-Launcher.apk` (77 MB) | **THE deliverable** — Godot engine + .NET 9 runtime + game code + game's `Byte.pck` appended as main pack. Signed. arm64-only. **Install this one.** |
| `Byte.pck` (36 MB) | The game's original data pack (extracted from `0.5.2 LIVE.zip`). |
| `byte.keystore` | Signing keystore (JKS, alias `byte`, pass `bytepass`). **Keep — needed to sign updates over existing installs.** |
| `decompiled/` (78 .cs) | Decompiled game C# source (ILSpy). Includes `Main.cs`, `ScreenDataHandler.cs`, etc. |
| `convert/project.godot` | Original project settings converted from `project.binary` (has Forward Plus feature — patch before export!). |
| `rebuild_proj.py` | Rebuilds `proj/` from `pck_out/` + `decompiled/`. Usage: `python3 rebuild_proj.py <path-to-Byte.pck>` |
| `strip_generated.py` | Removes Godot source-generator output (MethodName/PropertyName/SignalName/backing fields/EditorBrowsable members) from decompiled code. |
| `strip_v3.py` | Removes nested classes + signal delegates the generators re-add. |
| `setup_launcher.sh` | Applies launcher config: net9 csproj, Bootstrap.cs/.tscn, main_scene→Bootstrap, strips, writes `export_presets.cfg` (non-gradle). **NOTE: for the latest "game-first" build, Bootstrap is NOT used — see §5.** |
| `README.md` | User-facing install/usage notes. |

**IMPORTANT:** `/home/user/.cache/` (toolchain, build dir) does **NOT** persist between chats. Everything needed to rebuild is in `byte/` + re-downloadable tools (URLs in §4).

## 3. How the current APK is built ("game-first" packaging)

The clean approach that finally booted the game (per logcat):
1. **Export a normal launcher APK from Godot** (project with `Bootstrap` scene, net9, non-gradle, GL Compatibility, icon set, ETC2/ASTC on, transparency OFF, SDK paths set). Export exit may be 1 because Godot's internal apksigner fails — the produced APK zip is still valid.
2. **Slim it**: drop `lib/armeabi-v7a/`, `lib/x86_64/`, `assets/.godot/mono/publish/x86_64|arm32/`, `*.a` static libs; compress everything (allowed — manifest has `extractNativeLibs=true`), keep `resources.arsc` STORED.
3. **Append the game's `Byte.pck` to the END of the zip** (between entries and central directory), updating EOCD's central-dir offset — Godot's standard embedded-pack layout. The game pack's own `project.binary` then drives autoloads + main scene at startup (this is why the launcher/Bootstrap approach failed: autoloads from the game pack couldn't be created before the pack loaded).
4. **Patch the game pack's `project.binary`** inside the pck: `main_scene` from `uid://c5cgdds3ll0tb` → `Scenes/Main.tscn` (in-place byte patch; path is shorter, fits).
5. **Order matters**: `zipalign` FIRST (on the entries-only zip), THEN append pck, THEN `apksigner sign`. Never zipalign after appending (it drops the appended data).
6. Verify: `apksigner verify` (v2+v3), `unzip -t`, `zipalign -c 4`, check GDPC at end + EOCD at true end.

Signing (JKS keystore):
```
/usr/lib/jvm/jdk-11/bin/java -jar <build-tools>/lib/apksigner.jar sign \
  --ks byte.keystore --ks-pass pass:bytepass --ks-key-alias byte \
  --v1-signing-enabled true --v2-signing-enabled true --v3-signing-enabled true \
  --out out.apk in.apk
```

## 4. Toolchain (re-install in new chat; ~/.cache does NOT persist)

- **Godot editor**: `Godot_v4.6.2-stable_mono_linux_x86_64.zip` → https://github.com/godotengine/godot/releases/download/4.6.2-stable/Godot_v4.6.2-stable_mono_linux_x86_64.zip
- **Mono export templates**: `Godot_v4.6.2-stable_mono_export_templates.tpz` (extract `android_debug.apk`, `android_release.apk` into `~/.local/share/godot/export_templates/4.6.2.stable.mono/` — but since we hand-append the pck, only the APK template matters if you need a fresh base).
- **.NET 9 SDK**: https://dotnetcli.blob.core.windows.net/dotnet/Sdk/9.0.316/dotnet-sdk-9.0.316-linux-x64.tar.gz (tar-extract to a dir, use `/path/dotnet`). **JDK 11** exists at `/usr/lib/jvm/jdk-11` (jarsigner/apksigner).
- **Android build-tools 35**: https://dl.google.com/android/repository/build-tools_r35_linux.zip (contains zipalign + lib/apksigner.jar).
- **JDK 17** (for keytool): https://github.com/adoptium/temurin17-binaries/releases/download/jdk-17.0.13%2B11/OpenJDK17U-jdk_x64_linux_hotspot_17.0.13_11.tar.gz

**CRITICAL environment gotchas (learned the hard way):**
- The box has only **1.9 GB RAM** → create a **4 GB swapfile** with sudo before heavy builds: `sudo dd if=/dev/zero of=swapfile bs=1M count=4096 && sudo mkswap swapfile && sudo swapon swapfile`
- **`/tmp` is a tiny 993 MB tmpfs and silently breaks everything when full** (dotnet, godot, pip all fail weirdly). Clean it often; set `export TMPDIR=/home/user/.cache/tmp`.
- Set `export DOTNET_ROOT=<dotnet dir>` and add to PATH before `dotnet`/Godot mono builds.
- Godot editor settings: `~/.config/godot/editor_settings-4.6.tres` — set `export/android/android_sdk_path`, `java_sdk_path=/usr/lib/jvm/jdk-11`, `debug_keystore=byte.keystore`. A fake `platform-tools/adb` script is enough to pass Godot's validation.
- `project.binary` (if present in proj/) **overrides `project.godot`** — delete it when editing settings by hand.
- Godot Android export requires: `rendering/renderer/rendering_method=gl_compatibility` (+ `.mobile`), `window/per_pixel_transparency/allowed=false`, `window/size/transparent=false`, `viewport/transparent_background=false`, `textures/vram_compression/import_etc2_astc=true`, `application/config/icon=res://icon.svg`, and features must NOT contain "Forward Plus".

## 5. Why the launcher approach failed (context for new chat)

First APK used a `Bootstrap` scene that called `ProjectSettings.LoadResourcePack("user://Byte.pck")` etc. It failed because:
- On Android, `FileAccess` maps **every** path to APK `assets/` (strips leading `/`) → absolute external paths don't work.
- The game's autoloads (`ResourceCache`, `SignalEventBus`) are defined in the **game pack's** `project.binary`, so they can't exist until the pack loads — but Godot creates autoloads at startup from the launcher's project settings → "Failed to instantiate an autoload" → gray/black screen.
- Fix = **game pack as the main appended pack** (§3). Bootstrap is no longer used.

Also fixed along the way: renderer crash (SIGSEGV on GL thread — was "Forward Plus" feature tag), missing `resources.arsc` compression, `App not installed` (compressed native libs + bad alignment), keystore PKCS12 vs JKS (JDK11 apksigner can't read modern PKCS12 → use JKS).

## 6. Phone-side facts

- **Pixel 7** (Tensor G2 / Mali-G710), Android ~16 (API 33+), **rooted**, **Termux** available (root via `su`).
- Package: `com.desktop.byte` (app label "Byte"). Install: `pm install -r /data/local/tmp/byte.apk` after `cp` from `/storage/emulated/0/Download/` (SELinux blocks direct install from Download).
- Logs: `logcat -d | grep -iE "godot|mono|fatal|DEBUG|AndroidRuntime" | tail -40`. Godot also logs to `user://logs/godot/*.log` (= `/data/data/com.desktop.byte/files/logs/godot/`) — folder often empty in practice.
- User's Downloads contains `0.5.2 LIVE/` (the game zip extracted) → `Byte.pck` is at `/storage/emulated/0/Download/0.5.2 LIVE/Byte.pck` if ever needed.
- **User last report:** installed APK shows old launcher "Byte.pck not found" screen → **stale build**. Give them the newest 77 MB APK.

## 7. Next steps (in order)

1. **Have the user install `byte/Byte-Launcher.apk` (77 MB, current)** — uninstall first if signature mismatch: `pm uninstall com.desktop.byte` then `pm install /data/local/tmp/byte.apk`.
2. Ask: what happens? (a) Byte renders → great, test touch (tap, drag). (b) crash → `logcat -d | grep -iE "godot|mono|fatal" | tail -40`.
3. Likely fixes queue (already partially applied in decompiled source, verify they're in the current APK's DLL):
   - `Main._Ready`: desktop-only window calls guarded with `if (!_isMobile)` (done).
   - `DisplayServer.MouseGetPosition()` → `MobileMousePos()` touch-aware (done).
   - `_Input` handler: converts drag → right-button grab; taps handled by Godot's built-in `emulate_mouse_from_touch=true` (done; avoid double synthesis).
   - If EULA `ConfirmationMenu` blocks: game sets `GetTree().Paused=true` and instantiates a menu on first run — ensure touch can click "Accept" (built-in emulation should handle it).
   - `ScreenDataHandler` uses `DisplayServer.ScreenGetUsableRect` / taskbar math — harmless on Android (returns screen size) but `taskbarPos` may push her off-screen; if "she's gone", patch `taskbarPos` to `0` or guard on mobile.
4. If game crashes with a C# exception, logcat shows `ERROR: ... at: Scripts/...` — patch that line and rebuild (full pipeline §3).

## 8. Build pipeline cheat-sheet (new chat, from byte/)

```bash
# env
sudo dd if=/dev/zero of=/home/user/.cache/swapfile bs=1M count=4096 && sudo mkswap /home/user/.cache/swapfile && sudo swapon /home/user/.cache/swapfile
export TMPDIR=/home/user/.cache/tmp; mkdir -p $TMPDIR
export DOTNET_ROOT=/home/user/.cache/dotnet9; export PATH=$DOTNET_ROOT:$PATH
export XDG_CONFIG_HOME=/home/user/.cache/config XDG_DATA_HOME=/home/user/.cache/data

# 1. project
python3 rebuild_proj.py /home/user/byte/Byte.pck        # creates proj/ from pck_out + decompiled
rm -f proj/project.binary
sed -i 's|net8.0|net9.0|' proj/DesktopPets.csproj       # via setup_launcher.sh normally
# patch project.godot: gl_compatibility, transparency off, etc2_astc, icon, features (see §4)

# 2. C# build (must succeed, 0 errors)
cd proj && dotnet build -c Debug -v q

# 3. godot export (produces APK; exit 1 at sign step is OK)
godot --headless --path proj --export-debug Android out/Byte-Launcher.apk

# 4. slim → align → append game pck (patched project.binary inside) → sign
#    (see §3; patch main_scene inside pck's project.binary BEFORE appending)
```

## 9. Legal / courtesy notes

- User has the developers' permission to decompile **for personal use, NOT to share**. Do not publish the APK, source, or this handoff.
- 18+ NSFW game — content warnings apply.

## 10. Contact/context

- Original itch page: https://cindesh.itch.io/byte-desktop-pet (game + `Byte_SDK_0.5.zip` mod SDK).
- Game engine: Godot **4.6.2 stable mono**; assembly `DesktopPets.dll` (.NET 8 target on PC, recompiled net9 for Android arm64).
- The itch download flow (if re-download needed): GET page with cookies → POST `download_url` with `upload_id` (18434871 = Windows zip) → follow signed URL → extract `0.5.2 LIVE/Byte.pck`. Cookies.txt needed; may require age-gate POST first.
