using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;

public partial struct FireSpreadSystem : ISystem
{
    private const float heatScalePerSecond = .1f;
    
    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<GameSettings>();
        state.RequireForUpdate<FireTemperature>();
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        var gameSettings = SystemAPI.GetSingleton<GameSettings>();
        var temperatures = SystemAPI.GetSingletonBuffer<FireTemperature>();
        
        var size = temperatures.Length;
        var increase = heatScalePerSecond * SystemAPI.Time.DeltaTime;
        
        for (int i = 0; i < size; i++)
        {
            var heat = temperatures[i];
            if (heat <= 0f) continue;
            
            heat = heat + increase;
            temperatures[i] = math.min(1f, heat);
            
            if (heat < .2f) continue;

            var cols = gameSettings.RowsAndColumns;
            var x = i % cols;

            // left neighbor
            var neighborIndex = i - 1;
            if (x > 0 && neighborIndex > 0)
            {
                temperatures[neighborIndex] = math.min(1f, temperatures[neighborIndex] + increase);
            }
            
            // right neighbor
            neighborIndex = i + 1;
            if (x + 1 < cols && neighborIndex < size) 
            {
                temperatures[neighborIndex] = math.min(1f, temperatures[neighborIndex] + increase);
            }
            
            // above neighbor
            neighborIndex = i - cols;
            if (neighborIndex >= 0) 
            {
                temperatures[neighborIndex] = math.min(1f, temperatures[neighborIndex] + increase);
            }
            
            // below neighbor
            neighborIndex = i + cols;
            if (neighborIndex < size) 
            {
                temperatures[neighborIndex] = math.min(1f, temperatures[neighborIndex] + increase);
            }
        }
    }
}
