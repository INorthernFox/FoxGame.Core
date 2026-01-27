using UnityEngine;
using Zenject;

namespace Core.DI.Projects
{
    /// <summary>
    /// Deprecated: Use SettingsInstaller instead.
    /// This installer is kept for backwards compatibility.
    /// </summary>
    public class GeneralGameSettingInstaller : MonoBehaviour
    {
        public void InstallBindings(DiContainer container)
        {
            // Settings providers are now registered in SettingsInstaller
            // via ISettingsProvider interface for use with ISettingsService
        }
    }
}