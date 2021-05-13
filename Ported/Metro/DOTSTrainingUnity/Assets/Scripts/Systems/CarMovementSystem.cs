// Any type inheriting from SystemBase will be registered as a system and will start
// updating every frame.
using Unity.Entities;
using Unity.Transforms;
using Unity.Collections;
using Unity.Mathematics;
public class CarMovementSystem : SystemBase
{
    public const float trainCarLength = 3.0f;
    //EntityArchetype trainCarArchetype = World.EntityManager.CreateArchetype()
    protected override void OnUpdate()
    {
        var time = Time.ElapsedTime;
        NativeArray<BezierPoint> allBezierPaths = Line.allBezierPathSubarrays;
        NativeArray<int> bezierPathIndices = Line.bezierPathSubarrayIndices;
        NativeArray<float> allDistances = Line.allDistances;

        ComponentDataFromEntity<TrainCurrDistance> trainDistances = GetComponentDataFromEntity<TrainCurrDistance>(true);
        ComponentDataFromEntity<TrackIndex> trainTrackIndices = GetComponentDataFromEntity<TrackIndex>(true);
        
        Entities
            .WithReadOnly(allBezierPaths)
            .WithReadOnly(bezierPathIndices)
            .WithReadOnly(allDistances)
            .WithReadOnly(trainDistances)
            .WithReadOnly(trainTrackIndices)
            .ForEach((ref Translation translation, ref Rotation rotation, in TrainCarIndex carIndex,
            in TrainEngineRef engineRef) =>
        {
            //float trainDistance = 12.0f;// entityManager.GetComponentData<TrainCurrDistance>(engineRef.value).value;
            int engineTrackIndex = trainTrackIndices[engineRef.value].value; //entityManager.GetComponentData<TrackIndex>(engineRef.value).value;
            float trainDistance = trainDistances[engineRef.value].value;
            //int engineTrackIndex = trainTrackIndices[engineRef.value].value;
            
            int startIndex = bezierPathIndices[engineTrackIndex];
            float distance = allDistances[engineTrackIndex];
            
            float carDistance = trainDistance - (trainCarLength * carIndex.value);
            if (carDistance < 0)
                carDistance += distance;
            
            int onePastEndIndex;
            
            if (engineTrackIndex == bezierPathIndices.Length - 1)
            {
                onePastEndIndex = allBezierPaths.Length;
            }
            else
            {
                onePastEndIndex = bezierPathIndices[engineTrackIndex + 1];
            }
            
            int length = onePastEndIndex - startIndex;
            
            NativeArray<BezierPoint> points = allBezierPaths.GetSubArray(startIndex, length);
            
            float3 position = Get_Position(carDistance, points);
            float3 aheadPosition = Get_Position((carDistance + 0.01f) % distance, points);
            
            float3 normalAtPosition = math.normalize(aheadPosition - position);
            
            //float3 lookAtDirection = position - normalAtPosition;
            
            quaternion lookRotation = quaternion.LookRotation(normalAtPosition, new float3(0f, 1f, 0f));

            translation.Value = position;
            rotation.Value = lookRotation;
        
        }).Run();
    }

    public static float3 Get_Position(float sampleDistance, NativeArray<BezierPoint> points)
    {
        int pointIndex_region_start = 0;
        int totalPoints = points.Length;
        for (int i = 0; i < totalPoints; i++)
        {
            BezierPoint _PT = points[i];
            if (_PT.distanceAlongPath <= sampleDistance)
            {
                if (i == totalPoints - 1)
                {
                    // end wrap
                    pointIndex_region_start = i;
                    break;
                }
                else if (points[i + 1].distanceAlongPath >= sampleDistance)
                {
                    // start < progress, end > progress <-- thats a match
                    pointIndex_region_start = i;
                    break;
                }
                else
                {
                    continue;
                }
            }
        }

        int pointIndex_region_end = (pointIndex_region_start + 1) % points.Length;

        // get start and end bez points
        BezierPoint point_region_start = points[pointIndex_region_start];
        BezierPoint point_region_end = points[pointIndex_region_end];
        // lerp between the points to arrive at PROGRESS
        float pathProgress_start = point_region_start.distanceAlongPath;
        float pathProgress_end = (pointIndex_region_end != 0) ? point_region_end.distanceAlongPath : 1;
        float regionProgress = (sampleDistance - pathProgress_start) / (pathProgress_end - pathProgress_start);

        // do your bezier lerps
        // Round 1 --> Origins to handles, handle to handle

        // Round 1 --> Origins to handles, handle to handle
        float3 l1_a_aOUT = math.lerp(point_region_start.location, point_region_start.handle_out, regionProgress);
        float3 l2_bIN_b = math.lerp(point_region_end.handle_in, point_region_end.location, regionProgress);
        float3 l3_aOUT_bIN = math.lerp(point_region_start.handle_out, point_region_end.handle_in, regionProgress);
        // Round 2 
        float3 l1_to_l3 = math.lerp(l1_a_aOUT, l3_aOUT_bIN, regionProgress);
        float3 l3_to_l2 = math.lerp(l3_aOUT_bIN, l2_bIN_b, regionProgress);
        // Final Round
        float3 result = math.lerp(l1_to_l3, l3_to_l2, regionProgress);
        return result;
    }

    public int GetRegionIndex(float _progress, NativeArray<BezierPoint> points)
    {
        int result = 0;
        int totalPoints = points.Length;
        for (int i = 0; i < totalPoints; i++)
        {
            BezierPoint _PT = points[i];
            if (_PT.distanceAlongPath <= _progress)
            {
                if (i == totalPoints - 1)
                {
                    // end wrap
                    result = i;
                    break;
                }
                else if (points[i + 1].distanceAlongPath >= _progress)
                {
                    // start < progress, end > progress <-- thats a match
                    result = i;
                    break;
                }
                else
                {
                    continue;
                }
            }
        }
        return result;
    }

    public static float3 BezierLerp(BezierPoint _pointA, BezierPoint _pointB, float _progress)
    {
        // Round 1 --> Origins to handles, handle to handle
        float3 l1_a_aOUT = math.lerp(_pointA.location, _pointA.handle_out, _progress);
        float3 l2_bIN_b = math.lerp(_pointB.handle_in, _pointB.location, _progress);
        float3 l3_aOUT_bIN = math.lerp(_pointA.handle_out, _pointB.handle_in, _progress);
        // Round 2 
        float3 l1_to_l3 = math.lerp(l1_a_aOUT, l3_aOUT_bIN, _progress);
        float3 l3_to_l2 = math.lerp(l3_aOUT_bIN, l2_bIN_b, _progress);
        // Final Round
        float3 result = math.lerp(l1_to_l3, l3_to_l2, _progress);
        return result;
    }

    //public static float3 Get_NormalAtPosition(float _position, float3 _spatialPosition)
    //{
    //    float3 _ahead = Get_Position((_position + 0.0001f) % 1f);
    //    return (_ahead - _spatialPosition) / Vector3.Distance(_ahead, _spatialPosition);
    //}
}