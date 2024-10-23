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
        [SerializeField] private UnityEvent m_onSuccess;
        [SerializeField] private UnityEvent m_onFail;

        private void Start()
        {
            var t = Timer.Create(m_duration, null, OnTimerComplete);

            if (m_startOnAwake) t.Restart();
        }

        private void OnTimerComplete(Timer.TimerState state)
        {
            switch (state)
            {
                case Timer.TimerState.Succeeded:
                    m_onSuccess?.Invoke();
                    break;

                default:
                    m_onFail?.Invoke();
                    break;
            }
        }
    }
}
