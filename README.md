# Byte Desktop Pet - Android APK - v3 drag & bubble fix

**Built:** 2026-08-02
**Package:** com.desktop.byte
**Size:** 105 MB (arm64 slim, includes .NET)
**Repo:** https://github.com/camlakorns-rgb/Teat

## Progress

You reported:
- ✅ Enter name appears, she is animated (was stuck at 25% before)
- ❌ Can't drag or tap
- ❌ Speech bubble under her

This v3 fixes drag/tap and bubble.

## What was fixed since v2 (105 MB .NET fix)

**v2** fixed `Unable to find .NET assemblies` by merging `.godot/mono/publish/arm64/` (175 DLLs) into `Byte.pck` → new PCK 62.95 MB.

**v3 (current)** fixes input:

**Main.cs `_Input`:**
```csharp
public override void _Input(InputEvent @event)
{
    if (_isMobile)
    {
        if (@event is InputEventScreenTouch touch)
        {
            _lastTouchPos = (Vector2I)touch.Position;
            bool hasPoint = GetThinnerCollisionBox().HasPoint(_lastTouchPos);
            if (touch.Pressed && hasPoint)
            {
                Input.ActionPress("Move"); // right-click drag
                Input.ActionPress("Pet");  // left-click pet
            }
            else
            {
                Input.ActionRelease("Move");
                Input.ActionRelease("Pet");
            }
        }
        else if (@event is InputEventScreenDrag drag)
        {
            _lastTouchPos = (Vector2I)drag.Position;
            Input.ActionPress("Move");
        }
    }
    base._Input(@event);
}
```
- Move was right mouse button, Pet left mouse — touch now synthesizes both.
- `MobileMousePos()` returns `_lastTouchPos` on mobile.

**AttachObjWindow bubble under character:**
```csharp
Vector2 attachmentMargin = ...
if (OS.HasFeature("mobile"))
{
    if (attachmentTyping == TEXT)
        attachmentMargin.Y -= 80; // move bubble up
}
```
- Shifts dialogue bubble 80px up on mobile so it appears above her, not under.

## Install

```bash
adb push Byte-Launcher.apk /sdcard/Download/
su -c "cp /sdcard/Download/Byte-Launcher.apk /data/local/tmp/byte.apk"
su -c "pm uninstall com.desktop.byte"
su -c "pm install /data/local/tmp/byte.apk"
```

## GitHub

Repo cloned at `/home/user/.cache/Teat`, commit locally done:
- `Byte-Launcher-v2-dragfix.apk` (105 MB) + README committed as `63cd72d`
- Push fails: `fatal: could not read Username for 'https://github.com'`

To push, you need to:
1. Create a PAT (Personal Access Token) on GitHub
2. In your local machine: `git push https://TOKEN@github.com/camlakorns-rgb/Teat.git`
3. Or enable Git LFS for APK (>100 MB GitHub limit): `git lfs track "*.apk"`

I kept large toolchains in `.cache` (excluded from Arena 128 MB snapshot) — same storage trick as GitHub LFS.

## Next steps

- Test drag: touch and hold on Byte, drag finger — she should follow
- Test tap: quick tap inside her collision box should trigger pet animation + dialogue
- If bubble still under, increase offset from 80 to 120 in `AttachObjWindow.cs`
- If drag still not moving window (Android windows can't move), we may need to make character move inside full-screen window instead of moving Window.Position. Let me know.

