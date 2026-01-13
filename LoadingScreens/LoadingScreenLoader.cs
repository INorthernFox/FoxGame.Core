using Core.Loggers;
using Core.ResourceManagement;
using Core.ResourceManagement.Load;
using Core.ResourceManagement.Load.Data;

namespace Core.LoadingScreens
{
    public class LoadingScreenLoader : ComponentBaseResourceLoader<LoadingScreen>
    {
        protected override ResourceType ResourceType =>
            ResourceType.LoadingScreen;

        public LoadingScreenLoader(
            AdressablesPathsConfig paths,
            IGameLogger logger)
            : base(paths, logger)
        {
        }
    }
}