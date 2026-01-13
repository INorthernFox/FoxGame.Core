using System.Threading.Tasks;
using Core.Loggers;
using Core.ResourceManagement.Load.Data;
using FluentResults;
using UnityEngine;

namespace Core.ResourceManagement.Load
{
    public abstract class ComponentBaseResourceLoader<T> : BaseResourceLoader<T>
        where T : Component
    {
        protected ComponentBaseResourceLoader(
            AdressablesPathsConfig paths,
            IGameLogger logger)
            : base(paths, logger)
        {
        }

        protected override async Task<Result<T>> Load(object key)
        {
            Result<GameObject> loadResult = await LoadAssetAsync<GameObject>(key);

            if(loadResult.IsFailed)
                return Result.Fail<T>(loadResult.Errors);

            if(!loadResult.Value.TryGetComponent(out T component))
                return Result.Fail<T>($"Can't find component {typeof(T).Name}");

            return Result.Ok(component);
        }
    }
}