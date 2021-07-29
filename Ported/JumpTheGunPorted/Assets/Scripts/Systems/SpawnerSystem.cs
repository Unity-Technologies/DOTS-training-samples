using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

public class SpawnerSystem : SystemBase
{
    private EntityQuery _PlayerEntityQuery;
    private EntityQuery _TankEntityQuery;
    private EntityQuery _BoxEntityQuery;
    private EntityQuery _BoxMapEntityQuery;

    private Entity _BoxMapEntity;

    protected override void OnCreate()
    {
        _PlayerEntityQuery = EntityManager.CreateEntityQuery(typeof(Player));
        _TankEntityQuery = EntityManager.CreateEntityQuery(typeof(LookAtPlayer));

        _BoxEntityQuery = EntityManager.CreateEntityQuery(typeof(NonUniformScale)); // TODO: Reinforce this maybe
        _BoxMapEntityQuery =
            EntityManager.CreateEntityQuery(typeof(HeightBufferElement), typeof(OccupiedBufferElement));

        _BoxMapEntity = EntityManager.CreateEntity(typeof(HeightBufferElement), typeof(OccupiedBufferElement));

        RequireSingletonForUpdate<GameObjectRefs>();
        RequireSingletonForUpdate<HeightBufferElement>();
    }

    protected override void OnUpdate()
    {
        var ecb = new EntityCommandBuffer(Allocator.Temp);

        // The PRNG (pseudorandom number generator) from Unity.Mathematics is a struct
        // and can be used in jobs. For simplicity and debuggability in development,
        // we'll initialize it with a constant. (In release, we'd want a seed that
        // randomly varies, such as the time from the user's system clock.)
        var random = new Random(1234);

        var heightMapBuffer = EntityManager.GetBuffer<HeightBufferElement>(_BoxMapEntity);
        var occupiedMapBuffer = EntityManager.GetBuffer<OccupiedBufferElement>(_BoxMapEntity);

        var refs = this.GetSingleton<GameObjectRefs>();
        var boxPrefab = refs.BoxPrefab;
        var playerPrefab = refs.PlayerPrefab;
        var tankPrefab = refs.TankPrefab;
        var barrelPrefab = refs.BarrelPrefab;

        var config = refs.Config.Data;
        var tankCnt = config.TankCount;
        var reloadTime = config.TankReloadTime;

        Entities
            .WithAll<Spawner>()
            .WithoutBurst()
            .ForEach((Entity entity) =>
            {
                Config.Instance.TimeVal = 0.0f;

                ecb.DestroyEntity(entity);

                // build terrain
                heightMapBuffer.Clear();
                occupiedMapBuffer.Clear();
                ecb.DestroyEntitiesForEntityQuery(_BoxEntityQuery);
                for (int i = 0; i < config.TerrainWidth; ++i)
                {
                    for (int j = 0; j < config.TerrainLength; ++j)
                    {
                        float height = random.NextFloat(config.MinTerrainHeight, config.MaxTerrainHeight);
                        heightMapBuffer.Add((HeightBufferElement) height);
                        occupiedMapBuffer.Add((OccupiedBufferElement) false);

                        // We don't need to set the other components here, BoxHeightSystem will handle that
                        var box = ecb.Instantiate(boxPrefab);
                        ecb.SetComponent(box, new Translation
                        {
                            Value = new float3(j, height / 2, i) // reposition halfway to height to level all at 0 plane
                        });
                    }
                }

                // new fresh player at proper x/y and height and empty parabola t value
                ecb.DestroyEntitiesForEntityQuery(_PlayerEntityQuery);
                var player = ecb.Instantiate(playerPrefab);
                int playerX = config.TerrainLength / 2;
                int playerY = config.TerrainWidth / 2;
                float playerHeight =
                    heightMapBuffer[playerY * config.TerrainLength + playerX] +
                    Player.Y_OFFSET; // use box height map to figure out starting y
                ecb.SetComponent(player, new Translation
                {
                    Value = new float3(playerX, playerHeight, playerY)
                });
                ecb.AddComponent(player, new ParabolaTValue
                {
                    Value = -1 // less than zero will trigger recalculate the next bounce parabola
                });

                // make tanks!
                ecb.DestroyEntitiesForEntityQuery(
                    _TankEntityQuery); // find all things with LookAtPlayer which is both tank base and turret
                for (int i = 0; i < tankCnt; ++i)
                {
                    var tank = ecb.Instantiate(tankPrefab);
                    var barrel = ecb.Instantiate(barrelPrefab);
                    var tankX = random.NextInt(0, config.TerrainLength - 1);
                    var tankY = random.NextInt(0, config.TerrainWidth - 1);
                    int safetyCheckMax = config.TerrainLength * config.TerrainWidth;
                    int
                        safetyCheckCnt =
                            0; // we count how many loops below randomly picking a new unoccupied tile so this is just a sanity check to exit out if we try too long
                    while (occupiedMapBuffer[tankY * config.TerrainLength + tankX] && safetyCheckCnt++ < safetyCheckMax)
                    {
                        tankX = random.NextInt(0, config.TerrainLength - 1);
                        tankY = random.NextInt(0, config.TerrainWidth - 1);
                    }

                    if (safetyCheckCnt >= safetyCheckMax)
                    {
                        UnityEngine.Debug.LogWarning("Couldn't find an empty box position for tank " +
                                                     i); // NOTE: we could do a full iteration of the entire grid lookig for an empty one too, but mehhh
                    }

                    occupiedMapBuffer[tankY * config.TerrainLength + tankX] = true; // mark this tile as occupied
                    float tankHeight =
                        heightMapBuffer[tankY * config.TerrainLength + tankX] +
                        TankBase.Y_OFFSET; // use box height map to figure out starting y
                    ecb.SetComponent(tank, new Translation
                    {
                        Value = new float3(tankX, tankHeight, tankY)
                    });

                    ecb.SetComponent(barrel, new Translation
                    {
                        Value = new float3(tankX, tankHeight + TankBase.TURRET__Y_OFFSET, tankY)
                    });
                    ecb.SetComponent(barrel, new FiringTimer
                    {
                        NextFiringTime = (float) random.NextDouble(0, reloadTime)
                    });
                }
            }).Run();

        ecb.Playback(EntityManager);
        ecb.Dispose();
    }
}
