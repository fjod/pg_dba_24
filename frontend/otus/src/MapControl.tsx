// src/MapControl.tsx
import React from 'react';
import { TileLayer, Marker, Popup } from 'react-leaflet';
import { MapContainer } from 'react-leaflet'
import { PointWithOrder } from './models';
import 'leaflet/dist/leaflet.css';
import 'leaflet/dist/leaflet.css';

interface MapControlProps {
  points: PointWithOrder[];
}

const MapControl: React.FC<MapControlProps> = ({ points }) => {

function parseCoordinates(pointString: string): { lat: number, lon: number } {
    const prefix = 'POINT(';
    const suffix = ')';
    console.log(pointString);

    const coordinates = pointString.slice(prefix.length, -suffix.length).split(' ');
    console.log(coordinates);
    if (coordinates.length !== 2) {
        throw new Error('Invalid point string format');
    }
    const [lon, lat] = coordinates;
    return {
        lat: parseFloat(lat),
        lon: parseFloat(lon)
    };
}

    const parsedPoints = points.map(point => ({
        orderId: point.orderId,
        coordinates: parseCoordinates(point.coordinates)
    }));
  return (
      <MapContainer center={[51.505, -0.09]} zoom={13} scrollWheelZoom={false}>
          <TileLayer
              attribution='&copy; <a href="https://www.openstreetmap.org/copyright">OpenStreetMap</a> contributors'
              url="https://{s}.tile.openstreetmap.org/{z}/{x}/{y}.png"
          />
          <Marker position={[51.505, -0.09]}>
              <Popup>
                  A pretty CSS3 popup. <br /> Easily customizable.
              </Popup>
          </Marker>
      </MapContainer>
  );
};

export default MapControl;