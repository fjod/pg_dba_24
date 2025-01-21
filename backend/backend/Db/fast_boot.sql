-- Enable PostGIS extension
CREATE EXTENSION IF NOT EXISTS postgis;

drop table if exists deliveries cascade;
drop table if exists orders cascade;
drop table if exists couriers cascade;
drop table if exists logistic_centers cascade;

-- Create logistic_centers table
CREATE TABLE logistic_centers (
                                  id SERIAL PRIMARY KEY,
                                  name VARCHAR(255) NOT NULL,
                                  location GEOMETRY(Point, 4326) NOT NULL
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
                            id SERIAL,
                            order_id INT NOT NULL,
                            point GEOMETRY(Point, 4326) NOT NULL,
                            delivery_timestamp TIMESTAMP NOT NULL,
                            year INT NOT NULL,
                            PRIMARY KEY (id, year),
                            FOREIGN KEY (order_id) REFERENCES orders(id)
) PARTITION BY RANGE (year);

CREATE INDEX idx_deliveries_geom
    ON deliveries
        USING GIST (point);

-- Create partitions for the deliveries table

DO $$
    DECLARE
        year INT;
    BEGIN
        FOR year IN 2020..2025 LOOP
                EXECUTE format('CREATE TABLE deliveries_%s PARTITION OF deliveries FOR VALUES FROM (%s) TO (%s)', year, year, year + 1);
            END LOOP;
    END $$;
