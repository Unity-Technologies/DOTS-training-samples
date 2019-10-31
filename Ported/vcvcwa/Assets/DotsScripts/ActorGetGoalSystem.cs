using System.Reflection;
using Unity.Entities;
using Unity.Jobs;
using UnityEngine;
using Unity.Burst;
using Unity.Collections;
using Unity.Transforms;

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
            //If we have no goalPosition we need to find another one
            if (actor.targetPosition.x < 0.0f || actor.targetPosition.y < 0.0f)
            {
                Vector2 closestIntention = Vector2.one * -1;
                float currentDistance = -1.0f;

                /*for (int i = 0; i < 512 * 512; i++) //hardcoded for now but need to change that!
                {
                    if (internalBuffer[i].Value == (int)intention.intention)
                    {
                        var distanceTemp = Vector2.Distance(pos.position, new Vector2(i % 512, i / 512.0f));
                        
                        //If currentDistance not set yet
                        if (currentDistance <= -1.0f)
                        {
                            currentDistance = distanceTemp;
                        }
                        //If the new distance is closest than the old one
                        if (currentDistance > distanceTemp)
                        {
                            currentDistance = distanceTemp;
                        }
                    }
                }*/

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
                            if (i < 0 || i > 511 || j < 0 || j > 511)
                            {
                                continue;
                            }

                            if ((intention.intention == DotsIntention.Rock && internalBuffer[i + (j * 512)].IsRock())
                                || (intention.intention == DotsIntention.Plant && internalBuffer[i + (j * 512)].IsTilled())
                                || (intention.intention == DotsIntention.Till && internalBuffer[i + (j * 512)].IsNothing())
                                || (intention.intention == DotsIntention.Shop && internalBuffer[i + (j * 512)].IsShop())
                                || (intention.intention == DotsIntention.Harvest && internalBuffer[i + (j * 512)].IsPlant()
                                                                                 && internalBuffer[i + (j * 512)].GetPlantHealth() >= 75))

                            {
                                closestIntention = new Vector2(i, y);
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

                if (closestIntention == Vector2.one * -1)
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

    /* protected override JobHandle OnUpdate(JobHandle inputDependencies)
     {
        
         float dt = Time.deltaTime;
        
         
         var job1Handle = Entities
             .ForEach((ref GoalPositionComponent goalposition, in PositionComponent position, in DotsIntentionComponent intention) =>
             {
                 //If we have no goalPosition we need to find another one
                     if (goalposition.position == Vector2.one * -1)
                 {
                     var entityIndex = 0;
                     var gridIndex = 0;
                     
                         for (int i = gridIndex; i < 512 * 512; i++) //hardcoded for now but need to change that!
                     {
                         if (buffer[i].Value == (int)intention.intention)
                         {
                             //check la distance, a la fin on prend le plus pres? si yen n'a pas on change d'intention!
                             var x = i % 512;
                             var y = i / 512;
                         
                             entityIndex++;
                             gridIndex = i;
                             goalposition.position = new Vector2(x, y);
                             break;
                         }
                     }
                 
                 }
             })
             .Schedule(inputDependencies);
 
         // Return job's handle as the dependency for this system
         return job1Handle;
     }*/


    /*struct SearchGoalPositionJob : IJobForEachWithEntity_EBCCC<GridTile, GridComponent, GoalPositionComponent, DotsIntentionComponent>
   { 

       public void Execute(Entity entity, int index, DynamicBuffer<GridTile> gridTileBuffer,
           ref GoalPositionComponent goalPosition, ref DotsIntentionComponent intention, ref GridComponent gridComponent)
       {
           var entityIndex = 0;
           var gridIndex = 0;

           //If we have no goalPosition we need to find another one
           if (goalPosition.position == new Vector2(-1.0f, -1.0f))
           {
               
               
           }
          while (resourcesComponent.MoneyForFarmers >= 10)
           {
               for (int i = gridIndex; i < gridComponent.Size * gridComponent.Size; i++)
               {
                   if (gridTileBuffer[i].IsShop())
                   {
                       var x = i % gridComponent.Size;
                       var y = i / gridComponent.Size;
                       
                       resourcesComponent.MoneyForFarmers -= 10;
                       entityIndex++;
                       gridIndex = i;
                       break;
                   }
               }

               if (gridIndex == (gridComponent.Size * gridComponent.Size) - 1)
               {
                   gridIndex = 0;
               }
           }
       }
   }*/
}