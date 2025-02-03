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
            console.log(response.data.length);
            return response.data;
          } catch (error) {
            console.error(`Error fetching points for logistic center ${logisticCenterId}:`, error);
            throw error;
          }
        };

export const getPointsInRectangle = async (x1: number, y1: number, x2: number, y2: number): Promise<PointWithOrder[]> => {
    try {
        const response = await axios.get(`${API_BASE_URL}/fast/points-in-rectangle?x1=${x1}&y1=${y1}&x2=${x2}&y2=${y2}`);
        console.log(response.data);
        return response.data;
    }
    catch (error) {
        console.error(`Error fetching points` , error);
        throw error;
    }
};

