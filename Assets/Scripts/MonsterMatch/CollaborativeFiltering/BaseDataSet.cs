using System;
using System.Collections.Generic;
using System.Linq;

namespace MonsterMatch.CollaborativeFiltering
{
    public abstract class BaseDataSet
    {
        public abstract IEnumerable<UserItemRating> allRatings { get; }
        public abstract UserItemRating[] UserRatings(int u);
        public abstract UserItemRating[] ItemRatings(int i);
        public abstract IEnumerable<int> allUsers { get; }
        public abstract IEnumerable<int> allItems { get; }
        public abstract int userCount { get; }
        public abstract int itemCount { get; }
        public abstract double this[int u, int i] { get; set; }
        public abstract void Add(params UserItemRating[] ratings);
        public abstract void Set(double[,] ratings);

        public abstract void Resize(int users, int items);

        public double ratingScaleHigherBound { get; set; } = 100000d;
        public double ratingScaleLowerBound { get; set; } = 0d;

        public virtual bool KnowsUser(int u)
        {
            return u < userCount;
        }

        public virtual bool KnowsItem(int i)
        {
            return i < itemCount;
        }

        public virtual double GlobalMean()
        {
            return allRatings.Select(tuple => tuple.rating).Average();
        }

        /// <summary>
        /// The number of ratings this user has made
        /// </summary>
        /// <param name="u"></param>
        /// <returns></returns>
        public virtual int UserRatingsCount(int u)
        {
            return UserRatings(u).Length;
        }

        /// <summary>
        /// The number of ratings this item has received
        /// </summary>
        /// <param name="i"></param>
        /// <returns></returns>
        public virtual int ItemRatingsCount(int i)
        {
            return ItemRatings(i).Length;
        }

        public virtual IEnumerable<ValueTuple<int, UserItemRating[]>> userRatings
        {
            get
            {
                for (var u = 0; u < userCount; u++)
                {
                    yield return new ValueTuple<int, UserItemRating[]>(u, UserRatings(u));
                }
            }
        }
    }
}