using System;

namespace MonsterMatch.CollaborativeFiltering
{
    public struct Prediction : IEquatable<Prediction>
    {
        public int uid { get; set; }
        public int iid { get; set; }
        public double rUi { get; set; }
        public double est { get; set; }
        public bool wasImpossible { get; set; }

        public bool Equals(Prediction other)
        {
            return uid == other.uid && iid == other.iid && est.Equals(other.est);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            return obj is Prediction other && Equals(other);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int hashCode = uid;
                hashCode = (hashCode * 397) ^ iid;
                hashCode = (hashCode * 397) ^ est.GetHashCode();
                return hashCode;
            }
        }

        public static bool operator ==(Prediction left, Prediction right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(Prediction left, Prediction right)
        {
            return !left.Equals(right);
        }

        public override string ToString()
        {
            return
                $"{nameof(uid)}: {uid}, {nameof(iid)}: {iid}, {nameof(est)}: {est}, {nameof(wasImpossible)}: {wasImpossible}";
        }
    }
}