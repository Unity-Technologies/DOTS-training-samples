using Unity.Entities;
using Unity.Jobs;
using UnityEngine;

public partial struct AntAI : ISystem
{
    public JobHandle DoDynamics(SystemState state)
    {
        foreach (var position in SystemAPI.Query<RefRW<Position>>().WithAll<Ant>())
        {
            Debug.Log(position.ValueRW.position);
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

