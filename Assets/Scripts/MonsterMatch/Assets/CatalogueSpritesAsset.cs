using System;
using UnityEngine;

namespace MonsterMatch.Assets
{
    [Serializable]
    public sealed class CatalogueSpritesAsset : ScriptableObject
    {
        [SerializeField] private CategoryEnum m_CategoryEnum;
        [SerializeField] private Sprite[] m_Sprites;

        public CategoryEnum categoryEnum => m_CategoryEnum;

        public Sprite[] sprites => m_Sprites;
    }
}