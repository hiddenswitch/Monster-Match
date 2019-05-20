using UniRx;
using UniRx.Triggers;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace MonsterMatch.UI
{
    /// <summary>
    /// View for the buttons in the category list.
    /// </summary>
    public sealed class CategoryItemView : UIBehaviour, IHas<CategoryItem>
    {
        [Tooltip("Style for when the button is selected/not selected")] [SerializeField]
        private ButtonStyle m_Style;

        [SerializeField] private Text m_Label;
        [SerializeField] private Button m_Button;

        private CategoryItem m_Data;

        public CategoryItem data
        {
            get { return m_Data; }
            set
            {
                m_Data = value;
                m_Label.text = value.name;
                OnCategoryChanged(CreateProfileController.instance.category.Value);
            }
        }

        protected override void Start()
        {
            base.Start();

            // Whenever the category changes make sure the button is set to the appropriate "selected" sprites
            CreateProfileController.instance.category
                .StartWith(CreateProfileController.instance.category.Value)
                .DelayFrame(1)
                .Where(ignored => m_Style != null)
                .Subscribe(OnCategoryChanged)
                .AddTo(this);

            // Change the category whenever this button is clicked
            m_Button.OnPointerClickAsObservable()
                .Subscribe(ignored => { CreateProfileController.instance.category.Value = data.category; })
                .AddTo(this);
        }

        private void OnCategoryChanged(CategoryEnum category)
        {
            m_Button.spriteState = data?.category == category
                ? m_Style.selectedButtonSprites
                : m_Style.unselectedButtonSprites;

            m_Button.image.sprite = m_Button.spriteState.highlightedSprite;
        }
    }
}