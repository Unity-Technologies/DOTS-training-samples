using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;
using static Unity.Mathematics.math;

[UpdateInGroup(typeof(LateSimulationSystemGroup)), UpdateAfter(typeof(Sys_TranslationAddVelocity)), UpdateAfter(typeof(Sys_TranslationClampBounds))]
public class Sys_ResolveStacks : JobComponentSystem
{  

    struct Sys_ResolveStacksJob : IJobForEachWithEntity<C_GridIndex, Translation>
    {
        [ReadOnly] public int2 GridCounts;
        [ReadOnly] public float2 MinGridPos;
        [ReadOnly] public float2 GridSize;
        [ReadOnly] public float3 Field;
        [ReadOnly] public float ResourceSize;
        [ReadOnly] public int BeesPerResource;
        [ReadOnly] public Entity YellowBeePrefab;
        [ReadOnly] public Entity PurpleBeePrefab;
        [ReadOnly] public Entity SpawnerPrefab;
        [ReadOnly] public Entity SmokePrefab;

        public NativeArray<int> StackHeights;
        public EntityCommandBuffer ecb;

        private int GridIndex(int x, int y)
        {
            return GridCounts.x * y + x;
        }

        private float3 GetStackPos(int x, int y, int height)
        {

            return float3(MinGridPos.x + x * GridSize.x, -Field.y * .5f + (height + .5f) * ResourceSize, MinGridPos.y + y * GridSize.y);
        }

        public void Execute(Entity ent, int index, [ReadOnly] ref C_GridIndex Grid, ref Translation Position)
        {
            float floorY = GetStackPos(Grid.x, Grid.y, StackHeights[GridIndex(Grid.x, Grid.y)]).y;

            if (Position.Value.y > floorY)
                return;
            
            Position.Value.y = floorY;
            if (abs(Position.Value.x) > Field.x * .4f)
            {
                C_Spawner BeeSpawnData = new C_Spawner()
                {
                    Count = BeesPerResource
                };
                BeeSpawnData.Prefab = PurpleBeePrefab;
                if (Position.Value.x > 0f)
                {
                    BeeSpawnData.Prefab = YellowBeePrefab;
                }

                var localToWorld = new LocalToWorld()
                {
                    Value = float4x4(quaternion(0, 0, 0, 1), Position.Value)
                };

                var spawner = ecb.Instantiate(SpawnerPrefab);
                ecb.SetComponent(spawner, BeeSpawnData);
                ecb.SetComponent(spawner, localToWorld);

                //Spawn Smoke

                C_Spawner SmokeSpawnData = new C_Spawner()
                {
                    Count = 5,
                    Prefab = SmokePrefab

                };

                spawner = ecb.Instantiate(SpawnerPrefab);
                ecb.SetComponent(spawner, SmokeSpawnData);
                ecb.SetComponent(spawner, localToWorld);

                ecb.DestroyEntity(ent);
            }
            else
            {
                int stackIndex = StackHeights[GridIndex(Grid.x, Grid.y)];
                if((stackIndex + 1) * ResourceSize < Field.y)
                {
                    C_Stack stack = new C_Stack()
                    {
                        index = stackIndex
                    };

                    ecb.AddComponent(ent, stack);

                    StackHeights[GridIndex(Grid.x, Grid.y)]++;
                }
                else
                {
                    ecb.DestroyEntity(ent);
                }
            }
        }
    }

    public EntityCommandBufferSystem m_entityCommandBufferSystem;
    public EntityQuery m_group;

    protected override void OnCreate()
    {
        m_entityCommandBufferSystem = World.GetOrCreateSystem<EntityCommandBufferSystem>();

        var queryDesc = new EntityQueryDesc()
        {
            None = new ComponentType[] { typeof(C_Stack), typeof(Tag_IsHeld) },
            All = new ComponentType[] { ComponentType.ReadOnly<C_GridIndex>(), typeof(Translation), ComponentType.ReadOnly<Tag_Resource>() }
        };

        m_group = GetEntityQuery(queryDesc);
    }

    public static JobHandle job;

    protected override JobHandle OnUpdate(JobHandle inputDependencies)
    {
        job = new Sys_ResolveStacksJob()
        {
            GridCounts = ResourceManager.S.GridCounts,
            MinGridPos = ResourceManager.S.MinGridPos,
            GridSize = ResourceManager.S.GridSize,
            Field = Field.size,
            ResourceSize = ResourceManager.S.ResourceSize,
            BeesPerResource = ResourceManager.S.BeesPerResource,
            YellowBeePrefab = BeeManager.S.YellowBeeEnt,
            PurpleBeePrefab = BeeManager.S.PurpleBeeEnt,
            SpawnerPrefab = BeeManager.S.SpawnerEnt,
            SmokePrefab = GameConstants.S.SmokeEnt,
            StackHeights = ResourceManager.S.StackHeights,
            ecb = m_entityCommandBufferSystem.CreateCommandBuffer()

        }.ScheduleSingle(m_group, inputDependencies);

        m_entityCommandBufferSystem.AddJobHandleForProducer(job);
        
        return job;
    }
}
