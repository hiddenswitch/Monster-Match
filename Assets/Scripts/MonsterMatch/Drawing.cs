using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using MonsterMatch.UI;
using UnityEngine;
using UnityEngine.UI;

namespace MonsterMatch
{
    [Serializable]
    public sealed class Drawing
    {
        public const string StampGameObjectName = "Stamp";
        [SerializeField] private StampItem[] m_StampItems = new StampItem[0];

        private static ReadOnlyDictionary<string, Sprite> m_Sprites;

        public static IDictionary<string, Sprite> sprites
        {
            get
            {
                if (m_Sprites == null)
                {
                    var sprites = Resources.FindObjectsOfTypeAll<Sprite>();
                    m_Sprites = new ReadOnlyDictionary<string, Sprite>(sprites.ToDictionary(sprite => sprite.name,
                        sprite => sprite));
                }

                return m_Sprites;
            }
        }

        public static void PostDrawing(DrawingView source, DrawingView target)
        {
            target.Clear();

            foreach (var stamp in source.stampItems)
            {
                target.stampItems.Add(stamp);
            }
        }

        public static Drawing FromDrawingView(DrawingView source)
        {
            return new Drawing()
            {
                stampItems = source.stampItems.ToArray()
            };
        }

        public void PostDrawing(DrawingView target)
        {
            target.Clear();
            foreach (var stamp in stampItems)
            {
                target.stampItems.Add(stamp);
            }
        }

        public StampItem[] stampItems
        {
            get => m_StampItems;
            set => m_StampItems = value;
        }
    }
}