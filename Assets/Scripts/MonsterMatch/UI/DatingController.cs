using System.Linq;
using DG.Tweening;
using HiddenSwitch;
using MaterialUI;
using UniRx;
using UniRx.Diagnostics;
using UniRx.Triggers;
using UnityEngine;
using UnityEngine.EventSystems;

namespace MonsterMatch.UI
{
    public sealed class DatingController : UIBehaviour
    {
        [Tooltip("-1/2*this value = margins for the views relative to the screen in this hierarchy")] [SerializeField]
        private Vector2 m_ViewSizeDelta = new Vector2(-48f, -48f);

        [SerializeField] private Vector3 m_ViewStartingLocalPosition = Vector3.zero;

        [Tooltip(
            "Indicates how many pixels minimum the user must drag from the start of the drag point to count as a real drag.")]
        [SerializeField]
        private float m_MinimumDragThreshold = 30;

        [Tooltip("Indicates how \"left\" the drag must be to count as a left swipe")] [Range(0, 1f)] [SerializeField]
        private float m_LeftinessThreshold = 0.3f;

        [Range(01, 1f)] [SerializeField] private float m_RightinessThreshold = 0.3f;

        [Tooltip("The location that the screen should fall to")] [SerializeField]
        private Vector2 m_FallLocation = new Vector2(-192f, -645f);

        [SerializeField] private float m_FallDuration = 0.4f;
        [SerializeField] private Ease m_FallEase = Ease.OutQuad;

        [Tooltip("Z rotation as a function of the x component of drag / screen width")] [SerializeField]
        private AnimationCurve m_RotationAsFunctionOfDragXOverScreenWidth;

        [Tooltip(
            "The duration of the return animation when the screen is let go outside of the dragging zone or drag threshhold")]
        [SerializeField]
        private float m_ReturnDuration = 0.2f;

        [SerializeField] private Ease m_ReturnEase = Ease.InCubic;

        [Tooltip("The point during the \"return\" clip that we should start fading out")] [SerializeField]
        private float m_RelativeFadeOutReturn = 0.5f;

        [Tooltip("Configures a pool of profile pages to use to facilitate fast swiping")] [SerializeField]
        private DatingProfileView m_DatingProfileViewPrefab;

        [SerializeField] private float m_FadeOutDuration = 0.4f;
        [SerializeField] private Component m_Parent;
        [SerializeField] private RectTransform m_NewMessageModal;
        [SerializeField] private RectTransform m_YouMatchedModal;
        [SerializeField] private ScreenView m_UiScreenView;
        [SerializeField] private MaterialScreen m_MatchingScreen;
        [SerializeField] private OverlayView m_MatchOverlay;
        [SerializeField] private OverlayView m_PassOverlay;

        [Tooltip("Indicates which timer the TimerViews should render against")] [SerializeField]
        private int m_TimerId;

        private readonly DatingProfilePool m_DatingProfilePool = new DatingProfilePool();

        private readonly ReactiveCollection<DatingProfileItem> m_DatingProfiles =
            new ReactiveCollection<DatingProfileItem>();

        private readonly ReactiveCollection<DatingProfileState> m_Judgements =
            new ReactiveCollection<DatingProfileState>();

        private readonly IntReactiveProperty m_CurrentIndex = new IntReactiveProperty(0);
        private DatingProfileView m_CurrentProfileView;
        private DatingProfileView m_StagedProfileView;
        private int m_UnreadMessageCount;
        public IReactiveCollection<DatingProfileItem> datingProfiles => m_DatingProfiles;
        public IReadOnlyReactiveCollection<DatingProfileState> judgements => m_Judgements;
        public IReactiveProperty<int> currentIndex => m_CurrentIndex;
        public bool hasNext => currentIndex.Value + 1 < datingProfiles.Count;
        public DatingProfileView currentProfileView => m_CurrentProfileView;
        public AnimationCurve rotationAsFunctionOfDragXOverScreenWidth => m_RotationAsFunctionOfDragXOverScreenWidth;

        public OverlayView matchOverlay => m_MatchOverlay;

        public OverlayView passOverlay => m_PassOverlay;

        public int unreadMessageCount
        {
            get => m_UnreadMessageCount;
            set
            {
                m_UnreadMessageCount = value;
                var data = currentProfileView.data;
                data.unreadMessageCount = value;
                currentProfileView.data = data;
            }
        }

        protected override void Start()
        {
            base.Start();

            var canDrag = false;

            m_DatingProfilePool.prefab = m_DatingProfileViewPrefab;
            m_DatingProfilePool.parent = m_Parent;
            m_DatingProfilePool.onCreateInstance.AsObservable()
                .Subscribe(instance =>
                {
                    var dragDistance = 0f;
                    var drag = Vector2.zero;
                    Sequence sequence = null;
                    instance.OnBeginDragAsObservable()
                        .Where(ignored => instance == currentProfileView)
                        .Subscribe(ignored =>
                        {
                            sequence?.Kill();
                            dragDistance = 0f;
                            drag = Vector2.zero;
                            canDrag = true;
                        })
                        .AddTo(instance);

                    instance.OnDragAsObservable()
                        .Where(ignored => instance == currentProfileView)
                        .Subscribe(pointer =>
                        {
                            // As it's dragging, change the screen's position and rotation
                            drag = pointer.position - pointer.pressPosition;
                            dragDistance = Mathf.Abs(drag.magnitude);

                            var rotation = rotationAsFunctionOfDragXOverScreenWidth.Evaluate(drag.x / Screen.width);
                            var viewRect = instance.rectTransform;
                            viewRect.localRotation = Quaternion.Euler(0f, 0f, rotation);
                            viewRect.localPosition = drag;

                            if (dragDistance > m_MinimumDragThreshold
                                && canDrag)
                            {
                                if (Vector2.Dot(drag, Vector2.left) > m_LeftinessThreshold)
                                {
                                    matchOverlay.Hide();
                                    passOverlay.Show();
                                    return;
                                }

                                if (Vector2.Dot(drag, Vector2.right) > m_RightinessThreshold)
                                {
                                    passOverlay.Hide();
                                    matchOverlay.Show();
                                    return;
                                }
                            }

                            passOverlay.Hide();
                            matchOverlay.Hide();
                        })
                        .AddTo(instance);

                    // Always disable the drag zones when we're done dragging
                    instance.OnEndDragAsObservable()
                        .Subscribe(ignored => { canDrag = false; })
                        .AddTo(instance);

                    // Return screens that aren't dragged far enough
                    instance
                        .OnEndDragAsObservable()
                        .Where(pointer => dragDistance < m_MinimumDragThreshold)
                        .Subscribe(ignored =>
                        {
                            sequence?.Kill();
                            sequence = DOTween.Sequence();
                            sequence.Append(instance.rectTransform.DOAnchorPos(new Vector2(), m_ReturnDuration)
                                .SetEase(m_ReturnEase));
                            sequence.Insert(0f,
                                instance.rectTransform.DORotateQuaternion(Quaternion.identity, m_ReturnDuration)
                                    .SetEase(m_ReturnEase));
                            sequence.OnComplete(() => sequence = null);
                            sequence.Play();
                        })
                        .AddTo(instance);

                    // Whenever our drag ends in the right place, or we click the accept or reject buttons, issue the
                    // appropriate judgement
                    Observable.Merge(
                            instance
                                .OnEndDragAsObservable()
                                .SelectMany(pointer =>
                                {
                                    passOverlay.Hide();
                                    matchOverlay.Hide();

                                    if (dragDistance < m_MinimumDragThreshold)
                                    {
                                        return new DatingProfileJudgement[0];
                                    }

                                    if (Vector2.Dot(drag, Vector2.left) > m_LeftinessThreshold)
                                    {
                                        return new[] {DatingProfileJudgement.Passed};
                                    }

                                    if (Vector2.Dot(drag, Vector2.right) > m_RightinessThreshold)
                                    {
                                        return new[] {DatingProfileJudgement.Matched};
                                    }

                                    return new DatingProfileJudgement[0];
                                }),
                            instance.rejectButton.OnPointerClickAsObservable()
                                .Select(b => DatingProfileJudgement.Passed),
                            instance.acceptButton.OnPointerClickAsObservable()
                                .Select(b => DatingProfileJudgement.Matched))
                        .Where(ignored => instance == currentProfileView)
                        .Subscribe(judgement =>
                        {
                            m_Judgements.Add(new DatingProfileState()
                            {
                                profileIndex = currentProfileView.data.index,
                                judgement = judgement
                            });

                            var index = currentIndex.Value;
                            currentIndex.Value = Mathf.Min(datingProfiles.Count - 1, index + 1);
                        })
                        .AddTo(instance);
                })
                .AddTo(this);

            m_DatingProfilePool.AddTo(this);

            m_DatingProfilePool
                .PreloadAsync(3, 1)
                .Subscribe(ready => { AfterPreload(); }).AddTo(this);
        }

        private void AfterPreload()
        {
            LayoutProfileViews(m_DatingProfilePool.Rent(), m_DatingProfilePool.Rent());

            datingProfiles.Observe()
                .BatchFrame(1, FrameCountType.Update)
                .SkipWhile(ignored => datingProfiles.Count < 1)
                .Take(1)
                .Subscribe(ignored => SetData())
                .AddTo(this);

            // Whenever the dating profile index changes, fade out the old screen (wherever it is), and make sure the
            // next screen is ready underneath the current screen
            datingProfiles.Observe()
                .Where(ignored => datingProfiles.Count >= 2)
                .BatchFrame(1, FrameCountType.Update)
                .Take(1)
                .ContinueWith(m_CurrentIndex.StartWith(m_CurrentIndex.Value))
                .Subscribe(index =>
                {
                    var oldScreen = m_CurrentProfileView;
                    var sequence = DOTween.Sequence();

                    // Check if we have a judgement for this dating profile
                    if (index - 1 >= 0 && index - 1 < m_Judgements.Count &&
                        m_Judgements[index - 1].judgement == DatingProfileJudgement.Passed)
                    {
                        // Fall instead of fade out
                        sequence.Insert(0f, oldScreen.rectTransform.DOAnchorPos(m_FallLocation, m_FallDuration));
                        sequence.Insert(0f, oldScreen.rectTransform.DORotate(new Vector3(0, 0, 90f), m_FallDuration));
                        sequence.Insert(m_FallDuration * m_RelativeFadeOutReturn,
                            oldScreen.canvasGroup.DOFade(0f, m_FallDuration * (1 - m_RelativeFadeOutReturn)));
                        sequence.SetEase(m_FallEase);
                    }
                    else
                    {
                        // Just fade out if we don't have the data or it was accepted
                        sequence.Append(oldScreen.canvasGroup.DOFade(0f, m_FadeOutDuration));
                    }

                    // Changes the values of what is the current and staged profile views
                    LayoutProfileViews(m_StagedProfileView, m_DatingProfilePool.Rent());

                    SetData();

                    oldScreen.transform.SetAsLastSibling();
                    oldScreen.canvasGroup.blocksRaycasts = false;

                    // Fade out the staged profile if there is no next item
                    m_StagedProfileView.canvasGroup.alpha = hasNext ? 1f : 0f;
                    sequence.OnComplete(() => { m_DatingProfilePool.Return(oldScreen); });
                    sequence.Play();
                })
                .AddTo(this);
        }

        /// <summary>
        /// Shows the "new message" modal overlay, which will prompt users to continue to a chat with a monster
        /// </summary>
        public void ShowNewMessageModal(bool playerStarts = false)
        {
            m_YouMatchedModal.gameObject.SetActive(playerStarts);
            m_NewMessageModal.gameObject.SetActive(!playerStarts);
        }

        private void SetData()
        {
            if (datingProfiles.Count == 0)
            {
                return;
            }

            var datingProfileItem = datingProfiles[currentIndex.Value];
            datingProfileItem.reciprocalMatchCount = currentIndex.Value + 1;
            datingProfileItem.totalProfilesSeen = datingProfiles.Count;
            m_CurrentProfileView.data = datingProfileItem;
            if (hasNext)
            {
                datingProfileItem = datingProfiles[currentIndex.Value + 1];
                datingProfileItem.reciprocalMatchCount = currentIndex.Value + 1;
                datingProfileItem.totalProfilesSeen = datingProfiles.Count;
                m_StagedProfileView.data = datingProfileItem;
            }
        }

        private void LayoutProfileViews(DatingProfileView currentView, DatingProfileView stagedView)
        {
            m_CurrentProfileView = currentView;
            m_StagedProfileView = stagedView;
            foreach (var view in new[] {currentView, stagedView})
            {
                DOTween.Kill(view);
                var rectTransform = view.rectTransform;
                rectTransform.anchorMin = new Vector2(0f, 0f);
                rectTransform.anchorMax = new Vector2(1f, 1f);
                rectTransform.sizeDelta = m_ViewSizeDelta;
                rectTransform.localPosition = m_ViewStartingLocalPosition;
                rectTransform.localRotation = Quaternion.identity;
                view.canvasGroup.alpha = 1f;
            }

            currentView.transform.SetAsLastSibling();
            currentView.canvasGroup.blocksRaycasts = true;
            stagedView.transform.SetAsFirstSibling();
        }

        public void HideNewMessageModal()
        {
            m_YouMatchedModal.gameObject.SetActive(false);
            m_NewMessageModal.gameObject.SetActive(false);
        }
    }
}