using MaterialUI;
using UniRx;
using UnityEngine;
using UnityEngine.EventSystems;

namespace MonsterMatch.UI
{
    public sealed class DownloadInterstitialScreenView : UIBehaviour
    {
        [SerializeField] private DownloadControllerBase m_Controller;
        [SerializeField] private ProgressIndicator m_DownloadProgressIndicator;
        [SerializeField] private GameObject m_ActivateWhenDownloaded;
        [SerializeField] private GameObject m_DeactivateWhenDownloaded;

        protected override void Start()
        {
            base.Start();

            if (m_Controller.hasResult)
            {
                OnProgress(1f);
            }
            else
            {
                m_Controller.percentComplete
                    .Subscribe(progress => { OnProgress(progress); })
                    .AddTo(this);
            }
        }

        private void OnProgress(float progress)
        {
            m_DownloadProgressIndicator.SetProgress(progress);
            if (m_Controller.hasResult)
            {
                m_DeactivateWhenDownloaded.gameObject.SetActive(false);
                m_ActivateWhenDownloaded.gameObject.SetActive(true);
            }
        }
    }
}