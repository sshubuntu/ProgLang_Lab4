# Lab 2-4

## Описание
Приложение с Windows Forms gui.\
Вариант 7.

## Требования
- .NET 8.0 SDK
- Docker
- Windows OS (для Windows Forms)

## Установка и настройка

### 1. Настройка подключения к базе данных

Создать `.env` файл.

Убедитесь, что файл `.env` содержит следующие переменные:
```
DB_HOST="Ваш хост"
DB_PORT="Ваш порт"
DB_NAME="Ваше имя бд"
DB_USER="Ваш юзер"
DB_PASS="Ваш пароль от бд"
```

Если файл `.env` не найден, приложение использует значения по умолчанию:
- Host: localhost
- Port: 5432
- Database: gradebook
- Username: postgres
- Password: postgres
### 2. Запуск Postgres
`docker compose up -d` в папке проекта.

### 3. Запуск приложения
```powershell
dotnet run
```