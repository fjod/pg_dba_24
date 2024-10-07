-  захожу в psql в двух терминалах, ОС Ubuntu в Virtual Box

`sudo -u postgres psql`

-  выкл автокоммит

`\set AUTOCOMMIT off`

-  в первой сессии новую таблицу и наполнить ее данными

`create table persons(id serial, first_name text, second_name text); insert into persons(first_name, second_name) values('ivan', 'ivanov'); insert into persons(first_name, second_name) values('petr', 'petrov'); commit;`

получил `WARNING:  there is no transaction in progress`

-  Ввел `show transaction isolation level;`

```
transaction_isolation
-----------------------
read committed
(1 row)
```

-  начать новую транзакцию в обоих сессиях с дефолтным (не меняя) уровнем изоляции - `BEGIN; `или `START TRANSACTION;`

-  в первой сессии добавить новую запись `insert into persons(first_name, second_name) values('sergey', 'sergeev');`

-  сделать select from persons во второй сессии

```
select * from persons;
id | first_name | second_name
----+------------+-------------
1 | ivan       | ivanov
2 | petr       | petrov
(2 rows)
```

-  видите ли вы новую запись и если да то почему?

запись не видим, т.к. уровень транзакции `read commited` означает, что другие транзакции видят изменения из первой транзакции только если в ней выполнено `commit`. Пока этого не сделали, изменений видно не будет.

-  завершить первую транзакцию - `commit;`

-  сделать `select from persons` во второй сессии

-  видите ли вы новую запись и если да то почему?

```
select * from persons;
id | first_name | second_name
----+------------+-------------
1 | ivan       | ivanov
2 | petr       | petrov
3 | sergey     | sergeev
(3 rows)
```

теперь видим, т.к. первая транзакция закоммитила свои изменения, уровень изоляции `read commited`, т.е. другие транзакции видят эти изменения. При данном уровне транзакции можно получить разные результаты на чтении. 

-  начать новые но уже repeatable read транзации - set transaction isolation level repeatable read;

```
postgres=# begin;
BEGIN
postgres=*# set transaction isolation level repeatable read;
SET
```

-  в первой сессии добавить новую запись `insert into persons(first_name, second_name) values('sveta', 'svetova');`

-  сделать `select from persons` во второй сессии

-  видите ли вы новую запись и если да то почему?

не видно. При repeatable read любое чтение внутри транзакции будет всегда с одинаковым результатом, так как PostgreSQL создает “снимок” данных в момент начала транзакции. 

-  завершить первую транзакцию - `commit;`

-  сделать `select from persons` во второй сессии

-  видите ли вы новую запись и если да то почему?

не видно, данные все равно из снепшота, который был сделан на момент старта второй транзакции

-  завершить вторую транзакцию

-  сделать `select * from persons` во второй сессии

-  видите ли вы новую запись и если да то почему?

теперь видно, мы вышли из второй транзакции и теперь читаем более свежее состояние БД.
