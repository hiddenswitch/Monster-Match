using System;
using System.Collections.Generic;
using System.Linq;
using Com.TheFallenGames.OSA.Core;
using Com.TheFallenGames.OSA.CustomParams;
using MonsterMatch.UI;
using UniRx;
using UniRx.Diagnostics;
using UnityEngine;
using UnityEngine.UI;

namespace MonsterMatch
{
    public sealed class ConversationController : OSA<BaseParamsWithPrefab, ChatMessageViewsHolder>
    {
        private IDictionary<object, ChatMessageModelView[]> m_StateCache = new Dictionary<object, ChatMessageModelView[]>();

        private enum ScrollType
        {
            Smooth,
            Immediate
        }

        private enum TypingType
        {
            NotTyping,
            Self,
            NotSelf
        }

        [SerializeField] private float m_SmoothScrollDuration = 0.3f;
        [SerializeField] private GameObject m_IsTypingObject;
        [SerializeField] private Text m_IsTypingText;
        [SerializeField] private Text m_Banner;
        [SerializeField] private string m_ConversantName = "Mr. Pickles";
        [SerializeField] private string m_SelfMessage = "You are typing...";
        [SerializeField] private string m_NotSelfMessage = "{0} is typing...";

        private readonly ReactiveCollection<ChatMessageModelView> m_Data =
            new ReactiveCollection<ChatMessageModelView>();

        private readonly Subject<ScrollType> m_ScrollSubject = new Subject<ScrollType>();

        private readonly BehaviorSubject<TypingType>
            m_SetTyping = new BehaviorSubject<TypingType>(TypingType.NotTyping);

        public IReactiveCollection<ChatMessageModelView> data => m_Data;

        public string conversantName
        {
            get => m_ConversantName;
            set
            {
                m_ConversantName = value;
                m_Banner.text = value;
                var typingValue = m_SetTyping.Value;
                SetTypingText(typingValue);
            }
        }

        private void SetTypingText(TypingType typingValue)
        {
            m_IsTypingObject.SetActive(typingValue != TypingType.NotTyping);
            switch (typingValue)
            {
                case TypingType.Self:
                    m_IsTypingText.text = m_SelfMessage;
                    break;
                case TypingType.NotSelf:
                    m_IsTypingText.text = string.Format(m_NotSelfMessage, m_ConversantName);
                    break;
            }
        }

        /// <summary>
        /// Adds a chat message to this conversation controller for rendering
        /// </summary>
        /// <param name="chatMessages">A list of chat messages that should be added</param>
        public void Add(ChatMessage[] chatMessages)
        {
            var messages = chatMessages.ToArray();
            if (messages.Length == 0)
            {
                return;
            }

            if (data.Count > 0)
            {
                var lastIndex = data.Count - 1;
                var last = data[lastIndex];
                // If the message we're adding is from the same user as the last message, we have to remove the arrow on
                // the last message. This will be achieved by marking the last message as no longer the last in sequence
                // and flagging that it needs a visual size change.
                if (last.chatMessage.self == messages[0].self
                    && last.isLastInSequence)
                {
                    last.isLastInSequence = false;
                    last.hasPendingVisualSizeChange = true;
                    // Emits replace event, which may be excessive.
                    data[lastIndex] = last;
                }
            }

            for (var i = 0; i < messages.Length; i++)
            {
                var isLastInSequence = i + 1 == messages.Length || messages[i + 1].self != messages[i].self;
                if (!messages[i].self)
                {
                    m_SetTyping.OnNext(TypingType.NotTyping);
                }

                // Always add the item.
                data.Add(new ChatMessageModelView()
                {
                    chatMessage = messages[i],
                    hasPendingVisualSizeChange = true,
                    isLastInSequence = isLastInSequence
                });
            }
        }

        protected override void OnInitialized()
        {
            if (!IsInitialized)
            {
                var initial = Enumerable
                    .Range(0, data.Count)
                    .Select(i => new CollectionAddEvent<ChatMessageModelView>(i, data[i]));

                m_ScrollSubject
                    .BatchFrame(1, FrameCountType.Update)
                    .Subscribe(commands =>
                    {
                        if (data.Count == 0)
                        {
                            return;
                        }

                        // Determine if we actually need to scroll
                        if (GetContentSize() < GetViewportSize())
                        {
                            return;
                        }

                        switch (commands[commands.Count - 1])
                        {
                            case ScrollType.Smooth:
                                SmoothScrollTo(data.Count - 1, m_SmoothScrollDuration, 1f, 1f,
                                    overrideCurrentScrollingAnimation: true);
                                break;
                            default:
                            case ScrollType.Immediate:
                                ScrollTo(data.Count - 1, 1f, 1f);
                                break;
                        }
                    })
                    .AddTo(this);

                m_SetTyping.BatchFrame(1, FrameCountType.Update)
                    .Subscribe(typings =>
                    {
                        var value = typings[typings.Count - 1];
                        SetTypingText(value);
                    })
                    .AddTo(this);

                data.ObserveAdd()
                    .StartWith(initial)
                    .BatchFrame(1, FrameCountType.Update)
                    .Subscribe(items =>
                    {
                        // If the batch came in exactly the right order, then we can do an optimized addition.
                        if (Mathf.Abs(items[items.Count - 1].Index - items[0].Index) + 1 == items.Count)
                        {
                            ChangeItemsCount(ItemCountChangeMode.INSERT, items.Count, items[0].Index, false, true);
                        }
                        else
                        {
                            foreach (var item in items)
                            {
                                ChangeItemsCount(ItemCountChangeMode.INSERT, 1, item.Index);
                            }
                        }

                        ThrottledSmoothScrollToBottom();
                    })
                    .AddTo(this);

                data.ObserveRemove()
                    .BatchFrame(1, FrameCountType.Update)
                    .Subscribe(items =>
                    {
                        var last = items[items.Count - 1];
                        var first = items[0];
                        if (Mathf.Abs(last.Index - first.Index) + 1 == items.Count)
                        {
                            ChangeItemsCount(ItemCountChangeMode.REMOVE, items.Count, first.Index, false, false);
                        }
                        else
                        {
                            foreach (var item in items)
                            {
                                ChangeItemsCount(ItemCountChangeMode.REMOVE, 1, item.Index, false, false);
                            }
                        }
                    })
                    .AddTo(this);

                // A replace is almost always followed by an addition, so it's not strictly necessary to do a refresh!
                data.ObserveReplace()
                    .Where(change =>
                        !change.OldValue.hasPendingVisualSizeChange && change.NewValue.hasPendingVisualSizeChange)
                    .BatchFrame(1, FrameCountType.Update)
                    .Subscribe(ignored => ThrottledRefresh())
                    .AddTo(this);

                data.ObserveReset()
                    .BatchFrame(1, FrameCountType.Update)
                    .Subscribe(ignored => ResetItems(0))
                    .AddTo(this);
            }

            base.OnInitialized();
        }

        private void ThrottledRefresh()
        {
            Refresh();
        }

        public void ThrottledSmoothScrollToBottom()
        {
            m_ScrollSubject.OnNext(ScrollType.Smooth);
        }

        public void ThrottledScrollToBottom()
        {
            m_ScrollSubject.OnNext(ScrollType.Immediate);
        }

        public void StartTyping(bool self)
        {
            m_SetTyping.OnNext(self ? TypingType.Self : TypingType.NotSelf);
        }

        public void StopTyping()
        {
            m_SetTyping.OnNext(TypingType.NotTyping);
        }

        protected override ChatMessageViewsHolder CreateViewsHolder(int itemIndex)
        {
            var instance = new ChatMessageViewsHolder();
            instance.Init(_Params.itemPrefab, itemIndex);
            return instance;
        }

        protected override void UpdateViewsHolder(ChatMessageViewsHolder newOrRecycled)
        {
            var model = data[newOrRecycled.ItemIndex];
            newOrRecycled.chatMessageView.data = model;
            newOrRecycled.MarkForRebuild();
            ScheduleComputeVisibilityTwinPass(true);
        }

        protected override void OnItemHeightChangedPreTwinPass(ChatMessageViewsHolder viewsHolder)
        {
            base.OnItemHeightChangedPreTwinPass(viewsHolder);

            var model = data[viewsHolder.ItemIndex];
            model.hasPendingVisualSizeChange = false;
            data[viewsHolder.ItemIndex] = model;
            viewsHolder.chatMessageView.data = model;
            viewsHolder.chatMessageView.contentSizeFitter.enabled = false;
        }

        protected override void RebuildLayoutDueToScrollViewSizeChange()
        {
            for (var i = 0; i < data.Count; i++)
            {
                var chatMessageModelView = data[i];
                chatMessageModelView.hasPendingVisualSizeChange = false;
                data[i] = chatMessageModelView;
            }

            base.RebuildLayoutDueToScrollViewSizeChange();
        }

        /// <summary>
        /// Saves the current state of the conversation, returning a key you can later use to restore it.
        /// </summary>
        /// <returns></returns>
        public object SaveState(object id = null)
        {
            id = id ?? (m_StateCache.Count + 1).ToString();
            m_StateCache[id] = data.ToArray();
            return id;
        }

        public void RestoreState(object id)
        {
            data.Clear();
            if (!m_StateCache.ContainsKey(id))
            {
                return;
            }

            foreach (var item in m_StateCache[id])
            {
                data.Add(item);
            }
        }
    }
}