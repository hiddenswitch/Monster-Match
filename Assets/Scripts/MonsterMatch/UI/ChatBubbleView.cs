using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace MonsterMatch.UI
{
    [ExecuteAlways]
    [RequireComponent(typeof(RectTransform))]
    public sealed class ChatBubbleView : UIBehaviour, ILayoutSelfController, ILayoutController, ILayoutElement
    {
        [TextArea(3, 10)] [SerializeField] private string m_Text = string.Empty;
        [SerializeField] private Sprite m_Image;
        [SerializeField] private Text m_TextField;
        [SerializeField] private Image m_ImageField;
        [SerializeField] private int m_LayoutPriority;
        [SerializeField] private RectTransform m_ImageTarget;

        [FormerlySerializedAs("m_Target")] [SerializeField]
        private RectTransform m_TextTarget;

        [SerializeField] private float m_MinWidth;
        [SerializeField] private float m_MaxWidth;
        [SerializeField] private float m_MinHeight;
        [SerializeField] private float m_PaddingLeft;
        [SerializeField] private float m_PaddingTop;
        [SerializeField] private float m_PaddingRight;
        [SerializeField] private float m_PaddingBottom;
        [SerializeField] private bool m_ShowArrow = true;
        [SerializeField] private ChatBubbleArrowAlignment m_ArrowAlignment;
        [Header("Style")] [SerializeField] private ChatBubbleStyle m_Style;
        [SerializeField] private Image m_Frame;
        [SerializeField] private Image m_Stitching;
        [SerializeField] private Image m_Gloss;
        [SerializeField] private Image m_Arrow;
        private RectTransform m_Rect;
        private DrivenRectTransformTracker m_Tracker = new DrivenRectTransformTracker();
        private float m_PreferredWidth;
        private float m_PreferredHeight;
        private float m_PreferredWidthErrorCorrection = 14f;

        public RectTransform rectTransform
        {
            get
            {
                if (m_Rect == null)
                {
                    m_Rect = GetComponent<RectTransform>();
                }

                return m_Rect;
            }
        }

        public RectTransform target
        {
            get
            {
                if (m_Image != null)
                {
                    return m_ImageTarget;
                }
                else
                {
                    return m_TextTarget;
                }
            }
        }

        public string text
        {
            get => m_Text;
            set
            {
                m_Text = value;
                m_TextField.text = value;
                SetDirty();
            }
        }

        public Sprite image
        {
            get => m_Image;
            set
            {
                m_Image = value;
                m_ImageField.sprite = value;
                SetDirty();
            }
        }

        public bool showArrow
        {
            get { return m_ShowArrow; }
            set
            {
                m_ShowArrow = value;
                SetDirty();
            }
        }

        private void HandleSelfFittingAlongAxis(RectTransform.Axis axis)
        {
            if (axis == RectTransform.Axis.Vertical)
            {
                m_Tracker.Add(this, rectTransform, DrivenTransformProperties.SizeDeltaY);
                CalculateLayoutInputVertical();
                rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, m_PreferredHeight);
            }
            else
            {
                m_Tracker.Add(this, rectTransform, DrivenTransformProperties.PivotX);
                m_Tracker.Add(this, rectTransform, DrivenTransformProperties.SizeDeltaX);

                var pivot = rectTransform.pivot;
                switch (m_ArrowAlignment)
                {
                    case ChatBubbleArrowAlignment.BottomLeft:
                    default:
                        pivot.x = 0f;
                        break;
                    case ChatBubbleArrowAlignment.BottomRight:
                        pivot.x = 1f;
                        break;

                    case ChatBubbleArrowAlignment.TopCenter:
                        pivot.x = 0.5f;
                        break;
                }

                rectTransform.pivot = pivot;

                CalculateLayoutInputHorizontal();

                rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, m_PreferredWidth);
            }
        }

        /// <summary>
        ///   <para>Method called by the layout system.</para>
        /// </summary>
        public void SetLayoutHorizontal()
        {
            m_Tracker.Clear();
            HandleSelfFittingAlongAxis(RectTransform.Axis.Horizontal);
        }

        /// <summary>
        ///   <para>Method called by the layout system.</para>
        /// </summary>
        public void SetLayoutVertical()
        {
            HandleSelfFittingAlongAxis(RectTransform.Axis.Vertical);
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
            m_Tracker.Clear();
            LayoutRebuilder.MarkLayoutForRebuild(rectTransform);
            base.OnDisable();
        }


        /// <summary>
        ///   <para>Mark the view as dirty.</para>
        /// </summary>
        private void SetDirty()
        {
            if (!IsActive())
            {
                return;
            }

            LayoutRebuilder.MarkLayoutForRebuild(rectTransform);

            if (m_TextField != null)
            {
                m_TextField.enabled = !string.IsNullOrEmpty(m_Text);
                m_TextField.text = m_Text;
            }

            var showingImage = m_Image != null;

            if (m_ImageField != null)
            {
                m_ImageField.enabled = showingImage;
                m_ImageField.sprite = m_Image;
            }


            Vector2 anchorMin;
            Vector2 anchorMax;

            switch (m_ArrowAlignment)
            {
                case ChatBubbleArrowAlignment.BottomLeft:
                default:
                    anchorMin = new Vector2();
                    anchorMax = new Vector2();
                    break;
                case ChatBubbleArrowAlignment.BottomRight:
                    anchorMin = new Vector2(1, 0);
                    anchorMax = new Vector2(1, 0);
                    break;
                case ChatBubbleArrowAlignment.TopCenter:
                    anchorMin = new Vector2(0.5f, 1);
                    anchorMax = new Vector2(0.5f, 1);
                    break;
            }

            if (m_Style == null || m_Arrow == null || m_Frame == null || m_Stitching == null || m_TextField == null ||
                m_Gloss == null)
            {
                return;
            }

            var arrowStyle = m_Style.arrowStyles[m_ArrowAlignment];
            m_Arrow.sprite = arrowStyle.sprite;
            var arrowRect = m_Arrow.rectTransform;
            arrowRect.anchorMin = anchorMin;
            arrowRect.anchorMax = anchorMax;
            arrowRect.anchoredPosition = arrowStyle.anchoredPosition;
            m_Arrow.gameObject.SetActive(!showingImage && m_ShowArrow);

            m_Frame.sprite = m_Style.frame;
            m_Stitching.sprite = m_Style.stitching;
            m_Gloss.sprite = m_Style.gloss;
            m_TextField.color = m_Style.textColor;
        }

#if UNITY_EDITOR
        protected override void OnValidate()
        {
            SetDirty();
        }
#endif
        public void CalculateLayoutInputHorizontal()
        {
            if (target == null)
            {
                m_PreferredWidth = 0;
                return;
            }

            var preferredSize = LayoutUtility.GetPreferredSize(target, 0);
            m_PreferredWidth = m_PaddingLeft +
                               Mathf.Clamp(preferredSize + m_PreferredWidthErrorCorrection, m_MinWidth, m_MaxWidth) +
                               m_PaddingRight;
        }

        public void CalculateLayoutInputVertical()
        {
            if (target == null)
            {
                m_PreferredHeight = 0;
                return;
            }

            m_PreferredHeight = m_PaddingTop + Mathf.Max(m_MinHeight, LayoutUtility.GetPreferredSize(target, 1)) +
                                m_PaddingBottom;
        }

        public float minWidth => m_MinWidth;
        public float preferredWidth => m_PreferredWidth;
        public float flexibleWidth => 0f;
        public float minHeight => m_MinHeight;
        public float preferredHeight => m_PreferredHeight;
        public float flexibleHeight => 0f;
        public int layoutPriority => m_LayoutPriority;

        public ChatBubbleStyle style
        {
            get { return m_Style; }
            set
            {
                m_Style = value;
                SetDirty();
            }
        }

        public ChatBubbleArrowAlignment arrowAlignment
        {
            get => m_ArrowAlignment;
            set
            {
                m_ArrowAlignment = value;
                SetDirty();
            }
        }
    }
}