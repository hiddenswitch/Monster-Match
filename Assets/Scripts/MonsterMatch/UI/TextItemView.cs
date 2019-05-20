using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace MonsterMatch.UI
{
    public sealed class TextItemView : UIBehaviour, IHas<string>
    {
        [SerializeField] private Text m_Text;

        public string data
        {
            get => m_Text.text;
            set => m_Text.text = value;
        }
    }
}