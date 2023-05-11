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
public partial struct BucketFetcherSystem : ISystem
{
    //[BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<Config>();

    }

    //[BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        //Get the config 
        var config = SystemAPI.GetSingleton<Config>();
        EntityCommandBuffer ECB = SystemAPI.GetSingleton<BeginInitializationEntityCommandBufferSystem.Singleton>().CreateCommandBuffer(state.WorldUnmanaged);

        //Get all teams
        int teamNo = config.TotalTeams;
        var teamList = new NativeList<Team>(teamNo, Allocator.Persistent);

        //Get component for each team
        for (int t = 0; t < teamNo; t++)
        {
            var TeamComponent = new Team { Value = t };

            teamList.Add(TeamComponent);
        }

        foreach (Team team in teamList)
        {
            //Necessary queries 
            EntityQuery backBotsQ = SystemAPI.QueryBuilder().WithAll<LocalTransform, Team, BackBotTag>().Build();
            backBotsQ.SetSharedComponentFilter(team);

            if (backBotsQ.IsEmpty)
                continue;

            EntityQuery bucketsQ = SystemAPI.QueryBuilder().WithAll<Bucket>().WithNone<Team>().Build();

            // If all buckets are assigned to teams, the system is not useful anymore          
            if (bucketsQ.IsEmpty)
            {
                Debug.Log("All buckets are assigned. My work here is done !");
                state.Enabled = false;
                return;
            }

            LocalTransform backBotTransform = backBotsQ.ToComponentDataArray<LocalTransform>(Allocator.TempJob)[0];
            Entity backBot = backBotsQ.ToEntityArray(Allocator.TempJob)[0];

            float minDist = float.MaxValue;
            Entity closestBucketToBackE = Entity.Null;

            foreach (var (bucketTransform, entity) in SystemAPI.Query<LocalTransform>().WithAll<Bucket, FreeTag, EmptyTag>().WithNone<Team>().WithEntityAccess())
            {
                var distToBack = Vector3.Distance(backBotTransform.Position, bucketTransform.Position);
                if (distToBack < minDist)
                {
                    minDist = distToBack;
                    closestBucketToBackE = entity;
                }
            }

            if (closestBucketToBackE == Entity.Null || backBot == Entity.Null)
                continue;

            //The Bucket Fetcher should get a bucket, fill it, and assign it to the team 
            LocalTransform closestBucketToBackT = SystemAPI.GetComponent<LocalTransform>(closestBucketToBackE);

            var jobCarryingFetcherQuery = SystemAPI.QueryBuilder().WithAll<BucketFetcherBotTag, CarryingBotTag, Team>().Build();
            jobCarryingFetcherQuery.SetSharedComponentFilter(team);
            
            var jobFreeFetcherQuery = SystemAPI.QueryBuilder().WithAll<BucketFetcherBotTag, Team>().WithNone<CarryingBotTag>().Build();
            jobFreeFetcherQuery.SetSharedComponentFilter(team);

            // Assert that the results of the queries have a sum of 1
            // Debug.Log($"This should be 1: {jobFreeFetcherQuery.CalculateEntityCount() + jobCarryingFetcherQuery.CalculateEntityCount()}");              

            JobHandle bucketFetcherToBucket = new FetchBucket
            {
                arriveThreshold = config.arriveThreshold,
                closestBucket = closestBucketToBackE, // closest bucket seems wrong sometimes
                closestBucketTransform = closestBucketToBackT,
                backBot = backBot,
                backBotTransform = backBotTransform,
                botSpeed = config.botSpeed,
                dt = Time.deltaTime,
                ecb = ECB,
                teamId = team
            }.Schedule(jobFreeFetcherQuery, state.Dependency);

            JobHandle bucketFetcherToBackBot = new FetchBucketCarrying
            {
                arriveThreshold = config.arriveThreshold,
                closestBucket = closestBucketToBackE,
                closestBucketTransform = closestBucketToBackT,
                backBot = backBot,
                backBotTransform = backBotTransform,
                botSpeed = config.botSpeed,
                dt = Time.deltaTime,
                ecb = ECB,
                teamId = team
            }.Schedule(jobCarryingFetcherQuery, bucketFetcherToBucket);
            bucketFetcherToBackBot.Complete();

        }
    }
}


// This job handles the movement of the Bucket Fetcher bot while carrying the Bucket.
public partial struct FetchBucketCarrying : IJobEntity
{
    public float arriveThreshold;
    public Entity closestBucket;
    public Entity backBot;

    public LocalTransform closestBucketTransform;
    public LocalTransform backBotTransform;

    public float botSpeed;
    public float dt;
    public EntityCommandBuffer ecb;
    public Team teamId;

    public void Execute(ref LocalTransform fetcherTransform, Entity fetcherE)
    {
        if (fetcherE == Entity.Null)
            return;

        Vector3 fetcherPosXZ = new Vector3(fetcherTransform.Position.x, 0f, fetcherTransform.Position.z);
        Vector3 backBotPosXZ = new Vector3(backBotTransform.Position.x, 0f, backBotTransform.Position.z);

        float3 dirToBackBot = Vector3.Normalize(backBotPosXZ - fetcherPosXZ);
        var distToBackBot = Vector3.Distance(backBotPosXZ, fetcherPosXZ);

        // Move to backBot of team
        // Move with bucket to the water
        if (distToBackBot > arriveThreshold)
        {
            // Maybe this also needs to be in the ECB
            var newFetcherPos = fetcherTransform.Position + dirToBackBot * dt * botSpeed;
            fetcherTransform.Position = newFetcherPos;

            closestBucketTransform.Position = new float3(newFetcherPos.x, newFetcherPos.y + 0.5f, newFetcherPos.z);
            ecb.SetComponent(closestBucket, closestBucketTransform);
        }
        // Drop Bucket
        else if (distToBackBot <= arriveThreshold)
        {
            closestBucketTransform.Position = new float3(fetcherTransform.Position.x, 0f, fetcherTransform.Position.z);
            ecb.SetComponent(closestBucket, closestBucketTransform);

            // Find the teams BackBot, move to it, drop bucket
            ecb.AddSharedComponent(closestBucket, teamId);
            ecb.SetComponentEnabled<FreeTag>(closestBucket, true);
            ecb.SetComponentEnabled<CarryingBotTag>(fetcherE, false);
            ecb.SetComponentEnabled<FillingTag>(closestBucket, true);
        }
    }
}

public partial struct FetchBucket : IJobEntity
{
    public float arriveThreshold;
    public Entity closestBucket;
    public Entity backBot;

    public LocalTransform closestBucketTransform;
    public LocalTransform backBotTransform;

    public float botSpeed;
    public float dt;
    public EntityCommandBuffer ecb;
    public Team teamId;

    public void Execute(ref LocalTransform fetcherTransform, Entity fetcherE)
    {
        if (fetcherE == Entity.Null)
            return;

        Vector3 bucketPosXZ = new Vector3(closestBucketTransform.Position.x, 0f, closestBucketTransform.Position.z);
        Vector3 fetcherPosXZ = new Vector3(fetcherTransform.Position.x, 0f, fetcherTransform.Position.z);

        float3 dirToBucket = Vector3.Normalize(bucketPosXZ - fetcherPosXZ);
        var distToBucket = Vector3.Distance(bucketPosXZ, fetcherPosXZ);

        // Print bucket and bucket fetcher to see who gets what
        //Debug.Log()

        // Move to the bucket
        // If the BucketFetcher has not gone to the closest bucket and is not carrying anything
        if (distToBucket > arriveThreshold)
        {
            fetcherTransform.Position += dirToBucket * dt * botSpeed;
        }
        // Pick the bucket
        // If not holding anything, and close to the bucket, pick up the bucket
        else if (distToBucket <= arriveThreshold)
        {
            ecb.SetComponentEnabled<FreeTag>(closestBucket, false);
            ecb.SetComponentEnabled<CarryingBotTag>(fetcherE, true);
        }
    }
}