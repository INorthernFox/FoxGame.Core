using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using FluentResults;
using UnityEngine;

namespace Core.ResourceManagement.Load.Data
{
    [CreateAssetMenu(fileName = "AdressablesPaths", menuName = "Game Settings/Adressables/Paths", order = 0)]
    public class AdressablesPathsConfig : ScriptableObject
    {
        [SerializeField] private Data[] _data;

        private Dictionary<ResourceType, string> _dataByType;

        public Result<string> GetPath(ResourceType type, string fileName)
        {
            _dataByType ??= _data.ToDictionary(x => x.Type, x => x.Path);

            return !_dataByType.TryGetValue(type, out var path)
                ? Result.Fail<string>($"ResourceType {type} not found")
                : Path.Combine(path, fileName);
        }
        
        [Serializable]
        public struct Data : ISerializationCallbackReceiver
        {
            [HideInInspector]
            public string Name;
            
            public ResourceType Type;
            public string Path;
            
            void ISerializationCallbackReceiver.OnBeforeSerialize()
            {
                Name = Type.ToString();
            }

            void ISerializationCallbackReceiver.OnAfterDeserialize() { }
        }

    }
}