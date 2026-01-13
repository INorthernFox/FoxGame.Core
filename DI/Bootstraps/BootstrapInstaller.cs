using Core.LoadingScreens;
using Core.ResourceManagement.Load.Data;
using Core.StateMachines.Games.States.Games;
using Core.StateMachines.Games.States.LoadGames;
using Core.StateMachines.Games.States.MainMenus;
using Core.StateMachines.Games.States.UnloadGames;
using Zenject;

namespace Core.DI.Bootstraps
{
    public class BootstrapInstaller : MonoInstaller
    {
        public LoadingScreenConfig LoadingScreenConfig;
        public AdressablesPathsConfig AdressablesPathsConfig;
        
        public override void InstallBindings()
        {
            InstallStates();

            Container.Bind<AdressablesPathsConfig>().FromInstance(AdressablesPathsConfig).AsSingle();
            
            InstallLoadingScreen();
        }
        
        private void InstallStates()
        {
            Container.Bind<MainMenuStateFactory>().AsSingle();
            Container.Bind<LoadGameStateFactory>().AsSingle();
            Container.Bind<GameStateFactory>().AsSingle();
            Container.Bind<UnloadGameStateFactory>().AsSingle();
        }

        private void InstallLoadingScreen()
        {
            Container.Bind<LoadingScreenLoader>().AsSingle();
            Container.Bind<LoadingScreenConfig>().FromInstance(LoadingScreenConfig).AsSingle();
            Container.Bind<LoadingScreenService>().AsSingle();
        }
    }
}