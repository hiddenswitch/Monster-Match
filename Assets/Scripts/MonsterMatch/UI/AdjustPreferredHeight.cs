using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace MonsterMatch.UI
{
    public sealed class AdjustPreferredHeight : UIBehaviour, ILayoutElement
    {
        [SerializeField] private float m_OffsetHeight;
        [SerializeField] private Text m_Text;

        public void CalculateLayoutInputHorizontal()
        {
            m_Text?.CalculateLayoutInputHorizontal();
        }

        public void CalculateLayoutInputVertical()
        {
            m_Text?.CalculateLayoutInputVertical();
        }

        public float minWidth => m_Text?.minWidth ?? 0;
        public float preferredWidth => m_Text?.preferredWidth ?? 0;
        public float flexibleWidth => m_Text?.flexibleWidth ?? 0;
        public float minHeight => m_OffsetHeight;
        public float preferredHeight => m_OffsetHeight + m_Text?.preferredHeight ?? 0f;
        public float flexibleHeight => m_Text?.flexibleHeight ?? 0;
        public int layoutPriority => m_Text?.layoutPriority ?? 0;
    }
}