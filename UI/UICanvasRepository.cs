using System.Collections.Generic;
using Core.Loggers;
using FluentResults;

namespace Core.UI
{
    public class UICanvasRepository
    {
        private readonly Dictionary<string, IBaseUICanvas> _windows = new();
        private readonly PersonalizedLogger _logger;

        public UICanvasRepository(IGameLogger logger) =>
            _logger = new PersonalizedLogger(logger, IGameLogger.LogSystems.UIWindow, nameof(UICanvasRepository), this);

        public Result Add(IBaseUICanvas canvas)
        {
            if (canvas == null)
            {
                _logger.LogError("Attempted to add null UI canvas");
                return Result.Fail("Can't add null UI window");
            }

            if (_windows.TryAdd(canvas.ID, canvas))
            {
                _logger.LogInfo($"UI canvas '{canvas.ID}' registered");
                return Result.Ok();
            }

            _logger.LogWarning($"UI canvas '{canvas.ID}' already registered - duplicate add attempted");
            return Result.Fail($"Can't add UI window {canvas.ID} - already exists");
        }

        public Result<IBaseUICanvas> Get(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                _logger.LogError("Attempted to get UI canvas with null/empty ID");
                return Result.Fail("Can't find UI window: id is empty");
            }

            if (_windows.TryGetValue(id, out var window))
                return Result.Ok(window);

            _logger.LogWarning($"UI canvas '{id}' not found");
            return Result.Fail($"Can't find UI window {id}");
        }
    }
}