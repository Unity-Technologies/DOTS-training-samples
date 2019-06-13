using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;
using static Unity.Mathematics.math;

public class Sys_Bee_Init : JobComponentSystem
{
    // This declares a new kind of job, which is a unit of work to do.
    // The job is declared as an IJobForEach<Translation, Rotation>,
    // meaning it will process all entities in the world that have both
    // Translation and Rotation components. Change it to process the component
    // types you want.
    //
    // The job is also tagged with the BurstCompile attribute, which means
    // that the Burst compiler will optimize it for the best performance.
    [RequireComponentTag(typeof(Tag_Bee_Init))]
    struct Sys_Bee_InitJob : IJobForEachWithEntity<C_Size, C_Random>
    {
        public EntityCommandBuffer.Concurrent EntityCommandBuffer;
        [ReadOnly] public float MinBeeSize;
        [ReadOnly] public float MaxBeeSize;

        public void Execute(Entity entity, int index, ref C_Size size, ref C_Random Rand)
        {
            size.Value = Rand.Generator.NextFloat(MinBeeSize, MaxBeeSize);

            EntityCommandBuffer.RemoveComponent<Tag_Bee_Init>(index, entity);
        }
    }

    BeginInitializationEntityCommandBufferSystem m_EntityCommandBufferSystem;

    protected override void OnCreate()
    {
        // Cache the BeginInitializationEntityCommandBufferSystem in a field, so we don't have to create it every frame
        m_EntityCommandBufferSystem = World.GetOrCreateSystem<BeginInitializationEntityCommandBufferSystem>();
    }

    protected override JobHandle OnUpdate(JobHandle inputDependencies)
    {
        Random rand = new Random();
        rand.InitState(BeeManager.S.Rand.NextUInt());

        var job = new Sys_Bee_InitJob()
        {
            MaxBeeSize = BeeManager.S.MaxBeeSize,
            MinBeeSize = BeeManager.S.MinBeeSize,
            EntityCommandBuffer = m_EntityCommandBufferSystem.CreateCommandBuffer().ToConcurrent()
        }.Schedule(this, inputDependencies);

        m_EntityCommandBufferSystem.AddJobHandleForProducer(job);


        // Now that the job is set up, schedule it to be run. 
        return job;
    }
}