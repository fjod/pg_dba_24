-  создать ВМ с Ubuntu 20.04/22.04 или развернуть докер любым удобным способом

поставил на VirtualBox убунту:

```
lsb_release -a
No LSB modules are available.
Distributor ID:	Ubuntu
Description:	Ubuntu 22.04.5 LTS
Release:	22.04
Codename:	jammy
```

-  поставить на нем Docker Engine

докер устанавливал по гайду отсюда:

[Ubuntu | Docker Docs](https://docs.docker.com/engine/install/ubuntu/#install-using-the-repository)

для проверки:

```
sudo docker run hello-world
Unable to find image 'hello-world:latest' locally
latest: Pulling from library/hello-world
c1ec31eb5944: Pull complete 
Digest: sha256:d211f485f2dd1dee407a80973c8f129f00d54604d2c90732e8e320e5038a0348
Status: Downloaded newer image for hello-world:latest
Hello from Docker!
This message shows that your installation appears to be working correctly.
To generate this message, Docker took the following steps:
 1. The Docker client contacted the Docker daemon.
 2. The Docker daemon pulled the "hello-world" image from the Docker Hub.
    (amd64)
 3. The Docker daemon created a new container from that image which runs the
    executable that produces the output you are currently reading.
 4. The Docker daemon streamed that output to the Docker client, which sent it
    to your terminal.
To try something more ambitious, you can run an Ubuntu container with:
 $ docker run -it ubuntu bash
Share images, automate workflows, and more with a free Docker ID:
 https://hub.docker.com/
For more examples and ideas, visit:
 https://docs.docker.com/get-started/
```

- сделать каталог /var/lib/postgres

```

fjod@fjod-VirtualBox:/var/lib$ sudo mkdir postgres
fjod@fjod-VirtualBox:/var/lib$ dir
AccountsService    containerd		grub		plymouth	       sudo			upower
acpi-support	   dbus			hp		polkit-1	       swcatalog		usb_modeswitch
alsa		   dhcp			ispell		postgres	       systemd			usbutils
app-info	   dictionaries-common	libreoffice	postgresql	       tpm			VBoxGuestAdditions
apport		   docker		locales		power-profiles-daemon  ubiquity			vim
apt		   dpkg			logrotate	private		       ubuntu-advantage		whoopsie
aspell		   emacsen-common	man-db		python		       ubuntu-drivers-common	xfonts
avahi-autoipd	   fprint		misc		saned		       ubuntu-release-upgrader	xkb
bluetooth	   fwupd		NetworkManager	sgml-base	       ucf			xml-core
boltd		   gdm3			openvpn		shells.state	       udisks2
BrlAPI		   geoclue		os-prober	shim-signed	       unattended-upgrades
colord		   ghostscript		PackageKit	snapd		       update-manager
command-not-found  git			pam		snmp		       update-notifier
```

-  развернуть контейнер с PostgreSQL 15 смонтировав в него /var/lib/postgresql (тут пока небольшая ошибка в команде)

```

sudo docker run --name pg15  -p 5432:5432 -e POSTGRES_USER=postgres -e POSTGRES_PASSWORD=postgres -e PGDATA=/var/lib/postgresql/data -d -v "/var/lib/postgres":/var/lib/postgresql/data postgres:15

```

для проверки что контейнер тут:

```
fjod@fjod-VirtualBox:/var/lib\$ sudo docker ps
CONTAINER ID   IMAGE         COMMAND                  CREATED          STATUS          PORTS                                       NAMES
dad46556afdd   postgres:15   "docker-entrypoint.s…"   12 seconds ago   Up 10 seconds   0.0.0.0:5432->5432/tcp, :::5432->5432/tcp   pg15
```

на удивление смог подключится к ней из хоста из датагрипа (думал что нужно будет что-то редактировать в pg_conf и hba_conf)

```
select version()
PostgreSQL 15.8 (Debian 15.8-1.pgdg120+1) on x86_64-pc-linux-gnu, compiled by gcc (Debian 12.2.0-14) 12.2.0, 64-bit
```

насчет правильно ли подключен volume пока не уверен.

-  развернуть контейнер с клиентом postgres

не понял что такое клиент пг (занятия еще не было), нашел adminer, добавил сеть и рестартанул контейнер postgres

```
fjod@fjod-VirtualBox:/var/lib$ sudo docker network create pgn
435560622e8e3ccb22028b2d41f5722974db6549b12bca3da4479be1ae90c042
fjod@fjod-VirtualBox:/var/lib$ sudo docker run --name pg15_2 --network pgn  -p 5432:5432 -e POSTGRES_USER=postgres -e POSTGRES_PASSWORD=postgres -e PGDATA=/var/lib/postgresql/data -d -v "/var/lib/postgres":/var/lib/postgresql/data postgres:15
sudo docker run --network pgn --link pg15_2:db -p 8080:8080 adminer
```

вообще удобнее было бы через docker-compose это все делать

-  подключится из контейнера с клиентом к контейнеру с сервером и сделать таблицу с парой строк

из админера подключился к пг в браузере 

http://localhost:8080/?pgsql=pg15_2&username=postgres

в интерфейсе создал базу, таблицу и добавил в нее данных. 

-  подключится к контейнеру с сервером с ноутбука/компьютера извне инстансов ЯО/места установки докера

попробовал эти данные получить из хоста в датагрипе

```
test.public> select * from "testTable"
[2024-10-14 12:57:56] 3 rows retrieved starting from 1 in 40 ms (execution: 5 ms, fetching: 35 ms)
```

интересно что имя таблицы почему-то в кавычках

-  удалить контейнер с сервером

```
fjod@fjod-VirtualBox:/var/lib$ sudo docker stop  pg15_2
pg15_2
fjod@fjod-VirtualBox:/var/lib$ sudo docker rm pg15_2
pg15_2
```

-  создать его заново
  
здесь и далее ввожу предыдущие команды без комментариев.
```
fjod@fjod-VirtualBox:/var/lib$ sudo docker run --name pg15_2 --network pgn  -p 5432:5432 -e POSTGRES_USER=postgres -e POSTGRES_PASSWORD=postgres -e PGDATA=/var/lib/postgresql/data -d -v "/var/lib/postgres":/var/lib/postgresql/data postgres:15
```

-  подключится снова из контейнера с клиентом к контейнеру с сервером   

```
fjod@fjod-VirtualBox:/var/lib$ sudo docker run --network pgn --link pg15_2:db -p 8080:8080 adminer
```
подключился в браузере

-  проверить, что данные остались на месте

данные на месте. удивлен, думал что не будет. скрины не знаю как приложить в маркдаун.

-   попробовал сделать инсерт из хоста

   `insert into "testTable" values (5, 'n5')`

проверил в админере, данные появились

-  оставляйте в ЛК ДЗ комментарии что и как вы делали и как боролись с проблемами
  в принципе кроме ошибки с сетью в докере между контейнерами проблем не было
