// src/LogisticCentersControl.tsx
      import React, { useEffect, useState } from 'react';
      import { getFastLogisticCenters, getSlowLogisticCenters, getPointsForLogisticCenter } from './fetchLogisticCenters';
      import { LogisticCenter, PointWithOrder } from './models';
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
                      const points = await getPointsForLogisticCenter(selectedCenter.id, mode, { startTime: startDate, endTime: endDate });
                      setPoints(points);
                  } catch (error) {
                      console.error('Error fetching points:', error);
                  } finally {
                      const endTime = new Date().getTime();
                      setElapsedTime(endTime - startTime);
                  }
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
                  </div>
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
                          <button className="fetch-points-button" onClick={handleFetchPoints}>Fetch Points</button>
                          <p>Number of points: {points.length}</p>
                          {elapsedTime !== null && <p>Elapsed time: {elapsedTime} ms</p>}
                      </div>
                  )}
              </div>
          );
      };


      export default LogisticCentersControl;