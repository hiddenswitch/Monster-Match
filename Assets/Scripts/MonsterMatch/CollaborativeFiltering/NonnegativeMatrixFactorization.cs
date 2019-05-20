using System;
using System.Linq;
using UnityEngine;
using UnityEngine.Profiling;

namespace MonsterMatch.CollaborativeFiltering
{
    public class NonnegativeMatrixFactorization : BaseAlgorithm
    {
        public const int DefaultEpochsCount = 100;
        public const int DefaultFactorCount = 10;
        private const double Epsilon = 1.19209e-07;
        private int m_NFactors = DefaultFactorCount;
        private int m_NEpochs = DefaultEpochsCount;
        private bool m_Biased = false;
        private double m_RegPu = 0.06;
        private double m_RegQi = 0.06;
        private double m_RegBu = 0.02;
        private double m_RegBi = 0.02;
        private double m_LrBu = 0.005;
        private double m_LrBi = 0.005;
        private float m_InitLow;
        private float m_InitHigh = 1;
        private double[] m_Bu;
        private double[] m_Bi;
        private double[,] m_Pu;
        private double[,] m_Qi;
        private double[,] m_UserNum;
        private double[,] m_UserDenom;
        private double[,] m_ItemNum;
        private double[,] m_ItemDenom;
        private bool m_InitRandom;

        public NonnegativeMatrixFactorization() : this(MonsterMatchArrayData.FactorCount,
            MonsterMatchArrayData.UserCount,
            MonsterMatchArrayData.ItemCount,
            initialPu: MonsterMatchArrayData.Pu,
            initialQi: MonsterMatchArrayData.Qi)
        {
        }

        public NonnegativeMatrixFactorization(int nFactors, int userCount, int itemCount, bool initRandom = false,
            double[,] initialPu = null, double[,] initialQi = null, int nEpochs=DefaultEpochsCount)
        {
            m_NFactors = nFactors;
            m_InitRandom = initRandom;
            m_Pu = initialPu ?? new double[userCount, nFactors];
            m_Qi = initialQi ?? new double[itemCount, nFactors];
            m_Bu = new double[userCount];
            m_Bi = new double[itemCount];
            m_UserNum = new double[userCount, nFactors];
            m_UserDenom = new double[userCount, nFactors];
            m_ItemNum = new double[itemCount, nFactors];
            m_ItemDenom = new double[itemCount, nFactors];
            m_NEpochs = nEpochs;
        }

        public int nFactors
        {
            get { return m_NFactors; }
            set { m_NFactors = value; }
        }

        public int nEpochs
        {
            get { return m_NEpochs; }
            set { m_NEpochs = value; }
        }

        public float initLow
        {
            get => m_InitLow;
            set => m_InitLow = value;
        }

        public float initHigh
        {
            get => m_InitHigh;
            set => m_InitHigh = value;
        }

        public override void Fit(BaseDataSet dataSet)
        {
            Profiler.BeginSample("NMF.Fit");

            base.Fit(dataSet);

            if (m_InitRandom)
            {
                for (var i = 0; i < dataSet.userCount; i++)
                {
                    for (var j = 0; j < nFactors; j++)
                    {
                        if (Math.Abs(m_InitLow - m_InitHigh) < 0.001f)
                        {
                            m_Pu[i, j] = m_InitLow;
                        }
                        else
                        {
                            m_Pu[i, j] = Mathf.Lerp(m_InitLow, m_InitHigh, UnityEngine.Random.value);
                        }
                    }
                }

                for (var i = 0; i < dataSet.itemCount; i++)
                {
                    for (var j = 0; j < nFactors; j++)
                    {
                        if (Math.Abs(m_InitLow - m_InitHigh) < 0.001f)
                        {
                            m_Qi[i, j] = m_InitLow;
                        }
                        else
                        {
                            m_Qi[i, j] = Mathf.Lerp(m_InitLow, m_InitHigh, UnityEngine.Random.value);
                        }
                    }
                }
            }

            Array.Clear(m_Bu, 0, m_Bu.Length);
            Array.Clear(m_Bi, 0, m_Bi.Length);
            var globalMean = m_Biased ? dataSet.GlobalMean() : 0d;

            for (var currentEpoch = 0; currentEpoch < nEpochs; currentEpoch++)
            {
                Array.Clear(m_UserNum, 0, m_UserNum.Length);
                Array.Clear(m_UserDenom, 0, m_UserDenom.Length);
                Array.Clear(m_ItemNum, 0, m_ItemNum.Length);
                Array.Clear(m_ItemDenom, 0, m_ItemDenom.Length);

                foreach (var (u, i, r) in dataSet.allRatings)
                {
                    var dot = 0d;
                    for (var f = 0; f < nFactors; f++)
                    {
                        dot += m_Qi[i, f] * m_Pu[u, f];
                    }

                    var est = globalMean + m_Bu[u] + m_Bi[i] + dot;
                    var err = r - est;

                    // Update biases
                    if (m_Biased)
                    {
                        m_Bu[u] += m_LrBu * (err - m_RegBu * m_Bu[u]);
                        m_Bi[i] += m_LrBi * (err - m_RegBi * m_Bi[i]);
                    }

                    // Compute numerators and denominators
                    for (var f = 0; f < nFactors; f++)
                    {
                        m_UserNum[u, f] += m_Qi[i, f] * r;
                        m_UserDenom[u, f] += Math.Max(m_Qi[i, f] * est, Epsilon);
                        m_ItemNum[i, f] += m_Pu[u, f] * r;
                        m_ItemDenom[i, f] += Math.Max(m_Pu[u, f] * est, Epsilon);
                    }
                }

                // Update user factors
                foreach (var u in dataSet.allUsers)
                {
                    var nRatings = dataSet.UserRatingsCount(u);
                    for (var f = 0; f < nFactors; f++)
                    {
                        m_UserDenom[u, f] += nRatings * m_RegPu * m_Pu[u, f];
                        m_Pu[u, f] *= m_UserNum[u, f] / m_UserDenom[u, f];
                    }
                }

                // Update item factors
                foreach (var i in dataSet.allItems)
                {
                    var nRatings = dataSet.ItemRatingsCount(i);
                    for (var f = 0; f < nFactors; f++)
                    {
                        m_ItemDenom[i, f] += nRatings * m_RegQi * m_Qi[i, f];
                        m_Qi[i, f] *= m_ItemNum[i, f] / m_ItemDenom[i, f];
                    }
                }
            }

            Profiler.EndSample();
        }

        protected override double Estimate(int u, int i)
        {
            Profiler.BeginSample("NMF.Estimate");
            var knownUser = m_DataSet.KnowsUser(u);
            var knownItem = m_DataSet.KnowsItem(i);
            var est = 0d;

            if (m_Biased)
            {
                est = m_DataSet.GlobalMean();

                if (knownUser)
                {
                    est += m_Bu[u];
                }

                if (knownItem)
                {
                    est += m_Bi[i];
                }

                if (knownUser && knownItem)
                {
                    est += Dot(m_Qi, i, m_Pu, u, nFactors);
                }
            }
            else
            {
                if (knownUser && knownItem)
                {
                    est = Dot(m_Qi, i, m_Pu, u, nFactors);
                }
                else
                {
                    throw new Exception("Prediction impossible. User is unknown");
                }
            }

            Profiler.EndSample();
            return est;
        }

        private static double Dot(double[,] m1, int row1, double[,] m2, int row2, int f)
        {
            var dot = 0d;
            for (var i = 0; i < f; i++)
            {
                dot += m1[row1, i] * m2[row2, i];
            }

            return dot;
        }
    }
}