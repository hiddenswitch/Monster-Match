using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace MonsterMatch.Assets
{
    [Serializable]
    public sealed class ProfileCreatorAsset : ScriptableObject, IList<CatalogueItemAsset>
    {
        [SerializeField] private CatalogueItemAsset[] m_CatalogueItemAssets;

        private Dictionary<(CategoryEnum, CatalogueColorEnum),
            List<CatalogueItemAsset>> m_CategoryColors =
            new Dictionary<(CategoryEnum, CatalogueColorEnum), List<CatalogueItemAsset>>();

        public CatalogueItemAsset[] catalogueItemAssets
        {
            get => m_CatalogueItemAssets;
            set => m_CatalogueItemAssets = value;
        }

        public IEnumerator<CatalogueItemAsset> GetEnumerator()
        {
            foreach (var item in m_CatalogueItemAssets)
            {
                yield return item;
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public void Add(CatalogueItemAsset item)
        {
            throw new NotImplementedException();
        }

        public void Clear()
        {
            throw new NotImplementedException();
        }

        public bool Contains(CatalogueItemAsset item)
        {
            return m_CatalogueItemAssets.Contains(item);
        }

        public void CopyTo(CatalogueItemAsset[] array, int arrayIndex)
        {
            m_CatalogueItemAssets.CopyTo(array, arrayIndex);
        }

        public bool Remove(CatalogueItemAsset item)
        {
            throw new NotImplementedException();
        }

        public int Count => m_CatalogueItemAssets.Length;
        public bool IsReadOnly => true;

        public int IndexOf(CatalogueItemAsset item)
        {
            return Array.IndexOf(m_CatalogueItemAssets, item);
        }

        public void Insert(int index, CatalogueItemAsset item)
        {
            throw new NotImplementedException();
        }

        public void RemoveAt(int index)
        {
            throw new NotImplementedException();
        }

        public CatalogueItemAsset this[int index]
        {
            get => m_CatalogueItemAssets[index];
            set => m_CatalogueItemAssets[index] = value;
        }

        public IList<CatalogueItemAsset> this[CategoryEnum category, CatalogueColorEnum colorAndNone]
        {
            get
            {
                if (m_CategoryColors == null || m_CategoryColors.Count == 0)
                {
                    m_CategoryColors = m_CatalogueItemAssets
                        .SelectMany(item =>
                        {
                            if (item.color == CatalogueColorEnum.None)
                            {
                                return ((CatalogueColorEnum[]) Enum.GetValues(typeof(CatalogueColorEnum)))
                                    .Where(color => color != CatalogueColorEnum.None)
                                    .Select(color =>
                                    {
                                        var copy = item;
                                        copy.color = color;
                                        return copy;
                                    });
                            }
                            else
                            {
                                return new[] {item};
                            }
                        })
                        .GroupBy(item => new ValueTuple<CategoryEnum, CatalogueColorEnum>(item.category, item.color))
                        .ToDictionary(grouping => grouping.Key,
                            grouping => grouping.OrderBy(item => item.name).ToList());
                }

                var key = new ValueTuple<CategoryEnum, CatalogueColorEnum>(category, colorAndNone);
                if (!m_CategoryColors.ContainsKey(key))
                {
                    return new CatalogueItemAsset[0];
                }
                return m_CategoryColors[key];
            }
        }
    }
}