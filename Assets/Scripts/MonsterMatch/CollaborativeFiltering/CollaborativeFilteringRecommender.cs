using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace MonsterMatch.CollaborativeFiltering
{
    public class CollaborativeFilteringRecommender : IRecommender
    {
        private readonly BaseDataSet m_DataSet;
        private readonly BaseAlgorithm m_Algorithm;
        private readonly int m_PlayerUserId;
        private readonly Dictionary<int, int> m_ApplicationItemIdsToInternalIds;
        private readonly IList<int> m_InternalIdsToApplicationItemIds;
        public double matchedValue { get; set; } = 2.0;
        public double passValue { get; set; } = 1.0;

        public CollaborativeFilteringRecommender() : this(new NonnegativeMatrixFactorization())
        {
            (m_Algorithm as NonnegativeMatrixFactorization).nEpochs = 20;
        }

        public CollaborativeFilteringRecommender(BaseAlgorithm algorithm)
        {
            m_Algorithm = algorithm;
            m_DataSet = new ArrayBackedSinglePlayerDataSet(MonsterMatchArrayData.Data,
                MonsterMatchArrayData.UserCount, MonsterMatchArrayData.ItemCount);
            m_PlayerUserId = MonsterMatchArrayData.PlayerUserId;
            Debug.Log($"DefaultRecommender: m_PlayerUserId={m_PlayerUserId}");
            m_ApplicationItemIdsToInternalIds = Enumerable.Range(0, MonsterMatchArrayData.ItemCount)
                .ToDictionary(i => MonsterMatchArrayData.ForProfiles[i], i => i);
            m_InternalIdsToApplicationItemIds = MonsterMatchArrayData.ForProfiles;
        }

        public void AddPlayerRating(int profileZeroBasedIndex, bool matched)
        {
            m_DataSet.Add(new UserItemRating(m_PlayerUserId, m_ApplicationItemIdsToInternalIds[profileZeroBasedIndex],
                matched ? matchedValue : passValue));
        }

        public void Fit()
        {
            m_Algorithm.Fit(m_DataSet);
        }

        public IEnumerable<int> Top()
        {
            var estimates = Enumerable.Range(0, MonsterMatchArrayData.ItemCount)
                .Select(j => m_Algorithm.Predict(m_PlayerUserId, j))
                .OrderByDescending(prediction => prediction.est);
#if UNITY_EDITOR
            Debug.Log($"Estimates:\n" + string.Join("\n", estimates));
#endif
            return estimates
                .Select(prediction => m_InternalIdsToApplicationItemIds[prediction.iid]);
        }

        public static void TestSlopeOne()
        {
            var algorithm = new SlopeOne();
            var recommender = new CollaborativeFilteringRecommender(algorithm);
            recommender.AddPlayerRating(3, true);
            recommender.AddPlayerRating(6, false);
            recommender.AddPlayerRating(39, true);
            recommender.AddPlayerRating(50, false);
            recommender.AddPlayerRating(61, true);
            recommender.Fit();
            for (var i = 0; i < recommender.m_DataSet.itemCount; i++)
            {
                Debug.Log($"SlopeOne est={algorithm.Predict(recommender.m_PlayerUserId, i).est}");
            }
        }

        public static void TestNonnegativeMatrixFactorization()
        {
            var algorithm = new NonnegativeMatrixFactorization()
            {
                nEpochs = 1,
                initLow = 0.5f,
                initHigh = 0.5f
            };

            var recommender = new CollaborativeFilteringRecommender(algorithm);
            recommender.AddPlayerRating(3, true);
            recommender.AddPlayerRating(6, false);
            recommender.AddPlayerRating(39, true);
            recommender.AddPlayerRating(50, false);
            recommender.AddPlayerRating(61, true);
            recommender.Fit();
            for (var i = 0; i < recommender.m_DataSet.itemCount; i++)
            {
                Debug.Log($"NMF est={algorithm.Predict(recommender.m_PlayerUserId, i).est}");
            }
        }
    }
}