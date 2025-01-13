﻿-- Enable PostGIS extension
CREATE EXTENSION IF NOT EXISTS postgis;

drop table deliveries cascade;
drop table orders cascade;
drop table couriers cascade;
drop table logistic_centers cascade;

-- Create logistic_centers table
CREATE TABLE logistic_centers (
                                  id SERIAL PRIMARY KEY,
                                  name VARCHAR(255) NOT NULL,
                                  location TEXT NOT NULL
);

-- Create couriers table
CREATE TABLE couriers (
                          id SERIAL PRIMARY KEY,
                          name VARCHAR(255) NOT NULL,
                          logistic_center_id INT NOT NULL,
                          FOREIGN KEY (logistic_center_id) REFERENCES logistic_centers(id)
);

-- Create orders table
CREATE TABLE orders (
                        id SERIAL PRIMARY KEY,
                        courier_id INT NOT NULL,
                        order_timestamp TIMESTAMP NOT NULL,
                        FOREIGN KEY (courier_id) REFERENCES couriers(id)
);

-- Create deliveries table with year_month column
CREATE TABLE deliveries (
                            id SERIAL PRIMARY KEY,
                            order_id INT NOT NULL,
                            point GEOMETRY(Point, 4326) NOT NULL,
                            delivery_timestamp TIMESTAMP NOT NULL,
                            year_month INT NOT NULL,
                            FOREIGN KEY (order_id) REFERENCES orders(id)
);
