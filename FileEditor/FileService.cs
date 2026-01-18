using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Core.FileEditor.Readers;
using Core.FileEditor.Serialization;
using Core.Loggers;
using FluentResults;

namespace Core.FileEditor
{
    public sealed class FileService : IFileService
    {
        private readonly IGameLogger _logger;
        private readonly IFileSerializer _serializer;
        private readonly IStreamingAssetsReader _streamingAssetsReader;
        private readonly IReadOnlyDictionary<DirectoryType, DirectoryConfig> _directories;

        public FileService(
            IGameLogger logger,
            IFileSerializer serializer,
            IStreamingAssetsReader streamingAssetsReader,
            IReadOnlyDictionary<DirectoryType, DirectoryConfig> directories)
        {
            _logger = logger;
            _serializer = serializer;
            _streamingAssetsReader = streamingAssetsReader;
            _directories = directories;
        }

        public string GetBasePath(DirectoryType directoryType)
        {
            if (directoryType == DirectoryType.None)
            {
                return string.Empty;
            }

            return _directories.TryGetValue(directoryType, out var config) ? config.Path : string.Empty;
        }

        public async Task<Result<T>> ReadAsync<T>(string fullPath) where T : class
        {
            var validationResult = ValidateReadAccess(fullPath, out var directoryType);
            if (validationResult.IsFailed)
            {
                return validationResult.ToResult<T>();
            }

            var readResult = directoryType == DirectoryType.StreamingAssets
                ? await _streamingAssetsReader.ReadTextAsync(fullPath)
                : await ReadTextFromFileSystemAsync(fullPath);

            if (readResult.IsFailed)
            {
                return readResult.ToResult<T>();
            }

            var deserializeResult = _serializer.Deserialize<T>(readResult.Value);
            if (deserializeResult.IsFailed)
            {
                _logger.LogError(IGameLogger.LogSystems.FileEditor, $"Failed to deserialize: {fullPath}", nameof(ReadAsync), this);
            }
            else
            {
                _logger.LogInfo(IGameLogger.LogSystems.FileEditor, $"Read: {fullPath}", nameof(ReadAsync), this);
            }

            return deserializeResult;
        }

        public async Task<Result> WriteAsync<T>(string fullPath, T data) where T : class
        {
            var validationResult = ValidateWriteAccess(fullPath);
            if (validationResult.IsFailed)
            {
                return validationResult;
            }

            var serializeResult = _serializer.Serialize(data);
            if (serializeResult.IsFailed)
            {
                _logger.LogError(IGameLogger.LogSystems.FileEditor, $"Failed to serialize: {fullPath}", nameof(WriteAsync), this);
                return serializeResult.ToResult();
            }

            try
            {
                EnsureDirectoryExists(fullPath);
                await File.WriteAllTextAsync(fullPath, serializeResult.Value);
                _logger.LogInfo(IGameLogger.LogSystems.FileEditor, $"Wrote: {fullPath}", nameof(WriteAsync), this);
                return Result.Ok();
            }
            catch (Exception ex)
            {
                var error = $"Failed to write {fullPath}: {ex.Message}";
                _logger.LogError(IGameLogger.LogSystems.FileEditor, error, nameof(WriteAsync), this);
                return Result.Fail(error);
            }
        }

        public async Task<Result<bool>> ExistsAsync(string fullPath)
        {
            var validationResult = ValidateReadAccess(fullPath, out var directoryType);
            if (validationResult.IsFailed)
            {
                return validationResult.ToResult<bool>();
            }

            if (directoryType == DirectoryType.StreamingAssets)
            {
                return await _streamingAssetsReader.ExistsAsync(fullPath);
            }

            try
            {
                return Result.Ok(File.Exists(fullPath));
            }
            catch (Exception ex)
            {
                var error = $"Failed to check existence {fullPath}: {ex.Message}";
                _logger.LogError(IGameLogger.LogSystems.FileEditor, error, nameof(ExistsAsync), this);
                return Result.Fail<bool>(error);
            }
        }

        public Result Delete(string fullPath)
        {
            var validationResult = ValidateWriteAccess(fullPath);
            if (validationResult.IsFailed)
            {
                return validationResult;
            }

            try
            {
                if (!File.Exists(fullPath))
                {
                    _logger.LogWarning(IGameLogger.LogSystems.FileEditor, $"File not found: {fullPath}", nameof(Delete), this);
                    return Result.Fail($"File not found: {fullPath}");
                }

                File.Delete(fullPath);
                _logger.LogInfo(IGameLogger.LogSystems.FileEditor, $"Deleted: {fullPath}", nameof(Delete), this);
                return Result.Ok();
            }
            catch (Exception ex)
            {
                var error = $"Failed to delete {fullPath}: {ex.Message}";
                _logger.LogError(IGameLogger.LogSystems.FileEditor, error, nameof(Delete), this);
                return Result.Fail(error);
            }
        }

        private Result ValidateReadAccess(string fullPath, out DirectoryType directoryType)
        {
            directoryType = DirectoryType.None;

            var resolveResult = ResolveDirectoryType(fullPath, out directoryType, out var config);
            if (resolveResult.IsFailed)
            {
                return resolveResult;
            }

            if (!config.CanRead)
            {
                var error = $"Read access denied: {fullPath}";
                _logger.LogError(IGameLogger.LogSystems.FileEditor, error, nameof(ValidateReadAccess), this);
                return Result.Fail(error);
            }

            return Result.Ok();
        }

        private Result ValidateWriteAccess(string fullPath)
        {
            var resolveResult = ResolveDirectoryType(fullPath, out _, out var config);
            if (resolveResult.IsFailed)
            {
                return resolveResult;
            }

            if (!config.CanWrite)
            {
                var error = $"Write access denied: {fullPath}";
                _logger.LogError(IGameLogger.LogSystems.FileEditor, error, nameof(ValidateWriteAccess), this);
                return Result.Fail(error);
            }

            return Result.Ok();
        }

        private Result ResolveDirectoryType(string fullPath, out DirectoryType directoryType, out DirectoryConfig config)
        {
            directoryType = DirectoryType.None;
            config = null;

            if (string.IsNullOrEmpty(fullPath))
            {
                var error = "Path is null or empty";
                _logger.LogError(IGameLogger.LogSystems.FileEditor, error, nameof(ResolveDirectoryType), this);
                return Result.Fail(error);
            }

            var normalizedPath = NormalizePath(fullPath);

            foreach (var kvp in _directories)
            {
                var normalizedBasePath = NormalizePath(kvp.Value.Path);
                if (normalizedPath.StartsWith(normalizedBasePath, StringComparison.OrdinalIgnoreCase))
                {
                    directoryType = kvp.Key;
                    config = kvp.Value;
                    return Result.Ok();
                }
            }

            var accessError = $"Access denied - path not in allowed directories: {fullPath}";
            _logger.LogError(IGameLogger.LogSystems.FileEditor, accessError, nameof(ResolveDirectoryType), this);
            return Result.Fail(accessError);
        }

        private string NormalizePath(string path)
        {
            return path.Replace('\\', '/').TrimEnd('/');
        }

        private async Task<Result<string>> ReadTextFromFileSystemAsync(string fullPath)
        {
            try
            {
                if (!File.Exists(fullPath))
                {
                    return Result.Fail<string>($"File not found: {fullPath}");
                }

                var content = await File.ReadAllTextAsync(fullPath);
                return Result.Ok(content);
            }
            catch (Exception ex)
            {
                return Result.Fail<string>($"Failed to read file: {ex.Message}");
            }
        }

        private void EnsureDirectoryExists(string filePath)
        {
            var directory = Path.GetDirectoryName(filePath);
            if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }
        }
    }
}
