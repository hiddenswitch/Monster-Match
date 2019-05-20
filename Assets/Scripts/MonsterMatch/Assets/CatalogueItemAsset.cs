using System;
using UnityEngine;

namespace MonsterMatch.Assets
{
    [Serializable]
    public struct CatalogueItemAsset
    {
        [SerializeField] private string m_Name;
        [SerializeField] private Sprite m_Sprite;
        [SerializeField] private CategoryEnum m_Category;
        [SerializeField] private CatalogueColorEnum m_Color;
        [SerializeField] private bool m_FlipsAfterStamp;

        public string name
        {
            get => m_Name;
            set => m_Name = value;
        }

        public Sprite sprite
        {
            get => m_Sprite;
            set => m_Sprite = value;
        }

        public CategoryEnum category
        {
            get => m_Category;
            set => m_Category = value;
        }

        public CatalogueColorEnum color
        {
            get => m_Color;
            set => m_Color = value;
        }

        public bool flipsAfterStamp
        {
            get => m_FlipsAfterStamp;
            set => m_FlipsAfterStamp = value;
        }
    }
}