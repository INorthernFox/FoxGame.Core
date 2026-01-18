# FileEditor

Модуль для работы с файловой системой в Unity с поддержкой всех платформ.

## Возможности

- Чтение/запись JSON-файлов с сериализацией
- Проверка существования файлов
- Удаление файлов
- Контроль доступа (read-only / read-write)

## Поддерживаемые платформы

| Платформа | StreamingAssets | PersistentData |
|-----------|-----------------|----------------|
| Windows   | Чтение          | Чтение/Запись  |
| Mac       | Чтение          | Чтение/Запись  |
| Linux     | Чтение          | Чтение/Запись  |
| iOS       | Чтение          | Чтение/Запись  |
| Android   | Чтение          | Чтение/Запись  |
| WebGL     | Чтение          | Только чтение  |

## Использование

### Инъекция через Zenject

```csharp
public class MyService
{
    private readonly IFileService _fileService;

    public MyService(IFileService fileService)
    {
        _fileService = fileService;
    }
}
```

### Чтение файла

```csharp
// Получить базовый путь
string basePath = _fileService.GetBasePath(DirectoryType.PersistentData);
string fullPath = Path.Combine(basePath, "saves", "player.json");

// Прочитать и десериализовать
Result<PlayerData> result = await _fileService.ReadAsync<PlayerData>(fullPath);

if (result.IsSuccess)
{
    PlayerData data = result.Value;
}
else
{
    Debug.LogError(result.Errors[0].Message);
}
```

### Запись файла

```csharp
string basePath = _fileService.GetBasePath(DirectoryType.PersistentData);
string fullPath = Path.Combine(basePath, "saves", "player.json");

var data = new PlayerData { Name = "Player1", Level = 5 };
Result result = await _fileService.WriteAsync(fullPath, data);

if (result.IsFailed)
{
    Debug.LogError(result.Errors[0].Message);
}
```

### Проверка существования

```csharp
Result<bool> result = await _fileService.ExistsAsync(fullPath);

if (result.IsSuccess && result.Value)
{
    // Файл существует
}
```

### Удаление файла

```csharp
Result result = _fileService.Delete(fullPath);
```

## Типы директорий

| DirectoryType    | Описание                          | Права          |
|------------------|-----------------------------------|----------------|
| StreamingAssets  | Файлы из StreamingAssets          | Только чтение  |
| PersistentData   | Пользовательские данные           | Чтение/Запись* |

*На WebGL — только чтение.

## Архитектура

```
FileEditor/
├── IFileService.cs              # Интерфейс сервиса
├── FileService.cs               # Основная реализация
├── FileServiceFactory.cs        # Фабрика для создания сервиса
├── DirectoryType.cs             # Enum типов директорий
├── DirectoryConfig.cs           # Конфигурация директории
├── DirectoryPermission.cs       # Enum прав доступа
├── Readers/
│   ├── IStreamingAssetsReader.cs        # Интерфейс ридера
│   ├── StandardStreamingAssetsReader.cs # Win/Mac/Linux/iOS
│   ├── AndroidStreamingAssetsReader.cs  # Android (UnityWebRequest)
│   └── WebGLStreamingAssetsReader.cs    # WebGL (UnityWebRequest)
├── Serialization/
│   ├── IFileSerializer.cs       # Интерфейс сериализатора
│   └── JsonFileSerializer.cs    # JSON сериализация
└── Tests/
    └── EditMode/
        ├── FileServiceTests.cs
        ├── FileServiceFactoryTests.cs
        ├── DirectoryConfigTests.cs
        ├── JsonFileSerializerTests.cs
        ├── StandardStreamingAssetsReaderTests.cs
        ├── WebGLStreamingAssetsReaderTests.cs
        └── Mocks/
            └── MockStreamingAssetsReader.cs
```

## Тесты

Расположение: `Tests/EditMode/`

| Тест                              | Описание                                |
|-----------------------------------|-----------------------------------------|
| FileServiceTests                  | Основные операции FileService           |
| FileServiceFactoryTests           | Создание сервиса через фабрику          |
| DirectoryConfigTests              | Конфигурация и права доступа            |
| JsonFileSerializerTests           | Сериализация/десериализация JSON        |
| StandardStreamingAssetsReaderTests| Чтение через File.* API                 |
| WebGLStreamingAssetsReaderTests   | Проверка WebGL ридера (условная сборка) |

Запуск тестов:
```bash
Unity.exe -batchmode -projectPath . -runTests -testPlatform editmode
```

## Регистрация в DI

Модуль регистрируется автоматически через `FileEditorInstaller` в `ProjectInstaller`.

Платформо-зависимый выбор ридера происходит через директивы компилятора:
- `UNITY_ANDROID` → AndroidStreamingAssetsReader
- `UNITY_WEBGL` → WebGLStreamingAssetsReader
- Остальные → StandardStreamingAssetsReader
