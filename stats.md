- Реализовать прямое соединение двух или более таблиц
```
  SELECT
   *
   FROM
    bookings b
    INNER JOIN
    tickets t
    ON
    b.book_ref = t.book_ref
   limit 10;
```

прямое соединение бронирований и билетов


- Реализовать левостороннее (или правостороннее) соединение двух или более таблиц
```
SELECT
    f.flight_id,
    f.flight_no,
    f.scheduled_departure,
    f.scheduled_arrival,
    bp.ticket_no,
    bp.boarding_no,
    bp.seat_no
FROM
    flights f
LEFT JOIN
    boarding_passes bp
ON
    f.flight_id = bp.flight_id
LIMIT 10;
```
левое соединение рейса и посадочного талона (на рейсе могут быть опоздавшие? либо не проданные билеты?)
при проверке есть такие рейсы с пустыми талонами:
```
SELECT
    f.flight_id,
    f.flight_no,
    bp.ticket_no,
    bp.boarding_no,
    bp.seat_no
FROM
    flights f
LEFT JOIN
    boarding_passes bp
ON
    f.flight_id = bp.flight_id
WHERE
    bp.ticket_no IS NULL
LIMIT 10;
```
![image](https://github.com/user-attachments/assets/01afcaa0-293f-4621-92fc-29ee357f3bff)


- Реализовать кросс соединение двух или более таблиц
```
SELECT 
    ad.airport_name AS origin_airport_name,
    f.flight_no
FROM
    airports_data ad
CROSS JOIN
    flights f
LIMIT 10;
```
в результате будут все возможные комбинации названия аэропорта и номера полета

- Реализовать полное соединение двух или более таблиц
```
SELECT
    tf.ticket_no,
    tf.flight_id AS ticket_flight_id,
    tf.fare_conditions,
    tf.amount,
    f.flight_no
FROM
    ticket_flights tf
FULL OUTER JOIN
    flights f
ON
    tf.flight_id = f.flight_id
LIMIT 10;
```
Полное соединение рейса с билетами. Хотя наверное билета без рейса быть не может.

- Реализовать запрос, в котором будут использованы разные типы соединений
```
SELECT
    t.ticket_no AS ticket_number_from_tickets,
    t.passenger_name AS passenger_name,
    tf.flight_id AS flight_id_from_ticket_flights,
    tf.fare_conditions,
    tf.amount,
    f.flight_no AS flight_no,
    f.scheduled_departure,
    f.scheduled_arrival,
    ad.airport_code AS airport_code_from_airports_data,
    ad.airport_name AS airport_name
FROM
    tickets t
LEFT JOIN
    ticket_flights tf
ON
    t.ticket_no = tf.ticket_no
INNER JOIN
    flights f
ON
    tf.flight_id = f.flight_id
FULL OUTER JOIN
    airports_data ad
ON
    f.departure_airport = ad.airport_code
LIMIT 10;
```
Берем билеты с полетами, соединяем с рейсом и добавляем данные через outer аэропорты.

- Сделать комментарии на каждый запрос

сделал

- К работе приложить структуру таблиц, для которых выполнялись соединения
![bookings](https://github.com/user-attachments/assets/292b90ca-0237-4eac-84bd-d0885db85b45)

- Придумайте 3 своих метрики на основе показанных представлений
  
Для просмотра и анализа метрик нужна активная работа с БД и накопленная статистика, которой у меня нет. Можно посмотреть данные по использованию индексов:
```
select *
from pg_stat_all_indexes
where idx_scan = 0
  and not (relname like 'pg_%')
  and schemaname = 'bookings';
```
неиспользуемые индексы можно рассмотреть к удалению.

```
   SELECT *
     FROM pg_stat_database
     where datname = 'demo';
```
количество активных подключений (есть ли запас?)
количество транзакций с роллбеком (если их слишком много, мб что-то не так?)
количество попаданий в кеш (может увеличить shared_buffers?)
и т.д., также нужна рабочая база, а не демка с парой десятков запросов.
