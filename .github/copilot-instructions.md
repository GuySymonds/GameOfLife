# Copilot Instructions for GameOfLife

## Project Overview
Conway's Game of Life implemented as a .NET 10 solution with three runnable components:

- **GameOfLife.Engine** — Pure logic library. Contains `GameEngine` which generates seeds and computes next generations.
- **GameOfLife.Common** — Shared models (`GameModel`, `NewGameModel`) used by both the API and the console app.
- **GameOfLife.Api** — ASP.NET Core 10 Web API that stores game state in memory and exposes REST endpoints.
- **GameOfLife.ConApp** — Console app that calls the API over HTTP and renders the board using Spectre.Console.

## Technology Choices
- **Target framework:** `net10.0` for all projects.
- **JSON serialization:** `System.Text.Json` (built-in). No Newtonsoft.Json.
- **Grid representation:** `byte[][]` (jagged array, `[row][column]`). Outer index is row, inner index is column. Each cell is `0` (dead) or `1` (alive).
- **HTTP client:** `HttpClient` with `GetFromJsonAsync` / `PostAsJsonAsync` extension methods from `System.Net.Http.Json`.
- **Console rendering:** Spectre.Console (`Panel`, `Table`, `FigletText`, `Markup`).
- **Swagger/OpenAPI:** Swashbuckle.AspNetCore for API documentation.

## Code Conventions
- Use **file-scoped namespaces** (`namespace Foo.Bar;`) throughout.
- Enable **nullable reference types** (`<Nullable>enable</Nullable>`) in all projects.
- Enable **implicit usings** (`<ImplicitUsings>enable</ImplicitUsings>`) in all projects.
- Prefer **expression-bodied members** for simple single-expression methods.
- Use `TryGetValue` instead of `ContainsKey` + indexer when accessing dictionaries.
- Keep the `Extention` class (the class name in the codebase uses this spelling) in `GameOfLife.ConApp` for `byte[][]` helper methods (`IsEqual`, `TotalLife`).

## Architecture Rules
- **GameOfLife.Engine** has zero dependencies — it only contains pure computation.
- **GameOfLife.Common** has zero dependencies — it only contains plain model classes.
- **GameOfLife.Api** depends on `GameOfLife.Common` and `GameOfLife.Engine`. It keeps all game state in a static in-memory `Dictionary<Guid, GameModel>` inside `GameService`. There is no database.
- **GameOfLife.ConApp** depends only on `GameOfLife.Common`. It communicates with the API via HTTP; it does **not** reference `GameOfLife.Engine` directly.
- The console app's `IGame` interface abstracts the HTTP transport so it can be swapped without changing `Program.cs`.

## Game Rules (Conway's Game of Life)
The engine (`GameEngine.ApplyLaw`) implements the four standard rules:
1. A live cell with fewer than 2 live neighbors dies (underpopulation).
2. A live cell with 2 or 3 live neighbors survives.
3. A live cell with more than 3 live neighbors dies (overpopulation).
4. A dead cell with exactly 3 live neighbors becomes alive (reproduction).

The grid wraps at the edges (toroidal topology).

## API Endpoints (GameOfLife.Api)
| Method | Route | Description |
|--------|-------|-------------|
| `GET`  | `/api/game` | List all games |
| `POST` | `/api/game` | Create a new game (`NewGameModel` body) |
| `GET`  | `/api/game/{id}` | Get current state |
| `GET`  | `/api/game/{id}/next` | Advance one generation and return state |

## Building & Running
```bash
# Build the whole solution
dotnet build GameOfLife.sln

# Run the API (required before starting the console app)
dotnet run --project GameOfLife.Api

# Run the console app (in a second terminal)
dotnet run --project GameOfLife.ConApp
```
