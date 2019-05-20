using System;

namespace UnityEngine.UI
{
    [Serializable]
    public struct ChatBoxChoice : IEquatable<ChatBoxChoice>
    {
        private int m_Index;
        private string m_Text;

        public int index
        {
            get => m_Index;
            set => m_Index = value;
        }

        public string text
        {
            get => m_Text;
            set => m_Text = value;
        }

        public bool Equals(ChatBoxChoice other)
        {
            return m_Index == other.m_Index && string.Equals(m_Text, other.m_Text);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            return obj is ChatBoxChoice other && Equals(other);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return (m_Index * 397) ^ (m_Text != null ? m_Text.GetHashCode() : 0);
            }
        }

        public static bool operator ==(ChatBoxChoice left, ChatBoxChoice right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(ChatBoxChoice left, ChatBoxChoice right)
        {
            return !left.Equals(right);
        }
    }
}