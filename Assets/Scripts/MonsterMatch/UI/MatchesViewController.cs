using System.Linq;
using MonsterMatch.Assets;
using UnityEngine;
using UnityEngine.EventSystems;

namespace MonsterMatch.UI
{
    public class MatchesViewController : UIBehaviour, IHas<MonsterMatchProfilesAsset>
    {
        [SerializeField] protected DatingController m_DatingController;
        [SerializeField] protected ThumbnailsPager m_ThumbnailsPager;
        [SerializeField] protected bool m_Reversed;

        protected override void OnEnable()
        {
            base.OnEnable();

            var items = m_DatingController.judgements.Select((item, i) =>
            {
                return new ThumbnailItem()
                {
                    label = (i + 1).ToString(),
                    matched = item.judgement == DatingProfileJudgement.Matched,
                    sprite = data[item.profileIndex].portrait
                };
            });

            if (m_Reversed)
            {
                items = items.Reverse();
            }

            m_ThumbnailsPager.items.Value = items.ToArray();
        }

        public MonsterMatchProfilesAsset data { get; set; }
    }
}