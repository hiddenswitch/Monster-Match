using System;
using System.Collections.Generic;
using System.Linq;
using HiddenSwitch;
using MaterialUI;
using MonsterMatch.Assets;
using MonsterMatch.CollaborativeFiltering;
using UniRx;
using UniRx.Diagnostics;
using UniRx.Triggers;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace MonsterMatch.UI
{
    /// <summary>
    /// Controls the story elements while the player is swiping left and right in the matching screens.
    /// </summary>
    public sealed class DatingStoryController : UIBehaviour, IHas<MonsterMatchProfilesAsset>
    {
        [SerializeField] private DatingController m_DatingController;
        [SerializeField] private ScreenView m_ScreenView;
        [SerializeField] private DialogueConversationController m_DialogueConversationController;
        [SerializeField] private ConversationsView m_ConversationsView;

        [Tooltip("Show all the content one after another")] [SerializeField]
        private bool m_LinearAllContent = true;

        [Tooltip("Only applies when Linear All Content is true. Shows only profiles with images.")] [SerializeField]
        private bool m_ImagesOnly = false;

        [Header("Story")] [SerializeField] private DatingInterstitialItem[] m_Interstitials;

        [Tooltip("Shown if the player leaves a chat having successfully gotten a phone number")] [SerializeField]
        private DatingInterstitialItem m_AfterSuccessfulChatInterstitial;

        [Tooltip("The destination of this button is overriden once, when the player has a successful chat.")]
        [SerializeField]
        private ChangeScreenOnPressed m_ExitChatButton;

        [Tooltip("How many chats will the user start with?")] [SerializeField]
        private int m_Chats = 2;

        [Tooltip("For a successful conversation, give the player this many more chats in their chats deck")]
        [SerializeField]
        private int m_BonusChats = 2;

        private Deck m_Deck;

        [SerializeField] private bool m_UseInterstitials = true;

        [Header("Recommender")] [Tooltip("The number of profiles to show randomly")] [SerializeField]
        private int m_ColdStartProfiles = 6;

        [Tooltip("The number of profiles to retrieve from the recommender to be queued up in your profile feed")]
        [Range(1, 10)]
        [SerializeField]
        private int m_QueuedRecommendedProfiles = 2;

        [Tooltip("When true, the most recent judgements (pass/match) have more impact on the C.F. recommender")]
        [SerializeField]
        private bool m_UseRecency = true;

        [Header("B*A^(r*t)")] [SerializeField] private float m_MatchedValueA = 2f;
        [SerializeField] private float m_MatchedValueR = 1.1f;
        [SerializeField] private float m_MatchedValueB = 1f;
        [SerializeField] private float m_PassedValueA = 1.1f;
        [SerializeField] private float m_PassedValueR = 1.1f;
        [SerializeField] private float m_PassedValueB = 0.5f;
        [Header("Profiles")] [SerializeField] private int[] m_AlwaysShowTheseProfilesFirst = new int[0];

        [Tooltip("Never show these profiles")] [SerializeField]
        private int[] m_OmitTheseProfiles = new int[0];

        private MonsterMatchProfilesAsset m_DatingProfileItems;
        private IntReactiveProperty m_Successful = new IntReactiveProperty();
        private IList<ConversationItem> m_Conversations = new List<ConversationItem>();
        private bool m_DidShowChatSuccessInterstitial;
        public ISet<int> seen { get; private set; }
        public IRecommender recommender { get; private set; }

        public void InnerReset()
        {
            m_Deck = new Deck(m_Interstitials.Max(i => i.afterNSwipes) + 1, m_Chats);
            recommender = new CollaborativeFilteringRecommender();
            seen = new HashSet<int>();
        }

        private void OnLoadedProfiles()
        {
            InnerReset();

            // Fix the profile data
            for (var i = 0; i < m_DatingProfileItems.Length; i++)
            {
                var profile = m_DatingProfileItems[i];
                profile.body = profile.body.FixFancyQuotes();
                profile.index = i;
            }

            // Add data to the recommender
            m_DatingController.judgements
                .ObserveAdd()
                .Do(added =>
                {
                    // Your most recent judgements have the biggest impact on your recommendations
                    if (recommender is CollaborativeFilteringRecommender cf)
                    {
                        var t = m_DatingController.judgements.Count;
                        cf.matchedValue = m_MatchedValueB *
                                          (m_UseRecency ? Mathf.Pow(m_MatchedValueA, m_MatchedValueR * t) : 1f);
                        cf.passValue = m_PassedValueB *
                                       (m_UseRecency ? Mathf.Pow(m_PassedValueA, m_PassedValueR * t) : 1f);
                    }

                    // Add the data to the recommender
                    recommender.AddPlayerRating(added.Value.profileIndex,
                        added.Value.judgement == DatingProfileJudgement.Matched);

                    // End here if there's no reciprocal match.
                    // We only draw from the chats deck if we matched 
                    var matched = added.Value.judgement == DatingProfileJudgement.Matched;
                    if (!matched)
                    {
                        // If we didn't match, thin the chat deck of negatives
                        m_Deck.RemoveNegative();
                        return;
                    }

                    var chat = m_DatingProfileItems[added.Value.profileIndex].dialogue?.story != null
                               && m_Deck.Draw();
                    if (!chat)
                    {
                        return;
                    }

                    m_DatingController.unreadMessageCount += 1;
                    var profile = m_DatingProfileItems[added.Value.profileIndex];
                    m_DialogueConversationController.profile.Value = profile;
                    m_DialogueConversationController.selectedDialogue.Value =
                        profile.dialogue;
                    m_Conversations.Add(new ConversationItem()
                    {
                        profile = profile,
                        dialogue = profile.dialogue
                    });
                    m_ConversationsView.items.Value = m_Conversations.ToArray();
                    // Show a modal that you have a new message, and configure the conversation controller.
                    m_DatingController.ShowNewMessageModal(profile.dialogue.playerStarts);
                    m_DialogueConversationController.dialogueEnded
                        .Take(1)
                        .Subscribe(result =>
                        {
                            m_DatingController.HideNewMessageModal();
                            if (result.succeeded)
                            {
                                // For getting a date, the player gets a bonus chat 
                                for (var i = 0; i < m_BonusChats; i++)
                                {
                                    m_Deck.ShuffleIn(true);
                                }

                                // Transition to the chat successful interstitial if the chat was successful.
                                if (!m_DidShowChatSuccessInterstitial && m_UseInterstitials &&
                                    m_AfterSuccessfulChatInterstitial != null)
                                {
                                    m_DidShowChatSuccessInterstitial = true;
                                    var originalDestination = m_ExitChatButton.destination;
                                    m_ExitChatButton.destination = m_AfterSuccessfulChatInterstitial.screen;
                                    m_ExitChatButton.GetComponent<Button>().OnPointerClickAsObservable()
                                        .Take(1)
                                        .DelayFrame(1)
                                        .Subscribe(ignored => { m_ExitChatButton.destination = originalDestination; })
                                        .AddTo(this);
                                }
                            }
                        })
                        .AddTo(this);
                })
                .Subscribe()
                .AddTo(this);


            // Show only ones with art and in order
            if (m_LinearAllContent)
            {
                if (m_ImagesOnly)
                {
                    m_DatingProfileItems.Replace(m_DatingProfileItems.All().Where(e => e.portrait != null).ToArray());
                }

                foreach (var item in m_DatingProfileItems)
                {
                    m_DatingController.datingProfiles.Add(item);
                }

                recommender = new BaselineRecommender(m_DatingProfileItems.All());
                return;
            }

            // After m_ColdStartProfiles - 1, start calculating recommendations.
            // Always omit all the profiles that we don't have data before
            foreach (var i in Enumerable.Range(0, m_DatingProfileItems.Length)
                .Except(MonsterMatchArrayData.ForProfiles))
            {
                seen.Add(i);
            }

            // Also omit any profiles we preconfigured to be omitted
            foreach (var i in m_OmitTheseProfiles)
            {
                seen.Add(i);
            }

            // Show certain profiles first.
            var coldStartProfileIndices = m_AlwaysShowTheseProfilesFirst.Concat(MonsterMatchArrayData
                    .ForProfiles
                    .Shuffled()
                    .Except(seen)
                    .Take(m_ColdStartProfiles))
                .ToArray();

            foreach (var i in coldStartProfileIndices)
            {
                seen.Add(i);
            }

            // Retrieve m_QueuedRecommendedProfiles from the recommender and add them to your queue of profiles
            var recommendedProfiles = m_DatingController.judgements
                    .ObserveAdd()
                    .Where(ignored =>
                        m_DatingController.judgements.Count >= m_ColdStartProfiles - m_QueuedRecommendedProfiles)
                    .SelectMany(ignored =>
                    {
                        recommender.Fit();
                        return recommender.Top()
                            .Except(seen)
                            .ToObservable()
                            .Take(1)
                            .Do(i => seen.Add(i))
                            .Select(i => m_DatingProfileItems[i]);
                    })
#if UNITY_EDITOR
                    .Debug("Recommending")
#endif
                ;


            var coldStartProfiles = coldStartProfileIndices
                .Select(i => m_DatingProfileItems[i])
                .ToObservable();

            // Actually queue the profiles. 
            coldStartProfiles
                .Concat(recommendedProfiles)
                .Subscribe(profile => m_DatingController.datingProfiles.Add(profile))
                .AddTo(this);

            m_Successful.Subscribe(count =>
                {
                    for (var i = 0; i < m_DatingController.datingProfiles.Count; i++)
                    {
                        m_DatingController.datingProfiles[i].reciprocalMatchCount = count;
                    }
                })
                .AddTo(this);

            // Show the interstitials afer the right number of swipes 
            var interstitials = m_Interstitials.ToDictionary(item => item.afterNSwipes, item => item);
            m_DatingController.judgements
                .ObserveCountChanged()
                .Where(count => m_UseInterstitials && interstitials.ContainsKey(count))
                .Subscribe(count =>
                {
                    var item = interstitials[count];
                    m_ScreenView.Transition(item.screen.screenIndex);
                })
                .AddTo(this);
        }

        public MonsterMatchProfilesAsset data
        {
            get => m_DatingProfileItems;
            set
            {
                m_DatingProfileItems = value;
                OnLoadedProfiles();
            }
        }
    }
}