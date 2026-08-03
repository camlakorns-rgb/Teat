# HANDOFF8 — Byte Desktop Pet → Android APK (Session 8): v12 HANDOFF7 FIXES

**Date:** 2026-08-03
**Previous:** v11 (`Byte-Launcher-v11-bugfix.apk`, commit `d43798b`) — HANDOFF7 analysis found root causes.
**This session:** Built v12 with HANDOFF7 fixes applied.
**Repo:** https://github.com/camlakorns-rgb/Teat
**APK:** `Byte-Launcher-v12-handoff7fix.apk` (118 MB, signed, LFS)
**Commit:** `e4a8ea1`

---

## What v12 fixes (from HANDOFF7 root-cause analysis)

### Fix 1: ResourceCache.LoadData() now runs on mobile (THE BIG ONE)
**Root cause (HANDOFF7 §2):** On desktop, `LoadingPopup._Process` calls `ResourceCache.Instance.CallDeferred("LoadData")` which populates `resourcesLoaded` (items, characters, gallery, etc.) and `prefabsLoaded` (minigames). Our Android builds skipped LoadingPopup by patching `main_scene` → `Scenes/Main.tscn` directly, so `LoadData()` never ran → empty resource tables → no items, no NPCs, empty gallery, broken recipes, dead terminal commands.

**Fix:** In `Main._Ready()`, on mobile, `ResourceCache.Instance.CallDeferred("LoadData")` is called explicitly. The 5-second retry guard from v11 (`OnSpawnerTimeout` checks for empty ITEM table) now serves as the safety net while LoadData completes.

**Expected result:** Items spawn, NPCs spawn, gallery fills, recipe list works, minigames spawnable, terminal content commands (`force_spawn_*`, `ask_mode /ls`, etc.) work.

### Fix 2: AWAY (ToggleDespawnMobile) restore
**Root cause (HANDOFF7 §5):** The else branch (when Byte is hidden and AWAY pressed again) never set `Visible = true` — she disappeared permanently.

**Fix:** Else branch now sets `mainCharacter.Visible = true`, resets `Position` to ground level (`screenSize.Y - mainCharacter.trueSize.Y`), and plays Pet animation.

### Build marker
`public static readonly string V12_BUILD = "V12_HANDOFF7FIX_BUILD";`
**Verified present (UTF-16LE) in BOTH:**
- Loose DLL: `assets/.godot/mono/publish/arm64/DesktopPets.dll` ✓
- PCK DLL: inside `assets/assets.sparsepck` ✓

---

## Verification (all passed)

| Check | Result |
|---|---|
| `zipalign -c 4` | ✓ ALIGNED-OK |
| `unzip -t` | ✓ ZIP-OK |
| `apksigner verify` | ✓ v2+v3 true |
| `project.binary` md5 | `a33cb5892eb650a5b26a9541ba70b9ad` (known-good) |
| V12 marker in loose DLL | ✓ True |
| V12 marker in PCK | ✓ True |
| APK size | 118 MB |
| Same keystore (`byte`/`bytepass`) | ✓ → `pm install -r` upgrade over v11 |

---

## Install / test (Pixel 7)

```bash
adb push Byte-Launcher-v12-handoff7fix.apk /sdcard/Download/
su -c "cp /sdcard/Download/Byte-Launcher-v12-handoff7fix.apk /data/local/tmp/byte.apk"
su -c "pm install -r /data/local/tmp/byte.apk"
logcat -d | grep -iE "godot|mono|fatal|Spawner|ResourceCache" | tail -60
```

## Test checklist (what to verify on device)

1. **Items spawn** — within ~5-30s items should drop from top edge. If not: `logcat | grep -i Spawner` should no longer show "ITEM resources not ready" repeating forever.
2. **NPCs spawn** — companion and enemy NPCs appear after their timer.
3. **Gallery** (Pause menu → Gallery) — should show pieces now.
4. **Recipe list** (Pause menu → Recipe book) — should list combinations.
5. **AWAY** — tap → Byte disappears with dialogue. Tap again → she reappears at ground level.
6. **Terminal content commands** — `force_spawn_item /ls` should list actual IDs.
7. **Minigames** — reachable via items or terminal `force_spawn_minigame`.
8. All previous v11 features still work: drag, throw, pet, sit, menu, terminal open/close, magnifier, item drag/use/combine/throw, NPC tap-to-pop.

---

## Build pipeline (followed STEP_BY_STEPS.md)

```
STEP 0: Workspace at /home/user/teat/repo (symlink → /home/user/Teat), builds in /home/user/.cache/
STEP 1: Toolchain (dotnet9, godot 4.6.2 mono, templates, build-tools r35, GDRE v2.6.3, git-lfs, ilspycmd)
STEP 2: Game from itch.io → Byte.pck (35 MB, original, never re-exported)
STEP 3: Decompile reference (repo already has decompiled/)
STEP 4: strip_generated.py → stripped/ (77 files)
STEP 5: make_project.py → src/ (stripped + overlay patched/Scripts/)
STEP 6: v11_patch.py skipped (patched/Scripts/ already has all v11 + HANDOFF7 fixes)
STEP 7: dotnet build -c Debug → 0 errors, 1 warning
STEP 8: Godot export → launcher APK (27 MB, engine only)
STEP 9: GDRE extract Byte.pck (3342 files) + patched project.binary + .NET assemblies → merged PCK (64 MB)
STEP 10: assemble_v12.py → replace sparsepck + project.binary + loose DLLs, remove .a libs, rezip, zipalign
STEP 11: apksigner sign (JKS byte/bytepass, v1+v2+v3)
STEP 12: All verifications passed
STEP 13: Pushed to git via LFS (commit e4a8ea1)
```

## What's different from HANDOFF7

| Area | HANDOFF7 (analysis) | HANDOFF8 (this session) |
|---|---|---|
| ResourceCache.LoadData | Root cause identified, NOT fixed | ✓ Called from Main._Ready on mobile |
| AWAY restore | Bug confirmed in code, NOT fixed | ✓ Visible=true + ground reset |
| APK | v11 (last shipped) | v12 (new build) |
| Build marker | V11_BUGFIX_BUILD | V12_HANDOFF7FIX_BUILD |

## Known remaining items

1. **Magnifier lens**: viewport-texture rendering on GL Compatibility unverified on device — lens may show black/stale frame. If so, consider simplified fallback.
2. **Terminal soft keyboard**: whether the Android soft keyboard pops reliably when terminal opens is untested on device.
3. **Snake minigame**: GDScript-driven, D-pad injects ui_* + raw key events — verify on device.
4. **Performance**: debug export (no trimming). Release with PublishTrimmed may be smaller/faster but risks breaking reflection.
