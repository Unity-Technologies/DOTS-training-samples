
using Unity.Burst;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

[UpdateInGroup(typeof(PresentationSystemGroup))]
[UpdateAfter(typeof(Constraint1_System))]
public class Constraint2_System : JobComponentSystem
{
    protected override JobHandle OnUpdate(JobHandle inputDeps)
    {
        Entities.WithoutBurst().ForEach((Entity entity, ClothComponent cloth, in LocalToWorld localToWorld) =>
        {
            var vertices = EntityManager.GetBuffer<CurrentVertex>(entity);
            ref var constraintIndices = ref cloth.constraints.Value.Constraint2Indices;
            ref var constraintLengths = ref cloth.constraints.Value.Constraint2Lengths;
            
            {
                var indexCount = constraintIndices.Length;
                for (int i = 0; i < indexCount; i++)
                {
                    int2 pair = constraintIndices[i];

                    float3 p1 = vertices[pair.x];
                    float3 p2 = vertices[pair.y];

                    var delta = p2 - p1;
                    var length = math.length(delta);
                    var extra = (length - constraintLengths[i]) * .5f;
                    var dir = delta / length;
                    var offset = extra * dir;
                    vertices[pair.x] = p1 + offset;
                    vertices[pair.y] = p2 - offset;
                }
            }
        }).Run();

        return inputDeps;
    }
}



