using Unity.Burst;
using Unity.Burst.Intrinsics;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;


[BurstCompile]
public struct DetectFoodDeliveryJob : IJobChunk
{
    public EntityCommandBuffer.ParallelWriter ECB;

    [ReadOnly] public ComponentTypeHandle<LocalToWorldTransform> TransformHandle;
    [ReadOnly] public EntityTypeHandle EntityHandle;
    [ReadOnly] public AABB FieldArea;

    public void Execute(in ArchetypeChunk chunk, int unfilteredChunkIndex, bool useEnabledMask, in v128 chunkEnabledMask)
    {
        var chunkEntityEnumerator = new ChunkEntityEnumerator(useEnabledMask, chunkEnabledMask, chunk.ChunkEntityCount);
        var transforms = chunk.GetNativeArray(TransformHandle);
        var entities = chunk.GetNativeArray(EntityHandle);

        float k_eps = 1e-2f;
        while (chunkEntityEnumerator.NextEntityIndex(out var i))
        {
            var tmComp = transforms[i];
            // check if food has reached the ground.
            // if yes, remove it and issue a bee spawn request

            bool onGround = tmComp.Value.Position.y < FieldArea.Min.y + k_eps;

            if (onGround)
            {
                // todo: issue spawn request
                // SpawningRequest.CreateBeeSpawnRequest( SomeStruct{ ECB, prefab, number, region } );

                ECB.DestroyEntity(i, entities[i]);
            }
        }
    }
}
    
[BurstCompile]
partial struct FoodSystem : ISystem
{
    private EntityQuery FoodQuery;
    public ComponentTypeHandle<LocalToWorldTransform> TransformHandle;
    public EntityTypeHandle EntityHandle;

    public void OnCreate(ref SystemState state)
    {
        FoodQuery = SystemAPI.QueryBuilder().WithAll<Food, Faction, LocalToWorldTransform>().Build();
        TransformHandle = state.GetComponentTypeHandle<LocalToWorldTransform>();

        state.RequireForUpdate<BeeConfig>();
    }

    public void OnDestroy(ref SystemState state)
    {
    }

    public void OnUpdate(ref SystemState state)
    {
        var config = SystemAPI.GetSingleton<BeeConfig>();
        var ecbSingleton = SystemAPI.GetSingleton<BeginSimulationEntityCommandBufferSystem.Singleton>();

        TransformHandle.Update(ref state);
        EntityHandle.Update(ref state);

        var combinedHandle = new JobHandle(); 
        for (int i = (int) Factions.Team1; i < (int) Factions.NumFactions; i++)
        {
            FoodQuery.SetSharedComponentFilter( new Faction { Value = i } );
            var ecb = ecbSingleton.CreateCommandBuffer(state.WorldUnmanaged); 

            var job = new DetectFoodDeliveryJob()
            {
                ECB = ecb.AsParallelWriter(),
                TransformHandle = TransformHandle,
                EntityHandle = EntityHandle,
                FieldArea = config.fieldArea
            };

            var jobHandle = job.ScheduleParallel(FoodQuery, state.Dependency);
            combinedHandle = JobHandle.CombineDependencies(jobHandle, combinedHandle);
        }

        state.Dependency = combinedHandle;
    }
}