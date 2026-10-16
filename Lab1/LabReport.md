# Лаба 1 — Свой Docker

## Часть 0 — Свой сервис
Кодовая база для всех лабораторных работ расположена в [LabSolution](../LabSolution).

В рамках текущей лабораторной работы реализован http-сервис `api` на C# c тремя эндпоинтами:
* `GET /health` — возвращает `ok`;
* `GET /eat?mb=N` — выделяет `N` мегабайт памяти и держит их;
* `GET /burn` — нагружает одно ядро CPU в бесконечном цикле.

## Часть 1 — Запусти напрямую

Открываем `LabSolution` и выполняем `dotnet run --project LabApi` (сборка `LabApi.csproj` и запуск).
![LabApiStart](Images/Part1/LabApiStart.png)

Делаем запрос `http://localhost:5232/health`.
![HealthRequest](Images/Part1/HealthRequest.png)

Находим PID процесса — **2992** (на первом скрине в логах тоже можно найти).
![ProcessStatus](Images/Part1/ProcessStatus.png)

## Часть 2 — namespaces
Ранее запускали апи одной командой `dotnet run --project LabApi`. Теперь отдельно соберём апи командой `dotnet build LabApi` и получим `LabApi.dll`

Далее изолированно (изолируем видимость некоторых вещей) меняем имя хоста, поднимаем интерфейс `lo` и запускаем апи командой:

```
unshare \
  --pid --fork \
  --mount --mount-proc \
  --net \
  --uts \
  --ipc \
  --user --map-root-user \
  bash -c '
    hostname lab-container &&
    ip link set lo up &&
    exec dotnet LabApi/bin/Debug/net10.0/LabApi.dll --urls http://localhost:5232
  '
```

Команда изолирует шесть пространств имён:
* `pid` — API получает PID 1 и не видит процессы вне своего namespace;
* `mount` — создаётся отдельная таблица монтирований и свой `/proc`; файлы хоста остаются доступны;
* `net` — интерфейсы и маршруты хоста недоступны, внутри есть только включённый `lo`;
* `uts` — внутри используется своё имя хоста `lab-container`;
* `ipc` — изолированы System V IPC и очереди сообщений POSIX;
* `user` — root внутри соответствует обычному пользователю снаружи и не получает права root на хосте.

Процесс апи снаружи пространства имеет: PID 520, PPID 519 и UID 1000.
![OutsideProcess.png](Images/Part2/OutsideProcess.png)

Заходим внутрь через `sudo nsenter --target 520 --pid --mount --net --uts --ipc --user bash`.

Тот же процесс внутри имеет: PID 1, PPID 0 и UID 0. Также внутри не видно сторонних процессов, другое имя хоста, в сети только `lo` и UID 0, которому принадлежит процесс, снаружи соответствует UID 1000. Изоляция видимости действительно есть.
![InsideProcess.png](Images/Part2/InsideProcess.png)

## Часть 3 — cgroups

## Часть 4 — права

## Часть 5 — Собери свой Docker

## Часть 6 — Образы

## Часть 7 — Когда контейнера мало

## Часть 8 — Мониторинг
