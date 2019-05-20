using MaterialUI;
using MonsterMatch.Assets;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Vector2 = UnityEngine.Vector2;

namespace MonsterMatch.UI
{
    public sealed class CatalogueItemView : Graphic, IDragHandler, IBeginDragHandler, IEndDragHandler,
        IHas<CatalogueItem>
    {
        [SerializeField] private Image m_Image;
        [SerializeField] private GameObject m_Frame;
        [SerializeField] private AspectRatioFitter m_AspectRatioFitter;
        [SerializeField] private Button m_Button;
        [SerializeField] private Outline m_Outline;
        private CatalogueItem m_Data;

        public Button button => m_Button;
        public Image image => m_Image;

        public CatalogueItem data
        {
            get => m_Data;
            set
            {
                m_Data = value;
                m_Image.sprite = value.sprite;
                var spriteSize = value.sprite.GetSize();
                var thisSize = rectTransform.GetProperSize();
                var small = spriteSize.x <= thisSize.x || spriteSize.y <= thisSize.y;
                if (!small)
                {
                    m_Image.SetNativeSize();
                }

                m_AspectRatioFitter.aspectMode =
                    small ? AspectRatioFitter.AspectMode.None : AspectRatioFitter.AspectMode.FitInParent;
                m_AspectRatioFitter.aspectRatio = spriteSize.x / spriteSize.y;

                if (small)
                {
                    m_Image.SetNativeSize();
                    var imageTransform = m_Image.rectTransform;
                    var center = new Vector2(0.5f, 0.5f);
                    imageTransform.anchorMin = center;
                    imageTransform.anchorMax = center;
                    imageTransform.anchoredPosition = new Vector2();
                }

                m_Image.rectTransform.localScale = new Vector3(1f * (value.flipped ? -1.0f : 1.0f), 1f, 1f);
                m_Outline.enabled = m_Data.asset.color == CatalogueColorEnum.White;
                m_Frame.SetActive(false);
            }
        }

        public void OnDrag(PointerEventData eventData)
        {
        }

        public void OnBeginDrag(PointerEventData eventData)
        {
        }

        public void OnEndDrag(PointerEventData eventData)
        {
        }
    }
}