using System;
using System.Collections.Generic;
using System.Linq;
using HiddenSwitch;
using MonsterMatch.Assets;

namespace MonsterMatch.CollaborativeFiltering
{
    public class LastNRecommender : IRecommender
    {
        private readonly IList<DatingProfileItem> m_Asset;
        private List<LatentCategory1> m_LastMatchedCategories = new List<LatentCategory1>();
        private float[] m_PassedPenalties;
        private HashSet<int> m_Reciprocal = new HashSet<int>();
        public float passedPenalty { get; set; } = -1.0f;
        public float passedAgeDiscount { get; set; } = 0.95f;

        public int window { get; set; } = 3;

        public LastNRecommender(IList<DatingProfileItem> asset)
        {
            m_Asset = asset;
            m_PassedPenalties = new float[Enum.GetValues(typeof(LatentCategory1)).Length];
        }

        public void AddPlayerRating(int profileZeroBasedIndex, bool matched)
        {
            var latentCategory1 = m_Asset[profileZeroBasedIndex].latentCategory1;
            if (matched)
            {
                m_LastMatchedCategories.Add(latentCategory1);
            }
            else
            {
                m_PassedPenalties[(int) latentCategory1] += passedPenalty;
            }

            for (var i = 0; i < m_PassedPenalties.Length; i++)
            {
                m_PassedPenalties[i] *= passedAgeDiscount;
            }

            if (m_LastMatchedCategories.Count > window)
            {
                m_LastMatchedCategories.RemoveAt(0);
            }
        }

        public void Fit()
        {
        }

        public IEnumerable<int> Top()
        {
            var latents = (LatentCategory1[]) Enum.GetValues(typeof(LatentCategory1));
            return
                m_LastMatchedCategories.GroupBy(key => key)
                    .Select(cat =>
                        new Tuple<LatentCategory1, float>(cat.Key,
                            (float) cat.Count() + m_PassedPenalties[(int) cat.Key]))
                    .OrderByDescending(t => t.Item2)
                    .Concat(latents.Except(m_LastMatchedCategories.Distinct())
                        .Select(cat => new Tuple<LatentCategory1, float>(cat, 0f)))
                    .SelectMany(t => Enumerable.Range(0, m_Asset.Count)
                        .Where(j => m_Asset[j].latentCategory1 == t.Item1)
                        .Shuffled());
        }
    }
}