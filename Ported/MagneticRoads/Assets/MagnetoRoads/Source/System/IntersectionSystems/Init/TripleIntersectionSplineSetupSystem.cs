using Unity.Collections;
using Unity.Entities;

public class TripleIntersectionSplineSetupSystem : SystemBase
{
    // Turn splines are: aStraightToB, aTurnToC, bStraightToA, bTurnToC, cTurnToA, cTurnToB
    //
    //        | | c
    //        v ^
    // a  --<     <--   b
    //    -->     >--
    
    EntityQuery query;
    
    protected override void OnUpdate()
    {
        var ecb = new EntityCommandBuffer(Allocator.TempJob);

        var splineArchetype = EntityManager.CreateArchetype(typeof(Spline));

        // Must init entities ahead of time so when we assign to spline0/1, the entity has a valid non-deferred entity ID to assign to (when ECB creates and entity, ID is -1 until playback) 
        // * 6 because of the number of splines in a triple intersection
        var splineEntities = EntityManager.CreateEntity(splineArchetype, query.CalculateEntityCount() * 6, Allocator.TempJob);
        
        int splineCount = 0;
        
        Entities
            .WithStoreEntityQueryInField(ref query)
            .WithDisposeOnCompletion(splineEntities)
            .WithAll<IntersectionNeedsInit>()
            .ForEach((Entity entity, ref TripleIntersection tripleIntersection) =>
            {
                var splineIn0 = GetComponent<Spline>(tripleIntersection.laneIn0);
                var splineOut0 = GetComponent<Spline>(tripleIntersection.laneOut0);
                var splineIn1 = GetComponent<Spline>(tripleIntersection.laneIn1);
                var splineOut1 = GetComponent<Spline>(tripleIntersection.laneOut1);
                var splineIn2 = GetComponent<Spline>(tripleIntersection.laneIn2);
                var splineOut2 = GetComponent<Spline>(tripleIntersection.laneOut2);

                var aIn = splineIn0.endPos;
                var bIn = splineIn1.endPos;
                var cIn = splineIn2.endPos;
                var aOut = splineOut0.startPos;
                var bOut = splineOut1.startPos;
                var cOut = splineOut2.startPos;

                var aToB = bOut - aIn;
                var aToC = cOut - aIn;
                var bToA = aOut - bIn;
                var bToC = cOut - bIn;
                var cToA = aOut - cIn;
                var cToB = bOut - cIn;

                var aStraightToBSpline = new Spline
                {
                    startPos = aIn,
                    anchor1 = aIn + aToB * 0.33f,
                    anchor2 = aIn + aToB * 0.66f,
                    endPos = bOut
                };
                
                var spline0Entity = splineEntities[splineCount++];
                ecb.SetComponent(spline0Entity, aStraightToBSpline);
                tripleIntersection.aStraightToBSpline = spline0Entity;

                var aTurnToCSpline = new Spline
                {
                    startPos = aIn,
                    anchor1 = aIn + aToC * 0.33f,
                    anchor2 = aIn + aToC * 0.66f,
                    endPos = cOut
                };
                
                var spline1Entity = splineEntities[splineCount++];
                ecb.SetComponent(spline1Entity, aTurnToCSpline);
                tripleIntersection.aTurnToCSpline = spline1Entity;

                var bStraightToASpline = new Spline
                {
                    startPos = bIn,
                    anchor1 = bIn + bToA * 0.33f,
                    anchor2 = bIn + bToA * 0.66f,
                    endPos = aOut
                };
                
                var spline2Entity = splineEntities[splineCount++];
                ecb.SetComponent(spline2Entity, bStraightToASpline);
                tripleIntersection.bStraightToASpline = spline2Entity;

                var bTurnToCSpline = new Spline
                {
                    startPos = bIn,
                    anchor1 = bIn + bToC * 0.33f,
                    anchor2 = bIn + bToC * 0.66f,
                    endPos = cOut
                };
                
                var spline3Entity = splineEntities[splineCount++];
                ecb.SetComponent(spline3Entity, bTurnToCSpline);
                tripleIntersection.bTurnToCSpline = spline3Entity;

                var cTurnToASpline = new Spline
                {
                    startPos = cIn,
                    anchor1 = cIn + cToA * 0.33f,
                    anchor2 = cIn + cToA * 0.66f,
                    endPos = aOut
                };
                
                var spline4Entity = splineEntities[splineCount++];
                ecb.SetComponent(spline4Entity, cTurnToASpline);
                tripleIntersection.cTurnToASpline = spline4Entity;

                var cTurnToBSpline = new Spline
                {
                    startPos = cIn,
                    anchor1 = cIn + cToB * 0.33f,
                    anchor2 = cIn + cToB * 0.66f,
                    endPos = bOut
                };
                
                var spline5Entity = splineEntities[splineCount++];
                ecb.SetComponent(spline5Entity, cTurnToBSpline);
                tripleIntersection.cTurnToBSpline = spline5Entity;
                
                ecb.RemoveComponent<IntersectionNeedsInit>(entity);

            }).Run();
        
        ecb.Playback(EntityManager);
        ecb.Dispose();
    }
}
