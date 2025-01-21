namespace com.absence.timersystem.internals
{
    internal interface ITimer2
    {
        void Tick();
        void Succeed();
        void Dispose();
        void ResetProperties();

        TimerCompletionContext GenerateCompletionContext();
    }
}
