using UniRx;
using UnityEngine;
using UnityEngine.UI;

namespace MonsterMatch.UI
{
    [RequireComponent(typeof(Toggle))]
    public sealed class TranslateOnToggled : TranslateOnPressed
    {
        protected override void Start()
        {
            base.Start();

            var toggle = m_Selectable as Toggle;

            if (toggle == null)
            {
                return;
            }

            toggle.OnValueChangedAsObservable()
                .Subscribe(isOn => { isOffset.Value = isOn; })
                .AddTo(this);
        }
    }
}