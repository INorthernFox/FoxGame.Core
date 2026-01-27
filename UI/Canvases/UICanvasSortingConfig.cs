using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Core.UI.Canvases
{
    [CreateAssetMenu(fileName = "UICanvasSortingConfig", menuName = "Game Settings/UI/Canvas Sorting Config", order = 0)]
    public class UICanvasSortingConfig : ScriptableObject
    {
        [SerializeField]
        private Data[] _data;

        [SerializeField]
        private int _defaultPriority = int.MaxValue;

        private Dictionary<UICanvasType, int> _priorityByType;

        public int GetPriority(UICanvasType type)
        {
            if (type == UICanvasType.None)
                return _defaultPriority;

            _priorityByType ??= _data.ToDictionary(x => x.Type, x => x.Priority);

            return _priorityByType.TryGetValue(type, out int priority)
                ? priority
                : _defaultPriority;
        }

        [Serializable]
        public struct Data : ISerializationCallbackReceiver
        {
            [HideInInspector]
            public string Name;

            public UICanvasType Type;

            [Tooltip("Lower priority = rendered below. MainMenu=0 (bottom), Settings=1 (above MainMenu)")]
            public int Priority;

            void ISerializationCallbackReceiver.OnBeforeSerialize()
            {
                Name = Type.ToString();
            }

            void ISerializationCallbackReceiver.OnAfterDeserialize() { }
        }
    }
}
