using System;
using UnityEngine;

namespace MonsterMatch.CollaborativeFiltering
{
    /// <summary>
    /// Since users are items in this model, the ItemId is the same as the UserId.
    /// </summary>
    [Serializable]
    public struct RatingTuple : IEquatable<RatingTuple>
    {
        [SerializeField] private int m_ItemInnerId;

        [SerializeField] private double m_Rating;

        public int itemInnerId
        {
            get => m_ItemInnerId;
            set => m_ItemInnerId = value;
        }

        public double rating
        {
            get => m_Rating;
            set => m_Rating = value;
        }

        public bool Equals(RatingTuple other)
        {
            return itemInnerId == other.itemInnerId && rating.Equals(other.rating);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            return obj is RatingTuple other && Equals(other);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return (itemInnerId * 397) ^ rating.GetHashCode();
            }
        }

        public static bool operator ==(RatingTuple left, RatingTuple right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(RatingTuple left, RatingTuple right)
        {
            return !left.Equals(right);
        }

        public void Deconstruct(out int key, out double value)
        {
            key = itemInnerId;
            value = rating;
        }
    }
}