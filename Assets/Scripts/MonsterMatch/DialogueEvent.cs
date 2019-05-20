using System;
using System.Collections.Generic;
using Ink.Runtime;

namespace MonsterMatch
{
    public sealed class DialogueEvent : IEquatable<DialogueEvent>
    {
        public string text { get; set; }
        public bool self { get; set; }
        public List<Choice> choices { get; set; } = new List<Choice>();
        
        public bool finished { get; set; }
        public bool noDelay { get; set; }
        public bool noBubble { get; set; }
        
        public bool success { get; set; }
        
        public string riddle { get; set; }
        public bool silent { get; set; }

        public bool Equals(DialogueEvent other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return string.Equals(text, other.text);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            return obj is DialogueEvent other && Equals(other);
        }

        public override int GetHashCode()
        {
            return (text != null ? text.GetHashCode() : 0);
        }

        public static bool operator ==(DialogueEvent left, DialogueEvent right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(DialogueEvent left, DialogueEvent right)
        {
            return !Equals(left, right);
        }
    }
}