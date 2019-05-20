using System;
using UnityEngine.AddressableAssets;

namespace MonsterMatch.Assets
{
    [Serializable]
    public sealed class ProfileCreatorAssetReference : AssetReferenceT<ProfileCreatorAsset>
    {
        public ProfileCreatorAssetReference(string guid) : base(guid)
        {
        }
    }
}