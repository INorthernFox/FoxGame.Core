using System.Threading.Tasks;
using Core.ConfigProviders;
using Core.ConfigProviders.GeneralConfigs;
using Core.GameConfigs;
using Core.GameSettings;
using Core.Loggers;
using Core.UI;
using Core.UI.Canvases;
using Core.UI.Canvases.MainMenus;
using FluentResults;
using UnityEngine;

namespace Core.Initializers.MainMenus
{
    public class MainMenuUiInitializer : MonoBehaviour
    {
        [SerializeField] private Transform _canvasRoot;

        public async Task Initialize(
            MainMenuCanvasFactory mainMenuCanvasFactory,
            IConfigsService configsService,
            IGameLogger baseLogger)
        {
            PersonalizedLogger logger = new(baseLogger, IGameLogger.LogSystems.Initializers, nameof(MainMenuUiInitializer), this);

            Result<GeneralConfig> loadSettingsResult = await configsService.LoadDefaultAsync<GeneralConfig>();

            if(loadSettingsResult.IsFailed)
            {
                logger.LogError($"Failed to load GeneralGameSettings: {string.Join("; ", loadSettingsResult.Errors)}");
                return;
            }
            
            Result<UICanvasContainer<MainMenuCanvas, MainMenuCanvasView>> createResult = 
                await mainMenuCanvasFactory.CreateAsync(loadSettingsResult.Value.MainMenuCanvasID, _canvasRoot);

            if (createResult.IsFailed)
            {
                logger.LogError($"Failed to create MainMenuCanvas: {string.Join("; ", createResult.Errors)}");
                return;
            }

            UICanvasContainer<MainMenuCanvas, MainMenuCanvasView> container = createResult.Value;
            container.Model.Shove();
        }
    }
}