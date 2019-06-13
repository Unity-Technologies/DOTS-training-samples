using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;
using static Unity.Mathematics.math;

[UpdateInGroup(typeof(InitializationSystemGroup)), UpdateBefore(typeof(Sys_Bee_GetTarget))]
public class Sys_Held_Init : JobComponentSystem
{

    [RequireComponentTag(typeof(C_Stack), typeof(Tag_IsHeld))]
    struct Sys_Held_InitJob : IJobForEachWithEntity<C_Held, C_GridIndex>
    {
        public EntityCommandBuffer ecb;
        public NativeArray<int> StackHeights;

        [ReadOnly] public int2 GridCounts;
        private int GridIndex(int x, int y)
        {
            return GridCounts.x * y + x;
        }

        public void Execute(Entity ent, int index, [ReadOnly] ref C_Held held, [ReadOnly] ref C_GridIndex grid)
        {
            StackHeights[GridIndex(grid.x, grid.y)]--;

            ecb.RemoveComponent(ent, typeof(C_Stack));
        }
    }
    private EntityCommandBufferSystem m_entityCommandBufferSystem;

    protected override void OnCreate()
    {
        m_entityCommandBufferSystem = World.GetOrCreateSystem<EntityCommandBufferSystem>();
    }

    protected override JobHandle OnUpdate(JobHandle inputDependencies)
    {
        var job = new Sys_Held_InitJob()
        {
            ecb = m_entityCommandBufferSystem.CreateCommandBuffer(),

            GridCounts = ResourceManager.S.GridCounts,
            StackHeights = ResourceManager.S.StackHeights

        }.ScheduleSingle(this, inputDependencies);

        m_entityCommandBufferSystem.AddJobHandleForProducer(job);

        return job;
    }
}