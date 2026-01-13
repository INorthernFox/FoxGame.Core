using System.Threading.Tasks;
using Core.SceneManagers.Data;
using FluentResults;
using UnityEngine.SceneManagement;

namespace Core.SceneManagers
{
    public interface ISceneManager
    {
        Task<Result> LoadSceneAsync(SceneType type, LoadSceneMode loadSceneMode = LoadSceneMode.Single);
        Scene ActiveScene { get; }
    }
}