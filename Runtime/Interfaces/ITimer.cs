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
        /// Returns true if this timer is started but not completed yet.
        /// </summary>
        bool IsActive { get; }
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

        /// <summary>
        /// Use to start the timer if it's not started yet.
        /// </summary>
        void Start(bool startPaused);
        /// <summary>
        /// Use to restart the timer.
        /// </summary>
        void Restart(bool restartPaused);
        /// <summary>
        /// Use to pause the timer.
        /// </summary>
        void Pause();
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
        void Expand(float amount);
        /// <summary>
        /// Use to shrink the timer by some amoun.
        /// </summary>
        /// <param name="amount">Amount of shrink (s).</param>
        void Shrink(float amount);
    }
}
