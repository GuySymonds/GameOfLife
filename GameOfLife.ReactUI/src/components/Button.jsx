import React from 'react';

export default function Button({ children, variant = 'primary', className = '', ...props }) {
  const base = 'inline-flex items-center px-3 py-1.5 rounded-md text-sm font-medium focus:outline-none focus:ring-2 focus:ring-offset-2';
  const variants = {
    primary: 'bg-sky-600 text-white hover:bg-sky-700 focus:ring-sky-500',
    secondary: 'bg-slate-100 text-slate-800 hover:bg-slate-200 focus:ring-slate-300',
    ghost: 'bg-transparent text-slate-700 hover:bg-slate-50 focus:ring-slate-200',
  };
  const cls = `${base} ${variants[variant] ?? variants.primary} ${className}`;
  return (
    <button className={cls} {...props}>
      {children}
    </button>
  );
}
