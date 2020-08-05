using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;
using Random = Unity.Mathematics.Random;

[UpdateAfter(typeof(CarRenderingSystem))]
public class SegmentRenderingSystem : SystemBase
{
    protected override void OnUpdate()
    {
        //We can optimize by having very large duration so we don't need this for loop everyframe
        var duration = 0f;

        var segmentCollection = GetSingleton<SegmentCollection>();
        for(int i=0; i<segmentCollection.Value.Value.Segments.Length; i++)
        {
            var segment = segmentCollection.Value.Value.Segments[i];
            UnityEngine.Debug.DrawLine(segment.Start,segment.End,UnityEngine.Color.yellow, duration);
        }
    }
}