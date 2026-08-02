# HANDOFF7 — Byte Desktop Pet → Android APK (Session 7): BUG REPORT + ROOT-CAUSE ANALYSIS (no fixes)

**Date:** 2026-08-03
**Previous:** v11 (`Byte-Launcher-v11-bugfix.apk`, commit `d43798b`) — tap-based bar, terminal CLOSE, magnifier touch-follow + CLOSE/±, item/NPC self-touch, spawner timers + guard, AWAY hide/show, dino IsMouseOver.
**Repo:** https://github.com/camlakorns-rgb/Teat
**This session:** User-tested v11. **NO CODE CHANGES were made.** All findings below are analysis only.

---

## 1. User report (v11 on-device)

| # | Report | Status |
|---|--------|--------|
| 1 | **Zoom still sort of buggy** | ❌ (analyzed §3) |
| 2 | **Terminal ("thermal") still sort of buggy** | ❌ (analyzed §4) |
| 3 | **AWAY literally just makes her disappear** (and doesn't come back) | ❌ (confirmed bug §5) |
| 4 | **Items still aren't spawning** | ❌ (ROOT CAUSE FOUND §6 — this is the big one) |
| 5 | **Gallery (in the pause menu, "in the thermal") is empty** | ❌ (same root cause §6) |
| 6 | **Recipe list ("rescript list") is broken / empty** | ❌ (same root cause §6) |
| 7 | Sit works ✓, MENU works ✓, drag/throw/pet work ✓ | ✅ |

---

## 2. 🎯 THE BIG FINDING: ResourceCache.LoadData() is never called on Android

**This one root cause explains reports #4, #5, #6 and part of #2.**

### The flow on desktop (how the ORIGINAL game boots):
1. `project.binary` main_scene = **the LoadingPopup scene** (uid `uid://c5cgdds3ll0tb` — this is what got stuck at 25% in HANDOFF session 1; LoadingPopup's progress bar = threaded scene load + resource cache load).
2. `LoadingPopup._Process` waits for the Main scene thread-load, then calls **`ResourceCache.Instance.CallDeferred("LoadData")`** (LoadingPopup.cs:97).
3. `LoadData()` populates **`ResourceCache.resourcesLoaded`** — ITEM, CHARACTER, GALLERY, SPAM, BRAINDACE_WORLDS, ASK_CHARACTERS, H_SCENES — and **`ResourceCache.prefabsLoaded`** (MINIGAME, UNTYPED) (ResourceCache.cs:152+).
4. When both are ready, `ChangeSceneToPacked` → `Scenes/Main.tscn`.

### The flow on ALL our Android builds (v4 → v11):
- We patch `project.binary` main_scene → `Scenes/Main.tscn` **directly** (the v3/v4 recipe). The app boots straight into Main. **LoadingPopup never runs → `LoadData()` is never called → `resourcesLoaded` and `prefabsLoaded` stay EMPTY forever.**

### Consequences on Android (all confirmed in code):
- `OnSpawnerTimeout` (Main.cs:1072+): `resourcesLoaded[ITEM]` empty → v11 guard prints `[Spawner] ITEM resources not ready yet - retrying in 5s` **forever** → **no items ever spawn** (this was true in v4–v11, not just v11).
- `OnSpawnerActorTimeout`: `resourcesLoaded[CHARACTER]` empty → **no NPCs ever spawn either** (user hasn't mentioned it, but it's broken).
- `GalleryHandler.SetupGallery` iterates `resourcesLoaded[GALLERY]` → **gallery empty** (report #5).
- `RecipeMenuHandler` builds from `resourcesLoaded[ITEM]` → **recipe list empty/broken** (report #6).
- `CallMinigameSpawn` → `prefabsLoaded[MINIGAME]` empty → **minigames can't be spawned** (dino/snake/catch-her unreachable via items or terminal).
- Terminal commands `force_spawn_item/actor/popup/minigame/scene`, `ask_mode /ls`, `enter_brain_dance /ls`, adventure worlds, `unlock_gallery` — all ResourceCache-dependent → dead or empty output → **terminal "sort of buggy"** (report #2, part).
- Byte herself renders fine because her `characterInformation` is a **direct scene reference** (`Byte.tres` ext_resource), not via ResourceCache — which is why the core pet experience worked while everything resource-driven silently didn't.

**Recommended fix (next session):** in `Main._Ready` (mobile), call `ResourceCache.Instance.LoadData()` (deferred) instead of relying on LoadingPopup — or better, boot to the LoadingPopup scene by patching main_scene to its path (needs its scene path/uid from the pack). Wait for `ResourceCacheLoaded` before starting spawner timers (the v11 5s retry guard becomes the safety net). Also verify `LoadPrefabData` (MINIGAME/UNTYPED) runs on mobile — same LoadData call covers it.

---

## 3. Report #1 — Zoom still sort of buggy

### What v11 already does
- Magnifier follows touch (`MagnifierWindow._Input` → `_mobilePos`), clamped on-screen.
- CLOSE button (top-right) → `Main.CloseMagnifier()` (resets `_magnificationActive`).
- + / − buttons (bottom-right) → `AdjustMagnification(±0.25)`, which sets the shader param `magnification` (MagnifierWindow.cs:148-151 — the param IS wired, so ± should visually zoom the lens shader).

### What is still wrong / unverified (needs device eyes + logcat)
1. **The lens samples a `ViewportTexture` of the window under the cursor / root viewport** (MagnifierWindow.cs:100-145). On Android with GL Compatibility, viewport-texture capture of the root may render black/empty or a stale frame — the most likely "still buggy" visual.
2. The magnifier window is **fullscreen and transparent**, so it intercepts EVERY touch — you can move the lens but can't touch anything under it (expected for a lens, but on mobile there's no "peek" alternative).
3. `GetWindowUnderCursor` relies on `DisplayServer.MouseGetPosition()` (MagnifierWindow.cs:215) — on Android there's no mouse; the fallback path uses the root, but the branch itself may misbehave.
4. Button placement is on the window (fullscreen) — should be reachable; if the user sees them, ± "not working" is likely #1 (lens shows nothing, so zoom is invisible).

**Next-session actions:** device screenshot/logcat of the magnifier; if the lens is black, either (a) accept a simplified magnifier (scaled-up duplicate of Byte's sprite in a panel instead of viewport capture), or (b) try `window/texture` capture alternatives. Confirm whether +/− changes anything visually.

---

## 4. Report #2 — Terminal still sort of buggy

### What v11 already does
- Tap-based TERM button (open/close toggle works — the stuck-action lockout from v10 is fixed).
- CLOSE button inside the terminal (TerminalWindow.cs v11) — exit/re-enter works.
- SEND button (bottom-right) → `HandleSubmit(_inputTrap.Text)`.

### What remains
1. **All ResourceCache-dependent commands are dead** (§2): `force_spawn_*`, `ask_mode /ls`, `enter_brain_dance /ls`, adventure world listing, `unlock_gallery` counts. This alone makes the terminal feel "half broken".
2. **Soft keyboard**: the input trap is a transparent full-rect `LineEdit` with `MouseFilter = Ignore`; focus is grabbed via emulated mouse press. Whether the Android soft keyboard pops reliably is **untested on device** — if it doesn't, typing is impossible and only SEND-with-paste would work.
3. Minor: tapping the output area re-grabs focus (desktop behavior) — on mobile this is harmless but worth confirming.

**Next-session actions:** after the §2 fix, re-test terminal; add a device check for the soft keyboard (if it doesn't pop, force `DisplayServer.VirtualKeyboardShow` on the trap's focus, or add a dedicated visible LineEdit).

---

## 5. Report #3 — AWAY makes her disappear (confirmed bug in v11 code)

`Main.ToggleDespawnMobile()` (Main.cs, v11):
```csharp
if (mainCharacter.Visible)
    mainCharacter.Visible = false;          // first tap: hides her ✓
else
    mainCharacter.ForceMainBodyState(..., "Pet", 0.5f);   // second tap: plays an anim
                                                        // ON AN INVISIBLE CHARACTER — never sets Visible=true
```
**The else branch never restores `Visible = true`** → after the first tap she is gone and no further tap brings her back. (Desktop's `Despawn` never had to restore her — she returns via other systems; the mobile toggle must do it itself.)

**Fix (next session):** in the else branch set `mainCharacter.Visible = true;` and reset her `Position` to the ground line (`screenSize.Y - trueSize.Y`), then play the Pet/land animation. Also consider a one-time confirmation instead of instant vanish.

---

## 6. What's different from the previous handoffs (per request)

| Area | HANDOFF5 (v10) | HANDOFF6 (v11) | HANDOFF7 (now) |
|---|---|---|---|
| Button bar | hold-buttons (stuck-action bug) | tap-buttons + release-on-hide | kept; verified working (MENU/SIT OK) |
| Terminal | no close (locked out) | CLOSE added, SEND | close/reopen OK; content commands dead due to §2 |
| Magnifier | mouse-follow, no close (stuck) | touch-follow + CLOSE + ± | reachable & closable; lens rendering unverified on GL mobile |
| Items | routed via `Main._Input` (dead — windows swallow touches) | self-contained `ItemWindow._Input` (grab/drag/use/combine/throw) | drag/use logic in place but **no items exist to test** (§2) |
| NPCs | tap dead (same routing) | `ActorWindow._Input` tap→Pet | same; **no NPCs spawn** (§2) |
| Spawning | timer start unverified | explicit timers + guard + top-edge spawn | **guard now proves the true cause: ITEM resource table empty → LoadData never ran** |
| AWAY | desktop-only behavior (dead) | hide/show toggle (bug: no restore) | bug confirmed in code (§5) |
| Build pipeline | make_project patched v9→v10 | make_project overlays repo patched/ + v10_patch.py | (unchanged) — repo patched/Scripts = current v11 state, `build/v11_patch.py` re-applies fixes on top |
| **Key discovery** | — | — | **LoadingPopup is the only LoadData() caller; patching main_scene past it empties the whole resource cache on Android** — affects v4→v11 equally |

Also different from HANDOFF1–3: we now know the 25% loading screen from session 1 **is** LoadingPopup, i.e. the game's intended boot path — the Android port skipped it instead of porting it.

---

## 7. Recommended fix order (next session — NOT done yet)

1. **§6 fix — load the resource cache on mobile** (`ResourceCache.Instance.LoadData()` from `Main._Ready`, deferred; optionally boot LoadingPopup instead). Expected effect: items spawn, NPCs spawn, gallery fills, recipe list fills, minigames spawn, terminal content commands work. **This is the single highest-value fix.**
2. **§5 fix — AWAY restore**: `Visible = true` + ground reset in `ToggleDespawnMobile` else-branch.
3. **§3 zoom**: verify lens rendering on device; fallback plan if viewport texture is black on GL mobile.
4. **§4 terminal**: verify soft keyboard; retest content commands after #1.
5. Re-verify drag/throw/pet/item-use/combine with real items present.

## 8. Test checklist for after the fixes

- Items drop from top edge within ~5–30 s; drag → flick → use on Byte → combine item+item → use on NPC.
- NPCs spawn; tap enemy → pops; companions follow Byte.
- Pause menu → Gallery shows pieces; Recipe book lists combinations (after seeing items).
- Terminal: `force_spawn_item /ls` lists IDs; `ask_mode /ls`; `enter_brain_dance /ls`; adventure start; `unlock_gallery`.
- Minigames spawn via item or terminal; controls work (JUMP/DUCK, D-pad, catch-her halves).
- ZOOM: lens shows content, ± changes zoom, CLOSE returns.
- AWAY: tap hides; tap again brings her back on the ground.
- Watch `logcat -d | grep -iE "godot|mono|fatal|Spawner|ResourceCache"`.

## 9. Repo state (unchanged this session)

`patched/Scripts/` = v11 sources (12 files incl. MobileUI.cs) · `build/` = make_project.py (overlay), v10_patch.py, v11_patch.py, strip_generated.py, assemble_apk.py, pcktool.py · `Byte-Launcher-v11-bugfix.apk` (LFS) · handoffs 1–6 + FEATURES.md.
