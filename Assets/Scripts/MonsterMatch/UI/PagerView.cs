using UniRx;
using UniRx.Triggers;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace MonsterMatch.UI
{
    public abstract class PagerView<TItemView, TItem> : UIBehaviour
        where TItemView : MonoBehaviour, IHas<TItem>
    {
        [SerializeField] protected Button m_NextButton;
        [SerializeField] protected Button m_PreviousButton;
        [SerializeField] protected Text m_PageCounter;
        [SerializeField] protected TItemView[] m_ItemViews;
        [SerializeField] protected int m_StartingPage = 0;
        [SerializeField] private GameObject m_ShowIfEmpty;


        protected readonly ReactiveProperty<TItem[]> m_Items = new ReactiveProperty<TItem[]>(new TItem[0]);
        protected readonly IntReactiveProperty m_CurrentPage = new IntReactiveProperty();
        protected virtual bool shouldDelayFrame => false;

        public IReactiveProperty<TItem[]> items
        {
            get => m_Items;
        }

        public IReactiveProperty<int> currentPage
        {
            get => m_CurrentPage;
        }


        protected override void Start()
        {
            currentPage.Value = m_StartingPage;
            if (m_ItemViews.Length == 0)
            {
                return;
            }

            // All the buttons do is increment the page count
            m_NextButton.OnPointerClickAsObservable()
                .Subscribe(ignored => { currentPage.Value = Mathf.Min(currentPage.Value + 1, PageCount() - 1); })
                .AddTo(this);

            m_PreviousButton.OnPointerClickAsObservable()
                .Subscribe(ignored => { currentPage.Value = Mathf.Max(0, currentPage.Value - 1); })
                .AddTo(this);

            SubscribeItemChanges();

            // Ensures that when you change the items, the page will refresh
            var changes = Observable.Merge(
                currentPage.StartWith(m_StartingPage)
                    .DistinctUntilChanged()
                    .AsUnitObservable(),
                items.AsUnitObservable());

            if (shouldDelayFrame)
            {
                changes = changes.DelayFrame(1);
            }

            changes
                .Subscribe(ignored =>
                {
                    // Never allow the page index to exceed the page count. Very unlikely this will occur because the
                    // items observable above will reset current page to zero whenever the items change. 
                    var pageIndex = currentPage.Value;
                    var pageCount = PageCount();
                    if (pageIndex >= pageCount)
                    {
                        // Will be recursive
                        m_CurrentPage.SetValueAndForceNotify(pageCount - 1);
                        return;
                    }

                    m_PageCounter.text = $"{pageIndex + 1}/{pageCount}";

                    for (var viewIndex = 0; viewIndex < m_ItemViews.Length; viewIndex++)
                    {
                        var itemCount = items.Value.Length;
                        var itemIndex = pageIndex * m_ItemViews.Length + viewIndex;
                        var itemView = m_ItemViews[viewIndex];
                        var visible = itemIndex < itemCount && itemCount > 0;
                        itemView.gameObject.SetActive(visible);
                        if (visible)
                        {
                            itemView.data = items.Value[itemIndex];
                        }
                    }
                })
                .AddTo(this);

            if (m_ShowIfEmpty != null)
            {
                items.Subscribe(vals => { m_ShowIfEmpty.gameObject.SetActive(vals.Length == 0); })
                    .AddTo(this);
            }
        }

        protected virtual void ChangesSubscription()
        {
            // Never allow the page index to exceed the page count. Very unlikely this will occur because the
            // items observable above will reset current page to zero whenever the items change. 
            var pageIndex = currentPage.Value;
            var pageCount = PageCount();
            if (pageIndex >= pageCount)
            {
                // Will be recursive
                m_CurrentPage.SetValueAndForceNotify(pageCount - 1);
                return;
            }

            m_PageCounter.text = $"{pageIndex + 1}/{pageCount}";

            for (var viewIndex = 0; viewIndex < m_ItemViews.Length; viewIndex++)
            {
                var itemCount = items.Value.Length;
                var itemIndex = pageIndex * m_ItemViews.Length + viewIndex;
                var itemView = m_ItemViews[viewIndex];
                var visible = itemIndex < itemCount;
                itemView.gameObject.SetActive(visible);
                if (visible)
                {
                    itemView.data = items.Value[itemIndex];
                }
            }
        }

        protected virtual void SubscribeItemChanges()
        {
            // Ensures that when you change the items, the current page resets to zero.
            items.AsUnitObservable()
                .Subscribe(ignored => { currentPage.Value = 0; })
                .AddTo(this);
        }

        private int PageCount()
        {
            return Mathf.CeilToInt((float) items.Value.Length / m_ItemViews.Length);
        }
    }
}