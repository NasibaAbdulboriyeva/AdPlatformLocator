# Ad Platform Locator Service

## 📌 Назначение
Сервис для поиска рекламных площадок по вложенным локациям. Определяет, какие площадки активны для указанного региона с учетом иерархии локаций.

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

## 🛠 Технологии
- .NET 8
- Clean Architecture
- Swagger для документации
- xUnit для тестирования
