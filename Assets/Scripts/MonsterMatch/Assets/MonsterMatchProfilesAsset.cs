using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MonsterMatch.Assets
{
    [Serializable]
    public sealed class MonsterMatchProfilesAsset : ScriptableObject, IList<DatingProfileItem>
    {
        [SerializeField] private DatingProfileItem[] m_Items = new DatingProfileItem[0];

        public int IndexOf(DatingProfileItem item)
        {
            throw new NotImplementedException();
        }

        public void Insert(int index, DatingProfileItem item)
        {
            throw new NotImplementedException();
        }

        public void RemoveAt(int index)
        {
            throw new NotImplementedException();
        }

        public DatingProfileItem this[int index]
        {
            get { return m_Items[index]; }
            set { throw new NotImplementedException(); }
        }

        public int Length => m_Items.Length;

        public IEnumerator<DatingProfileItem> GetEnumerator()
        {
            return ((IEnumerable<DatingProfileItem>) m_Items).GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return m_Items.GetEnumerator();
        }

        private void Awake()
        {
            // Access the stories so they preload
            var stories = 0;
            foreach (var profile in m_Items)
            {
                if (profile.dialogue?.story != null)
                {
                    stories++;
                }
            }

            Debug.Log($"MonsterMatchProfilesAsset.Awake: Loaded {Length} profiles with {stories} stories.");
        }

        public void Add(DatingProfileItem item)
        {
            throw new NotImplementedException();
        }

        public void Clear()
        {
            throw new NotImplementedException();
        }

        public bool Contains(DatingProfileItem item)
        {
            throw new NotImplementedException();
        }

        public void CopyTo(DatingProfileItem[] array, int arrayIndex)
        {
            for (var i = 0; i < Count; i++)
            {
                array[arrayIndex + i] = this[i];
            }
        }

        public bool Remove(DatingProfileItem item)
        {
            throw new NotImplementedException();
        }

        public int Count => m_Items.Length;
        public bool IsReadOnly => true;

        public void Replace(DatingProfileItem[] replacement)
        {
            m_Items = replacement;
        }

        public DatingProfileItem[] All()
        {
            return m_Items;
        }
    }
}