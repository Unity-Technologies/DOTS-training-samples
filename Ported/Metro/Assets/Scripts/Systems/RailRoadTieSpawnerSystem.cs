using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEditor.Rendering;

public partial class RailRoadTieSpawnerSystem : SystemBase
{
    protected override void OnUpdate()
    {
        var ecb = new EntityCommandBuffer(Allocator.Temp);
        var tracks = GetEntityQuery(ComponentType.ReadOnly<Track>(), ComponentType.ReadOnly<Spline>());
        
        Entities
            .ForEach((Entity entity, in RailRoadTieSpawner spawner) =>
            {
                var splines = tracks.ToComponentDataArray<Spline>(Allocator.TempJob);
                
                // Destroying the current entity is a classic ECS pattern,
                // when something should only be processed once then forgotten.
                ecb.DestroyEntity(entity);

                for (int s = 0; s < splines.Length; ++s)
                {
                    ref var splineData = ref splines[s].splinePath.Value;

                    int count = (int)math.ceil(splineData.pathLength / spawner.Frequency);
                    float distanceBetweenTies = splineData.pathLength / count;
                    float currentDistance = 0;
                    int lookupCache = 0;
                    for (int i = 0; i < count; ++i)
                    {
                        var instance = ecb.Instantiate(spawner.TiePrefab);
                        float3 position, direction;
                        SplineInterpolationHelper.InterpolatePositionAndDirection(ref splineData, ref lookupCache, currentDistance, out position, out direction);
                        var translation = new Translation {Value = position};
                        var rotation = new Rotation {Value = quaternion.LookRotation(direction, new float3(0, 1, 0))};
                        ecb.SetComponent(instance, translation);
                        ecb.SetComponent(instance, rotation);
                        currentDistance += distanceBetweenTies;
                    }
                }
                splines.Dispose();
            }).Run();

        ecb.Playback(EntityManager);
        ecb.Dispose();
    }
}
