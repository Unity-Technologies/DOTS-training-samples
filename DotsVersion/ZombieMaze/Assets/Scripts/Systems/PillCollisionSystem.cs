using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

[BurstCompile]
partial struct PillCollisionSystem : ISystem
{
    EntityQuery Query;

    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<PlayerData>();

        EntityQueryBuilder builder = new EntityQueryBuilder(Allocator.Temp);
        builder.WithAny<RotationComponent>();
        Query = state.GetEntityQuery(builder);
    }

    // Every function defined by ISystem has to be implemented even if empty.
    [BurstCompile]
    public void OnDestroy(ref SystemState state)
    {
    }

    // See note above regarding the [BurstCompile] attribute.
    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        Entity mazeConfigEntity = SystemAPI.GetSingletonEntity<MazeConfig>();
        MazeConfig mazeConfig = SystemAPI.GetSingleton<MazeConfig>();


        NativeArray<ArchetypeChunk> chunks = Query.ToArchetypeChunkArray(Allocator.Temp);

        Entity playerEntity = SystemAPI.GetSingletonEntity<PlayerData>();
        PlayerData playerData = SystemAPI.GetSingleton<PlayerData>();
        TransformAspect playerTransform = SystemAPI.GetAspectRW<TransformAspect>(playerEntity);
        float3 playerPosition = playerTransform.Position;

        ComponentTypeHandle<LocalToWorld> positionHandle = state.EntityManager.GetComponentTypeHandle<LocalToWorld>(true);
        EntityTypeHandle entityHandle = state.EntityManager.GetEntityTypeHandle();
        EntityCommandBuffer ecb = new EntityCommandBuffer(Allocator.Temp);

        for (int i = 0; i < chunks.Length; ++i)
        {
            var chunk = chunks[i];
            var positions = chunk.GetNativeArray(positionHandle);
            var entities = chunk.GetNativeArray(entityHandle);

            for (int j = 0; j < chunk.Count; ++j)
            {
                var distance = math.distancesq(playerPosition, positions[j].Position);
                if (distance < 0.1f)
                {
                    if(++playerData.PillsCollected >= mazeConfig.PillsToSpawn)
                    {
                        playerData.PillsCollected = 0;
                        playerTransform.Position = new float3(playerData.spawnerPos.x, playerTransform.Position.y, playerData.spawnerPos.z);

                        mazeConfig.SpawnPills = true;
                        ecb.SetComponent<MazeConfig>(mazeConfigEntity, mazeConfig);
                    }
                    ecb.SetComponent<PlayerData>(playerEntity, playerData);

                    ecb.DestroyEntity(entities[j]);
                }
            }
        }

        ecb.Playback(state.EntityManager);
        ecb.Dispose();
    }
}
