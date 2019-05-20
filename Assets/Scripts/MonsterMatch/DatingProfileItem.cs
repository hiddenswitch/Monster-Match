using System;
using UnityEngine;

namespace MonsterMatch
{
    [Serializable]
    public sealed class DatingProfileItem
    {
        [SerializeField] private Sprite m_Portrait;
        [SerializeField] private string m_Name;
        [SerializeField] private string m_Byline;
        [SerializeField] private string m_Body;
        [SerializeField] private DialogueItem m_Dialogue;
        [SerializeField] private LatentCategory1 m_LatentCategory1;
        private int m_Index;

        public Sprite portrait
        {
            get => m_Portrait;
            set => m_Portrait = value;
        }

        public string name
        {
            get => m_Name;
            set => m_Name = value;
        }

        public string body
        {
            get => m_Body;
            set => m_Body = value;
        }

        public string byline
        {
            get => m_Byline;
            set => m_Byline = value;
        }

        public int reciprocalMatchCount { get; set; }
        public int totalProfilesSeen { get; set; }

        public int index { get; set; }

        public LatentCategory1 latentCategory1
        {
            get => m_LatentCategory1;
            set => m_LatentCategory1 = value;
        }

        public DialogueItem dialogue
        {
            get => m_Dialogue;
            set => m_Dialogue = value;
        }

        public int unreadMessageCount { get; set; }

        public override string ToString()
        {
            return $"{nameof(name)}: {name}, {nameof(index)}: {index}, {nameof(latentCategory1)}: {latentCategory1}";
        }
    }
}