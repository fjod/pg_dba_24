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
                                  <h1>PostgreSQL для админист2раторов баз данных и разработчиков</h1>
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