using UniRx;
using UniRx.Triggers;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace MonsterMatch.UI
{
    [RequireComponent(typeof(Button))]
    public sealed class UndoButton : UIBehaviour
    {
        [SerializeField] private CreateProfileController m_Controller;

        protected override void Awake()
        {
            var button = GetComponent<Button>();
            m_Controller.canUndo.Subscribe(canUndo => { button.interactable = canUndo; });

            // Undos the last stamp by returning it to the stamp pool. Clears whether or not we can undo.
            button.OnPointerClickAsObservable()
                .Where(ignored => m_Controller.canUndo.Value)
                .Subscribe(ignored => { m_Controller.Undo(); })
                .AddTo(this);
        }
    }
}