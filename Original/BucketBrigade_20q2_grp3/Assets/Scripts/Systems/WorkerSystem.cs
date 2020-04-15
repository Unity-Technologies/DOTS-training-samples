using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

public struct Worker : IComponentData
{

}

public struct WorkerMoveTo : IComponentData
{
    public float2 Value;
}

public class WorkerMoveToSystem : SystemBase
{
    static float3 MoveTo(float3 pos, float3 dest, float amount)
    {
        float len = math.length(pos - dest);
        if (len < amount)
            return dest;

        float3 moveDir = math.normalize(dest - pos);
        float3 newPos = pos + moveDir * amount;
        return newPos;
    }


    protected override void OnUpdate()
    {
        float speed = 25.0f;
        float deltaTime = Time.DeltaTime;

        Entities
            .ForEach((Entity e, ref Translation pos, in WorkerMoveTo target) =>
        {
            float3 targetPos = new float3(target.Value.x, pos.Value.y, target.Value.y);
            pos.Value = MoveTo(pos.Value, targetPos, deltaTime * speed);
        }).Run();
    }
}