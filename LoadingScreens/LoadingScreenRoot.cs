using UnityEngine;

namespace Core.LoadingScreens
{
    public class LoadingScreenRoot : MonoBehaviour
    {
        private void Awake() =>
            DontDestroyOnLoad(this);
    }
}