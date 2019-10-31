using Unity.Mathematics;

public static class Utilities
{
    public static bool IsMerging(this VehicleState state)
        => state == VehicleState.MERGE_LEFT || state == VehicleState.MERGE_RIGHT;

    public static float StraightPieceLength(float lane0Length)
        => (lane0Length - HighwayRacers.Highway.CURVE_LANE0_RADIUS * 4) / 4;

    public static float CurvePieceRadius(float lane)
        => HighwayRacers.Highway.CURVE_LANE0_RADIUS + lane * HighwayRacers.Highway.LANE_SPACING;

    public static float CurvePieceLength(float lane)
        => CurvePieceRadius(lane) * math.PI / 2;

    public static float LaneLength(float lane, float lane0Length)
        => StraightPieceLength(lane0Length) * 4 + CurvePieceLength(lane) * 4;

    public static float WrapPositionToLane(float position, float lane, float lane0Length)
    {
        float laneLen = LaneLength(lane, lane0Length);
        return position - math.floor(position / laneLen) * laneLen;
    }

    public static float ConvertPositionToLane(float position, float lane, float otherLane, float lane0Length)
    {
        // keep distance in [0, length)
        position = WrapPositionToLane(position, lane, lane0Length);

        var straightPieceLen = StraightPieceLength(lane0Length);
        var curvePieceLen = CurvePieceLength(lane);
        var otherCurvePieceLen = CurvePieceLength(otherLane);

        var otherPos = 0.0f;

        // 0 - straight
        if (position < straightPieceLen)
            return position;
        otherPos += straightPieceLen;
        position -= straightPieceLen;

        // 1 - curve
        if (position < curvePieceLen)
            return otherPos + position / curvePieceLen * otherCurvePieceLen;
        otherPos += otherCurvePieceLen;
        position -= curvePieceLen;

        // 2 - straight
        if (position < straightPieceLen)
            return otherPos + position;
        otherPos += straightPieceLen;
        position -= straightPieceLen;

        // 3 - curve
        if (position < curvePieceLen)
            return otherPos + position / curvePieceLen * otherCurvePieceLen;
        otherPos += otherCurvePieceLen;
        position -= curvePieceLen;

        // 4 - straight
        if (position < straightPieceLen)
            return otherPos + position;
        otherPos += straightPieceLen;
        position -= straightPieceLen;

        // 5 - curve
        if (position < curvePieceLen)
            return otherPos + position / curvePieceLen * otherCurvePieceLen;
        otherPos += otherCurvePieceLen;
        position -= curvePieceLen;

        // 6 - straight
        if (position < straightPieceLen)
            return otherPos + position;
        otherPos += straightPieceLen;
        position -= straightPieceLen;

        // 7 - curve
        UnityEngine.Debug.Assert(position <= curvePieceLen);
        return otherPos + position / curvePieceLen * otherCurvePieceLen;
    }

    // Returns the positive distance, wrapped in [0, length), from distance1 (in lane1) to distance2 (in lane2) in lane 1.
    public static float DistanceTo(float position1, float lane1, float position2, float lane2, float lane0Length)
    {
        return WrapPositionToLane(ConvertPositionToLane(position2, lane2, lane1, lane0Length) - position1, lane1, lane0Length);
    }
}
