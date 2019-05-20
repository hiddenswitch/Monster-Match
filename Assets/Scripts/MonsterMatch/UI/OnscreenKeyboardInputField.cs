using UniRx;
using UniRx.Diagnostics;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace MonsterMatch.UI
{
    public class OnscreenKeyboardInputField : InputField
    {
        [SerializeField] private UIKeyboard m_OnscreenKeyboard;
        private bool m_SelectionChangeRequested = false;

        protected override void Start()
        {
            base.Start();
            var keyboardLayer = LayerMask.NameToLayer("Keyboard");
            // If the last selected object isn't a keyboard button, deselect
            Observable.EveryUpdate()
                .Where(ignored => m_SelectionChangeRequested)
                .Select(ignored => ContinuousStandaloneInputModule.instance.pointerEventData.pointerPress)
                .Where(press => press != null)
                .Where(press => press.layer != keyboardLayer && press.gameObject != gameObject)
                .Subscribe(ignored =>
                {
                    m_SelectionChangeRequested = false;
                    DeactivateInputField();
                })
                .AddTo(this);
        }

        public override void OnDeselect(BaseEventData eventData)
        {
            m_SelectionChangeRequested = true;
        }

        /*
        public override void OnUpdateSelected(BaseEventData eventData)
        {
            
            if (eventData.selectedObject != gameObject)
            {
                m_SelectionChangeRequested = true;
            }
        }
*/
        public void PublicAppend(string s)
        {
            Append(s);
        }
    }
}