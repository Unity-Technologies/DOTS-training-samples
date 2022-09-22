using Unity.Entities;
using Unity.Burst;

[BurstCompile]
partial struct CombatTargetSystem : ISystem
{
    private EntityQuery beeQuery;

    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        beeQuery = SystemAPI.QueryBuilder().WithAll<BeeProperties>().Build();
    }

    [BurstCompile]
    public void OnDestroy(ref SystemState state)
    {
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
    }
}
