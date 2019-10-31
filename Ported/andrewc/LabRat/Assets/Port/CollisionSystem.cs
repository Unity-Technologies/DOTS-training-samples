using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Jobs;
using Unity.Transforms;

[AlwaysUpdateSystem]
public class CollisionSystem : JobComponentSystem
{
    EntityQuery m_CatQuery;
    EntityQuery m_MouseQuery;
    BeginInitializationEntityCommandBufferSystem m_EntityCommandBufferSystem;

    NativeMultiHashMap<int2, EntityAndTranslation> m_MiceMap;
    NativeMultiHashMap<int2, EntityAndTranslation> m_CatsMap;

    struct EntityAndTranslation
    {
        public Entity Entity;
        public float3 Translation;
    }

    struct MouseJob : IJobChunk
    {
        public NativeMultiHashMap<int2, EntityAndTranslation>.Concurrent MiceMap;
        [ReadOnly] public ArchetypeChunkComponentType<Translation> TranslationType;
        [ReadOnly] public ArchetypeChunkEntityType EntityType;

        public void Execute(ArchetypeChunk chunk, int chunkIndex, int firstEntityIndex)
        {
            var chunkEntities = chunk.GetNativeArray(EntityType);
            var chunkTranslations = chunk.GetNativeArray(TranslationType);

            for (var i = 0; i < chunk.Count; i++)
            {
                MiceMap.Add(
                    Board.ConvertWorldToTileCoordinates(chunkTranslations[i].Value),
                    new EntityAndTranslation{Entity = chunkEntities[i], Translation = chunkTranslations[i].Value});
            }
        }
    }

    struct CatJob : IJobForEachWithEntity<Translation>
    {
        [ReadOnly] public NativeMultiHashMap<int2, EntityAndTranslation> MiceMap;
        public NativeMultiHashMap<int2, EntityAndTranslation>.Concurrent CatsMap;
        public EntityCommandBuffer.Concurrent CommandBuffer;

        public void Execute(Entity entity, int threadId, [ReadOnly] ref Translation translation)
        {
            int2 tileCoords = Board.ConvertWorldToTileCoordinates(translation.Value);

            var mouse = default(EntityAndTranslation);
            var iter = default(NativeMultiHashMapIterator<int2>);
            if (!MiceMap.TryGetFirstValue(tileCoords, out mouse, out iter))
                return;

            CheckForAndHandleCollision(threadId, entity, translation.Value, mouse);
            while (MiceMap.TryGetNextValue(out mouse, ref iter))
                CheckForAndHandleCollision(threadId, entity, translation.Value, mouse);
        }

        void CheckForAndHandleCollision(int threadId, Entity catEntity, float3 catPosition, EntityAndTranslation mouse)
        {

        }
    }

    protected override void OnCreate()
    {
        m_CatQuery = EntityManager.CreateEntityQuery(ComponentType.ReadOnly<CatTag>(), ComponentType.ReadOnly<Translation>());
        m_MouseQuery = EntityManager.CreateEntityQuery(ComponentType.ReadOnly<MouseTag>(), ComponentType.ReadOnly<Translation>());
        m_EntityCommandBufferSystem = World.GetOrCreateSystem<BeginInitializationEntityCommandBufferSystem>();
    }

    protected override JobHandle OnUpdate(JobHandle deps)
    {
        DisposePerFrameMemory();

        m_MiceMap = new NativeMultiHashMap<int2, EntityAndTranslation>(m_MouseQuery.CalculateEntityCount(), Allocator.TempJob);
        m_CatsMap = new NativeMultiHashMap<int2, EntityAndTranslation>(m_CatQuery.CalculateEntityCount(), Allocator.TempJob);

        var mouseJobHandle = new MouseJob
        {
            MiceMap = m_MiceMap.ToConcurrent(),
            TranslationType = GetArchetypeChunkComponentType<Translation>(true),
            EntityType = GetArchetypeChunkEntityType()
        };
        deps = mouseJobHandle.Schedule(m_MouseQuery, deps);

        var catJobHandle = new CatJob
        {
            MiceMap = m_MiceMap,
            CatsMap = m_CatsMap.ToConcurrent(),
            CommandBuffer = m_EntityCommandBufferSystem.CreateCommandBuffer().ToConcurrent()
        };
        deps = catJobHandle.Schedule(m_CatQuery, deps);
        m_EntityCommandBufferSystem.AddJobHandleForProducer(deps);

        return deps;
    }

    protected override void OnDestroy()
    {
        DisposePerFrameMemory();
    }

    void DisposePerFrameMemory()
    {
        if (m_MiceMap.IsCreated)
            m_MiceMap.Dispose();

        if (m_CatsMap.IsCreated)
            m_CatsMap.Dispose();
    }
}
