using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

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
                var bucket = ecb.CreateEntity(entityInQueryIndex);
                var start = source.Value;
                var end = target.Value;
                float2 prevPos = default;
                for (int i = workers.Length - 1; i >= 0; i--)
                {
                    var pos = math.lerp(start, end, (float)i / workers.Length);
                    ecb.AddComponent(entityInQueryIndex, workers[i].Value, new WorkerStartEndPositions() { Start = pos, End = prevPos });
                    if (i == 0)
                    {
                        ecb.AddComponent(entityInQueryIndex, workers[i].Value, new BucketRef() { Bucket = bucket });
                        ecb.AddComponent(entityInQueryIndex, workers[i].Value, new WorkerMoveTo() { Value = prevPos });
                    }
                    else
                    {
                        ecb.AddComponent(entityInQueryIndex, workers[i].Value, new WorkerMoveTo() { Value = pos });
                    }
                    prevPos = pos;
                }
                ecb.AddComponent(entityInQueryIndex, e, new BrigadeLineEstablished());
          //      ecb.AddComponent(entityInQueryIndex, e, new Reset() { ResetTime = time + r.NextDouble(3, 10) });
            }).ScheduleParallel();
        m_ECBSystem.AddJobHandleForProducer(Dependency);
        rand = r;
    }
}