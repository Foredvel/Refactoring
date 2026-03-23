# Battleship (Task2)

Небольшой учебный проект на .NET 8 с простой логикой "Морского боя".

## Требования

- .NET SDK 8.0+

Проверка версии:

```bash
dotnet --version
```

## Структура проекта

- `Battleship.Core` — основная логика (поле, корабли, выстрелы)
- `Battleship.App` — консольное приложение (демо-игра)
- `Battleship.Tests` — unit-тесты (автономный test runner)

## Сборка

Из корня проекта:

```bash
dotnet build Battleship.sln
```

## Запуск приложения

```bash
dotnet run --project Battleship.App/Battleship.App.csproj
```

Управление:

- вводите координаты выстрела в формате: `row col` (например `0 1`)
- для выхода введите: `q`

## Запуск тестов

```bash
dotnet run --project Battleship.Tests/Battleship.Tests.csproj
```

Ожидаемый формат вывода:

- `[PASS] <имя теста>` для успешных тестов
- `[FAIL] <имя теста>: <ошибка>` для упавших
- итоговая строка: `Total: N, Failed: M`

Ненулевой код возврата означает, что есть проваленные тесты.
