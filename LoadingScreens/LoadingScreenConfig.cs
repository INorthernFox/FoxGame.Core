using System;
using System.Collections.Generic;
using System.Linq;
using FluentResults;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace Core.LoadingScreens
{
    [CreateAssetMenu(fileName = "LoadingScreenConfig", menuName = "Game Settings/LoadingScreens/Configs", order = 0)]
    public class LoadingScreenConfig : ScriptableObject
    {
        [SerializeField]
        private Data[] _data;

        private Dictionary<LoadingScreenType, AssetReferenceGameObject> _dataDictionary;

        public Result<AssetReferenceGameObject> GetData(LoadingScreenType type)
        {
            _dataDictionary ??= _data.ToDictionary(x => x.Type, x => x.Reference);

            return _dataDictionary.TryGetValue(type, out var result)
                ? result
                : Result.Fail<AssetReferenceGameObject>($"Can't find Loading Screen {type}");
        }

        [Serializable]
        public struct Data : ISerializationCallbackReceiver
        {
            [HideInInspector]
            public string Name;

            public LoadingScreenType Type;
            public AssetReferenceGameObject Reference;

            void ISerializationCallbackReceiver.OnBeforeSerialize()
            {
                Name = Type.ToString();
            }

            void ISerializationCallbackReceiver.OnAfterDeserialize() { }
        }
    }
}