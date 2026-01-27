using System;
using Core.LoadingScreens;

namespace Core.ConfigProviders.GeneralConfigs
{
    [Serializable]
    public class GeneralConfig : BaseConfig
    {
        public LoadingScreenType LoadingScreenType;
        public string MainMenuCanvasID = "main-menu-canvas";
    }
}