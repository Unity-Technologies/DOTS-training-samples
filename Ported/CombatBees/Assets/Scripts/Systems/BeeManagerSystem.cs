using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using static Unity.Mathematics.math;
using Unity.Transforms;

public class BeeManagerSystem : SystemBase
{
    private EntityQuery blueTeamQuery;
    private EntityQuery yellowTeamQuery;
    private EntityQuery unHeldResQuery;

    ResourceParams resParams;
    ResourceGridParams resGridParams;
    DynamicBuffer<StackHeightParams> stackHeights;

    protected override void OnCreate()
    {
        var notDead = GetEntityQuery(new EntityQueryDesc
        {
            None = new ComponentType[] { typeof(Dead) }
        });

        blueTeamQuery = GetEntityQuery(typeof(BeeTeam));
        blueTeamQuery.SetSharedComponentFilter(new BeeTeam { team = BeeTeam.TeamColor.BLUE });

        yellowTeamQuery = GetEntityQuery(typeof(BeeTeam));
        yellowTeamQuery.SetSharedComponentFilter(new BeeTeam { team = BeeTeam.TeamColor.YELLOW });

        unHeldResQuery = GetEntityQuery(new EntityQueryDesc
        {
            All = new ComponentType[] { typeof(StackIndex) },
            None = new ComponentType[] { typeof(Dead), typeof(TargetBee) }
        });

        resParams = GetSingleton<ResourceParams>();
        resGridParams = GetSingleton<ResourceGridParams>();
        var bufferFromEntity = GetBufferFromEntity<StackHeightParams>();
        var bufferEntity = GetSingletonEntity<ResourceParams>();
        stackHeights = bufferFromEntity[bufferEntity];
    }


    protected override void OnUpdate()
    {
        //EntityManager manager = this.EntityManager;

        var beeParams = GetSingleton<BeeControlParams>();
        var field = GetSingleton<FieldAuthoring>();
        var resParams = GetSingleton<ResourceParams>();
        var resGridParams = GetSingleton<ResourceGridParams>();
        var bufferFromEntity = GetBufferFromEntity<StackHeightParams>();
        var bufferEntity = GetSingletonEntity<ResourceParams>();
        var stackHeights = bufferFromEntity[bufferEntity];

        /*
        NativeList<Entity> teamsOfBlueBee = new NativeList<Entity>(beeParams.maxBeeCount, Allocator.TempJob);
        NativeList<Entity> teamsOfYellowBee = new NativeList<Entity>(beeParams.maxBeeCount, Allocator.TempJob);
        NativeList<Entity> notHoldResList = new NativeList<Entity>(resParams.maxResCount, Allocator.TempJob);
        */

        NativeArray<Entity> teamsOfBlueBee = blueTeamQuery.ToEntityArrayAsync(Allocator.TempJob, out var blueTeamHandle);
        NativeArray<Entity> teamsOfYellowBee = yellowTeamQuery.ToEntityArrayAsync(Allocator.TempJob, out var yellowTeamHandle);

        float deltaTime = Time.fixedDeltaTime;

        var random = new Random(1234);

        /*
        ///////////////////////////////////////////////////////
        Entities
            .WithName("Get_Bee_Team")
            .WithAll<BeeTeam>()
            .WithNone<Dead>()
            .ForEach((Entity beeEntity, in BeeTeam beeTeam, in Translation pos) =>
            {
                if (beeTeam.team == 0)
                {
                    teamsOfBlueBee.Add(beeEntity);
                }
                else
                {
                    teamsOfYellowBee.Add(beeEntity);
                }
            }).ScheduleParallel();
        */

        blueTeamHandle.Complete();
        yellowTeamHandle.Complete();

        ///////////////////////////////////////////////////////
        Entities
            .WithName("Bee_Move")
            .WithAll<BeeTeam>()
            .WithNone<Dead>()
            .WithReadOnly(teamsOfBlueBee)
            //.WithDisposeOnCompletion(teamsOfBlueBee)
            .WithReadOnly(teamsOfYellowBee)
            //.WithDisposeOnCompletion(teamsOfYellowBee)
            .WithoutBurst()
            .ForEach((ref Velocity velocity, ref Translation pos, in BeeTeam beeTeam) =>
            {
                // Random move
                var rndVel = random.NextFloat3();
                velocity.vel += rndVel * beeParams.flightJitter * deltaTime;
                velocity.vel *= (1f - beeParams.damping);

                // Get friend
                int rndIndex;
                Entity attractiveFriend;
                Entity repellentFriend;
                if (beeTeam.team == 0)
                {
                    rndIndex = random.NextInt(0, teamsOfBlueBee.Length);
                    attractiveFriend = teamsOfBlueBee[rndIndex];
                    rndIndex = random.NextInt(0, teamsOfBlueBee.Length);
                    repellentFriend = teamsOfBlueBee[rndIndex];
                }
                else
                {
                    rndIndex = random.NextInt(0, teamsOfYellowBee.Length);
                    attractiveFriend = teamsOfYellowBee[rndIndex];
                    rndIndex = random.NextInt(0, teamsOfYellowBee.Length);
                    repellentFriend = teamsOfYellowBee[rndIndex];
                }

                // Move towards friend
                float3 dir;
                float dist;
                dir = GetComponent<Translation>(attractiveFriend).Value - pos.Value;
                dist = sqrt(dir.x * dir.x + dir.y * dir.y + dir.z * dir.z);
                if (dist > 0f)
                {
                    velocity.vel += dir * (beeParams.teamAttraction * deltaTime / dist);
                }

                // Move away from repellent
                dir = GetComponent<Translation>(repellentFriend).Value - pos.Value;
                dist = sqrt(dir.x * dir.x + dir.y * dir.y + dir.z * dir.z);
                if (dist > 0f)
                {
                    velocity.vel -= dir * (beeParams.teamRepulsion * deltaTime / dist);
                }

                pos.Value += deltaTime * velocity.vel;
            }).Run();

        /*
        ///////////////////////////////////////////////////////
        Entities
            .WithName("Get_Not_Hold_Resource_List")
            .WithAll<StackIndex>()
            .WithNone<Dead>()
            .WithNone<TargetBee>()
            .ForEach((Entity resEntity) =>
            {
                notHoldResList.Add(resEntity);
            }).Run();
        */

        NativeArray<Entity> unHeldResArray = unHeldResQuery.ToEntityArrayAsync(Allocator.TempJob, out var unHeldResHandle);
        unHeldResHandle.Complete();

        ///////////////////////////////////////////////////////
        var ecb = new EntityCommandBuffer(Allocator.TempJob);
        Entities
            .WithName("Bee_Get_Target")
            .WithAll<BeeTeam>()
            .WithNone<TargetBee>()
            .WithNone<TargetResource>()
            .WithReadOnly(teamsOfBlueBee)
            .WithDisposeOnCompletion(teamsOfBlueBee)
            .WithReadOnly(teamsOfYellowBee)
            .WithDisposeOnCompletion(teamsOfYellowBee)
            .WithReadOnly(unHeldResArray)
            .WithDisposeOnCompletion(unHeldResArray)
            .WithoutBurst()
            .ForEach((Entity beeEntity, in BeeTeam beeTeam) =>
            {
                int rndIndex;
                TargetBee targetBee;
                TargetResource targetRes;
                if (random.NextFloat() < beeParams.aggression)
                {

                    if (beeTeam.team == 0)
                    {
                        rndIndex = random.NextInt(0, teamsOfYellowBee.Length);
                        targetBee.beeRef = teamsOfYellowBee[rndIndex];
                    }
                    else
                    {
                        rndIndex = random.NextInt(0, teamsOfBlueBee.Length);
                        targetBee.beeRef = teamsOfBlueBee[rndIndex];
                    }

                    ecb.AddComponent<TargetBee>(beeEntity, targetBee);
                }
                else
                {
                    if (unHeldResArray.Length > 1)
                    {
                        rndIndex = random.NextInt(0, unHeldResArray.Length);
                        targetRes.resRef = unHeldResArray[rndIndex];
                        ecb.AddComponent<TargetResource>(beeEntity, targetRes);
                    }
                }
            }).Run();

        ecb.Playback(EntityManager);
        ecb.Dispose();

        var ecb1 = new EntityCommandBuffer(Allocator.TempJob);
        Entities
            .WithName("Bee_Grap_Target_Resource")
            .WithAll<BeeTeam>()
            .WithAll<TargetResource>()
            .ForEach((Entity beeEntity, ref Velocity velocity, in TargetResource targetRes, in Translation pos) =>
            {
                // resource has no holder
                if (HasComponent<TargetBee>(targetRes.resRef) == false)
                {
                    // resource dead or not top of the stack
                    if (HasComponent<Dead>(targetRes.resRef) || !IsTopOfStack(targetRes.resRef))
                    {
                        ecb1.RemoveComponent<TargetResource>(beeEntity);
                    }
                    else
                    { 
                        var dir = GetComponent<Translation>(targetRes.resRef).Value - pos.Value;
                        float sqrDist = dir.x * dir.x + dir.y * dir.y + dir.z * dir.z;
                        if (sqrDist > beeParams.grabDistance * beeParams.grabDistance)
                        {
                            velocity.vel += dir * (beeParams.chaseForce * deltaTime / sqrt(sqrDist));
                        }
                        else if (HasComponent<Stacked>(targetRes.resRef))
                        {
                            ecb1.AddComponent<TargetBee>(targetRes.resRef);
                            ecb1.RemoveComponent<Stacked>(targetRes.resRef);
                            IncreaseStackHeights(targetRes.resRef); 
                        }

                    }
                    

                }
            }).Run();

        ecb1.Playback(EntityManager);
        ecb1.Dispose();

    }

    protected bool IsTopOfStack(Entity resource)
    {
        if(HasComponent<Stacked>(resource))
        {
            int gridX = GetComponent<GridX>(resource).gridX;
            int gridY = GetComponent<GridY>(resource).gridY;
            int index = resGridParams.gridCounts.x * gridX + gridY;
            if(GetComponent<StackIndex>(resource).index == stackHeights[index].Value - 1)
            {
                return true;
            }
        }
        return false;
    }

    protected bool IncreaseStackHeights(Entity resource)
    {
        if (HasComponent<Stacked>(resource))
        {
            int gridX = GetComponent<GridX>(resource).gridX;
            int gridY = GetComponent<GridY>(resource).gridY;
            int index = resGridParams.gridCounts.x * gridX + gridY;

            var element = stackHeights[index];
            element.Value++;
            stackHeights[index] = element;
        }
        return false;
    }

}

