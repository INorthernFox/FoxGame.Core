using System.Threading.Tasks;
using Core.Loggers;
using Core.UI;
using Core.UI.MainMenus;
using FluentResults;
using UnityEngine;

namespace Core.Initializers.MainMenus
{
    public class MainMenuUiInitializer : MonoBehaviour
    {
        [SerializeField]
        private Transform _canvasRoot;

        public async Task Initialize(
            MainMenuCanvasFactory mainMenuCanvasFactory,
            IGameLogger logger)
        {
            const string id = "main-menu-canvas";
            Result<UICanvasContainer<MainMenuCanvas, MainMenuCanvasView>> createResult =
                await mainMenuCanvasFactory.CreateAsync(id, _canvasRoot);

            if(createResult.IsFailed)
            {
                logger.LogError(
                    IGameLogger.LogSystems.UIWindow,
                    $"Failed to create MainMenuCanvas: {string.Join("; ", createResult.Errors)}",
                    "MainMenuInitializer");
                return;
            }

            UICanvasContainer<MainMenuCanvas, MainMenuCanvasView> container = createResult.Value;
            container.Model.Shove();
        }
    }
}