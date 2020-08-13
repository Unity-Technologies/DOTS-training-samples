using Unity.Entities;
using Unity.Transforms;
using Unity.Mathematics;
using UnityEngine;

public class BotBucketFinderSystem : SystemBase
{
    protected override void OnUpdate()
    {

        Entities
            .WithName("BotBucketFinderSystem")
            .ForEach((Entity entity, ref TargetPosition targetPosition, in Bot bot) =>
            {
                // 
            }).ScheduleParallel();
    }
}