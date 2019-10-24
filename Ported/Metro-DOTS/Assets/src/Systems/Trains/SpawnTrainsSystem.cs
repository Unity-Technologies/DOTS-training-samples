using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Transforms;

public struct SpawnTrain : IComponentData
{
    public Entity prefab;
    public TrainLine line;
    public uint numberOfCarriagePerTrain;
    public uint index;
}

public class SpawnTrainsSystem : JobComponentSystem
{
    EntityCommandBufferSystem m_Barrier;

    protected override void OnCreate()
    {
        m_Barrier = World.GetOrCreateSystem<BeginInitializationEntityCommandBufferSystem>();
    }

    protected override JobHandle OnUpdate(JobHandle inputDeps)
    {
        var job = new SpawnTrainsJob{ cmdBuffer = m_Barrier.CreateCommandBuffer().ToConcurrent()}.Schedule(this, inputDeps);
        m_Barrier.AddJobHandleForProducer(job);
        return job;
    }

    struct SpawnTrainsJob :  IJobForEachWithEntity<SpawnTrain>
    {
        [WriteOnly] public EntityCommandBuffer.Concurrent cmdBuffer;

        public void Execute(Entity entity, int index, ref SpawnTrain c0)
        {
            for (var i = 0; i < c0.numberOfCarriagePerTrain; i++)
            {
                var carriage = cmdBuffer.Instantiate(index, c0.prefab);
                var pos = c0.line.line.Value.points[i].location;
                cmdBuffer.SetComponent(index, carriage, new Translation { Value = pos });
                cmdBuffer.AddComponent(index, carriage, new TrainId{ value = c0.index });
                cmdBuffer.AddComponent(index, carriage, new TrainLine{ line = c0.line.line });
                cmdBuffer.AddComponent(index, carriage, new BezierTOffset{ offset = 0});
            }

            cmdBuffer.RemoveComponent(index, entity, typeof(SpawnTrain));
        }
    }
}