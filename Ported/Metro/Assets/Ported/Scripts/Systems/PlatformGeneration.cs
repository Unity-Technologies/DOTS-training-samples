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
                float totalDistance = pathRef.Data.Value.TotalDistance;
                float3 color = pathRef.Data.Value.Colour;

                for (var i = 1; i < positions.Length/2; i++)
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

                    AddPlatform(ecb, ecb.Instantiate(prefab), false, platEndIdx, color, positions, handlesIn, handlesOut, distances, totalDistance);
                    AddPlatform(ecb, ecb.Instantiate(prefab), true, positions.Length - platEndIdx, color, positions, handlesIn, handlesOut, distances, totalDistance);
                }
            }).Run();

            ecb.Playback(EntityManager);

            Enabled = false;
        }

        static void AddPlatform(EntityCommandBuffer ecs, Entity ent, bool flip, int idx, float3 color, NativeArray<float3> positions, NativeArray<float3> handlesIn, NativeArray<float3> handlesOut, NativeArray<float> distances, float totalDistance)
        {
            float3 platPos = positions[idx];
            var translation = new Translation { Value = platPos };

            // Make sure we use the exact same tangent for facing platforms
            bool facingPlatform = 2 * idx > positions.Length;
            int handleIdx = facingPlatform ? positions.Length - idx : idx;
            var tangent = facingPlatform ? handlesIn[handleIdx] - handlesOut[handleIdx] : handlesOut[handleIdx] - handlesIn[handleIdx];

            UnityEngine.Quaternion q = new UnityEngine.Quaternion();
            q.SetLookRotation(new UnityEngine.Vector3(tangent.x, tangent.y, tangent.z), UnityEngine.Vector3.up);
            UnityEngine.Vector3 ea = q.eulerAngles;
            q.eulerAngles = new UnityEngine.Vector3(ea.x, ea.y + 90f, ea.z);

            var rotation = new Rotation { Value = 
                (
                    new quaternion(q.x, q.y, q.z, q.w)
                ) };
            var col = new URPMaterialPropertyBaseColor { Value = new float4(color.x, color.y, color.z, 1f) };

            ecs.SetComponent(ent, translation);
            ecs.SetComponent(ent, rotation);
            ecs.AddComponent(ent, col);
        }
    }
}
