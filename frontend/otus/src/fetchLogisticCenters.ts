// src/fetchLogisticCenters.ts
        import axios from 'axios';
        import { LogisticCenter, PointWithOrder } from './models';

        const API_BASE_URL = 'http://localhost:5267';

        export const getFastLogisticCenters = async (): Promise<LogisticCenter[]> => {
          try {
            const response = await axios.get<LogisticCenter[]>(`${API_BASE_URL}/fast/logistic-centers`);
            return response.data;
          } catch (error) {
            console.error('Error fetching fast logistic centers:', error);
            throw error;
          }
        };

        export const getSlowLogisticCenters = async (): Promise<LogisticCenter[]> => {
          try {
            const response = await axios.get<LogisticCenter[]>(`${API_BASE_URL}/slow/logistic-centers`);
            return response.data;
          } catch (error) {
            console.error('Error fetching slow logistic centers:', error);
            throw error;
          }
        };

        export const getPointsForLogisticCenter = async (logisticCenterId: number, mode: 'fast' | 'slow', dateRange: { startTime: string, endTime: string }): Promise<PointWithOrder[]> => {
          try {
            const response = await axios.post<PointWithOrder[]>(`${API_BASE_URL}/${mode}/logistic-centers/${logisticCenterId}/first-10-points`, dateRange);
            return response.data;
          } catch (error) {
            console.error(`Error fetching points for logistic center ${logisticCenterId}:`, error);
            throw error;
          }
        };