using Unity.Entities;
using Unity.Burst;
using Unity.Transforms;
using Unity.Mathematics;

[BurstCompile]
partial struct CarriageMover : ISystem
{
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<Config>();
    }

    public void OnDestroy(ref SystemState state)
    {
        
    }

    public void OnUpdate(ref SystemState state)
    {
        foreach (var (carriage, position) in SystemAPI.Query<RefRO<Carriage>, RefRW<DistanceAlongBezier>>())
        {
            position.ValueRW.Distance += 0.5f;
        }
    }
}

