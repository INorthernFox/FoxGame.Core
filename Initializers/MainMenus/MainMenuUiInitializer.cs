using System.Threading.Tasks;
using Core.Loggers;
using Core.UI;
using Core.UI.MainMenus;
using UnityEngine;

namespace Core.Initializers.MainMenus
{
    public class MainMenuUiInitializer : MonoBehaviour
    {
        [SerializeField] private Transform _canvasRoot;

        public async Task Initialize(
            MainMenuCanvasFactory mainMenuCanvasFactory,
            IGameLogger baseLogger)
        {
            var logger = new PersonalizedLogger(baseLogger, IGameLogger.LogSystems.Initializers, nameof(MainMenuUiInitializer), this);

            const string id = "main-menu-canvas";
            var createResult = await mainMenuCanvasFactory.CreateAsync(id, _canvasRoot);

            if (createResult.IsFailed)
            {
                logger.LogError($"Failed to create MainMenuCanvas: {string.Join("; ", createResult.Errors)}");
                return;
            }

            var container = createResult.Value;
            container.Model.Shove();
        }
    }
}