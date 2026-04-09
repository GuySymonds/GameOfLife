import React, { useEffect, useRef } from 'react';

export default function CanvasBoard({ squares }) {
  const canvasRef = useRef(null);

  useEffect(() => {
    const canvas = canvasRef.current;
    if (!canvas || !squares) return;

    const rows = squares.length;
    const cols = squares[0].length;

    // Calculate cell size to fit available space without scrollbars
    // Reserve space for header (60px), toolbar (60px), padding (16px)
    const availableWidth = window.innerWidth - 60;
    const availableHeight = window.innerHeight - 140;

    const maxCellWidth = Math.floor(availableWidth / cols);
    const maxCellHeight = Math.floor(availableHeight / rows);
    const cellSize = Math.max(2, Math.min(maxCellWidth, maxCellHeight));

    const logicalWidth = cols * (cellSize + 1);
    const logicalHeight = rows * (cellSize + 1);
    const dpr = window.devicePixelRatio || 1;

    canvas.width = logicalWidth * dpr;
    canvas.height = logicalHeight * dpr;
    canvas.style.width = logicalWidth + 'px';
    canvas.style.height = logicalHeight + 'px';
    canvas.style.display = 'block';

    const ctx = canvas.getContext('2d');
    ctx.scale(dpr, dpr);

    const isDark = document.documentElement.classList.contains('theme-dark');
    const liveColor = isDark ? '#00ffba' : '#0f172a';
    const deadColor = isDark ? '#081018' : '#f0f4f8';

    ctx.fillStyle = deadColor;
    ctx.fillRect(0, 0, logicalWidth, logicalHeight);

    ctx.fillStyle = liveColor;
    if (isDark) {
      ctx.shadowColor = liveColor;
      ctx.shadowBlur = 4;
    }

    for (let r = 0; r < rows; r++) {
      const row = squares[r];
      for (let c = 0; c < cols; c++) {
        if (row[c]) {
          ctx.fillRect(c * (cellSize + 1), r * (cellSize + 1), cellSize, cellSize);
        }
      }
    }
    ctx.shadowBlur = 0;

  }, [squares]);

  return <canvas ref={canvasRef} style={{ display: 'block', maxWidth: '100%', maxHeight: '100%' }} />;
}
