using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Rendering;
using Unity.Transforms;

[UpdateInGroup(typeof(InitializationSystemGroup))]
public class PlatformSpawnerSystem : SystemBase
{
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

                for (int i = 0; i < stopPoints.Length; ++i)
                {
                    float stopPoint = stopPoints[i];
                    float3 position = CarMovementSystem.Get_Position(stopPoint, points);
                    ecb.Instantiate(spawnerData.PlatformPrefab);
                }
            }
        }).Run();

        Enabled = false;

        ecb.Playback(EntityManager);
        ecb.Dispose();
    }
}
