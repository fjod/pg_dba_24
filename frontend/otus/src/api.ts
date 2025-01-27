import axios from 'axios';
import {PointWithOrder} from "./models";

const API_BASE_URL = 'http://localhost:5267';

export const getFastLogisticCenters = async () => {
    try {
        const response = await axios.get(`${API_BASE_URL}/fast/logistic-centers`);
        return response.data;
    } catch (error) {
        console.error('Error fetching fast logistic centers:', error);
        throw error;
    }
};

export const getSlowLogisticCenters = async () => {
    try {
        const response = await axios.get(`${API_BASE_URL}/slow/logistic-centers`);
        return response.data;
    } catch (error) {
        console.error('Error fetching slow logistic centers:', error);
        throw error;
    }
};

export const getPointsForLogisticCenter = async (logisticCenterId: number, mode: 'fast' | 'slow'): Promise<PointWithOrder[]> => {
    try {
        const response = await axios.get<PointWithOrder[]>(`${API_BASE_URL}/${mode}/logistic-centers/${logisticCenterId}/first-10-points`);
        return response.data;
    } catch (error) {
        console.error(`Error fetching points for logistic center ${logisticCenterId}:`, error);
        throw error;
    }
};