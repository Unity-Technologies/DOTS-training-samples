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

[UpdateInGroup(typeof(MovementSystemGroup))]
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
   private int numTeams;
   
   private NativeList<Team> teamList;
   
   private bool hasCreatedTeamList;
   
   [BurstCompile]
   public void OnCreate(ref SystemState state)
   {
      state.RequireForUpdate<Config>();
            
      teamList = new NativeList<Team>(numTeams, Allocator.Persistent);
      hasCreatedTeamList = false;

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
      numTeams = config.TotalTeams;

      //GIGANTIC FOR LOOP AGAIN 
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
         
         //Needed queries 
         forwardBotsQ = SystemAPI.QueryBuilder().WithAll<LocalTransform, ForwardPassingBotTag,Team>().WithDisabled<CarryingBotTag>().Build();
         forwardBotsQ.SetSharedComponentFilter(teamList[i]);
         backwardBotsQ = SystemAPI.QueryBuilder().WithAll<LocalTransform, BackwardPassingBotTag,Team>().WithDisabled<CarryingBotTag>().Build();
         backwardBotsQ.SetSharedComponentFilter(teamList[i]);
         
         botTagsQF = SystemAPI.QueryBuilder().WithAll<BotTag,ForwardPassingBotTag,Team>().WithDisabled<CarryingBotTag>().Build();
         botTagsQF.SetSharedComponentFilter(teamList[i]);
         botTagsQB = SystemAPI.QueryBuilder().WithAll<BotTag,BackwardPassingBotTag,Team>().WithDisabled<CarryingBotTag>().Build();
         botTagsQB.SetSharedComponentFilter(teamList[i]);
         
         
         //Get Back guy position
         foreach (var backTransform in SystemAPI.Query<LocalTransform>().WithAll<BackBotTag,Team>().WithSharedComponentFilter(teamList[i]))
         {
            backPos = backTransform.Position;
         }
         

         //Get thrower guy
         foreach (var frontTransform in SystemAPI.Query<LocalTransform>().WithAll<FrontBotTag,Team>().WithSharedComponentFilter(teamList[i]))
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
      
         
         var ecbSingletonF = SystemAPI.GetSingleton<EndSimulationEntityCommandBufferSystem.Singleton>();
         var ecbF = ecbSingletonF.CreateCommandBuffer(state.WorldUnmanaged).AsParallelWriter();
         //Use job to distribute the bots (forward)
         var forwardJob = new MoveBotJob
         {
            ECB = ecbF,
            Entities = forwardEntities,
            front = frontPos+new float3(0.5f,0.0f,0.5f), 
            back = backPos,
            totalNumberBots = math.ceil(totalNumberOfBots / 2) - 1,
            localTransform = forwardTransform,
            botTags = botTagsF,
            deltaTime = dt,
            speed =  speed,
            arriveThreshold = arriveThreshold

         };

         var ecbSingletonB = SystemAPI.GetSingleton<EndSimulationEntityCommandBufferSystem.Singleton>();
         var ecbB = ecbSingletonB.CreateCommandBuffer(state.WorldUnmanaged).AsParallelWriter();
         var backwardJob = new MoveBotJob
         {
            ECB = ecbB,
            Entities = backwardEntities,
            front = backPos-new float3(0.5f,0.0f,0.5f),
            back = frontPos,
            totalNumberBots = math.ceil(totalNumberOfBots/2) - 1,
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
         botsInTeamQ.SetSharedComponentFilter(teamList[i]);
         int numBotsInTeam = botsInTeamQ.CalculateEntityCount();
         
         EntityQuery botsInTeamReachedQ =  SystemAPI.QueryBuilder().WithAll<Team,BotTag,ReachedTarget>().Build();
         botsInTeamReachedQ.SetSharedComponentFilter(teamList[i]);
         int numBbotsInTeamReachedQ =  botsInTeamReachedQ.CalculateEntityCount();

         // With this if any Team has completed the Chain then the botChainCompleteTag is set.
         if (numBotsInTeam == numBbotsInTeamReachedQ)
         {
            var TransitionManager = SystemAPI.GetSingletonEntity<Transition>();
            state.EntityManager.AddComponent<botChainCompleteTag>(TransitionManager);
         }
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
