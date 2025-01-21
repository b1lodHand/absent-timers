using System;

namespace com.absence.timersystem
{
    /// <summary>
    /// The class that lets you display timer data in inspector.
    /// </summary>
    [System.Serializable]
    public class TimerReference 
    {
        public Func<Timer> TimerProvider;
    }
}
