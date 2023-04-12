
using Unity.Burst;
using Unity.Collections;
using UnityEngine;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;

[UpdateAfter(typeof(FireHandlingSystem))]
[BurstCompile]
public partial struct BotNearestMovementSystem : ISystem
{
   
   private float3 backPos;
   private float3 frontPos;
   private float3 waterPos;
   private float3 firePos;
   private float speed;
   private float arriveThreshold;
   private float bucketCapacity;
   private NativeList<Team> teamList;

   [BurstCompile]
   public void OnCreate(ref SystemState state)
   {
      state.RequireForUpdate<Config>();
      state.RequireForUpdate<tileSpawnCompleteTag>();
      state.RequireForUpdate<updateBotNearestTag>();
      
      

   }

   public void OnDestroy(ref SystemState state)
   {
      teamList.Dispose();
   }

   [BurstCompile]
   public void OnUpdate(ref SystemState state)
   {
      //Get all teams
      teamList = new NativeList<Team>(1, Allocator.Persistent);
      
      //Get component for each team
      var TeamComponent1 = new Team { Value = 1};
      
      teamList.Add(TeamComponent1);

      //Add gigantic for loop to handle each team 
      for (int i = 0; i < teamList.Length; i++)
      {
         float dt = SystemAPI.Time.DeltaTime;
      
         //Get the config 
         var config = SystemAPI.GetSingleton<Config>();
         speed = config.botSpeed;
         arriveThreshold = config.arriveThreshold;
         bucketCapacity = config.bucketCapacity;
         
         float minDist = float.MaxValue;
         //For one team
         foreach (var backTransform in SystemAPI.Query<LocalTransform>().WithAll<BackBotTag,Team>().WithSharedComponentFilter(teamList[i]))
         {
            backPos = backTransform.Position;
         }
       
         
         //If the team has a bucket we should check if it is being filled before moving
         EntityQuery fillingBuckets = SystemAPI.QueryBuilder().WithAll<FillingTag,Team>().Build();
         fillingBuckets.SetSharedComponentFilter(teamList[i]);
         //Get closest water
         foreach (var (water,waterTransform) in SystemAPI.Query<Water,LocalTransform>())
         {
            
            if (water.CurrCapacity >= bucketCapacity && fillingBuckets.CalculateEntityCount() == 0) //Check if it has water in it
            {
               var dist = Vector3.Distance(waterTransform.Position, backPos);
               if (dist < minDist)
               {
                  minDist = dist;
                  waterPos = waterTransform.Position;
               }
            }
         }
         
         //Reset value
         minDist = float.MaxValue;
         //Get thrower guy
         foreach (var frontTransform in SystemAPI.Query<LocalTransform>().WithAll<FrontBotTag,Team>().WithSharedComponentFilter(teamList[i]))
         {
            frontPos = frontTransform.Position;
         }
         

         EntityQuery fireQ = SystemAPI.QueryBuilder().WithAll<LocalTransform, OnFire>().Build();
         NativeArray<LocalTransform> fireTransforms = fireQ.ToComponentDataArray<LocalTransform>(Allocator.Temp);

         int numFire = fireTransforms.Length;
         for (int j = 0; j < numFire; j++)
         {
            var dist = Vector3.Distance(fireTransforms[j].Position, backPos);
            if (dist < minDist)
            {
               minDist = dist; 
               firePos = fireTransforms[j].Position;
            }
         }

         fireTransforms.Dispose();
             //Get closest fire to the back pos
         /*foreach (var fireTransform in SystemAPI.Query<LocalTransform>().WithAll<OnFire>())
         {
            
            var dist = Vector3.Distance(fireTransform.Position, backPos);
            if (dist < minDist)
            {
               minDist = dist; 
               firePos = fireTransform.Position;
            }
         }

        */
         if (firePos.Equals(float3.zero))
         {
            return;
         }
         
         
         //Create the ECB's for adding the ReachedTarget Component 
         var ecbSingleton = SystemAPI.GetSingleton<BeginSimulationEntityCommandBufferSystem.Singleton>();
         var ecb = ecbSingleton.CreateCommandBuffer(state.WorldUnmanaged);
         
         var TransitionManager = SystemAPI.GetSingletonEntity<Transition>();
         
         //Make the slowest one be the one that determines the end 
         if (Vector3.Distance(waterPos, backPos) < Vector3.Distance(firePos, frontPos))
         {
            //Move the Initial Bots 
            var backJob = new MoveToNearestBackJob()
            {
               ECB = ecb,
               shouldSetReady = false,
               targetPos = waterPos,
               deltaTime = dt,
               speed = speed,
               arriveThreshold = arriveThreshold,
               transitionManager = TransitionManager,
               teamNo = teamList[i].Value
            };


            JobHandle backJobHandle = backJob.Schedule(state.Dependency);
         
            var frontJob = new MoveToNearestFrontJob()
            {
               ECB = ecb,
               shouldSetReady = true,
               targetPos = firePos,
               deltaTime = dt,
               speed = speed,
               arriveThreshold = arriveThreshold,
               transitionManager = TransitionManager,
               teamNo = teamList[i].Value
            };


            JobHandle frontJobHandle = frontJob.Schedule(backJobHandle);
         
            
            frontJobHandle.Complete();
         }
         else
         {
            //Move the Initial Bots 
            var frontJob = new MoveToNearestFrontJob()
            {
               ECB = ecb,
               shouldSetReady = false,
               targetPos = firePos,
               deltaTime = dt,
               speed = speed,
               arriveThreshold = arriveThreshold,
               transitionManager = TransitionManager,
               teamNo = teamList[i].Value
            };


            JobHandle frontJobHandle = frontJob.Schedule(state.Dependency);
         
            var backJob = new MoveToNearestBackJob()
            {
               ECB = ecb,
               shouldSetReady = true,
               targetPos = waterPos,
               deltaTime = dt,
               speed = speed,
               arriveThreshold = arriveThreshold,
               transitionManager = TransitionManager,
               teamNo = teamList[i].Value
            };


            JobHandle backJobHandle = backJob.Schedule(frontJobHandle);
         
         
            backJobHandle.Complete();
         
         }
      
      }
   
   }
}

[WithAll(typeof(BackBotTag))]
[BurstCompile]
//[WithDisabled(typeof(ReachedTarget))]

//[WithOptions(EntityQueryOptions.IncludeDisabledEntities)]
//This job will move the back bot to the water
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
   



