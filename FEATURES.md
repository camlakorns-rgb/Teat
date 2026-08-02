# Byte Desktop Pet — Full Feature Inventory (from all 77 scripts)

**UPDATE 2026-08-03 (v10):** All items in sections B and D are now IMPLEMENTED in `Byte-Launcher-v10-mobileui.apk` — see HANDOFF5.md. The button bar sits top-right (not on Byte).

**Date:** 2026-08-03 · **Base:** v9 `patched/Scripts` (Main.cs, Character.cs, AttachObjWindow.cs) + `decompiled/` (77 scripts)
**Goal:** list every feature in the game code, mark what's reachable on Android today, before implementing anything.

---

## ✅ A. Already working on mobile (confirmed on v9)

| # | Feature | Script | Notes |
|---|---------|--------|-------|
| A1 | **Drag Byte** (touch, any direction) | Main.cs `_Input`/`FollowMouse` | touch → `Move` action, Node2D `Position` |
| A2 | **Pet Byte** (tap → pet anim + talk bubble) | Main.cs `_Input`/`Pet` | touch → `Pet` action |
| A3 | **Throw / fling** (release → dangle → fly → fall → bounce → land) | Main.cs `_Process` + Character.cs | mobile branch, gravity 1400 |
| A4 | **Walk** (idle wander + walk-to-item) | Main.cs `Walk` | mobile branch |
| A5 | **Talk bubble** (TEXT attachment above head, follows her) | AttachObjWindow.cs | v9 fix; `MousePassthrough` |
| A6 | **Idle life**: blink, idle anims, random dialogue timer | Character.cs / Main.cs `OnRandomDialogueTimerTimeout` | auto |
| A7 | **Save/load** (settings + progress, `user://`) | SaveHandler.cs | works on Android |
| A8 | **EULA gate** on first run (Accept/Deny) | ConfirmationMenu.cs / Main.cs | buttons — touch via emulation, untested |
| A9 | **Auto spawn items & NPCs** on timers | Main.cs `OnSpawnerTimeout`, `spawnerActorTimer` | spawns, but see B1 (can't interact) |

---

## ❌ B. In the code but NOT reachable on mobile (blocked by input)

Every action below is bound to a **keyboard key / mouse wheel** in the game's input map. The mobile `_Input` handler only synthesizes `Move` + `Pet`, so these dead-end on Android:

| # | Feature | Script / action | Desktop trigger |
|---|---------|-----------------|-----------------|
| ~~B1~~ ✅ | **Pick up / drag / throw items** (v10: touch) (ItemWindow) | ItemWindow.cs | click item (`Move` action on window) — uses `DisplayServer.MouseGetPosition()` + window `Position`; **no touch code at all** |
| B2 | **Use item on Byte** (food, toys, outfits…) | ItemWindow.cs `UseOnMainActor` | drag item onto Byte |
| B3 | **Use item on NPC** (AI items) | ItemWindow.cs `UseOnOtherActor` | drag item onto NPC |
| B4 | **Combine two items** (discover recipes) | ItemWindow.cs `CombineItem` | drag item onto item |
| B5 | **Items that launch minigames** | ItemWindow.cs → `CallMinigameSpawn` | item use |
| B6 | **Sit** | Main.cs `Sit` | key |
| B7 | **Change outfit / clothing** | Main.cs `Clothing_Up` / `Clothing_Down` | keys |
| B8 | **Despawn Byte** (+ Shift+Despawn = move all items to her screen) | Main.cs `Despawn`, `Shift_Toggle` | keys |
| B9 | **Lock Byte to screen** (screen lock/unlock) | Main.cs `Screen_Lock` | key |
| B10 | **Magnifier** (zoom lens on Byte) | Main.cs `Magnifier` / MagnifierWindow.cs | key |
| B11 | **Pause menu** → gates everything below | Main.cs `PauseGame` | key |
| B12 | **Settings menu**: pet/item/UI scale, rendering driver, item spawns, actor spawns, passive-play AI, popups, convos, audio, **key rebinding UI**, blacklisted content | PauseMenu.cs → SettingsMenu.cs | via pause |
| B13 | **Recipe book** (discovered combinations) | PauseMenu.cs → RecipeMenuHandler.cs, CombinationUIHandler.cs | via pause |
| B14 | **Gallery** (collectible scene pieces) | PauseMenu.cs → GalleryWindow.cs, GalleryHandler.cs, GalleryItem.cs, GalleryPieceHandler.cs | via pause |
| B15 | **Guide / help state** in pause menu | PauseMenu.cs (PauseState.GUIDE) | via pause |
| B16 | **Terminal window** | Main.cs `Terminal` → TerminalWindow.cs, TerminalHandler.cs | key |
| B17 | **Terminal commands**: help, clear, exit, config, set_username, enable_cindesh_mode, enter_brain_dance, ask_mode, root (password), force_spawn_item/actor/popup/minigame/scene, unlock_gallery | TerminalHandler.cs | text input |
| B18 | **TerminalAdventure** — full text RPG: rooms, look/take/use/talk, NPCs, party, inventory, keys, trades | TerminalAdventure.cs + TA_*DataRes.cs | text input |
| B19 | **TerminalAsk** — chat with Byte/companions (keyword convo resolver) | TerminalAsk.cs + TAsk_*DataRes.cs | text input |
| B20 | **Pet NPCs** (companion/neutral actors) | ActorWindow.cs (`Pet` on actor) | click NPC — `Pet` only fires on Byte's box today |
| B21 | **Enemy/aggro NPCs** (chase, pop on pet/move/despawn) | ActorWindow.cs (AITypes.ENEMY) | clicks |
| B22 | **Dino Runner minigame** | DR_GameHandler.cs, DR_Dino.cs… | `DR_Jump` / `DR_Duck` keys |
| B23 | **Snake minigame** | SG_GameHandler.cs | arrow keys (no touch code) |
| B24 | **Catch Her minigame** | CatchHerGameLogic.cs | WASD + Space + clicks |
| B25 | **Lovense Idle clicker minigame** | LI_GameHandler.cs, LI_Clicker.cs | clicks (buttons) |
| B26 | **NPC attachments** (hats/clothes on actors from landing/touching) | ActorWindow.cs / AttachObjWindow.cs | landing interaction |
| B27 | **Popup windows (RANDOM_CLICKED_WINDOW)** | AttachObjWindow.cs | random popups — spawn only from actor interactions |

---

## ⚠️ C. Present but uncertain / environment-dependent on Android

| # | Feature | Notes |
|---|---------|-------|
| C1 | Menus/buttons (EULA, pause, settings, gallery) | Should respond to taps via `emulate_mouse_from_touch=true` — **untested on device** |
| C2 | Terminal text input | LineEdit focus should pop Android soft keyboard — **untested**; no on-screen send button |
| C3 | Window-based UI on Android | Pause/Terminal/Gallery are `Window` nodes → render fullscreen on Android (no real windows). Positioning/size code may misbehave |
| C4 | Mods (`settingMods`, ModManifest.cs) | file-path dependent — likely broken on Android |
| C5 | `enable_cindesh_mode` / NSFW toggles | content flags — reachable only via terminal |
| C6 | Kink blacklist (SaveHandler.Kinks) | settings UI — reachable only via pause menu |
| C7 | Mouse-wheel item zoom (`MouseWheelUp/Down` in ItemWindow?) | wheel has no touch equivalent |
| C8 | `PowerThrottling`, `ResourceCache`, `SignalEventBus`, `WeightGroup`, `FloatingText`, `AnimatedSpriteShaderSync`, `TimeSpinBox`, `DEBUG_SimSpawnItems`, `URLLinker` | background systems — mostly fine |

---

## 🔧 D. Proposed mobile mapping (for approval)

**D1 — Global touch shortcuts (no UI needed):**
- Long-press Byte (500 ms, no move) → **Pause menu**
- Two-finger tap on Byte → **Terminal**
- Two-finger drag up/down on Byte → **Clothing_Up / Clothing_Down** (or a small clothes button)
- Double-tap Byte → **Sit / stand**
- Long-press empty area + drag = despawn? (risk: accidental) → instead: **Sit button in pause menu; Despawn via pause menu**

**D2 — On-screen button bar** (always visible, small, semi-transparent):
`[Sit] [Clothes] [Lock] [Zoom] [Menu] [Terminal]` — each synthesizes its action via `Input.ActionPress/Release`. Cleanest, most reliable, matches how the desktop keys work.

**D3 — Items (biggest gap):** add touch handling to `ItemWindow` mirroring Main's pattern:
- touch on item → press `Move` + track touch pos; drag → move item window `Position` (mobile: Node2D-style positioning or window Position — items are Windows, so on Android they're fullscreen; need same "Position" treatment as Byte: give ItemWindow a mobile `_itemPos` and render sprite there, or move the whole window… simplest: on mobile, keep the item at a logical position and clamp like Byte)
- release on Byte → `UseOnMainActor`; release on NPC → `UseOnOtherActor`; release on item → `CombineItem`
- throw on release with velocity (same as Byte)

**D4 — NPCs:** extend touch synthesis to NPC windows (petting: tap NPC → `Pet` press; also `Despawn`-like pop for enemies). NPCs are `Window`s → same mobile positioning treatment as items/Byte if drag-NPC is desired (desktop lets you drag actors? — ActorWindow is a Window; keep scope: tap-to-pet only).

**D5 — Minigames:** on-screen touch buttons injected per game:
- Dino Runner: left side `[JUMP] [DUCK]` buttons
- Snake: `[▲][◀][▼][▶]` D-pad
- Catch Her: tap-left/tap-right halves + jump button (A/D + Space)
- Lovense Idle: already button-based (should work via emulation)

**D6 — Terminal text:** add a Send button + keep LineEdit (soft keyboard pops on focus); verify on device. `ask_mode`/adventure commands are typed.

**D7 — Settings UI reachability:** all via pause menu (D1/D2).

---

## E. Suggested order of implementation (awaiting your pick)

1. **D2 button bar + D1 pause shortcut** → unlocks pause, settings, gallery, recipes, terminal, sit, clothes, lock, zoom, despawn — ~70% of "missing features" in one shot
2. **D3 items** (pickup/drag/use/combine) — biggest single system
3. **D4 NPC petting**
4. **D5 minigame controls**
5. **D6 terminal send button** + device test of soft keyboard

*Everything above is read-only analysis — no code has been changed yet.*
