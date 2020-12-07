using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
//using static Unity.Mathematics.math;
using Unity.Transforms;
using UnityEngine;
using Unity.Rendering;


[UpdateAfter(typeof(BeeSpawnerSystem))]
[UpdateAfter(typeof(ResourceSpawnerSystem))]
[UpdateBefore(typeof(TransformSystemGroup))]
public class BeeManagerSystem : SystemBase
{
    private EntityQuery unHeldResQuery;

    protected override void OnCreate()
    {
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

        NativeList<Entity> teamsOfBlueBee = new NativeList<Entity>(beeParams.maxBeeCount, Allocator.TempJob);
        NativeList<Entity> teamsOfYellowBee = new NativeList<Entity>(beeParams.maxBeeCount, Allocator.TempJob);

        /* --------------------------------------------------------------------------------- */
        var ecb0 = new EntityCommandBuffer(Allocator.TempJob);
        Entities
            .WithName("Bee_Get_Teams")
            .WithNone<Dead>()
            .ForEach((Entity beeEntity, in BeeTeam beeTeam) =>
            {
                if(HasComponent<IsHoldingResource>(beeEntity))
                {
                    ecb0.RemoveComponent<IsHoldingResource>(beeEntity);
                }

                if (beeTeam.team == BeeTeam.TeamColor.BLUE)
                {
                    teamsOfBlueBee.Add(beeEntity);
                }
                else
                {
                    teamsOfYellowBee.Add(beeEntity);
                }
            }).Run();
        ecb0.Playback(EntityManager);
        ecb0.Dispose();


        /* --------------------------------------------------------------------------------- */

        Entities
            .WithName("Bee_Calculate_Velocity")
            .WithAll<BeeTeam>()
            .WithNone<Dead>()
            .WithReadOnly(teamsOfBlueBee)
            .WithReadOnly(teamsOfYellowBee)
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
                    attractiveFriend = teamsOfBlueBee.ElementAt(rndIndex);
                    rndIndex = random.NextInt(0, teamsOfBlueBee.Length);
                    repellentFriend = teamsOfBlueBee.ElementAt(rndIndex);
                }
                else
                {
                    rndIndex = random.NextInt(0, teamsOfYellowBee.Length);
                    attractiveFriend = teamsOfYellowBee.ElementAt(rndIndex);
                    rndIndex = random.NextInt(0, teamsOfYellowBee.Length);
                    repellentFriend = teamsOfYellowBee.ElementAt(rndIndex);
                }

                // Move towards friend
                float3 delta;
                float dist;
                delta = GetComponent<Translation>(attractiveFriend).Value - pos.Value;
                dist = math.sqrt(delta.x * delta.x + delta.y * delta.y + delta.z * delta.z);
                if (dist > 0f)
                {
                    velocity.vel += delta * (beeParams.teamAttraction * deltaTime / dist);
                }

                // Move away from repellent
                delta = GetComponent<Translation>(repellentFriend).Value - pos.Value;
                dist = math.sqrt(delta.x * delta.x + delta.y * delta.y + delta.z * delta.z);
                if (dist > 0f)
                {
                    velocity.vel -= delta * (beeParams.teamRepulsion * deltaTime / dist);
                }
                
            }).Run();


        /* --------------------------------------------------------------------------------- */

        NativeArray<Entity> unHeldResArray = unHeldResQuery.ToEntityArrayAsync(Allocator.TempJob, out var unHeldResHandle);
        unHeldResHandle.Complete();
        var ecb = new EntityCommandBuffer(Allocator.TempJob);
        Entities
            .WithName("Bee_Has_No_TargetBee_And_TargetResource")
            .WithNone<Dead>()
            .WithNone<TargetBee>()
            .WithNone<TargetResource>()
            .WithReadOnly(teamsOfBlueBee)
            .WithDisposeOnCompletion(teamsOfBlueBee)
            .WithReadOnly(teamsOfYellowBee)
            .WithDisposeOnCompletion(teamsOfYellowBee)
            .WithReadOnly(unHeldResArray)
            .WithDisposeOnCompletion(unHeldResArray)
            .ForEach((Entity beeEntity, in BeeTeam beeTeam) =>
            {
                int rndIndex;
                TargetBee targetBee;
                TargetResource targetRes;
                if (random.NextFloat() < beeParams.aggression)
                {
                    if (beeTeam.team == BeeTeam.TeamColor.BLUE)
                    {
                        if (teamsOfYellowBee.Length > 0)
                        {
                            rndIndex = random.NextInt(0, teamsOfYellowBee.Length);
                            targetBee.bee = teamsOfYellowBee.ElementAt(rndIndex);
                            ecb.AddComponent<TargetBee>(beeEntity, targetBee);
                        }
                    }
                    else
                    {
                        if (teamsOfBlueBee.Length > 0)
                        {
                            rndIndex = random.NextInt(0, teamsOfBlueBee.Length);
                            targetBee.bee = teamsOfBlueBee.ElementAt(rndIndex);
                            ecb.AddComponent<TargetBee>(beeEntity, targetBee);
                        }
                    }
                }
                else
                {
                    if (unHeldResArray.Length > 0)
                    {
                        rndIndex = random.NextInt(0, unHeldResArray.Length);
                        targetRes.res = unHeldResArray[rndIndex];

                        bool stacked = HasComponent<Stacked>(targetRes.res);
                        int gridX = GetComponent<GridX>(targetRes.res).gridX;
                        int gridY = GetComponent<GridY>(targetRes.res).gridY;
                        int stackIndex = GetComponent<StackIndex>(targetRes.res).index;

                        // Get latest buffer
                        bufferFromEntity = GetBufferFromEntity<StackHeightParams>();
                        stackHeights = bufferFromEntity[bufferEntity];
                        int index = resGridParams.gridCounts.y * gridX + gridY;
                        
                        if ((HasComponent<HolderBee>(targetRes.res) == false) ||
                            (stackIndex == stackHeights[index].Value - 1))
                        {
                            ecb.AddComponent<TargetResource>(beeEntity, targetRes);
                        }
                    }
                }
            }).Run();
        ecb.Playback(EntityManager);
        ecb.Dispose();

        /* --------------------------------------------------------------------------------- */

        NativeList<Entity> deadBeeList = new NativeList<Entity>(beeParams.maxBeeCount, Allocator.TempJob);
        var ecb1 = new EntityCommandBuffer(Allocator.TempJob);
        Entities
            .WithName("Bee_Has_Target_Bee")
            .WithNone<Dead>()
            .WithAll<Velocity>()
            .WithDisposeOnCompletion(deadBeeList)
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
                    // Bee is attacked be previous bee
                    int deadListIndex = Utils.SearchDeadBee(deadBeeList, beeEntity);
                    if (deadListIndex != -1)
                    {
                        deadBeeList.RemoveAt(deadListIndex);
                    }
                    else
                    {
                        float3 delta = GetComponent<Translation>(targetBee.bee).Value - pos.Value;
                        float sqrDist = delta.x * delta.x + delta.y * delta.y + delta.z * delta.z;

                        if (sqrDist > beeParams.attackDistance * beeParams.attackDistance)
                        {
                            velocity.vel += delta * (beeParams.chaseForce * deltaTime / math.sqrt(sqrDist));
                            ecb1.SetComponent<Velocity>(beeEntity, new Velocity { vel = velocity.vel });
                        }
                        else
                        {
                            // make sure targetBee is not itself
                            if (sqrDist > 0)
                            {
                                ecb1.AddComponent<IsAttacking>(beeEntity);

                                velocity.vel += delta * (beeParams.attackForce * deltaTime / math.sqrt(sqrDist));
                                ecb1.SetComponent<Velocity>(beeEntity, new Velocity { vel = velocity.vel });

                                if (sqrDist < beeParams.hitDistance * beeParams.hitDistance)
                                {
                                    //////////////////////////// ToDo, spawn blood particle
                                    //ParticleManager.SpawnParticle(bee.enemyTarget.position, ParticleType.Blood, bee.velocity * .35f, 2f, 6);
                                    ecb1.AddComponent<Dead>(targetBee.bee);
                                    Velocity targetVelocity = GetComponent<Velocity>(targetBee.bee);
                                    ecb1.SetComponent<Velocity>(targetBee.bee, new Velocity { vel = targetVelocity.vel * .5f });
                                    ecb1.RemoveComponent<TargetBee>(beeEntity);
                                    deadBeeList.Add(targetBee.bee);
                                }
                            }
                        }
                    }
                }
            }).Run();
        ecb1.Playback(EntityManager);
        ecb1.Dispose();

        /* --------------------------------------------------------------------------------- */

        var ecb2 = new EntityCommandBuffer(Allocator.TempJob);
        Entities
            .WithName("Bee_Has_Target_Resource")
            .WithNone<Dead>()
            .WithNone<TargetBee>()
            .ForEach((Entity beeEntity, ref Velocity velocity, in BeeTeam beeTeam, in TargetResource targetRes, in Translation pos) =>
            {
                // resource has no holder
                if (HasComponent<HolderBee>(targetRes.res) == false)
                {
                    // Get latest buffer
                    bufferFromEntity = GetBufferFromEntity<StackHeightParams>();
                    stackHeights = bufferFromEntity[bufferEntity];

                    bool dead = HasComponent<Dead>(targetRes.res);
                    bool stacked = HasComponent<Stacked>(targetRes.res);
                    int gridX = GetComponent<GridX>(targetRes.res).gridX;
                    int gridY = GetComponent<GridY>(targetRes.res).gridY;
                    int stackIndex = GetComponent<StackIndex>(targetRes.res).index;

                    // resource dead or not top of the stack
                    if (dead)
                    {
                        ecb2.RemoveComponent<TargetResource>(beeEntity);
                    }
                    else if(Utils.IsTopOfStack(resGridParams, stackHeights, gridX, gridY, stackIndex, stacked) == false)
                    {
                        ecb2.RemoveComponent<TargetResource>(beeEntity);
                    }
                    else
                    {
                        var delta = GetComponent<Translation>(targetRes.res).Value - pos.Value;
                        float sqrDist = delta.x * delta.x + delta.y * delta.y + delta.z * delta.z;
                        if (sqrDist > beeParams.grabDistance * beeParams.grabDistance)
                        {
                            velocity.vel += delta * (beeParams.chaseForce * deltaTime / math.sqrt(sqrDist));
                        }
                        // Grab source
                        else if (stacked)
                        {
                            //Debug.Log("add holder bee!!!!!!!!!!!!!!!!!!!!!!");
                            //var tempPos = GetComponent<Translation>(targetRes.res);
                            //Debug.Log("temp resource pos = " + tempPos.Value);
                            ecb2.AddComponent<HolderBee>(targetRes.res, new HolderBee { holder = beeEntity });
                            ecb2.RemoveComponent<Stacked>(targetRes.res);

                            // Get latest buffer
                            bufferFromEntity = GetBufferFromEntity<StackHeightParams>();
                            stackHeights = bufferFromEntity[bufferEntity];
                            Utils.UpdateStackHeights(resGridParams, stackHeights, gridX, gridY, stacked, -1);
                        }
                        
                    }
                }
                // resource has holder
                else
                {
                    Entity holder = GetComponent<HolderBee>(targetRes.res).holder;
                    if (holder == beeEntity)
                    {
                        int team = (int)beeTeam.team;
                        float3 targetPos = new float3(-field.size.x * .45f + field.size.x * .9f * team, 0f, pos.Value.z);
                        float3 delta = targetPos - pos.Value;
                        float dist = math.sqrt(delta.x * delta.x + delta.y * delta.y + delta.z * delta.z);

                        // make sure dist is non zero
                        if (dist > 0)
                        {
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
                    }
                    else
                    {
                        BeeTeam resHolderTeam = GetComponent<BeeTeam>(holder);
                        if(resHolderTeam.team != beeTeam.team)
                        {
                            if (HasComponent<TargetBee>(beeEntity) == false)
                            {
                                ecb2.AddComponent<TargetBee>(beeEntity, new TargetBee { bee = holder });
                            }
                        }
                        else
                        {
                            ecb2.RemoveComponent<TargetResource>(beeEntity);
                        }
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

                velocity.vel.y += field.gravity * deltaTime;
                //deathTimer.dTimer -= deltaTime / 10f;
                deathTimer.dTimer -= deltaTime;
                Debug.Log("bee dead, deathTimer.dTimer = " + deathTimer.dTimer);
                if (deathTimer.dTimer < 0f)
                {
                    Debug.Log("bee should disappear!!!");
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
            .WithName("Bee_Adjust_Move")
            .WithAll<BeeTeam>()
            .ForEach((Entity beeEntity, ref Velocity velocity, ref Translation pos) =>
            {
                float size = 0f;
                if(HasComponent<Size>(beeEntity))
                {
                    size = GetComponent<Size>(beeEntity).value;
                }

                if (math.abs(pos.Value.x) > field.size.x * .5f)
                {
                    pos.Value.x = field.size.x * .5f * math.sign(pos.Value.x);
                    velocity.vel.x *= -.5f;
                    velocity.vel.y *= .8f;
                    velocity.vel.z *= .8f;
                }

                if (math.abs(pos.Value.z) > field.size.z * .5f)
                {
                    pos.Value.z = field.size.z * .5f * math.sign(pos.Value.z);
                    velocity.vel.z *= -.5f;
                    velocity.vel.x *= .8f;
                    velocity.vel.y *= .8f;
                }

                float resModifier = 0f;
                if (HasComponent<IsHoldingResource>(beeEntity))
                {
                    resModifier = resParams.resourceSize;
                }

                if (math.abs(pos.Value.y) > field.size.y * .5f - resModifier - size * .5f)
                {
                    pos.Value.y = (field.size.y * .5f - resModifier - size * .5f) * math.sign(pos.Value.y);
                    velocity.vel.y *= -.5f;
                    velocity.vel.x *= .8f;
                    velocity.vel.z *= .8f;
                }
                
            }).ScheduleParallel();

        /* --------------------------------------------------------------------------------- */

        Entities
            .WithName("Bee_Smooth_Direction")
            .WithAll<BeeTeam>()
            .ForEach((Entity beeEntity, ref SmoothPosition smoothPos, ref SmoothDirection smoothDir, 
                        //ref NonUniformScale scale, ref URPMaterialPropertyBaseColor baseColor, 
                        ref NonUniformScale scale, 
                        in Velocity velocity, in Translation pos, in DeathTimer deathTimer) =>
            {
                float3 oldSmPos = smoothPos.smPos;
                if (HasComponent<IsAttacking>(beeEntity) == false)
                {
                    smoothPos.smPos = math.lerp(smoothPos.smPos, pos.Value, deltaTime * beeParams.rotationStiffness);
                }
                else 
                {
                    smoothPos.smPos = pos.Value;
                }

                smoothDir.smDir = smoothPos.smPos - oldSmPos;

                /*
                if(HasComponent<Dead>(beeEntity) == false)
                {
                    float stretch = math.max(1f, math.length(velocity.vel) * beeParams.speedStretch);
                    scale.Value.z *= stretch;
                    scale.Value.x /= (stretch - 1f) / 5f + 1f;
                    scale.Value.y /= (stretch - 1f) / 5f + 1f;
                    
                }

                if(HasComponent<Dead>(beeEntity))
                {
                    //baseColor.Value *= .75f;
                    scale.Value *= math.sqrt(deathTimer.dTimer);
                }
                */

            }).ScheduleParallel();

#if COMMENT
            /* --------------------------------------------------------------------------------- */

        Entities
            .WithName("Bee_Local_To_World_TRS")
            .WithAll<BeeTeam>()
            .ForEach((Entity beeEntity, ref LocalToWorld localToWorld, ref Rotation rotation, 
                        in NonUniformScale scale, in SmoothDirection smoothDir, in Translation translation) =>
            {
                if(!smoothDir.smDir.Equals(float3.zero))
                {
                    rotation.Value = quaternion.LookRotation(smoothDir.smDir, math.up());
                }

                localToWorld.Value = float4x4.TRS(translation.Value, rotation.Value, scale.Value);

            }).ScheduleParallel();
#endif

    }
}
