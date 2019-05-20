using System;
using Com.TheFallenGames.OSA.Core;
using Com.TheFallenGames.OSA.CustomParams;
using UnityEngine;

namespace MonsterMatch
{
    [Serializable]
    public class BaseParamsWithPrefabFixedDefaultSize : BaseParamsWithPrefab
    {
        public override float ItemPrefabSize
        {
            get { return DefaultItemSize; }
        }
    }
}