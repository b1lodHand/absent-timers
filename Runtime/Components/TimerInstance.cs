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
        [SerializeField] private UnityEvent<bool> m_onPauseStateChange;
        [SerializeField] private UnityEvent m_onSuccess;
        [SerializeField] private UnityEvent m_onFail;

        ITimer m_timer;

        private void Start()
        {
            if (m_startOnAwake) StartTimer();
        } 

        public void StartTimer()
        {
            if (m_timer != null)
                m_timer.Fail();

            m_timer = Timer.Create(m_duration)
                .OnTick(OnTimerTick)
                .OnComplete(OnTimerComplete)
                .OnPauseResume(OnPauseResume);

            m_timer.onComplete += OnTimerComplete;
            m_timer.onTick += OnTimerTick;

            m_onStart?.Invoke();
        }

        private void OnPauseResume(bool paused)
        {
            m_onPauseStateChange?.Invoke(paused);
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
