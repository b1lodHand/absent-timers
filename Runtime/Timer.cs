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
        public enum TimerState
        {
            NotStarted = 0,
            Running = 1,
            Paused = 2,
            Succeeded = 3,
            Failed = 4,
        }

        public TimerState State => m_state;

        /// <summary>
        /// The total duration the timer initialized with. Is not dynamic.
        /// </summary>
        public float Duration => m_duration;

        /// <summary>
        /// Use to check if this timer is completed.
        /// </summary>
        public bool HasCompleted => m_state == TimerState.Failed || m_state == TimerState.Succeeded;

        /// <summary>
        /// Use to check if the <see cref="Start"/> function of this timer got called.
        /// </summary>
        public bool HasStarted => m_state != TimerState.NotStarted;

        /// <summary>
        /// Returns true if this timer is started but not completed yet.
        /// </summary>
        public bool IsActive => HasStarted && !HasCompleted;

        /// <summary>
        /// Returns true if this timer is paused.
        /// </summary>
        public bool IsPaused => m_state == TimerState.Paused;
        
        public event Action OnTick = null;
        public event Action<TimerState> OnComplete = null;

        internal TimerBehaviour m_behaviour = null;
        internal TimerState m_state = TimerState.NotStarted;
        internal float m_duration = 0;

        internal Timer()
        {
            TimerBehaviour timerBehaviour = new GameObject("Timer").AddComponent<TimerBehaviour>();
            timerBehaviour.transform.SetParent(TimerManager.Instance.transform);
            timerBehaviour.Initialize(this);

            this.m_behaviour = timerBehaviour;
        }

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
        public static Timer Create(float duration, Action onTick = null, Action<TimerState> onComplete = null)
        {
            Timer t = TimerManager.Instance.Get();
            t.m_duration = duration;
            t.OnTick = onTick;
            t.OnComplete = onComplete;

            return t;
        }

        /// <summary>
        /// Rewinds the timer and starts again.
        /// </summary>
        public void Restart()
        {
            m_state = TimerState.Running;
            m_behaviour.Restart();
        }

        /// <summary>
        /// Starts the timer if it's not started yet.
        /// </summary>
        public void Start()
        {
            if (m_state != TimerState.NotStarted) return;

            m_state = TimerState.Running;
        }

        /// <summary>
        /// Pauses the timer at the current state.
        /// </summary>
        public void Pause()
        {
            if (m_state == TimerState.Running) m_state = TimerState.Paused;
        }

        /// <summary>
        /// Resumes the timer.
        /// </summary>
        public void Resume()
        {
            if (m_state == TimerState.Paused) m_state = TimerState.Running;
        }

        /// <summary>
        /// Lets you to quick end a timer. Timer counts as failed.
        /// </summary>
        public void Fail()
        {
            if (m_state == TimerState.Failed) return;
            if (m_state == TimerState.Succeeded) return;

            m_state = TimerState.Failed;

            OnComplete?.Invoke(m_state);

            TimerManager.Instance.Release(this);
        }

        internal void Tick()
        {
            OnTick?.Invoke();
        }
        internal void Succeed()
        {
            if (m_state == TimerState.Failed) return;
            if (m_state == TimerState.Succeeded) return;

            m_state = TimerState.Succeeded;

            OnComplete?.Invoke(m_state);

            TimerManager.Instance.Release(this);
        }
        internal void Dispose()
        {
            OnTick = null;
            OnComplete = null;

            m_behaviour.Destroy();
            m_behaviour = null;
        }
    }
}
