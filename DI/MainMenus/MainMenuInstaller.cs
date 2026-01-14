using Core.UI.MainMenus;
using Zenject;

namespace Core.DI.MainMenus
{
    public class MainMenuInstaller : MonoInstaller
    {
        public override void InstallBindings()
        {
            Container.Bind<MainMenuCanvasFactory>().AsSingle();
        }
    }
}