using UnityEngine;
using UnityEngine.EventSystems;

namespace MonsterMatch.UI
{
    public sealed class OnlyActiveOnMobile : UIBehaviour
    {
        protected override void Awake()
        {
            gameObject.SetActive(Application.isMobilePlatform);
        }
    }
}