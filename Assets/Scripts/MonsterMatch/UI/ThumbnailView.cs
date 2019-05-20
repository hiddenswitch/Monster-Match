using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace MonsterMatch.UI
{
    public sealed class ThumbnailView : UIBehaviour, IHas<ThumbnailItem>
    {
        [SerializeField] private Image m_ThumbnailImage;
        [SerializeField] private Image m_MatchOrPassImage;
        [SerializeField] private Sprite m_Matched;
        [SerializeField] private Sprite m_Passed;
        [SerializeField] private Text m_Label;
        private ThumbnailItem m_Data;

        public ThumbnailItem data
        {
            get => m_Data;
            set
            {
                m_Data = value;
                if (value == null)
                {
                    return;
                }

                m_ThumbnailImage.sprite = value.sprite;
                m_MatchOrPassImage.enabled = value.matched != null;
                m_MatchOrPassImage.sprite = value.matched ?? false ? m_Matched : m_Passed;
                m_MatchOrPassImage.SetNativeSize();
                m_Label.enabled = value.label != null;
                m_Label.text = value.label ?? "";
            }
        }
    }
}