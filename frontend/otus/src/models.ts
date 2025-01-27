// src/models.ts

export interface LogisticCenter {
    id: number;
    name: string;
    location: Point;
}

export interface Courier {
    id: number;
    logisticCenterId: number;
    name: string;
}

export interface Order {
    id: number;
    courierId: number;
    orderTimestamp: Date;
}

export interface Delivery {
    id: number;
    orderId: number;
    point: Point;
    deliveryTimestamp: Date;
    year: number;
}

export interface DateRange {
    startTime: Date;
    endTime: Date;
}

export interface Point {
    x: number;
    y: number;
}

export interface PointWithOrder {
    orderId: number;
    coordinates: string;
}