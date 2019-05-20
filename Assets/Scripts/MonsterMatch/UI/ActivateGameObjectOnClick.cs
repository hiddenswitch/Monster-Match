using UniRx;
using UniRx.Triggers;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace MonsterMatch.UI
{
    [RequireComponent(typeof(Button))]
    public sealed class ActivateGameObjectOnClick : UIBehaviour
    {
        [SerializeField] private GameObject m_Target;
        [SerializeField] private bool m_Active;

        protected override void Awake()
        {
            GetComponent<Button>()
                .OnPointerClickAsObservable()
                .Subscribe(ignored => { m_Target.SetActive(m_Active); });
        }
    }
}