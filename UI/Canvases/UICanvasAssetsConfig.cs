using System;
using System.Collections.Generic;
using System.Linq;
using FluentResults;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace Core.UI.Canvases
{
    [CreateAssetMenu(fileName = "UICanvasAssetsConfig", menuName = "Game Settings/UI/Canvas Assets Config", order = 1)]
    public class UICanvasAssetsConfig : ScriptableObject
    {
        [SerializeField]
        private Data[] _data;

        private Dictionary<UICanvasType, AssetReferenceGameObject> _assetsByType;

        public Result<AssetReferenceGameObject> GetAsset(UICanvasType type)
        {
            _assetsByType ??= _data.ToDictionary(x => x.Type, x => x.Asset);

            return _assetsByType.TryGetValue(type, out AssetReferenceGameObject asset)
                ? Result.Ok(asset)
                : Result.Fail<AssetReferenceGameObject>($"Asset for UICanvasType {type} not found");
        }

        [Serializable]
        public struct Data : ISerializationCallbackReceiver
        {
            [HideInInspector]
            public string Name;

            public UICanvasType Type;
            public AssetReferenceGameObject Asset;

            void ISerializationCallbackReceiver.OnBeforeSerialize()
            {
                Name = Type.ToString();
            }

            void ISerializationCallbackReceiver.OnAfterDeserialize() { }
        }
    }
}
