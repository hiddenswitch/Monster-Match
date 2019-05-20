using Com.TheFallenGames.OSA.Core;
using UnityEngine;

namespace MonsterMatch.UI
{
    public sealed class ChatMessageViewsHolder : BaseItemViewsHolder
    {
        private const string chatMessageObjectName = "Chat Message View";

        private ChatMessageView m_ChatMessageView;

        public ChatMessageView chatMessageView
        {
            get
            {
                if (m_ChatMessageView == null)
                {
                    m_ChatMessageView = root.GetComponent<ChatMessageView>();
                }

                return m_ChatMessageView;
            }
        }

        public override void Init(GameObject rootPrefabGo, int itemIndex, bool activateRootGameObject = true,
            bool callCollectViews = true)
        {
            base.Init(rootPrefabGo, itemIndex, activateRootGameObject, callCollectViews);
            root.name = $"{chatMessageObjectName} {itemIndex}";
        }

        public override void MarkForRebuild()
        {
            base.MarkForRebuild();
            chatMessageView.contentSizeFitter.enabled = true;
        }
    }
}