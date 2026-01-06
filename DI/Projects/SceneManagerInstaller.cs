using Core.SceneManagers;
using Core.SceneManagers.Data;
using UnityEngine;
using Zenject;

namespace Core.DI.Projects
{
    public class SceneManagerInstaller : MonoBehaviour
    {
        public ScenePreset ScenePreset;
        
        public  void InstallBindings(DiContainer container)
        {
            container.BindInterfacesTo<SceneManager>().AsSingle().WithArguments(ScenePreset);
        }
    }
}