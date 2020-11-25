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
            .WithName("Bee_Calculate_Velocity")
            .WithAll<BeeTeam>()
            .WithNone<Dead>()
            .WithReadOnly(teamsOfBlueBee)
            //.WithDisposeOnCompletion(teamsOfBlueBee)
            .WithReadOnly(teamsOfYellowBee)
            //.WithDisposeOnCompletion(teamsOfYellowBee)
            .WithoutBurst()
            .ForEach((ref Velocity velocity, in Translation pos, in BeeTeam beeTeam) =>
            {
                // Random move
                var rndVel = random.NextFloat3();
                velocity.vel += rndVel * beeParams.flightJitter * deltaTime;
                velocity.vel *= (1f - beeParams.damping);

                // Get friend
                int rndIndex;
                Entity attractiveFriend;
                Entity repellentFriend;
                if (beeTeam.team == BeeTeam.TeamColor.BLUE)
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
                float3 delta;
                float dist;
                delta = GetComponent<Translation>(attractiveFriend).Value - pos.Value;
                dist = sqrt(delta.x * delta.x + delta.y * delta.y + delta.z * delta.z);
                if (dist > 0f)
                {
                    velocity.vel += delta * (beeParams.teamAttraction * deltaTime / dist);
                }

                // Move away from repellent
                delta = GetComponent<Translation>(repellentFriend).Value - pos.Value;
                dist = sqrt(delta.x * delta.x + delta.y * delta.y + delta.z * delta.z);
                if (dist > 0f)
                {
                    velocity.vel -= delta * (beeParams.teamRepulsion * deltaTime / dist);
                }

                //pos.Value += deltaTime * velocity.vel;
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
                        targetBee.bee = teamsOfYellowBee[rndIndex];
                    }
                    else
                    {
                        rndIndex = random.NextInt(0, teamsOfBlueBee.Length);
                        targetBee.bee = teamsOfBlueBee[rndIndex];
                    }

                    ecb.AddComponent<TargetBee>(beeEntity, targetBee);
                }
                else
                {
                    if (unHeldResArray.Length > 1)
                    {
                        rndIndex = random.NextInt(0, unHeldResArray.Length);
                        targetRes.res = unHeldResArray[rndIndex];
                        ecb.AddComponent<TargetResource>(beeEntity, targetRes);
                    }
                }
            }).Run();

        ecb.Playback(EntityManager);
        ecb.Dispose();

        var ecb1 = new EntityCommandBuffer(Allocator.TempJob);
        Entities
            .WithName("Bee_Grap_Target_Resource")
            //WithAll<BeeTeam>()
            //.WithAll<TargetResource>()
            .WithoutBurst()
            .ForEach((Entity beeEntity, ref Velocity velocity, in BeeTeam beeTeam, in TargetResource targetRes, in Translation pos) =>
            {
                // resource has no holder
                if (HasComponent<HolderBee>(targetRes.res) == false)
                {
                    bool dead = HasComponent<Dead>(targetRes.res);
                    bool stacked = HasComponent<Stacked>(targetRes.res);
                    int gridX = GetComponent<GridX>(targetRes.res).gridX;
                    int gridY = GetComponent<GridY>(targetRes.res).gridY;
                    int stackIndex = GetComponent<StackIndex>(targetRes.res).index;

                    // resource dead or not top of the stack
                    if (dead || !Utils.IsTopOfStack(resGridParams, stackHeights, gridX, gridY, stackIndex, stacked))
                    {
                        ecb1.RemoveComponent<TargetResource>(beeEntity);
                    }
                    else
                    {
                        var delta = GetComponent<Translation>(targetRes.res).Value - pos.Value;
                        float sqrDist = delta.x * delta.x + delta.y * delta.y + delta.z * delta.z;
                        if (sqrDist > beeParams.grabDistance * beeParams.grabDistance)
                        {
                            velocity.vel += delta * (beeParams.chaseForce * deltaTime / sqrt(sqrDist));
                        }
                        else if (HasComponent<Stacked>(targetRes.res))
                        {
                            ecb1.AddComponent<HolderBee>(targetRes.res, new HolderBee { holder = beeEntity });
                            ecb1.RemoveComponent<Stacked>(targetRes.res);
                            Utils.UpdateStackHeights(resGridParams, stackHeights, gridX, gridY, stacked, 1);
                        }
                    }
                }
                // resource holder is the bee
                else if(GetComponent<HolderBee>(targetRes.res).holder == beeEntity)
                {
                    float team = (float)beeTeam.team;
                    float3 targetPos = new float3(-field.size.x * .45f + field.size.x * .9f * team, 0f, pos.Value.z);
                    float3 delta = targetPos - pos.Value;
                    float dist = sqrt(delta.x * delta.x + delta.y * delta.y + delta.z * delta.z);
                    velocity.vel += delta * (beeParams.carryForce * deltaTime / dist);
                    if(dist < 1f)
                    {
                        ecb1.RemoveComponent<HolderBee>(targetRes.res);
                        ecb1.RemoveComponent<TargetResource>(beeEntity);
                    }
                    else
                    {
                        ecb1.AddComponent<IsHoldingResource>(beeEntity);
                    }

                }
            }).Run();

        ecb1.Playback(EntityManager);
        ecb1.Dispose();

        Entities
            .WithName("Bee_Move")
            .WithAll<BeeTeam>()
            .WithNone<Dead>()
            .ForEach((ref Translation pos, in Velocity velocity) =>
            {
                pos.Value += deltaTime * velocity.vel;
            }).ScheduleParallel();
    }
}

