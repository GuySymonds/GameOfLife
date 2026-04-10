import { createRoot } from 'react-dom/client';
import { useState, useEffect, useCallback, useRef } from 'react';
import './index.css';
import Button from './components/Button';
import Panel from './components/Panel';
import Toolbar from './components/Toolbar';
import SectionHeading from './components/SectionHeading';
import CanvasBoard from './components/CanvasBoard';

const API_BASE = import.meta.env.VITE_API_BASE ?? 'http://localhost:5168/api/game';

function getViewportGameSize() {
  const viewportWidth = window.innerWidth;
  const viewportHeight = window.innerHeight;
  const isLandscape = viewportWidth >= viewportHeight;

  const availableWidth = Math.max(240, viewportWidth - (viewportWidth >= 768 ? 420 : 32));
  const availableHeight = Math.max(180, viewportHeight - 220);

  const widthUsage = isLandscape ? 0.9 : 0.65;
  const heightUsage = isLandscape ? 0.65 : 0.9;

  const baseCellSize = viewportWidth >= 768 ? 8 : 6;

  let width = Math.max(20, Math.floor((availableWidth * widthUsage) / baseCellSize));
  let height = Math.max(20, Math.floor((availableHeight * heightUsage) / baseCellSize));

  if (isLandscape && width <= height)
    width = height + 1;

  if (!isLandscape && height <= width)
    height = width + 1;

  return { width, height };
}

function countAlive(cells) {
  if (!cells) return 0;
  return cells.reduce((total, row) => total + row.reduce((rowTotal, cell) => rowTotal + (cell ? 1 : 0), 0), 0);
}

function Square({ value }) {
  const alive = !!value;
  return <span className={`cell ${alive ? 'alive' : ''}`} />;
}

function Board({ onStatsChange }) {
  const [squares, setSquares] = useState(null);
  const [gameId, setGameId] = useState(null);
  const [running, setRunning] = useState(false);
  const [error, setError] = useState(null);
  const intervalRef = useRef(null);
  const [renderMode, setRenderMode] = useState('canvas');
  const [speed, setSpeed] = useState(1);
  const [cycle, setCycle] = useState(0);
  const [alive, setAlive] = useState(0);

  const startNewGame = useCallback(async () => {
    try {
      setError(null);
      const { width, height } = getViewportGameSize();
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
      const aliveCount = countAlive(data.cells);
      setSquares(data.cells);
      setGameId(data.gameId);
      setCycle(0);
      setAlive(aliveCount);
      onStatsChange?.({ cycle: 0, alive: aliveCount });
    } catch (err) {
      setError(`Failed to start game: ${err.message}`);
    }
  }, [onStatsChange]);

  const fetchNext = useCallback(async (id) => {
    if (!id) return;
    try {
      const response = await fetch(`${API_BASE}/${id}/next`, {
        method: 'GET',
        headers: { Accept: 'application/json' },
      });
      if (!response.ok) throw new Error(`Server error: ${response.status}`);
      const data = await response.json();
      const aliveCount = countAlive(data.cells);
      setSquares(data.cells);
      setGameId(data.gameId);
      setAlive(aliveCount);
      setCycle((previousCycle) => {
        const nextCycle = previousCycle + 1;
        onStatsChange?.({ cycle: nextCycle, alive: aliveCount });
        return nextCycle;
      });
    } catch (err) {
      setRunning(false);
      setError(`Failed to fetch next generation: ${err.message}`);
    }
  }, [onStatsChange]);

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
    const cols = squares[0]?.length ?? getViewportGameSize().width;
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

      <div className="text-sm text-slate-600" style={{ marginTop: '8px' }}>
        Cycle: {cycle} • Alive: {alive}
      </div>

      <Panel style={{ marginTop: '12px' }}>{renderMode === 'canvas' ? <CanvasBoard squares={squares} /> : renderGrid()}</Panel>
    </div>
  );
}

function App() {
  const [stats, setStats] = useState({ cycle: 0, alive: 0 });

  useEffect(() => {
    const stored = localStorage.getItem('theme');
    let prefer = stored;

    if (!prefer) {
      const prefersDark = window.matchMedia && window.matchMedia('(prefers-color-scheme: dark)').matches;
      prefer = prefersDark ? 'dark' : 'light';
    }

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
              <Board onStatsChange={setStats} />
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
              <div className="text-sm text-slate-600">Cycle: {stats.cycle}</div>
              <div className="text-sm text-slate-600">Alive: {stats.alive}</div>
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

function ThemeToggle() {
  const [theme, setTheme] = useState(() => {
    const stored = localStorage.getItem('theme');
    if (stored) return stored;
    const prefersDark = window.matchMedia && window.matchMedia('(prefers-color-scheme: dark)').matches;
    return prefersDark ? 'dark' : 'light';
  });

  useEffect(() => {
    document.documentElement.classList.toggle('theme-dark', theme === 'dark');
    document.documentElement.classList.toggle('theme-light', theme !== 'dark');
    localStorage.setItem('theme', theme);
  }, [theme]);

  return (
    <div style={{ display: 'flex', alignItems: 'center', gap: '12px' }}>
      <label style={{ fontSize: '12px' }}>
        <input type="radio" name="theme" value="light" checked={theme === 'light'} onChange={e => setTheme(e.target.value)} />
        Light
      </label>
      <label style={{ fontSize: '12px' }}>
        <input type="radio" name="theme" value="dark" checked={theme === 'dark'} onChange={e => setTheme(e.target.value)} />
        Dark
      </label>
    </div>
  );
}

const container = document.getElementById('root');
if (!container) throw new Error('Root element #root not found in the document');
const root = createRoot(container);
root.render(<App />);
