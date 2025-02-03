import React, { useEffect, useState } from 'react';
                                  import {
                                    getFastLogisticCenters,
                                    getSlowLogisticCenters,
                                    getPointsForLogisticCenter,
                                    getPointsInRectangle
                                  } from './fetchLogisticCenters';
                                  import { LogisticCenter, PointWithOrder } from './models';
                                  import MapControl from './MapControl';
                                  import './LogisticCentersControl.css';

                                  interface LogisticCentersControlProps {
                                    mode: 'fast' | 'slow';
                                  }

                                  const LogisticCentersControl: React.FC<LogisticCentersControlProps> = ({ mode }) => {
                                    const [logisticCenters, setLogisticCenters] = useState<LogisticCenter[]>([]);
                                    const [selectedCenter, setSelectedCenter] = useState<LogisticCenter | null>(null);
                                    const [points, setPoints] = useState<PointWithOrder[]>([]);
                                    const [startDate, setStartDate] = useState<string>('');
                                    const [endDate, setEndDate] = useState<string>('');
                                    const [elapsedTime, setElapsedTime] = useState<number | null>(null);
                                    const [freeMode, setFreeMode] = useState<boolean>(false);
                                    const [mapCenter, setMapCenter] = useState<{ lat: number, lon: number }>({ lat: 0, lon: 0 });

                                    useEffect(() => {
                                      const fetchLogisticCenters = async () => {
                                        try {
                                          const centers = mode === 'fast' ? await getFastLogisticCenters() : await getSlowLogisticCenters();
                                          setLogisticCenters(centers);
                                        } catch (error) {
                                          console.error('Error fetching logistic centers:', error);
                                        }
                                      };

                                      fetchLogisticCenters();
                                    }, [mode]);

                                    const handleFetchPoints = async () => {
                                      if (selectedCenter && startDate && endDate) {
                                        const startTime = new Date().getTime();
                                        try {
                                          const points = await getPointsForLogisticCenter(selectedCenter.id, mode, {
                                            startTime: startDate,
                                            endTime: endDate
                                          });
                                          setPoints(points);
                                        } catch (error) {
                                          console.error('Error fetching points:', error);
                                        } finally {
                                          const endTime = new Date().getTime();
                                          setElapsedTime(endTime - startTime);
                                        }
                                      }
                                    };

                                    const handleFetchNearbyPoints = async (bounds: { x1: number, y1: number, x2: number, y2: number }) => {
                                      const startTime = new Date().getTime();
                                      try {
                                        const points = await getPointsInRectangle(bounds.x1, bounds.y1, bounds.x2, bounds.y2);
                                        setPoints(points);
                                      } catch (error) {
                                        console.error('Error fetching nearby points:', error);
                                      } finally {
                                        const endTime = new Date().getTime();
                                        setElapsedTime(endTime - startTime);
                                      }
                                    };

                                    const handleUpdateMapCenter = (center: { x: number, y: number }) => {
                                      if (freeMode) {
                                        setMapCenter({ lat: center.y, lon: center.x });
                                      }
                                    };

                                    return (
                                      <div className="logistic-centers-control">
                                        <div className="logistic-centers-list">
                                          <h2>{mode.charAt(0).toUpperCase() + mode.slice(1)} Logistic Centers</h2>
                                          <ul>
                                            {logisticCenters.map((center) => (
                                              <li key={center.id} className="clickable" onClick={() => setSelectedCenter(center)}>
                                                {center.name}
                                              </li>
                                            ))}
                                          </ul>
                                          {selectedCenter && (
                                            <div className="points-list">
                                              <h3>Points for {selectedCenter.name}</h3>
                                              <label>
                                                Start Date:
                                                <input type="date" value={startDate} onChange={(e) => setStartDate(e.target.value)} />
                                              </label>
                                              <label>
                                                End Date:
                                                <input type="date" value={endDate} onChange={(e) => setEndDate(e.target.value)} />
                                              </label>
                                              {!freeMode && <button className="fetch-points-button" onClick={handleFetchPoints}>Fetch Points</button>}
                                              <p>Number of points: {points.length}</p>
                                              {elapsedTime !== null && <p>Elapsed time: {elapsedTime} ms</p>}
                                            </div>
                                          )}
                                        </div>
                                        <div className="map-container">
                                          <label>
                                            Free Mode:
                                            <input type="checkbox" checked={freeMode} onChange={(e) => setFreeMode(e.target.checked)} />
                                          </label>
                                          <MapControl
                                            points={points}
                                            onUpdateMapCenter={handleUpdateMapCenter}
                                            center={freeMode ? mapCenter : selectedCenter ? { lat: selectedCenter.location.y, lon: selectedCenter.location.x } : { lat: 0, lon: 0 }}
                                            freeMode={freeMode}
                                            onFetchNearbyPoints={handleFetchNearbyPoints}
                                          />
                                        </div>
                                      </div>
                                    );
                                  };

                                  export default LogisticCentersControl;