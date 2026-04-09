import React from 'react';

export default function Panel({ children, className = '', style = {} }) {
  const panelStyle = { background: 'var(--panel-bg)', padding: '1rem', borderRadius: '8px', border: '1px solid rgba(255,255,255,0.04)', ...style };
  return (
    <div className={className} style={panelStyle}>
      {children}
    </div>
  );
}
