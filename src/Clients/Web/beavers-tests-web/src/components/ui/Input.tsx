import React from 'react';

interface InputProps extends React.InputHTMLAttributes<HTMLInputElement> {
  label?: string;
  error?: string;
  icon?: React.ReactNode;
}

export const Input: React.FC<InputProps> = ({ 
  label, 
  error, 
  icon,
  className = '', 
  ...props 
}) => {
  const inputClass = `input-field ${error ? 'error' : ''} ${className}`.trim();

  return (
    <div className="input-group">
      {label && <label className="input-label">{label}</label>}
      <div style={{ position: 'relative' }}>
        {icon && (
          <div style={{
            position: 'absolute',
            left: '0.75rem',
            top: '50%',
            transform: 'translateY(-50%)',
            color: 'var(--gray-400)'
          }}>
            {icon}
          </div>
        )}
        <input 
          className={inputClass}
          style={icon ? { paddingLeft: '2.5rem' } : {}}
          {...props} 
        />
      </div>
      {error && (
        <div className="input-error">
          <span>⚠</span>
          {error}
        </div>
      )}
    </div>
  );
};