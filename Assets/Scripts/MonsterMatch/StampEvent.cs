using UnityEngine;
using UnityEngine.EventSystems;

namespace MonsterMatch
{
    public sealed class StampEvent
    {
        public BrushItem brush { get; set; }
        public PointerEventData pointer { get; set; }
        public RectTransform layer { get; set; }
        public StampItem stamp { get; set; }
    }
}