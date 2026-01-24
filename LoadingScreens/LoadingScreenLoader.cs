using Core.Loggers;
using Core.ResourceManagement;
using Core.ResourceManagement.Load;
using Core.ResourceManagement.Load.Data;
using Core.ResourceManagement.Load.interfaces;

namespace Core.LoadingScreens
{
    public class LoadingScreenLoader : ComponentBaseResourceLoader<LoadingScreen>
    {
        protected override ResourceType ResourceType =>
            ResourceType.LoadingScreen;

        public LoadingScreenLoader(
            AdressablesPathsConfig paths,
            IGameLogger logger,
            IAddressableRegistry registry = null,
            AddressableRetryConfig retryConfig = null)
            : base(paths, logger, registry, retryConfig)
        {
        }
    }
}
