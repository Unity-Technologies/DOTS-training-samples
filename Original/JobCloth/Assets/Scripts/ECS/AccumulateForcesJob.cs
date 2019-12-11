using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

[UpdateInGroup(typeof(PresentationSystemGroup))]
[UpdateAfter(typeof(Constraint2_System))]
public class AccumulateForces_System : JobComponentSystem
{
    protected override JobHandle OnUpdate(JobHandle inputDeps)
    {
        Entities.WithoutBurst().ForEach((Entity entity, ClothComponent cloth, in LocalToWorld localToWorld) =>
        {
            var forces      = EntityManager.GetBuffer<Force>(entity);
            var vertices    = EntityManager.GetBuffer<CurrentVertex>(entity);
            var oldVertices = EntityManager.GetBuffer<PreviousVertex>(entity);
            var gravity     = cloth.Gravity;

            var firstPinnedIndex = cloth.constraints.Value.FirstPinnedIndex;
            //Execute
            for (int i = 0; i < firstPinnedIndex; ++i)
            {
                float3 oldVert  = oldVertices[i];
                float3 vert     = vertices[i];
                float3 startPos = vert;

                vert += (vert - oldVert + forces[i]);

                vertices[i]     = vert;
                oldVertices[i]  = startPos;
                forces[i]       = gravity;
            }
        }).Run();

        return inputDeps;
    }
}