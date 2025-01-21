using com.absence.timersystem.internals;
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
    public class Timer : ITimer, ITimer2
    {
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
        public static Timer Create(float duration, Action onTick = null, Action<TimerCompletionContext> onComplete = null)
        {
            Timer t = TimerManager.Instance.Get();
            t.m_duration = duration;
            t.m_timeLeft = duration;
            t.OnTick = onTick;
            t.OnComplete = onComplete;

            return t;
        }

        internal Timer()
        {
        }

        [SerializeField] private TimerState m_state = TimerState.NotStarted;
        [SerializeField] private float m_duration = 0;
        [SerializeField] private float m_timeLeft = 0;

        public event Action OnTick = null;
        public event Action<TimerCompletionContext> OnComplete = null;

        public TimerState State => m_state;

        public float Duration => m_duration;
        /// <summary>
        /// Use to get the amount of time left until this timer reaches 0f.
        /// </summary>
        public float CurrentTime => m_timeLeft;
        public bool HasCompleted => m_state == TimerState.Failed || m_state == TimerState.Succeeded;
        public bool HasStarted => m_state != TimerState.NotStarted;
        public bool IsActive => HasStarted && !HasCompleted;
        public bool IsPaused => m_state == TimerState.Paused;

        #region Public API

        public void Restart()
        {
            m_state = TimerState.Running;
        }
        public void Start()
        {
            if (m_state != TimerState.NotStarted) return;

            m_state = TimerState.Running;
        }
        public void Pause()
        {
            if (m_state == TimerState.Running) m_state = TimerState.Paused;
        }
        public void Resume()
        {
            if (m_state == TimerState.Paused) m_state = TimerState.Running;
        }
        public void Fail()
        {
            if (m_state == TimerState.Failed) return;
            if (m_state == TimerState.Succeeded) return;

            m_state = TimerState.Failed;

            OnComplete?.Invoke(GenerateCompletionContext());

            TimerManager.Instance.Release(this);
        }
        public void Expand(float amount)
        {
            if (!IsActive) return;

            m_duration += amount;
            m_timeLeft += amount;
        }
        public void Shrink(float amount)
        {
            if (!IsActive) return;

            m_duration -= amount;
            m_timeLeft -= amount;
        }

        #endregion

        #region Internal API

        public void Succeed()
        {
            if (m_state == TimerState.Failed) return;
            if (m_state == TimerState.Succeeded) return;

            m_state = TimerState.Succeeded;

            OnComplete?.Invoke(GenerateCompletionContext());

            TimerManager.Instance.Release(this);
        }
        public void Tick()
        {
            if (!IsActive) return;
            if (IsPaused) return;

            m_timeLeft -= Time.deltaTime;
            OnTick?.Invoke();

            if (m_timeLeft <= 0f)
                Succeed();
        }
        public void Dispose()
        {
            OnTick = null;
            OnComplete = null;
        }
        public void ResetProperties()
        {
            m_state = TimerState.NotStarted;
            m_timeLeft = 0f;

            OnTick = null;
            OnComplete = null;
        }
        public TimerCompletionContext GenerateCompletionContext()
        {
            return new TimerCompletionContext()
            {
                Succeeded = State == TimerState.Succeeded,
            };
        }

        #endregion
    }
}
