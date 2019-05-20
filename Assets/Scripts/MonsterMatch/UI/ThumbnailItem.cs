using UnityEngine;

namespace MonsterMatch.UI
{
    public sealed class ThumbnailItem
    {
        public Sprite sprite { get; set; }
        public bool? matched { get; set; }
        public string label { get; set; }
    }
}