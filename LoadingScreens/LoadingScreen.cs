using TMPro;
using UnityEngine;

namespace Core.LoadingScreens
{
    public class LoadingScreen : MonoBehaviour
    {
        [SerializeField] private TMP_Text _description;
        
        public bool State => gameObject.activeSelf;
        public LoadingScreenType Type { get; private set; }

        public void Initialize(LoadingScreenType type) =>
            Type = type;

        public void Show(string description)
        {
            _description.text = description;
            gameObject.SetActive(true);
        }

        public void Hide()
        {
            _description.text = "";
            gameObject.SetActive(false);
        }
    }
}