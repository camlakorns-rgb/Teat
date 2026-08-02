# HANDOFF3 — Byte Desktop Pet → Android APK (Session 3)

**Date:** 2026-08-02  
**Previous sessions:** Handoff (GDRE recovery failed), Handoff2 (use original PCK + append to APK, got past 25% but 128MB workspace lock)  
**Workspace status:** 105MB APK in `byte/` (under 128MB limit), heavy toolchain in `.cache/` (excluded), decompiled + all build artifacts stored in GitHub repo via LFS  
**Repo:** https://github.com/camlakorns-rgb/Teat

---

## Goal for Handoff3 (DIFFERENT from 1 and 2)

**Previous goals:**
- **HANDOFF (1):** Goal was to get game to launch at all. Used GDRE Tools to recover project, re-exported. Failed with broken scene references (`Can't load dependency: res://Scenes/ActorWindow.tscn`) and stuck at 25% loading.
- **HANDOFF2 (2):** Goal was to fix 25% loading by using original Byte.pck directly + minimal launcher APK + append PCK. Focused heavily on compression/workspace size (keep APK <100MB, aggressive cleanup, slim *.a libs). Got game to launch past 25% but still had touch/window issues.

**HANDOFF3 (3) Goal:**
- **Port the game while NOT caring too much about compression like 2 and 1.** Previous sessions obsessed over keeping APK <100MB and workspace <128MB, which led to over-optimization and broken drag/bubble.
- **Game should ALWAYS be sent to git, no longer workspace.** APKs, PCKs, DLLs, decompiled source now live in GitHub repo `Teat` via Git LFS. Workspace only keeps tiny README + keystore + final APK pointer (or even just README if APK >100MB). Chat size is the new limit, not export size.
- Replicate **all desktop features 1:1** on Android: dragging, falling, walking, petting, dialogue, pause menu, terminal, items, etc., not just getting past loading screen.
- Optimize for **correctness**, not size: include .NET assemblies inside PCK at `.godot/mono/publish/arm64/`, keep System.*.dlls, don't trim aggressively if it breaks.

---

## What's Different from HANDOFF and HANDOFF2

### vs HANDOFF (1):
- **1 used GDRE recovery** → broken UIDs, 25% stuck. **3 uses original Byte.pck** like 2, so correct references.
- **1 kept everything in workspace** (`byte/Byte.pck` 35MB + decompiled + Godot 500MB) → locked snapshot. **3 keeps large files in `.cache/` (excluded) and pushes everything to GitHub LFS** (`decompiled/`, `*.pck`, `*.apk`, `*.dll` tracked via `.gitattributes`).

### vs HANDOFF2 (2):
- **2 focused on compression:** slim APK by removing `*.a` static libs (57MB saved), `PublishTrimmed`, keeping APK <100MB, aggressive `rm -rf` after each step. **3 says don't care too much about compression** — final APK 105MB slim is okay, 134MB non-slim also okay if stored in git. Use git LFS for >100MB files instead of fighting snapshot limit.
- **2 approach for PCK:** Append original Byte.pck between zip entries and central directory, update EOCD. **3 approach:** Replace `assets/assets.sparsepck` (24KB) inside launcher APK with combined PCK that contains **both game resources AND .NET assemblies** at `.godot/mono/publish/arm64/` (62.97 MB PCK → 105 MB APK). This fixes "Unable to find .NET assemblies" error that happened in v2-v3.
- **2 workspace rule:** Keep only final deliverables <100MB in `byte/`. **3 rule:** Game ALWAYS sent to git, no longer workspace. Workspace keeps only README + keystore (<1MB) or small pointer APK. Final APK lives in repo `Byte-Launcher-v*.apk` via LFS.
- **2 mobile patches minimal:** Only guarded TransparentBg/Borderless and added _isMobile bool. **3 patches full 1:1 replication:** `FollowMouse()` moves `Position` not window on mobile, `Walk()` uses Position, `GetThinnerCollisionBox()` uses Position + trueSize on mobile, `_Process` throw/gravity replicated for mobile using `Position` and `groundY = screenSize.Y - trueSize.Y`, `Character.UpdateDangle()` uses `MobileMousePos()`, bubble centered above head.

---

## CRITICAL: Workspace Size Management (Same as 2, but now with Git)

**DO NOT let workspace exceed 128 MB or 10,000 files.**

Previous session failed because kept 124MB APK + Godot 500MB + templates 1.1GB in workspace.

**Rules (Handoff3 updated):**
1. Use `/home/user/.cache/` for ALL large downloads — excluded from snapshots
2. Store decompiled source, PCKs, APKs, DLLs in **GitHub repo Teat via LFS**, not workspace. `git lfs track "*.pck" "*.apk" "*.zip" "*.dll"`
3. Workspace (`/home/user/byte/`) should ideally be <10MB: README + keystore, or tiny APK stub. But if needed, 105MB APK still under 128MB is okay (current `byte/` is 105MB).
4. Monitor: `du -sh /home/user --exclude=/home/user/.cache` and `du -sh /home/user/.cache/Teat`
5. Clean before each step: `rm -rf /home/user/.cache/godot /home/user/.cache/templates* /home/user/.cache/game* /home/user/.dotnet /home/user/.nuget`
6. For chat size, avoid pasting huge logs — tail them.

---

## URLs

**Game:**
- Page: https://cindesh.itch.io/byte-desktop-pet
- Windows zip (upload_id=18434871): 97 MB, File `0.5.2 LIVE/Byte.pck` 35 MB
- Download flow (itch.io requires cookies + POST):
    ```bash
    COOKIE_JAR=/home/user/.cache/cookies.txt
    curl -s -c "$COOKIE_JAR" -b "$COOKIE_JAR" "https://cindesh.itch.io/byte-desktop-pet" -o /dev/null
    FILE_URL=$(curl -s -c "$COOKIE_JAR" -b "$COOKIE_JAR" -X POST "https://cindesh.itch.io/byte-desktop-pet/file/18434871" | python3 -c "import json,sys; print(json.load(sys.stdin)['url'])")
    curl -L -o game.zip "$FILE_URL"
    unzip game.zip "0.5.2 LIVE/Byte.pck" "0.5.2 LIVE/data_DesktopPets_windows_x86_64/DesktopPets.dll" "0.5.2 LIVE/data_DesktopPets_windows_x86_64/GodotSharp.dll"
    ```

**Decompiler:**
- ILSpy ilspycmd: `dotnet tool install -g ilspycmd --version 8.2.0.7535` (requires .NET 6 runtime)
- .NET 6 runtime: https://dotnetcli.blob.core.windows.net/dotnet/Runtime/6.0.36/dotnet-runtime-6.0.36-linux-x64.tar.gz
- GDRE Tools (PCK extraction only, NOT project recovery): https://github.com/GDRETools/gdsdecomp/releases/download/v2.6.3/GDRE_tools-v2.6.3-linux.zip

**Toolchain:**
- Godot 4.6.2 mono: https://github.com/godotengine/godot/releases/download/4.6.2-stable/Godot_v4.6.2-stable_mono_linux_x86_64.zip
- Export templates: https://github.com/godotengine/godot/releases/download/4.6.2-stable/Godot_v4.6.2-stable_mono_export_templates.tpz (contains android_debug.apk 119MB, android_release.apk 98MB, android_source.zip 200MB)
- .NET 9 SDK: https://dotnetcli.blob.core.windows.net/dotnet/Sdk/9.0.316/dotnet-sdk-9.0.316-linux-x64.tar.gz
- Android build-tools 35: https://dl.google.com/android/repository/build-tools_r35_linux.zip
- JDK 11: `/usr/lib/jvm/jdk-11` exists on box

**GitHub Repo (stores everything):**
- https://github.com/camlakorns-rgb/Teat
- LFS tracked: `*.apk`, `*.pck`, `*.zip`, `*.dll` via `.gitattributes`
- Contains: `decompiled/` (77 C# files), `byte.keystore`, `HANDOFF.md`, `Handoff2.md`, `HANDOFF3.md`, `patched/Scripts/Main.cs`, APKs v3-v8

---

## Build Pipeline (Handoff3 - Optimized for Correctness, not Size)

    # Setup (all in .cache)
    export TMPDIR=/home/user/.cache/tmp
    export DOTNET_ROOT=/home/user/.cache/dotnet9
    export XDG_DATA_HOME=/home/user/.cache/data
    export XDG_CONFIG_HOME=/home/user/.cache/config
    mkdir -p $TMPDIR
    export PATH=$DOTNET_ROOT:$PATH:$HOME/.dotnet/tools

    # 1. Download & extract game + decompile
    # (use curl flow above)
    ilspycmd DesktopPets.dll -p -o decompiled/ -r GodotSharp.dll

    # 2. Restructure decompiled files to original ScriptPath subfolders
    # (Main.cs res://Scripts/Main.cs, ActorWindow.cs res://Scripts/CharacterScripts/ActorWindow.cs, etc.)
    # See build_final.py in repo

    # 3. Strip generated Godot glue
    # - Remove MethodName/PropertyName/SignalName inner classes
    # - Remove [EditorBrowsable] methods: GetGodotMethodList, InvokeGodotClassMethod, etc.
    # - Remove backing_ fields, public event, EmitSignal methods
    # - Add partial keyword: public partial class ...

    # 4. Apply mobile patches (see below) for 1:1 replication

    # 5. Create minimal project.godot
    # config/features=PackedStringArray("4.6", "C#", "GL Compatibility")
    # rendering/renderer/rendering_method=gl_compatibility
    # Textures: rendering/textures/vram_compression/import_etc2_astc=true
    # window transparent false, per_pixel_transparency false, viewport transparent false
    # input_devices emulate_touch_from_mouse=true, emulate_mouse_from_touch=true

    # 6. dotnet build -c Debug (needs 4GB swapfile, TMPDIR set, /tmp cleaned)

    # 7. Patch project.binary inside Byte.pck
    # Change main_scene uid://c5cgdds3ll0tb -> Scenes/Main.tscn (16 chars vs 19, fits with padding)
    # Change features Forward Plus -> GL Compatibility (PackedStringArray)
    # Set transparent false, per_pixel false, etc.

    # 8. Merge .NET assemblies into PCK
    # Extract 175 files from assets/.godot/mono/publish/arm64/ in launcher APK
    # Replace DesktopPets.dll with newly built one (668KB with mobile patches)
    # Add them to Byte.patched.pck at .godot/mono/publish/arm64/ → new PCK 62.97 MB

    # 9. Export launcher APK with Godot --headless --export-debug Android
    # (non-gradle, arm64 only, 100MB)

    # 10. Replace assets inside APK
    # - assets/assets.sparsepck (24KB) -> Byte.patched.withmono.pck (62.97MB)
    # - assets/project.binary (938B) -> project.binary.patched (12787B)
    # - Remove *.a static libs (saves 57MB)
    # - Ensure resources.arsc STORED (compress_type=0)

    # 11. zipalign + sign
    # zipalign -f 4 in.apk aligned.apk
    # apksigner sign --ks byte.keystore --ks-pass pass:bytepass --ks-key-alias byte --out final.apk aligned.apk

    # 12. Store in git, NOT workspace (if >100MB)
    # cp final.apk Teat/Byte-Launcher-vX.apk
    # git lfs track "*.apk" ; git add ; git commit ; git push

---

## Mobile Patches (Handoff3 Final - 1:1 Replication)

**Main.cs:**

    public static bool _isMobile = false;
    private static bool _isMobileChecked = false;
    public static Vector2I _lastTouchPos = new Vector2I(0,0);
    public Vector2I MobileMousePos() { if (_isMobile && _lastTouchPos != Zero) return _lastTouchPos; return DisplayServer.MouseGetPosition(); }

    public override void _Input(InputEvent @event) {
        if (_isMobile) {
            if (@event is InputEventScreenTouch touch) {
                _lastTouchPos = (Vector2I)touch.Position;
                bool hasPoint = GetThinnerCollisionBox().HasPoint(_lastTouchPos);
                if (touch.Pressed && hasPoint) { Input.ActionPress("Move"); Input.ActionPress("Pet"); }
                else { Input.ActionRelease("Move"); Input.ActionRelease("Pet"); }
            } else if (@event is InputEventScreenDrag drag) {
                _lastTouchPos = (Vector2I)drag.Position;
                Input.ActionPress("Move");
            }
        }
        base._Input(@event);
    }

**_Ready():**
- Detect mobile: OS.HasFeature("mobile") || "android"
- Guard desktop window calls: if (!_isMobile) TransparentBg, Borderless, AlwaysOnTop, etc.
- On mobile center: Position = screenSize/2 - trueSize/2, Y = screenSize.Y - trueSize.Y

**FollowMouse() (FIXES stuck in middle + only left-right):**
- Desktop: moves mainWindow.Position clamped to leftmostScreenX/totalScreenWidth + taskbarPos
- Mobile: moves Position (Node2D) not window: newPos = mousePos + mouseOffset clamped 0..screenSize-trueSize

**Walk() (1:1):**
- Uses (Vector2I)Position on mobile, mainWindow.Position on desktop

**GetThinnerCollisionBox() (FIXES can't move at all):**
- Old used mainWindow.Position at 0,0 → box at top-left, touch failed
- New: if _isMobile, box centered on Position + trueSize, 33% width

**_Process() throw & gravity (FIXES floating):**
- Desktop: windowVelocity gravity 1400, bounce -0.55, taskbarPos
- Mobile: same but groundY = screenSize.Y - trueSize.Y, Position = pos + velocity*delta, landing calls BeginLand()

**Character.cs:**
- StartDangle/UpdateDangle use MobileMousePos() on mobile for throw velocity

**AttachObjWindow.cs (FIXES bubble moving + left side):**
- On mobile, bubble centered above parent using Main.Instance.Position, not parentWindow which is at 0,0
- Previously bubble flipped to left side if would go off right edge (isLeft logic) → now bypassed on mobile, always centered above with Y-20

---

## Known Issues (Current as of v8)

1. **Stuck in middle:** Fixed in v7/v8 by using Position not window, but still reported in latest test. Root cause: ScreenDataHandler taskbarPos may be 0 on Android, and ground snap MoveToward may be too slow. Current v8 sets initial Position to screen - trueSize, and ground snap every frame. If still middle, increase groundY or make snap instant.
2. **Chat bubble moving:** Bubble is AttachObjWindow that is child of Main. When Main moves, bubble should follow (desired), but user reports bubble moves for some reason (maybe jitters). Our FollowParent on mobile sets Position each frame to centered above Main. If Main moves, bubble moves too — expected, but if Main is at 0,0 and bubble uses Main.Position, bubble will move with drag (which is actually moving bubble when moving her). **Bug: When moving her it's actually moving her talk bubble.** Indeed FollowParent uses parentWindow = mainWindow (at 0,0) but we override to use Main.Instance.Position for mobile, so bubble Position = MainPos + offset. When we drag Main, bubble moves with it (good), but if we drag bubble itself? AttachObjWindow is Window, its own drag may be interpreted as moving bubble?
3. **Drag only left-right (old):** Fixed by mouseOffset using MobileMousePos not GetGlobalMousePosition.
4. **Floating (old):** Fixed by ground snap and throw physics.
5. **resources.arsc must be STORED**
6. **.NET assemblies not found:** Fixed by merging publish folder into PCK.

---

## Testing

Install v8 (latest):

    adb push Byte-Launcher-v8-collisionfix.apk /sdcard/Download/
    su -c "cp /sdcard/Download/Byte-Launcher-v8-collisionfix.apk /data/local/tmp/byte.apk"
    su -c "pm uninstall com.desktop.byte; pm install /data/local/tmp/byte.apk"
    logcat -d | grep -iE "godot|mono|fatal" | tail -60

Expected (v8):
- Spawns center-bottom, on ground, not floating
- Drag anywhere X+Y → follows finger
- Release high → falls, bounces, lands, walks
- Tap → pet + bubble centered above

Current bug report:
- Stuck in middle
- Bubble moving
- Moving her moves bubble (should be separate? Actually bubble should follow her, but not be draggable itself)

---

## Summary

**Handoff2 got past 25% loading** by using original PCK directly. **Handoff3's goal is different: don't care about compression, store everything in git, replicate 1:1 features.** We now store decompiled + APKs + PCKs in GitHub Teat via LFS, workspace stays under 128MB (currently 105MB APK + keystore). 

**What's different:**
- 1: GDRE recovery → broken, 2: original PCK + slim → worked but touch broken, 3: original PCK + merged .NET assemblies + full mobile physics + git storage
- 2 cared about compression (<100MB), 3 doesn't (105MB okay, 134MB non-slim also okay if in git)
- 2 kept game in workspace, 3 always sends game to git (APKs via LFS)

**Next steps for Handoff4:**
- Fix GetThinnerCollisionBox 100% width on mobile for easier drag (33% too small)
- Make bubble non-draggable (MousePassthrough true)
- Make ground snap instant, not MoveToward, to prevent floating in middle
- Test with logcat for "Position" values

