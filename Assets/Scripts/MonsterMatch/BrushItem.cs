using System;
using UnityEngine;

namespace MonsterMatch
{
    public sealed class BrushItem : IEquatable<BrushItem>
    {
        public CatalogueItem catalogueItem { get; set; }

        public Sprite sprite => catalogueItem.sprite;

        public bool flipped => catalogueItem.flipped;

        public bool flipsAfterStamp => catalogueItem.asset.flipsAfterStamp;

        public bool Equals(BrushItem other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return catalogueItem.Equals(other.catalogueItem);
        }

        public override bool Equals(object obj)
        {
            return ReferenceEquals(this, obj) || obj is BrushItem other && Equals(other);
        }

        public override int GetHashCode()
        {
            return catalogueItem.GetHashCode();
        }

        public static bool operator ==(BrushItem left, BrushItem right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(BrushItem left, BrushItem right)
        {
            return !Equals(left, right);
        }
    }
}