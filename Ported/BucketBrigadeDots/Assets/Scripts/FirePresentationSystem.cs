using Unity.Burst;
using Unity.Entities;
using Unity.Transforms;
using Unity.Mathematics;

[UpdateInGroup(typeof(PresentationSystemGroup))]
public partial struct FirePresentationSystem : ISystem
{
    private uint randomCounter;
    
    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<FireCell>();
        state.RequireForUpdate<GameSettings>();
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        var gameSettings = SystemAPI.GetSingleton<GameSettings>();
        var temperatures = SystemAPI.GetSingletonBuffer<FireTemperature>();
        
        var random = Random.CreateFromIndex(randomCounter++);

        var index = 0;
        foreach (var localTransform in SystemAPI.Query<RefRW<LocalTransform>>().WithAll<FireCell>())
        {
            var heat = temperatures[index];
            var x = index % gameSettings.RowsAndColumns;
            var z = index / gameSettings.RowsAndColumns;
            var noise = random.NextFloat(0, .01f);
            localTransform.ValueRW.Position = new float3(x * .3f, heat -1f, z * .3f);

            index++;
        }
    }
}
