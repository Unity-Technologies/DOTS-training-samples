using System.Collections.Generic;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Rendering;
using Unity.Transforms;
using UnityEngine;

public class RepositioningSystem : SystemBase
{
    private EndSimulationEntityCommandBufferSystem sys;

    protected override void OnCreate()
    {
        sys = World.GetExistingSystem<EndSimulationEntityCommandBufferSystem>();
    }
    
    protected override void OnUpdate()
    {
        var ecb = sys.CreateCommandBuffer();
        Dependency = Entities
            .WithAll<Line>()
            .WithAll<Reposition>()
            .ForEach((Entity entity, in Line line) =>
            {
                var emptyTailTarget = GetComponent<TargetPosition>(line.EmptyTail).Value;
                var fullTailTarget = GetComponent<TargetPosition>(line.FullTail).Value;

                if (emptyTailTarget.x > 0.01f || emptyTailTarget.y > 0.01f || emptyTailTarget.z > 0.01f)
                {
                    int points = line.HalfCount - 2;
                    float3 diff = emptyTailTarget - fullTailTarget;
                    float3 increment = diff / points;
                    float3 arcRight = new float3(diff.z, diff.y, -diff.x) * 0.01f;

                    var nextEmptyBucketer = line.EmptyHead;
                    float3 nextTarget = fullTailTarget;
                    int counter = 0;

                    while (nextEmptyBucketer != Entity.Null)
                    {
                        SetComponent<TargetPosition>(nextEmptyBucketer, new TargetPosition {Value = nextTarget});
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
                    var nextFullBucketer = line.FullHead;
                    nextTarget = emptyTailTarget;
                    counter = 0;

                    while (nextFullBucketer != Entity.Null)
                    {
                        SetComponent<TargetPosition>(nextFullBucketer, new TargetPosition {Value = nextTarget});
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

                    ecb.RemoveComponent<Reposition>(entity);
                }
            }).Schedule(Dependency);
    }
}