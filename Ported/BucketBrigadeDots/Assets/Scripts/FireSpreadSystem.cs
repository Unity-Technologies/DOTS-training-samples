using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;

public partial struct FireSpreadSystem : ISystem
{
    private float timeUntilFireUpdate;
    
    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<GameSettings>();
        state.RequireForUpdate<FireTemperature>();
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        var deltaTime = SystemAPI.Time.DeltaTime;
        timeUntilFireUpdate -= deltaTime;
        if (timeUntilFireUpdate > 0f) return;
        
        var settings = SystemAPI.GetSingleton<GameSettings>();
        timeUntilFireUpdate += settings.FireSimUpdateRate;
        var size = settings.Size;
        var cols = settings.RowsAndColumns;
        
        var temperatures = SystemAPI.GetSingletonBuffer<FireTemperature>();

        for (int i = 0; i < size; i++)
        {
            var heat = temperatures[i];
            if (heat < .2f) continue;

            var increase = heat * settings.HeatTransferRate;

            var currentX = i % cols;
            var currentY = i / cols;

            for (var offsetY = -settings.HeatRadius; offsetY <= settings.HeatRadius; offsetY++)
            {
                var neighborY = currentY + offsetY;
                if (neighborY < 0 || neighborY >= cols) continue;
                
                for (var offsetX = -settings.HeatRadius; offsetX <= settings.HeatRadius; offsetX++)
                {
                    var neighborX = currentX + offsetX;
                    if (neighborX < 0 || neighborX >= cols) continue;

                    var neighborIndex = neighborY * cols + neighborX;
                    temperatures[neighborIndex] = math.min(1f, temperatures[neighborIndex] + increase);
                }
            }
        }
    }
}
