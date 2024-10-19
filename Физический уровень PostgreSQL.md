-  создайте виртуальную машину c Ubuntu 20.04/22.04 LTS в ЯО/Virtual Box/докере 

-  поставьте на нее PostgreSQL 15
  
установил 17 в Virtual Box

- проверьте что кластер запущен через `sudo -u postgres pg_lsclusters`

```
fjod@fjod-VirtualBox:/etc/postgresql/17/main$ sudo -u postgres pg_lsclusters
Ver Cluster Port Status Owner    Data directory              Log file
17  main    5433 online postgres /var/lib/postgresql/17/main /var/log/postgresql/postgresql-17-main.log
```

- зайдите из под пользователя postgres в psql и сделайте произвольную таблицу с произвольным содержимым

создал из хоста через датагрип, проверил в вм:

```
fjod@fjod-VirtualBox:/etc/postgresql/17/main$ sudo -u postgres psql
[sudo] password for fjod: 
psql (17.0 (Ubuntu 17.0-1.pgdg22.04+1))
Type "help" for help.
postgres=# select * from test_dba;
 id | text 
----+------
  1 | '1'
  2 | '2'
(2 rows)
```

- остановите postgres например через sudo -u postgres pg_ctlcluster 15 main stop

```
fjod@fjod-VirtualBox:~$ sudo systemctl stop postgresql@17-main.service
fjod@fjod-VirtualBox:~$ systemctl status postgresql@17-main.service
○ postgresql@17-main.service - PostgreSQL Cluster 17-main
     Loaded: loaded (/lib/systemd/system/postgresql@.service; enabled-runtime; vendor preset: enabled)
     Active: inactive (dead) since Sat 2024-10-19 09:25:59 MSK; 2s ago
    Process: 3479 ExecStop=/usr/bin/pg_ctlcluster --skip-systemctl-redirect -m fast 17-main stop (code=exited, status=0/SUCCESS)
   Main PID: 832 (code=exited, status=0/SUCCESS)
        CPU: 904ms
окт 19 09:20:31 fjod-VirtualBox systemd[1]: Starting PostgreSQL Cluster 17-main...
окт 19 09:20:37 fjod-VirtualBox systemd[1]: Started PostgreSQL Cluster 17-main.
окт 19 09:25:59 fjod-VirtualBox systemd[1]: Stopping PostgreSQL Cluster 17-main...
окт 19 09:25:59 fjod-VirtualBox systemd[1]: postgresql@17-main.service: Deactivated successfully.
окт 19 09:25:59 fjod-VirtualBox systemd[1]: Stopped PostgreSQL Cluster 17-main.
```

проверил, запрос из хоста не выполняется

`Connection to 192.168.56.101:5433 refused. Check that the hostname and port are correct and that the postmaster is accepting TCP/ IP connections.`

- создайте новый диск к ВМ размером 10GB

создал через интерфейс VirtualBox по адресу E:\msiDownloads_wi11\NewVirtualDisk_1.vdi. ВМ не закрывал.

- добавьте свеже-созданный диск к виртуальной машине - надо зайти в режим ее редактирования и дальше выбрать пункт attach existing disk
```
  It's generally not possible to add an existing virtual hard disk (VDI) directly to a running VirtualBox VM through the graphical user interface.
   While you can add a new disk while the VM is running (provided you've pre-allocated sufficient ports on the controller ),
  attaching an existing VDI usually requires the VM to be powered off. 
```
видимо придется закрывать машину, чтобы подключить диск. В меню настроек опции подключения дисков покрашены серым, можно только вставить оптический диск в дисковод.

- проинициализируйте диск согласно инструкции и подмонтировать файловую систему, только не забывайте менять имя диска на актуальное, в вашем случае это скорее всего будет /dev/sdb - https://www.digitalocean.com/community/tutorials/how-to-partition-and-format-storage-devices-in-linux

 сделал через графический интерфейс убунты, назвал sdb
 ```
 fjod@fjod-VirtualBox:~$ lsblk
NAME MAJ:MIN RM   SIZE RO TYPE MOUNTPOINTS
loop0
       7:0    0     4K  1 loop /snap/bare/5
loop1
       7:1    0  61,9M  1 loop /snap/core20/1405
loop2
       7:2    0    64M  1 loop /snap/core20/2379
loop3
       7:3    0  74,3M  1 loop /snap/core22/1612
loop4
       7:4    0  74,2M  1 loop /snap/core22/1621
loop5
       7:5    0 155,6M  1 loop /snap/firefox/1232
loop6
       7:6    0 271,4M  1 loop /snap/firefox/4955
loop7
       7:7    0 349,7M  1 loop /snap/gnome-3-38-2004/143
loop8
       7:8    0 248,8M  1 loop /snap/gnome-3-38-2004/99
loop9
       7:9    0 505,1M  1 loop /snap/gnome-42-2204/176
loop10
       7:10   0  81,3M  1 loop /snap/gtk-common-themes/1534
loop11
       7:11   0  91,7M  1 loop /snap/gtk-common-themes/1535
loop12
       7:12   0  45,9M  1 loop /snap/snap-store/575
loop13
       7:13   0  38,8M  1 loop /snap/snapd/21759
loop14
       7:14   0   284K  1 loop /snap/snapd-desktop-integration/10
sda    8:0    0  31,7G  0 disk 
├─sda1
│      8:1    0     1M  0 part 
├─sda2
│      8:2    0   513M  0 part /boot/efi
└─sda3
       8:3    0  31,2G  0 part /
sdb    8:16   0    10G  0 disk /media/fjod/sdb
sr0   11:0    1  61,1M  0 rom  /media/fjod/VBox_GAs_6.1.50
```

а вот и он, размером 10 Гб.

- перезагрузите инстанс и убедитесь, что диск остается примонтированным (если не так смотрим в сторону fstab)

  перезагрузил виртуалку при помощи пользовательского интерфейса, проверил что диск на месте (команда lsblk его показывает как и в прошлый раз)

- сделайте пользователя postgres владельцем /mnt/data - `chown -R postgres:postgres /mnt/data/`

```
fjod@fjod-VirtualBox:~$ sudo chown -R postgres:postgres /media/fjod/sdb
fjod@fjod-VirtualBox:/media/fjod$ ls
sdb  VBox_GAs_6.1.50
fjod@fjod-VirtualBox:/media/fjod$ ls -la
total 15
drwxr-x---+ 4 root     root     4096 окт 19 09:22 .
drwxr-xr-x  3 root     root     4096 сен 18 16:22 ..
drwx------  3 postgres postgres 4096 окт 19 09:22 sdb
dr-xr-xr-x  5 fjod     fjod     2570 янв 11  2024 VBox_GAs_6.1.50

```

- перенесите содержимое /var/lib/postgres/15 в /mnt/data - mv /var/lib/postgresql/15/mnt/data

```
fjod@fjod-VirtualBox:/$ sudo mv /var/lib/postgresql/17 /media/fjod/sdb
```


- попытайтесь запустить кластер - sudo -u postgres pg_ctlcluster 15 main start

```
fjod@fjod-VirtualBox:/$ sudo -u postgres pg_ctlcluster 17 main start
Error: /var/lib/postgresql/17/main is not accessible or does not exist
```

- напишите получилось или нет и почему
служба запуска пытается найти кластер по старому адресу на диске. нужно ей указать на новое место


- задание: найти конфигурационный параметр в файлах раположенных в /etc/postgresql/15/main который надо поменять и поменяйте его
- напишите что и почему поменяли

  изменил
```
   data_directory = '/media/fjod/sdb/postgresql/17/main'              # use data in another directory
```

- попытайтесь запустить кластер - sudo -u postgres pg_ctlcluster 15 main start
- напишите получилось или нет и почему
- зайдите через через psql и проверьте содержимое ранее созданной таблицы

```
fjod@fjod-VirtualBox:/media/fjod$ sudo -u postgres pg_ctlcluster 17 main start
Warning: the cluster will not be running as a systemd service. Consider using systemctl:
  sudo systemctl start postgresql@17-main
fjod@fjod-VirtualBox:/media/fjod$ sudo -u postgres psql
psql (17.0 (Ubuntu 17.0-1.pgdg22.04+1))
Type "help" for help.
postgres=# select * from test;
 t1 
----
  1
(1 row)
```

домашка заняла несколько часов, кластер не хотел запускаться с ошибками
`Error: /media/fjod/sdb/postgresql/17/main is not accessible or does not exist` или `pg_ctl: could not access directory "директория": Permission denied`
вероятно нужно было стартовать не `systemctl start postgresql@17-main.service` а при помощи команды `pg_ctlcluster 17 main start`


