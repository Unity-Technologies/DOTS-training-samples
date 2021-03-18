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
                float3 arcRight = new float3(diff.z, diff.y, -diff.x) * 0.01f;

                var nextEmptyBucketer = GetComponent<NextPerson>(line.EmptyHead).Value;
                float3 nextTarget = fullTailTarget;
                int counter = 0;

                while (nextEmptyBucketer != Entity.Null)
                {
                    SetComponent<TargetPosition>(nextEmptyBucketer, new TargetPosition { Value = nextTarget });
                    nextEmptyBucketer = GetComponent<NextPerson>(nextEmptyBucketer).Value;
                    nextTarget += increment;
                    if (line.HalfCount / 2 > counter)
                    {
                        nextTarget -= arcRight;
                    }
                    else if (line.HalfCount / 2 < counter)
                    {
                        nextTarget += arcRight;
                    }

                    counter++;
                }

                float3 arcLeft = new float3(-diff.z, diff.y, diff.x) * 0.01f;
                var nextFullBucketer = GetComponent<NextPerson>(line.FullHead).Value;
                nextTarget = emptyTailTarget;
                counter = 0;

                while (nextFullBucketer != Entity.Null)
                {
                    SetComponent<TargetPosition>(nextFullBucketer, new TargetPosition { Value = nextTarget });
                    nextFullBucketer = GetComponent<NextPerson>(nextFullBucketer).Value;
                    nextTarget -= increment;
                    if (line.HalfCount / 2 > counter)
                    {
                        nextTarget -= arcLeft;
                    }
                    else if (line.HalfCount / 2 < counter)
                    {
                        nextTarget += arcLeft;
                    }

                    counter++;

                }

            }).Schedule();
    }
}