using System;
using System.Threading.Tasks;
using Core.GameConfigs;
using FluentResults;

namespace Core.GameSettings.Providers
{
    public interface ISettingsProvider
    {
        Type SettingsType { get; }
        string DefaultFileName { get; }
        Task<Result<BaseSettings>> LoadAsync(string fileName);
        Task<Result<BaseSettings>> WriteAsync(string fileName, BaseSettings settings);
    }

    public interface ISettingsProvider<T> : ISettingsProvider
        where T : BaseSettings
    {
        new Task<Result<T>> LoadAsync(string fileName);
        Task<Result<T>> WriteAsync(string fileName, T settings);
    }
}