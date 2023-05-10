using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

public partial struct WaterSpreadSystem : ISystem
{
    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        // state.RequireForUpdate<Water>();
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        var elapsedTime = math.sin((float)SystemAPI.Time.ElapsedTime);
        foreach (var water in SystemAPI.Query<RefRW<Water>>().WithAll<Water>())
        {
            water.ValueRW.Volume = math.clamp(water.ValueRO.Volume + water.ValueRO.RefillSpeed * SystemAPI.Time.DeltaTime,0,1);
        }

        foreach (var (water, ptMatrix) in SystemAPI.Query<RefRO<Water>, RefRW<PostTransformMatrix>>().WithAll<Water>())
        {
            ptMatrix.ValueRW.Value = float4x4.Scale(new float3(
                water.ValueRO.TargetScale.x * water.ValueRO.Volume,
                0.1f,
                water.ValueRO.TargetScale.y * water.ValueRO.Volume));
        }
    }
}

// [WithAll(typeof(Water))]
// [BurstCompile]
// partial struct AnimVolumeJob : IJobEntity
// {
//     public float ElapsedTime;
//     
//     void Execute(ref LocalTransform transform)
//     {
//         Debug.Log(math.sin(ElapsedTime));
//         transform = transform.WithScale(math.sin(ElapsedTime));
//     }
// }