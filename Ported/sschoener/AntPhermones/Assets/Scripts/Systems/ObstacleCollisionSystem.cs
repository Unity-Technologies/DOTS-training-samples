using System;
using Unity.Entities;
using Unity.Jobs;

[UpdateInGroup(typeof(SimulationSystemGroup))]
[UpdateAfter(typeof(UpdatePositionSystem))]
public class ObstacleCollisionSystem : JobComponentSystem
{
    protected override JobHandle OnUpdate(JobHandle inputDeps)
    {
        throw new System.NotImplementedException();
    }

    struct Job : IJobForEach<PositionComponent, VelocityComponent>
    {
        public void Execute(ref PositionComponent position, ref VelocityComponent velocity)
        {
            
        }
    }
}
