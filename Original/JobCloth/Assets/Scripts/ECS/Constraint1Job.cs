using Unity.Burst;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Entities;
using Unity.Transforms;
using UnityEngine;
using System;

[ExecuteAlways]
[AlwaysUpdateSystem]
[UpdateInGroup(typeof(PresentationSystemGroup))]
public class Constraint1_System : JobComponentSystem
{
    protected override JobHandle OnUpdate(JobHandle inputDeps)
    {
        return Entities.ForEach((Entity entity, ref DynamicBuffer<CurrentVertex> vertices, in ClothComponent cloth, in LocalToWorld localToWorld) =>
        {
            ref var constraintIndices = ref cloth.constraints.Value.Constraint1Indices;
            ref var constraintLengths = ref cloth.constraints.Value.Constraint1Lengths;

            //Execute
            {
                var indexCount = constraintIndices.Length;
                for (int i = 0; i < indexCount; i++)
                {
                    int2 pair = constraintIndices[i];

                    float3 p1 = vertices[pair.x];
                    float3 p2 = vertices[pair.y];

                    var delta = p2 - p1;
                    var length = math.length(delta);
                    var extra = (length - constraintLengths[i]);
                    var dir = delta / length;
                    var offset = extra * dir;
                    vertices[pair.x] = p1 + offset;
                }
            }            
        }).Schedule(inputDeps);
    }
}

