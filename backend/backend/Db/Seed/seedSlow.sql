-- Clear all tables
TRUNCATE TABLE deliveries RESTART IDENTITY CASCADE;
TRUNCATE TABLE orders RESTART IDENTITY CASCADE;
TRUNCATE TABLE couriers RESTART IDENTITY CASCADE;
TRUNCATE TABLE logistic_centers RESTART IDENTITY CASCADE;

-- Generate logistic centers
DO $$
BEGIN
    FOR i IN 1..10 LOOP
        INSERT INTO logistic_centers (name, location)
        VALUES (format('Logistic Center %s', i), ST_SetSRID(ST_MakePoint(32.0401 + random() * 0.1, 54.7818 + random() * 0.1), 4326));
    END LOOP;
END $$;

-- Generate couriers
DO $$
DECLARE
    center_rec RECORD;
BEGIN
    FOR center_rec IN (SELECT id FROM logistic_centers) LOOP
        FOR i IN 1..10 LOOP
            INSERT INTO couriers (logistic_center_id, name)
            VALUES (center_rec.id, format('Courier %s on center %s', i, center_rec.id));
        END LOOP;
    END LOOP;
END $$;

-- Generate orders
DO $$
    DECLARE
        courier_id int;
BEGIN
    FOR courier_id IN (SELECT id FROM couriers) LOOP
        FOR i IN 1..10 LOOP 
                INSERT INTO orders (courier_id, order_timestamp)
                VALUES (courier_id, '2020-01-01'::timestamp + (random() * interval '5 years'));
        END LOOP;
    END LOOP;
END $$;

-- Generate deliveries
DO $$
    DECLARE
        center_location geography;
        rec RECORD;
    BEGIN
        FOR rec IN (SELECT id, courier_id, order_timestamp FROM orders) LOOP
                SELECT location INTO center_location FROM logistic_centers lc
                                                              JOIN couriers c ON lc.id = c.logistic_center_id
                WHERE c.id = rec.courier_id;

                FOR i IN 1..(50 + floor(random() * 100)) LOOP
                        INSERT INTO deliveries (order_id, point, delivery_timestamp, year)
                        VALUES (
                                   rec.id,
                                   ST_SetSRID(ST_MakePoint(
                                                    ST_X(center_location::geometry) + (i * 0.001) + (random() * 0.001),
                                                    ST_Y(center_location::geometry) + (i * 0.001) + (random() * 0.001)
                                              ), 4326),
                                   rec.order_timestamp + (i * interval '30 seconds'),
                                   EXTRACT(YEAR FROM rec.order_timestamp)
                               );
                    END LOOP;
            END LOOP;
    END $$;

select count(*) from deliveries;