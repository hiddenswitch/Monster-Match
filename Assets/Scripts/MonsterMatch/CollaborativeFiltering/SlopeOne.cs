using System;
using System.Linq;
using UnityEngine.Profiling;

namespace MonsterMatch.CollaborativeFiltering
{
    /// <summary>
    /// An implementation of SlopeOne collaborative filtering, adapted from:
    /// <a href="https://github.com/NicolasHug/Surprise/blob/master/surprise/prediction_algorithms/slope_one.pyx">
    /// The Python Surprise Library.
    /// </a>
    /// </summary>
    public class SlopeOne : BaseAlgorithm
    {
        private int[,] m_Freq;
        private double[,] m_Dev;
        private double[] m_UserMean;

        public override void Fit(BaseDataSet dataSet)
        {
            Profiler.BeginSample("SlopeOne.Fit");
            var nItems = dataSet.itemCount;

            base.Fit(dataSet);
            m_Freq = new int[nItems, nItems];
            m_Dev = new double[nItems, nItems];
            foreach (var (_, uRatings) in dataSet.userRatings)
            {
                foreach (var (_, i, rUi) in uRatings)
                {
                    foreach (var (_, j, rUj) in uRatings)
                    {
                        m_Freq[i, j] += 1;
                        m_Dev[i, j] += rUi - rUj;
                    }
                }
            }

            for (var i = 0; i < nItems; i++)
            {
                m_Dev[i, i] = 0;
                for (var j = i + 1; j < nItems; j++)
                {
                    m_Dev[i, j] /= m_Freq[i, j];
                    m_Dev[j, i] = -m_Dev[i, j];
                }
            }

            m_UserMean = dataSet.allUsers
                .Select(u => dataSet.UserRatings(u).Select(rt => rt.rating).Average()).ToArray();
            Profiler.EndSample();
        }

        protected override double Estimate(int u, int i)
        {
            if (!(m_DataSet.KnowsUser(u) && m_DataSet.KnowsItem(i)))
            {
                throw new Exception("Prediction impossible. User is unknown");
            }

            Profiler.BeginSample("SlopeOne.Estimate");
            var ri = m_DataSet.UserRatings(u).Where(r => m_Freq[i, r.item] > 0).Select(r => r.item)
                .ToArray();
            var est = m_UserMean[u];
            if (ri.Length > 0)
            {
                est += ri.Select(j => m_Dev[i, j]).Sum() / ri.Length;
            }

            Profiler.EndSample();
            return est;
        }
    }
}