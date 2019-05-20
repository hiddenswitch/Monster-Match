using System;
using UnityEngine;

namespace MonsterMatch
{
    [Serializable]
    public sealed class StampDrawing
    {
        [SerializeField] private StampItem[] m_Stamps;

        public StampItem[] stamps
        {
            get => m_Stamps;
            set => m_Stamps = value;
        }
    }
}