using System.Collections.Generic;
using UnityEngine;

namespace Saveable
{
    public sealed class SerializeDictionary<TKey, TValue> : Dictionary<TKey, TValue>, ISerializationCallbackReceiver
    {
        [SerializeField] List<TKey> m_Keys = default;
        [SerializeField] List<TValue> m_Values = default;

        public void OnBeforeSerialize()
        {
            m_Keys.Clear();
            m_Values.Clear();

            foreach (var item in this)
            {
                m_Keys.Add(item.Key);
                m_Values.Add(item.Value);
            }
        }

        public void OnAfterDeserialize()
        {
            this.Clear();

            if (m_Keys.Count != m_Values.Count)
                Debug.LogError(message: "Error keys length don't match values count");

            for (int i = 0; i < m_Keys.Count; i++)
                this.Add(m_Keys[i], m_Values[i]);
        }
    }
}