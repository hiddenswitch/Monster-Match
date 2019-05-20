using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Com.TheFallenGames.OSA.Demos.Common;
using UniRx;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Serialization;
using UnityEngine.UI;
using Random = UnityEngine.Random;

namespace MonsterMatch
{
    public sealed class ConversationTest : UIBehaviour
    {
        [SerializeField] private ConversationController m_ConversationController;
        [SerializeField] private ChatBoxController m_ChatBoxController;
        private int m_ResponseCount;

        protected override void Start()
        {
            m_ConversationController.Add(new ChatMessage[]
            {
                new ChatMessage() {message = "Hello!", self = false},
                new ChatMessage() {message = "How's it going?", self = false},
                new ChatMessage() {message = "Great!", self = true},
                new ChatMessage() {message = "So you wanna do it?", self = false},
                new ChatMessage() {message = "Not really", self = true},
                new ChatMessage() {message = "I'm a fish", self = true},
                new ChatMessage() {message = "Hello!", self = false},
                new ChatMessage() {message = "How's it going?", self = false},
                new ChatMessage() {message = "Great!", self = true},
                new ChatMessage() {message = "So you wanna do it?", self = false},
                new ChatMessage() {message = "Not really", self = true},
                new ChatMessage() {message = "I'm a fish", self = true},
                new ChatMessage() {message = "Hello!", self = false},
                new ChatMessage() {message = "How's it going?", self = false},
                new ChatMessage() {message = "Great!", self = true},
                new ChatMessage() {message = "So you wanna do it?", self = false},
                new ChatMessage() {message = "Not really", self = true},
                new ChatMessage() {message = "I'm a fish", self = true},
                new ChatMessage() {message = "Hello!", self = false},
                new ChatMessage() {message = "How's it going?", self = false},
                new ChatMessage() {message = "Great!", self = true},
                new ChatMessage() {message = "So you wanna do it?", self = false},
                new ChatMessage() {message = "Not really", self = true},
                new ChatMessage() {message = "I'm a fish", self = true}
            });

            Observable.Interval(TimeSpan.FromSeconds(2.0))
                .Subscribe(ignored =>
                {
                    m_ConversationController.Add(new[]
                    {
                        new ChatMessage()
                        {
                            message = DemosUtil.GetRandomTextBody(0,
                                UnityEngine.Random.Range(DemosUtil.LOREM_IPSUM.Length / 50 + 1,
                                    DemosUtil.LOREM_IPSUM.Length / 2)),
                            self = false
                        }
                    });
                })
                .AddTo(this);

            Observable.Interval(TimeSpan.FromSeconds(8.0))
                .Subscribe(ignored =>
                {
                    m_ChatBoxController.responses.Clear();
                    AddResponses();
                })
                .AddTo(this);
        }

        private void AddResponses()
        {
            foreach (var response in new[]
            {
                $"test {m_ResponseCount}",
                $"test {m_ResponseCount + 1}",
                $"test {m_ResponseCount + 2}",
                $"test {m_ResponseCount + 3}"
            })
            {
                m_ChatBoxController.responses.Add(response);
            }

            m_ResponseCount += 4;
        }
    }
}