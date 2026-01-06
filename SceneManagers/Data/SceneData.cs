using System;
using UnityEditor;
using UnityEngine;

namespace Core.SceneManagers.Data
{
    [Serializable]
    public class SceneData
    {
        [HideInInspector]
        public string Name;
        public int Order;
        public SceneType  SceneType;
        
#if UNITY_EDITOR
        public SceneAsset Scene; 
#endif
    }
}