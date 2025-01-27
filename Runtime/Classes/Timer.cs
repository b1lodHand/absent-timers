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
    [System.Serializable]
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
        public static Timer Create(float duration, 
            Action onTick = null, 
            Action<TimerCompletionContext> onComplete = null,
            TimerManager manager = null)
        {
            Timer t = TimerManager.Instance.Get();
            t.m_duration = duration;
            t.m_timeLeft = duration;
            t.OnTick = onTick;
            t.OnComplete = onComplete;

            return t;
        }

        [SerializeField] private TimerState m_state = TimerState.NotStarted;
        [SerializeField] private float m_duration = 0;
        [SerializeField] private float m_timeLeft = 0;

#if UNITY_EDITOR
#pragma warning disable CS0414 // Assigned but its value never used
        [SerializeField] private bool m_isExpanded = true;
#pragma warning restore CS0414 // Assigned but its value never used
#endif

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

        internal Timer()
        {
        }

        #region Public API

        public void Restart(bool restartPaused = false)
        {
            m_timeLeft = m_duration;
            m_state = TimerState.Running;

            if (restartPaused) Pause();
        }
        public void Start(bool startPaused = false)
        {
            if (m_state != TimerState.NotStarted)
            {
                Debug.LogError("This timer is already started! If you're trying to restart, use 'Timer.Restart()' function.");
                return;
            }

            m_state = TimerState.Running;

            if (startPaused) Pause();
        }
        public void Pause()
        {
            if (!IsActive)
            {
                Debug.LogError("You can't pause an inactive timer.");
                return;
            }

            if (m_state == TimerState.Running) m_state = TimerState.Paused;
        }
        public void Resume()
        {
            if (!IsActive)
            {
                Debug.LogError("You can't resume an inactive timer.");
                return;
            }

            if (m_state == TimerState.Paused) m_state = TimerState.Running;
        }
        public void Fail()
        {
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
            OnComplete?.Invoke(GenerateCompletionContext());

            TimerManager.Instance.Release(this);
        }
        public void Succeed()
        {
            if (m_state == TimerState.Failed) return;
            if (m_state == TimerState.Succeeded) return;

            m_state = TimerState.Succeeded;

            OnComplete?.Invoke(GenerateCompletionContext());

            TimerManager.Instance.Release(this);
        }
        public void Expand(float amount)
        {
            if (HasCompleted) return;

            m_duration += amount;

            if (HasStarted) m_timeLeft += amount;
            else m_timeLeft = m_duration;
        }
        public void Shrink(float amount)
        {
            if (HasCompleted) return;

            m_duration -= amount;

            if (HasStarted) m_timeLeft -= amount;
            else m_timeLeft = m_duration;
        }

        #endregion

        #region Internal API

        internal void Tick()
        {
            if (!IsActive) return;
            if (IsPaused) return;

            m_timeLeft -= Time.deltaTime;
            OnTick?.Invoke();

            if (m_timeLeft <= 0f)
                Succeed();
        }
        internal void Dispose()
        {
            OnTick = null;
            OnComplete = null;
        }
        internal void ResetProperties()
        {
            m_state = TimerState.NotStarted;
            m_timeLeft = 0f;

            OnTick = null;
            OnComplete = null;
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
