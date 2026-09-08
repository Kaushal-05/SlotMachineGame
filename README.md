# Slot Machine Game — Unity Assignment

## 🎮 Game Overview

A classic 3-reel slot machine built in Unity, featuring the four provided
symbols Seven, Cherries, Bell, and BAR arranged in a weighted rarity
ladder so rarer symbols pay out more. Players spin using either the Spin
button or a fully animated pull-lever, place bets, and win when all three
reels land on the same symbol.

**Symbols & payouts:**

| Symbol | Rarity (spin weight) | Payout multiplier |
|---|---|---|
| BAR | Common (20) | 2x |
| Cherries | Common (15) | 3x |
| Bell | Uncommon (8) | 5x |
| Seven | Rare (3) | 10x (jackpot) |

Starting credits: 200. Default bet: 10 (adjustable in steps of 5 via the
Bet Up/Down buttons).

---

## ▶️ Instructions to Run the WebGL Build


🌟 **Play it live:** https://kaushal-05.github.io/SlotMachineGame/

Or run it locally from the repo:

1. Clone this repository.
2. Navigate to `Build/WebGL`.
3. WebGL builds won't run directly from a `file://` path due to browser CORS
   restrictions, so serve the folder with a local web server, e.g.:
   ```
   cd Build/WebGL
   python3 -m http.server 8000
   ```
4. Open `http://localhost:8000` in a browser (Chrome or Firefox recommended).
5. Alternatively, host the folder on GitHub Pages or itch.io for a
   shareable live link.

**Controls:** click the Spin button or click/tap the lever to spin. Use the
+ UP ARROW − button to Increase your bet before spinning.
+ DOWN ARROW - button to Decrease your bet before spinning.

---

## ✨ Bonus Features

- **Interactive pull-lever**: the lever physically travels down and springs
  back on click, and triggers a spin exactly when it reaches the bottom of
  its travel not just a button, but a second, fitting
  input matching classic slot machine hardware.
- **Weighted RNG**: symbol frequency and payout are inversely balanced (rare
  symbols pay more, common symbols pay less) rather than every symbol having
  equal odds and equal payout, which is closer to how real slot machines are
  tuned.
- **Adjustable betting**: players can raise or lower their bet between
  spins, with credits and bet amount both reflected live in the UI.
- **Contextual popups**: a win popup shows the multiplier and matched
  symbol; a separate popup warns when a spin is requested without enough
  credits to cover the bet. Losses are intentionally silent (no popup) so
  the game doesn't interrupt the player on the majority outcome. Only
  events that need the player's attention trigger a popup.
- **Sound feedback**: distinct audio cues for spin start (shared by both the
  Spin button and the lever), a win, and bet adjustment.
- **Smooth continuous reel animation** (see Thought Process below) rather
  than a simple sprite-swap flicker.

---

## 🧠 Thought Process & Approach

**Architecture.** The project is split into single-responsibility classes
rather than one monolithic controller:

- `SlotSymbol` — pure data (icon, rarity weight, payout multiplier).
- `SymbolRandomizer` — an isolated, weighted RNG function with no knowledge
  of animation, UI, or economy.
- `Reel` — owns one reel's spin animation and reports its landed symbol.
- `SlotMachine` — orchestrates the three reels and evaluates the win
  condition. Knows nothing about credits or UI.
- `PayoutManager` — owns the credit balance and bet amount; deducts bets and
  applies payouts. Knows nothing about reels or animation.
- `GameFlowController` — the only class that bridges input (Spin button,
  lever) to the check-bet-then-spin flow, since a UI Button's OnClick can't
  express that conditional logic on its own.
- `UIManager` / `PopupController` — subscribe to events from `PayoutManager`
  and `SlotMachine` and update text/popups reactively, rather than those
  classes reaching into the UI directly.

This separation meant animation, economy, and win-logic could each be
built, tested, and debugged independently. For example, the reel animation
went through two full rewrites during development without touching the win
condition, payout, or economy code at all.

**Fairness.** The actual result for each reel is decided by
`SymbolRandomizer` the instant a spin starts before any animation plays.
The scrolling animation is purely cosmetic and has no influence on the
outcome; it always resolves to the pre-determined result.

**Animation.** The reel animation went through two iterations. The first
approach swapped a single symbol sprite in place at shrinking intervals,
which looked like flickering rather than motion. The final version keeps a
small recycled ring of symbol images that scroll continuously at one steady
speed. Any symbol that scrolls past the bottom of the window is silently
repositioned above the top and given a new sprite, creating the illusion of
an endless strip using only a handful of images (similar visually to a
masked scrolling list, but hand-driven rather than physics/drag-based so it
can guarantee an exact landing). A short corrective ease at the end snaps
the nearest symbol precisely onto the result position, and a small
overshoot and settle on landing avoids an abrupt stop.

**Event-driven UI.** Rather than game logic classes updating UI text
directly, `PayoutManager` and `SlotMachine` expose events (credits changed,
bet changed, spin resolved) that `UIManager` and `PopupController` subscribe
to. This kept economy and game-state classes free of any UI references.

**What I'd improve with more time:** sliced sprite states for button
press/hover feedback (currently using a single static sprite per button),
and a free-spin or wild-symbol bonus round building on the existing
weighted-RNG system. Would've also added some particles and animation 
for Jackpot text above Slot Machine.
