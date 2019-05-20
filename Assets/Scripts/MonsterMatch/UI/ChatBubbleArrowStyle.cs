using System;
using UnityEngine;

namespace MonsterMatch.UI
{
    public enum ChatBubbleArrowAlignment
    {
        BottomLeft,
        BottomRight,
        TopCenter
    }
    [Serializable]
    public sealed class ChatBubbleArrowStyle
    {
        [SerializeField] private ChatBubbleArrowAlignment m_Alignment;
        [SerializeField] private Sprite m_Sprite;
        [SerializeField] private Vector2 m_AnchoredPosition;

        public ChatBubbleArrowAlignment alignment => m_Alignment;

        public Sprite sprite => m_Sprite;

        public Vector2 anchoredPosition => m_AnchoredPosition;
    }
}