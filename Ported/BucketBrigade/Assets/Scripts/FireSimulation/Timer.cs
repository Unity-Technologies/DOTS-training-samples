using Unity.Entities;

public struct Timer : IComponentData
{
    public float elapsedTime;
    public float timerValue;

    public bool TimerIsUp()
    {
        return elapsedTime > timerValue;
    }
}
