using UniRx;
using UniRx.Triggers;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace MonsterMatch.UI
{
    [RequireComponent(typeof(Button))]
    public sealed class LayerChangeButton : UIBehaviour
    {
        [SerializeField] private CreateProfileController m_Controller;
        [SerializeField] private int m_Amount;

        protected override void Awake()
        {
            base.Awake();
            var button = GetComponent<Button>();

            m_Controller.canUndo.Subscribe(canUndo => { button.interactable = canUndo; });
            button.OnPointerClickAsObservable()
                .Subscribe(ignored => { m_Controller.MoveLastStamp(siblingIndex: m_Amount); })
                .AddTo(this);
        }
    }
}