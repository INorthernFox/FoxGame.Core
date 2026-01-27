using System.Linq;
using System.Threading.Tasks;
using Core.GameConfigs;
using Core.GameSettings.Providers;
using FluentResults;

namespace Core.GameSettings
{
    public sealed class SettingsService : ISettingsService
    {
        private readonly ISettingsProvider[] _providers;

        public SettingsService(ISettingsProvider[] providers)
        {
            _providers = providers;
        }

        public async Task<Result<T>> LoadAsync<T>(string fileName) where T : BaseSettings
        {
            var provider = _providers.FirstOrDefault(p => p.SettingsType == typeof(T));

            if (provider == null)
                return Result.Fail<T>($"No provider registered for {typeof(T).Name}");

            Result<BaseSettings> result = await provider.LoadAsync(fileName);

            return result.IsSuccess
                ? Result.Ok((T)result.Value)
                : Result.Fail<T>(result.Errors);
        }

        public Task<Result<T>> LoadDefaultAsync<T>() where T : BaseSettings
        {
            var provider = _providers.FirstOrDefault(p => p.SettingsType == typeof(T));

            if (provider == null)
                return Task.FromResult(Result.Fail<T>($"No provider registered for {typeof(T).Name}"));

            return LoadAsync<T>(provider.DefaultFileName);
        }

        public async Task<Result<T>> WriteAsync<T>(T settings, string fileName) where T : BaseSettings
        {
            var provider = _providers.FirstOrDefault(p => p.SettingsType == typeof(T));

            if (provider == null)
                return Result.Fail<T>($"No provider registered for {typeof(T).Name}");

            Result<BaseSettings> result = await provider.WriteAsync(fileName, settings);

            return result.IsSuccess
                ? Result.Ok((T)result.Value)
                : Result.Fail<T>(result.Errors);
        }

        public Task<Result<T>> WriteDefaultAsync<T>(T settings) where T : BaseSettings
        {
            var provider = _providers.FirstOrDefault(p => p.SettingsType == typeof(T));

            if (provider == null)
                return Task.FromResult(Result.Fail<T>($"No provider registered for {typeof(T).Name}"));

            return WriteAsync(settings, provider.DefaultFileName);
        }
    }
}
