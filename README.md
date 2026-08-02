# Byte Desktop Pet - Android APK - v4 drag fix + bubble + top-left fix

**Built:** 2026-08-02 14:00 UTC
**Package:** com.desktop.byte
**Size:** 105 MB
**Repo:** https://github.com/camlakorns-rgb/Teat

## User report
- Enter name works, animated ✅
- Can't drag, stuck top-left ❌
- Speech bubble under her ❌
- Text button sort of works

## Fixes in v4 (current)

**1. Top-left stuck:**
- On mobile, `mainWindow.Position` cannot move (Android windows are full-screen). Guarded with `if (!_isMobile)`.
- Now on mobile we move `Position` (Main Node2D) instead:
  - `FollowMouse()` on mobile: `Position = newPos` clamped to screen, not `mainWindow.Position`
  - `Walk()` on mobile: uses `(Vector2I)Position` and sets `Position = ...`
  - `_Ready()`: on mobile center Byte: `Position = screenSize/2 - trueSize/2, screenSize.Y - trueSize.Y - 100`
  - Disabled throw physics on mobile: `else if (isThrown && !_isMobile)`

**2. Drag/Tap:**
- `_Input` now synthesizes Move (right-click) + Pet (left-click) from touch:
  - Touch press inside collision box → `Input.ActionPress("Move")` + `ActionPress("Pet")`
  - Touch release → Release both
  - Drag → keep Move pressed, update `_lastTouchPos`
- `MobileMousePos()` returns `_lastTouchPos` on mobile

**3. Bubble under:**
- `AttachObjWindow.cs` `FollowParent()` shifts `attachmentMargin.Y -= 80` on mobile for TEXT attachments

**4. .NET assemblies (v2):**
- Merged `assets/.godot/mono/publish/arm64/` (175 DLLs) into Byte.pck → 62.97 MB PCK

## Build steps

- Decompile + strip + partial + mobile patches
- `dotnet build -c Debug` (needs swapfile)
- Godot export Android arm64 only
- Replace `assets.sparsepck` with combined PCK + `project.binary` patched
- Remove `*.a` libs → 105 MB, zipalign + apksigner

## Install

```bash
adb push Byte-Launcher.apk /sdcard/Download/
su -c "cp /sdcard/Download/Byte-Launcher.apk /data/local/tmp/byte.apk"
su -c "pm uninstall com.desktop.byte; pm install /data/local/tmp/byte.apk"
```

## GitHub

Repo: https://github.com/camlakorns-rgb/Teat

I tried to push v3 with your PAT `github_pat_11BVUI5...`:
- Installed `git-lfs`, tracked `*.apk`, committed
- Push failed: `remote: Permission to camlakorns-rgb/Teat.git denied to camlakorns-rgb. 403`

**Possible causes:**
- Token is fine-grained but doesn't have **Contents: Read & Write** on Teat repo
- Or token expired / not have LFS access
- Or repo has no LFS enabled

**How to fix Deploy Keys or PAT:**

**PAT (easiest):**
1. GitHub → Settings → Developer settings → Personal access tokens → Fine-grained tokens → New token
2. Resource owner: `camlakorns-rgb`, Repository access: Only select `Teat`
3. Permissions: Repository → Contents: **Read and Write**, Metadata: Read
4. Give me new token, I'll push with `https://TOKEN@github.com/...`

**Deploy Key (SSH, more secure):**
1. I generate key: `ssh-keygen -t ed25519 -f /tmp/deploy`
2. You add public key to Repo → Settings → Deploy keys → Add deploy key → Allow write
3. I push via SSH.

**LFS:**
- APK 105 MB > GitHub 100 MB limit, needs LFS. Run:
```bash
git lfs install
git lfs track "*.apk"
git add .gitattributes
```

Current local commit in `/home/user/.cache/Teat` (not pushed):
- `Byte-Launcher-v3-dragfix.apk` 105 MB (LFS pointer), README

Tell me if drag works now, or if you want me to make character draggable inside full-screen (current fix does that) vs moving window.

