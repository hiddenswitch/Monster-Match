using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;

namespace MonsterMatch.UI
{
    public sealed class OverlayView : UIBehaviour
    {
        private enum Status
        {
            IdleShowing,
            IdleHiding,
            Showing,
            Hiding
        }

        [SerializeField] private CanvasGroup m_CanvasGroup;
        [SerializeField] private RectTransform m_Rect;
        [SerializeField] private float m_TransitionDuration = 0.6f;
        [SerializeField] private float m_OffscreenRelativeDistance = 1f;
        [SerializeField] private float m_OnscreenRelativeDistance = 0.1f;
        [SerializeField] private int m_Direction = 1;
        [SerializeField] private Ease m_ShowEase = Ease.InQuart;
        [SerializeField] private Ease m_HideEase = Ease.Linear;
        private Sequence m_Sequence;
        private Status m_Status = Status.IdleHiding;

        private float Remaining(Status desired)
        {
            if (m_Status == Status.IdleHiding && desired == Status.Showing
                || m_Status == Status.IdleShowing && desired == Status.Hiding)
            {
                return m_TransitionDuration;
            }

            if (m_Status == desired)
            {
                return m_TransitionDuration - (m_Sequence?.position ?? 0f);
            }

            if (m_Status != desired)
            {
                return (m_Sequence?.position ?? m_TransitionDuration);
            }

            return m_TransitionDuration;
        }

        /// <summary>
        /// Idempotent
        /// </summary>
        public void Show()
        {
            if (m_Status == Status.Showing
                || m_Status == Status.IdleShowing)
            {
                return;
            }

            var duration = Remaining(Status.Showing);
            var sequence = DOTween.Sequence();
            sequence.Append(m_Rect.DOAnchorPosX(m_OnscreenRelativeDistance * Screen.width * Mathf.Sign(m_Direction),
                duration).SetEase(m_ShowEase));
            sequence.Insert(0f, m_CanvasGroup.DOFade(1f, duration));
            sequence.OnComplete(() => m_Status = Status.IdleShowing);
            m_Sequence?.Kill();
            m_Sequence = sequence;
            m_Status = Status.Showing;
            sequence.Play();
        }

        /// <summary>
        /// Idempotent
        /// </summary>
        public void Hide()
        {
            if (m_Status == Status.Hiding || m_Status == Status.IdleHiding)
            {
                return;
            }

            var duration = Remaining(Status.Hiding);
            var sequence = DOTween.Sequence();
            sequence.Append(m_Rect.DOAnchorPosX(m_OffscreenRelativeDistance * Screen.width * Mathf.Sign(m_Direction),
                duration).SetEase(m_HideEase));
            sequence.Insert(0f, m_CanvasGroup.DOFade(0f, duration));
            sequence.OnComplete(() => m_Status = Status.IdleHiding);
            m_Sequence?.Kill();
            m_Sequence = sequence;
            m_Status = Status.Hiding;
            sequence.Play();
        }

#if UNITY_EDITOR
        protected override void OnValidate()
        {
            if (m_Status == Status.IdleHiding)
            {
                var pos = m_Rect.anchoredPosition;
                pos.x = m_OffscreenRelativeDistance * Screen.width * Mathf.Sign(m_Direction);
                m_Rect.anchoredPosition = pos;
            }
        }
#endif
    }
}