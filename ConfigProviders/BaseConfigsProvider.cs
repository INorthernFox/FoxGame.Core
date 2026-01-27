using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Core.FileEditor;
using FluentResults;

namespace Core.ConfigProviders
{
    public abstract class BaseConfigsProvider<T> : IConfigsProvider<T>
        where T : BaseConfig
    {
        private const string BaseFolderName = "game_configs";

        private readonly IFileService _fileService;

        private readonly Dictionary<string, T> _configs = new();

        public Type ConfigType => typeof(T);

        public abstract string DefaultFileName { get; }
        
        protected BaseConfigsProvider(IFileService fileService)
        {
            _fileService = fileService;
        }

        protected abstract string AddToPathPath(string path);
        
        async Task<Result<BaseConfig>> IConfigsProvider.LoadConfigAsync(string name)
        {
            Result<T> result = await LoadConfigAsync(name);

            return result.IsSuccess
                ? Result.Ok<BaseConfig>(result.Value)
                : Result.Fail<BaseConfig>(result.Errors);
        }

        public async Task<Result<T>> LoadConfigAsync(string name)
        {
            if (string.IsNullOrEmpty(name))
                return Result.Fail("File name is null or empty.");

            name = AddToPathPath(name);
            
            Result<T> tryGetInCacheResult = TryGetInCache(name);

            if(tryGetInCacheResult.IsSuccess)
                return tryGetInCacheResult;

            string basePath = _fileService.GetBasePath(DirectoryType.StreamingAssets);
            string fullPath = Path.Combine(basePath, BaseFolderName, name);

            Result<bool> existsResult = await _fileService.ExistsAsync(fullPath);

            if(existsResult.IsFailed)
                return Result.Fail($"File {name} not exists in {fullPath}");

            Result<T> readResult = await _fileService.ReadAsync<T>(fullPath);

            if(!readResult.IsSuccess)
                return Result.Fail($"File {name} cant read in {fullPath} || Error:{readResult.Errors.First()}");

            AddToCache(name, readResult.Value);
            return Result.Ok(readResult.Value);
        }

        private Result<T> TryGetInCache(string name)
        {
            return _configs.TryGetValue(name, out var config) 
                ? Result.Ok( config) 
                : Result.Fail<T>($"Config {name} not found");
        }

        private void AddToCache(string name, T config) =>
            _configs[name] = config;
    }
}