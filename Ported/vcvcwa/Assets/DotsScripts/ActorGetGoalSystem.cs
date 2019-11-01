using Unity.Entities;
using Unity.Jobs;
using Unity.Burst;
using Unity.Collections;
using Unity.Mathematics;
using System;

public class ActorGetGoalSystem : JobComponentSystem
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
        public void Execute(ref ActorMovementComponent actor, ref DotsIntentionComponent intention)
        {
            float size = internalBuffer.Length;
            //If we have no goalPosition we need to find another one
            if (actor.targetPosition.x < 0.0f || actor.targetPosition.y < 0.0f)
            {
                float2 closestIntention = new float2(-1,-1);

                int DeltaMax = 5;
                int Delta = 0;
                int x = (int) actor.position.x;
                int y = (int) actor.position.y;
                bool done = false;

                while (!done)
                {
                    for (int i = x - Delta; i <= x + Delta; i++)
                    {
                        for (int j = y - Delta; j <= y + Delta; j++)
                        {
                           // if (i < 0 || i > size || j < 0 || j > 511)
                           var testIndex = (i*512) + j;
                           if ( testIndex > size || testIndex < 0)
                           {
                               continue;
                           }

                           if ((intention.intention == DotsIntention.Rock && internalBuffer[testIndex].IsRock())
                               || (intention.intention == DotsIntention.Plant && internalBuffer[testIndex].IsTilled())
                               || (intention.intention == DotsIntention.Till && internalBuffer[testIndex].IsNothing())
                               || (intention.intention == DotsIntention.Shop && internalBuffer[testIndex].IsShop())
                               || (intention.intention == DotsIntention.Harvest && internalBuffer[testIndex].IsPlant()
                                   && internalBuffer[testIndex].GetPlantHealth() >= 75))

                           {
                               closestIntention = new float2(i, j);
                               done = true;
                           }
                        }
                    }

                    if (!done && Delta < DeltaMax)
                    {
                        Delta++;
                    }
                    else
                    {
                        done = true;
                    }
                }

                if (closestIntention.x < 0 && closestIntention.y < 0)
                {
                    intention.intention += 1;
                }
                else
                {
                    actor.targetPosition = closestIntention;
                }
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