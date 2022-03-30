using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

public partial class FireSuppressionSystem : SystemBase
{
    public static void AddSplashByCoordinate(int2 tileCoordinate)
    {
        
    }
    public void AddSplashByIndex(int tileIndex)
    {
        var heatmapBuffer = GetHeatmapBuffer();
        var splashmapBuffer = GetSplashmapBuffer();
        
        bool validIndex = tileIndex >= 0 && tileIndex < heatmapBuffer.Length;
        if (validIndex)
        {
            //Add splash, Find first slot available (-1) and set it to the tileIndex
            for (var i = 0; i < splashmapBuffer.Length; i++)
            {
                if (splashmapBuffer[i] < 0)
                {
                    splashmapBuffer[i] = tileIndex;
                    return;
                }
            }
        }
    }
    
    protected override void OnUpdate()
    {
        //var time = Time.ElapsedTime;
    }
    
    DynamicBuffer<HeatMapSplash> GetSplashmapBuffer() 
    {
        var splashmap = GetSingletonEntity<HeatMapSplash>();
        return EntityManager.GetBuffer<HeatMapSplash>(splashmap);
    }
    
    DynamicBuffer<HeatMapTemperature> GetHeatmapBuffer() 
    {
        var heatmap = GetSingletonEntity<HeatMapTemperature>();
        return EntityManager.GetBuffer<HeatMapTemperature>(heatmap);
    }
}
