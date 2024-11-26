Скачал демо базу  https://postgrespro.com/community/demodb
Восстановил ` sudo -u postgres psql -f demo-medium-en-20170815.sql  -U postgres`
Размер таблиц:

![image](https://github.com/user-attachments/assets/69b1c15e-03fb-4a01-9f28-a156e6d739a0)

Попробую разделить boarding_passes.

Данные там такие:
![image](https://github.com/user-attachments/assets/465bc36c-c908-4a67-bbb3-7dec6b1b298f)

Единственным подходящим кандидатом под ключ партиционирования выглядит номер рейса flight_id.

![image](https://github.com/user-attachments/assets/8043fed1-c67a-41bf-ab20-b9051278020c)

Это диапазон целых чисел от 1 до 65к, всего чисел в этом диапазоне 35к. 
Если делить по range, то нужно создавать дефолтную партицию для тех flight_id, которых нет в таблице сейчас (например 70000).
А вот по хешу выглядит наиболее рабочим вариантом, postgres будет брать хеш от flight_id и размещать в соответствующую секцию. 
При появлении новых номеров ничего не сломается и будет распределяться равномерно по секциям.

Создаю "главную" таблицу:
```
create table if not exists bookings.boarding_passes_hash
(
    ticket_no   char(13)   not null,
    flight_id   integer    not null,
    boarding_no integer    not null,
    seat_no     varchar(4) not null,
    primary key (ticket_no, flight_id),
    unique (flight_id, boarding_no),
    unique (flight_id, seat_no),
    constraint boarding_passes_ticket_no_fkey
        foreign key (ticket_no, flight_id) references bookings.ticket_flights
) partition by hash (flight_id);
```
Создаю секции:
```
create table bookings.boarding_passes_hash_0 partition of bookings.boarding_passes_hash for values with (modulus 3, remainder 0);
create table bookings.boarding_passes_hash_1 partition of bookings.boarding_passes_hash for values with (modulus 3, remainder 1);
create table bookings.boarding_passes_hash_2 partition of bookings.boarding_passes_hash for values with (modulus 3, remainder 2);
```
Копирую данные:
```
demo: bookings, public> insert into bookings.boarding_passes_hash(ticket_no, flight_id, boarding_no, seat_no)
                        select ticket_no, flight_id, boarding_no, seat_no from boarding_passes;
[2024-11-26 15:49:05] 1,894,295 rows affected in 44 s 419 ms
```
Попробую выбрать что-нибудь.
из старой таблицы:
`select * from boarding_passes where flight_id = 12347;`
![image](https://github.com/user-attachments/assets/0b9ab4b0-c71e-4ad6-ab35-f21ba1ac976e)
```
demo: bookings, public> select * from boarding_passes where flight_id = 12347;
[2024-11-26 15:50:10] 57 rows retrieved starting from 1 in 183 ms (execution: 3 ms, fetching: 180 ms)
```
теперь из хешированной:
```
select * from boarding_passes_hash where flight_id = 12347;
demo: bookings, public> select * from boarding_passes_hash where flight_id = 12347;
[2024-11-26 15:53:22] 57 rows retrieved starting from 1 in 263 ms (execution: 3 ms, fetching: 260 ms)
```
![image](https://github.com/user-attachments/assets/784829ed-e813-4bdc-8f6a-e2863af8faea)

Интересно что запрос из хешированной таблицы занимает больше времени (нет индекса на один flight_id). 
Попробую с составным индексом, который включает в себя flight_id.
И вот тут сразу видно ускорение в 3.5 раза:
```
demo: bookings, public> select * from boarding_passes_hash where flight_id = 12347 and boarding_no = 1;
[2024-11-26 15:56:54] 1 row retrieved starting from 1 in 100 ms (execution: 5 ms, fetching: 95 ms)
demo: bookings, public> select * from boarding_passes where flight_id = 12347 and boarding_no = 1;
[2024-11-26 15:57:04] 1 row retrieved starting from 1 in 361 ms (execution: 6 ms, fetching: 355 ms)
```
Хотя в обоих случаях используется Index Scan:
![image](https://github.com/user-attachments/assets/d673ed67-941d-441c-b14d-535b76337832)
![image](https://github.com/user-attachments/assets/86852a8d-c8b0-41bc-afdf-22cd753875a6)




