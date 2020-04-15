using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

public struct BrigadeInitInfo : IComponentData
{
    public int WorkerCount;
}

public struct BrigadeLine : IComponentData
{
}

public struct WorkerEntityElementData : IBufferElementData
{
    public Entity Value;
}

[UpdateInGroup(typeof(InitializationSystemGroup))]
public class CreateBrigadeSystem : SystemBase
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
                    ecb.SetComponent(entityInQueryIndex, worker, new Translation() { Value = random.NextFloat3(new float3(0,0,0), new float3(100,0,100)) });
                    ecb.AddComponent(entityInQueryIndex, worker, new Worker());
                    workerBuffer.Add(new WorkerEntityElementData() { Value = worker });
                }
            }).ScheduleParallel();
        m_ECBSystem.AddJobHandleForProducer(Dependency);
    }
}

public class BrigadeFindSourceSystem : SystemBase
{
    private EntityCommandBufferSystem m_ECBSystem;

    protected override void OnCreate()
    {
        m_ECBSystem = World.GetExistingSystem<EndSimulationEntityCommandBufferSystem>();
    }

    Random random = new Random(234523456);
    protected override void OnUpdate()
    {
        var rand = random;
        var ecb = m_ECBSystem.CreateCommandBuffer().ToConcurrent();
        Entities
            .WithNone<ResourceSourcePosition>()
            .ForEach((int entityInQueryIndex, Entity e, in BrigadeLine line) =>
            {
                ecb.RemoveComponent<BrigadeLineEstablished>(entityInQueryIndex, e);
                ecb.AddComponent(entityInQueryIndex, e, new ResourceSourcePosition() { Value = rand.NextFloat2(new float2(0,0), new float2(100,100))});
            }).ScheduleParallel();
        random = rand;
        m_ECBSystem.AddJobHandleForProducer(Dependency);
    }
}

public class BrigadeFindTargetSystem : SystemBase
{
    private EntityCommandBufferSystem m_ECBSystem;

    protected override void OnCreate()
    {
        m_ECBSystem = World.GetExistingSystem<EndSimulationEntityCommandBufferSystem>();
    }

    Random random = new Random(775453);
    protected override void OnUpdate()
    {
        var rand = random;
        var ecb = m_ECBSystem.CreateCommandBuffer().ToConcurrent();
        Entities
            .WithNone<ResourceTargetPosition>()
            .ForEach((int entityInQueryIndex, Entity e, in BrigadeLine line) =>
            {
                ecb.RemoveComponent<BrigadeLineEstablished>(entityInQueryIndex, e);
                ecb.AddComponent(entityInQueryIndex, e, new ResourceTargetPosition() { Value = rand.NextFloat2(new float2(0,0), new float2(100, 100)) });
            }).ScheduleParallel();
        random = rand;
        m_ECBSystem.AddJobHandleForProducer(Dependency);
    }
}

public struct BrigadeLineEstablished : IComponentData
{
}

public class BrigadeGenerateWorkerPositionsSystem : SystemBase
{
    private EntityCommandBufferSystem m_ECBSystem;

    protected override void OnCreate()
    {
        m_ECBSystem = World.GetExistingSystem<EndSimulationEntityCommandBufferSystem>();
    }

    Random rand = new Random(455676);
    protected override void OnUpdate()
    {
        var ecb = m_ECBSystem.CreateCommandBuffer().ToConcurrent();
        var r = rand;
        var time = Time.ElapsedTime;
        Entities
            .WithNone<BrigadeLineEstablished>()
            .ForEach((int entityInQueryIndex, Entity e, in BrigadeLine line, in ResourceSourcePosition source, in ResourceTargetPosition target, in DynamicBuffer<WorkerEntityElementData> workers) =>
            {
                var start = source.Value;
                var end = target.Value;
                for (int i = 0; i < workers.Length; i++)
                {
                    ecb.AddComponent(entityInQueryIndex, workers[i].Value, new WorkerMoveTo() { Value = math.lerp(start, end, (float)i / workers.Length) });
                }
                ecb.AddComponent(entityInQueryIndex, e, new BrigadeLineEstablished());
                ecb.AddComponent(entityInQueryIndex, e, new Reset() { ResetTime = time + r.NextDouble(3, 10) });
                }).ScheduleParallel();
        m_ECBSystem.AddJobHandleForProducer(Dependency);
        rand = r;
    }
}

public struct Reset : IComponentData
{
    public double ResetTime;
}

public class ResetSystem : SystemBase
{
    private EntityCommandBufferSystem m_ECBSystem;

    protected override void OnCreate()
    {
        m_ECBSystem = World.GetExistingSystem<EndSimulationEntityCommandBufferSystem>();
    }

    protected override void OnUpdate()
    {
        var time = Time.ElapsedTime;
        var ecb = m_ECBSystem.CreateCommandBuffer().ToConcurrent();
        Entities.ForEach((int entityInQueryIndex, Entity e, Reset r) =>
        {
            if (time > r.ResetTime)
            {
                ecb.RemoveComponent<ResourceSourcePosition>(entityInQueryIndex, e);
                ecb.RemoveComponent<ResourceTargetPosition>(entityInQueryIndex, e);
            }
        }).ScheduleParallel();
        m_ECBSystem.AddJobHandleForProducer(Dependency);
    }
}
