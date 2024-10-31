- Создать инстанс ВМ с 2 ядрами и 4 Гб ОЗУ и SSD 10GB

установлен в virtual box но на hdd

- Установить на него PostgreSQL 15 с дефолтными настройками

установлен 17

- Создать БД для тестов: выполнить pgbench -i postgres

```
fjod@fjod-VirtualBox:/usr/lib/postgresql/17/bin$ sudo -u postgres pgbench -i postgres
[sudo] password for fjod: 
dropping old tables...
NOTICE:  table "pgbench_accounts" does not exist, skipping
NOTICE:  table "pgbench_branches" does not exist, skipping
NOTICE:  table "pgbench_history" does not exist, skipping
NOTICE:  table "pgbench_tellers" does not exist, skipping
creating tables...
generating data (client-side)...
vacuuming...  
creating primary keys...
done in 0.26 s (drop tables 0.00 s, create tables 0.01 s, client-side generate 0.10 s, vacuum 0.05 s, primary keys 0.10 s).
```

- Запустить pgbench -c8 -P 6 -T 60 -U postgres postgres

```
fjod@fjod-VirtualBox:/usr/lib/postgresql/17/bin$ sudo -u postgres pgbench -c8 -P 6 -T 60 postgres
pgbench (17.0 (Ubuntu 17.0-1.pgdg22.04+1))
starting vacuum...end.
progress: 6.0 s, 508.8 tps, lat 15.640 ms stddev 11.586, 0 failed
progress: 12.0 s, 521.7 tps, lat 15.329 ms stddev 10.758, 0 failed
progress: 18.0 s, 513.7 tps, lat 15.547 ms stddev 11.345, 0 failed
progress: 24.0 s, 504.3 tps, lat 15.849 ms stddev 12.150, 0 failed
progress: 30.0 s, 512.7 tps, lat 15.584 ms stddev 10.670, 0 failed
progress: 36.0 s, 519.1 tps, lat 15.413 ms stddev 10.921, 0 failed
progress: 42.0 s, 528.7 tps, lat 15.113 ms stddev 11.507, 0 failed
progress: 48.0 s, 519.7 tps, lat 15.386 ms stddev 11.728, 0 failed
progress: 54.0 s, 519.6 tps, lat 15.380 ms stddev 11.856, 0 failed
progress: 60.0 s, 520.1 tps, lat 15.351 ms stddev 11.236, 0 failed
transaction type: <builtin: TPC-B (sort of)>
scaling factor: 1
query mode: simple
number of clients: 8
number of threads: 1
maximum number of tries: 1
duration: 60 s
number of transactions actually processed: 31018
number of failed transactions: 0 (0.000%)
latency average = 15.460 ms
latency stddev = 11.389 ms
initial connection time = 14.642 ms
tps = 516.876963 (without initial connection time)
```

- Применить параметры настройки PostgreSQL из прикрепленного к материалам занятия файла

```
max_connections = 40
shared_buffers = 1GB
effective_cache_size = 3GB
maintenance_work_mem = 512MB
checkpoint_completion_target = 0.9
wal_buffers = 16MB
default_statistics_target = 500
random_page_cost = 4
effective_io_concurrency = 2
work_mem = 65536kB
min_wal_size = 4GB
max_wal_size = 16GB
```
рестартанул кластер, проверил из хоста что настройки изменились
```
select * from pg_settings where name = 'shared_buffers';
```
появилось другое значение для shared_buffers.


- Протестировать заново

 ```
fjod@fjod-VirtualBox:/usr/lib/postgresql/17/bin$ sudo -u postgres pgbench -c8 -P 6 -T 60 postgres
[sudo] password for fjod: 
pgbench (17.0 (Ubuntu 17.0-1.pgdg22.04+1))
starting vacuum...end.
progress: 6.0 s, 474.1 tps, lat 16.660 ms stddev 11.498, 0 failed
progress: 12.0 s, 515.2 tps, lat 15.516 ms stddev 11.272, 0 failed
progress: 18.0 s, 522.3 tps, lat 15.292 ms stddev 11.055, 0 failed
progress: 24.0 s, 518.5 tps, lat 15.412 ms stddev 10.825, 0 failed
progress: 30.0 s, 526.2 tps, lat 15.206 ms stddev 10.900, 0 failed
progress: 36.0 s, 514.7 tps, lat 15.517 ms stddev 11.467, 0 failed
progress: 42.0 s, 524.7 tps, lat 15.251 ms stddev 10.799, 0 failed
progress: 48.0 s, 534.2 tps, lat 14.962 ms stddev 11.646, 0 failed
progress: 54.0 s, 561.7 tps, lat 14.234 ms stddev 10.490, 0 failed
progress: 60.0 s, 563.0 tps, lat 14.189 ms stddev 10.640, 0 failed
transaction type: <builtin: TPC-B (sort of)>
scaling factor: 1
query mode: simple
number of clients: 8
number of threads: 1
maximum number of tries: 1
duration: 60 s
number of transactions actually processed: 31535
number of failed transactions: 0 (0.000%)
latency average = 15.197 ms
latency stddev = 11.078 ms
initial connection time = 58.552 ms
tps = 525.829527 (without initial connection time)
```

- Что изменилось и почему?

Кластер смог обработать на 500 транзакций больше за минуту (на 8 клиентах).
повышение shared_buffers позволяет postgresql хранить больше данных в памяти и реже обращаться к диску.
возможно повышение effective_cache_size тоже помогло, в доке написано что это позволяет планировщику как-то эффективнее обрабатывать индексы. в тестовых таблицах 3 индекса есть.
random_page_cost до 4 указывает что работа с диском довольно дорогая, возможно планировщик это как-то учел.
насчет остальных настроек не уверен.

- Создать таблицу с текстовым полем и заполнить случайными или сгенерированными данным в размере 1млн строк
```
CREATE TABLE student(
                        id serial,
                        fio char(100)
);

INSERT INTO student(fio) SELECT 'noname' FROM generate_series(1,1000000);
select count(*) from student;
>1000000
```

- Посмотреть размер файла с таблицей

`SELECT pg_size_pretty(pg_total_relation_size('student'));`
`>135 MB`

- 5 раз обновить все строчки и добавить к каждой строчке любой символ
до обновления
![image](https://github.com/user-attachments/assets/f55cd47a-c273-4d90-829d-8385c9aa4912)
![image](https://github.com/user-attachments/assets/434b370f-0e88-46ef-9330-235936e0ddba)

обновляю врукопашную:

```
update student set fio = 'name1';
update student set fio = 'name2';
update student set fio = 'name3';
update student set fio = 'name4';
update student set fio = 'name5';
```

- Посмотреть количество мертвых строчек в таблице и когда последний раз приходил автовакуум
  ![image](https://github.com/user-attachments/assets/a3ad95ae-d8d2-4e8f-9eba-12dffbd8514c)
получилось 5 миллионов мертвых строк, т.к. update с алгоритмом MVCC создает новую строку, старую помечает как ненужную при помощи трюков с номерами транзакций.

- Подождать некоторое время, проверяя, пришел ли автовакуум
![image](https://github.com/user-attachments/assets/af74893a-31ff-4188-a23b-baf5443f0921)
прошел, удалил все мертвые. размер таблицы 808 МБ.

- 5 раз обновить все строчки и добавить к каждой строчке любой символ
```
update student set fio = 'name6';
update student set fio = 'name7';
update student set fio = 'name8';
update student set fio = 'name9';
update student set fio = 'name10';
```
- Посмотреть размер файла с таблицей
  те же самые 808 МБ.
  
- Отключить Автовакуум на конкретной таблице
`ALTER TABLE student SET (autovacuum_enabled = off);`
- 10 раз обновить все строчки и добавить к каждой строчке любой символ
- Посмотреть размер файла с таблицей
```
SELECT pg_size_pretty(pg_total_relation_size('student'));
1482 MB
```
- Объясните полученный результат
судя по статистике, имеем 1 миллион живых записей и 10 миллионов мертвых. миллион живых весит 135 мб. 5 миллионов мертвых 808-135 = 673. 135+673*2 = 1481, все сходится.

- Не забудьте включить автовакуум)
восстановлю машину из снимка и все

- Задание со *:Написать анонимную процедуру, в которой в цикле 10 раз обновятся все строчки в искомой таблице. Не забыть вывести номер шага цикла.

```
- CREATE OR REPLACE PROCEDURE update_fio_with_counter()
    LANGUAGE plpgsql
AS $$
DECLARE
    counter integer := 1;
BEGIN
    FOR counter IN 1..10 LOOP
            UPDATE student
            SET fio = student.fio || counter::text;
            RAISE NOTICE 'Cycle iteration: %', counter;
        END LOOP;
END;
$$;
```
и вызов:

```
postgres.public> CALL update_fio_with_counter();
Cycle iteration: 1
Cycle iteration: 2
Cycle iteration: 3
Cycle iteration: 4
Cycle iteration: 5
Cycle iteration: 6
Cycle iteration: 7
Cycle iteration: 8
Cycle iteration: 9
Cycle iteration: 10
[2024-10-31 10:08:28] completed in 31 s 512 ms
```
