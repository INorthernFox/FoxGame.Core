using Core.LoadingScreens;
using Core.StateMachines.Games;
using Core.StateMachines.Games.States;
using Core.StateMachines.Games.States.Games;
using Core.StateMachines.Games.States.LoadGames;
using Core.StateMachines.Games.States.LoadMainMenus;
using Core.StateMachines.Games.States.MainMenus;
using Core.StateMachines.Games.States.UnloadGames;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using Zenject;

namespace Core.Initializers.Bootstraps
{
    public class BootstrapInitializer : MonoBehaviour
    {
        [Inject]
        public async void Initialize(
            GameStateMachine gameStateMachine,
            LoadMainMenuStateFactory loadMainMenuStateFactory,
            MainMenuStateFactory mainMenuStateFactory,
            LoadGameStateFactory loadGameStateFactory,
            GameStateFactory gameStateFactory,
            UnloadGameStateFactory unloadGameStateFactory,
            LoadingScreenService loadingScreenService)
        {
            await Addressables.InitializeAsync();

            loadingScreenService.Initialize();
            await loadingScreenService.Preload(LoadingScreenType.Dev);

            AddCoreStates(
                gameStateMachine,
                loadMainMenuStateFactory,
                mainMenuStateFactory,
                loadGameStateFactory,
                gameStateFactory,
                unloadGameStateFactory);

            await gameStateMachine.Set(IGameState.StateType.LoadMainMenu);
        }

        private static void AddCoreStates(
            GameStateMachine gameStateMachine,
            LoadMainMenuStateFactory loadMainMenuStateFactory,
            MainMenuStateFactory mainMenuStateFactory,
            LoadGameStateFactory loadGameStateFactory,
            GameStateFactory gameStateFactory,
            UnloadGameStateFactory unloadGameStateFactory)
        {
            LoadMainMenuState loadMainMenuState = loadMainMenuStateFactory.Create();
            MainMenuState mainMenuState = mainMenuStateFactory.Create();
            LoadGameState loadGameState = loadGameStateFactory.Create();
            GameState gameState = gameStateFactory.Create();
            UnloadGameState unloadGameState = unloadGameStateFactory.Create();

            gameStateMachine.AddState(loadMainMenuState);
            gameStateMachine.AddState(mainMenuState);
            gameStateMachine.AddState(loadGameState);
            gameStateMachine.AddState(gameState);
            gameStateMachine.AddState(unloadGameState);
        }
    }
}
