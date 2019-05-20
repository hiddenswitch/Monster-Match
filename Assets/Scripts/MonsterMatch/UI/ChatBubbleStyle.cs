using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace MonsterMatch.UI
{
    [Serializable]
    public sealed class ChatBubbleStyle : ScriptableObject
    {
        [SerializeField] private Color m_TextColor;
        [SerializeField] private Sprite m_Frame;
        [SerializeField] private Sprite m_Stitching;
        [SerializeField] private Sprite m_Gloss;
        [SerializeField] private ChatBubbleArrowStyle[] m_ArrowStyles;
        private Dictionary<ChatBubbleArrowAlignment, ChatBubbleArrowStyle> m_ArrowStyleDictionary;

        public IReadOnlyDictionary<ChatBubbleArrowAlignment, ChatBubbleArrowStyle> arrowStyles
        {
            get
            {
                if (m_ArrowStyleDictionary == null)
                {
                    m_ArrowStyleDictionary =
                        m_ArrowStyles.ToDictionary(style => style.alignment, style => style);
                }

                return m_ArrowStyleDictionary;
            }
        }

        public Sprite frame => m_Frame;

        public Sprite stitching => m_Stitching;

        public Sprite gloss => m_Gloss;

        public Color textColor => m_TextColor;
    }
}