using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using static Unity.Mathematics.math;
using Unity.Transforms;

[UpdateAfter(typeof(BeeSpawnerSystem))]
[UpdateAfter(typeof(ResourceSpawnerSystem))]
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
            .WithNone<Stacked>()
            .ForEach((Entity resEntity, ref Velocity velocity, ref Translation pos, ref GridX gX, ref GridY gY, ref StackIndex stackIndex) =>
            {
                float3 targetPos = Utils.NearestSnappedPos(resGridParams, pos.Value);
                pos.Value = math.lerp(pos.Value, targetPos, resParams.snapStiffness * deltaTime);
                velocity.vel.y += field.gravity * deltaTime;
                pos.Value += velocity.vel * deltaTime;

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

                float floorY = Utils.GetStackPos(resParams, resGridParams, field, stackHeights, gX.gridX, gY.gridY).y;
                if(pos.Value.y < floorY)
                {
                    pos.Value.y = floorY;
                    if(math.abs(pos.Value.x) > field.size.x * .4f)
                    {
                        Entity beeSpawnerPrefab;
                        if(pos.Value.x < 0f)
                        {
                            beeSpawnerPrefab = beeParams.blueSpawnerPrefab;
                        }
                        else
                        {
                            beeSpawnerPrefab = beeParams.yellowSpawnerPrefab;
                        }

                        var spawner = ecb.Instantiate(beeSpawnerPrefab);
                        var beeSpawner = new BeeSpawner
                        {
                            beePrefab = beeSpawnerPrefab,
                            count = resParams.beesPerResource,
                            maxSpawnSpeed = beeParams.maxSpawnSpeed
                        };

                        ecb1.SetComponent<Translation>(spawner, pos);
                        ecb1.SetComponent(spawner, beeSpawner);

                        //////////////////////////// ToDo, spawn Falash particle
                        //ParticleManager.SpawnParticle(resource.position, ParticleType.SpawnFlash, Vector3.zero, 6f, 5);

                        ecb1.DestroyEntity(resEntity);
                    }
                    else
                    {
                        ecb1.AddComponent<Stacked>(resEntity);
                        int heightIndex = gX.gridX * resGridParams.gridCounts.y + gY.gridY;
                        stackIndex.index = stackHeights[heightIndex].Value;
                        if((stackIndex.index + 1) * resParams.resourceSize < field.size.y)
                        {
                            Utils.UpdateStackHeights(resGridParams, stackHeights, gX.gridX, gY.gridY, true, 1);
                        }
                        else
                        {
                            ecb1.DestroyEntity(resEntity);
                        }
                    }
                }
            }).Run();
        ecb1.Playback(EntityManager);
        ecb1.Dispose();

    }
}