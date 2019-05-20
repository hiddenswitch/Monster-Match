using System;
using UnityEngine;

namespace MonsterMatch.CollaborativeFiltering
{
    [Serializable]
    public struct UserTuple : IEquatable<UserTuple>
    {
        [SerializeField] private int m_UserInnerId;

        [SerializeField] private double m_Rating;

        public int userInnerId
        {
            get => m_UserInnerId;
            set => m_UserInnerId = value;
        }

        public double rating
        {
            get => m_Rating;
            set => m_Rating = value;
        }

        public bool Equals(UserTuple other)
        {
            return m_UserInnerId == other.m_UserInnerId && m_Rating.Equals(other.m_Rating);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            return obj is UserTuple other && Equals(other);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return (m_UserInnerId * 397) ^ m_Rating.GetHashCode();
            }
        }

        public static bool operator ==(UserTuple left, UserTuple right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(UserTuple left, UserTuple right)
        {
            return !left.Equals(right);
        }

        public void Deconstruct(out int key, out double value)
        {
            key = m_UserInnerId;
            value = m_Rating;
        }
    }
}