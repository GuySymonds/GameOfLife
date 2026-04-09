import { createRoot } from 'react-dom/client';
import { useState, useEffect, useCallback, useRef } from 'react';
import './index.css';

const API_BASE = import.meta.env.VITE_API_BASE ?? 'http://localhost:5168/api/game';

function Square({ value }) {
  const alive = !!value;
  return (
    <span
      className="square"
      style={alive ? { backgroundColor: 'black' } : undefined}
    />
  );
}

function Board() {
  const [squares, setSquares] = useState(null);
  const [gameId, setGameId] = useState(null);
  const [running, setRunning] = useState(false);
  const [error, setError] = useState(null);
  const intervalRef = useRef(null);

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
      intervalRef.current = setInterval(() => fetchNext(gameId), 200);
    } else {
      clearInterval(intervalRef.current);
    }
    return () => clearInterval(intervalRef.current);
  }, [running, gameId, fetchNext]);

  const renderGrid = () => {
    if (!squares) return <div className="loading">Loading…</div>;
    return squares.map((row, rowIdx) => (
      <div key={rowIdx} className="board-row">
        {row.map((cell, colIdx) => (
          <Square key={colIdx} value={cell} />
        ))}
      </div>
    ));
  };

  return (
    <div>
      {error && <div className="error">{error}</div>}
      <div className="controls">
        <button onClick={() => setRunning((r) => !r)}>
          {running ? 'Pause' : 'Play'}
        </button>
        <button onClick={() => fetchNext(gameId)}>Step</button>
        <button onClick={startNewGame}>New Game</button>
      </div>
      {renderGrid()}
    </div>
  );
}

function App() {
  return (
    <div className="game">
      <h1>Game of Life</h1>
      <div className="game-board">
        <Board />
      </div>
    </div>
  );
}

const container = document.getElementById('root');
if (!container) throw new Error('Root element #root not found in the document');
const root = createRoot(container);
root.render(<App />);
