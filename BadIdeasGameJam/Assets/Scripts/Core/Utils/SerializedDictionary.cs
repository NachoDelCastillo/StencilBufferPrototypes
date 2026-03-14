using System;
using System.Collections.Generic;
using UnityEngine;

namespace Core.Utils
{

    [Serializable]
    public class SerializedDictionary<TKey, TValue>
            : IEnumerable<KeyValuePair<TKey, TValue>>
    {
        [SerializeField] private List<Pair<TKey, TValue>> dictionary = new List<Pair<TKey, TValue>>();

        [Serializable]
        public class Pair<K, V>
        {
            [SerializeField]
            public K key;

            [SerializeField]
            public V value;
        }

        public void Add(TKey key, TValue value)
        {
            if (!ContainsKey(key))
                dictionary.Add(new Pair<TKey, TValue> { key = key, value = value });
            else
                Debug.LogWarning($"Clave duplicada en SimpleSerializedDictionary: {key}");
        }

        public bool ContainsKey(TKey key)
        {
            return dictionary.Exists(p => EqualityComparer<TKey>.Default.Equals(p.key, key));
        }

        public bool TryGetValue(TKey key, out TValue value)
        {
            var pair = dictionary.Find(p => EqualityComparer<TKey>.Default.Equals(p.key, key));
            if (pair != null)
            {
                value = pair.value;
                return true;
            }
            else
            {
                value = default;
                return false;
            }
        }

        /// <summary>
        /// Convierte la lista serializada en un Dictionary real de C#.
        /// </summary>
        public Dictionary<TKey, TValue> ToDictionary()
        {
            var result = new Dictionary<TKey, TValue>();

            foreach (var pair in dictionary)
            {
                // Evita claves duplicadas
                if (!result.ContainsKey(pair.key))
                    result[pair.key] = pair.value;
                else
                    Debug.LogWarning($"Clave duplicada encontrada en SimpleSerializedDictionary: {pair.key}");
            }

            return result;
        }

        // -------------------------
        // Métodos y propiedades base
        // -------------------------

        public TValue this[TKey key]
        {
            get
            {
                var pair = dictionary.Find(p => EqualityComparer<TKey>.Default.Equals(p.key, key));
                if (pair == null)
                    throw new KeyNotFoundException($"Clave no encontrada en SimpleSerializedDictionary: {key}");
                return pair.value;
            }
            set
            {
                var pair = dictionary.Find(p => EqualityComparer<TKey>.Default.Equals(p.key, key));
                if (pair != null)
                    pair.value = value;
                else
                    dictionary.Add(new Pair<TKey, TValue> { key = key, value = value });
            }
        }

        public ICollection<TKey> Keys
        {
            get
            {
                var keys = new List<TKey>();
                foreach (var p in dictionary)
                    keys.Add(p.key);
                return keys;
            }
        }

        public ICollection<TValue> Values
        {
            get
            {
                var values = new List<TValue>();
                foreach (var p in dictionary)
                    values.Add(p.value);
                return values;
            }
        }

        public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
        {
            foreach (var p in dictionary)
                yield return new KeyValuePair<TKey, TValue>(p.key, p.value);
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public int Count => dictionary.Count;
        public bool IsReadOnly => false;

        /// <summary>
        /// Indicates whether the dictionary contains no elements.
        /// </summary>
        public bool Empty => dictionary == null || dictionary.Count == 0;
    }
}