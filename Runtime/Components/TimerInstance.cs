using System;
using UnityEngine;
using UnityEngine.Events;

namespace com.absence.timersystem
{
    /// <summary>
    /// A component that represents a timer with UnityEvent based callbacks.
    /// </summary>
    [AddComponentMenu("absencee_/absent-timers/Timer Instance")]
    public class TimerInstance : MonoBehaviour
    {
        [SerializeField] private float m_duration = 0f;
        [SerializeField] private bool m_startOnAwake = true;

        [Space(10)]

        [SerializeField] private UnityEvent m_onStart;
        [SerializeField] private UnityEvent m_onTick;
        [SerializeField] private UnityEvent m_onSuccess;
        [SerializeField] private UnityEvent m_onFail;

        Timer m_timer;

        private void Start()
        {
            m_timer = Timer.Create(m_duration, null, OnTimerComplete);
            m_timer.OnComplete += OnTimerComplete;
            m_timer.OnTick += OnTimerTick;

            if (m_startOnAwake) StartTimer();
        } 

        public void StartTimer()
        {
            if (m_timer != null)
                m_timer.Fail();

            m_onStart?.Invoke();
            m_timer.Start();
        }

        private void OnTimerTick()
        {
            m_onTick?.Invoke();
        }

        private void OnTimerComplete(TimerCompletionContext context)
        {
            m_timer = null;

            if (context.Succeeded)
                m_onSuccess?.Invoke();
            else
                m_onFail?.Invoke();
        }
    }
}
