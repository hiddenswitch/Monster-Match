using MaterialUI;
using MonsterMatch.Assets;
using UniRx;
using UniRx.Triggers;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace MonsterMatch.UI
{
    [RequireComponent(typeof(Button))]
    public sealed class ChangeColorButton : UIBehaviour
    {
        [SerializeField] private CatalogueColorEnum m_ColorEnum;
        [SerializeField] private CreateProfileController m_Controller;

        protected override void Awake()
        {
            var button = GetComponent<Button>();
            button.OnPointerClickAsObservable()
                .Subscribe(ignored => { m_Controller.color.Value = m_ColorEnum; })
                .AddTo(this);
        }
    }
}