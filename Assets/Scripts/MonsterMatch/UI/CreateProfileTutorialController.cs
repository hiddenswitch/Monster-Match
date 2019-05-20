using System;
using UnityEngine;
using UnityEngine.EventSystems;

namespace MonsterMatch.UI
{
    public sealed class CreateProfileTutorialController : UIBehaviour, IPointerClickHandler
    {
        [Tooltip("Prevents users from dismissing the tutorial immediately by tapping through it")] [SerializeField]
        private float m_StartingCooldown = 1f;

        private float m_EnableTime;

        protected override void OnEnable()
        {
            m_EnableTime = Time.time;
        }

        public void OnSequenceComplete()
        {
            gameObject.SetActive(false);
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            if (Time.time < m_EnableTime + m_StartingCooldown)
            {
                return;
            }

            gameObject.SetActive(false);
        }
    }
}