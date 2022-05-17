using Unity.Entities;
using Unity.Burst;
using Unity.Transforms;
using Unity.Mathematics;

[BurstCompile]
partial struct CarriageMover : ISystem
{
    private EntityQuery _carriageQuery;

    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<Config>();
    }

    public void OnDestroy(ref SystemState state)
    {
        
    }

    public void OnUpdate(ref SystemState state)
    {
        foreach (var (carriage, position) in SystemAPI.Query<RefRO<Carriage>, RefRW<PositionOnBezier>>())
        {
            position.ValueRW.Position += 0.001f;
        }
    }
}

