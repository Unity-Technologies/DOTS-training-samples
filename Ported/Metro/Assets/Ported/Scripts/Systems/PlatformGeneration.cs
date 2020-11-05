using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Rendering;
using Unity.Transforms;

namespace MetroECS
{
    public class PlatformGeneration : SystemBase
    {
        protected override void OnUpdate()
        {
            var prefab = GetSingleton<MetroData>().PlatformPrefab;
            var ecb = new EntityCommandBuffer(Allocator.Temp);

            Entities.ForEach((in PathDataRef pathRef) =>
            {
                var positions = pathRef.Data.Value.Positions.ToNativeArray();
                var markerTypes = pathRef.Data.Value.MarkerTypes.ToNativeArray();
                var handlesIn = pathRef.Data.Value.HandlesIn.ToNativeArray();
                var handlesOut = pathRef.Data.Value.HandlesOut.ToNativeArray();
                var distances = pathRef.Data.Value.Distances.ToNativeArray();
                var totalDistance = pathRef.Data.Value.TotalDistance;
                float3 color = pathRef.Data.Value.Colour;

                for (var i = 1; i < positions.Length; i++)
                {
                    int platEndIdx = i;
                    int platStartIdx = i - 1;

                    if (!(markerTypes[platEndIdx] == (int) RailMarkerType.PLATFORM_END &&
                          markerTypes[platStartIdx] == (int) RailMarkerType.PLATFORM_START))
                    {
                        continue;
                    }

                    // Instantiate
                    var entity = ecb.Instantiate(prefab);

                    // Position
                    //var position = math.lerp(positions[i - 1], positions[i], 0.5f);
                    float3 platPos = positions[platEndIdx];
                    //BezierHelpers.BezierLerp(positions[i - 1], handlesOut[i - 1], positions[i], handlesIn[i], 0.5f);
                    var translation = new Translation { Value = platPos };

                    // Rotation
                    var normalizedPosition = distances[platEndIdx] / totalDistance;
                    var perp = BezierHelpers.GetPointPerpendicularOffset(platPos, distances[platEndIdx], positions, handlesIn, handlesOut, distances, totalDistance, Globals.BEZIER_PLATFORM_OFFSET);
                    var rotation = new Rotation {Value = quaternion.LookRotation(perp, new float3(0, 1, 0))};
                    var col = new URPMaterialPropertyBaseColor { Value = new float4(color.x, color.y, color.z, 1f) };

                    ecb.SetComponent(entity, translation);
                    ecb.SetComponent(entity, rotation);
                    ecb.AddComponent(entity, col);
                }
            }).Run();

            ecb.Playback(EntityManager);

            Enabled = false;
        }
    }
}
