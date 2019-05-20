using System;
using MaterialUI;
using UnityEngine;

namespace MonsterMatch
{
    [Serializable]
    public sealed class DatingInterstitialItem
    {
        [SerializeField] private MaterialScreen m_Screen;
        [SerializeField] private int m_AfterNSwipes;

        public MaterialScreen screen => m_Screen;
        
        public int afterNSwipes => m_AfterNSwipes;
    }
}