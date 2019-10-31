using System;
using Unity.Entities;
using UnityEngine;
using Unity.Jobs;

public class IntentionSystem : JobComponentSystem
{
    protected override JobHandle OnUpdate(JobHandle inputDependencies)
    {
        var job1Handle = Entities
            .ForEach((ref DotsIntentionComponent dotsIntention, in MoveComponent moveComponent) =>
            {
                if (moveComponent.fly)
                {
                    switch (dotsIntention.intention)
                    {
                        case DotsIntention.HarvestFinished:
                            dotsIntention.intention = DotsIntention.Shop;
                            break;
                        case DotsIntention.ShopFinished:
                            dotsIntention.intention = DotsIntention.Harvest;
                            break;
                    }
                }
                else
                {
                    switch (dotsIntention.intention)
                    {
                        case DotsIntention.RockFinished:
                            dotsIntention.intention = DotsIntention.Till;
                            break;
                        case DotsIntention.TillFinished:
                            dotsIntention.intention = DotsIntention.Plant;
                            break;
                        case DotsIntention.PlantFinished:
                            dotsIntention.intention = DotsIntention.Harvest;
                            break;
                        case DotsIntention.HarvestFinished:
                            dotsIntention.intention = DotsIntention.Shop;
                            break;
                        case DotsIntention.ShopFinished:
                            dotsIntention.intention = DotsIntention.Rock;
                            break;
                    }
                }
            })
            .Schedule(inputDependencies);

        // Return job's handle as the dependency for this system
        return job1Handle;
    }
}