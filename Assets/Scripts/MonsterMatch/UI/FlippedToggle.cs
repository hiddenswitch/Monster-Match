using UniRx;
using UniRx.Triggers;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace MonsterMatch.UI
{
    [RequireComponent(typeof(Toggle))]
    public sealed class FlippedToggle : UIBehaviour
    {
        [SerializeField] private CreateProfileController m_Controller;

        protected override void Awake()
        {
            base.Awake();

            var isUserInitiated = false;
            var toggle = GetComponent<Toggle>();
            Observable.Merge(toggle.OnPointerClickAsObservable()
                        .Where(p => p.button == PointerEventData.InputButton.Left)
                        .AsUnitObservable(),
                    toggle.OnSubmitAsObservable()
                        .AsUnitObservable())
                .Subscribe(ignored => { isUserInitiated = true; })
                .AddTo(this);
            m_Controller.flipped
                .DelayFrame(1)
                .Where(ignored => !isUserInitiated)
                .Subscribe(val => { toggle.isOn = val; })
                .AddTo(this);

            toggle.OnValueChangedAsObservable()
                .DelayFrame(1)
                .Where(ignored => isUserInitiated)
                .Subscribe(val =>
                {
                    m_Controller.flipped.Value = val;
                    isUserInitiated = false;
                })
                .AddTo(this);
        }
    }
}