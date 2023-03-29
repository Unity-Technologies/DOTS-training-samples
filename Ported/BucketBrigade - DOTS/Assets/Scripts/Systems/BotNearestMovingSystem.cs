using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System.Collections;
using System.Collections.Generic;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;
using Unity.VisualScripting;
using UnityEngine;

[UpdateAfter(typeof(BotSpawningSystem))]
[UpdateAfter(typeof(WaterSpawningSystem))]
[UpdateAfter(typeof(GridTilesSpawningSystem))]
public partial struct BotNearestMovementSystem : ISystem
{
   
   private float3 backPos;
   private float3 frontPos;
   private float3 waterPos;
   private float3 firePos;
   private float speed;
   private float arriveThreshold;

   public void OnCreate(ref SystemState state)
   {
      state.RequireForUpdate<Config>();
   }
   
  
   public void OnUpdate(ref SystemState state)
   {
      float dt = SystemAPI.Time.DeltaTime;
      
      //Get the config 
      var config = SystemAPI.GetSingleton<Config>();
      speed = config.botSpeed;
      arriveThreshold = config.arriveThreshold;
      
      
      
      float minDist = float.MaxValue;
      //For one team
      foreach (var backTransform in SystemAPI.Query<LocalTransform>().WithAll<BackBotTag,Team>())
      {
         backPos = backTransform.Position;
      }
    
      //Get closest water
      foreach (var (water,waterTransform) in SystemAPI.Query<Water,LocalTransform>())
      {
         if (water.CurrCapacity > 0) //Check if it has water in it
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

      //Get closest fire to the back pos
      foreach (var fireTransform in SystemAPI.Query<LocalTransform>().WithAll<OnFire>())
      {
        
         var dist = Vector3.Distance(fireTransform.Position, backPos);
         if (dist < minDist)
         {
            minDist = dist; 
            firePos = fireTransform.Position;
         }
         
      }
      
      
      //Create the ECB's for adding the ReachedTarget Component 
      var ecbSingleton = SystemAPI.GetSingleton<BeginSimulationEntityCommandBufferSystem.Singleton>();
      var ecb = ecbSingleton.CreateCommandBuffer(state.WorldUnmanaged);

      //Debug.Log("Water: " + waterPos  + " Fire: " + firePos + " Front: "+ frontPos + " Back: "+backPos );
      //state.Enabled = false;
      
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
            arriveThreshold = arriveThreshold
         };


         JobHandle backJobHandle = backJob.Schedule(state.Dependency);
      
         var frontJob = new MoveToNearestFrontJob()
         {
            ECB = ecb,
            shouldSetReady = true,
            targetPos = firePos,
            deltaTime = dt,
            speed = speed,
            arriveThreshold = arriveThreshold
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
            arriveThreshold = arriveThreshold
         };


         JobHandle frontJobHandle = frontJob.Schedule(state.Dependency);
      
         var backJob = new MoveToNearestBackJob()
         {
            ECB = ecb,
            shouldSetReady = true,
            targetPos = waterPos,
            deltaTime = dt,
            speed = speed,
            arriveThreshold = arriveThreshold
         };


         JobHandle backJobHandle = backJob.Schedule(frontJobHandle);
      
      
         backJobHandle.Complete();
         
      }
      
      
      
   }
}

[WithAll(typeof(BackBotTag),typeof(Team))]
//This job will move the back bot to the water
public partial struct MoveToNearestBackJob : IJobEntity
{
   public EntityCommandBuffer ECB;
   public bool shouldSetReady;
   public float3 targetPos;
   public float deltaTime;
   public float speed;
   public float arriveThreshold;
  
 
   public void Execute(ref LocalTransform localTransform, Entity e)
   {
      float3 dir = Vector3.Normalize(targetPos - localTransform.Position);
      if (Vector3.Distance(targetPos ,localTransform.Position) > 1)
      {
         localTransform.Position = localTransform.Position + dir * deltaTime * speed;
      }
      else
      {
         if (shouldSetReady)
         {
            ECB.AddComponent(e, new TeamReadyTag());
            ECB.SetComponentEnabled<ReachedTarget>(e, true);
         }
      }
      
      
   }
}

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
  
 
   public void Execute(ref LocalTransform localTransform, Entity e)
   {
      float3 dir = Vector3.Normalize(targetPos - localTransform.Position);
      if (Vector3.Distance(targetPos ,localTransform.Position) > 1)
      {
         localTransform.Position = localTransform.Position + dir * deltaTime * speed;
      }
      else
      {
         if (shouldSetReady)
         {
           
            ECB.AddComponent(e, new TeamReadyTag());
            ECB.SetComponentEnabled<ReachedTarget>(e, true);
         }

      }

   }
}
   



