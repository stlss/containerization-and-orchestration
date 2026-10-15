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

## Часть 3 — cgroups

## Часть 4 — права

## Часть 5 — Собери свой Docker

## Часть 6 — Образы

## Часть 7 — Когда контейнера мало

## Часть 8 — Мониторинг
