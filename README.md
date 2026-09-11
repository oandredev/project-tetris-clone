# Tetris Clone

A Tetris clone built in **Unity (C#)**, designed around three separate concerns: game logic, rendering, and game orchestration.

Personal study and portfolio project.

**Author:** André Rodrigues ([@oandredev](https://github.com/oandredev)) · [Portfolio](https://oandredev.vercel.app)

---

## 🎮 Controls

| Key | Action |
|---|---|
| `A` | Move piece left |
| `D` | Move piece right |
| `S` | Soft drop (move down one row) |
| `Q` | Rotate counter-clockwise |
| `E` | Rotate clockwise |
| `Space` | **Hard drop** — animated fast fall to the bottom; the piece can still be moved and rotated while falling |

---

## 📐 Rules

- Standard **10-column by 14-row** board.
- The 7 classic Tetris pieces (**I, O, T, S, Z, J, L**), each with 4 rotation states.
- Piece randomization via a **"7-bag" system**: all 7 pieces appear once, in random order, before any of them repeats — this avoids unlucky streaks (e.g. the same piece showing up several times in a row).
- **Next piece preview**, always known in advance thanks to the bag system.
- **Ghost piece**: a white silhouette shows where the current piece will land before you decide.
- **Scoring**: 1000 points per line, multiplied by the run's current speed.
- **Bonus for clearing multiple lines at once** — a reward that's more than proportional, encouraging players to hold out for multi-line clears:

  | Lines cleared at once | Multiplier |
  |---|---|
  | 1 | 1x |
  | 2 | 3x |
  | 3 | 5x |
  | 4 (Tetris) | 8x |

- Fall speed increases every 5 lines cleared.
- **High score and progress saved between runs** (via `PlayerPrefs`): best score, best number of lines in a single run, and lifetime totals across every run ever played.
- On game over, the entire board disappears in a bottom-to-top animation before the results screen appears.

---

## 🧱 Architecture

The project follows a clear separation of responsibilities — each piece of the system does exactly one thing:

```
GameLoopController   → orchestrates the game: fall timer, input, hard drop, game over, scoring
GridManager           → owns the board data (0/1/2 grid), movement, rotation, collision, line clearing
GridRenderer          → only reads GridManager's state and draws it (pooled cubes, no per-frame Instantiate/Destroy)
Piece (ScriptableObject) → defines each piece's shape (4 rotations), editable directly in the Inspector
PieceBag               → 7-bag randomization system
ScoreService / PrefsService → generic PlayerPrefs persistence (high score, lifetime totals)
```

**Why this separation:** `GridManager` knows nothing about GameObjects, colors, or rendering — it only works with numbers. `GridRenderer` knows nothing about game rules — it only reacts to events (`OnGridChanged`) and draws whatever exists. This keeps each part independently testable and replaceable (you could, for example, swap the entire visual representation without touching a single line of game logic).

**Object pooling:** every visual cube (grid, ghost piece, and next-piece preview) is instantiated **once** and reused via `SetActive`/`MaterialPropertyBlock`, avoiding garbage collection spikes during gameplay.

---

## 🛠️ Tech Stack

- Unity (C#)
- `MaterialPropertyBlock` for coloring without generating duplicate materials
- `ScriptableObject` for piece data definitions
- `PlayerPrefs` + `JsonUtility` for local persistence

---

## ▶️ Getting Started
 
1. Clone the repository:
```bash
   git clone https://github.com/oandredev/project-tetris-clone
```
2. Open the project with **Unity 6000.6.0f1**.
3. Open the main scene and hit Play.
Alternatively, skip building from source and download the ready-to-play build directly from the [Releases](https://github.com/oandredev/project-tetris-clone/releases) page.


## 📌 Status

Completed, part of my game development study portfolio.