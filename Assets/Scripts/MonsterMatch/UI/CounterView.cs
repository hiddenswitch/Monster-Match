using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace MonsterMatch.UI
{
    public sealed class CounterView : UIBehaviour, IHas<CountItem>
    {
        [SerializeField] private string m_TextTemplate = "{0}/{1} Matches";
        [SerializeField] private Text m_Text;
        private CountItem m_Data;

        public CountItem data
        {
            get => m_Data;
            set
            {
                m_Data = value;
                m_Text.text = string.Format(m_TextTemplate, value.numerator, value.denominator);
            }
        }
    }
}