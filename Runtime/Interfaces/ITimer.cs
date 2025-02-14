using System;

namespace com.absence.timersystem
{
    public interface ITimer
    {
        /// <summary>
        /// Current state of the timer.
        /// </summary>
        public TimerState State { get; }

        /// <summary>
        /// The total duration the timer initialized with. Is not dynamic.
        /// </summary>
        float Duration { get; }
        /// <summary>
        /// The current time of the timer. Logic depends on the type of timer.
        /// </summary>
        float CurrentTime { get; }
        /// <summary>
        /// The current completion percentage of the timer.
        /// </summary>
        float Percentage { get; }
        /// <summary>
        /// Current speed modifier of the timer.
        /// </summary>
        float Speed { get; }
        /// <summary>
        /// Returns true if this timer is paused.
        /// </summary>
        bool IsPaused { get; }
        /// <summary>
        /// Returns true if the <see cref="ITimer.Start"/> function got called after initializing the timer.
        /// </summary>
        bool HasStarted { get; }
        /// <summary>
        /// Returns true if this timer has ended.
        /// </summary>
        bool HasCompleted { get; }

        public event Action onTick;
        public event Action<bool> onPauseStateChange;
        public event Action<TimerCompletionContext> onComplete;

        /// <summary>
        /// Use to manually tick the timer.
        /// </summary>
        void Tick();
        /// <summary>
        /// Use to manually tick the timer with a custom amount.
        /// </summary>
        /// <param name="amount">Amount of ticks in ms.</param>
        void Tick(float amount);
        /// <summary>
        /// Use to restart the timer.
        /// </summary>
        void Restart(bool restartPaused = false);
        /// <summary>
        /// Use to pause the timer.
        /// </summary>
        ITimer Pause();
        /// <summary>
        /// Use to resume the timer.
        /// </summary>
        void Resume();
        /// <summary>
        /// Use to force-end a timer. Timer counts as failed.
        /// </summary>
        void Fail();
        /// <summary>
        /// Use to force-end a timer. Timer counts as succeeded.
        /// </summary>
        void Succeed();
        /// <summary>
        /// Use to expand the timer by some amount.
        /// </summary>
        /// <param name="amount">Amount of expansion (s).</param>
        ITimer Expand(float amount);
        /// <summary>
        /// Use to shrink the timer by some amoun.
        /// </summary>
        /// <param name="amount">Amount of shrink (s).</param>
        ITimer Shrink(float amount);
        /// <summary>
        /// Use to make the timer tick in the opposite direction.
        /// </summary>
        ITimer Reverse();
        /// <summary>
        /// Use to mirror the timer. Simply reverses the timer but keeps the time left to completion same.
        /// </summary>
        ITimer Flip();

        /// <summary>
        /// Use to set the speed modifier of the timer to the given value. Default value for modifier is 1f.
        /// </summary>
        /// <param name="newSpeed">New value of the speed modifier. <b>CAN NOT BE BELOW OR EQUAL TO 0f</b>.</param>
        ITimer SetSpeed(float newSpeed);
        /// <summary>
        /// Use to change the duration of the timer.
        /// </summary>
        /// <param name="newDuration">New duration.</param>
        ITimer SetDuration(float newDuration);
        /// <summary>
        /// Use to make the timer jump to a percentage.
        /// </summary>
        /// <param name="newPercentage">New percentage between 0-1.</param>
        ITimer SetPercentage(float newPercentage);

        /// <summary>
        /// Use to append an action to the completion event.
        /// </summary>
        /// <param name="action">The action to append.</param>
        public ITimer OnComplete(Action<TimerCompletionContext> action);
        /// <summary>
        /// Use to append an action to the tick event.
        /// </summary>
        /// <param name="action">The action to append.</param>
        public ITimer OnTick(Action action);
        /// <summary>
        /// Use to append an action to the pause/resume event.
        /// </summary>
        /// <param name="action">The action to append. Boolean parameter is true if the event is a pause.</param>
        public ITimer OnPauseResume(Action<bool> action);
    }
}
