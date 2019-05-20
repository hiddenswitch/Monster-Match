using System;
using UnityEngine;

namespace MonsterMatch.UI
{
    [Serializable]
    public struct DatingProfileState : IEquatable<DatingProfileState>
    {
        [SerializeField] private int m_ProfileIndex;
        [SerializeField] private DatingProfileJudgement m_Judgement;

        public int profileIndex
        {
            get => m_ProfileIndex;
            set => m_ProfileIndex = value;
        }

        public DatingProfileJudgement judgement
        {
            get => m_Judgement;
            set => m_Judgement = value;
        }

        public bool Equals(DatingProfileState other)
        {
            return m_ProfileIndex == other.m_ProfileIndex && m_Judgement == other.m_Judgement;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            return obj is DatingProfileState other && Equals(other);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return (m_ProfileIndex * 397) ^ (int) m_Judgement;
            }
        }

        public static bool operator ==(DatingProfileState left, DatingProfileState right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(DatingProfileState left, DatingProfileState right)
        {
            return !left.Equals(right);
        }
    }
}