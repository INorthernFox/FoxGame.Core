using Core.Loggers;
using Core.ResourceManagement;
using Core.ResourceManagement.Load;
using Core.ResourceManagement.Load.Data;
using UnityEngine;

namespace Core.UI
{
    public class UICanvasViewLoader : ComponentBaseResourceLoader<BaseUICanvasView>
    {
        protected override ResourceType ResourceType => ResourceType.UICanvas;

        public UICanvasViewLoader(
            AdressablesPathsConfig paths,
            IGameLogger logger)
            : base(paths, logger)
        {
        }
    }
}
