using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace MonsterMatch.CollaborativeFiltering
{
    public class ArrayBackedSinglePlayerDataSet : BaseDataSet
    {
        private readonly double[,] m_Data;
        private readonly int m_PlayerUserId;
        private const double noRating = double.NaN;
        private const int userDimension = 0;
        private const int itemDimension = 1;

        public ArrayBackedSinglePlayerDataSet(double[,] baseData, int userCount, int itemCount)
        {
            Debug.Assert(baseData.GetLength(userDimension) == userCount,
                "baseData.GetLength(userDimension)==profileCount+1");
            Debug.Assert(baseData.GetLength(itemDimension) == itemCount,
                "baseData.GetLength(itemDimension)==profileCount");
            m_Data = baseData;
            m_PlayerUserId = userCount - 1;
            // Negative-out this player so that no ratings for them exists
            for (var i = 0; i < itemCount; i++)
            {
                m_Data[m_PlayerUserId, i] = noRating;
            }
        }

        public override IEnumerable<UserItemRating> allRatings
        {
            get
            {
                for (var u = 0; u < userCount - 1; u++)
                {
                    for (var i = 0; i < itemCount; i++)
                    {
                        yield return new UserItemRating(u, i, m_Data[u, i]);
                    }
                }

                // Last user, i.e. player
                for (var i = 0; i < itemCount; i++)
                {
                    if (PlayerDidRate(i))
                    {
                        yield return new UserItemRating(m_PlayerUserId, i, m_Data[m_PlayerUserId, i]);
                    }
                }
            }
        }

        public override UserItemRating[] UserRatings(int u)
        {
            if (u != m_PlayerUserId)
            {
                var tuples = new UserItemRating[itemCount];
                for (var i = 0; i < itemCount; i++)
                {
                    tuples[i].user = u;
                    tuples[i].item = i;
                    tuples[i].rating = m_Data[u, i];
                }

                return tuples;
            }

            return Enumerable.Range(0, itemCount)
                .Where(PlayerDidRate)
                .Select(i => new UserItemRating(u, i, m_Data[u, i])).ToArray();
        }

        public override UserItemRating[] ItemRatings(int i)
        {
            var playerDidRate = PlayerDidRate(i);
            var count = playerDidRate ? userCount : userCount - 1;
            var tuples = new UserItemRating[count];
            for (var u = 0; u < count; u++)
            {
                tuples[u].user = u;
                tuples[u].item = i;
                tuples[u].rating = m_Data[u, i];
            }

            return tuples;
        }

        private bool PlayerDidRate(int i)
        {
            return !double.IsNaN(m_Data[m_PlayerUserId, i]);
        }

        public override IEnumerable<int> allUsers => Enumerable.Range(0, userCount);

        public override IEnumerable<int> allItems => Enumerable.Range(0, itemCount);
        public override int userCount => m_Data.GetLength(userDimension);
        public override int itemCount => m_Data.GetLength(itemDimension);

        public override double this[int u, int i]
        {
            get => m_Data[u, i];
            set => m_Data[u, i] = value;
        }

        public override void Add(params UserItemRating[] ratings)
        {
            foreach (var (u, i, r) in ratings)
            {
                m_Data[u, i] = r;
            }
        }

        public override void Set(double[,] ratings)
        {
            throw new System.NotImplementedException();
        }

        public override void Resize(int users, int items)
        {
            throw new System.NotImplementedException();
        }

        public override int UserRatingsCount(int u)
        {
            if (u == m_PlayerUserId)
            {
                var count = 0;
                for (var i = 0; i < itemCount; i++)
                {
                    if (PlayerDidRate(i))
                    {
                        count += 1;
                    }
                }

                return count;
            }

            return itemCount;
        }

        public override int ItemRatingsCount(int i)
        {
            if (PlayerDidRate(i))
            {
                return userCount;
            }

            return userCount - 1;
        }
    }
}