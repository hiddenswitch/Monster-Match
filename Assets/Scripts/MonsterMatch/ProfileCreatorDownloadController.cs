using MonsterMatch.Assets;
using UnityEngine;

namespace MonsterMatch
{
    public sealed class ProfileCreatorDownloadController : DownloadControllerBase<ProfileCreatorAsset>
    {
        [SerializeField] private CreateProfileController m_CreateProfileController;

        public override IHas<ProfileCreatorAsset> target => m_CreateProfileController;
    }
}