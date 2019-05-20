using System;
using MonsterMatch.Assets;
using UnityEngine;

namespace MonsterMatch
{
    [Serializable]
    public struct CatalogueItem : IEquatable<CatalogueItem>
    {
        [SerializeField] private bool m_Flipped;

        public Sprite sprite => asset.sprite;
        public CategoryEnum category => asset.category;

        public bool flipped
        {
            get => m_Flipped;
            set => m_Flipped = value;
        }
        
        public CatalogueItemAsset asset { get; set; }

        public bool Equals(CatalogueItem other)
        {
            return m_Flipped == other.m_Flipped && asset.Equals(other.asset);
        }

        public override bool Equals(object obj)
        {
            return obj is CatalogueItem other && Equals(other);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return (m_Flipped.GetHashCode() * 397) ^ asset.GetHashCode();
            }
        }

        public static bool operator ==(CatalogueItem left, CatalogueItem right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(CatalogueItem left, CatalogueItem right)
        {
            return !left.Equals(right);
        }
    }
}