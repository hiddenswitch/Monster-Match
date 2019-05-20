using MaterialUI;
using UniRx;
using UnityEngine;
using UnityEngine.EventSystems;

namespace MonsterMatch.UI
{
    public sealed class ActiveForScreen : UIBehaviour
    {
        [SerializeField] private ScreenView m_UiScreenView;
        [SerializeField] private MaterialScreen m_Screen;
        [SerializeField] private GameObject m_Target;
        [SerializeField] private bool m_Default = false;

        protected override void Awake()
        {
            m_Target.SetActive(m_Default);
            m_UiScreenView.onScreenBeginTransition.AsObservable()
                .StartWith(m_UiScreenView.currentScreen)
                .BatchFrame(1, FrameCountType.Update)
                .Select(frame => frame[frame.Count - 1])
                .Subscribe(targetScreen => { m_Target.SetActive(targetScreen == m_Screen.screenIndex); })
                .AddTo(this);
        }
    }
}