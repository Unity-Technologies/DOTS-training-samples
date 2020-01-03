using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;

[UpdateInGroup(typeof(FixedTimeStepSystemGroup))]
public class DropPheromonesSystem : JobComponentSystem
{
  public EntityQuery PheromoneQuery;

  protected override void OnCreate()
  {
    base.OnCreate();
    PheromoneQuery = GetEntityQuery(ComponentType.ReadWrite<PheromoneElement>());
  }

  [BurstCompile]
  protected override JobHandle OnUpdate(JobHandle inputDependencies)
  {
    var pheromonesFromEntity = GetBufferFromEntity<PheromoneElement>();
    var pheromonesEntity = PheromoneQuery.GetSingletonEntity();
    var config = GetSingleton<ConfigComponent>();

    inputDependencies.Complete();
    var pheromones = pheromonesFromEntity[pheromonesEntity].Reinterpret<float>().AsNativeArray();

    Entities
      .WithAll<AntTag>()
      .WithNativeDisableParallelForRestriction(pheromones)
      .ForEach((in Translation translation) =>
      {
        int ix = (int)math.floor(translation.Value.x * config.MapSize);
        int iy = (int)math.floor(translation.Value.y * config.MapSize);
        if (ix >= 0 && ix < config.MapSize && iy >= 0 && iy < config.MapSize)
        {
          int index = ix + iy * config.MapSize;
          pheromones[index] = math.min(1f, pheromones[index] + config.DropRate * (1f - pheromones[index]));
        }
      })
      .Run();

    return default;
  }
}
