using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;

public struct SpawnTrain : IComponentData
{
    public Entity prefab;
    public TrainLine line;
    public uint numberOfCarriagePerTrain;
    public uint index;
    public float carriageOffset;

    public SpawnTrain(Entity m_prefab, TrainLine m_Line, int m_numberOfCarriagePerTrain, int m_index, float m_carriageOffset)
    {
        prefab = m_prefab;
        line = m_Line;
        numberOfCarriagePerTrain = (uint)m_numberOfCarriagePerTrain;
        index = (uint)m_index;
        carriageOffset = m_carriageOffset;
    }
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
            var offset = 0f;
            for (var i = 0; i < c0.numberOfCarriagePerTrain; i++)
            {
                var carriage = cmdBuffer.Instantiate(index, c0.prefab);
                var pos = BezierUtils.GetPositionAtT(c0.line.line, offset);
                var rot = BezierUtils.GetNormalAtT(c0.line.line, offset);
                var lookAt = quaternion.LookRotation( rot, new float3(0, 1, 0));

                cmdBuffer.SetComponent(index, carriage, new Translation { Value = pos });
                cmdBuffer.SetComponent(index, carriage, new Rotation { Value = lookAt });
                cmdBuffer.AddComponent(index, carriage, new TrainId{ value = c0.index });
                cmdBuffer.AddComponent(index, carriage, new TrainLine{ line = c0.line.line });
                cmdBuffer.AddComponent(index, carriage, new BezierTOffset{ offset = offset});
                offset += c0.carriageOffset;
            }

            cmdBuffer.RemoveComponent(index, entity, typeof(SpawnTrain));
        }
    }
}