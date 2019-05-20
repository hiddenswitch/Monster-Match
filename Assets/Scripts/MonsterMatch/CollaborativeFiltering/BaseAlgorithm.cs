using System;

namespace MonsterMatch.CollaborativeFiltering
{
    public abstract class BaseAlgorithm
    {
        protected BaseDataSet m_DataSet;

        public virtual void Fit(BaseDataSet dataSet)
        {
            m_DataSet = dataSet;
        }

        public Prediction Predict(int uid, int iid, double rUi = double.NaN, bool clip = true,
            bool verbose = false)
        {
            double est;
            var wasImpossible = false;
            try
            {
                est = Estimate(uid, iid);
            }
            catch
            {
                est = DefaultPrediction();
                wasImpossible = true;
            }

            est = Math.Min(m_DataSet.ratingScaleHigherBound, est);
            est = Math.Max(m_DataSet.ratingScaleLowerBound, est);

            return new Prediction
            {
                est = est,
                uid = uid,
                iid = iid,
                rUi = rUi,
                wasImpossible = wasImpossible
            };
        }

        protected double DefaultPrediction()
        {
            return m_DataSet.GlobalMean();
        }

        protected abstract double Estimate(int u, int i);
    }
}