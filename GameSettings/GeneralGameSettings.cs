using System;
using Core.LoadingScreens;

namespace Core.GameConfigs
{
    [Serializable]
    public class GeneralGameSettings : BaseSettings
    {
        public LoadingScreenType LoadingScreenType;
        public string MainMenuCanvasID = "main-menu-canvas";
    }
}