using Unity.Burst;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Entities;
using Unity.Jobs;
using Unity.Jobs.LowLevel.Unsafe;
using Unity.Mathematics;

[UpdateInGroup(typeof(SimulationSystemGroup))]
[UpdateAfter(typeof(RadialMovementSystem))]
public class PheromoneDropSystem : JobComponentSystem
{
    EntityQuery m_PheromoneMapQuery;
    EntityQuery m_MapQuery;
    EntityQuery m_AntSteeringQuery;
    
    protected override void OnCreate()
    {
        base.OnCreate();
        m_PheromoneMapQuery = GetEntityQuery(ComponentType.ReadWrite<PheromoneBuffer>());
        m_MapQuery = GetEntityQuery(ComponentType.ReadOnly<MapSettingsComponent>());
        m_AntSteeringQuery = GetEntityQuery(ComponentType.ReadOnly<AntSteeringSettingsComponent>());
    }

    protected override JobHandle OnUpdate(JobHandle inputDeps)
    {
        var map = m_MapQuery.GetSingleton<MapSettingsComponent>();
        var pheromoneFromEntity = GetBufferFromEntity<PheromoneBuffer>();
        var pheromoneMap = pheromoneFromEntity[m_PheromoneMapQuery.GetSingletonEntity()];
        var antSteering = m_AntSteeringQuery.GetSingleton<AntSteeringSettingsComponent>();
        var trailAdd = map.TrailAdd * 1/50f;
        var maxSpeed = antSteering.MaxSpeed;
        var mapSize = map.MapSize;
        var pheromones = pheromoneMap;
        inputDeps.Complete();
        Entities.ForEach((in PositionComponent position, in SpeedComponent speed, in HasResourcesComponent hasResources) =>
        {
            var p = (int2)math.floor(position.Value);
            if (math.any(p < 0) || math.any(p >= mapSize))
                return;
            const float searcherExcitement = .3f;
            const float carrierExcitement = 1;
            float excitement = hasResources.Value ? carrierExcitement : searcherExcitement;
            float strength = excitement * speed.Value / maxSpeed;
            int idx = p.y * mapSize + p.x;
            
            float rate = trailAdd * strength;
            float pheromone = pheromones[idx];
            pheromone += rate * (1 - pheromone);
            pheromones[idx] = math.min(1, pheromone);
        }).WithNativeDisableParallelForRestriction(pheromones).Run();
        return default;
    }
    
}
