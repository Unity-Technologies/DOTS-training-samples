using Unity.Collections;
using Unity.Entities;

public class SimpleIntersectionSplineSetupSystem : SystemBase
{
    protected override void OnUpdate()
    {
        var ecb = new EntityCommandBuffer(Allocator.TempJob);
        
        Entities
            .WithNone<Spline>()
            .ForEach((Entity entity, ref SimpleIntersection simpleIntersection) =>
        {
            var splineIn = GetComponent<Spline>(simpleIntersection.laneIn0);
            var inPos = splineIn.endPos;
            var splineOut = GetComponent<Spline>(simpleIntersection.laneOut0);
            var outPos = splineOut.startPos;

            var inToOut = outPos - inPos;
            
            var spline = new Spline
            {
                startPos = inPos,
                anchor1 = inPos + inToOut * 0.33f,
                anchor2 = inPos + inToOut * 0.66f,
                endPos = outPos
            };

            ecb.AddComponent(entity, spline);
            
        }).Run();
        
        ecb.Playback(EntityManager);
        ecb.Dispose();
    }
}
