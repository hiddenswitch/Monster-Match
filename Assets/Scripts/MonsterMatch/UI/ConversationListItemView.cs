using MaterialUI;
using UniRx;
using UniRx.Triggers;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace MonsterMatch.UI
{
    [RequireComponent(typeof(Button))]
    public sealed class ConversationListItemView : UIBehaviour, IHas<ConversationItem>
    {
        [SerializeField] private Text m_NameText;
        [SerializeField] private Text m_MessageText;
        [SerializeField] private ThumbnailView m_ThumbnailView;
        [SerializeField] private DialogueConversationController m_Controller;
        [SerializeField] private ScreenView m_ScreenView;
        [SerializeField] private MaterialScreen m_Screen;
        private ConversationItem m_Data;

        protected override void Awake()
        {
            base.Awake();
            var button = GetComponent<Button>();
            button.OnPointerClickAsObservable()
                .Subscribe(ignored =>
                {
                    m_Controller.profile.Value = m_Data.profile;
                    m_Controller.selectedDialogue.Value = m_Data.dialogue;
                    m_ScreenView.Transition(m_Screen.screenIndex);
                })
                .AddTo(this);
        }

        public ConversationItem data
        {
            get => m_Data;
            set
            {
                m_Data = value;
                m_NameText.text = value.profile.name;
                m_MessageText.text = value.dialogue.story.currentText;
                m_ThumbnailView.data = new ThumbnailItem()
                {
                    sprite = value.profile.portrait
                };
            }
        }
    }
}