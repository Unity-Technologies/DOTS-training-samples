using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using static Unity.Mathematics.math;
using Unity.Transforms;
using UnityEngine;

[UpdateAfter(typeof(BeeSpawnerSystem))]
[UpdateBefore(typeof(TransformSystemGroup))]
public class BeeManagerSystem : SystemBase
{
    private EntityQuery blueTeamQuery;
    private EntityQuery yellowTeamQuery;
    private EntityQuery unHeldResQuery;

    protected override void OnCreate()
    {
        /*
        var notDead = GetEntityQuery(new EntityQueryDesc
        {
            None = new ComponentType[] { typeof(Dead) }
        });
        */

        blueTeamQuery = GetEntityQuery(typeof(BeeTeam));
        blueTeamQuery.SetSharedComponentFilter(new BeeTeam { team = BeeTeam.TeamColor.BLUE });

        yellowTeamQuery = GetEntityQuery(typeof(BeeTeam));
        yellowTeamQuery.SetSharedComponentFilter(new BeeTeam { team = BeeTeam.TeamColor.YELLOW });

        unHeldResQuery = GetEntityQuery(new EntityQueryDesc
        {
            All = new ComponentType[] { typeof(StackIndex) },
            None = new ComponentType[] { typeof(Dead), typeof(HolderBee) }
        });
    }


    protected override void OnUpdate()
    {
        var beeParams = GetSingleton<BeeControlParams>();
        var field = GetSingleton<FieldAuthoring>();
        var resParams = GetSingleton<ResourceParams>();
        var resGridParams = GetSingleton<ResourceGridParams>();
        var bufferFromEntity = GetBufferFromEntity<StackHeightParams>();
        var bufferEntity = GetSingletonEntity<ResourceParams>();
        var stackHeights = bufferFromEntity[bufferEntity];

        float deltaTime = Time.fixedDeltaTime;
        var random = new Unity.Mathematics.Random(1234);

        /*
        NativeArray<Entity> teamsOfBlueBee = blueTeamQuery.ToEntityArrayAsync(Allocator.TempJob, out var blueTeamHandle);
        NativeArray<Entity> teamsOfYellowBee = yellowTeamQuery.ToEntityArrayAsync(Allocator.TempJob, out var yellowTeamHandle);
        blueTeamHandle.Complete();
        yellowTeamHandle.Complete();
        */
        NativeArray<Entity> teamsOfBlueBee = blueTeamQuery.ToEntityArray(Allocator.TempJob);
        NativeArray<Entity> teamsOfYellowBee = yellowTeamQuery.ToEntityArray(Allocator.TempJob);

        //Debug.Log("Get Entity Arrays for Blue and Yellow team");

        /* --------------------------------------------------------------------------------- */

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


        /* --------------------------------------------------------------------------------- */

        NativeArray<Entity> unHeldResArray = unHeldResQuery.ToEntityArrayAsync(Allocator.TempJob, out var unHeldResHandle);
        unHeldResHandle.Complete();
        var ecb = new EntityCommandBuffer(Allocator.TempJob);
        Entities
            .WithName("Bee_Has_No_TargetBee_And_TargetResource")
            //.WithAll<BeeTeam>()
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

                    if (beeTeam.team == BeeTeam.TeamColor.BLUE)
                    {
                        if (teamsOfYellowBee.Length > 1)
                        {
                            rndIndex = random.NextInt(0, teamsOfYellowBee.Length);
                            targetBee.bee = teamsOfYellowBee[rndIndex];
                            ecb.AddComponent<TargetBee>(beeEntity, targetBee);
                        }
                    }
                    else
                    {
                        if (teamsOfBlueBee.Length > 1)
                        {
                            rndIndex = random.NextInt(0, teamsOfBlueBee.Length);
                            targetBee.bee = teamsOfBlueBee[rndIndex];
                            ecb.AddComponent<TargetBee>(beeEntity, targetBee);
                        }
                    }
                }
                else
                {
                    if (unHeldResArray.Length > 1)
                    {
                        rndIndex = random.NextInt(0, unHeldResArray.Length);
                        targetRes.res = unHeldResArray[rndIndex];

                        bool stacked = HasComponent<Stacked>(targetRes.res);
                        int gridX = GetComponent<GridX>(targetRes.res).gridX;
                        int gridY = GetComponent<GridY>(targetRes.res).gridY;
                        int stackIndex = GetComponent<StackIndex>(targetRes.res).index;

                        if (Utils.IsTopOfStack(resGridParams, stackHeights, gridX, gridY, stackIndex, stacked))
                        {
                            ecb.AddComponent<TargetResource>(beeEntity, targetRes);
                        }
                    }
                }
            }).Run();
        ecb.Playback(EntityManager);
        ecb.Dispose();

        /* --------------------------------------------------------------------------------- */

        var ecb1 = new EntityCommandBuffer(Allocator.TempJob);
        Entities
            .WithName("Bee_Has_Target_Bee")
            .WithAll<BeeTeam>()
            .WithAll<TargetBee>()
            .WithoutBurst()
            .ForEach((Entity beeEntity, in BeeTeam beeTeam, in TargetBee targetBee, in Translation pos) =>
            {
                Velocity velocity = GetComponent<Velocity>(beeEntity);

                // target bee is dead
                if (HasComponent<Dead>(targetBee.bee))
                {
                    ecb1.RemoveComponent<TargetBee>(beeEntity);
                }
                else
                {
                    float3 delta = GetComponent<Translation>(targetBee.bee).Value - pos.Value;
                    float sqrDist = delta.x * delta.x + delta.y * delta.y + delta.z * delta.z;

                    // make targetBee is not itself
                    if (sqrDist != 0)
                    {
                        if (sqrDist > beeParams.attackDistance * beeParams.attackDistance)
                        {
                            velocity.vel += delta * (beeParams.chaseForce * deltaTime / sqrt(sqrDist));
                        }
                        else
                        {
                            ecb1.AddComponent<IsAttacking>(beeEntity);
                            velocity.vel += delta * (beeParams.attackForce * deltaTime / sqrt(sqrDist));

                            if (sqrDist < beeParams.hitDistance * beeParams.hitDistance)
                            {
                                //////////////////////////// ToDo, spawn blood particle
                                //ParticleManager.SpawnParticle(bee.enemyTarget.position, ParticleType.Blood, bee.velocity * .35f, 2f, 6);
                                ecb1.AddComponent<Dead>(targetBee.bee);
                                Velocity targetVelocity = GetComponent<Velocity>(targetBee.bee);
                                SetComponent<Velocity>(targetBee.bee, new Velocity { vel = targetVelocity.vel * .5f });
                                ecb1.RemoveComponent<TargetBee>(beeEntity);
                            }
                        }

                        SetComponent<Velocity>(beeEntity, new Velocity { vel = velocity.vel });
                    }
                }
            }).Run();
        ecb1.Playback(EntityManager);
        ecb1.Dispose();


        /* --------------------------------------------------------------------------------- */

        var ecb2 = new EntityCommandBuffer(Allocator.TempJob);
        Entities
            .WithName("Bee_Has_Target_Resource")
            .WithAll<BeeTeam>()
            .WithAll<TargetResource>()
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
                        ecb2.RemoveComponent<TargetResource>(beeEntity);
                    }
                    else
                    {
                        var delta = GetComponent<Translation>(targetRes.res).Value - pos.Value;
                        float sqrDist = delta.x * delta.x + delta.y * delta.y + delta.z * delta.z;
                        if (sqrDist > beeParams.grabDistance * beeParams.grabDistance)
                        {
                            velocity.vel += delta * (beeParams.chaseForce * deltaTime / sqrt(sqrDist));
                        }
                        // Grab source
                        else if (stacked)
                        {
                            ecb2.AddComponent<HolderBee>(targetRes.res, new HolderBee { holder = beeEntity });
                            ecb2.RemoveComponent<Stacked>(targetRes.res);
                            Utils.UpdateStackHeights(resGridParams, stackHeights, gridX, gridY, stacked, -1);
                        }
                    }
                }
                // resource holder is the bee
                else
                {
                    Entity holder = GetComponent<HolderBee>(targetRes.res).holder;
                    if (holder == beeEntity)
                    {
                        float team = (float)beeTeam.team;
                        float3 targetPos = new float3(-field.size.x * .45f + field.size.x * .9f * team, 0f, pos.Value.z);
                        float3 delta = targetPos - pos.Value;
                        float dist = sqrt(delta.x * delta.x + delta.y * delta.y + delta.z * delta.z);
                        velocity.vel += delta * (beeParams.carryForce * deltaTime / dist);
                        if (dist < 1f)
                        {
                            ecb2.RemoveComponent<HolderBee>(targetRes.res);
                            ecb2.RemoveComponent<TargetResource>(beeEntity);
                        }
                        else
                        {
                            ecb2.AddComponent<IsHoldingResource>(beeEntity);
                        }
                    }
                    else
                    {
                        /*
                        //////////////////////////// ToDo
                        // how to get shared component in job???????????????????????????
                         BeeTeam holderTeam = GetComponent<BeeTeam>(holder);
                        if (holderTeam.team != beeTeam.team)
                        {
                            ecb2.AddComponent<TargetBee>(beeEntity, new TargetBee { bee = holder });
                        }
                        else
                        {
                            ecb2.RemoveComponent<TargetResource>(beeEntity);
                        }
                        */
                    }
                }

            }).Run();
        ecb2.Playback(EntityManager);
        ecb2.Dispose();

        /* --------------------------------------------------------------------------------- */

        var ecb3 = new EntityCommandBuffer(Allocator.TempJob);
        Entities
            .WithName("Bee_Is_Dead")
            .WithAll<BeeTeam>()
            .WithAll<Dead>()
            .ForEach((Entity beeEntity, ref DeathTimer deathTimer, ref Velocity velocity, in Translation pos) =>
            {
                if (random.NextFloat() < (deathTimer.dTimer - .5f) * .5f)
                {
                    //////////////////////////// ToDo
                    // Spawn blood particle
                    //ParticleManager.SpawnParticle(bee.position, ParticleType.Blood, Vector3.zero);
                }

                velocity.vel.y += field.gravity * deathTimer.dTimer;
                deathTimer.dTimer -= deltaTime / 10f;
                if (deathTimer.dTimer < 0f)
                {
                    ecb3.DestroyEntity(beeEntity);
                }

            }).Run();
        ecb3.Playback(EntityManager);
        ecb3.Dispose();

        /* --------------------------------------------------------------------------------- */

        Entities
            .WithName("Bee_Calculate_Position")
            .WithAll<BeeTeam>()
            .WithNone<Dead>()
            .ForEach((ref Translation pos, in Velocity velocity) =>
            {
                pos.Value += deltaTime * velocity.vel;
            }).ScheduleParallel();


        /* --------------------------------------------------------------------------------- */

        Entities
            .WithName("Bee_Move")
            .WithAll<BeeTeam>()
            .ForEach((Entity beeEntity, ref Velocity velocity, ref Translation pos) =>
            {
                if (abs(pos.Value.x) > field.size.x * .5f)
                {
                    pos.Value.x = field.size.x * .5f * sign(pos.Value.x);
                    velocity.vel.x *= -.5f;
                    velocity.vel.y *= .8f;
                    velocity.vel.z *= .8f;
                }

                if (abs(pos.Value.z) > field.size.z * .5f)
                {
                    pos.Value.z = field.size.z * .5f * sign(pos.Value.z);
                    velocity.vel.z *= -.5f;
                    velocity.vel.x *= .8f;
                    velocity.vel.y *= .8f;
                }

                /*
                float resModifier = 0f;
                if (HasComponent<IsHoldingResource>(beeEntity))
                {
                    resModifier = resParams.resourceSize;
                }

                if (abs(pos.Value.y) > field.size.y * .5f - resModifier)
                {
                    pos.Value.y = (field.size.y * .5f - resModifier) * sign(pos.Value.y);
                    velocity.vel.y *= -.5f;
                    velocity.vel.x *= .8f;
                    velocity.vel.z *= .8f;
                }
                */
            }).ScheduleParallel();

        /* --------------------------------------------------------------------------------- */

#if COMMENT
        Entities
            .WithName("Bee_Smooth_Direction")
            .WithAll<BeeTeam>()
            .ForEach((Entity beeEntity, ref SmoothPosition smoothPos, ref SmoothDirection smoothDir, 
                        ref NonUniformScale scale, in Velocity velocity, in Translation pos) =>
            {
                float3 oldSmPos = smoothPos.smPos;
                if (!HasComponent<IsAttacking>(beeEntity))
                {
                    smoothPos.smPos = lerp(smoothPos.smPos, pos.Value, deltaTime * beeParams.rotationStiffness);
                    smoothPos.smPos = pos.Value;
                }
                else 
                {
                    smoothPos.smPos = pos.Value;
                }

                smoothDir.smDir = smoothPos.smPos - oldSmPos;

                if(!HasComponent<Dead>(beeEntity))
                {
                    float stretch = max(1f, math.length(velocity.vel) * beeParams.speedStretch);
                    scale.Value.z *= stretch;
                    scale.Value.x /= (stretch - 1f) / 5f + 1f;
                    scale.Value.y /= (stretch - 1f) / 5f + 1f;
                    
                }

                /*
                Quaternion rotation = Quaternion.identity;
                if (bees[i].smoothDirection != Vector3.zero)
                {
                    rotation = Quaternion.LookRotation(bees[i].smoothDirection);
                }
                Color color = teamColors[bees[i].team];
                if (bees[i].dead)
                {
                    color *= .75f;
                    scale *= Mathf.Sqrt(bees[i].deathTimer);
                }

                beeMatrices[i / beesPerBatch][i % beesPerBatch] = Matrix4x4.TRS(bees[i].position, rotation, scale);
                beeColors[i / beesPerBatch][i % beesPerBatch] = color;
                */

            }).ScheduleParallel();
#endif
        
    }
}
