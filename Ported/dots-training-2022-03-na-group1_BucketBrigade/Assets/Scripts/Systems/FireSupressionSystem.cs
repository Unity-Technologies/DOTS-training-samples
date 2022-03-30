using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;
using UnityInput = UnityEngine.Input;
using Unity.Collections;

public partial class FireSuppressionSystem : SystemBase
{
    int fireSuppresionRadius = 1;

    NativeArray<int2> checkAdjacents;

    protected override void OnCreate()
    {
        GridUtil.CreateAdjacentTileArray(ref checkAdjacents,fireSuppresionRadius);
    }
    protected override void OnDestroy()
    {
        checkAdjacents.Dispose();
    }
    
    int2 PlotTileCoordFromWorldPosition(float3 worldPosition)
    {
        int width = GetSingleton<HeatMapData>().mapSideLength;
        float offset = (width - 1) * 0.5f;

        int x =(int) math.remap(-offset, offset, 0f, width - 1, worldPosition.x);
        int z =(int) math.remap(-offset, offset, 0f, width - 1, worldPosition.z);

        int2 coord = new int2(x, z);
        UnityEngine.Debug.Log($"world position: {worldPosition} | grid coordinate: {coord}");
        return coord;
    }
    
    public void AddSplashByTileCoordinate(int tileCoordinateX, int tileCoordinateY)
    {
        int width = GetSingleton<HeatMapData>().mapSideLength;

        int tileIndex = GridUtil.GetTileIndex(tileCoordinateX, tileCoordinateY, width);
        
        AddSplashByIndex(tileIndex);
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
            int2 tileCoord = GridUtil.GetTileCoordinate(tileIndex, width);
            int x = tileCoord.x + adjacentOffsets[iCheck].x;
            int z = tileCoord.y + adjacentOffsets[iCheck].y;

            bool inBounds = (x >= 0 && x <= width-1 && z >= 0 && z <= width-1);

            if (inBounds)
            {
                int adjacentIndex = GridUtil.GetTileIndex(x, z, width);
                heatmapBuffer[adjacentIndex] = 0f;
            }
        }
    }
    
    protected override void OnUpdate()
    {
        HandleMouseClick();
        
        int width = GetSingleton<HeatMapData>().mapSideLength;

        var localCheckAdjacents = checkAdjacents;
        var splashmapBuffer = GetSplashmapBuffer();
        var heatmapBuffer = GetHeatmapBuffer();
        
        Job.WithCode(() => {

            for (var i = 0; i < splashmapBuffer.Length; i++)
            {
                if (splashmapBuffer[i] < 0)//-1
                    continue;
                
                SuppressAdjacents(ref heatmapBuffer, ref splashmapBuffer, localCheckAdjacents,i,width);
            }

        }).Run();
    }

    void HandleMouseClick()
    {
        if (UnityInput.GetMouseButtonDown(0))
        {
            var camera = this.GetSingleton<GameObjectRefs>().Camera;
            Ray ray = camera.ScreenPointToRay (UnityInput.mousePosition);
            
            RaycastHit hit;
            if (Physics.Raycast (ray, out hit, math.INFINITY)) 
            {
                //draw invisible ray cast/vector
                Debug.DrawLine (ray.origin, hit.point);
                
                float3 worldPosition = hit.point;
                int2 tileCoord = PlotTileCoordFromWorldPosition(worldPosition);
                AddSplashByTileCoordinate(tileCoord.x, tileCoord.y);
            } 
        }
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
