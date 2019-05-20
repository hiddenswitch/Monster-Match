using UnityEngine;

namespace HiddenSwitch
{
    public class SixtyFPS : MonoBehaviour
    {
        void Awake()
        {
#if UNITY_WEBGL
            Application.targetFrameRate = -1;
#else
            Application.targetFrameRate = 60;
#endif
            Debug.Log($"Awake: Framerate set to {Application.targetFrameRate}");
        }
    }
}