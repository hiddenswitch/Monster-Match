using System.Collections.Generic;
using UniRx;
using UnityEngine;
using UnityEngine.EventSystems;

namespace MonsterMatch
{
    /// <summary>
    /// The player's gender is set to the body sprite specified here
    /// </summary>
    public sealed class SetGenderBasedOnBody : UIBehaviour
    {
        [SerializeField] private PlayerProfileController m_PlayerProfileController;
        [SerializeField] private CreateProfileController m_CreateProfileController;
        [SerializeField] private string m_MaleGender = "male";
        [SerializeField] private string m_FemaleGender = "female";
        [SerializeField] private string[] m_MaleCatalogueItemNames;
        [SerializeField] private string[] m_FemaleCatalogueItemNames;

        protected override void Awake()
        {
            base.Awake();
            var male = new HashSet<string>(m_MaleCatalogueItemNames);
            var female = new HashSet<string>(m_FemaleCatalogueItemNames);
            m_CreateProfileController.stampEvents
                .Where(s => s != null)
                .Subscribe(stampEvent =>
                {
                    var name = stampEvent.brush.catalogueItem.asset.name;
                    if (male.Contains(name))
                    {
                        m_PlayerProfileController.playerGender = m_MaleGender;
                    }
                    else if (female.Contains(name))
                    {
                        m_PlayerProfileController.playerGender = m_FemaleGender;
                    }
                })
                .AddTo(this);
        }
    }
}