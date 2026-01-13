using System.Collections.Generic;
using FluentResults;

namespace Core.UI
{
    public class UICamvasRepository
    {
        private readonly Dictionary<string, IBaseUICanvas> _windows = new();

        public Result Add(IBaseUICanvas canvas)
        {
            if(canvas == null)
                return Result.Fail($"Can't add null UI window");

            if(_windows.TryAdd(canvas.ID, canvas))
                return Result.Ok();

            return Result.Fail($"Can't add UI window {canvas?.ID}");
        }

        public Result<IBaseUICanvas> Get(string id)
        {
            if(string.IsNullOrEmpty(id))
                return Result.Fail($"Can't find UI window: id is empty");

            return _windows.TryGetValue(id, out IBaseUICanvas window)
                ? Result.Ok(window)
                : Result.Fail($"Can't find UI window {id}");
        }
    }
}