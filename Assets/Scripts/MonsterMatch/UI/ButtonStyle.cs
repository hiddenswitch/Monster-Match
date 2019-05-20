using UnityEngine;
using UnityEngine.UI;

namespace MonsterMatch.UI
{
    /// <summary>
    /// Stores styles for buttons.
    /// </summary>
    public sealed class ButtonStyle : MonoBehaviour
    {
        
        [Header("Selected")] [SerializeField] private SpriteState m_SelectedButtonSprites;
        [Header("Unselected")]
        [SerializeField] private SpriteState m_UnselectedButtonSprites;

        public SpriteState selectedButtonSprites => m_SelectedButtonSprites;

        public SpriteState unselectedButtonSprites => m_UnselectedButtonSprites;
    }
}