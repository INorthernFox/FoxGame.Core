using Core.Loggers;
using Core.UI.Factories;

namespace Core.UI.MainMenus
{
    public class MainMenuCanvasFactory : BaseUICanvasFactory<MainMenuCanvas, MainMenuCanvasView>
    {
        protected override UICanvasType CanvasType => UICanvasType.MainMenu;

        public MainMenuCanvasFactory(
            UICanvasAssetsConfig assetsConfig,
            UICanvasViewLoader viewLoader,
            UIForegroundSortingService sortingService,
            UICanvasRepository canvasRepository,
            IGameLogger logger)
            : base(assetsConfig, viewLoader, sortingService, canvasRepository, logger)
        {
        }

        protected override MainMenuCanvas CreateModel(string id)
        {
            return new MainMenuCanvas(id);
        }
    }
}
