using Unity.Collections;
using Unity.Entities;
using Unity.Transforms;
using Unity.Mathematics;

[UpdateAfter(typeof(TransformSystemGroup))]
public partial class StationSpawnerSystem : SystemBase
{
    protected override void OnUpdate() 
    {
        var ecb = new EntityCommandBuffer(Allocator.Temp);
        var trackQuery = GetEntityQuery(ComponentType.ReadOnly<Track>(), ComponentType.ReadOnly<TrackID>(), ComponentType.ReadOnly<Spline>());
        var spawner = GetSingleton<StationSpawner>();
        var tracks = trackQuery.ToEntityArray(Allocator.TempJob);
        var splines = trackQuery.ToComponentDataArray<Spline>(Allocator.TempJob);
        var trackIDs = trackQuery.ToComponentDataArray<TrackID>(Allocator.TempJob);

        Entities
            .ForEach((Entity entity, in Station station, in TrackID trackId) =>
            {
                // Get the buffer component as it contains the children
                var childBuffer = GetBuffer<Child>(entity);
                // Destroy all that hierarchy level
                for (int i = 0; i < childBuffer.Length; ++i)
                    ecb.DestroyEntity(childBuffer[i].Value);
                // Destroy station itself
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
                        GetBuffer<FloatBufferElement>(tracks[i]).Add(station.trackDistance);
                        // Propagates station and trackid on the prefab
                        ecb.AddComponent(instance, station);
                        ecb.AddComponent(instance, trackId);
                        break;
                    }
                }
            }).Run();

        // Example for fetching station distances
        Entities
            .ForEach((in DynamicBuffer<FloatBufferElement> buffer, in TrackID trackId) => {
                UnityEngine.Debug.Log($"Track ID: {trackId.id}");
                for (int i = 0; i < buffer.Length; i++) {
                    UnityEngine.Debug.Log($"Station Distance: {buffer[i].Value}");
                }
            }).Run();

        tracks.Dispose();
        splines.Dispose();
        trackIDs.Dispose();
        ecb.Playback(EntityManager);
        ecb.Dispose();

        Enabled = false;
    }
}