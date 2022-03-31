using Unity.Collections;
using Unity.Entities;
using Unity.Transforms;
using Unity.Mathematics;
using UnityEngine;

public partial class FireSpotLookupSystem : SystemBase
{

    protected override void OnUpdate()
    {
        //var heatmapData = BucketBrigadeUtility.GetHeatmapData(this);
        
        //Every 1s
        //ForEach Fetcher
        //-Fetcher.HomePosition 
        //--
        /*
         * float closestDistance = infinity;
         * GetHeatmapBuffer
         * For (i = 0; i < heatmap.Length; i++)
         * {
         *  float3 tileWorldPosition = GridUtility.PlotTileWorldPositionFromIndex(i, heatmapData.mapSideLength);
         *  float distance = math.distance(Fetcher.HomePosition , tileWorldPosition);
         *  if(distance < closestDistance)
         *    closestDistance = distance;
         * }
        */
        
    }
}
