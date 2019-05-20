using System;
using System.Collections;

namespace MonsterMatch.CollaborativeFiltering
{
    public struct UserItemRating : IEquatable<UserItemRating>
    {
        public int user { get; set; }
        public int item { get; set; }
        public double rating { get; set; }

        public UserItemRating(int u, int i, double r)
        {
            user = u;
            item = i;
            rating = r;
        }

        public void Deconstruct(out int u, out int i, out double r)
        {
            u = user;
            i = item;
            r = rating;
        }

        public bool Equals(UserItemRating other)
        {
            return user == other.user && item == other.item && rating.Equals(other.rating);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            return obj is UserItemRating other && Equals(other);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = user;
                hashCode = (hashCode * 397) ^ item;
                hashCode = (hashCode * 397) ^ rating.GetHashCode();
                return hashCode;
            }
        }

        public static bool operator ==(UserItemRating left, UserItemRating right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(UserItemRating left, UserItemRating right)
        {
            return !left.Equals(right);
        }
    }
}