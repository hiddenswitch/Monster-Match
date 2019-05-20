using System.Numerics;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Vector2 = UnityEngine.Vector2;

namespace MonsterMatch.UI
{
    public sealed class ChatMessageView : UIBehaviour, IHas<ChatMessageModelView>, /*IHas<ChatMessage>,*/ ILayoutElement
    {
        [SerializeField] private ChatBubbleView m_Bubble;
        [SerializeField] private ContentSizeFitter m_ContentSizeFitter;
        [SerializeField] private float m_ArrowPaddingY = 14f;
        [SerializeField] private ChatBubbleStyle m_SelfStyle;
        [SerializeField] private ChatBubbleStyle m_OtherStyle;
        [SerializeField] private Text m_TextMessage;
        private ChatMessageModelView m_Data;
        private static readonly Vector2 m_Vector01 = new Vector2(0, 1);
        private static readonly Vector2 m_Vector11 = new Vector2(1, 1);
        public ChatBubbleView bubble => m_Bubble;
        public bool lastInSequence => m_Data.isLastInSequence;
        public ContentSizeFitter contentSizeFitter => m_ContentSizeFitter;

#if UNITY_EDITOR
        protected override void OnValidate()
        {
            base.OnValidate();
        }
#endif
        public ChatMessageModelView data
        {
            get { return m_Data; }
            set
            {
                m_Data = value;
                var self = m_Data.chatMessage.self;
                if (value.chatMessage.noBubble)
                {
                    bubble.gameObject.SetActive(false);
                    m_TextMessage.gameObject.SetActive(true);
                    m_TextMessage.text = value.chatMessage?.message;
                }
                else
                {
                    bubble.gameObject.SetActive(true);
                    m_TextMessage.gameObject.SetActive(false);
                    bubble.style = self ? m_SelfStyle : m_OtherStyle;
                    bubble.text = value.chatMessage?.message;
                    bubble.image = value.chatMessage?.image;
                    bubble.showArrow = value.isLastInSequence;
                    bubble.arrowAlignment =
                        self ? ChatBubbleArrowAlignment.BottomRight : ChatBubbleArrowAlignment.BottomLeft;
                    bubble.rectTransform.anchorMin = self ? m_Vector11 : m_Vector01;
                    bubble.rectTransform.anchorMax = self ? m_Vector11 : m_Vector01;
                    bubble.rectTransform.anchoredPosition = new Vector2();
                }
            }
        }

/*
        ChatMessage IHas<ChatMessage>.data
        {
            get => m_Data.chatMessage;
            set
            {
                var chatMessageModelView = m_Data;
                chatMessageModelView.chatMessage = value;
                m_Data = chatMessageModelView;
            }
        }
*/
        public void CalculateLayoutInputHorizontal()
        {
        }

        public void CalculateLayoutInputVertical()
        {
            if (bubble.gameObject.activeSelf)
            {
                bubble.CalculateLayoutInputVertical();
            }
            else
            {
                m_TextMessage.CalculateLayoutInputVertical();
            }
        }

        public float minWidth { get; }
        public float preferredWidth { get; }
        public float flexibleWidth { get; }
        public float minHeight { get; }

        public float preferredHeight
        {
            get
            {
                var bubbleHeight = bubble.gameObject.activeSelf
                    ? bubble.preferredHeight + (lastInSequence ? m_ArrowPaddingY : 0f)
                    : 0;
                var textHeight = m_TextMessage.gameObject.activeSelf ? m_TextMessage.preferredHeight : 0;
                return bubbleHeight + textHeight;
            }
        }

        public float flexibleHeight { get; }
        public int layoutPriority { get; }
    }
}