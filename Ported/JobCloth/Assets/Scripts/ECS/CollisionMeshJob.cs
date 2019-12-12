
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

[ExecuteAlways]
[AlwaysUpdateSystem]
[UpdateInGroup(typeof(LateSimulationSystemGroup))]
[UpdateAfter(typeof(AccumulateForces_System))]
public unsafe class CollisionMesh_System : JobComponentSystem
{
    protected override JobHandle OnUpdate(JobHandle inputDeps)
    {
        return Entities.ForEach((Entity entity, ref DynamicBuffer<CurrentVertex> vertices, ref DynamicBuffer<PreviousVertex> oldVertices, in ClothComponent cloth, in LocalToWorld localToWorldParam) =>
        {
            var localToWorld = localToWorldParam.Value;
            var worldToLocal = math.inverse(localToWorldParam.Value);

            var verticesPtr = (float3*)vertices.GetUnsafePtr();
            var oldVerticesPtr = (float3*)oldVertices.GetUnsafePtr();

            var firstPinnedIndex = cloth.constraints.Value.FirstPinnedIndex;

            for (int i = 0; i < firstPinnedIndex; ++i)
            {
                float3 oldVert = oldVerticesPtr[i];
                float3 vert = verticesPtr[i];

                float3 worldPos = math.mul(localToWorld, new float4(vert, 1)).xyz;

                if (worldPos.y < 0f)
                {
                    float3 oldWorldPos = math.mul(localToWorld, new float4(oldVert, 1)).xyz;
                    oldWorldPos.y = (worldPos.y - oldWorldPos.y) * .5f;
                    worldPos.y = 0f;
                    vert = math.mul(worldToLocal, new float4(worldPos, 1)).xyz;
                    oldVert = math.mul(worldToLocal, new float4(oldWorldPos, 1)).xyz;
                }

                verticesPtr[i] = vert;
                oldVerticesPtr[i] = oldVert;
            }
        }).Schedule(inputDeps);
    }
}