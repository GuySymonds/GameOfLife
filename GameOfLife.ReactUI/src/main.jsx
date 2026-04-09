import { createRoot } from 'react-dom/client';
import { useState, useEffect, useCallback, useRef } from 'react';
import './index.css';
import Button from './components/Button';
import Panel from './components/Panel';
import Toolbar from './components/Toolbar';
import SectionHeading from './components/SectionHeading';
import CanvasBoard from './components/CanvasBoard';

const API_BASE = import.meta.env.VITE_API_BASE ?? 'http://localhost:5168/api/game';

function Square({ value }) {
  const alive = !!value;
  return <span className={`cell ${alive ? 'alive' : ''}`} />;
}

function Board() {
  const [squares, setSquares] = useState(null);
  const [gameId, setGameId] = useState(null);
  const [running, setRunning] = useState(false);
  const [error, setError] = useState(null);
  const intervalRef = useRef(null);
  const [renderMode, setRenderMode] = useState('canvas');
  const [speed, setSpeed] = useState(1); // 1 = 200ms per generation, 2 = 100ms, 3 = 50ms, 0.5 = 400ms

  const width = 200;
  const height = 80;

  const startNewGame = useCallback(async () => {
    try {
      setError(null);
      const response = await fetch(`${API_BASE}`, {
        method: 'POST',
        headers: {
          Accept: 'application/json',
          'Content-Type': 'application/json',
        },
        body: JSON.stringify({ width, height }),
      });
      if (!response.ok) throw new Error(`Server error: ${response.status}`);
      const data = await response.json();
      setSquares(data.cells);
      setGameId(data.gameId);
    } catch (err) {
      setError(`Failed to start game: ${err.message}`);
    }
  }, [width, height]);

  const fetchNext = useCallback(async (id) => {
    if (!id) return;
    try {
      const response = await fetch(`${API_BASE}/${id}/next`, {
        method: 'GET',
        headers: { Accept: 'application/json' },
      });
      if (!response.ok) throw new Error(`Server error: ${response.status}`);
      const data = await response.json();
      setSquares(data.cells);
      setGameId(data.gameId);
    } catch (err) {
      setRunning(false);
      setError(`Failed to fetch next generation: ${err.message}`);
    }
  }, []);

  useEffect(() => {
    startNewGame();
  }, [startNewGame]);

  useEffect(() => {
    if (running && gameId) {
      const interval = Math.max(50, Math.floor(200 / speed));
      intervalRef.current = setInterval(() => fetchNext(gameId), interval);
    } else {
      clearInterval(intervalRef.current);
    }
    return () => clearInterval(intervalRef.current);
  }, [running, gameId, fetchNext, speed]);

  const renderGrid = () => {
    if (!squares) return <div className="loading">Loading…</div>;
    const cols = squares[0]?.length ?? width;
    return (
      <div
        className="game-board-grid"
        style={{ gridTemplateColumns: `repeat(${cols}, var(--cell-size))` }}
      >
        {squares.flat().map((cell, idx) => (
          <Square key={idx} value={cell} />
        ))}
      </div>
    );
  };

  return (
    <div>
      {error && <div className="error">{error}</div>}
      <Toolbar>
        <Button onClick={() => setRunning((r) => !r)}>
          {running ? 'Pause' : 'Play'}
        </Button>
        <Button variant="secondary" onClick={() => fetchNext(gameId)}>
          Step
        </Button>
        <Button variant="ghost" onClick={startNewGame}>
          New Game
        </Button>
        <div style={{ display: 'flex', alignItems: 'center', gap: '6px' }}>
          <label style={{ fontSize: '12px' }}>Speed:</label>
          <input type="range" min="0.5" max="3" step="0.5" value={speed} onChange={(e) => setSpeed(parseFloat(e.target.value))} style={{ width: '80px' }} />
          <span style={{ fontSize: '12px' }}>×{speed}</span>
        </div>
      </Toolbar>

      <Panel className="mt-4">{renderMode === 'canvas' ? <CanvasBoard squares={squares} /> : renderGrid()}</Panel>
    </div>
  );
}

function App() {
  useEffect(() => {
    // Initialize theme from localStorage, default to dark
    const stored = localStorage.getItem('theme');
    const prefer = stored ?? 'dark';
    document.documentElement.classList.toggle('theme-dark', prefer === 'dark');
    document.documentElement.classList.toggle('theme-light', prefer !== 'dark');
  }, []);

  return (
    <div className="app-root min-h-screen p-6">
      <div className="app-container">
        <header className="mb-6">
          <h1 className="text-3xl font-semibold tracking-tight">Game of Life</h1>
        </header>

        <main className="layout-main">
          <section className="md:col-span-2">
            <Panel>
              <SectionHeading>Simulation</SectionHeading>
              <Board />
            </Panel>
          </section>

          <aside>
            <Panel>
              <SectionHeading>Controls</SectionHeading>
              <BoardControlsPlaceholder />
              <div className="mt-3">
                <ThemeToggle />
              </div>
            </Panel>
            <Panel className="mt-4">
              <SectionHeading>Stats</SectionHeading>
              <div className="text-sm text-slate-600">Live cells, generation, etc.</div>
            </Panel>
          </aside>
        </main>
      </div>
    </div>
  );
}

function BoardControlsPlaceholder() {
  return (
    <div className="space-y-3">
      <div className="text-sm text-slate-600">Play / Pause / Step / New game controls are available in the simulation panel.</div>
    </div>
  );
}

// Small ThemeToggle control
function ThemeToggle() {
  const [theme, setTheme] = useState(() => localStorage.getItem('theme') ?? 'dark');
  useEffect(() => {
    document.documentElement.classList.toggle('theme-dark', theme === 'dark');
    document.documentElement.classList.toggle('theme-light', theme !== 'dark');
    localStorage.setItem('theme', theme);
  }, [theme]);
  return (
    <div className="flex items-center gap-2">
      <label className="text-sm text-slate-500">Theme</label>
      <select value={theme} onChange={e => setTheme(e.target.value)} className="text-sm border rounded px-2 py-1">
        <option value="light">Light</option>
        <option value="dark">Dark</option>
      </select>
    </div>
  );
}

const container = document.getElementById('root');
if (!container) throw new Error('Root element #root not found in the document');
const root = createRoot(container);
root.render(<App />);
