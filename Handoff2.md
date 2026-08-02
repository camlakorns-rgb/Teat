# HANDOFF2 — Byte Desktop Pet → Android APK (Session 2)

**Date:** 2026-08-02  
**Previous session:** Got game to launch but stuck at 25% loading  
**Workspace status:** Locked (over 128 MB limit) — start fresh

---

## CRITICAL: Workspace Size Management

**DO NOT let workspace exceed 128 MB or 10,000 files.**

Previous session failed because:
- Built 124 MB APK
- Kept large build artifacts (Godot 500+ MB, templates 1.1 GB)
- Workspace snapshot failed, locked entire session

**Rules:**
1. Use `/home/user/.cache/` for ALL large downloads — excluded from snapshots
2. Delete build artifacts immediately after use
3. Keep only final deliverables: APK (<100 MB), keystore, scripts
4. Monitor: `du -sh /home/user/` frequently
5. Clean before each step: `rm -rf .cache/godot .cache/templates* byte/proj`

---

## What's Different from HANDOFF.md

### Previous approach (FAILED):
- Used GDRE Tools to recover project from Byte.pck
- Re-exported with Godot (repacked all resources)
- Result: Broken scene references (`Can't load dependency: res://Scenes/ActorWindow.tscn`)

### New approach (USE THIS):
- **Use original Byte.pck directly** — don't recover/re-export
- Build minimal launcher APK with recompiled .NET code
- **Append original Byte.pck** to APK (between zip entries and central directory)
- Patch main_scene in PCK's project.binary (UID → path)
- Game loads from original PCK (correct references) + our arm64 .NET assemblies

---

## URLs

**Game:**
- Page: https://cindesh.itch.io/byte-desktop-pet
- Windows zip (upload_id=18434871): 97 MB
- Download flow:
    COOKIE_JAR=/home/user/.cache/cookies.txt
    curl -s -c "$COOKIE_JAR" -b "$COOKIE_JAR" "https://cindesh.itch.io/byte-desktop-pet" -o /dev/null
    FILE_URL=$(curl -s -c "$COOKIE_JAR" -b "$COOKIE_JAR" -X POST "https://cindesh.itch.io/byte-desktop-pet/file/18434871" | python3 -c "import json,sys; print(json.load(sys.stdin)['url'])")
    curl -L -o game.zip "$FILE_URL"

**Decompiler:**
- ILSpy: `dotnet tool install -g ilspycmd --version 8.2.0.7535`
- GDRE Tools (for PCK extraction only, NOT project recovery): https://github.com/GDRETools/gdsdecomp/releases/download/v2.6.3/GDRE_tools-v2.6.3-linux.zip

**Toolchain:**
- Godot 4.6.2 mono: https://github.com/godotengine/godot/releases/download/4.6.2-stable/Godot_v4.6.2-stable_mono_linux_x86_64.zip
- Export templates: https://github.com/godotengine/godot/releases/download/4.6.2-stable/Godot_v4.6.2-stable_mono_export_templates.tpz
- .NET 9 SDK: https://dotnetcli.blob.core.windows.net/dotnet/Sdk/9.0.316/dotnet-sdk-9.0.316-linux-x64.tar.gz
- .NET 6 runtime (for ilspycmd): https://dotnetcli.blob.core.windows.net/dotnet/Runtime/6.0.36/dotnet-runtime-6.0.36-linux-x64.tar.gz
- Android build-tools 35: https://dl.google.com/android/repository/build-tools_r35_linux.zip

---

## Build Pipeline (Optimized)

    # Setup (all in .cache to avoid workspace bloat)
    export TMPDIR=/home/user/.cache/tmp
    export DOTNET_ROOT=/home/user/.cache/dotnet9
    export XDG_DATA_HOME=/home/user/.cache/data
    export XDG_CONFIG_HOME=/home/user/.cache/config
    mkdir -p $TMPDIR
    
    # 1. Download & extract game
    # (use curl commands from URLs section above)
    unzip game.zip "0.5.2 LIVE/Byte.pck" "0.5.2 LIVE/data_DesktopPets_windows_x86_64/DesktopPets.dll"
    
    # 2. Decompile DLL
    ilspycmd DesktopPets.dll -p -o decompiled/ -r GodotSharp.dll
    
    # 3. Create minimal project (just .NET code, no game resources)
    mkdir -p proj/Scripts
    cp decompiled/*.cs proj/Scripts/
    # (apply mobile patches below)
    
    # 4. Build .NET
    cd proj && dotnet build -c Debug
    
    # 5. Export launcher APK with Godot
    # (creates APK with .NET runtime + our assemblies, but minimal game resources)
    
    # 6. Patch original Byte.pck
    # Change main_scene from "uid://c5cgdds3ll0tb" to "Scenes/Main.tscn" in project.binary
    
    # 7. Append PCK to APK
    # Insert between zip entries and central directory, update EOCD offset
    
    # 8. Fix resources.arsc (must be STORED, not compressed)
    # Extract APK, rebuild with: zip -0 apk resources.arsc
    
    # 9. Sign
    apksigner sign --ks byte.keystore --ks-pass pass:bytepass --ks-key-alias byte --out final.apk aligned.apk
    
    # 10. CLEAN UP IMMEDIATELY
    rm -rf /home/user/.cache/godot /home/user/.cache/templates* /home/user/byte/proj /home/user/.cache/game*

---

## Mobile Patches

**Main.cs — Add at top of class:**

    public static bool _isMobile = false;
    private static bool _isMobileChecked = false;

**Main.cs — Guard _Ready():**

    if (!_isMobileChecked) {
        _isMobile = OS.HasFeature("mobile") || OS.HasFeature("android");
        _isMobileChecked = true;
    }
    if (!_isMobile) {
        // Desktop-only window calls (TransparentBg, Borderless, AlwaysOnTop, etc.)
    }

**project.godot — Change:**

    config/features=PackedStringArray("4.6", "C#", "GL Compatibility")
    window/size/transparent=false
    viewport/transparent_background=false
    textures/vram_compression/import_etc2_astc=true

**C# classes — Add `partial` keyword:**

    public partial class Main : Node2D  // was: public class Main : Node2D

**Strip generated code:**
- Remove MethodName/PropertyName/SignalName inner classes
- Remove GetGodotMethodList, InvokeGodotClassMethod, etc.
- Keep [Signal] delegate declarations

---

## Known Issues

1. **resources.arsc must be STORED** (compress_type=0), not DEFLATED — Android 11+ requirement
2. **Zipalign BEFORE appending PCK**, then sign AFTER
3. **Editor settings file:** `editor_settings-4.6.tres` (not `editor_settings-4.tres`)
4. **Solution file required:** Godot export needs `DesktopPets.sln` alongside `.csproj`
5. **.NET trimming:** Use `<PublishTrimmed>true</PublishTrimmed>` in .csproj to reduce APK size

---

## Testing

Install on Pixel 7:

    su -c "cp /sdcard/Download/Byte-Launcher.apk /data/local/tmp/byte.apk"
    su -c "pm uninstall com.desktop.byte"
    su -c "pm install /data/local/tmp/byte.apk"

Check logs:

    logcat -d | grep -iE "godot|mono|fatal" | tail -60

Expected: Game loads past 25%, shows main scene with Byte character.

---

## Summary

**Previous session:** Got 95% there — app installs, launches, shows loading screen, but stuck at 25% due to broken scene references from GDRE recovery.

**Fix:** Use original Byte.pck (correct resources) + our recompiled .NET assemblies (arm64). Don't recover/re-export.

**Critical:** Manage workspace size aggressively. Use .cache for everything large. Delete after use.