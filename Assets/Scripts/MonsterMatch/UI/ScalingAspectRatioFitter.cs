using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace MonsterMatch.UI
{
    [ExecuteAlways]
    [RequireComponent(typeof(RectTransform))]
    [DisallowMultipleComponent]
    public class ScalingAspectRatioFitter : UIBehaviour, ILayoutSelfController, ILayoutController
    {
        [SerializeField] private AspectRatioFitter.AspectMode m_AspectMode = AspectRatioFitter.AspectMode.None;
        [SerializeField] private float m_AspectRatio = 1f;
        private bool m_DelayedSetDirty;
        [NonSerialized] private RectTransform m_Rect;
        private DrivenRectTransformTracker m_Tracker;

        protected ScalingAspectRatioFitter()
        {
        }

        /// <summary>
        ///   <para>The mode to use to enforce the aspect ratio.</para>
        /// </summary>
        public AspectRatioFitter.AspectMode aspectMode
        {
            get { return m_AspectMode; }
            set
            {
                if (!SetPropertyUtility.SetStruct(ref m_AspectMode, value))
                    return;
                SetDirty();
            }
        }

        /// <summary>
        ///   <para>The aspect ratio to enforce. This means width divided by height.</para>
        /// </summary>
        public float aspectRatio
        {
            get { return m_AspectRatio; }
            set
            {
                if (!SetPropertyUtility.SetStruct(ref m_AspectRatio, value))
                    return;
                SetDirty();
            }
        }

        private RectTransform rectTransform
        {
            get
            {
                if (m_Rect == null)
                    m_Rect = GetComponent<RectTransform>();
                return m_Rect;
            }
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            SetDirty();
        }

        /// <summary>
        ///   <para>See MonoBehaviour.OnDisable.</para>
        /// </summary>
        protected override void OnDisable()
        {
            m_Tracker.Clear();
            LayoutRebuilder.MarkLayoutForRebuild(rectTransform);
            base.OnDisable();
        }

        protected virtual void Update()
        {
            if (!m_DelayedSetDirty)
                return;
            m_DelayedSetDirty = false;
            SetDirty();
        }

        protected override void OnRectTransformDimensionsChange()
        {
            UpdateRect();
        }

        private void UpdateRect()
        {
            if (!IsActive())
                return;
            m_Tracker.Clear();
            var localScale = rectTransform.localScale;
            localScale.z = 1f;

            switch (m_AspectMode)
            {
                case AspectRatioFitter.AspectMode.None:
                    if (Application.isPlaying)
                        break;
                    m_AspectRatio = Mathf.Clamp(rectTransform.rect.width / rectTransform.rect.height, 1f / 1000f,
                        1000f);
                    break;
                case AspectRatioFitter.AspectMode.WidthControlsHeight:
                    m_Tracker.Add(this, rectTransform, DrivenTransformProperties.Scale);
                    var targetHeight = rectTransform.rect.width / m_AspectRatio;
                    var scaleY = targetHeight / rectTransform.rect.height;
                    localScale.x = scaleY;
                    localScale.y = scaleY;
                    rectTransform.localScale = localScale;
                    break;
                case AspectRatioFitter.AspectMode.HeightControlsWidth:
                    m_Tracker.Add(this, rectTransform, DrivenTransformProperties.Scale);
                    var targetWidth = rectTransform.rect.height * m_AspectRatio;
                    var scaleX = targetWidth / rectTransform.rect.width;
                    localScale.x = scaleX;
                    localScale.y = scaleX;
                    rectTransform.localScale = localScale;
                    break;
                case AspectRatioFitter.AspectMode.FitInParent:
                case AspectRatioFitter.AspectMode.EnvelopeParent:
                    m_Tracker.Add(this, rectTransform,
                        DrivenTransformProperties.Anchors | DrivenTransformProperties.AnchoredPosition |
                        DrivenTransformProperties.Scale);
                    rectTransform.anchorMin = Vector2.zero;
                    rectTransform.anchorMax = Vector2.one;
                    rectTransform.anchoredPosition = Vector2.zero;
                    Vector2 zero = Vector2.zero;
                    Vector2 parentSize = GetParentSize();
                    if (parentSize.y * (double) aspectRatio < parentSize.x ^
                        m_AspectMode == AspectRatioFitter.AspectMode.FitInParent)
                        zero.y = GetSizeDeltaToProduceSize(parentSize.x / aspectRatio, 1);
                    else
                        zero.x = GetSizeDeltaToProduceSize(parentSize.y * aspectRatio, 0);
                    rectTransform.sizeDelta = zero;
                    break;
            }
        }

        private float GetSizeDeltaToProduceSize(float size, int axis)
        {
            return size - GetParentSize()[axis] * (rectTransform.anchorMax[axis] - rectTransform.anchorMin[axis]);
        }

        private Vector2 GetParentSize()
        {
            RectTransform parent = rectTransform.parent as RectTransform;
            if (!(bool) parent)
                return Vector2.zero;
            return parent.rect.size;
        }

        /// <summary>
        ///   <para>Method called by the layout system.</para>
        /// </summary>
        public virtual void SetLayoutHorizontal()
        {
        }

        /// <summary>
        ///   <para>Method called by the layout system.</para>
        /// </summary>
        public virtual void SetLayoutVertical()
        {
        }

        /// <summary>
        ///   <para>Mark the AspectRatioFitter as dirty.</para>
        /// </summary>
        protected void SetDirty()
        {
            UpdateRect();
        }

#if UNITY_EDITOR
        protected override void OnValidate()
        {
            m_AspectRatio = Mathf.Clamp(m_AspectRatio, 1f / 1000f, 1000f);
            m_DelayedSetDirty = true;
        }
#endif


        /// <summary>
        ///   <para>Specifies a mode to use to enforce an aspect ratio.</para>
        /// </summary>
        public enum AspectMode
        {
            /// <summary>
            ///   <para>The aspect ratio is not enforced.</para>
            /// </summary>
            None,

            /// <summary>
            ///   <para>Changes the height of the rectangle to match the aspect ratio.</para>
            /// </summary>
            WidthControlsHeight,

            /// <summary>
            ///   <para>Changes the width of the rectangle to match the aspect ratio.</para>
            /// </summary>
            HeightControlsWidth,

            /// <summary>
            ///   <para>Sizes the rectangle such that it's fully contained within the parent rectangle.</para>
            /// </summary>
            FitInParent,

            /// <summary>
            ///   <para>Sizes the rectangle such that the parent rectangle is fully contained within.</para>
            /// </summary>
            EnvelopeParent
        }
    }
}