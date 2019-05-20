using UnityEngine;

namespace MonsterMatch
{
    public static class SnappingExtensions
    {
        public static Vector2 Round(this Vector2 round)
        {
            return new Vector2(Mathf.Round(round.x), Mathf.Round(round.y));
        }

        public static Vector2 Snap(this Vector2 point, int snappingStride = 2)
        {
            return snappingStride * Round(point / snappingStride);
        }

        public static void SetPositionSnapped(this RectTransform rect, Vector2 position, int snappingStride = 2)
        {
            var rectSizeDelta = rect.sizeDelta;
            var destination = position.Snap(rectSizeDelta, snappingStride);
            rect.position = destination;
        }

        public static Vector2 Snap(this Vector2 point, Vector2 rectSizeDelta, int snappingStride = 2)
        {
            point = point.Snap(snappingStride);
            var offsetX = (int) rectSizeDelta.x / snappingStride % snappingStride;
            if (offsetX != 0)
            {
                point.x += (float) offsetX / snappingStride;
            }

            var offsetY = (int) rectSizeDelta.y / snappingStride % snappingStride;
            if (offsetY != 0)
            {
                point.y += (float) offsetY / snappingStride;
            }

            return point;
        }
    }
}