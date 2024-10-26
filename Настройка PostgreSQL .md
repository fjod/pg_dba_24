pg 17 создана в virtual box, создал таблицу test:


```
postgres=# \l
                                                     List of databases

   Name    |  Owner   | Encoding | Locale Provider |   Collate   |    Ctype    | Locale | ICU Rules |   Access privileges   
-----------+----------+----------+-----------------+-------------+-------------+--------+-----------+-----------------------
 postgres  | postgres | UTF8     | libc            | en_US.UTF-8 | en_US.UTF-8 |        |           | 
 template0 | postgres | UTF8     | libc            | en_US.UTF-8 | en_US.UTF-8 |        |           | =c/postgres          +
           |          |          |                 |             |             |        |           | postgres=CTc/postgres
 template1 | postgres | UTF8     | libc            | en_US.UTF-8 | en_US.UTF-8 |        |           | =c/postgres          +
           |          |          |                 |             |             |        |           | postgres=CTc/postgres
 test      | postgres | UTF8     | libc            | en_US.UTF-8 | en_US.UTF-8 |        |           | 
(4 rows)
postgres=# 
```

наполнил бд таблицами:

```
fjod@fjod-VirtualBox:/usr/lib/postgresql/17/bin$ sudo -u postgres pgbench -i test
dropping old tables...
NOTICE:  table "pgbench_accounts" does not exist, skipping
NOTICE:  table "pgbench_branches" does not exist, skipping
NOTICE:  table "pgbench_history" does not exist, skipping
NOTICE:  table "pgbench_tellers" does not exist, skipping
creating tables...
generating data (client-side)...
vacuuming... 
creating primary keys...
done in 0.59 s (drop tables 0.02 s, create tables 0.02 s, client-side generate 0.41 s, vacuum 0.10 s, primary keys 0.05 s).
```

проверил что они есть через датагрип на хосте:
![image](https://github.com/user-attachments/assets/f1d3826e-ee24-4fe0-b1c8-ae48783f2056)

запустил бенч:

```
fjod@fjod-VirtualBox:/usr/lib/postgresql/17/bin$ sudo -u postgres pgbench  -c 50 -j 2 -P 10 -T 60 test
pgbench (17.0 (Ubuntu 17.0-1.pgdg22.04+1))
starting vacuum...end.
progress: 10.0 s, 390.5 tps, lat 122.129 ms stddev 157.680, 0 failed
progress: 20.0 s, 404.5 tps, lat 124.257 ms stddev 146.934, 0 failed
progress: 30.0 s, 421.1 tps, lat 118.294 ms stddev 138.258, 0 failed
progress: 40.0 s, 413.0 tps, lat 121.542 ms stddev 154.275, 0 failed
progress: 50.0 s, 411.8 tps, lat 120.788 ms stddev 143.746, 0 failed
progress: 60.0 s, 411.8 tps, lat 120.865 ms stddev 142.046, 0 failed
transaction type: <builtin: TPC-B (sort of)>
scaling factor: 1
query mode: simple
number of clients: 50
number of threads: 2
maximum number of tries: 1
duration: 60 s
number of transactions actually processed: 24577
number of failed transactions: 0 (0.000%)
latency average = 121.727 ms
latency stddev = 148.249 ms
initial connection time = 286.910 ms
tps = 409.839756 (without initial connection time)
```

попробую найти какие-нибудь настройки для моей виртуалки
(взял с сайта https://www.pgconfig.org/#/?max_connections=100&pg_version=16&environment_name=Desktop&total_ram=4&cpus=2&drive_type=HDD&arch=x86-64&os_type=linux)

```
-- Memory Configuration
ALTER SYSTEM SET shared_buffers TO '256MB';
ALTER SYSTEM SET effective_cache_size TO '614MB';
ALTER SYSTEM SET work_mem TO '839kB';
ALTER SYSTEM SET maintenance_work_mem TO '41MB';

-- Checkpoint Related Configuration
ALTER SYSTEM SET min_wal_size TO '2GB';
ALTER SYSTEM SET max_wal_size TO '3GB';
ALTER SYSTEM SET checkpoint_completion_target TO '0.9';
ALTER SYSTEM SET wal_buffers TO '-1';

-- Network Related Configuration
ALTER SYSTEM SET listen_addresses TO '*';
ALTER SYSTEM SET max_connections TO '100';

-- Storage Configuration
ALTER SYSTEM SET random_page_cost TO '4.0';
ALTER SYSTEM SET effective_io_concurrency TO '2';

-- Worker Processes Configuration
ALTER SYSTEM SET max_worker_processes TO '8';
ALTER SYSTEM SET max_parallel_workers_per_gather TO '2';
ALTER SYSTEM SET max_parallel_workers TO '2';
накатил их через датагрип на хосте, рестартанул кластер, проверил что настройки применились. второй раз запущу бенч:
```
получил
`tps = 390.197664 (without initial connection time)`

и еще запуск
`tps = 379.509699 (without initial connection time)`

стало хуже :)

поставил shared_buffers в 1 Гб (виртуалка имеет 4Гб), получил
`tps = 413.410236 (without initial connection time)`

ALTER SYSTEM SET effective_cache_size TO '3048MB';

`tps = 416.363819 (without initial connection time)`

вывод - настройки лучше не трогать

