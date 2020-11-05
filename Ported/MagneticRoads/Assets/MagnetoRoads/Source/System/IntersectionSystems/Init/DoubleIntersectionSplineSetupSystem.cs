using Unity.Collections;
using Unity.Entities;

public class DoubleIntersectionSplineSetupSystem : SystemBase
{
    protected override void OnUpdate()
    {
        var ecb = new EntityCommandBuffer(Allocator.TempJob);
        
        Entities
            .WithNone<Spline>()
            .ForEach((Entity entity, ref DoubleIntersection doubleIntersection) =>
            {
                var splineIn0 = GetComponent<Spline>(doubleIntersection.laneIn0);
                var inPos0 = splineIn0.endPos;
                var splineOut0 = GetComponent<Spline>(doubleIntersection.laneOut1);
                var outPos0 = splineOut0.startPos;
                
                var splineIn1 = GetComponent<Spline>(doubleIntersection.laneIn1);
                var inPos1 = splineIn1.endPos;
                var splineOut1 = GetComponent<Spline>(doubleIntersection.laneOut0);
                var outPos1 = splineOut1.startPos;

                var inToOut0 = outPos0 - inPos0;
                var inToOut1 = outPos1 - inPos1;
            
                var spline0 = new Spline
                {
                    startPos = inPos0,
                    anchor1 = inPos0 + inToOut0 * 0.33f,
                    anchor2 = inPos0 + inToOut0 * 0.66f,
                    endPos = outPos0
                };
                
                var spline1 = new Spline
                {
                    startPos = inPos1,
                    anchor1 = inPos1 + inToOut1 * 0.33f,
                    anchor2 = inPos1 + inToOut1 * 0.66f,
                    endPos = outPos1
                };

                ecb.AddComponent(entity, spline0);
                ecb.AddComponent(entity, spline1);
            
            }).Run();
        
        ecb.Playback(EntityManager);
        ecb.Dispose();
    }
}
