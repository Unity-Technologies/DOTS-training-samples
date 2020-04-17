using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;


public class BrigadeFindSourceSystem : SystemBase
{
    private EntityCommandBufferSystem m_ECBSystem;
    NativeArray<float2> resourcePositions;
    NativeArray<Entity> resourceEntities;
    EntityQuery lineQuery;
    EntityQuery resourceQuery;

    protected override void OnCreate()
    {
        lineQuery = GetEntityQuery(new EntityQueryDesc()
        {
            All = new[] { ComponentType.ReadOnly<BrigadeLine>() },
            None = new[] { ComponentType.ReadOnly<ResourceSourcePosition>() }
        });
        resourceQuery = GetEntityQuery(new EntityQueryDesc()
        {
            All = new[] { ComponentType.ReadOnly<ResourceAmount>() },
            None = new[] { ComponentType.ReadOnly<ResourceSourcePosition>() }
        });
        m_ECBSystem = World.GetExistingSystem<EndSimulationEntityCommandBufferSystem>();
    }

    protected override void OnDestroy()
    {
        resourceEntities.Dispose();
        resourcePositions.Dispose();
    }

    [BurstCompile(Debug = true)]
    struct FindSourceJob : IJobChunk
    {
        public ArchetypeChunkEntityType entityType;
        public ArchetypeChunkComponentType<BrigadeLine> LineType;
        public NativeArray<bool> resourceClaimed;
        public NativeArray<ResourceAmount> resourceAmounts;
        public NativeArray<float2> resourcePositions;
        public NativeArray<Entity> resourceEntities;
        //[ReadOnly]
        public EntityCommandBuffer CommandBuffer;
        public void Execute(ArchetypeChunk chunk, int chunkIndex, int firstEntityIndex)
        {
            var lineEntities = chunk.GetNativeArray(entityType);
            var lines = chunk.GetNativeArray(LineType);
            for (var li = 0; li < chunk.Count; li++)
            {
                var line = lines[li];

                var bestDist = float.MaxValue;
                var bestPos = float2.zero;
                var bestIndex = -1;
                for (int i = 0; i < resourceEntities.Length; i++)
                {
                    var resourceEntity = resourceEntities[i];
                    if (resourceClaimed[i] || resourceAmounts[i].Value <= 0)
                        continue;

                    var rp = resourcePositions[i];
                    var d2 = math.distancesq(rp, line.Center);
                    if (d2 < bestDist)
                    {
                        bestIndex = i;
                        bestDist = d2;
                        bestPos = rp;
                    }
                }
                if (bestIndex >= 0)
                {
                    var e = lineEntities[li];
                    CommandBuffer.RemoveComponent<BrigadeLineEstablished>(e);
                    CommandBuffer.AddComponent(e, new ResourceSourcePosition() { Value = bestPos, Id = resourceEntities[bestIndex] });
                    CommandBuffer.AddComponent<ResourceClaimed>(resourceEntities[bestIndex]);
                }
            }
        }
    }

    protected override void OnUpdate()
    {
        if (!resourceEntities.IsCreated)
        {
            var resourceQuery = GetEntityQuery(typeof(ResourceAmount));
            resourceEntities = resourceQuery.ToEntityArray(Allocator.Persistent);
            resourcePositions = new NativeArray<float2>(resourceEntities.Length, Allocator.Persistent);
            for (int i = 0; i < resourceEntities.Length; i++)
            {
                var pos = EntityManager.GetComponentData<Translation>(resourceEntities[i]).Value;
                resourcePositions[i] = new float2(pos.x, pos.z);
            }
        }

        var count = lineQuery.CalculateChunkCount();
        if (count == 0)
            return;

        var amounts = resourceQuery.ToComponentDataArrayAsync<ResourceAmount>(Allocator.TempJob, out var fireTargetQueryHandle);
        Dependency = JobHandle.CombineDependencies(Dependency, fireTargetQueryHandle);
        var resourceClaimed = new NativeArray<bool>(resourceEntities.Length, Allocator.TempJob);
        for (int i = 0; i < resourceEntities.Length; i++)
            resourceClaimed[i] = EntityManager.HasComponent<ResourceClaimed>(resourceEntities[i]);
        var lineType = GetArchetypeChunkComponentType<BrigadeLine>(true);
        var ecb = m_ECBSystem.CreateCommandBuffer();
        var job = new FindSourceJob()
        {
            CommandBuffer = ecb,
            resourceAmounts = amounts,
            resourceClaimed = resourceClaimed,
            entityType = GetArchetypeChunkEntityType(),
            resourceEntities = resourceEntities,
            resourcePositions = resourcePositions,
            LineType = lineType
        };
        Dependency = job.ScheduleSingle(lineQuery, Dependency);
        Dependency = amounts.Dispose(Dependency);
        Dependency = resourceClaimed.Dispose(Dependency);
        m_ECBSystem.AddJobHandleForProducer(Dependency);
    }

    /*
    //NOTE: this needs to be run in a single job to ensure that multiple lines do not claim the same resource
    protected override void OnUpdate()
    {
        if (!resourceEntities.IsCreated)
        {
            var resourceQuery = GetEntityQuery(typeof(ResourceAmount));
            resourceEntities = resourceQuery.ToEntityArray(Allocator.Persistent);
            resourcePositions = new NativeArray<float2>(resourceEntities.Length, Allocator.Persistent);
            for (int i = 0; i < resourceEntities.Length; i++)
            {
                var pos = EntityManager.GetComponentData<Translation>(resourceEntities[i]).Value;
                resourcePositions[i] = new float2(pos.x, pos.z);
            }
        }

        var ecb = new EntityCommandBuffer();//m_ECBSystem.CreateCommandBuffer();
        var resEntities = resourceEntities;
        var resPositions = resourcePositions;

        Entities
            .WithoutBurst()
            .WithNone<ResourceSourcePosition>()
            .ForEach((Entity e, in BrigadeLine line) =>
            {
                var bestDist = float.MaxValue;
                var bestPos = float2.zero;
                var bestIndex = -1;
                for (int i = 0; i < resEntities.Length; i++)
                {
                    var resourceEntity = resEntities[i];
                    if (HasComponent<ResourceClaimed>(resourceEntity))
                        continue;
                    var resourceAmount = GetComponent<ResourceAmount>(resourceEntity);
                    if (resourceAmount.Value <= 0)
                        continue;

                    var rp = resPositions[i];
                    var d2 = math.distancesq(rp, line.Center);
                    if (d2 < bestDist)
                    {
                        bestIndex = i;
                        bestDist = d2;
                        bestPos = rp;
                    }
                }
                if (bestIndex >= 0)
                {
                    ecb.RemoveComponent<BrigadeLineEstablished>(e);
                    ecb.AddComponent(e, new ResourceSourcePosition() { Value = bestPos, Id = resEntities[bestIndex]});
                    ecb.AddComponent<ResourceClaimed>(resEntities[bestIndex]);
                }
            }).Run();
        ecb.Playback(EntityManager);
        //m_ECBSystem.AddJobHandleForProducer(Dependency);
    }*/
}


