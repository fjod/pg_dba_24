// src/ModeSelector.tsx
      import React from 'react';
      import './ModeSelector.css';

      interface ModeSelectorProps {
        onSelectMode: (mode: 'fast' | 'slow') => void;
      }

      const ModeSelector: React.FC<ModeSelectorProps> = ({ onSelectMode }) => {
        return (
          <div className="mode-selector">
            <h2>Выбрать БД</h2>
            <button className="mode-button" onClick={() => onSelectMode('fast')}>Быстрая</button>
            <button className="mode-button" onClick={() => onSelectMode('slow')}>Медленная</button>
          </div>
        );
      };

      export default ModeSelector;