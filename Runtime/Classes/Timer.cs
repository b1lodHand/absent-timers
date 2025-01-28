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
            t.m_time = duration;
            t.OnTick = onTick;
            t.OnComplete = onComplete;

            return t;
        }

        [SerializeField] private TimerState m_state = TimerState.NotStarted;
        [SerializeField] private float m_duration = 0f;
        [SerializeField] private float m_time = 0f;
        [SerializeField] private float m_speedMultiplier = 1f;
        [SerializeField] private bool m_reversed = false;

        public event Action OnTick = null;
        public event Action<TimerCompletionContext> OnComplete = null;

        public TimerState State => m_state;

        public float Duration => m_duration;
        /// <summary>
        /// Use to get the amount of time left until this timer reaches to its destination.
        /// </summary>
        public float CurrentTime => m_time;
        public float Speed => m_speedMultiplier;
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
            m_time = m_duration;
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

            return;
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

            if (m_reversed)
                return;

            if (HasStarted) m_time += amount;
            else m_time = m_duration;
        }
        public void Shrink(float amount)
        {
            if (HasCompleted) return;

            m_duration -= amount;

            if (m_reversed)
                return;

            if (HasStarted) m_time -= amount;
            else m_time = m_duration;
        }
        public void Reverse()
        {
            if (HasCompleted) return;

            m_reversed = !m_reversed;

            return;
        }
        public void Flip()
        {
            if (HasCompleted) return;

            m_time = m_duration - m_time;
            Reverse();
        }
        public void SetSpeed(float newSpeed)
        {
            if (HasCompleted) return;
            if (newSpeed <= 0f)
                throw new Exception("Speed of a timer cannot be zero or lower! Use Reverse() instead.");

            m_speedMultiplier = newSpeed;

            return;
        }

        #endregion

        #region Internal API

        internal void Tick()
        {
            if (!IsActive) return;
            if (IsPaused) return;

            if (m_reversed) m_time += Time.deltaTime * m_speedMultiplier;
            else m_time -= Time.deltaTime * m_speedMultiplier;

            OnTick?.Invoke();

            if (m_time <= 0f && !m_reversed)
                Succeed();
            else if (m_time >= m_duration && m_reversed) 
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
            m_time = 0f;
            m_speedMultiplier = 1f;
            m_reversed = false;

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
