using System;
using UnityEngine;

namespace MonsterMatch
{
    [Serializable]
    public struct ChatMessageModelView
    {
        [SerializeField] private ChatMessage m_ChatMessage;
        [SerializeField] private bool m_HasPendingVisualSizeChange;
        [SerializeField] private bool m_IsLastInSequence;

        public ChatMessage chatMessage
        {
            get => m_ChatMessage;
            set => m_ChatMessage = value;
        }

        public bool hasPendingVisualSizeChange
        {
            get => m_HasPendingVisualSizeChange;
            set => m_HasPendingVisualSizeChange = value;
        }

        public bool isLastInSequence
        {
            get => m_IsLastInSequence;
            set => m_IsLastInSequence = value;
        }
    }
}