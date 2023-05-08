using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;


//It should run after the bot moving system
[UpdateInGroup(typeof(MovementSystemGroup))]
[UpdateAfter(typeof(BotMovementSystem))]
[UpdateAfter(typeof(BucketSpawningSystem))]
public partial struct BucketMovingSystem : ISystem
{    
    private float speed;
    private int numTeams;
    private NativeList<Team> teamList;
    
    private bool hasCreatedTeamList;
    
    //[BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<Config>();
        state.RequireForUpdate<botChainCompleteTag>();

        //Instantiating Variables
        numAssignedBuckets = 0;
        isFilling = false;
        isFull = false;
        teamBuckets = new NativeArray<Entity>(10, Allocator.Persistent);
        teamBucketTransforms = new NativeArray<LocalTransform>(10, Allocator.Persistent);
        
        
        
        //Needed ComponentLookups
        fullBuckets = SystemAPI.GetComponentLookup<FullTag>();
        
        teamList = new NativeList<Team>(numTeams, Allocator.Persistent);
        hasCreatedTeamList = false;
        
    }

    //[BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        //Get the config 
        var config = SystemAPI.GetSingleton<Config>();
        EntityCommandBuffer ECB = SystemAPI.GetSingleton<BeginInitializationEntityCommandBufferSystem.Singleton>().CreateCommandBuffer(state.WorldUnmanaged);

        speed = config.botSpeed;
        numTeams = config.TotalTeams;
            
        //Get delta time
        var dt = SystemAPI.Time.DeltaTime;
        
      
        //Get component for each team
        for (int t = 0; t < numTeams  && !hasCreatedTeamList; t++)
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
            //Necessary queries 
            EntityQuery frontBotsQ = SystemAPI.QueryBuilder().WithAll<LocalTransform,Team,FrontBotTag>().Build();
            frontBotsQ.SetSharedComponentFilter(teamList[i]);

            EntityQuery backBotsQ = SystemAPI.QueryBuilder().WithAll<LocalTransform,Team,BackBotTag>().Build();
            backBotsQ.SetSharedComponentFilter(teamList[i]);

            if (frontBotsQ.IsEmpty || backBotsQ.IsEmpty)
                continue;

            LocalTransform frontBotTransform = frontBotsQ.ToComponentDataArray<LocalTransform>(Allocator.TempJob)[0];
            LocalTransform backBotTransform = backBotsQ.ToComponentDataArray<LocalTransform>(Allocator.TempJob)[0];
            Entity frontBot = frontBotsQ.ToEntityArray(Allocator.TempJob)[0];
            Entity backBot = backBotsQ.ToEntityArray(Allocator.TempJob)[0];

            foreach( var (currentTeamBucketTransform, currentTeamBucket) in SystemAPI.Query<LocalTransform>().WithAll<Bucket,Team> ().WithSharedComponentFilter(teamList[i]).WithEntityAccess())
            {

                bool isFull = state.EntityManager.IsComponentEnabled<FullTag>(currentTeamBucket);
                bool isFilling = state.EntityManager.IsComponentEnabled<FillingTag>(currentTeamBucket);
                var isEmptying = state.EntityManager.IsComponentEnabled<EmptyingTag>(currentTeamBucket);
                var isEmpty = state.EntityManager.IsComponentEnabled<EmptyTag>(currentTeamBucket);

                // For the free bucket find the closest bot without a bucket
                // WHo is the next one to carry a bucket either full or empty to the next BOT
                Entity closestBotE = Entity.Null;

                //We need to know if the bucket is full or empty
                if (SystemAPI.IsComponentEnabled<FreeTag>(currentTeamBucket))
                {
                    var minDist = float.MaxValue;
                    foreach (var (botTransform, tag, botEntity) in SystemAPI.Query<LocalTransform, BotTag>().WithAll<Team>().WithNone<BucketFetcherBotTag>()
                        .WithDisabled<FrontBotTag, BackBotTag, CarryingBotTag>().WithSharedComponentFilter(teamList[i]).WithEntityAccess())
                    {
                        //Check if I should even go for this bucket
                        var isBackward = state.EntityManager.IsComponentEnabled<BackwardPassingBotTag>(botEntity);
                        var isForward = state.EntityManager.IsComponentEnabled<ForwardPassingBotTag>(botEntity);                        

                        //We need to wait for it to fill or empty
                        if (isFilling || isEmptying)
                        {
                            break;
                        }
                        //If it is full and we are backwards we should skip
                        if (isBackward && isFull)
                        {
                            continue;
                        }
                        //If it is empty and filling and we are forwards we should skip(we can not move it while it is filling
                        if (isForward && !isFull)
                        {
                            continue;
                        }
                        //Check if I am on a cooldown
                        if (tag.cooldown > 0.0f)
                        {
                            //If I am I will update the cooldown and skip
                            state.EntityManager.SetComponentData(botEntity, new BotTag { cooldown = tag.cooldown - 0.1f, noInChain = tag.noInChain, indexInChain = tag.indexInChain });
                            continue;
                        }

                        //If not I will check my distance
                        var dist = Vector3.Distance(currentTeamBucketTransform.Position, botTransform.Position);
                        if (dist < minDist)
                        {
                            minDist = dist;
                            closestBotE = botEntity;
                        }
                    }
                }

                if (closestBotE == Entity.Null)
                    return;

                //At this point we should have the closest bot to our team's bucket
                //Set it to being occupied and the corresponding bot to be carrying 
                state.EntityManager.SetComponentEnabled<FreeTag>(currentTeamBucket, false);
                state.EntityManager.SetComponentEnabled<CarryingBotTag>(closestBotE, true);

                BotTag closestBotTag = SystemAPI.GetComponent<BotTag>(closestBotE);
                LocalTransform closestBotETransform = SystemAPI.GetComponent<LocalTransform>(closestBotE);
                var nextBotPosition = new float3(0.0f);
                var endPosition = new float3(0.0f);
                var targetBot = Entity.Null;

                if (!SystemAPI.IsComponentEnabled<FreeTag>(currentTeamBucket))
                {
                    //Get the bot who is carrying the bucket (ClosestBotE)
                    //Get the next guy in line (using the no In chain)

                    //For the bots with a bucket we should move to the next bot in line (the i-1 bot)
                    foreach (var (botTransform, botTag, t) in SystemAPI.Query<LocalTransform, BotTag, Team>().WithAll<CarryingBotTag>()
                        .WithNone<BucketFetcherBotTag>().WithSharedComponentFilter(teamList[i]))
                    {

                        if (isFull && (botTag.noInChain == closestBotTag.noInChain - 1))
                        {
                            nextBotPosition = botTransform.Position;
                            endPosition = frontBotTransform.Position;
                            targetBot = frontBot;
                            break;
                        }
                        else if (!isFull && botTag.noInChain == closestBotTag.noInChain - 1)
                        {
                            nextBotPosition = botTransform.Position;
                            endPosition = backBotTransform.Position;
                            targetBot = backBot;
                            break;
                        }
                    }

                    if (endPosition.Equals(float3.zero) || nextBotPosition.Equals(float3.zero))
                    {
                        return;
                    }
                }

                //Now make a job to move the carrying bot to the next bot in the chain
                MoveToNextInChainJob moveToNextInChainJob = new MoveToNextInChainJob
                {
                    self = closestBotE,
                    selfTransform = closestBotETransform,
                    currentBotTag = closestBotTag,
                    EndPos = endPosition,
                    bucketTransform = currentTeamBucketTransform,
                    bucket = currentTeamBucket,
                    deltaTime = dt,
                    speed = speed,
                    ECB = ECB,
                    arriveThreshold = config.arriveThreshold,
                    isForward = isFull,
                    TargetPos = nextBotPosition,
                    TargetEntity = targetBot

                };

                JobHandle moveToNextHandle = moveToNextInChainJob.Schedule(state.Dependency);
                moveToNextHandle.Complete();
            }
                                
        }
    }
}

public partial struct MoveToNextInChainJob : IJob
{
    public Entity self;
    public LocalTransform selfTransform;
    public BotTag currentBotTag;
    public float3 TargetPos;
    public Entity TargetEntity;
    public float3 EndPos;
    
    public LocalTransform bucketTransform;
    public Entity bucket;
    
    public float deltaTime;
    public float speed;
    public float arriveThreshold;
    public bool isForward;
    
    public EntityCommandBuffer ECB;
    
    public void Execute()
    {
        if (currentBotTag.cooldown > 0.0f)
        {
            return;
        }
        //First I will make sure the bucket is not being filled
        ECB.SetComponentEnabled<FillingTag>(bucket, false);

        var targetPos = new float3(0.0f);

        //If it is not the front bot 
        if (currentBotTag.indexInChain != 0)
        {
            //Get the position of the next bot in line
            targetPos = TargetPos;
        }
        else
        {
            //I should move to the front bot 
            targetPos = EndPos;
        }
        
        //LocalTransform transform = movingBotTransforms[currentBotTag.noInChain];
        float3 dir = Vector3.Normalize(targetPos - selfTransform.Position);
        if (Vector3.Distance(targetPos ,selfTransform.Position) > arriveThreshold)
        {
            selfTransform.Position = selfTransform.Position + dir * deltaTime * speed;
            ECB.SetComponent(self,selfTransform);
            bucketTransform.Position = selfTransform.Position + new float3(0.0f, 0.5f,0.0f);
            ECB.SetComponent(bucket,bucketTransform);
        }
        else //We reached the target pos
        {
            //Put myself on a cooldown, so that I don't pick up the bucket again
            currentBotTag.cooldown = 5f;
            ECB.SetComponent(self,currentBotTag);

            //Stop carrying the bucket 
            ECB.SetComponentEnabled<CarryingBotTag>(self,false);
            
            if (currentBotTag.indexInChain == 0)
            {
                //If I am the front guy I should set the bucket to being emptying 
                if (isForward)
                {
                    ECB.SetComponentEnabled<EmptyingTag>(bucket,true);
                }
                else
                {
                    ECB.SetComponentEnabled<FillingTag>(bucket,true);                   
                }                
                //Signal that the team isn't ready anymore
                ECB.SetComponentEnabled<TeamReadyTag>(TargetEntity,false);                                
            }
            
            // Make the bucket free 
            ECB.SetComponentEnabled<FreeTag>(bucket,true);            
        }
    }
    
}


