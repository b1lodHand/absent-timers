using System;
using UnityEngine;

namespace com.absence.timersystem
{
    /// <summary>
    /// It is the base timer class. Use this to instantiate one:
    /// <code>
    /// Timer.Create();
    /// </code>
    /// </summary>
    public class Timer
    {
        internal Timer() { }

        /// <summary>
        /// The total duration the timer initialized with. Is not dynamic.
        /// </summary>
        public float Duration { get; private set; }

        /// <summary>
        /// Returns true if the timer is initialized for at least once. (Not dependent on state of the timer.)
        /// </summary>
        public bool HasStarted { get; private set; }

        /// <summary>
        /// Returns true if the timer has stopped.
        /// </summary>
        public bool HasEnded { get; private set; }

        /// <summary>
        /// Returns true if the timer is initialized but is not counting right now.
        /// </summary>
        public bool IsPaused => HasStarted && m_isPaused;

        /// <summary>
        /// Returns true if the timer has stopped without completing it's duty due to any reason.
        /// </summary>
        public bool HasFailed => HasEnded && m_hasFailed;

        /// <summary>
        /// Returns true if the timer successfully ended counting.
        /// </summary>
        public bool HasSucceeded => HasEnded && m_hasSucceeded;

        /// <summary>
        /// Returns true if the timer is counting at the moment.
        /// </summary>
        public bool IsActive => HasStarted && !HasEnded;

        public event Action OnTick;
        public event Action OnSuccess;
        public event Action OnFailure;

        private TimerBehaviour m_behaviour;
        private bool m_hasSucceeded = false;
        private bool m_hasFailed = false;
        private bool m_isPaused = false;

        /// <summary>
        /// Use to create a new timer.
        /// </summary>
        /// <param name="duration">The total duration timer will have.</param>
        /// <param name="onTick">Action will get invoked when timer ticks.</param>
        /// <param name="onSuccess">Action will get invoked when timer ends successfully.</param>
        /// <param name="onFail">Action will get invoked when timer fails.</param>
        /// <param name="oneTimeOnly">
        /// If true, you'll be able to use the timer more than once. If false, the timer will
        /// get destroyed right after it ends.
        /// </param>
        /// <returns></returns>
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

        /// <summary>
        /// Rewinds the timer and starts again.
        /// </summary>
        public void Restart()
        {
            HasEnded = false;
            m_hasSucceeded = false;
            m_hasFailed = false;
            m_isPaused = false;

            HasStarted = true;
            m_behaviour.Restart();
        }

        /// <summary>
        /// Pauses the timer at the current state.
        /// </summary>
        public void Pause()
        {
            m_isPaused = true;
        }

        /// <summary>
        /// Resumes the timer.
        /// </summary>
        public void Resume()
        {
            m_isPaused = false;
        }

        /// <summary>
        /// Lets you to quick end a timer. Timer counts as failed.
        /// </summary>
        public void Fail()
        {
            if (HasEnded) return;

            HasEnded = true;
            m_hasFailed = true;

            OnFailure?.Invoke();
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
    }
}
