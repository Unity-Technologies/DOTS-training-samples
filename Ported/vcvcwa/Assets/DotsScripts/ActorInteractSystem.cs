using System.Reflection;
using System;
using Unity.Entities;
using Unity.Jobs;
using UnityEngine;
using Unity.Burst;
using Unity.Collections;
using Unity.Mathematics;
using Unity.Transforms;

public class ActorInteractSystem : JobComponentSystem
{
    private DynamicBuffer<GridTile> buffer;
    private EntityQuery query;

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

        // The [ReadOnly] attribute tells the job scheduler that this job will not write to rotSpeedIJobForEach
        public void Execute([ReadOnly] ref ActorMovementComponent actor, [ReadOnly] ref DotsIntentionComponent intention)
        {
            var atGoal = math.abs(actor.targetPosition.x - actor.position.x) < 0.5f;
            if (!atGoal)
            {
                return;
            }
            var gridTile = internalBuffer[Convert.ToInt32(actor.targetPosition.x) + (512 * Convert.ToInt32(actor.targetPosition.y))];
            if (intention.intention == DotsIntention.Rock && gridTile.IsRock())
            {
                //TODO insert bang rock in command buffer
            } 
            else if (intention.intention == DotsIntention.Till && gridTile.IsNothing())
            {
                //TODO insert till in command buffer
            } 
            else if (intention.intention == DotsIntention.Plant && gridTile.IsTilled())
            {
                //TODO insert plant in command buffer
            } 
            else if (intention.intention == DotsIntention.Harvest && gridTile.IsPlant() && gridTile.GetPlantHealth() > 75f)
            {
                //TODO insert harvest in command buffer
            }
            else if (intention.intention == DotsIntention.Shop && gridTile.IsShop())
            {
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
            internalBuffer = buffer
            //DeltaTime = Time.deltaTime
        };

        return job.Schedule(this, inputDependencies);
    }
}