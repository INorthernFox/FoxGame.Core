using Core.StateMachines.Games;
using Core.StateMachines.Games.States.Games;
using Core.StateMachines.Games.States.LoadGames;
using Core.StateMachines.Games.States.LoadMainMenus;
using Core.StateMachines.Games.States.MainMenus;
using Core.StateMachines.Games.States.UnloadGames;
using UnityEngine;
using Zenject;

namespace Core.DI.Projects
{
    public class GameStatesInstaller : MonoBehaviour
    {
        public void InstallBindings(DiContainer container)
        {
            container.Bind<GameStateMachine>().AsSingle();
            container.Bind<BootstrapStateFactory>().AsSingle();
            container.Bind<LoadMainMenuStateFactory>().AsSingle();
            container.Bind<MainMenuStateFactory>().AsSingle();
            container.Bind<LoadGameStateFactory>().AsSingle();
            container.Bind<GameStateFactory>().AsSingle();
            container.Bind<UnloadGameStateFactory>().AsSingle();
        }
    }
}
