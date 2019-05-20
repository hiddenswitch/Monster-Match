using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace MonsterMatch.UI
{
    public sealed class FormattedText : UIBehaviour, IHas<string>
    {
        [TextArea(3, 10)] [SerializeField] private string m_Template;
        [SerializeField] private string m_DefaultValue;
        [SerializeField] private Text m_Text;
        [SerializeField] private bool m_AllCaps;
        private string m_Data;

        public string data
        {
            get => m_Data;
            set
            {
                m_Data = value;
                SetText();
            }
        }

        private void SetText()
        {
            var value = string.Format(m_Template, string.IsNullOrEmpty(m_Data) ? m_DefaultValue : m_Data);
            if (m_AllCaps)
            {
                value = value.ToUpper();
            }

            m_Text.text = value;
        }

#if UNITY_EDITOR
        protected override void OnValidate()
        {
            SetText();
        }
#endif
    }
}