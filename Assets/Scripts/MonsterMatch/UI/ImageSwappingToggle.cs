using UniRx;
using UnityEngine;
using UnityEngine.UI;

namespace MonsterMatch.UI
{
    public sealed class ImageSwappingToggle : Toggle
    {
        [SerializeField] private Image m_Button;
        [SerializeField] private Image m_Icon;
        [SerializeField] private Sprite m_IconOn;
        [SerializeField] private Sprite m_IconOff;

        protected override void Awake()
        {
            base.Awake();

            if (Application.isEditor && !Application.isPlaying)
            {
                return;
            }

            this.OnValueChangedAsObservable()
                .Subscribe(val =>
                {
                    if (m_Button != null)
                    {
                        image.enabled = !val;
                        m_Button.gameObject.SetActive(val);
                        m_Button.overrideSprite = val ? spriteState.pressedSprite : spriteState.highlightedSprite;
                    }

                    if (m_Icon != null)
                    {
                        m_Icon.sprite = val ? m_IconOn : m_IconOff;
                    }
                })
                .AddTo(this);
        }
    }
}