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
    protected override void OnUpdate()
    {
        var prefabs = GetSingleton<GlobalPrefabs>();
        var random = new Random(745453);
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
                    ecb.SetComponent(worker, new Translation() { Value = random.NextFloat3(new float3(0,0,0), new float3(100,0,100)) });
                    ecb.AddComponent(worker, new Worker());
                    workerBuffer.Add(new WorkerEntityElementData() { Value = worker });
                }
            }).Run();
        ecb.Playback(EntityManager);
    }
}

public class BrigadeFindSourceSystem : SystemBase
{
    Random random = new Random(234523456);
    protected override void OnUpdate()
    {
        var rand = random;
        var ecb = new EntityCommandBuffer(Unity.Collections.Allocator.Temp);
        Entities
            .WithNone<ResourceSourcePosition>()
            .ForEach((Entity e, in BrigadeLine line) =>
            {
                ecb.RemoveComponent<BrigadeLineEstablished>(e);
                ecb.AddComponent(e, new ResourceSourcePosition() { Value = rand.NextFloat2(new float2(0,0), new float2(100,100))});
            }).Run();
        ecb.Playback(EntityManager);
        random = rand;
    }
}

public class BrigadeFindTargetSystem : SystemBase
{
    Random random = new Random(775453);
    protected override void OnUpdate()
    {
        var rand = random;
        var ecb = new EntityCommandBuffer(Unity.Collections.Allocator.Temp);
        Entities
            .WithNone<ResourceTargetPosition>()
            .ForEach((Entity e, in BrigadeLine line) =>
            {
                ecb.RemoveComponent<BrigadeLineEstablished>(e);
                ecb.AddComponent(e, new ResourceTargetPosition() { Value = rand.NextFloat2(new float2(0,0), new float2(100, 100)) });
            }).Run();
        ecb.Playback(EntityManager);
        random = rand;
    }
}

public struct BrigadeLineEstablished : IComponentData
{
}

public class BrigadeGenerateWorkerPositionsSystem : SystemBase
{
    Random rand = new Random(455676);
    protected override void OnUpdate()
    {
        var ecb = new EntityCommandBuffer(Unity.Collections.Allocator.Temp);
        var r = rand;
        var time = Time.ElapsedTime + r.NextDouble(3, 10);
        Entities
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
                ecb.AddComponent(e, new Reset() { ResetTime = time });
                }).Run();
        ecb.Playback(EntityManager);
        rand = r;
    }
}

public struct Reset : IComponentData
{
    public double ResetTime;
}

public class ResetSystem : SystemBase
{
    protected override void OnUpdate()
    {
        var time = Time.ElapsedTime;
        var ecb = new EntityCommandBuffer(Unity.Collections.Allocator.Temp);
        Entities.ForEach((Entity e, Reset r) =>
        {
            if (time > r.ResetTime)
            {
                ecb.RemoveComponent<ResourceSourcePosition>(e);
                ecb.RemoveComponent<ResourceTargetPosition>(e);
            }
        }).Run();
        ecb.Playback(EntityManager);
    }
}
