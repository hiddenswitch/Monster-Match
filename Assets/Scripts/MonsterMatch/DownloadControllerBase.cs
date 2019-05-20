using HiddenSwitch;
using UniRx;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.EventSystems;

namespace MonsterMatch
{
    public abstract class DownloadControllerBase : UIBehaviour
    {
        [SerializeField] protected bool m_StartsDownloadingAutomatically;
        [SerializeField] protected DownloadControllerBase m_NextController;
        [SerializeField] protected AssetReference m_Asset;
        protected FloatReactiveProperty m_PercentComplete = new FloatReactiveProperty(0f);
        public IReadOnlyReactiveProperty<float> percentComplete => m_PercentComplete;
        public virtual bool hasResult => false;

        public abstract void StartDownloading();
    }

    public abstract class DownloadControllerBase<TAssetType> : DownloadControllerBase
        where TAssetType : class
    {
        public virtual IHas<TAssetType> target => null;
        public virtual IHas<TAssetType>[] targets => null;
        public AsyncOperationHandle<TAssetType> operation { get; private set; }
        public TAssetType result => m_BakedAsset ?? (hasResult ? operation.Result : null);

        [SerializeField] protected TAssetType m_BakedAsset;

        public override bool hasResult => m_BakedAsset != null ||
                                          (operation.IsValid() && operation.IsDone && operation.Result != null);

        protected override void Awake()
        {
            base.Awake();
            if (m_StartsDownloadingAutomatically)
            {
                StartDownloading();
            }
        }

        public override void StartDownloading()
        {
            if (hasResult)
            {
                OnCompleted(result);
                return;
            }

            operation = m_Asset.LoadAsset<TAssetType>();
            if (operation.IsDone)
            {
                OnCompleted(operation.Result);
                return;
            }

            operation.OnPercentProgressAsObservable()
                .Subscribe(progress => m_PercentComplete.Value = progress)
                .AddTo(this);
            operation.OnCompletedAsObservable()
                .Subscribe(res => { OnCompleted(res); },
                    err => { Debug.LogError($"DownloadController {typeof(TAssetType)} error: {err.Message}"); });
        }

        private void OnCompleted(TAssetType res)
        {
            m_PercentComplete.Value = 1f;
            if (target != null)
            {
                target.data = res;
            }

            if (targets?.Length > 0)
            {
                foreach (var t in targets)
                {
                    t.data = res;
                }
            }

            if (m_NextController != null)
            {
                m_NextController.StartDownloading();
            }
        }
    }
}