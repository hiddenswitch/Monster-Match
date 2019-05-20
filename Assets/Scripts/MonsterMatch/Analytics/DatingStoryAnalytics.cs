using System.Collections.Generic;
using Ink.Runtime;
using MonsterMatch.UI;
using UniRx;
using UnityEngine;
using UnityEngine.Analytics;
using UnityEngine.EventSystems;

namespace MonsterMatch.Analytics
{
    [RequireComponent(typeof(DatingController))]
    public sealed class DatingStoryAnalytics : UIBehaviour
    {
        protected override void Awake()
        {
            base.Awake();
            var controller = GetComponent<DatingController>();
            controller.judgements.ObserveAdd()
                .Subscribe(judgement =>
                {
                    var profile = controller.currentProfileView.data;
                    AnalyticsEvent.Custom("swipe", new Dictionary<string, object>()
                    {
                        {
                            "on_profile", profile.name
                        },
                        {
                            "matched", judgement.Value.judgement == DatingProfileJudgement.Matched
                        },
                        {
                            "order", judgement.Index
                        }
                    });
                    var eventHitBuilder = new EventHitBuilder()
                        .SetEventCategory("dating")
                        .SetEventAction("swipe")
                        .SetEventLabel(profile.name)
                        .SetEventValue(judgement.Value.judgement == DatingProfileJudgement.Matched ? 1L : 0L)
                        .SetCustomMetric(0, judgement.Index);
                    GoogleAnalyticsV4.getInstance().LogEvent(eventHitBuilder);
                })
                .AddTo(this);
        }
    }
}