using Core.Loggers;
using Core.ResourceManagement;
using Core.ResourceManagement.Load;
using Core.ResourceManagement.Load.Data;
using Core.ResourceManagement.Load.interfaces;

namespace Core.UI
{
    public class UICanvasViewLoader : ComponentBaseResourceLoader<BaseUICanvasView>
    {
        protected override ResourceType ResourceType => ResourceType.UICanvas;

        public UICanvasViewLoader(
            AdressablesPathsConfig paths,
            IGameLogger logger,
            IAddressableRegistry registry = null,
            AddressableRetryConfig retryConfig = null)
            : base(paths, logger, registry, retryConfig)
        {
        }
    }
}
