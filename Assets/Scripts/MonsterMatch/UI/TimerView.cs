using System;
using System.Collections.Generic;
using UniRx;
using UniRx.Diagnostics;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace MonsterMatch.UI
{
    public sealed class TimerView : UIBehaviour
    {
        private static readonly Dictionary<int, DateTime> m_Timers = new Dictionary<int, DateTime>();
        [SerializeField] private int m_TimerIndex;
        [SerializeField] private string m_Template = "mm:ss";
        [SerializeField] private Text m_Text;
        private IDisposable m_Timer;

        public static IDictionary<int, DateTime> timers => m_Timers;

        public static void StartTimer(int id)
        {
            if (!timers.ContainsKey(id))
            {
                timers[id] = DateTime.Now;
            }
        }

        protected override void OnEnable()
        {
            m_Timer?.Dispose();

            m_Timer = Observable.Return(0L)
                .Concat(Observable.Interval(TimeSpan.FromSeconds(0.5f)))
                .Where(t => timers.ContainsKey(m_TimerIndex))
                .Select(t => DateTime.Now - timers[m_TimerIndex])
                .Subscribe(timeSpan => { m_Text.text = timeSpan.ToString(m_Template); })
                .AddTo(this);
        }

        protected override void OnDisable()
        {
            m_Timer?.Dispose();
            m_Timer = null;
        }

        protected override void OnDestroy()
        {
            m_Timer?.Dispose();
            m_Timer = null;
        }
    }
}