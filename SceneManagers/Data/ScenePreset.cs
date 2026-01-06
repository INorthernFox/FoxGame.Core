using System.Collections.Generic;
using UnityEngine;

namespace Core.SceneManagers.Data
{
    [CreateAssetMenu(fileName = "ScenePreset", menuName = "Game Settings/Scene Management/Scene Preset")]
    public sealed class ScenePreset : ScriptableObject
    {
        [SerializeField]
        private SceneData[] _scenes;

        public IReadOnlyList<SceneData> Scenes => _scenes;

        private void OnValidate()
        {
            ValidateOrders();
        }

        private void ValidateOrders()
        {
            HashSet<int> usedOrders = new();

            for( int index = 0; index < _scenes.Length; index++ )
            {
                SceneData sceneData = _scenes[index];
                sceneData.Name = sceneData.SceneType.ToString();
                _scenes[index] = sceneData;
                
                if(!usedOrders.Add(sceneData.Order))
                {
                    Debug.LogError($"Scene order {sceneData.Order} is duplicated in preset {name}. Each scene entry must have a unique order.", this);
                }
            }
        }
    }
}