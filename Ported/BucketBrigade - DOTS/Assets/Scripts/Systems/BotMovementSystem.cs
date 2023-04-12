using System.Collections;
using System.Collections.Generic;
using Systems;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering;



[UpdateAfter(typeof(BotNearestMovementSystem))]
[BurstCompile]

public partial struct BotMovementSystem : ISystem
{
   private NativeArray<LocalTransform> forwardTransform;
   private NativeArray<LocalTransform> backwardTransform;
   private NativeArray<BotTag> botTagsF;
   private NativeArray<BotTag> botTagsB;
   private float3 backPos;
   private float3 frontPos;
   private float speed;
   private float totalNumberOfBots;
   private float arriveThreshold;
   private EntityQuery forwardBotsQ;
   private EntityQuery backwardBotsQ;
   private EntityQuery botTagsQF;
   private EntityQuery botTagsQB;
   
   [BurstCompile]
   public void OnCreate(ref SystemState state)
   {
      state.RequireForUpdate<Config>();
            
      //Before moving the rest of the bots i need to ensure that the other system has run 
      state.RequireForUpdate<TeamReadyTag>();

      forwardBotsQ = SystemAPI.QueryBuilder().WithAll<LocalTransform, ForwardPassingBotTag>().WithDisabled<CarryingBotTag>().Build();
      backwardBotsQ = SystemAPI.QueryBuilder().WithAll<LocalTransform, BackwardPassingBotTag>().WithDisabled<CarryingBotTag>().Build();
      botTagsQF = SystemAPI.QueryBuilder().WithAll<BotTag,ForwardPassingBotTag>().WithDisabled<CarryingBotTag>().Build();
      botTagsQB = SystemAPI.QueryBuilder().WithAll<BotTag,BackwardPassingBotTag>().WithDisabled<CarryingBotTag>().Build();
      
   }

   [BurstCompile]
   public void OnUpdate(ref SystemState state)
   {
      
      float dt = SystemAPI.Time.DeltaTime;
      
      //Get the config 
      var config = SystemAPI.GetSingleton<Config>();
      speed = config.botSpeed;
      totalNumberOfBots = config.TotalBots;
      arriveThreshold = config.arriveThreshold;
      
      //Get Back guy position
      //For one team
      foreach (var backTransform in SystemAPI.Query<LocalTransform>().WithAll<BackBotTag,Team>())
      {
         backPos = backTransform.Position;
      }


      //Get thrower guy
      foreach (var frontTransform in SystemAPI.Query<LocalTransform>().WithAll<FrontBotTag,Team>())
      {
         frontPos = frontTransform.Position;
      }

      forwardTransform = forwardBotsQ.ToComponentDataArray<LocalTransform>(Allocator.TempJob);
      backwardTransform = backwardBotsQ.ToComponentDataArray<LocalTransform>(Allocator.TempJob);
      botTagsF = botTagsQF.ToComponentDataArray<BotTag>(Allocator.TempJob);
      botTagsB = botTagsQB.ToComponentDataArray<BotTag>(Allocator.TempJob);

      //Turn them into a native array 
      var forwardEntities = forwardBotsQ.ToEntityArray(Allocator.TempJob);
      var backwardEntities = backwardBotsQ.ToEntityArray(Allocator.TempJob);
   
      
      var ecbSingletonF = SystemAPI.GetSingleton<BeginSimulationEntityCommandBufferSystem.Singleton>();
      var ecbF = ecbSingletonF.CreateCommandBuffer(state.WorldUnmanaged).AsParallelWriter();
      //Use job to distribute the bots (forward)
      var forwardJob = new MoveBotJob
      {
         ECB = ecbF,
         Entities = forwardEntities,
         front = frontPos+new float3(0.5f,0.0f,0.5f), 
         back = backPos,
         totalNumberBots = totalNumberOfBots/2,
         localTransform = forwardTransform,
         botTags = botTagsF,
         deltaTime = dt,
         speed =  speed,
         arriveThreshold = arriveThreshold

      };

      var ecbSingletonB = SystemAPI.GetSingleton<BeginSimulationEntityCommandBufferSystem.Singleton>();
      var ecbB = ecbSingletonB.CreateCommandBuffer(state.WorldUnmanaged).AsParallelWriter();
      var backwardJob = new MoveBotJob
      {
         ECB = ecbB,
         Entities = backwardEntities,
         front = backPos-new float3(0.5f,0.0f,0.5f),
         back = frontPos,
         totalNumberBots = totalNumberOfBots/2,
         localTransform = backwardTransform,
         botTags = botTagsB,
         deltaTime = dt,
         speed = speed,
         arriveThreshold = arriveThreshold

      };


      JobHandle forwardHandle = forwardJob.Schedule(forwardTransform.Length,1);
      forwardHandle.Complete();
      
      JobHandle backwardHandle = backwardJob.Schedule(backwardTransform.Length,1);
      backwardHandle.Complete();
      forwardBotsQ.CopyFromComponentDataArray(forwardTransform);
      backwardBotsQ.CopyFromComponentDataArray(backwardTransform);

      //Check if everyone has reached their target. If so the BucketMovingSystem should start running. 
      //Query all the reached target entities and check if len = total num bots in team 
      EntityQuery botsInTeamQ =  SystemAPI.QueryBuilder().WithAll<Team,BotTag>().Build();
      int numBotsInTeam = botsInTeamQ.CalculateEntityCount();
      EntityQuery botsInTeamReachedQ =  SystemAPI.QueryBuilder().WithAll<Team,BotTag,ReachedTarget>().Build();
      int numBbotsInTeamReachedQ =  botsInTeamReachedQ.CalculateEntityCount();

      if (numBotsInTeam == numBbotsInTeamReachedQ)
      {
         var TransitionManager = SystemAPI.GetSingletonEntity<Transition>();
         state.EntityManager.AddComponent<botChainCompleteTag>(TransitionManager);
      }
      
   }
}


[BurstCompile]
public partial struct MoveBotJob : IJobParallelFor
{
   public EntityCommandBuffer.ParallelWriter ECB;
   public NativeArray<Entity> Entities;
   public float3 front;
   public float3 back;
   public float totalNumberBots;
   public NativeArray<LocalTransform> localTransform;
   
   [NativeDisableParallelForRestriction]
   public NativeArray<BotTag> botTags;
   public float deltaTime;
   public float speed;
   public float arriveThreshold;
  
   public void Execute(int index)
   {
      LocalTransform transform = localTransform[index];
      Entity e = Entities[index];
      float botNo = botTags[index].indexInChain;

      
      float progress = (float) botNo/ totalNumberBots;
      float curveOffset = Mathf.Sin(progress * Mathf.PI) * 1f;

      // get Vec2 data
      Vector2 heading = new Vector2(front.x, front.z) -  new Vector2(back.x, back.y);
      float distance = heading.magnitude;
      Vector2 direction = heading / distance;
      Vector2 perpendicular = new Vector2(direction.y, -direction.x);
      
      float3 targetPos = Vector3.Lerp(front, back, (float)botNo / (float)totalNumberBots) +
                         (new Vector3(perpendicular.x, 0.0f, perpendicular.y) * curveOffset);


      float3 dir = Vector3.Normalize(targetPos - transform.Position);

      if (Vector3.Distance(transform.Position, targetPos) > arriveThreshold)
      {
         
         transform.Position = transform.Position + dir * deltaTime * speed;
         localTransform[index] = transform;
         
        
      } else if(Vector3.Distance(transform.Position, targetPos) < arriveThreshold)
      {
         ECB.SetComponentEnabled<ReachedTarget>(index,e, true);
      }
     
   }
}
