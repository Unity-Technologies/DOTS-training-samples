using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
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
        return new Job
        {
            MapSize = map.MapSize,
            MaxSpeed = antSteering.MaxSpeed,
            TrailAdd = map.TrailAdd * 1/50f,
            Pheromones = pheromoneMap
        }.ScheduleSingle(this, inputDeps);
    }


    [BurstCompile]
    struct Job : IJobForEach<PositionComponent, SpeedComponent, HasResourcesComponent>
    {
        public int MapSize;
        public float MaxSpeed;
        public float TrailAdd;
        [NativeDisableParallelForRestriction]
        public DynamicBuffer<PheromoneBuffer> Pheromones;
        
        public void Execute([ReadOnly] ref PositionComponent position, [ReadOnly] ref SpeedComponent speed, [ReadOnly] ref HasResourcesComponent hasResources)
        {
            var p = (int2)math.floor(position.Value);
            if (math.any(p < 0) || math.any(p >= MapSize))
                return;
            const float searcherExcitement = .3f;
            const float carrierExcitement = 1;
            float excitement = hasResources.Value ? carrierExcitement : searcherExcitement;
            float strength = excitement * speed.Value / MaxSpeed;
            int idx = p.y * MapSize + p.x;
            
            float rate = TrailAdd * strength;
            float pheromone = Pheromones[idx];
            pheromone += rate * (1 - pheromone);
            Pheromones[idx] = math.min(1, pheromone);
        }
    }
}
