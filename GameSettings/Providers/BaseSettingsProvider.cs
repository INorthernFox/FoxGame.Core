using System;
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

    public abstract class BaseSettingsProvider<T> : ISettingsProvider<T>
        where T : BaseSettings
    {
        private readonly IFileService _fileService;
        private readonly PersonalizedLogger _logger;

        private readonly Dictionary<string, T> _cache = new();

        private const string SettingsPath = "game_settings";
        private const string SettingsExtension = ".json";

        public Type SettingsType => typeof(T);

        public abstract string DefaultFileName { get; }

        protected abstract bool IsWritableSettings { get; }

        protected BaseSettingsProvider(IFileService fileService, IGameLogger logger)
        {
            _fileService = fileService;
            _logger = new PersonalizedLogger(logger, LogSystems.FileEditor, GetType().Name, this);
        }

        async Task<Result<BaseSettings>> ISettingsProvider.LoadAsync(string fileName)
        {
            Result<T> result = await LoadAsync(fileName);

            return result.IsSuccess
                ? Result.Ok<BaseSettings>(result.Value)
                : Result.Fail<BaseSettings>(result.Errors);
        }

        async Task<Result<BaseSettings>> ISettingsProvider.WriteAsync(string fileName, BaseSettings settings)
        {
            if (settings is not T typedSettings)
                return Result.Fail<BaseSettings>($"Settings type mismatch. Expected {typeof(T).Name}");

            Result<T> result = await WriteAsync(fileName, typedSettings);

            return result.IsSuccess
                ? Result.Ok<BaseSettings>(result.Value)
                : Result.Fail<BaseSettings>(result.Errors);
        }

        public async Task<Result<T>> WriteAsync(string fileName, T settings)
        {
            _logger.LogInfo($"Writing settings: {fileName}");

            if (!IsWritableSettings)
            {
                _logger.LogWarning($"Attempted to write to non-writable settings: {fileName}");
                return Result.Fail<T>($"Can't write {fileName} to settings");
            }

            if (string.IsNullOrEmpty(fileName))
            {
                _logger.LogWarning($"File name is empty");
                return Result.Fail<T>("File name is empty");
            }

            string fullPath = GetFullPath(fileName, DirectoryType.PersistentData);

            Result writeResult = await _fileService.WriteAsync(fullPath, settings);

            if (writeResult.IsFailed)
            {
                _logger.LogError($"Failed to write settings: {fullPath}. Error: {writeResult.Errors.First().Message}");
                return Result.Fail<T>(writeResult.Errors.First());
            }

            _cache[fileName] = settings;
            _logger.LogInfo($"Settings written successfully: {fullPath}");
            return Result.Ok(settings);
        }

        public async Task<Result<T>> LoadAsync(string fileName)
        {
            _logger.LogInfo($"Loading settings: {fileName}");

            if (string.IsNullOrEmpty(fileName))
            {
                _logger.LogError("File name is empty");
                return Result.Fail<T>("File name is empty");
            }

            if (_cache.TryGetValue(fileName, out T cachedValue))
            {
                _logger.LogInfo($"Cache hit: {fileName}");
                return await Task.FromResult(cachedValue);
            }

            if (IsWritableSettings)
            {
                _logger.LogInfo($"Attempting to load writable settings: {fileName}");
                Result<T> writableResult = await TryLoadWritableSettings(fileName);

                if (writableResult.IsSuccess)
                {
                    _cache[fileName] = writableResult.Value;
                    _logger.LogInfo($"Settings loaded successfully from writable location: {fileName}");
                    return writableResult;
                }
            }

            _logger.LogInfo($"Attempting to load readonly settings: {fileName}");
            Result<T> readonlyResult = await TryLoadNotWritableSettings(fileName);

            if (readonlyResult.IsSuccess)
            {
                _cache[fileName] = readonlyResult.Value;
                _logger.LogInfo($"Settings loaded successfully from readonly location: {fileName}");
                return readonlyResult;
            }
            else
            {
                _logger.LogError($"Can't load {GetFullPath(fileName, DirectoryType.StreamingAssets)} {readonlyResult.Errors.First().Message}");
            }

            _logger.LogError($"Failed to load settings from any location: {fileName}");
            return Result.Fail<T>($"Can't load {fileName}");
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