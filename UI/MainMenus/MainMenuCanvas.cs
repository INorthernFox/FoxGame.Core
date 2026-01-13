namespace Core.UI.MainMenus
{
    public class MainMenuCanvas : BaseUICanvas
    {
        public override UICanvasType CanvasType => UICanvasType.MainMenu;

        public MainMenuCanvas(string id)
        {
            SetID(id);
        }
    }
}