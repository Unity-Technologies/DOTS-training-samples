using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Rendering;
using Unity.Transforms;

public class RailGeneration : SystemBase
{
    protected override void OnUpdate()
    {
        var railPrefab = GetSingleton<MetroData>().RailPrefab;
        var ecb = new EntityCommandBuffer(Allocator.Temp);

        Entities.ForEach((in PathRef pathRef) =>
        {
            var positions = pathRef.Data.Value.Positions.ToNativeArray();
            var handlesIn = pathRef.Data.Value.HandlesIn.ToNativeArray();
            var handlesOut = pathRef.Data.Value.HandlesOut.ToNativeArray();
            var distances = pathRef.Data.Value.Distances.ToNativeArray();
            
            float totalAbsoluteDistance = pathRef.Data.Value.TotalDistance;
            float absoluteDistance = 0.0f;
            float3 color = pathRef.Data.Value.Colour;

            while (absoluteDistance < totalAbsoluteDistance)
            {
                float coef = absoluteDistance / totalAbsoluteDistance;

                float3 railPos = BezierHelpers.GetPosition(positions, handlesIn, handlesOut, distances, totalAbsoluteDistance, coef);
                float3 railRot = BezierHelpers.GetNormalAtPosition(positions, handlesIn, handlesOut, distances, totalAbsoluteDistance, coef);

                var railEntity = ecb.Instantiate(railPrefab);
                var railTranslation = new Translation { Value = railPos };
                var railRotation = new Rotation { Value = quaternion.LookRotation(railRot, new float3(0, 1, 0)) };

                var col = new URPMaterialPropertyBaseColor { Value = new float4(color.x, color.y, color.z, 1f) };

                ecb.SetComponent(railEntity, railTranslation);
                ecb.SetComponent(railEntity, railRotation);
                ecb.AddComponent(railEntity, col);

                absoluteDistance += Globals.RAIL_SPACING;
            }
        }).Run();

        ecb.Playback(EntityManager);

        Enabled = false;
    }
}

namespace MetroECS
{
}
