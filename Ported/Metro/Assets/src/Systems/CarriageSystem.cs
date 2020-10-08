using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

public class CarriageSystem : SystemBase
{
    protected override void OnUpdate()
    {
        var railPointFromEntity = GetBufferFromEntity<RailPoint>(true);
        var railPointDistanceFromEntity = GetBufferFromEntity<RailPointDistance>(true);
        
        Entities
            .WithReadOnly(railPointFromEntity)
            .WithReadOnly(railPointDistanceFromEntity)
            .ForEach((ref Translation translation, ref Rotation rotation, in Rail rail, in CarriagePosition carriagePosition) =>
            {
                var railPoints = railPointFromEntity[rail]; // GetBuffer<RailPoint>(rail);
                var railPointsDistances = railPointDistanceFromEntity[rail]; // GetBuffer<RailPointDistance>(rail);
                var railLength = GetComponent<RailLength>(rail);

                var pointA = SampleRail(railPoints, railPointsDistances, railLength, carriagePosition - TrainCarriage.CARRIAGE_LENGTH / 4f);
                var pointB = SampleRail(railPoints, railPointsDistances, railLength, carriagePosition + TrainCarriage.CARRIAGE_LENGTH / 4f);

                translation = new Translation {Value = (pointA + pointB) * 0.5f};
                rotation = new Rotation { Value = quaternion.LookRotation(math.normalize(pointB - pointA), new float3(0f, 1f, 0f))};
            }).ScheduleParallel();
    }

    static float3 SampleRail(DynamicBuffer<RailPoint> points, DynamicBuffer<RailPointDistance> pointDistances, float length, float distance)
    {
        if (distance < 0f)
            distance += length;
        else if (distance >= length)
            distance -= length;

        for (int i = 0; i < pointDistances.Length; i++)
        {
            var pointDistance = (float)pointDistances[i];
            if (distance <= pointDistance)
            {
                var nextPointIndex = i + 1;
                if (nextPointIndex == pointDistances.Length)
                    nextPointIndex = 0;

                var pointA = points[i];
                var pointB = points[nextPointIndex];

                var previousPointDistance = i == 0 ? 0f : (float)pointDistances[i - 1];

                var distanceBetweenPoints = pointDistance - previousPointDistance;
                distance -= previousPointDistance;

                return math.lerp(pointA, pointB, distance / distanceBetweenPoints);
            }
        }

        return 0f;

        
        /* var pointDistance = distance / length * points.Length;
        var pointIndex = (int)math.floor(pointDistance);
        var lerpTime = pointDistance - pointIndex;
        var nextPoint = pointIndex + 1;
        if (nextPoint == points.Length)
            nextPoint = 0;
        return math.lerp(points[pointIndex], points[nextPoint], lerpTime); */
    }
}
