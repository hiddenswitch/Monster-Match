using UniRx;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace MonsterMatch.UI
{
    [RequireComponent(typeof(VerticalLayoutGroup))]
    [ExecuteAlways]
    public sealed class PaddingAdjustment : UIBehaviour
    {
        [SerializeField] private float m_OffsetAtMinimumHeight = -24;
        [SerializeField] private float m_MinimumHeight = 460;
        [SerializeField] private float m_MaximumHeight = 635;
        private int m_LastHeight = 0;
        private VerticalLayoutGroup m_VerticalLayoutGroup;

        protected override void Awake()
        {
            base.Awake();
            m_VerticalLayoutGroup = GetComponent<VerticalLayoutGroup>();
        }


        private void Update()
        {
            var height = Screen.height;
            if (height == m_LastHeight)
            {
                return;
            }

            var padding = m_VerticalLayoutGroup.padding;
            padding.top =
                (int) Mathf.Lerp(0, m_OffsetAtMinimumHeight,
                    Mathf.InverseLerp(m_MaximumHeight, m_MinimumHeight, height));
            m_VerticalLayoutGroup.padding = padding;
            m_LastHeight = height;
        }
    }
}