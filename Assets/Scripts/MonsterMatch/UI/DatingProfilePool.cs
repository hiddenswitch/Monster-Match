using System;
using DG.Tweening;
using MaterialUI;
using UniRx.Toolkit;
using UnityEngine;
using UnityEngine.Events;
using Object = UnityEngine.Object;

namespace MonsterMatch.UI
{
    public sealed class DatingProfilePool : ObjectPool<DatingProfileView>
    {
        public sealed class DatingProfileViewCreateEvent : UnityEvent<DatingProfileView>
        {
        }

        private DatingProfileViewCreateEvent m_OnCreateInstance = new DatingProfileViewCreateEvent();

        public DatingProfileViewCreateEvent onCreateInstance => m_OnCreateInstance;

        public Component parent { get; set; }
        public DatingProfileView prefab { get; set; }


        protected override DatingProfileView CreateInstance()
        {
            var instance = Object.Instantiate(prefab, parent.transform, true);
            instance.canvasGroup.alpha = 1f;
            onCreateInstance.InvokeIfNotNull(instance);
            instance.gameObject.SetActive(false);
            return instance;
        }

        protected override void OnBeforeReturn(DatingProfileView instance)
        {
            // Don't deactivate it because it's probably fading out
            base.OnBeforeReturn(instance);
        }

        protected override void OnBeforeRent(DatingProfileView instance)
        {
            // Disable all tweens
            instance.canvasGroup.alpha = 1f;
            DOTween.Kill(instance, false);
            base.OnBeforeRent(instance);
        }
    }
}