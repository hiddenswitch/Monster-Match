using System;
using UnityEngine;

namespace MonsterMatch
{
    [Serializable]
    public struct CountItem
    {
        [SerializeField] private int m_Numerator;
        [SerializeField] private int m_Denominator;

        public int numerator
        {
            get => m_Numerator;
            set => m_Numerator = value;
        }

        public int denominator
        {
            get => m_Denominator;
            set => m_Denominator = value;
        }
    }
}