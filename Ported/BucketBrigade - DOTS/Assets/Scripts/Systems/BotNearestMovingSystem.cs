
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

   [BurstCompile]
   public void OnCreate(ref SystemState state)
   {
      state.RequireForUpdate<Config>();
      state.RequireForUpdate<tileSpawnCompleteTag>();
      state.RequireForUpdate<updateBotNearestTag>();
      
   }
   
   [BurstCompile]
   public void OnUpdate(ref SystemState state)
   {
      float dt = SystemAPI.Time.DeltaTime;
      
      //Get the config 
      var config = SystemAPI.GetSingleton<Config>();
      speed = config.botSpeed;
      arriveThreshold = config.arriveThreshold;
      bucketCapacity = config.bucketCapacity;
      
      float minDist = float.MaxValue;
      //For one team
      foreach (var backTransform in SystemAPI.Query<LocalTransform>().WithAll<BackBotTag,Team>())
      {
         backPos = backTransform.Position;
      }
    
      
      //If the team has a bucket we should check if it is being filled before moving
      EntityQuery fillingBuckets = SystemAPI.QueryBuilder().WithAll<FillingTag, Team>().Build();
      
      
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
      foreach (var frontTransform in SystemAPI.Query<LocalTransform>().WithAll<FrontBotTag,Team>())
      {
         frontPos = frontTransform.Position;
      }
      

      EntityQuery fireQ = SystemAPI.QueryBuilder().WithAll<LocalTransform, OnFire>().Build();
      NativeArray<LocalTransform> fireTransforms = fireQ.ToComponentDataArray<LocalTransform>(Allocator.Temp);

      int numFire = fireTransforms.Length;
      for (int i = 0; i < numFire; i++)
      {
         var dist = Vector3.Distance(fireTransforms[i].Position, backPos);
         if (dist < minDist)
         {
            minDist = dist; 
            firePos = fireTransforms[i].Position;
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
            transitionManager = TransitionManager
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
            transitionManager = TransitionManager
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
            transitionManager = TransitionManager
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
            transitionManager = TransitionManager
         };


         JobHandle backJobHandle = backJob.Schedule(frontJobHandle);
      
      
         backJobHandle.Complete();
         
      }
      
      
      
   }
}

[WithAll(typeof(BackBotTag),typeof(Team))]
[BurstCompile]

//BUG: Using the following in conjunction with EnabledRefRW<ReachedTarget> causes an error 
[WithDisabled(typeof(ReachedTarget))]


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
  
 
   public void Execute(ref LocalTransform localTransform, Entity e, EnabledRefRW<ReachedTarget> reachedState)
   {
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
[WithAll(typeof(FrontBotTag),typeof(Team))]
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
  
 
   public void Execute(ref LocalTransform localTransform, Entity e)
   {
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
   



