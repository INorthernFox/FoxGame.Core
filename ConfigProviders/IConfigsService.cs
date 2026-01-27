using System.Threading.Tasks;
using FluentResults;

namespace Core.ConfigProviders
{
    public interface IConfigsService
    {
        Task<Result<T>> LoadAsync<T>(string name) where T : BaseConfig;
        Task<Result<T>> LoadDefaultAsync<T>() where T : BaseConfig;
    }
}
