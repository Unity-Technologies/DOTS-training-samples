using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

[UpdateBefore(typeof(SteeringSystem))]
public class PheromonePlacementSystem : SystemBase {

    protected override void OnCreate() {
        base.OnCreate();
        RequireForUpdate(GetEntityQuery(typeof(PheromoneMap), typeof(PheromoneStrength)));
    }

    protected override void OnUpdate() {
        var mapEntity = GetSingletonEntity<PheromoneMap>();
        var map = EntityManager.GetComponentData<PheromoneMap>(mapEntity);
        var pheromones = GetBuffer<PheromoneStrength>(mapEntity).AsNativeArray();
        var dt = Time.DeltaTime;
        
        Dependency = Entities.WithName("PheromonePlacement").ForEach((in AntTag ant, in Translation translation) => {
            int2 gridPos = PheromoneMap.WorldToGridPos(map, translation.Value);
            int index = PheromoneMap.GridPosToIndex(map, gridPos);
            if (index < 0 || index >= pheromones.Length) {
                return;
            }

            float excitementFactor = (ant.HasFood ? map.HasFoodExcitementFactor : 1.0f);
            excitementFactor += ant.GoalSeekAmount * map.GoalSeekExcitementFactor;

            pheromones[index] = math.saturate(pheromones[index] + map.AntPheremoneStrength * dt * excitementFactor);
        }).ScheduleParallel(Dependency);
    }
}
