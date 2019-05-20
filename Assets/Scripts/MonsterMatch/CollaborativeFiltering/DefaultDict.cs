using System;
using System.Collections;
using System.Collections.Generic;

namespace MonsterMatch.CollaborativeFiltering
{
    internal class DefaultDict<TKey, TValue> : IDictionary<TKey, TValue>, IReadOnlyDictionary<TKey, TValue>
        where TValue : new()
    {
        private Dictionary<TKey, TValue> m_Dict = new Dictionary<TKey, TValue>();

        public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
        {
            return m_Dict.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public void Add(KeyValuePair<TKey, TValue> item)
        {
            m_Dict.Add(item.Key, item.Value);
        }

        public void Clear()
        {
            m_Dict.Clear();
        }

        public bool Contains(KeyValuePair<TKey, TValue> item)
        {
            return m_Dict.ContainsKey(item.Key) && m_Dict[item.Key].Equals(item.Value);
        }

        public void CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex)
        {
            throw new NotImplementedException();
        }

        public bool Remove(KeyValuePair<TKey, TValue> item)
        {
            return m_Dict.Remove(item.Key);
        }

        public int Count => m_Dict.Count;
        public bool IsReadOnly => false;

        public void Add(TKey key, TValue value)
        {
            m_Dict.Add(key, value);
        }

        public bool ContainsKey(TKey key)
        {
            return m_Dict.ContainsKey(key);
        }

        public bool Remove(TKey key)
        {
            return m_Dict.Remove(key);
        }

        public bool TryGetValue(TKey key, out TValue value)
        {
            return m_Dict.TryGetValue(key, out value);
        }

        public TValue this[TKey key]
        {
            get
            {
                if (m_Dict.ContainsKey(key))
                {
                    return m_Dict[key];
                }

                var value = new TValue();
                m_Dict[key] = value;
                return value;
            }
            set => m_Dict[key] = value;
        }

        IEnumerable<TKey> IReadOnlyDictionary<TKey, TValue>.Keys => Keys;

        IEnumerable<TValue> IReadOnlyDictionary<TKey, TValue>.Values => Values;

        public ICollection<TKey> Keys => m_Dict.Keys;
        public ICollection<TValue> Values => m_Dict.Values;
    }
}