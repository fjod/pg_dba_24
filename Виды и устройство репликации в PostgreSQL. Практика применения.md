```
Описание/Пошаговая инструкция выполнения домашнего задания:
На 1 ВМ создаем таблицы test для записи, test2 для запросов на чтение.
Создаем публикацию таблицы test и подписываемся на публикацию таблицы test2 с ВМ №2.
На 2 ВМ создаем таблицы test2 для записи, test для запросов на чтение.
Создаем публикацию таблицы test2 и подписываемся на публикацию таблицы test1 с ВМ №1.
3 ВМ использовать как реплику для чтения и бэкапов (подписаться на таблицы из ВМ №1 и №2 ).
```

Одна ВМ у меня была, создал еще две. Клонированием не получилось. Адрес первой 192.168.56.101, для других 102 и 103 соответственно. 
Проверил, пинг проходит. Во всех postgresql.conf написал wal_level = logical.

hba_conf одинаковые: ![image](https://github.com/user-attachments/assets/9167237b-64c1-4842-91d7-a7ea958ac53e)


На 101 и 102 создаю таблицы test1 test2.
```
-- Create the student table
CREATE TABLE test1 (
                         id SERIAL PRIMARY KEY,
                         name VARCHAR(100)
);

-- Insert 10 rows of generated data
INSERT INTO test2 (name)
SELECT 'Student ' || generate_series(1, 10);
```
![image](https://github.com/user-attachments/assets/fc797f11-f632-4d4e-80dd-fa49e7b9b477)

Создаю публикации и подписки:
![image](https://github.com/user-attachments/assets/1b6f3e5d-a50d-4caa-bb81-7d18a06dc009)

При записи в test1 на сервере 101 она появилась в той же таблице но на сервере 102:
![image](https://github.com/user-attachments/assets/88d7b006-1585-4037-81ba-286abb43ca99)


Верно и для test2, в обратную сторону:
![image](https://github.com/user-attachments/assets/6055b34a-3a17-4cbb-8c13-9971f9fa2237)


Добавил публикации и подписки на 103, добаил данные в таблицу на 102, они появились на 103:
![image](https://github.com/user-attachments/assets/0de4c9b9-b5f1-4c73-bb0b-b6e76a3b2e09)

