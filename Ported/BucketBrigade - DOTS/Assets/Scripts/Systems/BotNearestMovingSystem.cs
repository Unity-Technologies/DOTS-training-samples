
using Unity.Burst;
using Unity.Collections;
using UnityEngine;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;

[UpdateInGroup(typeof(MovementSystemGroup))]
[UpdateAfter(typeof(FireHandlingSystem))]
[BurstCompile]
public partial struct BotNearestMovementSystem : ISystem
{
   
   
   private float speed;
   private float arriveThreshold;
   private float bucketCapacity;
   private NativeList<Team> teamList;
   private int numTeams;
   private EntityCommandBuffer ecb;
   private ComponentLookup<TeamReadyTag> teamReadyTagComponentLookup;
   private bool hasCreatedTeamList;
   
  

   [BurstCompile]
   public void OnCreate(ref SystemState state)
   {
      state.RequireForUpdate<Config>();
      state.RequireForUpdate<tileSpawnCompleteTag>();


      //Initialize team list
      teamList = new NativeList<Team>(1, Allocator.Persistent);
      teamReadyTagComponentLookup = SystemAPI.GetComponentLookup<TeamReadyTag>();
      hasCreatedTeamList = false;

   }

   public void OnDestroy(ref SystemState state)
   {
      teamList.Dispose();
   }

   [BurstCompile]
   public void OnUpdate(ref SystemState state)
   {
      ecb = SystemAPI.GetSingleton<BeginSimulationEntityCommandBufferSystem.Singleton>()
         .CreateCommandBuffer(state.WorldUnmanaged);
      
      
      //Get the config 
      var config = SystemAPI.GetSingleton<Config>();
      speed = config.botSpeed;
      arriveThreshold = config.arriveThreshold;
      bucketCapacity = config.bucketCapacity;
      numTeams = config.TotalTeams;
      
      //Update the component lookup
      teamReadyTagComponentLookup.Update(ref state);
      
      
      //Get the front and back bots in one query
      EntityQuery backBotsQ = SystemAPI.QueryBuilder().WithAll<LocalTransform, BackBotTag, Team>().Build();
      EntityQuery frontBotsQ = SystemAPI.QueryBuilder().WithAll<LocalTransform, FrontBotTag, Team>().Build();
      
      //Get component for each team
      for (int t = 0; t < numTeams && !hasCreatedTeamList; t++)
      {
         var TeamComponent = new Team { Value = t};
      
         teamList.Add(TeamComponent);
         if (t == numTeams - 1)
         {
            hasCreatedTeamList = true;
         }
      }
      
      //Add gigantic for loop to handle each team 
      for (int i = 0; i < teamList.Length; i++)
      {
         float3 waterPos = float3.zero;
         float3 firePos = float3.zero;
         Entity frontBot = Entity.Null;
         Entity backBot = Entity.Null;
         LocalTransform frontBotTransform = LocalTransform.Identity;
         LocalTransform backBotTransform=LocalTransform.Identity;
         float minDist = float.MaxValue;
         float dt = SystemAPI.Time.DeltaTime;

         backBotsQ.SetSharedComponentFilter(teamList[i]);
         frontBotsQ.SetSharedComponentFilter(teamList[i]);

         backBotTransform = backBotsQ.ToComponentDataArray<LocalTransform>(Allocator.Temp)[0];
         backBot = backBotsQ.ToEntityArray(Allocator.Temp)[0];
         
         frontBotTransform = frontBotsQ.ToComponentDataArray<LocalTransform>(Allocator.Temp)[0];
         frontBot = frontBotsQ.ToEntityArray(Allocator.Temp)[0];
         
         //Check if we should skip to the next team
         //ComponentLookup<TeamReadyTag> readyTeams = SystemAPI.GetComponentLookup<TeamReadyTag>();
         if (teamReadyTagComponentLookup.IsComponentEnabled(frontBot) && teamReadyTagComponentLookup.IsComponentEnabled(backBot))
         {
            //If both the front and back bot are at their goal position we should not update 
            continue;
         }
       
         
         //If the team has a bucket we should check if it is being filled before moving
         EntityQuery fillingBuckets = SystemAPI.QueryBuilder().WithAll<FillingTag,Team>().Build();
         fillingBuckets.SetSharedComponentFilter(teamList[i]);
         //Get closest water
         foreach (var (water,waterTransform) in SystemAPI.Query<Water,LocalTransform>())
         {
            
            if (water.CurrCapacity >= bucketCapacity && fillingBuckets.CalculateEntityCount() == 0) //Check if it has water in it
            {
               var dist = Vector3.Distance(waterTransform.Position, backBotTransform.Position);
               if (dist < minDist)
               {
                  minDist = dist;
                  waterPos = waterTransform.Position;
               }
            }
         }
         
         
         
         //Reset value
         minDist = float.MaxValue;


         //Get closest fire to the back pos
         var fireTransformQ = SystemAPI.QueryBuilder().WithAll<LocalTransform,OnFire>().Build();
         var fireTransforms = fireTransformQ.ToComponentDataArray<LocalTransform>(Allocator.Temp);
         for (int f = 0; f < fireTransforms.Length; f++)
         {
            var dist = Vector3.Distance(fireTransforms[f].Position, backBotTransform.Position);
            if (dist < minDist )
            {
               minDist = dist; 
               firePos = fireTransforms[f].Position;
            } 
         }
         
         if (firePos.Equals(float3.zero))
         {
            continue;
         }
         
         if (waterPos.Equals(float3.zero))
         {
            continue;
         }


         
         
         //Move the Initial Bots 
         if (!teamReadyTagComponentLookup.IsComponentEnabled(backBot))
         {
            
            waterPos.y = 0.25f;
            float3 dir = Vector3.Normalize(waterPos - backBotTransform.Position);
            if (Vector3.Distance(waterPos ,backBotTransform.Position) > arriveThreshold)
            {
               backBotTransform.Position +=  dir * dt * speed;
               state.EntityManager.SetComponentData(backBot, backBotTransform);
            }
            else
            {
               teamReadyTagComponentLookup.SetComponentEnabled(backBot, true);
            }

         }
         
         teamReadyTagComponentLookup.Update(ref state);

         if(!teamReadyTagComponentLookup.IsComponentEnabled(frontBot))
         {
            firePos.y = 0.25f;
            float3 dir = Vector3.Normalize(firePos - frontBotTransform.Position);
            if (Vector3.Distance(firePos ,frontBotTransform.Position) > arriveThreshold)
            {
               frontBotTransform.Position += dir * dt * speed;
               state.EntityManager.SetComponentData(frontBot, frontBotTransform);
            }
            else
            {
               teamReadyTagComponentLookup.SetComponentEnabled(frontBot, true);
            }
            
         }
         
         
            
         fireTransforms.Dispose();
      }

   }
}


[WithAll(typeof(BackBotTag))]
[BurstCompile]
public partial struct MoveToNearestBackJob : IJobEntity
{
   public EntityCommandBuffer ECB;
   public bool shouldSetReady;
   public float3 targetPos;
   public float deltaTime;
   public float speed;
   public float arriveThreshold;
   public Entity transitionManager;
   public int teamNo;
  
 
   public void Execute(ref LocalTransform localTransform, Entity e, in Team team)
   {
      if (team.Value != teamNo)
      {
         return;
      }
      float3 dir = Vector3.Normalize(targetPos - localTransform.Position);
      if (Vector3.Distance(targetPos ,localTransform.Position) > arriveThreshold)
      {
         localTransform.Position = localTransform.Position + dir * deltaTime * speed;
      }
      else
      {
         ECB.SetComponentEnabled<ReachedTarget>(e, true);
         if (shouldSetReady)
         {
            ECB.AddComponent(e, new TeamReadyTag());
            ECB.RemoveComponent<updateBotNearestTag>(transitionManager);
            
         }
      }
      
      
   }
}
[BurstCompile]

[WithAll(typeof(FrontBotTag))]

//This job will move the front bot to the fire
public partial struct MoveToNearestFrontJob : IJobEntity
{
   
   public EntityCommandBuffer ECB;
   public bool shouldSetReady;
   public float3 targetPos;
   public float deltaTime;
   public float speed;
   public float arriveThreshold;
   public Entity transitionManager;
   public int teamNo;
  
 
   public void Execute(ref LocalTransform localTransform, Entity e, in Team team)
   {
      if (team.Value != teamNo)
      {
         return;
      }
      targetPos.y = 0.25f;
      float3 dir = Vector3.Normalize(targetPos - localTransform.Position);
      if (Vector3.Distance(targetPos ,localTransform.Position) > arriveThreshold)
      {
         localTransform.Position = localTransform.Position + dir * deltaTime * speed;
      }
      else
      {
         ECB.SetComponentEnabled<ReachedTarget>(e, true);
         if (shouldSetReady)
         {
           
            ECB.AddComponent(e, new TeamReadyTag());
            ECB.RemoveComponent<updateBotNearestTag>(transitionManager);

         }

      }

   }
}


public partial struct MoveToTarget: IJob
{
   
   public EntityCommandBuffer ECB;

   public Entity bot;
   public LocalTransform botTransform;
   public float3 targetPos;
   
   public float deltaTime;
   public float speed;
   public float arriveThreshold;

   
   public void Execute()
   {
      
      targetPos.y = 0.25f;
      float3 dir = Vector3.Normalize(targetPos - botTransform.Position);
      if (Vector3.Distance(targetPos ,botTransform.Position) > arriveThreshold)
      {
         botTransform.Position += dir * deltaTime * speed;
         ECB.SetComponent(bot, botTransform);
      }
      else
      {
         ECB.SetComponentEnabled<ReachedTarget>(bot, true);
         //Indicate that one of our bots are ready
         ECB.SetComponentEnabled<TeamReadyTag>(bot, true);
      }

   }
}



[WithAny(typeof(FrontBotTag),typeof(BackBotTag))]
[WithNone(typeof(TeamReadyTag))]
public partial struct MoveToNearestEJob : IJobEntity
{
   
   public EntityCommandBuffer ECB;
   public float3 targetPos;
   public float deltaTime;
   public float speed;
   public float arriveThreshold;
   public int teamNo;
  
 
   public void Execute(ref LocalTransform localTransform, Entity e, in Team team)
   {
      if (team.Value != teamNo)
      {
         return;
      }
      targetPos.y = 0.25f;
      float3 dir = Vector3.Normalize(targetPos - localTransform.Position);
      if (Vector3.Distance(targetPos ,localTransform.Position) > arriveThreshold)
      {
         localTransform.Position = localTransform.Position + dir * deltaTime * speed;
      }
      else
      {
         ECB.SetComponentEnabled<ReachedTarget>(e, true);
         ECB.AddComponent(e, new TeamReadyTag());
            
         

      }

   }
}



   



