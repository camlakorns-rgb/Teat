# STEP BY STEPS — How the Byte Desktop Pet Android APK is built

**Purpose:** a fresh AI session (or human) can rebuild the APK from zero by following this guide top to bottom.
**Applies to:** Byte Desktop Pet 0.5.2 (Godot 4.6.2 mono / C#) → `Byte-Launcher-vXX.apk` for Android arm64.
**Current shipped build:** v11 (`Byte-Launcher-v11-bugfix.apk`, commit `d43798b`).
**Repo:** https://github.com/camlakorns-rgb/Teat (git-lfs: `*.apk *.pck *.zip *.dll`).

---

## The pipeline at a glance

```
game zip (itch.io)
   │ unzip  → Byte.pck (original game pack, 35 MB) + DesktopPets.dll (original C#)
   ▼
ILSpy decompile DLL → decompiled/*.cs (77 files, raw w/ generator glue)
   ▼
strip_generated.py  → stripped/*.cs (glue removed, `partial` added)
   ▼
make_project.py     → src/Scripts/** (stripped + overlay repo patched/Scripts = v11 mobile state)
   ▼
v11_patch.py        → src patched (bugfixes)  →  proj/Scripts (copied)
   ▼
dotnet build        → DesktopPets.dll (net9.0, Debug)
   ▼
godot export        → launcher Byte-Launcher.apk (engine + .NET runtime + empty-ish pack)
   ▼
GDRE pck-create     → Byte-v11.pck = original Byte.pck contents + patched project.binary
                     + our DesktopPets.dll + 173 framework DLLs at .godot/mono/publish/arm64/
   ▼
assemble_apk.py     → replace assets/assets.sparsepck + assets/project.binary in launcher,
                     delete *.a libs, rezip (STORED vs DEFLATE rules), zipalign
   ▼
apksigner sign      → Byte-Launcher-v11-bugfix.apk  (JKS: byte/bytepass)
```

---

## STEP 0 — Workspace strategy (IMPORTANT, read first)

- The sandbox's **workspace snapshot (~/home/user) persists between chats but is capped (~128 MB / 10k files)** and the **`.cache` directory does NOT persist** between sessions. Everything big goes in `/home/user/.cache/`; only tiny files live in the workspace.
- Keep the git clone at **`/home/user/teat/repo`** in the workspace (it's small: sources + docs + LFS pointers). Run heavy builds from `/home/user/.cache`.
- Monitor: `du -sh /home/user /home/user/.cache`.
- 4 GB swapfile is REQUIRED for dotnet (box has 1.9 GB RAM).

---

## STEP 1 — Download & install the toolchain (all in /home/user/.cache)

```bash
mkdir -p /home/user/.cache/dl /home/user/.cache/tools && cd /home/user/.cache/dl

# --- parallel downloads ---
curl -sL -o git-lfs.tar.gz https://github.com/git-lfs/git-lfs/releases/download/v3.5.1/git-lfs-linux-amd64-v3.5.1.tar.gz
curl -sL -o dotnet9.tgz https://dotnetcli.blob.core.windows.net/dotnet/Sdk/9.0.316/dotnet-sdk-9.0.316-linux-x64.tar.gz
curl -sL -o godot.zip https://github.com/godotengine/godot/releases/download/4.6.2-stable/Godot_v4.6.2-stable_mono_linux_x86_64.zip
curl -sL -o templates.tpz https://github.com/godotengine/godot/releases/download/4.6.2-stable/Godot_v4.6.2-stable_mono_export_templates.tpz
curl -sL -o buildtools.zip https://dl.google.com/android/repository/build-tools_r35_linux.zip
curl -sL -o gdre.zip https://github.com/GDRETools/gdsdecomp/releases/download/v2.6.3/GDRE_tools-v2.6.3-linux.zip

# --- extract ---
cd /home/user/.cache
mkdir -p dotnet9 godot tools/buildtools gdre
tar xzf dl/dotnet9.tgz -C dotnet9
unzip -q -o dl/godot.zip -d godot
unzip -q -o dl/buildtools.zip -d tools/buildtools
unzip -q -o dl/gdre.zip -d gdre
tar xzf dl/git-lfs.tar.gz
sudo cp git-lfs-3.5.1/git-lfs /usr/local/bin/ || cp git-lfs-3.5.1/git-lfs /usr/local/bin/
git lfs install --skip-repo

# --- swapfile (needed for dotnet) ---
sudo dd if=/dev/zero of=/home/user/.cache/swapfile bs=1M count=4096
sudo mkswap /home/user/.cache/swapfile && sudo swapon /home/user/.cache/swapfile

# --- ilspycmd (needs an SDK present to install; roll forward to run) ---
export DOTNET_ROOT=/home/user/.cache/dotnet9
export PATH=/home/user/.cache/dotnet9:/usr/local/bin:$PATH
export DOTNET_CLI_TELEMETRY_OPTOUT=1 DOTNET_NOLOGO=1 DOTNET_ROLL_FORWARD=Major
export DOTNET_CLI_HOME=/home/user/.cache/dotnet-cli-home
dotnet tool install ilspycmd --version 8.2.0.7535 --tool-path /home/user/.cache/tools/ilspy
```

---

## STEP 2 — Get the game files (itch.io)

```bash
cd /home/user/.cache
curl -s -c dl/cookies.txt -b dl/cookies.txt "https://cindesh.itch.io/byte-desktop-pet" -o /dev/null
FILE_URL=$(curl -s -b dl/cookies.txt -c dl/cookies.txt -X POST "https://cindesh.itch.io/byte-desktop-pet/file/18434871" \
  | python3 -c "import json,sys; print(json.load(sys.stdin)['url'])")
curl -sL -o dl/game-real.zip "$FILE_URL"
mkdir -p game
unzip -o -q dl/game-real.zip "0.5.2 LIVE/Byte.pck" -d game
# Byte.pck = the ORIGINAL game pack (35 MB) — never re-export it, never convert scenes,
# use it as-is; this is what keeps all scene references valid (HANDOFF1 lesson).
```

---

## STEP 3 — Decompile the original DLL (reference source)

```bash
cd /home/user/.cache
mkdir -p game2
unzip -o -q dl/game-real.zip "0.5.2 LIVE/data_DesktopPets_windows_x86_64/DesktopPets.dll" \
  "0.5.2 LIVE/data_DesktopPets_windows_x86_64/GodotSharp.dll" -d game2
rm -rf decompiled && mkdir -p decompiled
/home/user/.cache/tools/ilspy/ilspycmd "game2/0.5.2 LIVE/data_DesktopPets_windows_x86_64/DesktopPets.dll" \
  -p -o /home/user/.cache/decompiled \
  -r "game2/0.5.2 LIVE/data_DesktopPets_windows_x86_64/"
# → 77 .cs files. The repo already stores this output in decompiled/ (committed, not LFS).
```

---

## STEP 4 — Strip generator glue

`build/strip_generated.py` (in the repo) converts raw ILSpy output into compilable code:

- removes `public new class MethodName/PropertyName/SignalName` nested classes
- removes `[EditorBrowsable(EditorBrowsableState.Never)]` generated members (`GetGodotMethodList`, `InvokeGodotClassMethod`, `GetGodotPropertyList`, `SaveGodotObjectData`, `RaiseGodotClassSignalCallbacks`, …)
- removes `backing_*` fields, event add/remove accessors, generated `EmitSignalX()` methods
- adds `partial` to class declarations (so the Godot.NET.Sdk source generators re-create the glue at build time)

```bash
cd /home/user/.cache
rm -rf v9/stripped && mkdir -p v9/stripped
python3 /home/user/teat/repo/build/strip_generated.py /home/user/teat/repo/decompiled /home/user/.cache/v9/stripped
# sanity: grep -l "backing_\|GetGodotMethodList" v9/stripped/*.cs  →  must print nothing
```

---

## STEP 5 — Assemble the project source

`build/make_project.py` (repo):
1. copies stripped files into `src/Scripts/<res://subfolder>/` using the exact folder map from the original project (e.g. `Main.cs → Scripts/`, `Character.cs → Scripts/CharacterScripts/`, `ItemWindow.cs → Scripts/ItemScripts/`, …)
2. **overlays the repo's `patched/Scripts/` files** — these already contain ALL mobile patches (v9 drag/throw, v10 feature port, v11 bugfixes). They are the source of truth for mobile behavior; the stripped files are just the untouched desktop baseline.

```bash
cd /home/user/.cache
rm -rf v9/src
python3 /home/user/teat/repo/build/make_project.py
# prints "overlaid <file>" for each patched file
```

---

## STEP 6 — Apply the current patch level

The repo tracks two patch scripts:
- `build/v10_patch.py` — full feature port (button bar wiring, item/NPC touch routing, minigame controls, terminal SEND). **Only for rebuilding a v10; v11 supersedes it.**
- `build/v11_patch.py` — current bugfixes on top of the v10 sources: tap-buttons + release-on-hide, terminal CLOSE, magnifier touch-follow + CLOSE/±, `ItemWindow._Input` self-touch, `ActorWindow._Input` tap→Pet, spawner timers + resource guard + mobile spawn position, `ToggleDespawnMobile`, dino `IsMouseOver`.

```bash
cd /home/user/.cache
python3 /home/user/teat/repo/build/v11_patch.py
# prints "✓ <tag>" per applied patch; FATAL + aborts if an anchor doesn't match exactly.
# v11_patch.py also copies Scripts → proj/Scripts at the end.
```

**Project scaffolding** (needed once per fresh cache; all files are committed in the repo under nothing — recreate by hand or copy from a previous build):

```bash
mkdir -p /home/user/.cache/v9/proj/out
cd /home/user/.cache/v9/proj
# write: project.godot (features "4.6","C#","GL Compatibility", gl_compatibility renderer,
#        transparent off, emulate_touch_from_mouse + emulate_mouse_from_touch true, etc2_astc)
# write: DesktopPets.csproj  → Sdk="Godot.NET.Sdk/4.6.2", TargetFramework net9.0,
#        AssemblyName DesktopPets, PublishTrimmed false
# write: DesktopPets.sln    (Godot export needs it)
# write: export_presets.cfg → Android preset, non-gradle (gradle_build/use_gradle_build=false),
#        gradle_build/compress_native_libraries=false (else export refuses), arm64-v8a only,
#        package/unique_name=com.desktop.byte, version/code incremented per build
# write: icon.svg, main.tscn + Bootstrap.cs (launcher main scene)
```

---

## STEP 7 — C# build

```bash
export HOME=/home/user/.cache/home
export DOTNET_ROOT=/home/user/.cache/dotnet9
export PATH=/home/user/.cache/dotnet9:/usr/local/bin:$PATH
export DOTNET_CLI_TELEMETRY_OPTOUT=1 DOTNET_NOLOGO=1
export DOTNET_CLI_HOME=/home/user/.cache/dotnet-cli-home
export NUGET_PACKAGES=/home/user/.cache/nuget
export TMPDIR=/home/user/.cache/tmp        # /tmp is a tiny tmpfs and breaks builds
mkdir -p $TMPDIR
cd /home/user/.cache/v9/proj
dotnet build -c Debug -v q
# MUST end with "Build succeeded. 0 Error(s)"
# artifact: .godot/mono/temp/bin/Debug/DesktopPets.dll
```

---

## STEP 8 — Godot editor env + export the launcher APK

```bash
# templates (must live under the XDG data dir used by the editor)
python3 - <<'EOF'
import zipfile, shutil
z = zipfile.ZipFile('/home/user/.cache/dl/templates.tpz')
for n in ['templates/android_debug.apk','templates/android_release.apk']:
    out = '/home/user/.cache/home/.local/share/godot/export_templates/4.6.2.stable.mono/' + n.split('/')[-1]
    with z.open(n) as s, open(out,'wb') as d: shutil.copyfileobj(s,d)
EOF

# fake Android SDK: Godot only validates paths; real build-tools are used later
mkdir -p /home/user/.cache/fakesdk/platform-tools /home/user/.cache/fakesdk/build-tools/35.0.0
printf '#!/bin/bash\necho "Android Debug Bridge version 1.0.41"\nexit 0\n' > /home/user/.cache/fakesdk/platform-tools/adb
chmod +x /home/user/.cache/fakesdk/platform-tools/adb
BT=$(find /home/user/.cache/tools/buildtools -maxdepth 2 -name zipalign -printf "%h\n" | head -1)
cp -r "$BT"/. /home/user/.cache/fakesdk/build-tools/35.0.0/

# editor settings
mkdir -p $HOME/.config/godot
cat > $HOME/.config/godot/editor_settings-4.6.tres <<'EOF'
[gd_resource type="EditorSettings" format=3]
[resource]
export/android/android_sdk_path = "/home/user/.cache/fakesdk"
export/android/java_sdk_path = "/usr/lib/jvm/jdk-11"
export/android/debug_keystore = "/home/user/teat/repo/byte.keystore"
export/android/debug_keystore_pass = "bytepass"
export/android/debug_keystore_alias = "byte"
export/android/force_system_user = false
export/android/always_export_binary = false
EOF

# export (out/ dir MUST exist first)
mkdir -p /home/user/.cache/v9/proj/out
export XDG_CONFIG_HOME=/home/user/.cache/config
export XDG_DATA_HOME=/home/user/.cache/data
mkdir -p $XDG_CONFIG_HOME $XDG_DATA_HOME
GODOT=/home/user/.cache/godot/Godot_v4.6.2-stable_mono_linux_x86_64/Godot_v4.6.2-stable_mono_linux.x86_64
$GODOT --headless --path /home/user/.cache/v9/proj --export-debug Android /home/user/.cache/v9/proj/out/Byte-Launcher.apk
# exit 0 = success (the "EditorSettings not instantiated" ERROR line at the end is harmless)
# → launcher APK ~104 MB: Godot engine + .NET runtime + our assemblies + a tiny empty pack
```

---

## STEP 9 — Build the merged game PCK

The final APK's `assets/assets.sparsepck` must contain the **original game resources** (so every scene reference stays valid) PLUS the patched `project.binary` PLUS our .NET assemblies:

```bash
cd /home/user/.cache/v9
rm -rf pck_src && mkdir -p pck_src
# 1. original game resources
/home/user/.cache/gdre/gdre_tools.x86_64 --headless \
  --extract="/home/user/.cache/game/0.5.2 LIVE/Byte.pck" --output=/home/user/.cache/v9/pck_src \
  2>&1 | grep "Extracted [0-9]* files"      # → 3342 files
# 2. patched project.binary (from the LAST good APK — md5 a33cb5892eb650a5b26a9541ba70b9ad)
unzip -o -q /home/user/.cache/v9/proj/out/Byte-Launcher.apk "assets/project.binary" -d /tmp/pb
cp /tmp/pb/assets/project.binary pck_src/project.binary
# 3. .NET assemblies from our export (DesktopPets.dll + 173 runtime DLLs)
mkdir -p pck_src/.godot/mono/publish/arm64
unzip -o -q /home/user/.cache/v9/proj/out/Byte-Launcher.apk "assets/.godot/mono/publish/arm64/*" -d /tmp/exp
cp -r /tmp/exp/assets/.godot/mono/publish/arm64/* pck_src/.godot/mono/publish/arm64/
# 4. create merged PCK (v3, engine 4.6.2) — out dir must exist
mkdir -p out
/home/user/.cache/gdre/gdre_tools.x86_64 --headless \
  --pck-create=/home/user/.cache/v9/pck_src --pck-version=3 --pck-engine-version=4.6.2 \
  --output=/home/user/.cache/v9/out/Byte-v11.pck
# → ~66 MB, 3517 files
```

**IMPORTANT (HANDOFF7 finding):** the patched `project.binary` sets main_scene to `Scenes/Main.tscn` directly, which **skips the game's LoadingPopup boot scene — the only caller of `ResourceCache.LoadData()`**. On Android the resource cache (items/NPCs/gallery/recipes/minigames/terminal content) therefore never loads. The v12 fix (not yet built) is to call `ResourceCache.Instance.LoadData()` from `Main._Ready` on mobile.

---

## STEP 10 — Assemble the final APK

`build/assemble_apk.py` (repo):
1. extracts the launcher APK
2. deletes all `*.a` static libs (13 files, ~57 MB)
3. replaces `assets/assets.sparsepck` ← merged PCK, `assets/project.binary` ← patched
4. re-zips with the exact compression layout that Android requires:
   - **STORED:** `resources.arsc`, `assets/assets.sparsepck`, mono `.so/.jar/.dex` libs
   - **DEFLATED:** everything else, including `libc++_shared.so` + `libgodot_android.so` (manifest has `extractNativeLibs=true`)
5. `zipalign -f 4`

```bash
cd /home/user/.cache/v9
# edit assemble_apk.py: PCK = out/Byte-v11.pck, FINAL = out/Byte-Launcher-v11-bugfix.apk
python3 /home/user/teat/repo/build/assemble_apk.py
# → ~110 MB, aligned
```

---

## STEP 11 — Sign

```bash
BT=/home/user/.cache/fakesdk/build-tools/35.0.0
KS=/home/user/teat/repo/byte.keystore        # JKS, alias byte, pass bytepass
cd /home/user/.cache/v9/out
/usr/lib/jvm/jdk-11/bin/java -jar $BT/lib/apksigner.jar sign \
  --ks $KS --ks-pass pass:bytepass --ks-key-alias byte \
  --v1-signing-enabled true --v2-signing-enabled true --v3-signing-enabled true \
  --out Byte-Launcher-v11-bugfix-signed.apk Byte-Launcher-v11-bugfix.apk
# same keystore every build → `pm install -r` upgrades without uninstall
```

---

## STEP 12 — Verify (all must pass)

```bash
FINAL=/home/user/.cache/v9/out/Byte-Launcher-v11-bugfix-signed.apk
BT=/home/user/.cache/fakesdk/build-tools/35.0.0

$BT/zipalign -c 4 $FINAL && echo ALIGNED-OK
unzip -t $FINAL > /dev/null && echo ZIP-OK
/usr/lib/jvm/jdk-11/bin/java -jar $BT/lib/apksigner.jar verify --verbose $FINAL   # v2+v3 true
unzip -p $FINAL assets/project.binary | md5sum    # → a33cb5892eb650a5b26a9541ba70b9ad

# build marker inside BOTH shipped DLLs (the v8 lesson: stale DLLs shipped silently)
python3 - <<'EOF'
import subprocess, zipfile, io
m = "V11_BUGFIX_BUILD".encode('utf-16-le')   # .NET strings are UTF-16 in the binary
z = zipfile.ZipFile("$FINAL".replace("$FINAL", "/home/user/.cache/v9/out/Byte-Launcher-v11-bugfix-signed.apk"))
loose = z.read("assets/.godot/mono/publish/arm64/DesktopPets.dll")
print("LOOSE marker:", m in loose)
open("/tmp/sparse.pck","wb").write(z.read("assets/assets.sparsepck"))
EOF
# extract DesktopPets.dll out of the PCK (GDRE --extract --include="DesktopPets.dll") and check marker there too

# boot test: merged PCK must load in Godot 4.6.2 (script loads pack, prints load("res://Scenes/Main.tscn"))
```

---

## STEP 13 — Ship to git (per handoff rule: game ALWAYS goes to git)

```bash
export PATH=/usr/local/bin:$PATH
cd /home/user/.cache
rm -rf teat-git
GIT_LFS_SKIP_SMUDGE=1 git clone --depth 1 https://github.com/camlakorns-rgb/Teat.git teat-git
cd teat-git
cp /home/user/.cache/v9/out/Byte-Launcher-v11-bugfix-signed.apk Byte-Launcher-v11-bugfix.apk
cp /home/user/.cache/v9/src/Scripts/Main.cs /home/user/.cache/v9/src/Scripts/Character.cs \
   ... (all 12 patched files) → patched/Scripts/
cp /home/user/teat/repo/build/*.py build/
git add -A
git commit -m "..."
git push https://<PAT>@github.com/camlakorns-rgb/Teat.git HEAD:main
# APK goes through LFS (git lfs install + .gitattributes *.apk)
```

---

## TROUBLESHOOTING (every gotcha hit so far)

| Symptom | Cause / fix |
|---|---|
| dotnet OOM / weird failures | missing swap → create 4 GB swapfile; set `TMPDIR=/home/user/.cache/tmp` (/tmp is 993 MB tmpfs) |
| ilspycmd "SDK not found" | install with dotnet9 SDK; run with `DOTNET_ROLL_FORWARD=Major` |
| Godot export "Unable to open Android build-tools" | real `build-tools` dir must exist at the fakesdk path (copy from r35 zip); fake `platform-tools/adb` script passes validation |
| "No export template found at …/data/godot/export_templates" | templates must be under `$XDG_DATA_HOME/godot/export_templates/4.6.2.stable.mono/` (both home and XDG data locations) |
| "Compress Native Libraries is only valid with Use Gradle Build" | set `gradle_build/compress_native_libraries=false` |
| "Target folder does not exist" | `mkdir -p proj/out` before export |
| GDRE pck-create "Error opening PCK file: <dir>" | `--output` must point to an existing directory or full .pck path with the dir existing |
| GDRE --extract writes only .dll with --include="*.dll" | fine; use `--include="DesktopPets.dll"` or `"*.dll"` |
| marker not found with `grep -a` | .NET strings are UTF-16LE — search with `b"MARKER".encode('utf-16-le')` |
| v3/v4/v5 APKs identical (same LFS oid) | those "fix" uploads were never rebuilt — always verify the marker inside the shipped APK |
| `App not installed` on phone | resources.arsc must be STORED; zipalign before signing; sign with v2/v3 |
| `pm install` from /sdcard blocked | copy to /data/local/tmp first: `su -c "cp /sdcard/Download/x.apk /data/local/tmp/byte.apk"` |
| stale DLL in APK | the PCK DLL and the loose assets DLL must BOTH be the new build (v8 shipped a stale loose DLL) |
| C# string markers | add `public static readonly string VXX_BUILD = "...";` to Main.cs, verify UTF-16 in both DLLs |

---

## Current status & known pending work (as of HANDOFF7)

- **Shipped:** v11 — drag/throw/pet/sit/menu work; terminal opens/closes; magnifier reachable; item & NPC touch logic in place.
- **Known broken (root cause found, NOT fixed yet):** `ResourceCache.LoadData()` never runs on Android (LoadingPopup skipped) → no item/NPC spawns, empty gallery, broken recipe list, dead terminal content commands.
- **Confirmed bug:** `ToggleDespawnMobile` hides Byte but the else-branch never restores `Visible = true` (AWAY = one-way disappear).
- **Unverified:** magnifier lens viewport-texture rendering on GL mobile; terminal soft keyboard.
- Full details + recommended fix order: `HANDOFF7.md` §2–§8.
