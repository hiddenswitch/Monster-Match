using System;
using MonsterMatch.Assets;
using UniRx;
using UniRx.Triggers;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace MonsterMatch.UI
{
    [RequireComponent(typeof(Button))]
    public sealed class ToggleColorTrayButton : UIBehaviour
    {
        [SerializeField] private RectTransform m_Tray;
        [SerializeField] private Image m_Image;
        [SerializeField] private CreateProfileController m_Controller;
        [SerializeField] private Sprite[] m_Colors;

        [Tooltip(
            "Indicates a rect that when this tray is open, clicking anywhere on this rect will close the tray. This is the equivalent of \"tapping away\" the tray.")]
        [SerializeField]
        private Button m_ClickAwayRect;

        protected override void Awake()
        {
            base.Awake();

            var button = GetComponent<Button>();

            var open = new BoolReactiveProperty(false);

            open.Subscribe(isOpen =>
                {
                    m_Tray.gameObject.SetActive(isOpen);
                    m_ClickAwayRect.gameObject.SetActive(isOpen);
                })
                .AddTo(this);

            button.OnPointerClickAsObservable()
                .Subscribe(ignored => { open.Value = !open.Value; })
                .AddTo(this);

            m_Controller.color.Subscribe(color =>
            {
                var sprite = Array.Find(m_Colors,
                    c => c.name == Enum.GetName(typeof(CatalogueColorEnum), color).ToLower());
                m_Image.sprite = sprite;
                open.Value = false;
            }).AddTo(this);

            m_ClickAwayRect.OnPointerClickAsObservable()
                .Subscribe(ignored => { open.Value = false; })
                .AddTo(this);
        }
    }
}