using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;

[UpdateInGroup(typeof(SimulationSystemGroup))]
public class TargetingSystem : JobComponentSystem
{
    protected override JobHandle OnUpdate(JobHandle inputDeps)
    {
        
    }

    [RequireComponentTag(typeof(HasResourcesTagComponent))]
    struct SetTargetJob : IJobForEach<TargetComponent>
    {
        public void Execute(ref TargetComponent )
        {
            
        }
    }
}

[UpdateInGroup(typeof(SimulationSystemGroup))]
[UpdateAfter(typeof(TargetingSystem))]
public class ResourceCarrySystem : JobComponentSystem
{
    protected override JobHandle OnUpdate(JobHandle inputDeps)
    {
        throw new System.NotImplementedException();
    }
}

[UpdateInGroup(typeof(SimulationSystemGroup))]
[UpdateAfter(typeof(ResourceCarrySystem))]
public class MapBorderAvoidanceSystem : JobComponentSystem
{
    protected override JobHandle OnUpdate(JobHandle inputDeps)
    {
        throw new System.NotImplementedException();
    }
}

[UpdateInGroup(typeof(SimulationSystemGroup))]
[UpdateAfter(typeof(MapBorderAvoidanceSystem))]
public class UpdatePositionSystem : JobComponentSystem
{
    protected override JobHandle OnUpdate(JobHandle inputDeps)
    {
        throw new System.NotImplementedException();
    }
}

[UpdateInGroup(typeof(SimulationSystemGroup))]
[UpdateAfter(typeof(UpdatePositionSystem))]
public class ObstacleCollisionSystem : JobComponentSystem
{
    protected override JobHandle OnUpdate(JobHandle inputDeps)
    {
        throw new System.NotImplementedException();
    }
}

[UpdateInGroup(typeof(SimulationSystemGroup))]
[UpdateAfter(typeof(ObstacleCollisionSystem))]
public class PheromoneDropSystem : JobComponentSystem
{
    protected override JobHandle OnUpdate(JobHandle inputDeps)
    {
        throw new System.NotImplementedException();
    }
}

[UpdateInGroup(typeof(SimulationSystemGroup))]
[UpdateAfter(typeof(PheromoneDropSystem))]
public class PheromoneDecaySystem : JobComponentSystem
{
    protected override JobHandle OnUpdate(JobHandle inputDeps)
    {
        throw new System.NotImplementedException();
    }
}

[UpdateInGroup(typeof(SimulationSystemGroup))]
[UpdateAfter(typeof(ObstacleCollisionSystem))]
public class RadialMovementSystem : JobComponentSystem
{
    protected override JobHandle OnUpdate(JobHandle inputDeps)
    {
        throw new System.NotImplementedException();
    }
}

[UpdateInGroup(typeof(SimulationSystemGroup))]
[UpdateAfter(typeof(RadialMovementSystem))]
public class WriteVelocityToFacingSystem : JobComponentSystem
{
    protected override JobHandle OnUpdate(JobHandle inputDeps)
    {
        throw new System.NotImplementedException();
    }
}




[UpdateInGroup(typeof(PresentationSystemGroup))]
public class UpdateAntColorSystem : JobComponentSystem
{
    protected override JobHandle OnUpdate(JobHandle inputDeps)
    {
        throw new System.NotImplementedException();
    }
}

[UpdateInGroup(typeof(SimulationSystemGroup))]
public class UpdateLocalToWorld : JobComponentSystem
{
    protected override JobHandle OnUpdate(JobHandle inputDeps)
    {
        throw new System.NotImplementedException();
    }
}