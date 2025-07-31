# Ad Platform Locator Service

## 📌 Назначение
Сервис для поиска рекламных площадок по вложенным локациям. Определяет, какие площадки активны для указанного региона с учетом иерархии локаций.

## 🚀 Быстрый старт
1. Установите .NET 8 SDK
2. Клонируйте репозиторий:
   ```bash
   git clone https://github.com/your-repo/ad-platform-locator.git
   cd ad-platform-locator
   ```
3. Запустите сервис:
   ```bash
   dotnet run --project AdService.Web
   ```

## 🔧 API Endpoints

### POST `/api/AdPlatform/upload`
**Загрузка данных**  
Принимает текстовый файл формата:
```
Яндекс.Директ:/ru
Ревдинский рабочий:/ru/svrd/revda,/ru/svrd/pervik
```

Пример:
```bash
curl -X POST -F "file=@platforms.txt" http://localhost:5000/api/AdPlatform/upload
```

### GET `/api/AdPlatform/search`
**Поиск площадок**  
Параметры:
- `location` - иерархическая локация (например `/ru/svrd`)

Пример:
```bash
curl "http://localhost:5000/api/AdPlatform/search?location=/ru/svrd/revda"
```

Ответ:
```json
{
  "location": "/ru/svrd/revda",
  "count": 3,
  "platforms": ["Яндекс.Директ", "Ревдинский рабочий", "Крутая реклама"]
}
```

## 🌟 Особенности
- Вложенная обработка локаций (регион → город → район)
- In-memory хранилище для максимальной скорости
- Автоматическая нормализаци данных
- Подробное логирование ошибок

## 🛠 Технологии
- .NET 8
- Clean Architecture
- Swagger для документации
- xUnit для тестирования

## 📊 Примеры запросов
Смотрите [примеры тестовых запросов](docs/EXAMPLES.md)
