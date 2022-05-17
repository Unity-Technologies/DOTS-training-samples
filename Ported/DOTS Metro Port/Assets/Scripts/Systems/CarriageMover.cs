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

        _carriageQuery = state.GetEntityQuery(typeof (Carriage));
    }

    public void OnDestroy(ref SystemState state)
    {
        
    }

    public void OnUpdate(ref SystemState state)
    {
        var entities = _carriageQuery.ToEntityArray(state.WorldUnmanaged.UpdateAllocator.ToAllocator);

        foreach (var (carriage, transform) in SystemAPI.Query<RefRO<Carriage>, TransformAspect>())
        {
            
        }
    }
}

