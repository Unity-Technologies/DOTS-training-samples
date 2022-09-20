using Unity.Burst;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;

[BurstCompile]
struct SpawnJob : IJobParallelFor
{
    // A regular EntityCommandBuffer cannot be used in parallel, a ParallelWriter has to be explicitly used.
    public EntityCommandBuffer.ParallelWriter ECB;
    
    public Entity Prefab;

    public LocalToWorldTransform InitTM;

    public void Execute(int index)
    {
        var entity = ECB.Instantiate(index, Prefab);
        var random = Random.CreateFromIndex((uint) index);

        float radius = 10.0f;
        var pos = random.NextFloat3();
        pos *= radius;

        var tm = InitTM;
        tm.Value.Position = pos;
        ECB.SetComponent(index, entity, tm);
    }
}