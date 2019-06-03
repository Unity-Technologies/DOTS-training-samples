using Unity.Burst;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;
using static Unity.Mathematics.math;

[UpdateInGroup(typeof(InitializationSystemGroup))][UpdateAfter(typeof(Sys_Random_Init))]
public class Sys_Bee_GetTarget : JobComponentSystem
{
    struct Sys_Bee_GetTargetJob : IJobChunk
    {

        [ReadOnly] public ArchetypeChunkEntityType entityType;
        [NativeDisableContainerSafetyRestriction]public ArchetypeChunkComponentType<C_Random> randomType;
        
        public EntityCommandBuffer.Concurrent ecb;

        [ReadOnly] public NativeArray<Entity> Enemies;
        [DeallocateOnJobCompletion][ReadOnly] public NativeArray<Entity> Resources;

        [ReadOnly] public NativeArray<int> StackHeights;
        [ReadOnly] public ComponentDataFromEntity<C_Stack> StackType;
        [ReadOnly] public ComponentDataFromEntity<C_GridIndex> GridType;
        [ReadOnly] public int2 GridCounts;

        [ReadOnly] public float Aggression;

        private int GridIndex(int x, int y)
        {
            return GridCounts.x * y + x;
        }

        public void Execute(ArchetypeChunk chunk, int chunkIndex, int firstEntityIndex)
        {

            var entities = chunk.GetNativeArray(entityType);
            var randoms = chunk.GetNativeArray(randomType);


            for (int i = 0; i < chunk.Count; ++i)
            { 
                if(randoms[i].Generator.NextFloat(0,1) < Aggression && Enemies.Length > 0)
                {
                    C_Target target;
                    target.Value = Enemies[randoms[i].Generator.NextInt(0, Enemies.Length)];

                    C_TargetType targetType;
                    targetType.Type = TargetTypes.Enemy;

                    ecb.AddComponent(chunkIndex, entities[i], target);
                    ecb.SetSharedComponent(chunkIndex, entities[i], targetType);
                }
                else if(Resources.Length > 0 )
                {
                    C_Target target;
                    target.Value = Resources[randoms[i].Generator.NextInt(0, Resources.Length)];

                    C_TargetType targetType;
                    targetType.Type = TargetTypes.Resource;

                    C_GridIndex grid = GridType[target.Value];
                    C_Stack stack = StackType[target.Value];
                    int stackHeight = StackHeights[GridIndex(grid.x, grid.y)];
                    if (stack.index == stackHeight - 1)
                    {
                        ecb.AddComponent(chunkIndex, entities[i], target);
                        ecb.SetSharedComponent(chunkIndex, entities[i], targetType);
                    }
                }
            }
        }
    }

    private EntityCommandBufferSystem m_EntityCommandBufferSystem;

    private EntityQuery m_yellowGroup;
    private EntityQuery m_purpleGroup;
    private EntityQuery m_YellowTargets;
    private EntityQuery m_PurpleTargets;
    private EntityQuery m_resourceTargets;

    protected override void OnCreate()
    {
        m_EntityCommandBufferSystem = World.GetOrCreateSystem<EntityCommandBufferSystem>();

        var queryDesc = new EntityQueryDesc
        {
            None = new ComponentType[] { typeof(C_Target), ComponentType.ReadOnly<Tag_IsDead>(), ComponentType.ReadOnly<Tag_IsDying>(), ComponentType.ReadOnly<C_Holding>() },
            All = new ComponentType[] { ComponentType.ReadOnly<Tag_Bee>(), ComponentType.ReadOnly<C_Shared_Team>(), typeof(C_Random) }
        };

        var targetQueryDesc = new EntityQueryDesc
        {
            None = new ComponentType[] { ComponentType.ReadOnly<Tag_IsDead>(), ComponentType.ReadOnly<Tag_IsDying>(), ComponentType.ReadOnly<Tag_Bee_Init>() },
            All = new ComponentType[] { ComponentType.ReadOnly<Tag_Bee>(), ComponentType.ReadOnly<C_Shared_Team>() }
        };

        var resourceQueryDesc = new EntityQueryDesc
        {
            None = new ComponentType[] {ComponentType.ReadOnly<Tag_IsHeld>()},
            All = new ComponentType[] {ComponentType.ReadOnly<Tag_Resource>(), ComponentType.ReadOnly<C_GridIndex>(), ComponentType.ReadOnly<C_Stack>()}
        };

        m_yellowGroup = GetEntityQuery(queryDesc);
        m_purpleGroup = GetEntityQuery(queryDesc);
        m_YellowTargets = GetEntityQuery(targetQueryDesc);
        m_PurpleTargets = GetEntityQuery(targetQueryDesc);
        m_resourceTargets = GetEntityQuery(resourceQueryDesc);
    }
    
    public static NativeArray<Entity> purpleEntities;
    public static NativeArray<Entity> yellowEntities;
    NativeArray<Entity> purpleResources;
    NativeArray<Entity> yellowResources;

    protected override JobHandle OnUpdate(JobHandle inputDependencies)
    {
        Sys_ResolveStacks.job.Complete();

        m_PurpleTargets.SetFilter(new C_Shared_Team() { Team = Proxy_Bee.Team.PurpleTeam });
        purpleEntities = m_PurpleTargets.ToEntityArray(Allocator.TempJob);

        m_YellowTargets.SetFilter(new C_Shared_Team() { Team = Proxy_Bee.Team.YellowTeam });
        yellowEntities = m_YellowTargets.ToEntityArray(Allocator.TempJob);

        yellowResources = m_resourceTargets.ToEntityArray(Allocator.TempJob);

        JobHandle dependencies = new JobHandle();

        if (yellowEntities.Length > 0 || yellowResources.Length > 0)
        {
            m_yellowGroup.SetFilter(new C_Shared_Team() {Team = Proxy_Bee.Team.YellowTeam});
            var yellowJob = new Sys_Bee_GetTargetJob()
            {
                entityType = GetArchetypeChunkEntityType(),
                randomType = GetArchetypeChunkComponentType<C_Random>(false),

                ecb = m_EntityCommandBufferSystem.CreateCommandBuffer().ToConcurrent(),
                Enemies = purpleEntities,
                Resources = yellowResources,
                StackHeights = ResourceManager.S.StackHeights,
                StackType = GetComponentDataFromEntity<C_Stack>(true),
                GridType = GetComponentDataFromEntity<C_GridIndex>(true),
                GridCounts = ResourceManager.S.GridCounts,
                Aggression = BeeManager.S.Aggression

            }.Schedule(m_yellowGroup, inputDependencies);

            dependencies = yellowJob;
        }
        else
        {
            purpleEntities.Dispose();
            yellowResources.Dispose();
        }


        purpleResources = m_resourceTargets.ToEntityArray(Allocator.TempJob);

        if (purpleEntities.Length > 0 || purpleResources.Length > 0)
        {
            m_purpleGroup.SetFilter(new C_Shared_Team() {Team = Proxy_Bee.Team.PurpleTeam});
            var purpleJob = new Sys_Bee_GetTargetJob()
            {
                entityType = GetArchetypeChunkEntityType(),
                randomType = GetArchetypeChunkComponentType<C_Random>(false),

                ecb = m_EntityCommandBufferSystem.CreateCommandBuffer().ToConcurrent(),
                Enemies = yellowEntities,
                Resources = purpleResources,
                StackHeights = ResourceManager.S.StackHeights,
                StackType = GetComponentDataFromEntity<C_Stack>(true),
                GridType = GetComponentDataFromEntity<C_GridIndex>(true),
                GridCounts = ResourceManager.S.GridCounts,
                Aggression = BeeManager.S.Aggression

            }.Schedule(m_purpleGroup, inputDependencies);

            dependencies = JobHandle.CombineDependencies(purpleJob, dependencies);
        }
        else
        {
            yellowEntities.Dispose();
            purpleResources.Dispose();
        }
        
        m_EntityCommandBufferSystem.AddJobHandleForProducer(dependencies);

        
        return dependencies;
    }
}