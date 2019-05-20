using System;
using System.Linq;
using MaterialUI;
using MonsterMatch.Assets;
using MonsterMatch.UI;
using UniRx;
using UniRx.Triggers;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace MonsterMatch
{
    /// <summary>
    /// Controls the profile sections of the game. Sets up the sprite catalogue, sprite categories and text.
    /// </summary>
    public sealed class CreateProfileController : UIBehaviour, IHas<ProfileCreatorAsset>
    {
        public static CreateProfileController instance { get; private set; }
        [SerializeField] private ProfileCreatorAsset m_Asset;
        [SerializeField] private CategoryItem[] m_Categories;
        [SerializeField] private CategoryPagerView m_CategoryPagerView;

        [Tooltip("Assign the catalogue pager to enable drag and dropping sprites onto the drawing canvas")]
        [SerializeField]
        private CataloguePagerView m_CataloguePagerView;

        [SerializeField] private DrawingView m_EditingProfileTextDrawingView;
        [Header("Screens")] [SerializeField] private ScreenView m_UiScreenView;
        [SerializeField] private MaterialScreen m_EditingProfileTextScreen;
        [Header("Drawing")] [SerializeField] private int m_SnappingStride = 2;
        [SerializeField] private DrawingView m_DrawingView;
        private readonly ReactiveProperty<BrushItem> m_Brush = new ReactiveProperty<BrushItem>();
        public IReactiveProperty<BrushItem> brush => m_Brush;

        [Tooltip(
            "Indicates how much to fade out the preview brush when it is being dragged and not yet over the drawing view")]
        [SerializeField]
        private float m_PreviewBrushTransparency = 0.5f;

        [Tooltip("Set the screen rect to allow the preview brush to render above everything else")] [SerializeField]
        private RectTransform m_ScreenRectTransform;

        [Tooltip("Offsets the brush's dragging position on mobile devices so that it's visible.")] [SerializeField]
        private Vector2 m_OffsetOfBrushPositionOnMobile;

        private readonly BoolReactiveProperty m_CanUndo = new BoolReactiveProperty(false);
        private readonly BehaviorSubject<StampEvent> m_StampEvents = new BehaviorSubject<StampEvent>(null);
        public DrawingView drawingView => m_DrawingView;

        public IObservable<StampEvent> stampEvents => m_StampEvents;

        private readonly ReactiveProperty<CategoryEnum> m_Category = new ReactiveProperty<CategoryEnum>();


        public ReactiveProperty<CategoryEnum> category => m_Category;
        public ProfileCreatorAsset asset => m_Asset;

        public IReactiveProperty<CatalogueColorEnum> color { get; } =
            new ReactiveProperty<CatalogueColorEnum>(CatalogueColorEnum.Blue);

        public IReadOnlyReactiveProperty<bool> canUndo => m_CanUndo;
        public IReactiveProperty<bool> flipped { get; } = new BoolReactiveProperty(false);

        protected override void Awake()
        {
            base.Awake();
            instance = this;
        }

        // Start is called before the first frame update
        protected override void Start()
        {
            m_CategoryPagerView.items.Value = m_Categories;

            var categoryObservable = category as IObservable<CategoryEnum>;
            var isDragAndDropping = new BoolReactiveProperty(false);

            // If we have at least one category, start with it
            if (m_Categories.Length > 0)
            {
                categoryObservable = categoryObservable.StartWith(m_Categories[0].category);
            }

            // Change the catalogue pager whenever the category button is pressed or the color changes.
            Observable.Merge(
                    categoryObservable
                        .DistinctUntilChanged()
                        .AsUnitObservable(),
                    flipped.DistinctUntilChanged()
                        .AsUnitObservable(),
                    color.DistinctUntilChanged()
                        .AsUnitObservable()
                )
                .Subscribe(ignored =>
                {
                    var catalogueItems = m_Asset[category.Value, color.Value]
                        .Select(item => new CatalogueItem()
                        {
                            flipped = flipped.Value,
                            asset = item
                        })
                        .ToArray();

                    m_CataloguePagerView.items.Value = catalogueItems;

                    if (m_Brush.Value != null)
                    {
                        var index = Array.FindIndex(catalogueItems,
                            item => item.asset.name == m_Brush.Value.catalogueItem.asset.name);

                        if (index >= 0)
                        {
                            // If the color or flip state changed, it now changed here
                            var brush = new BrushItem()
                            {
                                catalogueItem = catalogueItems[index]
                            };
                            m_Brush.SetValueAndForceNotify(brush);
                        }
                    }

                    var hit = new EventHitBuilder()
                        .SetEventCategory("profile creator")
                        .SetEventAction("brush")
                        .SetEventLabel($"category={category.Value}, color={color.Value}, flipped={flipped.Value}")
                        .SetEventValue(drawingView.stampItems.Count);
                    GoogleAnalyticsV4.getInstance().LogEvent(hit);
                })
                .AddTo(this);

            category.Value = m_Categories[0].category;

            // Whenever we click on a catalogue item, set that image as the current brush, then stamp its pixels onto
            // the drawing image with our brush when it is clicked
            m_CataloguePagerView.OnCatalogueItemPointerClickAsObservable()
                .Subscribe(tuple =>
                {
                    // If we clicked instead of dragged the item, we're definitely not dragging
                    isDragAndDropping.Value = false;
                    brush.Value = new BrushItem()
                    {
                        catalogueItem = tuple.Item2
                    };
                })
                .AddTo(this);

            // If we switch into the screen view that contains the editing profile text, copy in the image to it
            m_UiScreenView.onScreenBeginTransition.AsObservable()
                .Where(targetScreen => targetScreen == m_EditingProfileTextScreen.screenIndex)
                .Subscribe(ignored => { Drawing.PostDrawing(drawingView, m_EditingProfileTextDrawingView); })
                .AddTo(this);

            // Drawing code
            var opaque = new Color(1f, 1f, 1f, 1f);
            var transparent = new Color(1f, 1f, 1f, m_PreviewBrushTransparency);

            // Draw a preview of the brush
            var previewBrush = new GameObject("Preview Brush");
            var previewImage = previewBrush.AddComponent<Image>();
            // The preview image brush naturally should not receive raycasts / should not block
            previewImage.raycastTarget = false;
            var previewBrushTransform = previewBrush.transform as RectTransform;
            Debug.Assert(previewBrushTransform != null, nameof(previewBrushTransform) + " != null");

            // Changes the preview's sprite as long as the brush is not null. The brush is actually set inactive later.
            // Also reacts to when the flipping setting changes.
            brush
                .Where(b => brush.Value != null && brush.Value.sprite != null)
                .Subscribe(b =>
                {
                    previewImage.sprite = b.sprite;
                    previewImage.rectTransform.localScale =
                        new Vector3(b.flipped ? -1.0f : 1f, 1f, 1f);
                    previewImage.SetNativeSize();
                })
                .AddTo(this);

            // Move the preview brush along with the mouse as long as there is a brush selected
            // Whether or not it will be visible depends on which layer it's on and whether or not we're on a mobile
            // device. Mobile devices do not show the preview when the item was just clicked. The layer is changed later.
            Observable.EveryUpdate()
                .Where(ignored => isActiveAndEnabled)
                .Subscribe(ignored =>
                {
                    if (brush.Value == null
                        || Application.isMobilePlatform && !isDragAndDropping.Value)
                    {
                        previewBrush.SetActive(false);
                        return;
                    }

                    previewBrush.SetActive(true);
                    var pointer = ContinuousStandaloneInputModule.instance.pointerEventData;
                    var isOverDrawingView = pointer.pointerCurrentRaycast.gameObject ==
                                            m_DrawingView.raycastTarget.gameObject;

                    previewImage.color = isDragAndDropping.Value ? isOverDrawingView ? opaque : transparent : opaque;
                    previewBrushTransform.transform.position = BrushPosition(pointer);
                })
                .AddTo(this);


            // When we are drag and dropping, put the preview brush on the root canvas layer so that its totally in
            // the foreground. Otherwise, put it in the preview layer so it only appears when the user's pointer is
            // hovering over the drawing canvas
            isDragAndDropping.StartWith(false)
                .Subscribe(dragging =>
                {
                    previewBrushTransform.SetParent(dragging ? m_ScreenRectTransform : m_DrawingView.previewLayer);
                })
                .AddTo(this);

            // Draw the brush when it is stamped
            var foreground = m_DrawingView.stampLayer;
            m_DrawingView.OnPointerClickAsObservable()
                .Where(ignored => brush.Value != null)
                .Subscribe(pointer => { Stamp(pointer, foreground, brush.Value.flipped); })
                .AddTo(this);

            // Enable drag and dropping a specific sprite. Temporarily switches the brush and activates drag and drop mode
            m_CataloguePagerView
                .OnCatalogueItemBeginDragAsObservable()
                .Subscribe(tuple =>
                {
                    isDragAndDropping.Value = true;
                    brush.Value = new BrushItem()
                    {
                        catalogueItem = tuple.Item2
                    };
                })
                .AddTo(this);

            // Stamp when we stop dragging a catalogue item
            m_CataloguePagerView
                .OnCatalogueItemEndDragAsObservable()
                .Subscribe(tuple =>
                {
                    isDragAndDropping.Value = false;
                    // If we're over the drawing view, we should stamp the sprite
                    var isOverDrawingView = tuple.Item1.pointerCurrentRaycast.gameObject ==
                                            m_DrawingView.raycastTarget.gameObject;
                    if (isOverDrawingView)
                    {
                        Stamp(tuple.Item1, m_DrawingView.stampLayer, tuple.Item2.flipped);
                    }

                    // Then clear the brush no matter what
                    brush.Value = null;
                })
                .AddTo(this);

            // Toggle the flip automatically if this particular asset is configured to do so
            stampEvents.Where(stamp => stamp != null && stamp.brush.flipsAfterStamp)
                .Subscribe(stamp => { flipped.Value = !flipped.Value; })
                .AddTo(this);

            drawingView.stampItems.ObserveCountChanged(true)
                .Subscribe(count => { m_CanUndo.Value = count > 0; })
                .AddTo(this);

            // Save as the user stamps
            stampEvents
                .Subscribe(stampEvent =>
                {
                    // This throttles internally
                    SaveGameController.instance.Save();

                    if (stampEvent != null)
                    {
                        // Analytics
                        var hit = new EventHitBuilder()
                            .SetEventCategory("profile creator")
                            .SetEventAction("stamp")
                            .SetEventLabel(stampEvent.brush.sprite.name)
                            .SetEventValue(drawingView.stampItems.Count);
                        GoogleAnalyticsV4.getInstance().LogEvent(hit);
                    }
                })
                .AddTo(this);
        }

        /// <summary>
        /// Calculates the brush world position given the pointer data and the layer to add the brush to
        /// </summary>
        /// <param name="pointer"></param>
        /// <param name="layer"></param>
        /// <returns></returns>
        private Vector2 BrushPosition(PointerEventData pointer, RectTransform layer = null)
        {
            if (layer == null)
            {
                layer = drawingView.stampLayer;
            }

            var position = pointer.position.Snap(layer.sizeDelta, m_SnappingStride);
            // Allegedly works in https://issuetracker.unity3d.com/issues/application-dot-ismobileplatform-returns-false-on-webgl-projects-launched-on-mobile-browser
            if (Application.isMobilePlatform)
            {
                position += m_OffsetOfBrushPositionOnMobile;
            }

            return position;
        }

        /// <summary>
        /// Stamps the current brush onto the specified layer using the pointer as the location.
        /// </summary>
        /// <param name="pointer"></param>
        /// <param name="layer"></param>
        private void Stamp(PointerEventData pointer, RectTransform layer, bool horizontalFlipped = false)
        {
            // For now, let's just stamp the sprite instead of drawing pixels.
            var stamp = new StampItem
            {
                spriteName = brush.Value.sprite.name,
                localScale = new Vector2(horizontalFlipped ? -1f : 1f, 1f),
                localPosition = layer.InverseTransformPoint(BrushPosition(pointer, layer))
            };

            drawingView.stampItems.Add(stamp);
            m_CanUndo.Value = true;
            m_StampEvents.OnNext(new StampEvent()
            {
                brush = brush.Value,
                layer = layer,
                pointer = pointer,
                stamp = stamp
            });
        }

        /// <summary>
        /// Undoes the last stamping action
        /// </summary>
        public void Undo()
        {
            drawingView.stampItems.RemoveAt(drawingView.stampItems.Count - 1);

            // Analytics
            var hit = new EventHitBuilder()
                .SetEventCategory("profile creator")
                .SetEventAction("undo")
                .SetEventValue(drawingView.stampItems.Count);
            GoogleAnalyticsV4.getInstance().LogEvent(hit);
            m_CanUndo.Value = drawingView.stampItems.Count > 0;
        }

        public void MoveLastStamp(int siblingIndex = 0)
        {
            var transform = drawingView[-1]?.gameObject.transform;
            if (transform == null)
            {
                return;
            }

            transform.SetSiblingIndex(Mathf.Clamp(transform.GetSiblingIndex() + siblingIndex, 0,
                transform.parent.childCount));
        }

        public ProfileCreatorAsset data
        {
            get => m_Asset;
            set => m_Asset = value;
        }
    }
}