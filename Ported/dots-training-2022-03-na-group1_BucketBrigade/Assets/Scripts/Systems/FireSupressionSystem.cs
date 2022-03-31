using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;
using UnityInput = UnityEngine.Input;
using Unity.Collections;

public partial class FireSuppressionSystem : SystemBase
{
    int fireSuppresionRadius = 1;

    NativeArray<int2> checkAdjacents;

    protected override void OnCreate()
    {
        GridUtility.CreateAdjacentTileArray(ref checkAdjacents,fireSuppresionRadius);
    }
    protected override void OnDestroy()
    {
        checkAdjacents.Dispose();
    }
    
    public static void AddSplashByWorldPosition(
        ref DynamicBuffer<HeatMapSplash> splashmapBuffer, 
        int gridSideWidth, 
        float3 worldPosition)
    {
        int2 tileCoord = GridUtility.PlotTileCoordFromWorldPosition( worldPosition, gridSideWidth);
        AddSplashByTileCoordinate(ref splashmapBuffer, gridSideWidth, tileCoord.x, tileCoord.y);
    }
    public static void AddSplashByTileCoordinate(
        ref DynamicBuffer<HeatMapSplash> splashmapBuffer, 
        int gridSideWidth, 
        int tileCoordinateX, int tileCoordinateY)
    {
        int heatmapBufferLength = gridSideWidth * gridSideWidth;
        
        int tileIndex = GridUtility.GetTileIndex(tileCoordinateX, tileCoordinateY, gridSideWidth);
        
        AddSplashByIndex(ref splashmapBuffer, heatmapBufferLength, tileIndex);
    }
    public static void AddSplashByIndex(
        ref DynamicBuffer<HeatMapSplash> splashmapBuffer, 
        int heatmapBufferLength, 
        int tileIndex)
    {
        bool validIndex = tileIndex >= 0 && tileIndex < heatmapBufferLength;
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
    
    static void SuppressAdjacents(
        ref DynamicBuffer<HeatMapTemperature> heatmapBuffer, 
        ref DynamicBuffer<HeatMapSplash> splashmapBuffer, 
        NativeArray<int2> adjacentOffsets, 
        int splashIndex,
        int width)
    {
        //reset splashmap
        int tileIndex = splashmapBuffer[splashIndex];
        splashmapBuffer[splashIndex] = -1;
        
        //apply suppression to targetTile and adjacents
        heatmapBuffer[tileIndex] = 0f;
        
        for (int iCheck = 0; iCheck < adjacentOffsets.Length; iCheck++)
        {
            int2 tileCoord = GridUtility.GetTileCoordinate(tileIndex, width);
            int x = tileCoord.x + adjacentOffsets[iCheck].x;
            int z = tileCoord.y + adjacentOffsets[iCheck].y;

            bool inBounds = (x >= 0 && x <= width-1 && z >= 0 && z <= width-1);

            if (inBounds)
            {
                int adjacentIndex = GridUtility.GetTileIndex(x, z, width);
                heatmapBuffer[adjacentIndex] = 0f;
            }
        }
    }
    
    protected override void OnUpdate()
    {
        int width = GetSingleton<HeatMapData>().mapSideLength;
        
         var localCheckAdjacents = checkAdjacents;
         var splashmapBuffer = GetSplashmapBuffer();
         var heatmapBuffer = GetHeatmapBuffer();
         
         //HANDLE MOUSE CLICK
         if (UnityInput.GetMouseButtonDown(0))
         {
             var camera = this.GetSingleton<GameObjectRefs>().Camera;
             Ray ray = camera.ScreenPointToRay (UnityInput.mousePosition);
            
             RaycastHit hit;
             if (Physics.Raycast (ray, out hit, math.INFINITY)) 
             {
                 //Debug.DrawLine (ray.origin, hit.point);
                 AddSplashByWorldPosition(ref splashmapBuffer, width, hit.point);
             } 
         }
        
         Job.WithCode(() => {
        
             for (var i = 0; i < splashmapBuffer.Length; i++)
             {
                 if (splashmapBuffer[i] < 0)//-1
                     continue;
                 
                 SuppressAdjacents(ref heatmapBuffer, ref splashmapBuffer, localCheckAdjacents,i,width);
             }
        
         }).Run();

         
         Entities
             .WithAll<SplashEvent>()
             .ForEach((ref SplashEvent splashEvent) =>
             {
                 //AddSplashByWorldPosition(splashEvent.splashWorldPositionn); 
                 AddSplashByIndex(ref splashmapBuffer, splashEvent.fireTileIndex, heatmapBuffer.Length);
             })
             .Run();
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
