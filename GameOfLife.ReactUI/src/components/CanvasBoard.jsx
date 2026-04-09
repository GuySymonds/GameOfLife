import React, { useEffect, useRef } from 'react';

export default function CanvasBoard({ squares }) {
  const canvasRef = useRef(null);

  useEffect(() => {
    const canvas = canvasRef.current;
    if (!canvas || !squares) return;
    const ctx = canvas.getContext('2d');
    const rows = squares.length;
    const cols = squares[0].length;
    // Compute cell size; cap canvas largest logical dimension to keep rendering snappy
    const baseCell = parseInt(getComputedStyle(document.documentElement).getPropertyValue('--cell-size')) || 10;
    const maxPixels = 1200; // limit largest logical canvas dimension
    const widthPixels = cols * baseCell;
    const heightPixels = rows * baseCell;
    const scale = Math.max(1, Math.ceil(Math.max(widthPixels, heightPixels) / maxPixels));
    const cellSize = Math.max(1, Math.floor(baseCell / scale));
    // logical dimensions (CSS pixels)
    const logicalWidth = cols * (cellSize + 1);
    const logicalHeight = rows * (cellSize + 1);
    const dpr = Math.max(1, window.devicePixelRatio || 1);
    // Set actual canvas pixel size for crisp rendering on high-DPI displays
    canvas.width = Math.floor(logicalWidth * dpr);
    canvas.height = Math.floor(logicalHeight * dpr);
    canvas.style.width = logicalWidth + 'px';
    canvas.style.height = logicalHeight + 'px';
    // clear using pixel dimensions, then set transform so drawing uses logical coords
    ctx.setTransform(1, 0, 0, 1, 0, 0);
    ctx.clearRect(0, 0, canvas.width, canvas.height);
    ctx.setTransform(dpr, 0, 0, dpr, 0, 0);

    // Fast clear
    ctx.clearRect(0, 0, canvas.width, canvas.height);

    // Determine colors based on prefers-color-scheme or body class
    const dark = document.documentElement.classList.contains('theme-dark') || window.matchMedia && window.matchMedia('(prefers-color-scheme: dark)').matches;
    const liveColor = dark ? '#00ffba' : '#0f172a';
    const deadColor = dark ? '#081018' : '#ffffff';

    ctx.fillStyle = deadColor;
    ctx.fillRect(0, 0, canvas.width, canvas.height);

    // Use requestAnimationFrame to avoid blocking the main thread during paint
    let mounted = true;
    const draw = () => {
      if (!mounted) return;
      // draw dead background first
      ctx.fillStyle = deadColor;
      ctx.fillRect(0, 0, logicalWidth, logicalHeight);
      ctx.fillStyle = liveColor;
      // Add a subtle neon glow in dark mode; using shadow is fast on canvas when drawing rectangles
      const isDark = document.documentElement.classList.contains('theme-dark');
      if (isDark) {
        ctx.shadowColor = liveColor;
        ctx.shadowBlur = Math.max(2, Math.min(8, Math.floor(cellSize / 1.5)));
      } else {
        ctx.shadowBlur = 0;
      }
      for (let r = 0; r < rows; r++) {
        const row = squares[r];
        for (let c = 0; c < cols; c++) {
          if (row[c]) ctx.fillRect(c * (cellSize + 1), r * (cellSize + 1), cellSize, cellSize);
        }
      }
      // reset shadow for safety
      ctx.shadowBlur = 0;
    };
    const raf = requestAnimationFrame(draw);
    return () => {
      mounted = false;
      cancelAnimationFrame(raf);
    };
  }, [squares]);

  return <canvas ref={canvasRef} className="w-full h-auto block" />;
}
