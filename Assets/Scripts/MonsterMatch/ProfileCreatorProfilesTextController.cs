using System.Linq;
using MonsterMatch.UI;
using UnityEngine;
using UnityEngine.EventSystems;

namespace MonsterMatch
{
    public sealed class ProfileCreatorProfilesTextController : UIBehaviour
    {
        [SerializeField] private string[] m_Profiles;
        [SerializeField] private ProfileTextPagerView m_ProfileTextPagerView;

        public string profileText => m_Profiles[m_ProfileTextPagerView.currentPage.Value];

        public int profileTextIndex
        {
            get => m_ProfileTextPagerView.currentPage.Value;
            set => m_ProfileTextPagerView.currentPage.Value = value;
        }

        protected override void Start()
        {
            m_ProfileTextPagerView.items.Value = m_Profiles
                .Select(text => text.FixFancyQuotes())
                .ToArray();
        }
    }
}