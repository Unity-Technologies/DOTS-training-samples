using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using static Unity.Mathematics.math;
using Unity.Transforms;

public class BeeMoveSystem : SystemBase
{
    protected override void OnUpdate()
    {
        EntityManager manager = this.EntityManager;

        var beeParams = GetSingleton<BeeControlParamsAuthoring>();
        var field = GetSingleton<FieldAuthoring>();

        NativeList<Entity> teamsOfBlueBee = new NativeList<Entity>(beeParams.maxBeeCount, Allocator.TempJob);
        //var blueBeeWriter = teamsOfBlueBee.AsParallelWriter();
        //var blueBeeReader = teamsOfBlueBee.AsParallelReader();
        NativeList<Entity> teamsOfYellowBee = new NativeList<Entity>(beeParams.maxBeeCount, Allocator.TempJob);
        var yellowBeeWriter = teamsOfYellowBee.AsParallelWriter();

        float deltaTime = Time.fixedDeltaTime;

        var random = new Random(1234);

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
            }).Run();

        Entities
            .WithName("Bee_Move")
            .WithAll<BeeTeam>()
            .WithNone<Dead>()
            .WithReadOnly(teamsOfBlueBee)
            .WithDisposeOnCompletion(teamsOfBlueBee)
            .WithReadOnly(teamsOfYellowBee)
            .WithDisposeOnCompletion(teamsOfYellowBee)
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
                    attractiveFriend = teamsOfBlueBee[rndIndex];
                    rndIndex = random.NextInt(0, teamsOfYellowBee.Length);
                    repellentFriend = teamsOfBlueBee[rndIndex];
                }

                // Move towards friend
                float3 dir;
                float dist;
                dir = manager.GetComponentData<Translation>(attractiveFriend).Value - pos.Value;
                dist = sqrt(dir.x * dir.x + dir.y * dir.y + dir.z * dir.z);
                if (dist > 0f)
                {
                    velocity.vel += dir * (beeParams.teamAttraction * deltaTime / dist);
                }

                // Move away from repellent
                dir = manager.GetComponentData<Translation>(repellentFriend).Value - pos.Value;
                dist = sqrt(dir.x * dir.x + dir.y * dir.y + dir.z * dir.z);
                if (dist > 0f)
                {
                    velocity.vel -= dir * (beeParams.teamRepulsion * deltaTime / dist);
                }

                pos.Value += deltaTime * velocity.vel;
            }).Run();

#if COMMENT
        Entities
            .WithName("bee_get_target")
            .WithAll<BeeTeam>()
            .WithNone<TargetBee>()
            .WithNone<TargetResource>()
            .WithReadOnly(teamsOfBlueBee)
            .WithDisposeOnCompletion(teamsOfBlueBee)
            .WithReadOnly(teamsOfYellowBee)
            .WithDisposeOnCompletion(teamsOfYellowBee)
            .ForEach((Entity beeEntity, in BeeTeam beeTeam) =>
            {
                int rndIndex;
                TargetBee targetBee;
                //TargetResource targetRes;
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

                    manager.AddComponentData<TargetBee>(beeEntity, targetBee);
                }
                else
                {

                }
                /*
                if (bee.enemyTarget == null && bee.resourceTarget == null) {
                            if (Random.value < aggression) {
                                List<Bee> enemyTeam = teamsOfBees[1 - bee.team];
                                if (enemyTeam.Count > 0) {
                                    bee.enemyTarget = enemyTeam[Random.Range(0,enemyTeam.Count)];
                                }
                            } else {
                                bee.resourceTarget = ResourceManager.TryGetRandomResource();
                            }
                        }
                */
            }).ScheduleParallel();
#endif
    }
}

#if COMMENT

    protected override void OnUpdate()
    {
        /*
        var beeParams = GetSingleton<BeeControlParamsAuthoring>();
        var time = Time.ElapsedTime;
        float deltaTime = Time.fixedDeltaTime;

        Entities
        .WithName("bee_move")
        .WithAll<BeeTeam>()
        .WithNone<Dead>()
        .ForEach((ref Velocity velocity, ref Translation pos, in BeeTeam beeTeam) =>
        {
            if (beeTeam.team == 0)
            {
                velocity.vel = float3(0.1f, 0f, 0f) * beeParams.flightJitter * deltaTime;
                pos.Value += velocity.vel * (float)time;
            }
        }).ScheduleParallel();
        */

        var beeParams   = GetSingleton<BeeControlParamsAuthoring>();
        var field       = GetSingleton<FieldAuthoring>();

        NativeList<Translation> teamsOfBlueBee = new NativeList<Translation>(beeParams.maxBeeCount, Allocator.TempJob);
        NativeList<Translation> teamsOfYellowBee = new NativeList<Translation>(beeParams.maxBeeCount, Allocator.TempJob);

        float deltaTime = Time.fixedDeltaTime;

        var random = new Random(1234);

        Entities
            .WithName("get_bee_team")
            .WithAll<BeeTeam>()
            .WithNone<Dead>()
            .ForEach((in BeeTeam beeTeam, in Translation pos) =>
            {
                if(beeTeam.team == 0)
                {
                    teamsOfBlueBee.Add(pos);
                }
                else
                {
                    teamsOfYellowBee.Add(pos);
                }
            }).Run();

        Entities
            .WithName("bee_move")
            .WithAll<BeeTeam>()
            .WithNone<Dead>()
            .WithReadOnly(teamsOfBlueBee)
            .WithDisposeOnCompletion(teamsOfBlueBee)
            .WithReadOnly(teamsOfYellowBee)
            .WithDisposeOnCompletion(teamsOfYellowBee)
            .ForEach((ref Velocity velocity, ref Translation pos, in BeeTeam beeTeam) =>
            {
                // Random move
                var rndVel = random.NextFloat3(new float3(0f, 0f, 0f), new float3(1f, 1f, 1f));
                velocity.vel += rndVel * beeParams.flightJitter * deltaTime;
                velocity.vel *= (1f - beeParams.damping);

                // Get friend
                int rndIndex;
                Translation attractivePos;
                Translation repellentPos;
                if (beeTeam.team == 0)
                {
                    rndIndex = random.NextInt(0, teamsOfBlueBee.Length);
                    attractivePos = teamsOfBlueBee[rndIndex];
                    rndIndex = random.NextInt(0, teamsOfBlueBee.Length);
                    repellentPos = teamsOfBlueBee[rndIndex];
                }
                else
                {
                    rndIndex = random.NextInt(0, teamsOfYellowBee.Length);
                    attractivePos = teamsOfBlueBee[rndIndex];
                    rndIndex = random.NextInt(0, teamsOfYellowBee.Length);
                    repellentPos = teamsOfBlueBee[rndIndex];
                }

                // Move towards friend
                float3 dir;
                float dist;
                dir = attractivePos.Value - pos.Value;
                dist = sqrt(dir.x * dir.x + dir.y * dir.y + dir.z * dir.z);
                if(dist > 0f)
                {
                    velocity.vel += dir * (beeParams.teamAttraction * deltaTime / dist);
                }

                // Move away from repellent
                dir = repellentPos.Value - pos.Value;
                dist = sqrt(dir.x * dir.x + dir.y * dir.y + dir.z * dir.z);
                if (dist > 0f)
                {
                    velocity.vel -= dir * (beeParams.teamRepulsion * deltaTime / dist);
                }

                pos.Value += deltaTime * velocity.vel;
            }).ScheduleParallel();
    }
}
#endif
