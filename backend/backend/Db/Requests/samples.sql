
-- запрос 1 точки в радиусе 0.1 градуса от точки 32.0401 54.7818
SELECT ST_AsText(point), order_id, delivery_timestamp
FROM deliveries
WHERE ST_DWithin(

              'SRID=4326;POINT(32.0401 54.7818)', point,0.1
      )
order by delivery_timestamp limit 100 ;

-- подготовка для второго запроса - идентификаторы и дата
SELECT
    o.id AS order_id,
    c.id AS courier_id,
    lc.id AS logistic_center_id,
    d.delivery_timestamp
FROM
    orders o
        JOIN
    couriers c ON o.courier_id = c.id
        JOIN
    logistic_centers lc ON c.logistic_center_id = lc.id
        JOIN
    deliveries d ON o.id = d.order_id
ORDER BY
    o.id, d.delivery_timestamp
    LIMIT 100;

-- запрос 2
-- первые 10 точек доставок от логистического центра 1
-- за указанный период времени
WITH ranked_deliveries AS (
    SELECT
        d.id,
        d.order_id,
        d.point,
        d.delivery_timestamp,
        ROW_NUMBER() OVER (PARTITION BY d.order_id ORDER BY d.delivery_timestamp) AS rn
    FROM deliveries d
             JOIN orders o ON o.id = d.order_id
             JOIN couriers c ON c.id = o.courier_id
             JOIN logistic_centers lc ON lc.id = c.logistic_center_id
    WHERE lc.id = 1
      AND ST_DWithin(lc.location, d.point, 0.1)
      AND d.delivery_timestamp >= '2022-09-17 21:00:00.000000'
      AND d.delivery_timestamp < '2024-09-17 21:00:00.000000'
)
SELECT
    ST_AsText(point) AS delivery_point,
    order_id,
    delivery_timestamp
FROM ranked_deliveries
WHERE rn <= 10
ORDER BY order_id, delivery_timestamp;

-- запрос 3 
-- точки в заданном прямоугольнике
SELECT ST_AsText(point), order_id, delivery_timestamp
FROM deliveries
WHERE ST_Intersects(
              point,
              ST_MakeEnvelope(32.10, 54.4, 32.20, 54.5, 4326)
      )
ORDER BY delivery_timestamp
    LIMIT 100;