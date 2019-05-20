using System.Collections.Generic;
using UniRx.Toolkit;
using UnityEngine;
using UnityEngine.UI;

namespace MonsterMatch
{
    public sealed class StampPool : ObjectPool<Image>
    {
        private static Transform m_PoolParent;

        private static Transform PoolParent
        {
            get
            {
                if (m_PoolParent == null)
                {
                    var gameObject = new GameObject("Stamp Pool");
                    m_PoolParent = gameObject.transform;
                    gameObject.SetActive(false);
                }

                return m_PoolParent;
            }
        }

        protected override Image CreateInstance()
        {
            var stampGameObject = new GameObject($"{Drawing.StampGameObjectName} {Count}");
            var stampImage = stampGameObject.AddComponent<Image>();
            // Stamps should not receive raycasts either
            stampImage.raycastTarget = false;
            stampGameObject.transform.SetParent(PoolParent);
            return stampImage;
        }

        protected override void OnBeforeRent(Image instance)
        {
            base.OnBeforeRent(instance);
        }

        protected override void OnBeforeReturn(Image instance)
        {
            instance.sprite = null;
            var stamp = instance.gameObject;
            Debug.Assert(stamp != null && stamp.name.Contains(Drawing.StampGameObjectName),
                $"{nameof(stamp)} != null && {nameof(stamp)}.name.Contains({Drawing.StampGameObjectName})");
            instance.transform.SetParent(PoolParent);
            base.OnBeforeReturn(instance);
        }
    }
}