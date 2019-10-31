using System;
using Unity.Entities;
using Unity.Jobs;
using UnityEngine;
using Unity.Burst;
using Unity.Collections;
using Unity.Mathematics;

public class ActorInteractSystem : JobComponentSystem
{
    DynamicBuffer<GridTile> buffer;
    EntityQuery query;
    NativeQueue<GridOperation> gridOperations;

    protected override void OnCreate()
    {
        EntityQueryDesc queryDescription = new EntityQueryDesc();
        queryDescription.All = new[] {ComponentType.ReadOnly<GridTile>()};
        query = GetEntityQuery(queryDescription);
    }

    [BurstCompile]
    struct GoalPositionJob : IJobForEach<ActorMovementComponent, DotsIntentionComponent>
    {
        [ReadOnly] public DynamicBuffer<GridTile> internalBuffer;
        [ReadOnly] public NativeQueue<GridOperation> internalGridOperations;

        // The [ReadOnly] attribute tells the job scheduler that this job will not write to rotSpeedIJobForEach
        public void Execute([ReadOnly] ref ActorMovementComponent actor, ref DotsIntentionComponent intention)
        {
            var atGoal = math.abs(actor.targetPosition.x - actor.position.x) < 0.5f;
            if (!atGoal)
            {
                return;
            }
            var gridTile = internalBuffer[Convert.ToInt32(actor.targetPosition.x) + (512 * Convert.ToInt32(actor.targetPosition.y))];
            if (intention.intention == DotsIntention.Rock && gridTile.IsRock())
            {
                intention.intention = DotsIntention.RockFinished;
                internalGridOperations.Enqueue(new GridOperation(){actor = actor.actor, gridTile = gridTile, desiredGridValue = gridTile.Value - 2});
            } 
            else if (intention.intention == DotsIntention.Till && gridTile.IsNothing())
            {
                intention.intention = DotsIntention.TillFinished;
                internalGridOperations.Enqueue(new GridOperation(){actor = actor.actor, gridTile = gridTile, desiredGridValue = 1});
            } 
            else if (intention.intention == DotsIntention.Plant && gridTile.IsTilled())
            {
                intention.intention = DotsIntention.PlantFinished;
                internalGridOperations.Enqueue(new GridOperation(){actor = actor.actor, gridTile = gridTile, desiredGridValue = 3});
            } 
            else if (intention.intention == DotsIntention.Harvest && gridTile.IsPlant() && gridTile.GetPlantHealth() > 75f)
            {
                intention.intention = DotsIntention.HarvestFinished;
                internalGridOperations.Enqueue(new GridOperation(){actor = actor.actor, gridTile = gridTile, desiredGridValue = 1});
            }
            else if (intention.intention == DotsIntention.Shop && gridTile.IsShop())
            {
                intention.intention = DotsIntention.ShopFinished;

                //TODO insert shop in command buffer
            }
        }
    }


// OnUpdate runs on the main thread.
    protected override JobHandle OnUpdate(JobHandle inputDependencies)
    {
        Entity farm = query.GetSingletonEntity();
        buffer = EntityManager.GetBuffer<GridTile>(farm);
        
        var job = new GoalPositionJob
        {
            internalBuffer = buffer,
            internalGridOperations = gridOperations
        };

        return job.Schedule(this, inputDependencies);
    }
}