using Core.UI;
using UnityEngine;
using Zenject;

namespace Core.DI.Projects
{
    public class UIInstaller : MonoBehaviour
    {
        public UICanvasSortingConfig UICanvasSortingConfig;
        public UICanvasAssetsConfig UICanvasAssetsConfig;

        public void InstallBindings(DiContainer container)
        {
            container.Bind<UICanvasSortingConfig>().FromInstance(UICanvasSortingConfig).AsSingle();
            container.Bind<UICanvasAssetsConfig>().FromInstance(UICanvasAssetsConfig).AsSingle();
            container.Bind<UIForegroundSortingService>().AsSingle().WithArguments(0);
            container.Bind<UICanvasViewLoader>().AsSingle();
            container.Bind<UICanvasRepository>().AsSingle();
        }
    }
}
