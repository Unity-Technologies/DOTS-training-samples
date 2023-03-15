using System.Collections;
using System.Collections.Generic;
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
public partial struct BotMovementSystem : ISystem
{
   private NativeArray<LocalTransform> forwardTransform;
   private NativeArray<LocalTransform> backwardTransform;
   private float3 backPos;
   private float3 frontPos;
   private float3 waterPos;
   private float3 firePos;
   private float speed;
   private float totalNumberOfBots;
   private float arriveThreshold;

   public void OnCreate(ref SystemState state)
   {
      state.RequireForUpdate<Config>();
   }

   public void OnUpdate(ref SystemState state)
   {
      
      //Get the config 
      var config = SystemAPI.GetSingleton<Config>();
      speed = config.botSpeed;
      totalNumberOfBots = config.TotalBots;
      arriveThreshold = config.arriveThreshold;
      
      
      
      
      
      float minDist = float.MaxValue;
      //For one team
      
      //Get Back guy position
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

      Debug.Log("Water: " + waterPos  + " Fire: " + firePos + " Front: "+ frontPos + " Back: "+backPos );
      //state.Enabled = false;
      
      //Query the rest of the bots
      EntityQuery forwardBotsQ = state.GetEntityQuery(typeof(ForwardPassingBotTag), typeof(LocalTransform));
      EntityQuery backwardBotsQ = state.GetEntityQuery(typeof(BackwardPassingBotTag),typeof(LocalTransform));
      
      //Turn them into a native array 
      var forwardBots = forwardBotsQ.ToEntityArray(Allocator.TempJob);
      var backwardBots =  backwardBotsQ.ToEntityArray(Allocator.TempJob);
      
      forwardTransform = forwardBotsQ.ToComponentDataArray<LocalTransform>(Allocator.TempJob);
      backwardTransform = backwardBotsQ.ToComponentDataArray<LocalTransform>(Allocator.TempJob);

      
      float dt = SystemAPI.Time.DeltaTime;
      //Use job to distribute the bots (forward)
      var forwardJob = new MoveBotJob
      {
         front = frontPos, 
         back = backPos,
         totalNumberBots = totalNumberOfBots/2,
         localTransform = forwardTransform,
         deltaTime = dt,
         speed =  speed,
         arriveThreshold = arriveThreshold

      };

      var backwardJob = new MoveBotJob
      {
         front = backPos,
         back = frontPos,
         totalNumberBots = totalNumberOfBots / 2,
         localTransform = backwardTransform,
         deltaTime = dt,
         speed = speed,
         arriveThreshold = arriveThreshold

      };


      JobHandle forwardHandle = forwardJob.Schedule(forwardTransform.Length,2);
      JobHandle backwardHandle = backwardJob.Schedule(backwardTransform.Length,2);

      forwardHandle.Complete();
      backwardHandle.Complete();
      forwardBotsQ.CopyFromComponentDataArray(forwardTransform);
      backwardBotsQ.CopyFromComponentDataArray(backwardTransform);

   }
}

public partial struct MoveBotJob : IJobParallelFor
{
   public float3 front;
   public float3 back;
   public float totalNumberBots;
   public NativeArray<LocalTransform> localTransform;
   public float deltaTime;
   public float speed;
   public float arriveThreshold;
   public bool isForward;
   
   public void Execute(int index)
   {
      LocalTransform transform = localTransform[index];
      
      
      float progress = (float)index / totalNumberBots;
      float curveOffset = Mathf.Sin(progress * Mathf.PI) * 1f;

      // get Vec2 data
      Vector2 heading = new Vector2(front.x, front.z) -  new Vector2(back.x, back.y);
      float distance = heading.magnitude;
      Vector2 direction = heading / distance;
      Vector2 perpendicular = new Vector2(direction.y, -direction.x);

      Debug.Log("chain progress: " + progress + ",  curveOffset: " + curveOffset);
      
      float3 targetPos = Vector3.Lerp(front, back, (float)index / (float)totalNumberBots) +
                         (new Vector3(perpendicular.x, 0f, perpendicular.y) * curveOffset);


      float3 dir = targetPos - transform.Position;
      float dist = Vector3.Distance(targetPos, transform.Position);
      dir = Vector3.Normalize(dir);
      
      if (dist > arriveThreshold)
      {
         transform.Position = localTransform[index].Position + dir * deltaTime * speed;
         localTransform[index] = transform;
      }
      
      
     
      
      
   }
}
