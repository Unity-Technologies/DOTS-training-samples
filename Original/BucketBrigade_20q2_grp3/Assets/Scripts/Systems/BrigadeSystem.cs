using Unity.Entities;
using Unity.Mathematics;



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
    protected override void OnUpdate()
    {
        var prefabs = GetSingleton<GlobalPrefabs>();
        var ecb = new EntityCommandBuffer(Unity.Collections.Allocator.Temp);
        Entities
            .WithNone<BrigadeLine>()
            .ForEach((Entity e, in BrigadeInitInfo info) =>
            {
                ecb.AddComponent<BrigadeLine>(e);
                var workerBuffer = ecb.AddBuffer<WorkerEntityElementData>(e);
                for (int i = 0; i < info.WorkerCount; i++)
                {
                    var worker = ecb.Instantiate(prefabs.WorkerPrefab);
                    ecb.AddComponent(worker, new Worker());
                    workerBuffer.Add(new WorkerEntityElementData() { Value = worker });
                }
            }).Run();
        ecb.Playback(EntityManager);
    }
}

public class BrigadeFindSourceSystem : SystemBase
{
    protected override void OnUpdate()
    {
        var random = new Random(234523456);
        var ecb = new EntityCommandBuffer(Unity.Collections.Allocator.Temp);
        Entities
            .WithNone<ResourceSourcePosition>()
            .ForEach((Entity e, in BrigadeLine line) =>
            {
                ecb.RemoveComponent<BrigadeLineEstablished>(e);
                ecb.AddComponent(e, new ResourceSourcePosition() { Value = random.NextFloat2(new float2(-100,-100), new float2(100,100))});
            }).Run();
        ecb.Playback(EntityManager);
    }
}

public class BrigadeFindTargetSystem : SystemBase
{
    protected override void OnUpdate()
    {
        var random = new Random(775453);
        var ecb = new EntityCommandBuffer(Unity.Collections.Allocator.Temp);
        Entities
            .WithNone<ResourceTargetPosition>()
            .ForEach((Entity e, in BrigadeLine line) =>
            {
                ecb.RemoveComponent<BrigadeLineEstablished>(e);
                ecb.AddComponent(e, new ResourceTargetPosition() { Value = random.NextFloat2(new float2(-10, -10), new float2(10, 10)) });
            }).Run();
        ecb.Playback(EntityManager);
    }
}

public struct BrigadeLineEstablished : IComponentData
{
}

public class BrigadeGenerateWorkerPositionsSystem : SystemBase
{
    protected override void OnUpdate()
    {
        var ecb = new EntityCommandBuffer(Unity.Collections.Allocator.Temp);
        Entities
            .WithoutBurst()
            .WithNone<BrigadeLineEstablished>()
            .ForEach((Entity e, in BrigadeLine line, in ResourceSourcePosition source, in ResourceTargetPosition target, in DynamicBuffer<WorkerEntityElementData> workers) =>
            {
                var start = source.Value;
                var end = target.Value;
                for (int i = 0; i < workers.Length; i++)
                {
                    ecb.AddComponent(workers[i].Value, new WorkerMoveTo() { Value = math.lerp(start, end, (float)i / workers.Length) });
                }
                ecb.AddComponent(e, new BrigadeLineEstablished());
            }).Run();
        ecb.Playback(EntityManager);
    }
}