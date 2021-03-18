using System.Collections.Generic;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Rendering;
using Unity.Transforms;
using UnityEngine;

public class RepositioningSystem : SystemBase
{
    protected override void OnUpdate()
    {
        Entities
            .ForEach((Entity entity, in Line line) =>
            {
                var emptyTailTarget = GetComponent<TargetPosition>(line.EmptyTail).Value;
                var fullTailTarget = GetComponent<TargetPosition>(line.FullTail).Value;

                int points = line.HalfCount - 2;
                float3 diff = emptyTailTarget - fullTailTarget;
                float3 increment = diff / points;

                var nextEmptyBucketer = GetComponent<NextPerson>(line.EmptyHead).Value;
                float3 nextTarget = fullTailTarget;

                while (nextEmptyBucketer != Entity.Null)
                {
                    SetComponent<TargetPosition>(nextEmptyBucketer, new TargetPosition { Value = nextTarget });
                    nextEmptyBucketer = GetComponent<NextPerson>(nextEmptyBucketer).Value;
                    nextTarget += increment;
                }

                var nextFullBucketer = GetComponent<NextPerson>(line.FullHead).Value;
                nextTarget = emptyTailTarget;

                while (nextFullBucketer != Entity.Null)
                {
                    SetComponent<TargetPosition>(nextFullBucketer, new TargetPosition { Value = nextTarget });
                    nextFullBucketer = GetComponent<NextPerson>(nextFullBucketer).Value;
                    nextTarget -= increment;
                }

            }).Run();
    }
}