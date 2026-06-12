# Card Pile-Movement Trail — Script & Prefab Reference

> The streak / afterimage that follows a card while it flies between piles —
> when a played card flies to the **discard** pile, or when cards fly from the
> **discard** pile back to the **draw** pile during a reshuffle.

> Paths below are relative to the repository root.

---

## 1. The Prefabs (Godot `.tscn` scenes)

Godot has no "prefab" type — the equivalent is a **PackedScene `.tscn`**. There
are two layers: the **flyer** (moves something along an arc) and the **trail**
(the per-character streak that follows it).

### 1a. Trail prefabs — the actual visual (one per character)

Path pattern: `scenes/vfx/card_trail_<character>.tscn`
Resolved at runtime by `CharacterModel.TrailPath`
([src/Core/Models/CharacterModel.cs:53](../src/Core/Models/CharacterModel.cs)):
`"vfx/card_trail_" + Id.Entry.ToLowerInvariant()`

| Character | Prefab |
|-----------|--------|
| Ironclad | `scenes/vfx/card_trail_ironclad.tscn` |
| Silent | `scenes/vfx/card_trail_silent.tscn` |
| Defect | `scenes/vfx/card_trail_defect.tscn` |
| Regent | `scenes/vfx/card_trail_regent.tscn` |
| Necrobinder | `scenes/vfx/card_trail_necrobinder.tscn` |

**Node tree (from `card_trail_ironclad.tscn`):**

```
CardTrailIronclad (Node2D)            script = NCardTrailVfx.cs ; show_behind_parent = true
├─ Trails (Node2D)
│  ├─ OuterTrail (Line2D)             script = NCardTrail.cs ; width 96 ; tex trail.png ; orange-red, additive
│  └─ InnerTrail (Line2D)             script = NCardTrail.cs ; width 64 ; tex trail2.png ; yellow, additive
└─ Sprites (Node2D)                   <- the node NCardTrailVfx scales/fades
   ├─ BigSparks  (CPUParticles2D)     64 sparks, 2.0s life, brush_particle_2.png, red→orange ramp
   ├─ LittleSparks (CPUParticles2D)   64 sparks, 0.3s life, sparkle.png, gravity spray
   ├─ Sprite2D2  (Sprite2D)           small_card_silhouette.png (large, orange)
   └─ Sprite2D3  (Sprite2D)           small_card_silhouette.png (small, orange)
```

All children use the shared additive material
`themes/canvas_item_material_additive_shared.tres` (the glow). Other characters'
prefabs are the same tree, recolored.

**Textures:** `images/packed/vfx/trail.png`, `trail2.png`,
`images/vfx/brush_particle_2.png`, `images/packed/vfx/small_card_silhouette.png`,
`images/vfx/vfx_ghostly_power_up/sparkle.png`.
**Glow shader (used by some setups):** `shaders/vfx/vfx_card_trails.gdshader`.

### 1b. Flyer prefabs — the mover the trail follows

| Prefab | Script | Used for |
|--------|--------|----------|
| `scenes/vfx/vfx_card_fly.tscn` | `NCardFlyVfx.cs` | a real played card flying to discard / exhaust |
| `scenes/vfx/vfx_card_shuffle_fly.tscn` | `NCardFlyShuffleVfx.cs` | **discard → draw reshuffle** (invisible `CardSoul` silhouette) |
| `scenes/vfx/vfx_card_power_fly.tscn` | `NCardFlyPowerVfx.cs` | power cards |

`vfx_card_shuffle_fly.tscn` is tiny — a `Control` (script) with one `CardSoul`
(Sprite2D, `small_card_silhouette.png`) that arcs between the pile buttons; the
visible streak comes entirely from the trail prefab it spawns.

---

## 2. The Scripts

All under [src/Core/Nodes/Vfx/](../src/Core/Nodes/Vfx).

### `NCardTrailVfx.cs` — the trail controller (the heart of it)
- `Create(Control card, string characterTrailPath)` `:20` — instantiates the
  `card_trail_<character>.tscn` prefab and remembers the node to follow.
- `_Ready` `:31` — grabs the `Sprites` node and tweens it (scale → 0.5 over 0.5s,
  fade the followed card to 0.75 alpha, fade sprites in).
- `_Process` `:46` — **every frame copies the flyer's `GlobalPosition` and
  `Rotation`**, so the trail tracks the card.
- `FadeOut` `:55` + `StopParticles` `:65` — fade alpha to 0 and ramp particle
  `amount` to 1 when the flight ends, then `QueueFree`.

### `NCardTrail.cs` — the Line2D streak (on Outer/InnerTrail nodes)
- `_pointDuration = 0.8s`; spawn spacing `_minSpawnDist 12px` … `_maxSpawnDist 48px`.
- `_Process` `:26` — ages each point, drops expired ones, then `CreatePoint` at
  the parent's current world position.
- `CreatePoint` `:52` — adds a point only after ≥12px of travel; for big jumps
  (>48px) inserts Bézier-interpolated in-between points so the streak stays smooth.
- Variants in the same folder: `NBezierTrail.cs` (0.5s points), `NBasicTrail.cs`
  (last-10-segments simple trail).

### `NCardFlyVfx.cs` — flyer for a played card
- `Create(NCard card, Vector2 end, bool isAddingToPile, string trailPath)` `:51`.
- `_Ready` `:66` — **creates the trail**: `_vfx = NCardTrailVfx.Create(_card, _trailPath)` `:68`,
  parents it via `GetParent().AddChildSafely(_vfx)`.
- `PlayAnim` `:108` — random Bézier arc (dur 1–1.75s); moves/rotates the card,
  scales it 1.0 → 0.1, fades white → black; calls `_vfx.FadeOut()` near the end.

### `NCardFlyShuffleVfx.cs` — flyer for the discard → draw reshuffle
- `Create(CardPile startPile, CardPile targetPile, string trailPath)` `:48` —
  start/end positions come from `PileType.GetTargetPosition()` (the pile buttons).
- `_Ready` `:62` — **creates the trail** at `:69`, parents it to
  `NCombatRoom.Instance.CombatVfxContainer` `:72`.
- `PlayAnim` `:79` — same Bézier arc on an invisible placeholder; at the end calls
  `_targetPile.InvokeCardAddFinished()` and fades the trail.

---

## 3. Where it's triggered (the wiring)

[src/Core/Commands/CardPileCmd.cs](../src/Core/Commands/CardPileCmd.cs) — when a
card with no on-table node changes pile:

- **Played card → Discard/Exhaust** `:538`
  `NCardFlyVfx.Create(cardNode2, targetPosition, isAddingToPile:true, card3.Owner.Character.TrailPath)`
- **Discard → Draw reshuffle** `:555`
  `NCardFlyShuffleVfx.Create(oldPile3, card4.Pile, card4.Owner.Character.TrailPath)`
  reached via `ShuffleIfNecessary()` → `Shuffle()` → `Add(card, drawPile)`.

Container parent: `NCombatRoom.Instance.CombatVfxContainer` (or
`NRun.Instance.GlobalUi.TopBar.TrailContainer` when the pile is the Deck).

---

## 4. End-to-end map

```
CardPileCmd.cs (:538 played→discard / :555 discard→draw)
  └─ flyer prefab: vfx_card_fly.tscn  /  vfx_card_shuffle_fly.tscn
       script:    NCardFlyVfx.cs      /  NCardFlyShuffleVfx.cs   (Bézier arc mover)
       └─ NCardTrailVfx.Create(trailPath)            <- the trail controller
            prefab: scenes/vfx/card_trail_<character>.tscn
              ├─ Trails/OuterTrail + InnerTrail (Line2D, NCardTrail.cs)  <- the streak
              ├─ Sprites/BigSparks + LittleSparks (CPUParticles2D)        <- sparks
              └─ Sprites/Sprite2D2 + Sprite2D3 (card silhouettes)
```

---

## 5. How to see it in-game
Enter a combat, play a card (watch it fly to the discard pile with a trail), then
empty the draw pile so a reshuffle triggers (watch trails arc from the discard
pile button back to the draw pile button).
