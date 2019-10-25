using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;

public struct SpawnTrain : IComponentData
{
    public Entity prefab;
    public BezierCurve line;
    public int numberOfCarriagePerTrain;
    public int trainId;
    public float carriageOffset;
    public float initialT;
    public int initialPlatformId;

    public SpawnTrain(Entity m_prefab, BezierCurve m_Line, int m_numberOfCarriagePerTrain, int m_index, float m_carriageOffset, float m_initialT, int currentPlatform)
    {
        prefab = m_prefab;
        line = m_Line;
        numberOfCarriagePerTrain = m_numberOfCarriagePerTrain;
        trainId = m_index;
        carriageOffset = m_carriageOffset;
        initialT = m_initialT;
        initialPlatformId = currentPlatform;
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

    struct SpawnTrainsJob :  IJobForEachWithEntity_EBC<StationData, SpawnTrain>
    {
        [WriteOnly] public EntityCommandBuffer.Concurrent cmdBuffer;

        public void Execute(Entity entity, int index, [ReadOnly] DynamicBuffer<StationData> stations, ref SpawnTrain c0)
        {
            var carriageOffset = 0f;

            for (var i = 0; i < c0.numberOfCarriagePerTrain; i++)
            {
                var carriage = cmdBuffer.Instantiate(index, c0.prefab);
                var t = c0.initialT + carriageOffset;
                var pos = BezierUtils.GetPositionAtT(c0.line.line, t);
                var rot = BezierUtils.GetNormalAtT(c0.line.line, t);
                var lookAt = quaternion.LookRotation( rot, new float3(0, 1, 0));

                cmdBuffer.SetComponent(index, carriage, new Translation { Value = pos });
                cmdBuffer.SetComponent(index, carriage, new Rotation { Value = lookAt });
                cmdBuffer.AddComponent(index, carriage, new Speed{ value = 0.02f});
                cmdBuffer.AddComponent(index, carriage, new TrainId{ value = c0.trainId });
                cmdBuffer.AddComponent(index, carriage, new CarriageId{ value = i });
                cmdBuffer.AddComponent(index, carriage, new BezierCurve{ line = c0.line.line });
                cmdBuffer.AddComponent(index, carriage, new PlatformId{ value = c0.initialPlatformId});
                cmdBuffer.AddComponent(index, carriage, new BezierTOffset{ offset = c0.initialT, renderOffset = carriageOffset});
                cmdBuffer.AddComponent(index, carriage, new EN_ROUTE());
                cmdBuffer.AddBuffer<StationData>(index, carriage).CopyFrom(stations);
                carriageOffset += c0.carriageOffset;
            }
            cmdBuffer.RemoveComponent(index, entity, typeof(SpawnTrain));
            cmdBuffer.RemoveComponent(index, entity, typeof(StationData));
        }
    }
}