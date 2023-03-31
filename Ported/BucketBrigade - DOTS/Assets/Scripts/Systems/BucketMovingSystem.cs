using System.Runtime.CompilerServices;
using Unity.Collections;
using Unity.Entities;
using Unity.Entities.UniversalDelegates;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;


//It should run after the bot moving system
[UpdateAfter(typeof(BotMovementSystem))]
[UpdateAfter(typeof(BucketFillingSystem))]
public partial struct BucketMovingSystem : ISystem
{
    private NativeArray<LocalTransform> forwardTransform;
    private NativeArray<LocalTransform> backwardTransform;
    private NativeArray<LocalTransform> frontBotTransforms;
    private NativeArray<LocalTransform> backBotTransforms;
    
    private EntityQuery forwardBotsQ;
    private EntityQuery frontBotsQ;
    private EntityQuery backwardBotsQ;
    private EntityQuery backBotsQ;
    private EntityQuery botsQ;
    private EntityQuery movingBotsQ;

    private ComponentLookup<FullTag> fullBuckets;
    
    

    //This is to find the bot and bucket, which are the closest to each other 
    private LocalTransform closestFilledBucketTransform;
    private Entity closestFilledBucketE;
    
    private LocalTransform closestBotTransform;
    private Entity closestBotE;
    private BotTag closestBotTag;

    private float speed;
    private int totalNumberOfBots;
    private int numAssignedBuckets;
    private bool isFilling;
    private bool isFull;
    
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<Config>();
        
        //Needed Entity Queries 
      
        forwardBotsQ = new EntityQueryBuilder(Allocator.Persistent).WithAll<LocalTransform, ForwardPassingBotTag, Team,ReachedTarget>()
            .Build(state.EntityManager);
        backwardBotsQ = new EntityQueryBuilder(Allocator.Persistent).WithAll<LocalTransform, BackwardPassingBotTag,Team,ReachedTarget>()
            .Build(state.EntityManager);
        movingBotsQ = new EntityQueryBuilder(Allocator.Persistent).WithAll<LocalTransform,Team,ReachedTarget,BotTag>().WithDisabled<BackBotTag,FrontBotTag>()
            .Build(state.EntityManager);
        frontBotsQ = new EntityQueryBuilder(Allocator.Persistent).WithAll<LocalTransform,Team,FrontBotTag>()
            .Build(state.EntityManager);
        backBotsQ = new EntityQueryBuilder(Allocator.Persistent).WithAll<LocalTransform,Team,BackBotTag>()
            .Build(state.EntityManager);

        //Instantiating Variables
        numAssignedBuckets = 0;
        isFilling = false;
        isFull = false;
        //Needed ComponentLookups
        fullBuckets = SystemAPI.GetComponentLookup<FullTag>();
        
        
        
        

    }

    public void OnDestroy(ref SystemState state)
    {
        forwardBotsQ.Dispose();
        backwardBotsQ.Dispose();
        movingBotsQ.Dispose();
        frontBotsQ.Dispose();
        backBotsQ.Dispose();
    }

    public void OnUpdate(ref SystemState state)
    {
        //Get the config 
        var config = SystemAPI.GetSingleton<Config>();
        speed = config.botSpeed;
        totalNumberOfBots = config.TotalBots;
        
        //Get delta time
        var dt = SystemAPI.Time.DeltaTime;

        //Get all the forward bots in an array (from a specific team)
        //Turn them into a native array 
        forwardTransform = forwardBotsQ.ToComponentDataArray<LocalTransform>(Allocator.Temp);
        backwardTransform = backwardBotsQ.ToComponentDataArray<LocalTransform>(Allocator.Temp);
    
        frontBotTransforms = frontBotsQ.ToComponentDataArray<LocalTransform>(Allocator.Temp);
        backBotTransforms = backBotsQ.ToComponentDataArray<LocalTransform>(Allocator.Temp);
        

        if (forwardTransform.Length + backwardTransform.Length < totalNumberOfBots - 2)
        {
            return;
        }
        else
        {
            forwardTransform.Dispose();
            backwardTransform.Dispose();
        }
        
       var minDist = float.MaxValue;
        
       //The back guy i should get a bucket, fill it, and assign it to the team 
       if (numAssignedBuckets == 0)
        {
           
            minDist = float.MaxValue;
           
            foreach (var (bucketTransform,entity) in SystemAPI.Query<LocalTransform>().WithAll<Bucket,FreeTag,EmptyTag>().WithEntityAccess())
            {
                var dist = Vector3.Distance(backBotTransforms[0].Position, bucketTransform.Position);
                if (dist < minDist)
                {
                    minDist = dist;                                             
                    closestFilledBucketTransform = bucketTransform;
                    closestFilledBucketE = entity;
                }
            }

            if (closestFilledBucketE != Entity.Null)
            {
                //Set it to being assigned to the team
                state.EntityManager.AddComponent(closestFilledBucketE, typeof(Team));
               
                //For visuals move it to the bots position 
                closestFilledBucketTransform.Position = backBotTransforms[0].Position + new float3(0.0f, 0.5f, 0.0f);
                state.EntityManager.SetComponentData(closestFilledBucketE, closestFilledBucketTransform);
               
                //And fill it 
                state.EntityManager.SetComponentEnabled<FillingTag>(closestFilledBucketE,true);
                numAssignedBuckets++;
                Debug.Log("Bucket for team was fetched by: " + backBotTransforms[0]);
            } 
            
        }


       //Update changed lookups 
       fullBuckets.Update(ref state);


       
        //For the bots without a bucket we should find our closest free team bucket
        if (numAssignedBuckets > 0)
        {
            closestFilledBucketTransform = state.EntityManager.GetComponentData<LocalTransform>(closestFilledBucketE);
            //We need to know if the buck is full or empty
            minDist = float.MaxValue;
            foreach ((var botTransform, var tag, var botEntity)  in SystemAPI.Query<LocalTransform, BotTag>().WithAll<Team>().WithDisabled<FrontBotTag,BackBotTag>().WithEntityAccess())
            {
                //Check if I should even go for this bucket
                var isBackward = state.EntityManager.IsComponentEnabled<BackwardPassingBotTag>(botEntity);
                var isForward = state.EntityManager.IsComponentEnabled<ForwardPassingBotTag>(botEntity);
                isFull = state.EntityManager.IsComponentEnabled<FullTag>(closestFilledBucketE);
                isFilling = state.EntityManager.IsComponentEnabled<FillingTag>(closestFilledBucketE);
                var isEmptying = state.EntityManager.IsComponentEnabled<EmptyingTag>(closestFilledBucketE);
                var isEmpty = state.EntityManager.IsComponentEnabled<EmptyTag>(closestFilledBucketE);

                if (isFilling || isEmptying)
                {
                    //We need to wait for it to fill or empty
                    break;
                }
                
                //If it is full and we are backwards we should skip
                if (isBackward && isFull && !isEmpty)
                {
                    continue;
                } 
                //If it is empty and filling and we are forwards we should skip(we can not move it while it is filling
                if (isForward && !isFull && isEmpty)
                {
                    continue;
                }
                
                
                //Check if I am on a cooldown
                if (tag.cooldown > 0.0f)
                {
                    //If I am I will update the cooldown and skip
                    state.EntityManager.SetComponentData(botEntity, new BotTag{cooldown =  tag.cooldown - 0.1f, noInChain = tag.noInChain, indexInChain = tag.indexInChain});
                    continue;
                }
                
                
                
                //If not I will check my distance
                var dist = Vector3.Distance(closestFilledBucketTransform.Position, botTransform.Position);

                if ( dist < minDist)
                {
                    minDist = dist;
                    closestBotTransform = botTransform;
                    closestBotE = botEntity;
                    closestBotTag = tag;
                }
            }

            if (closestBotE == Entity.Null)
            {
                return;
            }
            else
            {
                Debug.Log("The current closest bot is " + closestBotTag.noInChain + " chain index " + closestBotTag.indexInChain);
            }
            //At this point we should have the closest bot to our team's bucket
            //Set it to being occupied and the corresponding bot to be carrying 
            state.EntityManager.SetComponentEnabled<FreeTag>(closestFilledBucketE, false);
            state.EntityManager.SetComponentEnabled<CarryingBotTag>(closestBotE, true);
        }
        
        
        //For the bots with a bucket we should move to the next bot in line (the i-1 bot)
        
        //Get the bot who is carrying the bucket (ClosestBotE)
        //Get the next guy in line (using the no In chain)
        var nextBotPosition = new float3(0.0f);
        var endPosition = new float3(0.0f);
        foreach (var (botTransform,botTag) in SystemAPI.Query<LocalTransform, BotTag>())
        {
           isFull = state.EntityManager.IsComponentEnabled<FullTag>(closestFilledBucketE);
            isFilling = state.EntityManager.IsComponentEnabled<FillingTag>(closestFilledBucketE);
            
            if (isFull  && (botTag.noInChain == closestBotTag.noInChain - 1))
            {
                nextBotPosition = botTransform.Position;
                endPosition = frontBotTransforms[0].Position;
                break;
            } else if (!isFull  && botTag.noInChain == closestBotTag.noInChain - 1)
            {
                nextBotPosition = botTransform.Position;
                endPosition = backBotTransforms[0].Position;
                break;
            }

            
        }
        
        //Now make a job to move the carrying bot to the next bot in the chain

        var ecb = SystemAPI.GetSingleton<EndSimulationEntityCommandBufferSystem.Singleton>()
            .CreateCommandBuffer(state.WorldUnmanaged);

        MoveToNextJob moveToNextJob = new MoveToNextJob
        {
            TargetPos = nextBotPosition,
            currentBotTag = closestBotTag,
            EndPos = endPosition,
            bucketTransform = closestFilledBucketTransform,
            bucket = closestFilledBucketE,
            deltaTime = dt,
            speed = speed,
            ECB = ecb,
            arriveThreshold = 0.2f,
            isForward  = isFull
            
        };

        JobHandle moveToNextHandle = moveToNextJob.Schedule(state.Dependency);
        moveToNextHandle.Complete();
        //state.Dependency = moveToNextHandle;

    }
}   


public partial struct MoveToNextBotJob : IJobParallelFor
{
    [NativeDisableParallelForRestriction]
    public NativeArray<LocalTransform> movingBotTransforms;
    public NativeArray<Entity> bots;
    public NativeArray<BotTag> botTags;

    [NativeDisableParallelForRestriction]
    public ComponentLookup<CarryingBotTag> CarryingBotsLookup;
    [NativeDisableParallelForRestriction]
    public ComponentLookup<FullTag> FullBucketsLookup;
    [NativeDisableParallelForRestriction]
    public ComponentLookup<ForwardPassingBotTag> FowardBotsLookup;

    public LocalTransform frontBotTransform;
    public LocalTransform bucketTransform;
    public Entity bucket;
    
    public float deltaTime;
    public float speed;
    public float arriveThreshold;
    
    public EntityCommandBuffer.ParallelWriter ECB;


    public void Execute(int index)
    {

        //If we are not carrying a bucket, we should not execute this 
        if (!CarryingBotsLookup.IsComponentEnabled(bots[index]) )
        {
            return;
        }
        //If we are a forward moving bot and the bucket is empty we should not execute this 
        if (FowardBotsLookup.IsComponentEnabled(bots[index]) && !FullBucketsLookup.IsComponentEnabled(bucket))
        {
            return;
        }
        
        //If we are a backward moving bot and the bucket is full we should not execute this 
        if (!FowardBotsLookup.IsComponentEnabled(bots[index]) && FullBucketsLookup.IsComponentEnabled(bucket))
        {
            return;
        }
        
        
        
        //Else we should move
        //Just make it not be filling forever as it will drain the water source
        ECB.SetComponentEnabled<FillingTag>(index, bucket, false);
        //How to signal this?

        float3 targetPos = new float3(0.0f);
        
        //If it is not the front bot 
        if (index != 0)
        {
            //Get the position of the next bot in line
            targetPos = movingBotTransforms[index - 1].Position;
        }
        else
        {
            //I should move to the front bot 
            targetPos = frontBotTransform.Position;

        }
        
        LocalTransform transform = movingBotTransforms[index];
        float3 dir = Vector3.Normalize(targetPos - transform.Position);
        if (Vector3.Distance(targetPos ,transform.Position) > arriveThreshold)
        {
            transform.Position = transform.Position + dir * deltaTime * speed;
            
            bucketTransform.Position = transform.Position + new float3(0.0f, 0.5f,0.0f);
            ECB.SetComponent(index, bucket,bucketTransform);
            Debug.Log("Bucket after: " + bucketTransform.Position);
                        
        }
        else //We reached the target pos
        {
            //Get the tag so we can set our cooldown
            BotTag currentBot = botTags[index];
            
            //Put myself on a cooldown, so that I don't pick up the bucket again
            currentBot.cooldown = 1000f;
            ECB.SetComponent<BotTag>(index, bots[index],currentBot);
            
            //Stop carrying the bucket 
            ECB.SetComponentEnabled<CarryingBotTag>(index,bots[index],false);
            // Make the bucket free 
            ECB.SetComponentEnabled<FreeTag>(index, bucket,true);

            //If I am the front guy I should set the bucket to being emptying 
            if (index == 0)
            {
                ECB.SetComponentEnabled<EmptyingTag>(index, bucket,true);
            }
            

        }
        movingBotTransforms[index] = transform;

    }


}


[WithAll(typeof(CarryingBotTag))]
//This job will move a bot to the next one in line
public partial struct MoveToNextJob : IJobEntity
{
    
    public BotTag currentBotTag;
    
    public float3 TargetPos;
    public float3 EndPos;
    
    public LocalTransform bucketTransform;
    public Entity bucket;
    
    public float deltaTime;
    public float speed;
    public float arriveThreshold;
    public bool isForward;
    
    public EntityCommandBuffer ECB;
    public void Execute(Entity self, LocalTransform selfTransform)
    {
        
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
            currentBotTag.cooldown = 100f;
            ECB.SetComponent(self,currentBotTag);

            //Stop carrying the bucket 
            ECB.SetComponentEnabled<CarryingBotTag>(self,false);
            // Make the bucket free 
            ECB.SetComponentEnabled<FreeTag>(bucket,true);

            //If I am the front guy I should set the bucket to being emptying 
            if (currentBotTag.indexInChain == 0)
            {
                if (isForward)
                {
                    Debug.Log("I am the front guy so I should empty the bucket" + self);
                    ECB.SetComponentEnabled<EmptyingTag>(bucket,true);
                }
                else
                {
                    Debug.Log("I am the back guy so I should fill the bucket" + self);
                    ECB.SetComponentEnabled<FillingTag>(bucket,true);
                }
                
            }
            

        }

        //movingBotTransforms[currentBotTag.noInChain] = transform;
    }
    
}