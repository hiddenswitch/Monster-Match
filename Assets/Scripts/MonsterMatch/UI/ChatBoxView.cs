using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace MonsterMatch.UI
{
    public class ChatBoxView : UIBehaviour, IHas<string>
    {
        [SerializeField] private Text m_Text;
        [SerializeField] private Button m_Button;
        private string m_Data;

        public string data
        {
            get => m_Data;
            set
            {
                m_Data = value;
                // Include one space to prevent this weird breaking bug
                m_Text.text = value?.Trim('"', '\n');
                m_Button.interactable = m_Data != null;
            }
        }

        public Button button => m_Button;
    }
}