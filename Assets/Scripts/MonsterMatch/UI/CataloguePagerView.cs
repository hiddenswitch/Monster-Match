using System;
using System.Linq;
using UniRx;
using UniRx.Triggers;
using UnityEngine;
using UnityEngine.EventSystems;

namespace MonsterMatch.UI
{
    public sealed class CataloguePagerView : PagerView<CatalogueItemView, CatalogueItem>
    {
        protected override bool shouldDelayFrame => false;

        public IObservable<Tuple<PointerEventData, CatalogueItem>> OnCatalogueItemPointerClickAsObservable()
        {
            return m_ItemViews.Select(view => view.OnPointerClickAsObservable().Select(eventData =>
                new Tuple<PointerEventData, CatalogueItem>(eventData, view.data))).Merge();
        }

        public IObservable<Tuple<PointerEventData, CatalogueItem>> OnCatalogueItemDragAsObservable()
        {
            return m_ItemViews.Select(view => view.OnDragAsObservable().Select(eventData =>
                new Tuple<PointerEventData, CatalogueItem>(eventData, view.data))).Merge();
        }

        public IObservable<Tuple<PointerEventData, CatalogueItem>> OnCatalogueItemBeginDragAsObservable()
        {
            return m_ItemViews.Select(view => view.OnBeginDragAsObservable().Select(eventData =>
                new Tuple<PointerEventData, CatalogueItem>(eventData, view.data))).Merge();
        }

        public IObservable<Tuple<PointerEventData, CatalogueItem>> OnCatalogueItemEndDragAsObservable()
        {
            return m_ItemViews.Select(view => view.OnEndDragAsObservable().Select(eventData =>
                new Tuple<PointerEventData, CatalogueItem>(eventData, view.data))).Merge();
        }

        protected override void SubscribeItemChanges()
        {
            items
                .SubscribeWithState(new CategoryEnum?[] {null}, (theseItems, lastCategory) =>
                {
                    if (theseItems.Length == 0)
                    {
                        currentPage.Value = 0;
                        return;
                    }

                    var thisCategory = theseItems[0].category;
                    if (thisCategory != lastCategory[0])
                    {
                        currentPage.Value = 0;
                    }

                    lastCategory[0] = thisCategory;
                })
                .AddTo(this);
        }
    }
}