using Core.StateMachines.Games;
using Zenject;

namespace Core.DI.Projects
{
    public class ProjectInstaller : MonoInstaller
    {
        public SceneManagerInstaller SceneManagerInstaller;
        public LoggerInstaller LoggerInstaller;

        public override void InstallBindings()
        {
            InstallLogger();
            InstallSceneManager();
            InstallGameStateMachine();
            InstallBootstrapStateFactory();
        }

        private void InstallLogger() =>
            LoggerInstaller.InstallBindings(Container);

        private void InstallSceneManager() =>
            SceneManagerInstaller.InstallBindings(Container);

        private void InstallGameStateMachine() =>
            Container.Bind<GameStateMachine>().AsSingle();

        private void InstallBootstrapStateFactory() =>
            Container.Bind<BootstrapStateFactory>().AsSingle();
    }
}