using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using UnityEngine;

[UpdateInGroup(typeof(FixedTimeStepSystemGroup))]
public class PheromonesToTextureSystem : JobComponentSystem
{
  public NativeArray<Color> Pixels;
  public JobHandle Handle;

  protected override void OnDestroy()
  {
    base.OnDestroy();
    Pixels.Dispose();
  }

  [BurstCompile]
  protected override JobHandle OnUpdate(JobHandle inputDependencies)
  {
    if (!Pixels.IsCreated)
    {
      var config = GetSingleton<ConfigComponent>();
      Pixels = new NativeArray<Color>(config.MapSize * config.MapSize, Allocator.Persistent);
    }

    var pixels = Pixels;

    var jobHandle = Entities
      .ForEach((in DynamicBuffer<PheromoneElement> buffer) =>
      {
        var pheromones = buffer.Reinterpret<float>().AsNativeArray();
        if (pheromones.IsCreated)
        {
          for (int i = 0; i < pheromones.Length; ++i)
          {
            Color c = new Color();
            c.r = pheromones[i];
            c.g = 0;
            c.b = 0;
            c.a = 1f;
            pixels[i] = c;
          }
        }
      })
      .Schedule(inputDependencies);

    Handle = jobHandle;
    return jobHandle;
  }
}