using System;
using System.Collections.Concurrent;
using Unity.Entities;
using Unity.Jobs;
using UnityEngine;
using Unity.Burst;
using Unity.Collections;
using Unity.Mathematics;

public class ActorInteractSystem : JobComponentSystem
{
    DynamicBuffer<GridTile> bufferReading;
    DynamicBuffer<GridTile> bufferWriting;
    EntityQuery queryReading;
    EntityQuery queryWriting;
    NativeQueue<GridOperation> gridOperations;

    protected override void OnCreate()
    {
        EntityQueryDesc queryDescription = new EntityQueryDesc();
        queryDescription.All = new[] {ComponentType.ReadOnly<GridTile>()};
        queryReading = GetEntityQuery(queryDescription);
        
        queryDescription.All = new[] {ComponentType.ReadWrite<GridTile>()};
        queryWriting = GetEntityQuery(queryDescription);
        
        gridOperations = new NativeQueue<GridOperation>(Allocator.Persistent);
    }
    [BurstCompile]
    struct InteractJob : IJobForEach<ActorMovementComponent, DotsIntentionComponent>
    {
        [ReadOnly] public DynamicBuffer<GridTile> internalBuffer;
        public NativeQueue<GridOperation>.ParallelWriter internalGridOperations;

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

    [BurstCompile]
    struct ChangeTilesValueJob : IJob
    {
        public DynamicBuffer<GridTile> internalBuffer;
         public NativeQueue<GridOperation> internalGridOperations;

        public void Execute()
        {
            while (internalGridOperations.TryDequeue(out var v))
            {
                var grid =  internalBuffer[v.gridTile];
                grid.Value = v.desiredGridValue;
                internalBuffer[v.gridTile] = grid;
            }
        }

    }
    

// OnUpdate runs on the main thread.
    protected override JobHandle OnUpdate(JobHandle inputDependencies)
    {
        Entity farm = queryReading.GetSingletonEntity();
        bufferReading = EntityManager.GetBuffer<GridTile>(farm);
        bufferWriting = EntityManager.GetBuffer<GridTile>(farm);
        
        var job1 = new InteractJob
        {
            internalBuffer = bufferReading,
            internalGridOperations = gridOperations.AsParallelWriter()
        };
        var job1Handle = job1.Schedule(this, inputDependencies);

        
        var job2 = new ChangeTilesValueJob()
        {
            internalBuffer = bufferWriting,
            internalGridOperations = gridOperations
        };

        return job2.Schedule(job1Handle);

    }
}