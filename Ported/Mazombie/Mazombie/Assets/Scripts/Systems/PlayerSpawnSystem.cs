using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

[UpdateInGroup(typeof(InitializationSystemGroup))]
[UpdateAfter(typeof(MazeGeneratorSystem))]
[BurstCompile]
public partial struct PlayerSpawnSystem : ISystem
{
    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<PlayerSpawn>();
    }
    
    [BurstCompile]
    public void OnDestroy(ref SystemState state)
    {
    }
    
    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        var gameConfig = SystemAPI.GetSingleton<GameConfig>();

        var ecb = new EntityCommandBuffer(Allocator.Temp);
        foreach (var localToWorld in SystemAPI.Query<RefRO<LocalToWorldTransform>>().WithAll<PlayerSpawn>())
        {
            var playerEntity = ecb.Instantiate(gameConfig.playerPrefab);
            ecb.SetComponent(playerEntity, new LocalToWorldTransform
            {
                Value = UniformScaleTransform.FromPosition(localToWorld.ValueRO.Value.Position + math.up() * 0.5f)
            });
            ecb.AddComponent<Player>(playerEntity);
            break;
        }
        
        ecb.Playback(state.EntityManager);
        ecb.Dispose();
        
        state.Enabled = false;
    }
}