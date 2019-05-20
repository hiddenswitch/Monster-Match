using System;
using UnityEngine;

namespace MonsterMatch
{
    [Serializable]
    public class CategoryItem
    {
        [SerializeField] private CategoryEnum m_Category;
        [SerializeField] private string m_Name;

        public CategoryEnum category => m_Category;

        public string name => m_Name;
    }
}