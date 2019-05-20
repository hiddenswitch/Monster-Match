using System.Linq;
using MonsterMatch.CollaborativeFiltering;
using MonsterMatch.UI;
using UnityEngine;
using UnityEngine.EventSystems;

namespace MonsterMatch
{
    public sealed class RecommenderTest : UIBehaviour
    {
        [SerializeField] private bool m_TestSlopeOne;
        [SerializeField] private bool m_TestNonnegativeMatrixFactorization;

        protected override void Start()
        {
            base.Start();
            if (m_TestNonnegativeMatrixFactorization)
            {
                CollaborativeFilteringRecommender.TestNonnegativeMatrixFactorization();
            }
            else if (m_TestSlopeOne)
            {
                CollaborativeFilteringRecommender.TestSlopeOne();
            }
        }
    }
}