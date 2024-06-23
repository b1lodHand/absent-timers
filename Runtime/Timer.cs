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
        internal Timer()
        {
            TimerBehaviour timerBehaviour = new GameObject("Timer").AddComponent<TimerBehaviour>();
            timerBehaviour.transform.SetParent(TimerManager.Instance.transform);
            timerBehaviour.Initialize(this);

            this.m_behaviour = timerBehaviour;
        }

        /// <summary>
        /// The total duration the timer initialized with. Is not dynamic.
        /// </summary>
        public float Duration => m_duration;

        /// <summary>
        /// Returns true if the timer is initialized for at least once. (Not dependent on state of the timer.)
        /// </summary>
        public bool HasStarted => m_hasStarted;

        /// <summary>
        /// Returns true if the timer is initialized but is not counting right now.
        /// </summary>
        public bool IsPaused => m_isPaused;

        /// <summary>
        /// Returns true if the timer has stopped.
        /// </summary>
        public bool HasEnded => m_hasEnded;

        /// <summary>
        /// Returns true if the timer has stopped without completing it's duty due to any reason.
        /// </summary>
        public bool HasFailed => m_hasEnded && m_hasFailed;

        /// <summary>
        /// Returns true if the timer successfully ended counting.
        /// </summary>
        public bool HasSucceeded => m_hasEnded && m_hasSucceeded;

        /// <summary>
        /// Returns true if the timer is counting at the moment.
        /// </summary>
        public bool IsActive => m_hasStarted && !m_hasEnded;

        public event Action OnTick = null;
        public event Action OnSuccess = null;
        public event Action OnFailure = null;

        internal TimerBehaviour m_behaviour = null;
        internal bool m_hasSucceeded = false;
        internal bool m_hasFailed = false;
        internal bool m_isPaused = false;
        internal bool m_hasEnded = false;
        internal bool m_hasStarted = false;

        internal float m_duration = 0;

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
        public static Timer Create(float duration, Action onTick = null, Action onSuccess = null, Action onFail = null)
        {
            Timer t = TimerManager.Instance.Get();
            t.m_duration = duration;
            t.OnTick = onTick;
            t.OnSuccess = onSuccess;
            t.OnFailure = onFail;

            return t;
        }

        /// <summary>
        /// Rewinds the timer and starts again.
        /// </summary>
        public void Restart()
        {
            m_hasEnded = false;
            m_hasSucceeded = false;
            m_hasFailed = false;
            m_isPaused = false;

            m_hasStarted = true;
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
            if (m_hasEnded) return;

            m_hasEnded = true;
            m_hasFailed = true;

            OnFailure?.Invoke();

            TimerManager.Instance.Release(this);
        }

        internal void Tick()
        {
            OnTick?.Invoke();
        }

        internal void Succeed()
        {
            if (m_hasEnded) return;

            m_hasEnded = true;
            m_hasSucceeded = true;

            OnSuccess?.Invoke();

            TimerManager.Instance.Release(this);
        }

        internal void Dispose()
        {
            OnTick = null;
            OnFailure = null;
            OnSuccess = null;

            m_behaviour.Destroy();
            m_behaviour = null;
        }
    }
}
