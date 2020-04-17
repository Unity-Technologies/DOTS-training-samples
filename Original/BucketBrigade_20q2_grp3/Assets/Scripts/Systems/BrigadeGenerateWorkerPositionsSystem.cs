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

    static float2 GetWaypoint(float2 src, float2 dest, int pointIndex, int pointCount)
    {
        return math.lerp(src, dest, ((float)pointIndex / (float)pointCount));
    }

    Random rand = new Random(455676);
    protected override void OnUpdate()
    {
        var prefabs = GetSingleton<GlobalPrefabs>();
        var ecb = m_ECBSystem.CreateCommandBuffer().ToConcurrent();
        var r = rand;
        var time = Time.ElapsedTime;
        Entities
            .WithNone<BrigadeLineEstablished>()
            .ForEach((int entityInQueryIndex, Entity e, ref BrigadeLine line, in ResourceSourcePosition source, in ResourceTargetPosition target, in DynamicBuffer<WorkerEntityElementData> workers) =>
            {
                var start = source.Value;
                var end = target.Value;
                for(int i = 0; i < workers.Length; i++)
                {
                    var positions = new WorkerStartEndPositions()
                    {
                        Start = GetWaypoint(start, end, i, workers.Length + 1),
                        End = GetWaypoint(start, end, i + 1, workers.Length + 1),
                    };
                    var initialDestination = new WorkerMoveTo() { Value = positions.Start };
                    ecb.AddComponent(entityInQueryIndex, workers[i].Value, positions);
                    if (i == 0)
                    {
                        var bucket = ecb.Instantiate(entityInQueryIndex, prefabs.BucketPrefab);
                        ecb.AddComponent(entityInQueryIndex, bucket, new BucketWorkerRef() { WorkerRef = workers[i].Value });
                        ecb.AddComponent(entityInQueryIndex, workers[i].Value, new BucketRef() { Bucket = bucket });
                        initialDestination.Value = positions.End;
                    }
                    ecb.AddComponent(entityInQueryIndex, workers[i].Value, initialDestination);
                }
                line.Center = (start + end) * .5f;
                ecb.AddComponent(entityInQueryIndex, e, new BrigadeLineEstablished());
                ecb.AddComponent(entityInQueryIndex, e, new Reset() { ResetTime = time + r.NextDouble(3, 6) });
            }).ScheduleParallel();
        m_ECBSystem.AddJobHandleForProducer(Dependency);
        rand = r;
    }
}