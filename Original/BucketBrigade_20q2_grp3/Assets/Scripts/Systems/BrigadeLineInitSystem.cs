using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;


[UpdateInGroup(typeof(InitializationSystemGroup))]
public class BrigadeLineInitSystem : SystemBase
{
    private EntityCommandBufferSystem m_ECBSystem;
    private bool m_Initialized;

    protected override void OnCreate()
    {
        m_ECBSystem = World.GetExistingSystem<EndSimulationEntityCommandBufferSystem>();
    }

    protected override void OnUpdate()
    {
        if (!m_Initialized)
        {
            var prefabs = GetSingleton<GlobalPrefabs>();
            var random = new Random(745453);
            var ecb = m_ECBSystem.CreateCommandBuffer().ToConcurrent();
            Entities
                .WithNone<BrigadeLine>()
                .ForEach((int entityInQueryIndex, Entity e, in BrigadeInitInfo info) =>
                {
                    var center = random.NextFloat2(new float2(100, 100));
                    ecb.AddComponent(entityInQueryIndex, e, new BrigadeLine() {Center = center});
                    var workerBuffer = ecb.AddBuffer<WorkerEntityElementData>(entityInQueryIndex, e);

                    for (int i = 0; i < info.WorkerCount; i++)
                    {
                        var worker = ecb.Instantiate(entityInQueryIndex, prefabs.WorkerPrefab);
                        var wp = center + random.NextFloat2Direction() * 25;
                        ecb.SetComponent(entityInQueryIndex, worker,
                            new Translation() {Value = new float3(wp.x, 2.5f, wp.y)});
                        ecb.AddComponent(entityInQueryIndex, worker, new Worker() {NextWorkerInLine = Entity.Null});
                        workerBuffer.Add(new WorkerEntityElementData() {Value = worker});
                    }

                    for (int i = 0; i < info.WorkerCount - 1; i++)
                        ecb.SetComponent(entityInQueryIndex, workerBuffer[i].Value,
                            new Worker() {NextWorkerInLine = workerBuffer[i + 1].Value});
                }).ScheduleParallel();
            m_ECBSystem.AddJobHandleForProducer(Dependency);
            m_Initialized = true;
        }
    }
}
