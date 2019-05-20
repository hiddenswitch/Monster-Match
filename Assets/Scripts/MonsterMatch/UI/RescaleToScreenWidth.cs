using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace MonsterMatch.UI
{
    public class RescaleToScreenWidth : UIBehaviour
    {
        [SerializeField] private float m_TargetWidth;
        [SerializeField] private RectTransform m_ToRescale;

        protected override void OnEnable()
        {
            var xy = Mathf.Clamp(Screen.width / m_TargetWidth, 0.001f, 1f);
            m_ToRescale.localScale = new Vector3(xy, xy, 1);
        }
    }
}