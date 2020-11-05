using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;

public class DoubleIntersectionSplineSetupSystem : SystemBase
{
    EntityQuery query;
    
    protected override void OnUpdate()
    {
        var ecb = new EntityCommandBuffer(Allocator.TempJob);

        var splineArchetype = EntityManager.CreateArchetype(typeof(Spline));

        // Must init entities ahead of time so when we assign to spline0/1, the entity has a valid non-deferred entity ID to assign to (when ECB creates and entity, ID is -1 until playback) 
        // * 2 because of the number of splines in a double intersection
        var splineEntities = EntityManager.CreateEntity(splineArchetype, query.CalculateEntityCount() * 2, Allocator.TempJob);
        
        int splineCount = 0;
        
        Entities
            .WithStoreEntityQueryInField(ref query)
            .WithDisposeOnCompletion(splineEntities)
            .WithAll<IntersectionNeedsInit>()
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

                var spline0Data = new Spline
                {
                    startPos = inPos0,
                    anchor1 = inPos0 + inToOut0 * 0.33f,
                    anchor2 = inPos0 + inToOut0 * 0.66f,
                    endPos = outPos0
                };
                
                var spline0Entity = splineEntities[splineCount++];
                ecb.SetComponent(spline0Entity, spline0Data);
                doubleIntersection.spline0 = spline0Entity;

                var spline1Data = new Spline
                {
                    startPos = inPos1,
                    anchor1 = inPos1 + inToOut1 * 0.33f,
                    anchor2 = inPos1 + inToOut1 * 0.66f,
                    endPos = outPos1
                };
                
                var spline1Entity = splineEntities[splineCount++];
                ecb.SetComponent(spline1Entity, spline1Data);
                doubleIntersection.spline1 = spline1Entity;
                
                ecb.RemoveComponent<IntersectionNeedsInit>(entity);

            }).Run();
        
        ecb.Playback(EntityManager);
        ecb.Dispose();
    }
}
