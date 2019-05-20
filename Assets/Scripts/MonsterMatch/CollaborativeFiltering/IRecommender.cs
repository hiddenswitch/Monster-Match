using System.Collections.Generic;

namespace MonsterMatch.CollaborativeFiltering
{
    public interface IRecommender
    {
        void AddPlayerRating(int profileZeroBasedIndex, bool matched);
        void Fit();
        IEnumerable<int> Top();
    }
}