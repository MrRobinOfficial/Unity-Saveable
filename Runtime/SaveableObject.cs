using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using UnityEngine;

namespace Saveable
{
    [AddComponentMenu("Tools/Saveables/Saveable [Object]"), DisallowMultipleComponent]
    public sealed class SaveableObject : MonoBehaviour, ISerializationCallbackReceiver
    {
        [SerializeField, HideInInspector] string m_ID = string.Empty;

        private void OnEnable()
        {
            _saveables.Clear();
            _saveables.AddRange(GetComponentsInChildren<ISaveable>());
        }

        private void OnDisable()
        {
            _saveables.Clear();
            _saveables.AddRange(GetComponentsInChildren<ISaveable>());
        }

        public IReadOnlyList<ISaveable> Saveables => _saveables;

        private List<ISaveable> _saveables = new();

        public string ID => m_ID;

        public void LoadAllComponents(IReadOnlyDictionary<int, JObject> collection)
        {
            for (int i = 0; i < _saveables.Count; i++)
            {
                if (collection.TryGetValue(i, out var data))
                    _saveables[i].Load(data);
            }
        }

        public Dictionary<int, JObject> SaveAllComponents()
        {
            var collection = new Dictionary<int, JObject>();

            for (int i = 0; i < _saveables.Count; i++)
                collection[i] = _saveables[i].Save();

            return collection;
        }

        public void OnBeforeSerialize()
        {
            if (!string.IsNullOrEmpty(m_ID))
                return;

            m_ID = System.Guid.NewGuid().ToString();
        }

        public void OnAfterDeserialize() { }
    }
}