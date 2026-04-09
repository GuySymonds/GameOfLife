import React from 'react';

export default function Toolbar({ children }) {
  return <div className="flex flex-wrap items-center gap-3">{children}</div>;
}
