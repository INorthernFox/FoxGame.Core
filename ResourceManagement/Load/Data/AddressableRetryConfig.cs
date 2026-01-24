using UnityEngine;

namespace Core.ResourceManagement.Load.Data
{
    [CreateAssetMenu(fileName = "AddressableRetryConfig", menuName = "Game Settings/Adressables/Retry Config", order = 1)]
    public class AddressableRetryConfig : ScriptableObject
    {
        [SerializeField] private int _maxRetryAttempts = 3;
        [SerializeField] private int _baseDelayMs = 100;
        [SerializeField] private int _maxDelayMs = 2000;
        [SerializeField] private bool _useExponentialBackoff = true;

        public int MaxRetryAttempts => _maxRetryAttempts;
        public int BaseDelayMs => _baseDelayMs;
        public int MaxDelayMs => _maxDelayMs;
        public bool UseExponentialBackoff => _useExponentialBackoff;
    }
}
