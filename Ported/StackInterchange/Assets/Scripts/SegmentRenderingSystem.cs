using Unity.Entities;

[UpdateAfter(typeof(CarRenderingSystem))]
public class SegmentRenderingSystem : SystemBase
{
    protected override void OnUpdate()
    {
        //having very large duration as we don't need this for loop everyframe
        var duration = 9999f;

        var segmentCollection = GetSingleton<SegmentCollection>();
        for(int i=0; i<segmentCollection.Value.Value.Segments.Length; i++)
        {
            var segment = segmentCollection.Value.Value.Segments[i];
            UnityEngine.Debug.DrawLine(segment.Start,segment.End,UnityEngine.Color.yellow, duration);
        }

        //Disable the system as the debug lines will stay anyway
        this.Enabled = false;
    }
}