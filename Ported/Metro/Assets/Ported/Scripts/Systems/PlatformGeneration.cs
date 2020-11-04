using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

namespace MetroECS
{
    public class PlatformGeneration : SystemBase
    {
        protected override void OnUpdate()
        {
            var prefab = GetSingleton<MetroData>().PlatformPrefab;
            var ecb = new EntityCommandBuffer(Allocator.Temp);

            Entities.ForEach((in PathRef pathRef) =>
            {
                var positions = pathRef.Data.Value.Positions.ToNativeArray();
                var markerTypes = pathRef.Data.Value.MarkerTypes.ToNativeArray();
                var handlesIn = pathRef.Data.Value.HandlesIn.ToNativeArray();
                var handlesOut = pathRef.Data.Value.HandlesOut.ToNativeArray();
                var distances = pathRef.Data.Value.Distances.ToNativeArray();
                var totalDistance = pathRef.Data.Value.TotalDistance;

                var sumDistances = 0f;
                for (var i = 1; i < positions.Length; i++)
                {
                    sumDistances += distances[i - 1];
                    
                    if (!(markerTypes[i] == (int) RailMarkerType.PLATFORM_END &&
                          markerTypes[i - 1] == (int) RailMarkerType.PLATFORM_START))
                    {
                        continue;
                    }

                    // Instantiate
                    var entity = ecb.Instantiate(prefab);

                    // Position
                    var position = math.lerp(positions[i - 1], positions[i], 0.5f);
                    var translation = new Translation {Value = position};
                    ecb.SetComponent(entity, translation);

                    // Rotation
                    var platformDistance = sumDistances + (distances[i] / 2f);
                    var normalizedPosition = platformDistance / totalDistance;
                    var normal = BezierHelpers.GetNormalAtPosition(positions, handlesIn, handlesOut, distances, totalDistance, normalizedPosition);
                    var rotation = new Rotation {Value = quaternion.LookRotation(normal, new float3(0, 1, 0))};
                    ecb.SetComponent(entity, rotation);
                }
            }).Run();

            ecb.Playback(EntityManager);
        
            Enabled = false;
        }
    }
}