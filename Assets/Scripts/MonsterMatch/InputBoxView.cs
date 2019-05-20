using UnityEngine.EventSystems;

namespace UnityEngine.UI
{
    [RequireComponent(typeof(Button))]
    [ExecuteAlways]
    public class InputBoxView : UIBehaviour
    {
        [SerializeField] private bool m_Interactable;

        [Tooltip("Should omit the button because it handles its own transition")] [SerializeField]
        private GameObject m_NonInteractableShade;

        public bool interactable
        {
            get => m_Interactable;
            set
            {
                m_Interactable = value;
                SetDirty();
            }
        }

        private void SetDirty()
        {
            m_NonInteractableShade.SetActive(!m_Interactable);
        }

#if UNITY_EDITOR
        protected override void OnValidate()
        {
            SetDirty();
        }
#endif
    }
}