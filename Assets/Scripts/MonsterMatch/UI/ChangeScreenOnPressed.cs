using MaterialUI;
using UniRx;
using UniRx.Triggers;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace MonsterMatch.UI
{
    public class ChangeScreenOnPressed : UIBehaviour
    {
        [SerializeField] protected Button m_Button;
        [SerializeField] protected ScreenView m_ScreenView;
        [SerializeField] protected MaterialScreen m_Destination;

        protected override void Start()
        {
            base.Start();
            m_Button.OnClickAsObservable()
                .Subscribe(ignored => m_ScreenView.Transition(m_Destination.screenIndex))
                .AddTo(this);
        }

        public MaterialScreen destination
        {
            get => m_Destination;
            set => m_Destination = value;
        }
    }
}