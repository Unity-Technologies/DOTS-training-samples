using Unity.Burst;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;

[UpdateInGroup(typeof(FixedTimeStepSystemGroup))]
public class SwitchTargetSystem : JobComponentSystem
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
    var config = GetSingleton<ConfigComponent>();
    var gameboard = GetSingleton<GameboardComponent>();
    float sqrDistance = config.TargetRadius * config.TargetRadius;

    var jobHandle = Entities
      .WithAll<AntTag>()
      .ForEach((ref TargetComponent target, ref ResourceComponent resource, in Translation translation) =>
      {
        float2 pos = new float2(translation.Value.x, translation.Value.y);
        if (math.distancesq(pos, target.Value) < sqrDistance)
        {
          if (resource.HasResource)
          {
            resource.HasResource = false;
            target.Value = gameboard.ResourcePosition;
          }
          else
          {
            resource.HasResource = true;
            target.Value = gameboard.ColonyPosition;
          }
        }
      })
      .Schedule(inputDependencies);

    return jobHandle;
  }
}
