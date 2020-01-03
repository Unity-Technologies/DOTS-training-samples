using Unity.Burst;
using Unity.Entities;
using Unity.Jobs;

[UpdateInGroup(typeof(FixedTimeStepSystemGroup))]
public class DecayPheromonesSystem : JobComponentSystem
{
  public EntityQuery ConfigQuery;

  protected override void OnCreate()
  {
    base.OnCreate();
    ConfigQuery = GetEntityQuery(ComponentType.ReadOnly<ConfigComponent>());
  }

  [BurstCompile]
  protected override JobHandle OnUpdate(JobHandle inputDependencies)
  {
    var configFromEntity = GetComponentDataFromEntity<ConfigComponent>(true);
    var configEntity = ConfigQuery.GetSingletonEntity();
    var config = configFromEntity[configEntity];

    var jobHandle = Entities
      .ForEach((DynamicBuffer<PheromoneElement> buffer) =>
      {
        var pheromones = buffer.Reinterpret<float>().AsNativeArray();
        for (int i = 0; i < pheromones.Length; ++i)
        {
          pheromones[i] *= config.DecayRate;
        }
      })
      .Schedule(inputDependencies);

    return jobHandle;
  }
}
