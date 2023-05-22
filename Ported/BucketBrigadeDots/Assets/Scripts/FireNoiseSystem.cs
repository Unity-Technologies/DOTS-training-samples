using Unity.Burst;
using Unity.Entities;
using Unity.Transforms;
using Unity.Mathematics;

public partial struct FireNoiseSystem : ISystem
{
    private uint index;
    
    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<FireCell>();
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        var random = Random.CreateFromIndex(index++);
        
        foreach (var localTransform in SystemAPI.Query<RefRW<LocalTransform>>().WithAll<FireCell>())
        {
            var position = localTransform.ValueRW.Position;
            position.y += random.NextFloat(-.1f, .1f);
            localTransform.ValueRW.Position = position;
        }
    }
}
