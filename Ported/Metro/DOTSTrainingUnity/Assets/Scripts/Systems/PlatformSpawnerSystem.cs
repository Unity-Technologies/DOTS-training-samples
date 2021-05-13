using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Rendering;
using Unity.Transforms;

[UpdateInGroup(typeof(InitializationSystemGroup))]
public class PlatformSpawnerSystem : SystemBase
{
    private static float3 GetTrackDirection(float stopPoint, NativeArray<BezierPoint> trackPoints)
    {
        float pointAhead = stopPoint + 10f;
        float3 stopPointPosition = CarMovementSystem.Get_Position(stopPoint, trackPoints);
        float3 aheadPointPosition = CarMovementSystem.Get_Position(pointAhead, trackPoints);

        return aheadPointPosition - stopPointPosition;
    }

    private static quaternion GetRotationFromDirection(float3 direction)
    {
        float3 yAxis = new float3(0, 1f, 0);
        float3 referenceDirection = new float3(1, 0, 0);
        float angle = math.acos(math.dot(direction, referenceDirection) / math.length(direction) / math.length(referenceDirection));
        return quaternion.AxisAngle(yAxis, angle);
    }

    protected override void OnUpdate()
    {
        NativeArray<float> stopPointSubarrays = Line.allStopPointSubarrays;
        NativeArray<int> stopPointArrayIndices = Line.stopPointSubarrayIndices;
        NativeArray<int> numStopPointsInLine = Line.numStopPointsInLine;

        NativeArray<BezierPoint> bezierPointsSubarrays = Line.allBezierPathSubarrays;
        NativeArray<int> bezierPointArrayIndices = Line.bezierPathSubarrayIndices;
        
        var ecb = new EntityCommandBuffer(Allocator.Temp);

        Entities.ForEach((Entity entity, in PlatformSpawnerComponent spawnerData) =>
        {
            for (int lineNum = 0; lineNum < numStopPointsInLine.Length; ++lineNum)
            {
                int subarrayOffset = stopPointArrayIndices[lineNum];
                int numStopPoints = numStopPointsInLine[lineNum];

                NativeArray<float> stopPoints = stopPointSubarrays.GetSubArray(subarrayOffset, numStopPoints);
               
                int startIndex = bezierPointArrayIndices[lineNum];
                int onePastEndIndex;

                if (lineNum == bezierPointArrayIndices.Length - 1)
                {
                    onePastEndIndex = bezierPointsSubarrays.Length;
                }
                else
                {
                    onePastEndIndex = bezierPointArrayIndices[lineNum + 1];
                }
                int length = onePastEndIndex - startIndex;

                NativeArray<BezierPoint> points = bezierPointsSubarrays.GetSubArray(startIndex, length);

                for (int i = 0; i < stopPoints.Length / 2; ++i)
                {
                    float stopPoint = stopPoints[i];
                    float3 position = CarMovementSystem.Get_Position(stopPoint, points);

                    float3 direction = GetTrackDirection(stopPoint, points);
                    quaternion rotation = GetRotationFromDirection(direction);
                    float3 offset = math.rotate(rotation, new float3(0, 0, 5.25f));
                    Entity platform = ecb.Instantiate(spawnerData.PlatformPrefab);
                    ecb.SetComponent(platform, new Translation() { Value = position + offset });
                    ecb.SetComponent(platform, new Rotation() { Value = rotation });
                }
            }
        }).Run();

        Enabled = false;

        ecb.Playback(EntityManager);
        ecb.Dispose();
    }
}
