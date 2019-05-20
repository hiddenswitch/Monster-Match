using UniRx;
using UniRx.Triggers;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace MonsterMatch
{
    [RequireComponent(typeof(Button))]
    public class NavigateToWebPage : UIBehaviour
    {
        [SerializeField] private string m_Url;
        [SerializeField] private GameObject m_LoadingOverlay;

        protected override void Start()
        {
            base.Start();
            var button = GetComponent<Button>();
            button.OnPointerClickAsObservable()
                .Subscribe(ignored =>
                {
                    Application.OpenURL(m_Url);
                    button.interactable = false;
                    m_LoadingOverlay?.SetActive(true);
                })
                .AddTo(this);
        }
    }
}