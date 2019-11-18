using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;

[UpdateInGroup(typeof(SimulationSystemGroup))]
[UpdateAfter(typeof(ObstacleCollisionSystem))]
public class PheromoneDropSystem : JobComponentSystem
{
    EntityQuery m_PheromoneMapQuery;
    EntityQuery m_MapQuery;
    protected override void OnCreate()
    {
        base.OnCreate();
        m_PheromoneMapQuery = GetEntityQuery(ComponentType.ReadWrite<PheromoneMapComponent>());
        m_MapQuery = GetEntityQuery(ComponentType.ReadOnly<MapSettingsComponent>());
    }
    
    protected override JobHandle OnUpdate(JobHandle inputDeps)
    {
        var map = m_MapQuery.GetSingleton<MapSettingsComponent>();
        var pheromoneMap = m_PheromoneMapQuery.GetSingleton<PheromoneMapComponent>();
        new Job
        {
            CarrierExcitement = 1f,
            SearcherExcitement = .3f,
            TrailAdd = map.TrailAdd,
            MaxSpeed = map.MaxSpeed,
            MapSize = map.MapSize,
            Pheromones = pheromoneMap.PheromoneMap.Value.Map,
        }.Run(this, inputDeps);
        return default;
    }
    
    struct Job : IJobForEach<PositionComponent, SpeedComponent, HasResourcesComponent>
    {
        public BlobArray<float> Pheromones;
        public float SearcherExcitement;
        public float CarrierExcitement;
        public float MaxSpeed;

        public float TrailAdd;
        
        public int MapSize;

        public void Execute(
            [ReadOnly] ref PositionComponent position,
            [ReadOnly] ref SpeedComponent speed,
            [ReadOnly] ref HasResourcesComponent hasResources)
        {
            var p = (int2)math.floor(position.Value);
            if (math.any(p < 0) || math.any(p >= MapSize))
                return;
            float e = hasResources.Value ? CarrierExcitement : SearcherExcitement;
            float strength = e * speed.Value / MaxSpeed;
            float pheromone = Pheromones[p.y * MapSize + p.x];
            Pheromones[p.y * MapSize + p.x] = math.min(1, pheromone + TrailAdd * strength * (1 - pheromone));
        }
    }
}