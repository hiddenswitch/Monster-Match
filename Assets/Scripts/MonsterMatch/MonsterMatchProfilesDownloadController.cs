using System.Linq;
using MonsterMatch.Assets;
using MonsterMatch.UI;
using UnityEngine;

namespace MonsterMatch
{
    public sealed class MonsterMatchProfilesDownloadController : DownloadControllerBase<MonsterMatchProfilesAsset>
    {
        [SerializeField] private DatingStoryController m_DatingStoryController;
        [SerializeField] private DialogueConversationController m_DialogueConversationController;
        [SerializeField] private MatchesViewController[] m_MatchesViewControllers = new MatchesViewController[0];

        public override IHas<MonsterMatchProfilesAsset>[] targets =>
            new IHas<MonsterMatchProfilesAsset>[] {m_DatingStoryController, m_DialogueConversationController}
                .Concat(m_MatchesViewControllers).ToArray();
    }
}