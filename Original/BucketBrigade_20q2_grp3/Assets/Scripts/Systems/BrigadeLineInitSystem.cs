using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;


[UpdateInGroup(typeof(InitializationSystemGroup))]
public class BrigadeLineInitSystem : SystemBase
{
    private EntityCommandBufferSystem m_ECBSystem;

    protected override void OnCreate()
    {
        m_ECBSystem = World.GetExistingSystem<EndSimulationEntityCommandBufferSystem>();
    }

    protected override void OnUpdate()
    {
        var prefabs = GetSingleton<GlobalPrefabs>();
        var random = new Random(745453);
        var ecb = m_ECBSystem.CreateCommandBuffer().ToConcurrent();
        Entities
            .WithNone<BrigadeLine>()
            .ForEach((int entityInQueryIndex, Entity e, in BrigadeInitInfo info) =>
            {
                ecb.AddComponent<BrigadeLine>(entityInQueryIndex, e);
                var workerBuffer = ecb.AddBuffer<WorkerEntityElementData>(entityInQueryIndex, e);
                
                for (int i = 0; i < info.WorkerCount; i++)
                {
                    var worker = ecb.Instantiate(entityInQueryIndex, prefabs.WorkerPrefab);
                    ecb.SetComponent(entityInQueryIndex, worker, new Translation() { Value = random.NextFloat3(new float3(0, 0, 0), new float3(100, 0, 100)) });
                    ecb.AddComponent(entityInQueryIndex, worker, new Worker() { NextWorkerInLine = Entity.Null });
                    workerBuffer.Add(new WorkerEntityElementData() { Value = worker });
                }

                for (int i = 0; i < info.WorkerCount-1; i++)
                    ecb.SetComponent(entityInQueryIndex, workerBuffer[i].Value, new Worker() { NextWorkerInLine = workerBuffer[i + 1].Value });

            }).ScheduleParallel();
        m_ECBSystem.AddJobHandleForProducer(Dependency);
    }
}
