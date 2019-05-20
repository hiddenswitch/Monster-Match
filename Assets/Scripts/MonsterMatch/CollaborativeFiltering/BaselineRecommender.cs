using System;
using System.Collections.Generic;
using System.Linq;
using HiddenSwitch;
using UnityEngine;

namespace MonsterMatch.CollaborativeFiltering
{
    public class BaselineRecommender : IRecommender
    {
        private readonly DatingProfileItem[] m_Items;
        private readonly float[] m_Weights;
        private int m_Count;
        private int m_BootstrappingSteps = 8;
        private float m_PassedDiscount = 0.8f;

        public BaselineRecommender(DatingProfileItem[] items)
        {
            m_Items = items;
            m_Weights = new float[Enum.GetValues(typeof(LatentCategory1)).Length];
        }

        public void AddPlayerRating(int profileZeroBasedIndex, bool matched)
        {
            var isBootstrapping = m_Count < m_BootstrappingSteps;
            var value = isBootstrapping
                ? (m_BootstrappingSteps - 1.0f) / m_BootstrappingSteps
                : (m_Count - 1.0f) / m_BootstrappingSteps;
            m_Weights[(int) m_Items[profileZeroBasedIndex].latentCategory1] +=
                matched ? value : -m_PassedDiscount * value;
            m_Count += 1;
        }

        public void Fit()
        {
        }

        public IEnumerable<int> Top()
        {
            var isBootstrapping = m_Count < m_BootstrappingSteps;
            if (isBootstrapping)
            {
                return Enumerable.Range(0, m_Items.Length).Shuffled();
            }
            else
            {
                return Enumerable.Range(0, m_Weights.Length)
                    .OrderByDescending(i => m_Weights[i])
                    .SelectMany(i =>
                        Enumerable.Range(0, m_Items.Length)
                            .Where(j => m_Items[j].latentCategory1 == (LatentCategory1) i)
                            .Shuffled()
                    );
            }
        }
    }
}