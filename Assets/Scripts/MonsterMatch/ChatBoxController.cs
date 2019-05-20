using System;
using HiddenSwitch;
using MonsterMatch;
using MonsterMatch.UI;
using UniRx;
using UniRx.Triggers;
using UnityEngine.EventSystems;

namespace UnityEngine.UI
{
    public class ChatBoxController : UIBehaviour
    {
        [SerializeField] private ConversationController m_ConversationController;
        [SerializeField] private ChatBoxView[] m_ChatBoxViews;
        [SerializeField] private InputBoxView m_InputBox;
        [SerializeField] private bool m_PostOwnMessages;
        [SerializeField] private TextKeyboardConnector m_KeyboardConnector;
        [SerializeField] private Text m_Text;
        [SerializeField] private string m_PlaceholderMessage = "Tap here";
        [SerializeField] private Color m_PlaceholderColor = Color.gray;
        [SerializeField] private Color m_TextColor = Color.black;

        [Tooltip("This spacer is shown/hidden whenever the text field should behave like a real text input box")]
        [SerializeField]
        private RectTransform m_SpacerForKeyboard;

        private readonly ReactiveCollection<string> m_Responses = new ReactiveCollection<string>();
        private readonly Subject<ChatBoxChoice> m_Choices = new Subject<ChatBoxChoice>();
        private readonly BoolReactiveProperty m_Open = new BoolReactiveProperty(false);

        public IReactiveCollection<string> responses => m_Responses;
        public IObservable<ChatBoxChoice> choices => m_Choices;
        private bool m_IsRequestingTextInput;
        protected override void Awake()
        {
            base.Awake();

            var version = new IntReactiveProperty(0);
            var sentVersion = -1;
            responses.Observe()
                .Subscribe(ignored => { version.Value = version.Value + 1; })
                .AddTo(this);


            for (var i = 0; i < m_ChatBoxViews.Length; i++)
            {
                var j = i;
                var view = m_ChatBoxViews[i];
                view.button.OnPointerClickAsObservable()
                    .Where(ignored => view.button.interactable)
                    .Subscribe(v =>
                    {
                        if (m_PostOwnMessages)
                        {
                            m_ConversationController.Add(new ChatMessage[]
                            {
                                new ChatMessage() {self = true, message = view.data}
                            });
                        }

                        SetChatBoxesInteractive(false);
                        sentVersion = version.Value;

                        m_Choices.OnNext(new ChatBoxChoice()
                        {
                            index = j,
                            text = view.data
                        });
                    })
                    .AddTo(view);
            }

            m_InputBox.OnPointerClickAsObservable()
                .Where(ignored => !m_IsRequestingTextInput)
                .Subscribe(ignored =>
                {
                    m_Open.Value = !m_Open.Value;
                    m_ConversationController.ThrottledScrollToBottom();
                })
                .AddTo(this);

            Observable.Merge(
                    responses.Observe().AsUnitObservable(),
                    m_Open.StartWith(false).AsUnitObservable())
                .BatchFrame(1, FrameCountType.Update)
                .Subscribe(ignored =>
                {
                    var count = responses.Count;
                    var isOpen = m_Open.Value;

                    if (count == 0)
                    {
                        isOpen = false;
                    }
                    else
                    {
                        if (m_Open.Value)
                        {
                            m_ConversationController.StartTyping(true);
                        }
                        else
                        {
                            m_ConversationController.StopTyping();
                        }
                    }

                    for (var i = 0; i < m_ChatBoxViews.Length; i++)
                    {
                        if (i < count)
                        {
                            m_ChatBoxViews[i].data = responses[i];
                        }
                        else
                        {
                            m_ChatBoxViews[i].data = null;
                        }

                        m_ChatBoxViews[i].gameObject.SetActive(isOpen && i < count);
                    }

                    m_InputBox.interactable = count > 0;
                    SetChatBoxesInteractive(version.Value != sentVersion);
                })
                .AddTo(this);

            // The input text that this has is only enabled when the user actually has to type something in.
            m_SpacerForKeyboard.gameObject.SetActive(false);
            m_KeyboardConnector.enabled = false;
        }

        private void SetChatBoxesInteractive(bool value)
        {
            foreach (var otherView in m_ChatBoxViews)
            {
                otherView.button.interactable = value;
            }
        }

        public void ShowOptions()
        {
            m_Open.Value = true;
        }

        public void HideOptions()
        {
            m_Open.Value = false;
        }

        public void RequestInput(Action<string> callback)
        {
            Debug.Assert(callback != null, "callback!=null");
            m_IsRequestingTextInput = true;
            m_KeyboardConnector.enabled = true;
            m_KeyboardConnector.SetKeyboardActive(true);
            m_KeyboardConnector.firstClick = true;
            HideOptions();
            m_SpacerForKeyboard.gameObject.SetActive(true);
            m_KeyboardConnector.textUpdateReceivers.RemoveAllListeners();
            m_KeyboardConnector.textUpdateReceivers.AsObservable().Take(1).Subscribe(ignored =>
                {
                    // On the first input, set the text to black
                    m_Text.color = m_TextColor;
                })
                .AddTo(this);
            // Show the keyboard and an input box
            m_KeyboardConnector.textDoneReceivers.RemoveAllListeners();
            m_KeyboardConnector.textDoneReceivers.AddListener(res =>
            {
                
                m_IsRequestingTextInput = false;
                // Hide the input spacer and disable the keyboard
                m_KeyboardConnector.SetKeyboardActive(false);
                m_SpacerForKeyboard.gameObject.SetActive(false);
                m_KeyboardConnector.enabled = false;
                m_Text.text = m_PlaceholderMessage;
                m_Text.color = m_PlaceholderColor;
                callback(res);
            });
        }
    }
}