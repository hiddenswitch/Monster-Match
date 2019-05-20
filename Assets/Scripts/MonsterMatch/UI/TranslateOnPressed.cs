using UniRx;
using UniRx.Triggers;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace MonsterMatch.UI
{
    public class TranslateOnPressed : UIBehaviour
    {
        protected bool m_Pressed;
        protected bool m_Entered;
        [SerializeField] protected RectTransform m_ToTranslate;
        [SerializeField] protected Vector2 m_Offset;
        protected Selectable m_Selectable;

        protected BoolReactiveProperty isOffset { get; } = new BoolReactiveProperty(false);

        protected override void Awake()
        {
            base.Awake();
            m_Selectable = GetComponent<Selectable>();
        }

        protected override void Start()
        {
            isOffset.DistinctUntilChanged()
                .Skip(1)
                .Subscribe(val =>
                {
                    if (val)
                    {
                        m_ToTranslate.anchoredPosition += m_Offset;
                    }
                    else
                    {
                        m_ToTranslate.anchoredPosition -= m_Offset;
                    }
                })
                .AddTo(this);
            
            m_Selectable.OnPointerEnterAsObservable()
                .Subscribe(ignored =>
                {
                    if (m_Pressed && !m_Entered)
                    {
                        isOffset.Value = true;
                    }

                    m_Entered = true;
                })
                .AddTo(this);

            m_Selectable.OnPointerDownAsObservable()
                .Where(ignored => !m_Pressed && m_Selectable.interactable)
                .Subscribe(ignored =>
                {
                    if (m_Selectable.interactable && !m_Pressed)
                    {
                        isOffset.Value = true;
                    }

                    m_Pressed = true;
                })
                .AddTo(this);

            m_Selectable.OnPointerUpAsObservable()
                .Where(ignored => m_Pressed)
                .Subscribe(ignored =>
                {
                    if (m_Pressed && m_Entered)
                    {
                        isOffset.Value = false;
                    }

                    m_Pressed = false;
                })
                .AddTo(this);

            m_Selectable.OnPointerExitAsObservable()
                .Where(ignored => m_Pressed)
                .Subscribe(ignored =>
                {
                    if (m_Pressed && m_Entered)
                    {
                        isOffset.Value = false;
                    }

                    m_Entered = false;
                })
                .AddTo(this);
        }
    }
}