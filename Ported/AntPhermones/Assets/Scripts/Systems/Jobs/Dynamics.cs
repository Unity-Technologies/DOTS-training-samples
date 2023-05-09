using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

public partial struct AntAI : ISystem
{
    public JobHandle DoDynamics(SystemState state)
    {
        var colony = SystemAPI.GetSingleton<Colony>();
        foreach (var (position, transform) in SystemAPI.Query<RefRW<Position>, RefRW<LocalTransform>>().WithAll<Ant>())
        {
            var position2D = position.ValueRW.position;
            transform.ValueRW.Position = new float3(position2D.x, position2D.y, 0);
            transform.ValueRW.Scale = colony.antScale;
        }
        return new();
    }

    public struct Dynamics : IJobParallelFor
    {
        public void Execute(int index)
        {
            // Here we do the work
            throw new System.NotImplementedException();
        }
    }
}

