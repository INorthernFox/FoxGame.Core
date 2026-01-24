using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Core.FileEditor;
using Core.GameConfigs;
using Core.Loggers;
using FluentResults;
using static Core.Loggers.IGameLogger;

namespace Core.GameSettings.Providers
{
    public abstract class BaseSettingsProvider<T>
        where T : BaseSettings
    {
        private readonly IFileService _fileService;
        private readonly PersonalizedLogger _logger;

        private readonly Dictionary<string, T> _cache = new();

        private const string SettingsPath = "game_settings";
        private const string SettingsExtension = ".json";

        protected abstract bool IsWritableSettings { get; }

        protected BaseSettingsProvider(IFileService fileService, IGameLogger logger)
        {
            _fileService = fileService;
            _logger = new PersonalizedLogger(logger, LogSystems.FileEditor, GetType().Name, this);
        }

        protected abstract string GetCustomPath(string fileName);

        public async Task<Result<T>> WriteSettings(string fileName, T settings)
        {
            _logger.LogInfo($"Writing settings: {fileName}");

            if (!IsWritableSettings)
            {
                _logger.LogWarning($"Attempted to write to non-writable settings: {fileName}");
                return Result.Fail<T>($"Can't write {fileName} to settings");
            }

            string customPath = GetCustomPath(fileName);

            if (string.IsNullOrEmpty(customPath))
            {
                _logger.LogWarning($"Custom path is empty for fileName: {fileName}");
                return Result.Fail<T>($"File name {fileName} is empty");
            }

            string fullPath = GetFullPath(customPath, DirectoryType.PersistentData);

            Result writeResult = await _fileService.WriteAsync(fullPath, settings);

            if (writeResult.IsFailed)
            {
                _logger.LogError($"Failed to write settings: {fullPath}. Error: {writeResult.Errors.First().Message}");
                return Result.Fail<T>(writeResult.Errors.First());
            }

            _cache[customPath] = settings;
            _logger.LogInfo($"Settings written successfully: {fullPath}");
            return Result.Ok(settings);
        }

        public async Task<Result<T>> Load(string fileName)
        {
            _logger.LogInfo($"Loading settings: {fileName}");

            string customPath = GetCustomPath(fileName);

            if (string.IsNullOrEmpty(customPath))
            {
                _logger.LogError($"Custom path is empty for fileName: {fileName}");
                return Result.Fail<T>($"File name {fileName} is empty");
            }

            if (_cache.TryGetValue(customPath, out T cachedValue))
            {
                _logger.LogInfo($"Cache hit: {customPath}");
                return await Task.FromResult(cachedValue);
            }

            if (IsWritableSettings)
            {
                _logger.LogInfo($"Attempting to load writable settings: {customPath}");
                Result<T> writableResult = await TryLoadWritableSettings(customPath);

                if (writableResult.IsSuccess)
                {
                    _cache[customPath] = writableResult.Value;
                    _logger.LogInfo($"Settings loaded successfully from writable location: {customPath}");
                    return writableResult;
                }
            }

            _logger.LogInfo($"Attempting to load readonly settings: {customPath}");
            Result<T> readonlyResult = await TryLoadNotWritableSettings(customPath);

            if (readonlyResult.IsSuccess)
            {
                _cache[customPath] = readonlyResult.Value;
                _logger.LogInfo($"Settings loaded successfully from readonly location: {customPath}");
                return readonlyResult;
            }
            else
            {
                _logger.LogError($"Can't load {GetFullPath(customPath, DirectoryType.StreamingAssets)} {readonlyResult.Errors.First().Message}");
            }

            _logger.LogError($"Failed to load settings from any location: {customPath}");
            return Result.Fail<T>($"Can't load {customPath}");
        }

        private async Task<Result<T>> TryLoadNotWritableSettings(string customPath)
        {
            string fullPath = GetFullPath(customPath, DirectoryType.StreamingAssets);
            return await TryLoadSettings(fullPath);
        }

        private async Task<Result<T>> TryLoadWritableSettings(string customPath)
        {
            string fullPath = GetFullPath(customPath, DirectoryType.PersistentData);
            return await TryLoadSettings(fullPath);
        }

        private async Task<Result<T>> TryLoadSettings(string fullPath)
        {
            Result<bool> existsResult = await _fileService.ExistsAsync(fullPath);

            if (existsResult.IsFailed || !existsResult.Value)
            {
                _logger.LogWarning($"Settings file not found: {fullPath}");
                return Result.Fail($"Settings file not found: {fullPath}");
            }

            Result<T> loadResult = await _fileService.ReadAsync<T>(fullPath);

            if (loadResult.IsFailed)
            {
                _logger.LogError($"Failed to read settings file: {fullPath}. Error: {loadResult.Errors.First().Message}");
                return Result.Fail<T>(loadResult.Errors.First());
            }

            _logger.LogInfo($"Settings file read successfully: {fullPath}");
            return loadResult;
        }

        private string GetFullPath(string customPath, DirectoryType directoryType)
        {
            string basePath = _fileService.GetBasePath(directoryType);
            string pathWithExtension = customPath.EndsWith(SettingsExtension)
                ? customPath
                : customPath + SettingsExtension;
            return Path.Combine(basePath, SettingsPath, pathWithExtension);
        }
    }
}