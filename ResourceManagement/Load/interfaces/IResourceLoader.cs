using System.Threading.Tasks;
using FluentResults;
using UnityEngine.AddressableAssets;

namespace Core.ResourceManagement.Load.interfaces
{
    public interface IResourceLoader<T>
    {
        Task<Result<T>> LoadAsync(string key);
        Task<Result<T>> LoadAsync(AssetReference key);
        Task<Result<IAddressableHandle<T>>> LoadWithHandleAsync(string key);
        Task<Result<IAddressableHandle<T>>> LoadWithHandleAsync(AssetReference key);
        Result Unload(string key);
        Result UnloadAll();
    }
}