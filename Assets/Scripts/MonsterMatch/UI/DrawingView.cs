using System.Collections.Generic;
using UniRx;
using UniRx.Diagnostics;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace MonsterMatch.UI
{
    public sealed class DrawingView : UIBehaviour
    {
        private const int m_StampPoolInitialSize = 100;
        private static readonly StampPool m_StampPool = new StampPool();
        public static StampPool stampPool => m_StampPool;

        [Tooltip("Indicates the region that will receive raycasts. Should be above everything else!")] [SerializeField]
        private Graphic m_RaycastTarget;

        [FormerlySerializedAs("m_StampForeground")] [SerializeField]
        private RectTransform m_StampLayer;

        [SerializeField] private RectTransform m_PreviewLayer;
        private readonly ReactiveCollection<StampItem> m_StampItems = new ReactiveCollection<StampItem>();
        private readonly Dictionary<StampItem, Image> m_Borrowed = new Dictionary<StampItem, Image>();
        public Graphic raycastTarget => m_RaycastTarget;
        public RectTransform stampLayer => m_StampLayer;
        public RectTransform previewLayer => m_PreviewLayer;
        public ReactiveCollection<StampItem> stampItems => m_StampItems;

        [RuntimeInitializeOnLoadMethod]
        private static void StaticAwake()
        {
            m_StampPool.PreloadAsync(m_StampPoolInitialSize, 1).Subscribe();
        }

        protected override void Awake()
        {
            base.Awake();

            stampItems.ObserveAdd()
                .Where(ignored => isActiveAndEnabled)
                .Subscribe(stamp => { Add(stamp.Value); })
                .AddTo(this);

            stampItems.ObserveRemove()
                .Where(ignored => isActiveAndEnabled)
                .Subscribe(stamp => { Remove(stamp.Value); })
                .AddTo(this);

            stampItems.ObserveReset()
                .Where(ignored => isActiveAndEnabled)
                .Subscribe(ignored => throw new UnityException("Use Clear() instead."))
                .AddTo(this);

            stampItems.ObserveReplace()
                .Where(ignored => isActiveAndEnabled)
                .Subscribe(replaceEvent =>
                {
                    Remove(replaceEvent.OldValue);
                    Add(replaceEvent.NewValue);
                })
                .AddTo(this);
        }

        protected override void OnDisable()
        {
            // Return all the stamps to the stamp pool
            foreach (var borrowed in m_Borrowed)
            {
                stampPool.Return(borrowed.Value);
            }

            m_Borrowed.Clear();
        }

        protected override void OnEnable()
        {
            foreach (var stamp in stampItems)
            {
                if (!m_Borrowed.ContainsKey(stamp))
                {
                    Add(stamp);
                }
            }
        }

        private void Remove(StampItem stamp)
        {
            stampPool.Return(m_Borrowed[stamp]);
            m_Borrowed.Remove(stamp);
        }

        private void Add(StampItem stamp)
        {
            if (!Drawing.sprites.ContainsKey(stamp.spriteName))
            {
                return;
            }

            var image = stampPool.Rent();
            image.sprite = Drawing.sprites[stamp.spriteName];
            var rectTransform = image.rectTransform;
            rectTransform.SetParent(stampLayer);
            rectTransform.localPosition = stamp.localPosition;
            rectTransform.localScale = new Vector3(stamp.localScale.x, stamp.localScale.y, 1f);
            image.SetNativeSize();
            m_Borrowed[stamp] = image;
        }

        public void Clear()
        {
            var targetCount = stampItems.Count;
            for (var i = 0; i < targetCount; i++)
            {
                stampItems.RemoveAt(targetCount - 1 - i);
            }
        }

        public Image this[StampItem item] => m_Borrowed[item];

        public Image this[int index]
        {
            get
            {
                if (m_StampItems.Count == 0)
                {
                    return null;
                }

                if (index < 0)
                {
                    index = m_StampItems.Count - 1;
                }

                return m_Borrowed[m_StampItems[index]];
            }
        }
    }
}