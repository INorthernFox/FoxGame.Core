using System.Collections.Generic;
using Core.Loggers;
using FluentResults;

namespace Core.UI
{
    public class UICanvasRepository
    {
        private const string LogKey = nameof(UICanvasRepository);

        private readonly Dictionary<string, IBaseUICanvas> _windows = new();
        private readonly IGameLogger _logger;

        private static IGameLogger.LogSystems LogSystem => IGameLogger.LogSystems.UIWindow;

        public UICanvasRepository(IGameLogger logger)
        {
            _logger = logger;
        }

        public Result Add(IBaseUICanvas canvas)
        {
            if(canvas == null)
            {
                _logger.LogError(LogSystem, "Attempted to add null UI canvas", LogKey, this);
                return Result.Fail("Can't add null UI window");
            }

            if(_windows.TryAdd(canvas.ID, canvas))
            {
                _logger.LogInfo(LogSystem, $"UI canvas '{canvas.ID}' registered", LogKey, this);
                return Result.Ok();
            }

            _logger.LogWarning(LogSystem, $"UI canvas '{canvas.ID}' already registered - duplicate add attempted", LogKey, this);
            return Result.Fail($"Can't add UI window {canvas.ID} - already exists");
        }

        public Result<IBaseUICanvas> Get(string id)
        {
            if(string.IsNullOrEmpty(id))
            {
                _logger.LogError(LogSystem, "Attempted to get UI canvas with null/empty ID", LogKey, this);
                return Result.Fail("Can't find UI window: id is empty");
            }

            if (_windows.TryGetValue(id, out IBaseUICanvas window))
            {
                return Result.Ok(window);
            }

            _logger.LogWarning(LogSystem, $"UI canvas '{id}' not found", LogKey, this);
            return Result.Fail($"Can't find UI window {id}");
        }
    }
}