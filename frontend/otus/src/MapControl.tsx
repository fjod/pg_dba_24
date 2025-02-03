import React, { useEffect, useState } from 'react';
                                                              import { TileLayer, Polyline, useMap, useMapEvents } from 'react-leaflet';
                                                              import { MapContainer } from 'react-leaflet';
                                                              import { PointWithOrder } from './models';
                                                              import 'leaflet/dist/leaflet.css';

                                                              interface MapControlProps {
                                                                points: PointWithOrder[];
                                                                center: { lat: number, lon: number };
                                                                freeMode: boolean;
                                                                onFetchNearbyPoints: (bounds: { x1: number, y1: number, x2: number, y2: number }) => void;
                                                                onUpdateMapCenter: (center: { x: number, y: number }) => void;
                                                              }

                                                              const MapControl: React.FC<MapControlProps> = ({ points, center, freeMode, onFetchNearbyPoints, onUpdateMapCenter }) => {
                                                                const [parsedPoints, setParsedPoints] = useState<{ orderId: number, coordinates: { lat: number, lon: number } }[]>([]);

                                                                useEffect(() => {
                                                                  const parseCoordinates = (pointString: string): { lat: number, lon: number } => {
                                                                    const prefix = 'POINT(';
                                                                    const suffix = ')';
                                                                    const coordinates = pointString.slice(prefix.length + 1, -suffix.length).split(' ');
                                                                    if (coordinates.length !== 2) {
                                                                      throw new Error('Invalid point string format');
                                                                    }
                                                                    const [lon, lat] = coordinates;
                                                                    return {
                                                                      lat: parseFloat(lat),
                                                                      lon: parseFloat(lon)
                                                                    };
                                                                  };

                                                                  setParsedPoints(points.map(point => ({
                                                                    orderId: point.orderId,
                                                                    coordinates: parseCoordinates(point.coordinates)
                                                                  })));
                                                                }, [points]);

                                                                const groupedPoints = parsedPoints.reduce((acc, point) => {
                                                                  if (!acc[point.orderId]) {
                                                                    acc[point.orderId] = [];
                                                                  }
                                                                  acc[point.orderId].push(point.coordinates);
                                                                  return acc;
                                                                }, {} as { [key: string]: { lat: number, lon: number }[] });

                                                                const colors = ['red', 'blue', 'green', 'orange', 'purple', 'brown', 'pink', 'yellow'];

                                                                const MapCenter = ({ center, freeMode }: { center: { lat: number, lon: number }, freeMode: boolean }) => {
                                                                  const map = useMap();
                                                                  useEffect(() => {
                                                                    if (!freeMode) {
                                                                      map.setView([center.lat, center.lon], 13);
                                                                    }
                                                                  }, [center, freeMode, map]);
                                                                  return null;
                                                                };

                                                                const FetchNearbyPointsButton = () => {
                                                                  const map = useMap();
                                                                  const handleFetchNearbyPoints = () => {
                                                                    const bounds = {
                                                                      x1: map.getBounds().getWest(),
                                                                      y1: map.getBounds().getSouth(),
                                                                      x2: map.getBounds().getEast(),
                                                                      y2: map.getBounds().getNorth()
                                                                    };
                                                                    onFetchNearbyPoints(bounds);
                                                                  };

                                                                  return (
                                                                    <button className="fetch-nearby-points-button" onClick={handleFetchNearbyPoints} style={{ position: 'absolute', top: 10, left: 10, zIndex: 1000 }}>
                                                                      Fetch Nearby Points
                                                                    </button>
                                                                  );
                                                                };

                                                                const UpdateMapCenter = () => {
                                                                  const map = useMapEvents({
                                                                    moveend: () => {
                                                                      const center = map.getCenter();
                                                                      onUpdateMapCenter({ x: center.lng, y: center.lat });
                                                                    }
                                                                  });
                                                                  return null;
                                                                };

                                                                return (
                                                                  <div style={{ position: 'relative', height: '100%', width: '100%' }}>
                                                                    <MapContainer center={[center.lat, center.lon]} zoom={13} scrollWheelZoom={freeMode} style={{ height: '100%', width: '100%' }}>
                                                                      <MapCenter center={center} freeMode={freeMode} />
                                                                      <TileLayer
                                                                        attribution='&copy; OpenStreetMap contributors'
                                                                        url="https://{s}.tile.openstreetmap.org/{z}/{x}/{y}.png"
                                                                      />
                                                                      {Object.keys(groupedPoints).map((orderId, index) => (
                                                                        <Polyline key={orderId} positions={groupedPoints[orderId].map(point => [point.lat, point.lon])} color={colors[index % colors.length]} />
                                                                      ))}
                                                                      {freeMode && <FetchNearbyPointsButton />}
                                                                      <UpdateMapCenter />
                                                                    </MapContainer>
                                                                  </div>
                                                                );
                                                              };

                                                              export default MapControl;