# GameOfLife

Conway's Game of Life implemented as a .NET 10 solution with a REST API, a terminal console app, and a React web UI.

## Rules

The universe of the Game of Life is an infinite, two-dimensional orthogonal grid of square cells, each of which is in one of two possible states — **alive** or **dead**. Every cell interacts with its eight Moore-neighbourhood cells. At each tick the following transitions occur:

- Any live cell with **fewer than two** live neighbours dies (underpopulation).
- Any live cell with **two or three** live neighbours lives on to the next generation.
- Any live cell with **more than three** live neighbours dies (overpopulation).
- Any dead cell with **exactly three** live neighbours becomes a live cell (reproduction).

---

## Projects

| Project | Description |
|---------|-------------|
| `GameOfLife.Engine` | Pure game-logic library — no dependencies |
| `GameOfLife.Common` | Shared model classes (`GameModel`, `NewGameModel`) |
| `GameOfLife.Api` | ASP.NET Core 10 Web API (REST) |
| `GameOfLife.ConApp` | Console front-end using Spectre.Console |
| `GameOfLife.ReactUI` | React 18 web front-end (Vite) |
| `GameOfLife.WebApi` | Legacy .NET 2.2 Web API (not used) |

---

## Prerequisites

| Tool | Minimum version |
|------|----------------|
| [.NET SDK](https://dotnet.microsoft.com/download) | 10.0 |
| [Node.js](https://nodejs.org/) | 18 LTS or later |
| npm | 9 or later (bundled with Node) |

---

## Building the .NET solution

```bash
# From the repository root
dotnet build GameOfLife.slnx
```

---

## Running as a Console App

The console app renders the board in your terminal using Spectre.Console and calls the API over HTTP.

**Step 1 — start the API**

```bash
dotnet run --project GameOfLife.Api
```

The API starts at `https://localhost:7168` (HTTP on `http://localhost:5168`).  
Swagger UI is available at `https://localhost:7168/swagger` in Development mode.

**Step 2 — start the console app** (in a second terminal)

```bash
dotnet run --project GameOfLife.ConApp
```

The console app will connect to the API, create a new game, and start rendering generations automatically.

---

## Running as a React Web App

The React UI (Vite dev server) communicates with the same `GameOfLife.Api`.

**Step 1 — start the API**

```bash
dotnet run --project GameOfLife.Api
```

**Step 2 — install front-end dependencies** (only needed once)

```bash
cd GameOfLife.ReactUI
npm install
```

**Step 3 — start the Vite dev server**

```bash
npm run dev
```

Open [http://localhost:5173](http://localhost:5173) in your browser.

### React UI controls

| Button | Action |
|--------|--------|
| **Play / Pause** | Start or stop auto-advancing generations |
| **Step** | Advance a single generation manually |
| **New Game** | Create a fresh randomised board |

### Production build

```bash
cd GameOfLife.ReactUI
npm run build      # output goes to GameOfLife.ReactUI/dist/
npm run preview    # serve the production build locally
```

---

## API Endpoints

| Method | Route | Description |
|--------|-------|-------------|
| `GET`  | `/api/game` | List all active games |
| `POST` | `/api/game` | Create a new game (body: `{ "width": N, "height": N }`) |
| `GET`  | `/api/game/{id}` | Get current state of a game |
| `GET`  | `/api/game/{id}/next` | Advance one generation and return the new state |

