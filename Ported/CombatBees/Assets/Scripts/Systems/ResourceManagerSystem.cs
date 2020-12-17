using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

[UpdateAfter(typeof(BeeSpawnerSystem))]
[UpdateAfter(typeof(ResourceSpawnerSystem))]
[UpdateAfter(typeof(BeeManagerSystem))]
[UpdateBefore(typeof(TransformSystemGroup))]
public class ResourceManagerSystem : SystemBase
{
    private EntityQuery resQuery;

    protected override void OnCreate()
    {
        resQuery = GetEntityQuery(new EntityQueryDesc
        {
            All = new ComponentType[] { typeof(StackIndex) }
        });
    }

    protected override void OnUpdate()
    {
        var beeParams = GetSingleton<BeeControlParams>();
        var field = GetSingleton<FieldAuthoring>();
        var resParams = GetSingleton<ResourceParams>();
        var particleParams = GetSingleton<ParticleParams>();
        var resGridParams = GetSingleton<ResourceGridParams>();
        var bufferFromEntity = GetBufferFromEntity<StackHeightParams>();
        var bufferEntity = GetSingletonEntity<ResourceParams>();
        var stackHeights = bufferFromEntity[bufferEntity];

        float deltaTime = Time.fixedDeltaTime;

#if COMMENT
        NativeArray<Entity> resArray = resQuery.ToEntityArrayAsync(Allocator.TempJob, out var resHandle);
        resHandle.Complete();

        /*
        if (resArray.Length < 1000 && MouseRaycaster.isMouseTouchingField)
        {
            if (Input.GetKey(KeyCode.Mouse0))
            {
                spawnTimer += Time.deltaTime;
                while (spawnTimer > 1f / spawnRate)
                {
                    spawnTimer -= 1f / spawnRate;
                    SpawnResource(MouseRaycaster.worldMousePosition);
                }
            }
        }
        */
        resArray.Dispose();
#endif

        /* --------------------------------------------------------------------------------- */

        /*
        var ecb0 = new EntityCommandBuffer(Allocator.TempJob);
        Entities
            .WithName("Resource_Is_Dead")
            .WithAll<StackIndex>()
            .WithAll<Dead>()
            .ForEach((Entity resEntity, ref DeathTimer deathTimer) =>
            {
                deathTimer.dTimer -= 5f * deltaTime;
                if (deathTimer.dTimer < 0f)
                {
                    ecb0.DestroyEntity(resEntity);
                }

            }).Run();
        ecb0.Playback(EntityManager);
        ecb0.Dispose();
        */

        /* --------------------------------------------------------------------------------- */

        var ecb = new EntityCommandBuffer(Allocator.TempJob);
        Entities
            .WithName("Resource_Has_Holder")
            .ForEach((Entity resEntity, in HolderBee holderBee) =>
            {
                if (HasComponent<Dead>(holderBee.holder))
                {
                    ecb.RemoveComponent<HolderBee>(resEntity);
                }
                else
                {
                    float3 pos = GetComponent<Translation>(resEntity).Value;
                    float3 holderPos = GetComponent<Translation>(holderBee.holder).Value;
                    float holderSize = GetComponent<Size>(holderBee.holder).value;
                    float3 targetPos = holderPos - math.up() * (resParams.resourceSize + holderSize) * .5f;
                    targetPos = math.lerp(pos, targetPos, resParams.carryStiffness * deltaTime);
                    ecb.SetComponent<Translation>(resEntity, new Translation { Value = targetPos });

                    float3 velocity =  GetComponent<Velocity>(holderBee.holder).vel;
                    ecb.SetComponent<Velocity>(resEntity, new Velocity { vel = velocity });
                }
            }).Run();
        ecb.Playback(EntityManager);
        ecb.Dispose();


        /* --------------------------------------------------------------------------------- */

        var ecb1 = new EntityCommandBuffer(Allocator.TempJob);
        Entities
            .WithName("Resource_Not_Stacked")
            .WithNone<HolderBee>()
            .WithNone<Stacked>()
            .WithNone<Dead>()
            .ForEach((Entity resEntity, ref Velocity velocity, ref Translation pos, ref GridX gX, ref GridY gY, ref StackIndex stackIndex) =>
            {
                //Debug.Log("resource not stacked????????????");
                //Debug.Log("before pos = " + pos.Value);

                float3 targetPos = Utils.NearestSnappedPos(resGridParams, pos.Value);
                pos.Value = math.lerp(pos.Value, targetPos, resParams.snapStiffness * deltaTime);
                velocity.vel.y += field.gravity * deltaTime;
                pos.Value += velocity.vel * deltaTime;
                //Debug.Log("after pos = " + pos.Value);

                // get gridX and gridY
                //int x, y;
                Utils.GetGridIndex(resGridParams, pos.Value, out int x, out int y);
                gX.gridX = x;
                gY.gridY = y;
                
                // check resource position x
                if(math.abs(pos.Value.x) > field.size.x * .5f)
                {
                    pos.Value.x = field.size.x * .5f * math.sign(pos.Value.x);
                    velocity.vel.x *= -.5f;
                    velocity.vel.y *= .8f;
                    velocity.vel.z *= .8f;
                }

                // check resource position y
                if (math.abs(pos.Value.y) > field.size.y * .5f)
                {
                    pos.Value.y = field.size.y * .5f * math.sign(pos.Value.y);
                    velocity.vel.y *= -.5f;
                    velocity.vel.x *= .8f;
                    velocity.vel.z *= .8f;
                }

                // check resource position z
                if (math.abs(pos.Value.z) > field.size.z * .5f)
                {
                    pos.Value.z = field.size.z * .5f * math.sign(pos.Value.z);
                    velocity.vel.z *= -.5f;
                    velocity.vel.x *= .8f;
                    velocity.vel.y *= .8f;
                }

                //Debug.Log("pos = " + pos.Value);

                // Get latest buffer
                bufferFromEntity = GetBufferFromEntity<StackHeightParams>();
                stackHeights = bufferFromEntity[bufferEntity];
                float floorY = Utils.GetStackPos(resParams, resGridParams, field, stackHeights, gX.gridX, gY.gridY).y;
                if(pos.Value.y < floorY)
                {
                    //Debug.Log("pos = " + pos.Value + ", floorY = " + floorY + ", !!!!!!!!!!!!!!!!!!");
                    pos.Value.y = floorY;
                    if(math.abs(pos.Value.x) > field.size.x * .4f)
                    {
                        Entity newSpawner = ecb1.CreateEntity();
                        BeeSpawner beeSpawner;
                        if(pos.Value.x < 0f)
                        {
                            beeSpawner = new BeeSpawner
                            {
                                count = resParams.beesPerResource,
                                maxSpawnSpeed = beeParams.maxSpawnSpeed,
                                team = BeeTeam.TeamColor.BLUE
                            };
                        }
                        else
                        {
                            beeSpawner = new BeeSpawner
                            {
                                count = resParams.beesPerResource,
                                maxSpawnSpeed = beeParams.maxSpawnSpeed,
                                team = BeeTeam.TeamColor.YELLOW
                            };
                        }

                        ecb1.AddComponent<Translation>(newSpawner, pos);
                        ecb1.AddComponent(newSpawner, beeSpawner);

                        //////////////////////////// ToDo, spawn Falash particle
                        //ParticleManager.SpawnParticle(resource.position, ParticleType.SpawnFlash, Vector3.zero, 6f, 5);
                        var spawnerEntity = ecb1.CreateEntity();
                        var particleSpawner = new ParticleSpawner
                        {
                            type = ParticleType.Type.Flash,
                            count = particleParams.numberOfFlashParticles,
                            velocity = float3.zero,
                            velocityJitter = 6f
                        };
                        ecb1.AddComponent<Translation>(spawnerEntity, pos);
                        ecb1.AddComponent<ParticleSpawner>(spawnerEntity, particleSpawner);

                        ecb1.AddComponent<Dead>(resEntity);
                        // destory later to avoid race condition in BeeManagerSystem
                        //ecb1.DestroyEntity(resEntity);
                    }
                    else
                    {
                        ecb1.AddComponent<Stacked>(resEntity);
                        int heightIndex = gX.gridX * resGridParams.gridCounts.y + gY.gridY;

                        // Get latest buffer
                        bufferFromEntity = GetBufferFromEntity<StackHeightParams>();
                        stackHeights = bufferFromEntity[bufferEntity];
                        stackIndex.index = stackHeights[heightIndex].Value;
                        if((stackIndex.index + 1) * resParams.resourceSize < field.size.y)
                        {
                            Utils.UpdateStackHeights(resGridParams, stackHeights, gX.gridX, gY.gridY, true, 1);
                        }
                        else
                        {
                            ecb1.AddComponent<Dead>(resEntity);
                            //ecb1.DestroyEntity(resEntity);
                        }
                    }
                }
            }).Run();
        ecb1.Playback(EntityManager);
        ecb1.Dispose();

        Entities
            .WithName("Resource_Local_To_World_TRS")
            .WithNone<Dead>()
            .WithAll<StackIndex>()
            .ForEach((Entity resEntity, ref LocalToWorld localToWorld, in Scale scale, in Translation translation) =>
            {
                localToWorld.Value = float4x4.TRS(translation.Value, quaternion.identity, scale.Value);

            }).ScheduleParallel();


    }
}


[UpdateAfter(typeof(BeeSpawnerSystem))]
[UpdateAfter(typeof(ResourceSpawnerSystem))]
[UpdateAfter(typeof(BeeManagerSystem))]
[UpdateAfter(typeof(ResourceManagerSystem))]
[UpdateBefore(typeof(TransformSystemGroup))]
public class ResourceDeathSystem : SystemBase
{
    protected override void OnUpdate()
    {
        float deltaTime = Time.fixedDeltaTime;
        var ecb0 = new EntityCommandBuffer(Allocator.TempJob);
        Entities
            .WithName("Resource_Is_Dead")
            .WithAll<StackIndex>()
            .WithAll<Dead>()
            .ForEach((Entity resEntity, ref DeathTimer deathTimer) =>
            {
                deathTimer.dTimer -= 5f * deltaTime;
                if (deathTimer.dTimer < 0f)
                {
                    ecb0.DestroyEntity(resEntity);
                }

            }).Run();
        ecb0.Playback(EntityManager);
        ecb0.Dispose();
    }
}

[UpdateAfter(typeof(BeeSpawnerSystem))]
[UpdateAfter(typeof(ResourceSpawnerSystem))]
[UpdateAfter(typeof(BeeManagerSystem))]
[UpdateAfter(typeof(ResourceManagerSystem))]
[UpdateBefore(typeof(TransformSystemGroup))]
public class MouseRayCasterSystem : SystemBase
{
    Camera camera;
    Entity marker;

    private EntityCommandBufferSystem ECBSystem;

    static bool init = false;

    protected override void OnCreate()
    {
        ECBSystem = World.GetExistingSystem<EndSimulationEntityCommandBufferSystem>();
    }

    protected override void OnUpdate()
    {
        /*
        var resParams = GetSingleton<ResourceParams>();

        var field = GetSingleton<FieldAuthoring>();
        int numRes = 0;
        float spawnRate = resParams.spawnRate;
        float spawnTimer = 0;

        ECBSystem = World.GetExistingSystem<EndSimulationEntityCommandBufferSystem>();
        var ecb = ECBSystem.CreateCommandBuffer();

        if(init == false)
        {
            marker = ecb.Instantiate(resParams.markerPrefab);
            ecb.AddComponent<Disabled>(marker);

            camera = Camera.main;
            init = true;
        }

        bool markerActive = Utils.MouseRayTrace(camera, field, out float3 pos);
        if (markerActive)
        {
            if (HasComponent<Disabled>(marker))
            {
                ecb.RemoveComponent<Disabled>(marker);
            }

            if (Input.GetKey(KeyCode.Mouse0))
            {
                spawnTimer += Time.DeltaTime;
                while ((numRes < resParams.maxGeneratedByMouseClick) && (spawnTimer > 1f / spawnRate))
                {
                    spawnTimer -= 1f / spawnRate;
                    numRes++;
                    //SpawnResource(MouseRaycaster.worldMousePosition);
                }
            }
        }
        else
        {
            if (HasComponent<Disabled>(marker) == false)
            {
                ecb.AddComponent<Disabled>(marker);
            }
        }
        
        if(numRes > 0)
        {
            var spawnerEntity = ecb.CreateEntity();
            var resSpawner = new ResourceSpawner
            {
                count = numRes,
                isPosRandom = false
            };
            ecb.AddComponent(spawnerEntity, resSpawner);
            ecb.AddComponent(spawnerEntity, new Translation { Value = pos });
        }

        ecb.Playback(EntityManager);
        ecb.Dispose();
        */
    }
}


