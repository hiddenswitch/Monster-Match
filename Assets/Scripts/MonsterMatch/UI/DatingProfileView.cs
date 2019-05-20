using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace MonsterMatch.UI
{
    public sealed class DatingProfileView : Graphic, IDragHandler, IBeginDragHandler, IEndDragHandler,
        IHas<DatingProfileItem>
    {
        [SerializeField] private Button m_AcceptButton;
        [SerializeField] private Button m_RejectButton;
        [SerializeField] private Text m_BylineText;
        [SerializeField] private Text m_NameText;
        [SerializeField] private Text m_BodyText;
        [SerializeField] private CanvasGroup m_CanvasGroup;
        [SerializeField] private Image m_Image;
        [SerializeField] private DrawingView m_DrawingView;
        [SerializeField] private CounterView m_MatchingCount;
        [SerializeField] private NumberView m_MessagesCount;
        private DatingProfileItem m_Data;

        public Button acceptButton => m_AcceptButton;
        public Button rejectButton => m_RejectButton;

        public CanvasGroup canvasGroup => m_CanvasGroup;

        public Image image => m_Image;

        public DrawingView drawingView => m_DrawingView;

        public CounterView matchingCount => m_MatchingCount;

        public NumberView messagesCount => m_MessagesCount;

        public void OnDrag(PointerEventData eventData)
        {
        }

        public void OnBeginDrag(PointerEventData eventData)
        {
        }

        public void OnEndDrag(PointerEventData eventData)
        {
        }

        public DatingProfileItem data
        {
            get => m_Data;
            set
            {
                m_Data = value;
                m_BodyText.text = m_Data.body;
                m_NameText.text = m_Data.name;
                m_BylineText.text = m_Data.byline;
                m_Image.sprite = m_Data.portrait;
                if (m_MatchingCount != null)
                {
                    m_MatchingCount.data = new CountItem()
                    {
                        numerator = m_Data.reciprocalMatchCount,
                        denominator = m_Data.totalProfilesSeen
                    };
                }

                LayoutRebuilder.ForceRebuildLayoutImmediate(rectTransform);
            }
        }
    }
}