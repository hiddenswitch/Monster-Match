using MaterialUI;
using UniRx;
using UnityEngine;
using UnityEngine.Analytics;
using UnityEngine.EventSystems;

namespace MonsterMatch.Analytics
{
    [RequireComponent(typeof(ScreenView))]
    public sealed class ScreenViewAnalytics : UIBehaviour
    {
        protected override void Awake()
        {
            base.Awake();
            var screenView = GetComponent<ScreenView>();
            screenView.onScreenBeginTransition.AsObservable()
                .Subscribe(next =>
                {
                    var nextScreen = screenView.materialScreen[next];
                    AnalyticsEvent.ScreenVisit(nextScreen.gameObject.name);
                    GoogleAnalyticsV4.getInstance().LogScreen(nextScreen.gameObject.name);
                })
                .AddTo(this);
        }
    }
}