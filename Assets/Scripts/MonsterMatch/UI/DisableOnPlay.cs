using UnityEngine.EventSystems;

namespace MonsterMatch.UI
{
    public sealed class DisableOnPlay : UIBehaviour
    {
        protected override void Awake()
        {
            base.Awake();
            gameObject.SetActive(false);
        }
    }
}