- создайте новый кластер PostgresSQL 14;
зайдите в созданный кластер под пользователем postgres;
создайте новую базу данных testdb;
зайдите в созданную базу данных под пользователем postgres;
создайте новую схему testnm;
создайте новую таблицу t1 с одной колонкой c1 типа integer;
вставьте строку со значением c1=1

делаю все в ui datagrip. на скринах подключен как пользователь postgres.
создание базы:
![image](https://github.com/user-attachments/assets/ddb9cd32-026c-48cd-b539-f0cf7a624653)
создание схемы, таблицы, колонки, наполнение данными
![image](https://github.com/user-attachments/assets/6fad3f4b-94c6-40fa-9ab4-c3f0afb592d8)


- создайте новую роль readonly;
дайте новой роли право на подключение к базе данных testdb;
дайте новой роли право на использование схемы testnm;
дайте новой роли право на select для всех таблиц схемы testnm;

![image](https://github.com/user-attachments/assets/82bcf1f5-b802-488a-9136-98ed2cef2974)
судя по всему через UI можно только создать роль, проверил что она создалась через
`SELECT * FROM pg_roles WHERE rolname = 'readonly';`
Однако если спросить есть ли у нее права
`SELECT grantee, table_catalog, table_schema, table_name, column_name, privilege_type
FROM information_schema.column_privileges
WHERE grantee = 'readonly';`
то ничего не находится. Так что создал второе подключение к базе testdb, в ней дописал разрешения и проверил что они появпились:
![image](https://github.com/user-attachments/assets/0e3e991a-af78-42b2-b703-1dc33270b3a6)

- создайте пользователя testread с паролем test123;
дайте роль readonly пользователю testread

пользователя можно создать через ui, а вот дать ему роль опять ручками
`grant readonly TO testread;`
и пароль тоже ручками `ALTER USER testread WITH PASSWORD 'test123';`

проверил, вроде получилось:
![image](https://github.com/user-attachments/assets/d12c1930-576f-4313-849f-38881179e2da)
 
- зайдите под пользователем testread в базу данных testdb
![image](https://github.com/user-attachments/assets/8be1716c-067c-44d5-abed-c910dfde66d8)
подключилось

- сделайте `select * from t1;`
получилось? (могло если вы делали сами не по шпаргалке и не упустили один существенный момент про который позже)
![image](https://github.com/user-attachments/assets/4cf928b8-d67c-4470-a985-e76a237b029c)
получилось

а на вставку не работает:
![image](https://github.com/user-attachments/assets/ad829b0c-dce8-4804-8621-daee55bdc9c5)
вроде так и должно быть?

- напишите что именно произошло в тексте домашнего задания; у вас есть идеи почему? ведь права то дали? посмотрите на список таблиц
подсказка в шпаргалке под пунктом 20; а почему так получилось с таблицей (если делали сами и без шпаргалки то может у вас все нормально)

подразумеваю что в шпаргалку по дз намеренно вставили небольшую ошибку, которую я не допустил, создав базу и схему через ui.

- вернитесь в базу данных testdb под пользователем postgres;
удалите таблицу t1;
создайте ее заново но уже с явным указанием имени схемы testnm;
вставьте строку со значением c1=1;
зайдите под пользователем testread в базу данных testdb;
сделайте select * from testnm.t1;;
получилось?

оно и так работает

- есть идеи почему? если нет - смотрите шпаргалку;
как сделать так чтобы такое больше не повторялось? если нет идей - смотрите шпаргалку;
сделайте select * from testnm.t1;;
получилось?;
есть идеи почему? если нет - смотрите шпаргалку;
сделайте select * from testnm.t1;;
получилось?;
ура!;
теперь попробуйте выполнить команду create table t2(c1 integer); insert into t2 values (2);
а как так? нам же никто прав на создание таблиц и insert в них под ролью readonly?;
есть идеи как убрать эти права? если нет - смотрите шпаргалку;

нужно под ролью postgres добавить прав роли readonly
`grant INSERT on all TABLEs in SCHEMA testnm TO readonly;` выполнять в подключении postgres к testdb (в другом подключении не работало)

- если вы справились сами то расскажите что сделали и почему, если смотрели шпаргалку - объясните что сделали и почему выполнив указанные в ней команды
теперь попробуйте выполнить команду create table t3(c1 integer); insert into t2 values (2);
расскажите что получилось и почему;

теперь insert сработал для роли readonly:
![image](https://github.com/user-attachments/assets/fe7ef827-ed81-43d5-918c-c6dc6518775e)

