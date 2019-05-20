using UnityEngine;

namespace MonsterMatch
{
    public static class SpriteExtensions
    {
        public static Vector2 GetSize(this Sprite sprite)
        {
            return new Vector2(sprite.rect.width, sprite.rect.height) * sprite.pixelsPerUnit;
        }
    }
}