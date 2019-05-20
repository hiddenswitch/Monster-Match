using System;
using UnityEngine;

namespace MonsterMatch
{
    [Serializable]
    public sealed class ChatMessage : IComparable<ChatMessage>
    {
        [SerializeField] private string m_Message;
        [SerializeField] private Sprite m_Image;
        [SerializeField] private bool m_Self;
        [SerializeField] private string m_DateTime;
        [SerializeField] private bool m_NoBubble;

        public string message
        {
            get => m_Message;
            set => m_Message = value;
        }

        public Sprite image
        {
            get => m_Image;
            set => m_Image = value;
        }

        public bool self
        {
            get => m_Self;
            set => m_Self = value;
        }

        public string dateTime
        {
            get => m_DateTime;
            set => m_DateTime = value;
        }

        public bool noBubble
        {
            get => m_NoBubble;
            set => m_NoBubble = value;
        }

        public int CompareTo(ChatMessage other)
        {
            if (ReferenceEquals(this, other)) return 0;
            if (ReferenceEquals(null, other)) return 1;
            var messageComparison = string.Compare(m_Message, other.m_Message, StringComparison.Ordinal);
            if (messageComparison != 0) return messageComparison;
            return m_Self.CompareTo(other.m_Self);
        }

        private bool Equals(ChatMessage other)
        {
            return string.Equals(m_Message, other.m_Message) && Equals(m_Image, other.m_Image) &&
                   m_Self == other.m_Self && string.Equals(m_DateTime, other.m_DateTime);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            return obj is ChatMessage other && Equals(other);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = (m_Message != null ? m_Message.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (m_Image != null ? m_Image.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ m_Self.GetHashCode();
                hashCode = (hashCode * 397) ^ (m_DateTime != null ? m_DateTime.GetHashCode() : 0);
                return hashCode;
            }
        }
    }
}