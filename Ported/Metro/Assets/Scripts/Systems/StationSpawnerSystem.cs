using Unity.Collections;
using Unity.Entities;
using Unity.Transforms;
using Unity.Mathematics;

public partial class StationSpawnerSystem : SystemBase
{
    protected override void OnUpdate() {
        //UnityEngine.Debug.Log("update");
        var ecb = new EntityCommandBuffer(Allocator.Temp);
        var tracks = GetEntityQuery(ComponentType.ReadOnly<Track>(), ComponentType.ReadOnly<TrackID>(), ComponentType.ReadOnly<Spline>());
        var spawner = GetSingleton<StationSpawner>();
        var splines = tracks.ToComponentDataArray<Spline>(Allocator.TempJob);
        var trackIDs = tracks.ToComponentDataArray<TrackID>(Allocator.TempJob);

        Entities
            .ForEach((Entity entity, in Station station, in TrackID trackId) =>
            {
                //UnityEngine.Debug.Log($"{station.trackDistance}");
                ecb.DestroyEntity(entity);
                for (int i = 0; i < trackIDs.Length; i++) {
                    if (trackIDs[i].id == trackId.id) {
                        ref var splineData = ref splines[i].splinePath.Value;
                        int lookupCache = 0;
                        var instance = ecb.Instantiate(spawner.prefab);
                        float3 position, direction;
                        SplineInterpolationHelper.InterpolatePositionAndDirection(ref splineData, ref lookupCache, station.trackDistance, out position, out direction);
                        var translation = new Translation { Value = position };
                        var rotation = new Rotation { Value = quaternion.LookRotationSafe(direction, new float3(0, 1, 0)) };
                        ecb.SetComponent(instance, translation);
                        ecb.SetComponent(instance, rotation);
                        break;
                        //UnityEngine.Debug.Log($"{instance.Index}");
                    }
                }
            }).Run();

        splines.Dispose();
        trackIDs.Dispose();
        ecb.Playback(EntityManager);
        ecb.Dispose();
    }
}