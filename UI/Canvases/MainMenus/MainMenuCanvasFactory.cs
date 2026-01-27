using Core.Loggers;
using Core.StateMachines.Games;
using Core.UI.Canvases.Factories;

namespace Core.UI.Canvases.MainMenus
{
    public class MainMenuCanvasFactory : BaseUICanvasFactory<MainMenuCanvas, MainMenuCanvasView>
    {
        private readonly GameStateMachine _gameStateMachine;
        protected override UICanvasType CanvasType => UICanvasType.MainMenu;

        public MainMenuCanvasFactory(
            UICanvasAssetsConfig assetsConfig,
            UICanvasViewLoader viewLoader,
            UIForegroundSortingService sortingService,
            UICanvasRepository canvasRepository,
            IGameLogger logger,
            GameStateMachine gameStateMachine)
            : base(assetsConfig, viewLoader, sortingService, canvasRepository, logger)
        {
            _gameStateMachine = gameStateMachine;
        }

        protected override MainMenuCanvas CreateModel(string id)
        {
            return new MainMenuCanvas(id, _gameStateMachine);
        }
    }
}
