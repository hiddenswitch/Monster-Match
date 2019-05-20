using System;
using MaterialUI;
using UnityEngine;

namespace MonsterMatch
{
    [Serializable]
    public sealed class DialogueChoiceCorrespondingScreen
    {
        [SerializeField] private int m_Choice;
        [SerializeField] private MaterialScreen m_Screen;

        public int choice
        {
            get => m_Choice;
            set => m_Choice = value;
        }

        public MaterialScreen screen
        {
            get => m_Screen;
            set => m_Screen = value;
        }
    }
}