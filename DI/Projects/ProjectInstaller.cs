using Zenject;

namespace Core.DI.Projects
{
    public class ProjectInstaller : MonoInstaller
    {
        public SceneManagerInstaller SceneManagerInstaller;
        public LoggerInstaller LoggerInstaller;
        public GameStatesInstaller GameStatesInstaller;
        public LoadingScreenInstaller LoadingScreenInstaller;
        public UIInstaller UIInstaller;
        public AddressablesInstaller AddressablesInstaller;

        public override void InstallBindings()
        {
            LoggerInstaller.InstallBindings(Container);
            SceneManagerInstaller.InstallBindings(Container);
            GameStatesInstaller.InstallBindings(Container);
            LoadingScreenInstaller.InstallBindings(Container);
            UIInstaller.InstallBindings(Container);
            AddressablesInstaller.InstallBindings(Container);
        }
    }
}
