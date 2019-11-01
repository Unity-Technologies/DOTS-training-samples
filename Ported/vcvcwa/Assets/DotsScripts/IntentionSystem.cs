using System;
using Unity.Entities;
using UnityEngine;
using Unity.Jobs;
using Unity.Mathematics;

public class IntentionSystem : JobComponentSystem
{
    protected override JobHandle OnUpdate(JobHandle inputDependencies)
    {
        var job1Handle = Entities
            .ForEach((ref ActorMovementComponent actor, ref DotsIntentionComponent dotsIntention, in MoveComponent moveComponent) =>
            {
                if (moveComponent.fly)
                {
                    switch (dotsIntention.intention)
                    {
                        case DotsIntention.HarvestFinished:
                            dotsIntention.intention = DotsIntention.Shop;
                            actor.targetPosition = new float2(-1, -1);
                            break;
                        case DotsIntention.ShopFinished:
                            dotsIntention.intention = DotsIntention.Harvest;
                            actor.targetPosition = new float2(-1, -1);
                            break;
                    }
                }
                else
                {
                    switch (dotsIntention.intention)
                    {
                        case DotsIntention.RockFinished:
                            dotsIntention.intention = DotsIntention.Till;
                            actor.targetPosition = new float2(-1, -1);
                            break;
                        case DotsIntention.TillFinished:
                            dotsIntention.intention = DotsIntention.Plant;
                            actor.targetPosition = new float2(-1, -1);
                            break;
                        case DotsIntention.PlantFinished:
                            dotsIntention.intention = DotsIntention.Harvest;
                            actor.targetPosition = new float2(-1, -1);
                            break;
                        case DotsIntention.HarvestFinished:
                            dotsIntention.intention = DotsIntention.Shop;
                            actor.targetPosition = new float2(-1, -1);
                            break;
                        case DotsIntention.ShopFinished:
                            dotsIntention.intention = DotsIntention.Rock;
                            actor.targetPosition = new float2(-1, -1);
                            break;
                    }
                }
            })
            .Schedule(inputDependencies);

        // Return job's handle as the dependency for this system
        return job1Handle;
    }
}