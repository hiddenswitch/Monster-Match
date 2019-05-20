using System.Linq;
using HiddenSwitch;
using UnityEngine;

namespace MonsterMatch.UI
{
    public sealed class RecommendationsViewController : MatchesViewController
    {
        [SerializeField] private DatingStoryController m_DatingStoryController;
        [SerializeField] private int m_Take = 3;
        [SerializeField] private bool m_Shuffle = false;
        [SerializeField] private int m_TakeShuffle = 10;

        protected override void OnEnable()
        {
            var items = m_DatingStoryController.recommender.Top()
                .Except(m_DatingStoryController.seen)
                .Select((profileIndex, i) =>
                {
                    return new ThumbnailItem()
                    {
                        label = null,
                        matched = null,
                        sprite = data[profileIndex].portrait
                    };
                });

            if (m_Reversed)
            {
                items = items.Reverse();
            }

            if (m_Shuffle)
            {
                items = items.Take(m_TakeShuffle).Shuffled();
            }

            m_ThumbnailsPager.items.Value = items
                .Take(m_Take)
                .ToArray();
        }
    }
}