using UniRx;
using UniRx.Triggers;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace MonsterMatch.UI
{
    [RequireComponent(typeof(Button))]
    public sealed class ChatBoxBackButton : UIBehaviour
    {
        [SerializeField] private DatingController m_Controller;

        protected override void Awake()
        {
            base.Awake();

            var button = GetComponent<Button>();
            button.OnPointerClickAsObservable()
                .Subscribe(ignored => { m_Controller.HideNewMessageModal(); })
                .AddTo(this);
        }
    }
}