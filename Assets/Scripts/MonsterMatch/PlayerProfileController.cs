using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace MonsterMatch
{
    public class PlayerProfileController : UIBehaviour
    {
        [SerializeField] private string m_PlayerName = "Nameless Witch";
        [SerializeField] private string m_PlayerGender = "Male";

        public string playerName
        {
            get => m_PlayerName;
            set => m_PlayerName = value;
        }

        public string playerGender
        {
            get => m_PlayerGender.ToLower();
            set => m_PlayerGender = value;
        }

        public static PlayerProfileController instance { get; private set; }

        protected override void Awake()
        {
            base.Awake();
            instance = this;
        }
    }
}