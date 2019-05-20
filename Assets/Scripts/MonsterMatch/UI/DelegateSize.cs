using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace MonsterMatch.UI
{
    public sealed class DelegateSize : UIBehaviour, ILayoutElement
    {
        [SerializeField] private Component m_Target;
        [SerializeField] private float m_PaddingX;
        [SerializeField] private float m_PaddingY;
        private ILayoutElement m_LayoutElement;
        private float m_PreferredHeight;
        private float m_PreferredWidth;

        private ILayoutElement layoutElement
        {
            get
            {
                if (m_LayoutElement == null && m_Target != null)
                {
                    m_LayoutElement = m_Target as ILayoutElement;
                }

                return m_LayoutElement;
            }
        }

        public Component target
        {
            get => m_Target;
            set
            {
                m_Target = value;
                SetDirty();
            }
        }

        public float paddingX
        {
            get => m_PaddingX;
            set
            {
                m_PaddingX = value;
                CalculateLayoutInputHorizontal();
                LayoutRebuilder.MarkLayoutForRebuild(transform as RectTransform);
            }
        }

        public float paddingY
        {
            get => m_PaddingY;
            set
            {
                m_PaddingY = value;
                CalculateLayoutInputVertical();
                LayoutRebuilder.MarkLayoutForRebuild(transform as RectTransform);
            }
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            SetDirty();
        }

        /// <summary>
        ///   <para>See MonoBehaviour.OnDisable.</para>
        /// </summary>
        protected override void OnDisable()
        {
            base.OnDisable();
        }


        /// <summary>
        ///   <para>Mark the view as dirty.</para>
        /// </summary>
        public void SetDirty()
        {
            if (!IsActive())
            {
                return;
            }

            m_LayoutElement = m_Target as ILayoutElement;
            CalculateLayoutInputHorizontal();
            CalculateLayoutInputVertical();
            LayoutRebuilder.MarkLayoutForRebuild(transform as RectTransform);
        }

        public void CalculateLayoutInputHorizontal()
        {
            layoutElement.CalculateLayoutInputHorizontal();
            m_PreferredWidth = (layoutElement?.preferredWidth ?? 0f) + m_PaddingX;
        }

        public void CalculateLayoutInputVertical()
        {
            layoutElement.CalculateLayoutInputVertical();
            m_PreferredHeight = (layoutElement?.preferredHeight ?? 0f) + m_PaddingY;
        }

        public float minWidth => (layoutElement?.minWidth ?? 0f) + m_PaddingX;
        public float preferredWidth => m_PreferredWidth;
        public float flexibleWidth => layoutElement?.flexibleWidth ?? 0f;
        public float minHeight => (layoutElement?.minHeight ?? 0f) + m_PaddingY;
        public float preferredHeight => m_PreferredHeight;
        public float flexibleHeight => layoutElement?.flexibleHeight ?? 0f;
        public int layoutPriority => layoutElement?.layoutPriority ?? 0;


#if UNITY_EDITOR
        protected override void OnValidate()
        {
            SetDirty();
        }
#endif
    }
}