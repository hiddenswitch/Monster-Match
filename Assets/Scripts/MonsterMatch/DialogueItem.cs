using System;
using Ink.Runtime;
using UnityEngine;

namespace MonsterMatch
{
    [Serializable]
    public class DialogueItem
    {
        [SerializeField] private TextAsset m_InkConversation;
        [SerializeField] private bool m_PlayerStarts;
        private Story m_Story;

        public Story story
        {
            get
            {
                if (m_Story == null)
                {
                    m_Story = m_InkConversation != null ? new Story(m_InkConversation.text) : null;
                }

                return m_Story;
            }
        }

        public bool playerStarts => m_PlayerStarts;

        /// <summary>
        /// Tracks whether the story has ended at any point
        /// </summary>
        public bool ended { get; set; }

        /// <summary>
        /// Tracks whether this is the first message
        /// </summary>
        public bool first { get; set; } = true;

        /// <summary>
        /// Suppresses the next choices if they are specified. Usually follows a function like riddle
        /// </summary>
        public bool suppressChoices { get; set; } = false;

        public bool unparking { get; set; } = false;

        /// <summary>
        /// Reset the state of whether or not the player succeeded in getting a date in this story.
        /// </summary>
        public bool succeeded { get; set; } = false;
        
        public DialogueEvent lastEvent { get; set; }

        public void Reset()
        {
            ended = false;
            first = true;
            suppressChoices = false;
            unparking = false;
            succeeded = false;
            story.ResetState();
        }
    }
}