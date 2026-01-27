using System;
using System.Threading.Tasks;
using FluentResults;

namespace Core.ConfigProviders
{
    public interface IConfigsProvider
    {
        Type ConfigType { get; }
        string DefaultFileName { get; }
        Task<Result<BaseConfig>> LoadConfigAsync(string name);
    }

    public interface IConfigsProvider<T> : IConfigsProvider
        where T : BaseConfig
    {
        new Task<Result<T>> LoadConfigAsync(string name);
    }
}
