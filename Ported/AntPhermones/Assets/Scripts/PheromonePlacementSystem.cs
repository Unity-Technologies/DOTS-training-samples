using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

public class PheromonePlacementSystem : SystemBase {

    protected override void OnUpdate() {
        var mapEntity = GetSingletonEntity<PheromoneMap>();
        var map = EntityManager.GetComponentData<PheromoneMap>(mapEntity);
        var pheromones = EntityManager.GetBuffer<PheromoneStrength>(mapEntity).AsNativeArray();
        var dt = Time.DeltaTime;

        Dependency = Entities.ForEach((in AntTag ant, in Translation translation) => {
            int2 gridPos = PheromoneMap.WorldToGridPos(map, translation.Value);
            int index = PheromoneMap.GridPosToIndex(map, gridPos);
            if (index < 0 || index >= pheromones.Length) {
                return;
            }

            float excitementFactor = (ant.HasFood ? map.HasFoodExcitementFactor : 1.0f);
            excitementFactor += ant.GoalSeekAmount * map.GoalSeekExcitementFactor;

            pheromones[index] = math.saturate(pheromones[index] + map.AntPheremoneStrength * dt * excitementFactor);
        }).ScheduleParallel(Dependency);

        Dependency = Entities.ForEach((ref DynamicBuffer<PheromoneStrength> p, in PheromoneMap m) => {
        }).Schedule(Dependency);
    }
}