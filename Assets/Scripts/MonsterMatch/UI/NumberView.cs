using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace MonsterMatch.UI
{
    public sealed class NumberView : UIBehaviour, IHas<int>
    {
        private int m_Data;
        [SerializeField] private string m_TextTemplate = "{0}";
        [SerializeField] private Text m_Text;

        public int data
        {
            get { return m_Data; }
            set
            {
                m_Data = value;
                m_Text.text = string.Format(m_TextTemplate, value);
            }
        }
    }
}