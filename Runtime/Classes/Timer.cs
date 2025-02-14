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
        /// <param name="onComplete">Action will get invoked when timer ends.</param>
        /// <param name="manager">TimerManager to use. Leave empty of you have no use of it. 
        /// It will use the singleton that way.</param>
        /// <returns>Returns the timer created. It won't start automatically. To start it, 
        /// call <see cref="Timer.Start"/>.</returns>
        public static ITimer Create(float duration, TimerManager manager = null)
        {
            if (manager == null) manager = TimerManager.Instance;

            Timer t = manager.Get();
            t.m_duration = duration;
            t.m_time = duration;
            t.onTick = null;
            t.onComplete = null;
            t.onPauseStateChange = null;
            t.m_manager = manager;

            t.Start();
            return t;
        }

        [SerializeField] private TimerState m_state = TimerState.NotStarted;
        [SerializeField] private float m_duration = 0f;
        [SerializeField] private float m_time = 0f;
        [SerializeField] private float m_speedMultiplier = 1f;
        [SerializeField] private bool m_reversed = false;

        TimerManager m_manager;

        public event Action onTick = null;
        public event Action<bool> onPauseStateChange;
        public event Action<TimerCompletionContext> onComplete = null;

        public TimerState State => m_state;

        public float Duration => m_duration;
        /// <summary>
        /// Use to get the amount of time left until this timer reaches to its destination.
        /// </summary>
        public float CurrentTime => m_time;
        public float Speed => m_speedMultiplier;
        public bool HasCompleted => m_state == TimerState.Failed || m_state == TimerState.Succeeded;
        public bool HasStarted => m_state != TimerState.NotStarted;
        public bool IsPaused => m_state == TimerState.Paused;

        public bool IsValid { get; internal set; }
        public bool IsActive => HasStarted && !HasCompleted;
        public bool IsActiveAndRunning => IsActive && !IsPaused;

        internal Timer()
        {
        }

        #region Public API

        /// <summary>
        /// Use to manually tick the timer. Only to use with unmanaged timers.
        /// </summary>
        public void Tick()
        {
            if (!IsValid) return;

            if (!IsActive) return;
            if (IsPaused) return;

            if (m_reversed) m_time += Time.deltaTime * m_speedMultiplier;
            else m_time -= Time.deltaTime * m_speedMultiplier;

            onTick?.Invoke();

            if (m_time <= 0f && !m_reversed)
                Succeed();
            else if (m_time >= m_duration && m_reversed)
                Succeed();
        }
        public void Restart(bool restartPaused = false)
        {
            if (!IsValid) return;

            m_time = m_duration;
            m_state = TimerState.Running;

            if (restartPaused) Pause();
        }
        public ITimer Pause()
        {
            if (!IsValid) return this;

            if (!IsActive)
            {
                Debug.LogError("You can't pause an inactive timer.");
                return this;
            }

            if (m_state == TimerState.Running)
            {
                m_state = TimerState.Paused;
                onPauseStateChange?.Invoke(true);
            }

            return this;
        }
        public void Resume()
        {
            if (!IsValid) return;

            if (!IsActive)
            {
                Debug.LogError("You can't resume an inactive timer.");
                return;
            }

            if (m_state == TimerState.Paused)
            {
                m_state = TimerState.Running;
                onPauseStateChange?.Invoke(false);
            }
        }
        public void Fail()
        {
            if (!IsValid) return;

            if (m_state == TimerState.Failed)
            {
                Debug.LogError("This timer has already failed.");
                return;
            }
            if (m_state == TimerState.Succeeded)
            {
                Debug.LogError("This timer has already succeeded.");
                return;
            }

            m_state = TimerState.Failed;
            onComplete?.Invoke(GenerateCompletionContext());

            if (m_manager != null) m_manager.Release(this);
        }
        public void Succeed()
        {
            if (!IsValid) return;

            if (m_state == TimerState.Failed) return;
            if (m_state == TimerState.Succeeded) return;

            m_state = TimerState.Succeeded;

            onComplete?.Invoke(GenerateCompletionContext());

            if (m_manager != null) m_manager.Release(this);
        }
        public ITimer Expand(float amount)
        {
            if (!IsValid) return this;

            m_duration += amount;

            if (m_reversed)
                return this;

            if (HasStarted) m_time += amount;
            else m_time = m_duration;

            return this;
        }
        public ITimer Shrink(float amount)
        {
            if (!IsValid) return this;

            m_duration -= amount;

            if (m_reversed)
                return this;

            if (HasStarted) m_time -= amount;
            else m_time = m_duration;

            return this;
        }
        public ITimer Reverse()
        {
            if (!IsValid) return this;

            m_reversed = !m_reversed;

            return this;
        }
        public ITimer Flip()
        {
            if (!IsValid) return this;

            m_time = m_duration - m_time;
            Reverse();

            return this;
        }
        public ITimer SetSpeed(float newSpeed)
        {
            if (!IsValid) return this;

            if (newSpeed <= 0f)
                throw new Exception("Speed of a timer cannot be zero or lower! Use Reverse() instead.");

            m_speedMultiplier = newSpeed;

            return this;
        }   
        public ITimer SetDuration(float newDuration)
        {
            if (newDuration <= 0f)
                throw new Exception("Duration of a timer cannot be zero or lower!");

            m_duration = newDuration;
            return this;
        }
        public ITimer OnComplete(Action<TimerCompletionContext> action)
        {
            onComplete += action;
            return this;
        }
        public ITimer OnTick(Action action)
        {
            onTick += action;
            return this;
        }
        public ITimer OnPauseResume(Action<bool> action)
        {
            onPauseStateChange += action;
            return this;
        }

        #endregion

        #region Internal API

        internal void Start(bool startPaused = false)
        {
            if (m_state != TimerState.NotStarted)
            {
                Debug.LogError("This timer is already started! If you're trying to restart, use 'Timer.Restart()' function.");
                return;
            }

            m_state = TimerState.Running;

            if (startPaused) Pause();

            return;
        }
        internal void Dispose()
        {
            ResetProperties();
        }
        internal void ResetProperties()
        {
            m_manager = null;
            m_state = TimerState.NotStarted;
            m_time = 0f;
            m_speedMultiplier = 1f;
            m_reversed = false;

            onTick = null;
            onPauseStateChange = null;
            onComplete = null;
        }

        #endregion

        TimerCompletionContext GenerateCompletionContext()
        {
            return new TimerCompletionContext()
            {
                Succeeded = State == TimerState.Succeeded,
            };
        }
    }
}
