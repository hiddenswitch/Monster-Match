using System;
using UnityEngine.AddressableAssets;

namespace MonsterMatch.Assets
{
    [Serializable]
    public sealed class MonsterMatchProfilesAssetReference : AssetReferenceT<MonsterMatchProfilesAsset>
    {
        public MonsterMatchProfilesAssetReference(string guid) : base(guid)
        {
        }
    }
}