using System.Threading.Tasks;
using Core.SceneManagers.Data;
using UnityEngine.SceneManagement;

namespace Core.SceneManagers
{
    public interface ISceneManager
    {
        Task<bool> LoadSceneAsync(SceneType type, LoadSceneMode loadSceneMode = LoadSceneMode.Single);
        Scene ActiveScene { get; }
    }
}