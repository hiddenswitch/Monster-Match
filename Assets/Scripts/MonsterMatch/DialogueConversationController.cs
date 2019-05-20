using System;
using System.Collections.Generic;
using System.Linq;
using HiddenSwitch;
using Ink.Runtime;
using MaterialUI;
using MonsterMatch.Assets;
using MonsterMatch.UI;
using UniRx;
using UniRx.Diagnostics;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Analytics;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Object = System.Object;

namespace MonsterMatch
{
    public sealed class DialogueConversationController : UIBehaviour, IHas<MonsterMatchProfilesAsset>
    {
        private const string success = "success";
        private const string noBubble = "nobubble";
        private const string riddle = "riddle";
        private const string playerName = "name";
        private const string gender = "gender";
        private const string self = "self";
        private const string silent = "silent";
        [SerializeField] private ScreenView m_UiScreenView;
        [SerializeField] private MaterialScreen m_Screen;
        [SerializeField] private ConversationController m_ConversationController;
        [SerializeField] private ChatBoxController m_ChatBoxController;
        [SerializeField] private GameObject m_ExitButton;
        [SerializeField] private float m_SecondsPerCharacterWriting = 0.015f;
        [SerializeField] private float m_SecondsPerCharacterReading = 0.007f;
        [SerializeField] private float m_SecondsPerImageWriting = 0.5f;
        [SerializeField] private float m_SecondsPerImageReading = 3.5f;
        [SerializeField] private string m_StartOfSelfConversation = "^";
        [SerializeField] private Sprite[] m_Sprites;
        [SerializeField] private DatingProfileView m_DatingProfileView;

        [Header("Conversations")] private MonsterMatchProfilesAsset m_Profiles;

        [SerializeField] private int m_DefaultDialogueIndex = -1;

        private readonly ReactiveProperty<DialogueItem> m_SelectedDialogue =
            new ReactiveProperty<DialogueItem>(null);

        public IReactiveProperty<DialogueItem> selectedDialogue => m_SelectedDialogue;
        private readonly Subject<DialogEndedEvent> m_DialogueEnded = new Subject<DialogEndedEvent>();
        public IObservable<DialogEndedEvent> dialogueEnded => m_DialogueEnded;

        private readonly ReactiveProperty<DatingProfileItem> m_Profile = new ReactiveProperty<DatingProfileItem>();
        public IReactiveProperty<DatingProfileItem> profile => m_Profile;

        private void OnNextStory(DialogueItem dialogue, Subject<DialogueEvent> events, Choice choice = null,
            string overrideMessage = null, bool silent = false)
        {
            var story = dialogue.story;
            var self = false;
            var noDelay = false;
            var noBubble = false;
            var success = false;
            if (choice != null)
            {
                var substring = choice.text?.Substring(0, Mathf.Min(choice.text?.Length ?? 0, 8));
                AnalyticsEvent.Custom("choice", new Dictionary<string, object>()
                {
                    {"path", story.state?.currentPathString},
                    {"profile", profile.Value?.name},
                    {"text", substring},
                    {"index", choice.index}
                });
                var eventHitBuilder1 = new EventHitBuilder()
                    .SetEventCategory("conversation")
                    .SetEventAction("choice")
                    .SetEventLabel(choice.text)
                    .SetCustomDimension(0, story.state?.currentPathString)
                    .SetCustomDimension(1, profile.Value?.name);
                GoogleAnalyticsV4.getInstance().LogEvent(eventHitBuilder1);

                story.ChooseChoiceIndex(choice.index);
                self = true;
                noDelay = true;
            }

            var eventHitBuilder2 = new EventHitBuilder()
                .SetEventCategory("conversation")
                .SetEventAction("position")
                .SetEventLabel(story.currentText)
                .SetCustomDimension(0, story.state?.currentPathString)
                .SetCustomDimension(1, profile.Value?.name);
            GoogleAnalyticsV4.getInstance().LogEvent(eventHitBuilder2);

            if (story.canContinue)
            {
                var message = story.Continue();
                success = story.currentTags.Any(s => s.ToLower() == DialogueConversationController.success);
                noBubble = story.currentTags.Any(s => s.ToLower() == DialogueConversationController.noBubble);
                self |= story.currentTags.Any(s => s.ToLower() == DialogueConversationController.self);
                silent |= story.currentTags.Any(s => s.ToLower() == DialogueConversationController.silent);
                if (success)
                {
                    dialogue.succeeded = true;
                }

                // Remove starting and ending quotation marks, extraneous line returns
                message = message.Trim('\n', '"');

                if (message.StartsWith(m_StartOfSelfConversation))
                {
                    self |= true;
                    message = message.Substring(1);
                }

                // If this is the last message of the dialogue, no delay
                if (!story.canContinue && (story.currentChoices?.Count ?? 0) == 0)
                {
                    noDelay = true;
                }

                var dialogueEvent = new DialogueEvent
                {
                    text = overrideMessage ?? message,
                    noBubble = noBubble,
                    choices = story.currentChoices,
                    self = self,
                    noDelay = noDelay,
                    success = success,
                    silent = silent
                };
                dialogue.lastEvent = dialogueEvent;
                events.OnNext(dialogueEvent);
            }
            else
            {
                var dialogueEvent = new DialogueEvent
                {
                    finished = true,
                    success = dialogue.succeeded
                };
                dialogue.lastEvent = dialogueEvent;
                events.OnNext(dialogueEvent);
            }
        }

        protected override void Awake()
        {
            base.Awake();

            Debug.Log($"DialogueConversationController: Loaded {m_Sprites.Length} sprite(s).");

            // When a text phase event is processed from the dialogue system and it didn't have choices, this will post
            // a delayed dialogue continuation to make it look like the other person was typing. Posts the length
            // of the output for the purposes of making an appropriate delay.
            var lastChoice = -1;
            CompositeDisposable subscriptions = null;

            m_ChatBoxController.choices
                .Subscribe(choice => { lastChoice = choice.index; })
                .AddTo(this);

            profile
                .Where(item => item != null)
                .Subscribe(item =>
                {
                    m_DatingProfileView.data = item;
                    m_ConversationController.conversantName = profile.Value.name;
                })
                .AddTo(this);

            selectedDialogue
                .StartWith((DialogueItem) null)
                .Pairwise()
                .Subscribe(dialogues =>
                {
                    var oldDialogue = dialogues.Previous;
                    var newDialogue = dialogues.Current;
                    if (oldDialogue == newDialogue || newDialogue == null)
                    {
                        return;
                    }

                    subscriptions?.Dispose();
                    subscriptions = ResumeStory(newDialogue, oldDialogue);

                    AnalyticsEvent.Custom("chat_with", new Dictionary<string, object>()
                    {
                        {"on_profile", profile.Value?.name}
                    });
                    var eventHitBuilder1 = new EventHitBuilder()
                        .SetEventCategory("conversation")
                        .SetEventAction("chat_with")
                        .SetEventLabel(profile.Value?.name);
                    GoogleAnalyticsV4.getInstance().LogEvent(eventHitBuilder1);
                })
                .AddTo(this);
        }

        /// <summary>
        /// Rigs the views in this screen to start showing the specified dialogue
        /// </summary>
        /// <param name="dialogue"></param>
        /// <returns></returns>
        private CompositeDisposable ResumeStory(DialogueItem dialogue, DialogueItem oldDialogue = null)
        {
            var subscriptions = new CompositeDisposable();
            if (oldDialogue != null)
            {
                m_ConversationController.SaveState(oldDialogue);
            }

            m_ConversationController.RestoreState(dialogue);

            // Disable the exit button
            m_ExitButton.SetActive(false);
            // Make sure the chat box is enabled
            m_ChatBoxController.gameObject.SetActive(true);
            // Tracks all the dialogue events in the story
            var events = new Subject<DialogueEvent>();
            // A subject that's used to delay incoming chats
            var dialogueContinue = new Subject<DialogueEvent>();
            // Get the actual story
            var story = dialogue.story;
            // Gives the player's name and gender a variable
            if (story.variablesState.ContainsDefaultGlobalVariable(playerName))
            {
                story.variablesState[playerName] = PlayerProfileController.instance.playerName;
            }

            if (story.variablesState.ContainsDefaultGlobalVariable(gender))
            {
                story.variablesState[gender] = PlayerProfileController.instance.playerGender;
            }

            if (!story.ContainsExternalBinding(riddle))
            {
                story.BindExternalFunction(riddle,
                    (string riddle) =>
                    {
                        // This prevents the function from causing side effects when one of the story threads is
                        // unparking. For example, during a reset state or going to a specific knot.
                        if (dialogue.unparking)
                        {
                            return;
                        }

                        events.OnNext(new DialogueEvent()
                        {
                            riddle = riddle,
                            noDelay = true
                        });
                    });
            }

            // Start the story when this screen comes into view
            IObservable<int> transition;

            if (m_UiScreenView.currentScreen == m_Screen.screenIndex &&
                m_UiScreenView.screensTransitioning == 0)
            {
                transition = Observable.Return(m_Screen.screenIndex);
            }
            else
            {
                transition = m_UiScreenView.onScreenBeginTransition.AsObservable()
                    .Where(index => index == m_Screen.screenIndex);
            }

            // Triggers the transition to the scene based on the current activity.
            transition.Take(1)
                .Subscribe(ignored => { OnDialogueResume(dialogue, events); })
                .AddTo(subscriptions);

            // When the dialogue ends, fire a dialogue ended event for the current dialogue
            events
                .Where(d => d.finished)
                .Subscribe(ignored => { OnDialogueEnded(dialogue, subscriptions); })
                .AddTo(subscriptions);

            // Show messages and choices from the dialogue system
            events
                .Do(d2 =>
                {
                    if (!d2.noBubble)
                    {
                        m_ConversationController.StartTyping(d2.self);
                    }
                })
                .Where(data => data != null)
                // Include a typing delay
                .SelectMany(data => Observable.Return(data)
                    // Show the typing box right away, before the delay
                    .Delay(TimeSpan.FromSeconds(data.noBubble && dialogue.first || data.noDelay
                        ? 0
                        : (data.text ?? "").StartsWith("!")
                            ? m_SecondsPerImageWriting
                            : (m_SecondsPerCharacterWriting * data.text?.Length ?? 0))))
                .Subscribe(data => { OnDialogueEvent(dialogue, data, events, dialogueContinue); })
                .AddTo(subscriptions);

            // Continue with a choice whenever one is given
            m_ChatBoxController.choices
                .Subscribe(choice => { OnNextStory(dialogue, events, story.currentChoices[choice.index]); })
                .AddTo(subscriptions);

            // Continue after a short delay when a continue is requested without choices
            dialogueContinue
                .Where(ignored => !dialogue.ended)
                .SelectMany(data => Observable.Return(data)
                    .Delay(TimeSpan.FromSeconds(data.text == null
                        ? 0
                        : data.text.StartsWith("!")
                            ? m_SecondsPerImageReading
                            : (m_SecondsPerCharacterReading * data.text.Length))))
                .Subscribe(ignored => { OnNextStory(dialogue, events); })
                .AddTo(subscriptions);
            return subscriptions;
        }

        private void OnDialogueResume(DialogueItem dialogue, Subject<DialogueEvent> events)
        {
            var story = dialogue.story;
            // If there was a previous event, we should re-run it with null continue. This puts the choices back in the
            // right place but doesn't mess with the state of the conversation
            if (dialogue.lastEvent != null)
            {
                // Mark the event as silent, even if it's not, to prevent it from rendering another box
                var wasSilent = dialogue.lastEvent.silent;
                dialogue.lastEvent.silent = true;
                OnDialogueEvent(dialogue, dialogue.lastEvent, events, null);
                dialogue.lastEvent.silent = wasSilent;
            }

            // Advance the story at least once. This way, if the other character talks first, the message will
            // be there. Otherwise, if there are choices after continuing at least once, the chat box will
            // default to open, making conversations where the player starts less confusing.
            if (story.canContinue)
            {
                OnNextStory(dialogue, events);
            }

            if (story.currentChoices?.Count > 0)
            {
                // We open with choices, so pop open the chat box controller for us
                m_ChatBoxController.ShowOptions();
            }
            else
            {
                m_ChatBoxController.HideOptions();
                m_ConversationController.StartTyping(false);
            }
        }

        private void OnDialogueEnded(DialogueItem dialogue, CompositeDisposable subscriptions)
        {
            subscriptions.Dispose();
            dialogue.ended = true;

            m_ChatBoxController.HideOptions();
            m_ConversationController.StopTyping();
            m_DialogueEnded.OnNext(new DialogEndedEvent
            {
                item = selectedDialogue.Value,
                profile = profile.Value,
                succeeded = dialogue.succeeded
            });

            m_ExitButton.SetActive(true);
            m_ChatBoxController.gameObject.SetActive(false);

            AnalyticsEvent.LevelComplete(profile.Value.name, new Dictionary<string, object>()
            {
                {"succeeded", dialogue.succeeded}
            });
            var eventHitBuilder1 = new EventHitBuilder()
                .SetEventCategory("conversation")
                .SetEventAction("ended")
                .SetEventLabel(profile.Value.name)
                .SetEventValue(dialogue.succeeded ? 1L : 0L);
            GoogleAnalyticsV4.getInstance().LogEvent(eventHitBuilder1);
        }

        private void OnDialogueEvent(DialogueItem dialogue, DialogueEvent data, Subject<DialogueEvent> events,
            Subject<DialogueEvent> dialogueContinue)
        {
            // Handle the function call for riddles here. This flag prevents also fake choices used to implement
            // the "Future<int>" that we're doing here from appearing inside the chat box.
            var story = dialogue.story;
            var dataRiddle = data.riddle;
            if (dataRiddle != null)
            {
                dialogue.suppressChoices = true;
                m_ChatBoxController.RequestInput(res =>
                {
#if UNITY_WEBGL
                    // ReSharper disable once SpecifyStringComparison
                    var answerWasCorrect = res.Trim().ToLower() == dataRiddle.Trim().ToLower();
#else
                    var answerWasCorrect = string.Equals(res, riddle, StringComparison.CurrentCultureIgnoreCase);
#endif
                    AnalyticsEvent.Custom("riddle_input", new Dictionary<string, object>()
                    {
                        {"answer", res}
                    });

                    var eventHitBuilder1 = new EventHitBuilder()
                        .SetEventCategory("conversation")
                        .SetEventAction("riddle_input")
                        .SetEventLabel(res);
                    GoogleAnalyticsV4.getInstance().LogEvent(eventHitBuilder1);

                    OnNextStory(dialogue, events,
                        story.currentChoices[answerWasCorrect ? 1 : 0], overrideMessage: res);
                });
            }
            else if (data.text != null && !data.silent)
            {
                var self = data.self;
                var text = data.text;
                // Render images if one was specified
                var spriteName = ParseSpriteName(text);
                // Retrieves all the sprites that have been referenced SOMEWHERE in the scene. See m_Sprites on this
                // instance and add the sprites to make sure the image can render in the chat.
                var sprite = spriteName == null ? null : Drawing.sprites[spriteName];

                m_ConversationController.Add(new[]
                {
                    new ChatMessage
                    {
                        noBubble = data.noBubble,
                        message = spriteName != null ? "" : data.text, image = sprite,
                        self = self
                    }
                });
            }

            // This is definitely no longer the first message.
            dialogue.first = false;

            // Show the choices if there are any to show AND we're not awaiting an input
            if (data.choices?.Count == 0 && dataRiddle == null)
            {
                dialogueContinue?.OnNext(data);
            }
            else if (data.choices?.Count > 0 && dataRiddle == null)
            {
                m_ChatBoxController.responses.Clear();
                if (dialogue.suppressChoices)
                {
                    dialogue.suppressChoices = false;
                }
                else
                {
                    foreach (var choice in data.choices)
                    {
                        m_ChatBoxController.responses.Add(choice.text);
                    }
                }
            }
        }

        /// <summary>
        /// Quickly find a ![ ... ] pattern without bringing in a bajillion dependencies
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        private string ParseSpriteName(string input)
        {
            for (var i = 0; i < input.Length - 2; i++)
            {
                if ((i == 0 || input[i - 1] != '\\')
                    && input[i] == '!'
                    && input[i + 1] == '[')
                {
                    var start = i + 2;
                    var end = -1;
                    for (var j = start + 1; j < input.Length; j++)
                    {
                        if ((input[j - 1] != '\\')
                            && input[j] == ']')
                        {
                            end = j;
                            break;
                        }
                    }

                    if (end == -1)
                    {
                        return null;
                    }

                    var spriteName = input.Substring(start, end - start);
                    return spriteName;
                }
            }

            return null;
        }

        public MonsterMatchProfilesAsset data
        {
            get => m_Profiles;
            set
            {
                m_Profiles = value;
            }
        }
    }
}