using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

public class RailGeneration : SystemBase
{
    protected override void OnUpdate()
    {
        var railPrefab = GetSingleton<MetroData>().RailPrefab;
        var ecb = new EntityCommandBuffer(Allocator.Temp);

        Entities.WithoutBurst().ForEach((in PathRef pathdata) =>
        { 
            ref var positions = ref pathdata.Data.Value.Positions;
            ref var handlesIn = ref pathdata.Data.Value.HandlesIn;
            ref var handlesOut = ref pathdata.Data.Value.HandlesOut;
            ref var distances = ref pathdata.Data.Value.Distances;
            float totalAbsoluteDistance = pathdata.Data.Value.TotalDistance;
            float absoluteDistance = 0.0f;

            int count = positions.Length;

            //float3[] pos = positions.ToArray();
            //float3[] hIn = handlesIn.ToArray();
            //float3[] hOut = handlesOut.ToArray();
            //float[] dist = distances.ToArray();

            float3[] pos = new float3[count];
            float3[] hIn = new float3[count];
            float3[] hOut = new float3[count];
            float[] dist = new float[count];
            for (int i = 0; i < count; ++i)
            {
                pos[i] = distances[i];
                hIn[i] = handlesIn[i];
                hOut[i] = handlesOut[i];
                dist[i] = distances[i];
            }

            while (absoluteDistance < totalAbsoluteDistance)
            {
                float coef = absoluteDistance / totalAbsoluteDistance;

                float3 railPos = BezierHelpers.GetPosition(pos, hIn, hOut, dist, totalAbsoluteDistance, coef);
                float3 railRot = BezierHelpers.GetNormalAtPosition(pos, hIn, hOut, dist, totalAbsoluteDistance, coef);

                var railEntity = ecb.Instantiate(railPrefab);
                var railTranslation = new Translation { Value = railPos };
                var railRotation = new Rotation {Value = quaternion.LookRotation(railRot, new float3(0, 1, 0)) };

                ecb.SetComponent(railEntity, railTranslation);
                ecb.SetComponent(railEntity, railRotation);

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
