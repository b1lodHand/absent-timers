using System;
using UnityEngine;

namespace com.absence.timersystem
{
    public class Timer
    {
        internal Timer() { }

        public float Duration { get; private set; }

        public bool HasStarted { get; private set; }
        public bool HasEnded { get; private set; }

        public bool IsPaused => HasStarted && m_isPaused;
        public bool HasFailed => HasEnded && m_hasFailed;
        public bool HasSucceeded => HasEnded && m_hasSucceeded;
        public bool IsActive => HasStarted && !HasEnded;

        public event Action OnTick;
        public event Action OnSuccess;
        public event Action OnFailure;

        private TimerBehaviour m_behaviour;
        private bool m_hasSucceeded = false;
        private bool m_hasFailed = false;
        private bool m_isPaused = false;

        public static Timer Create(float duration, Action onTick = null, Action onSuccess = null, Action onFail = null, bool oneTimeOnly = true)
        {
            Timer t = new Timer()
            {
                Duration = duration,
                OnTick = onTick,
                OnSuccess = onSuccess,
                OnFailure = onFail
            };

            TimerBehaviour timerBehaviour = new GameObject("Timer").AddComponent<TimerBehaviour>();
            timerBehaviour.Initialize(t, oneTimeOnly);

            t.m_behaviour = timerBehaviour;

            return t;
        }

        public void Restart()
        {
            HasEnded = false;
            m_hasSucceeded = false;
            m_hasFailed = false;
            m_isPaused = false;

            HasStarted = true;
            m_behaviour.Restart();
        }

        public void Pause()
        {
            m_isPaused = true;
        }

        public void Resume()
        {
            m_isPaused = false;
        }

        internal void Tick()
        {
            OnTick?.Invoke();
        }

        internal void Succeed()
        {
            if (HasEnded) return;

            HasEnded = true;
            m_hasSucceeded = true;

            OnSuccess?.Invoke();
        }

        public void Fail()
        {
            if (HasEnded) return;

            HasEnded = true;
            m_hasFailed = true;

            OnFailure?.Invoke();
        }
    }
}
