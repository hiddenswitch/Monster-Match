using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace MonsterMatch.UI
{
#if UNITY_EDITOR
    [ExecuteInEditMode]
#endif
    public class TextFillForOutline : UIBehaviour
    {
        [FormerlySerializedAs("m_OutlineText")] [SerializeField]
        private Text m_Host;

        [FormerlySerializedAs("m_FillText")] [SerializeField]
        private Text m_Slave;

        private readonly HashSet<Text> m_Registrations = new HashSet<Text>();

        protected override void Awake()
        {
            HandleRegistration();
        }

        private void HandleRegistration()
        {
            if (m_Host == null && m_Registrations.Count == 0)
            {
                return;
            }

            if (m_Host == null || !m_Registrations.Contains(m_Host))
            {
                foreach (var registration in m_Registrations)
                {
                    if (registration != null && !registration.IsDestroyed())
                    {
                        registration.UnregisterDirtyVerticesCallback(OnOutlineDirty);
                    }
                }

                m_Registrations.Clear();
                if (m_Host != null)
                {
                    m_Host.RegisterDirtyVerticesCallback(OnOutlineDirty);
                    m_Registrations.Add(m_Host);
                }
            }
        }

#if UNITY_EDITOR
        protected override void OnValidate()
        {
            base.OnValidate();
            HandleRegistration();
        }
#endif

        private void OnOutlineDirty()
        {
            if (m_Slave != null && m_Host != null)
            {
                m_Slave.text = m_Host.text;
                // TODO: Update other settings here    
            }
        }
    }
}