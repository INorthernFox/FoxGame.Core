using System.Linq;
using System.Threading.Tasks;
using FluentResults;

namespace Core.ConfigProviders
{
    public sealed class ConfigsService : IConfigsService
    {
        private readonly IConfigsProvider[] _providers;

        public ConfigsService(IConfigsProvider[] providers)
        {
            _providers = providers;
        }

        public async Task<Result<T>> LoadAsync<T>(string name) where T : BaseConfig
        {
            var provider = _providers.FirstOrDefault(p => p.ConfigType == typeof(T));

            if (provider == null)
                return Result.Fail<T>($"No provider registered for {typeof(T).Name}");

            Result<BaseConfig> result = await provider.LoadConfigAsync(name);

            return result.IsSuccess
                ? Result.Ok((T)result.Value)
                : Result.Fail<T>(result.Errors);
        }

        public Task<Result<T>> LoadDefaultAsync<T>() where T : BaseConfig
        {
            var provider = _providers.FirstOrDefault(p => p.ConfigType == typeof(T));

            if (provider == null)
                return Task.FromResult(Result.Fail<T>($"No provider registered for {typeof(T).Name}"));

            return LoadAsync<T>(provider.DefaultFileName);
        }
    }
}
