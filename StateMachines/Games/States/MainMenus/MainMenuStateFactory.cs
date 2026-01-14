using Zenject;

namespace Core.StateMachines.Games.States.MainMenus
{
    public class MainMenuStateFactory : IFactory<MainMenuState>
    {
        public MainMenuState Create() => new();
    }
}
