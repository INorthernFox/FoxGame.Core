using System.Threading.Tasks;
using Core.GameConfigs;
using FluentResults;

namespace Core.GameSettings
{
    public interface ISettingsService
    {
        Task<Result<T>> LoadAsync<T>(string fileName) where T : BaseSettings;
        Task<Result<T>> LoadDefaultAsync<T>() where T : BaseSettings;
        Task<Result<T>> WriteAsync<T>(T settings, string fileName) where T : BaseSettings;
        Task<Result<T>> WriteDefaultAsync<T>(T settings) where T : BaseSettings;
    }
}
