using System.Runtime.InteropServices;
using UniRx;
using UniRx.Triggers;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace MonsterMatch.UI
{
    [RequireComponent(typeof(Toggle))]
    public sealed class MuteButton : UIBehaviour
    {
#if UNITY_WEBGL && !UNITY_EDITOR
        [DllImport("__Internal")]
#else
        private static bool m_Muted = false;
#endif
        private static
#if UNITY_WEBGL && !UNITY_EDITOR
            extern
#endif
            bool ToggleMute()
#if UNITY_WEBGL && !UNITY_EDITOR
            ;
#else
        {
            Debug.Log("MuteButton.ToggleMute");
            m_Muted = !m_Muted;
            return m_Muted;
        }
#endif

        protected override void Awake()
        {
            base.Awake();
            var toggle = GetComponent<Toggle>();
            toggle.OnPointerClickAsObservable()
                .Select(ignored => { return ToggleMute(); })
                .DelayFrame(1)
                .Subscribe(muted => { toggle.isOn = muted; })
                .AddTo(this);
        }
    }
}