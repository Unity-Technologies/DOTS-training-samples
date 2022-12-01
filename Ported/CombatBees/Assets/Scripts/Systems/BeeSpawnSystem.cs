using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Rendering;
using Unity.Transforms;

[BurstCompile]
partial struct BeeSpawnSystem : ISystem
{
    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<Config>();
    }

    [BurstCompile]
    public void OnDestroy(ref SystemState state)
    {
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        var config = SystemAPI.GetSingleton<Config>();
        var beeSizeHalfRange = (config.maximumBeeSize - config.minimumBeeSize) * .5f;
        var beeSizeMiddle = config.minimumBeeSize + beeSizeHalfRange;
        var ecbSingleton = SystemAPI.GetSingleton<BeginSimulationEntityCommandBufferSystem.Singleton>();
        var ecb = ecbSingleton.CreateCommandBuffer(state.WorldUnmanaged);

        foreach(var (hive, team) in SystemAPI.Query<RefRO<Hive>, Team>())
        {
            var bees = new NativeArray<Entity>(config.startBeeCount, Allocator.Temp);
            ecb.Instantiate(config.beePrefab, bees);

            if (team.number != 0)
            {
                ecb.SetSharedComponent(bees, new Team()
                {
                    number = team.number
                });
            }
            var hiveValue = hive.ValueRO;
            var color = new URPMaterialPropertyBaseColor { Value = hiveValue.color };

            foreach (var bee in bees)
            {
                ecb.SetComponent(bee, color);
                var pos = hiveValue.boundsPosition;
                pos.y = bee.Index;
                var position = hiveValue.boundsPosition;
                // position.x += noise.cnoise(pos / 10f) * hiveValue.boundsExtents.x;
                // position.y += noise.cnoise(pos / 11f) * hiveValue.boundsExtents.y;
                // position.z += noise.cnoise(pos / 12f) * hiveValue.boundsExtents.z;
                var scaleRandom = math.clamp(noise.cnoise(pos / 13f) * 2f, -1f, 1f);
                var scaleDelta = scaleRandom * beeSizeHalfRange;
                var scale = math.clamp(scaleDelta + beeSizeMiddle,
                    config.minimumBeeSize, config.maximumBeeSize);
                ecb.SetComponent(bee, new LocalTransform
                {
                    Position = position,
                    Scale = scale,
                    Rotation = quaternion.identity
                });
                ecb.SetComponent(bee, new WorldTransform()
                {
                    Position = position,
                    Scale = scale,
                    Rotation = quaternion.identity
                });

                ecb.SetComponent(bee, new BeeState
                {
                    beeState = BeeStateEnumerator.Idle,
                    velocity = float3.zero
                });
                ecb.SetComponent<LocalToWorld>(bee, new LocalToWorld());
            }
        }

        state.Enabled = false;
    }
}