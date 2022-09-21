using Unity.Entities;

partial struct BeecaySystem : ISystem
{
    public void OnCreate(ref SystemState state)
    {
    }

    public void OnDestroy(ref SystemState state)
    {
    }

    public void OnUpdate(ref SystemState state)
    {
        new DecayJob().ScheduleParallel();
    }
}