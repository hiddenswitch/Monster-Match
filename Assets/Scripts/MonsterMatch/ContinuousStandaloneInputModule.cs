using UnityEngine;
using UnityEngine.EventSystems;

namespace MonsterMatch
{
    public class ContinuousStandaloneInputModule : StandaloneInputModule
    {
        public static ContinuousStandaloneInputModule instance { get; private set; }

        protected override void Awake()
        {
            base.Awake();
            Debug.Assert(instance == null, $"{nameof(instance)} == null");
            instance = this;
        }

        public PointerEventData pointerEventData
        {
            get
            {
                bool pressed;
                bool released;
                if (input.touchSupported && input.touchCount > 0)
                {
                    return GetTouchPointerEventData(input.GetTouch(0), out pressed, out released);
                }

                return GetLastPointerEventData(kMouseLeftId);
            }
        }
    }
}