import React, { useState } from 'react';
                          import './App.css';
                          import ModeSelector from './ModeSelector';
                          import LogisticCentersControl from './LogisticCentersControl';
                          import 'leaflet/dist/leaflet.css';

                          function App() {
                            const [mode, setMode] = useState<'fast' | 'slow' | null>(null);

                            return (
                              <div className="App">
                                <header className="App-header">
                                <label> PostgreSQL для администраторов баз данных и разработчиков </label>
                                  {mode === null ? (
                                    <ModeSelector onSelectMode={setMode} />
                                  ) : (
                                    <LogisticCentersControl mode={mode} />
                                  )}
                                </header>

                              </div>
                            );
                          }

                          export default App;