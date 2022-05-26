using Unity.Mathematics;

class TrackUtilities
{
    private const int NUM_LANES = 4;
    private const float LANE_SPACING = 1.9f;
    private const float MID_RADIUS = 31.46f;
    private const float CURVE_LANE0_RADIUS = MID_RADIUS - LANE_SPACING * (NUM_LANES - 1) / 2f;
    private const float MIN_HIGHWAY_LANE0_LENGTH = CURVE_LANE0_RADIUS * 4;
    private const float MIN_DIST_BETWEEN_CARS = .7f;
    private const float BASE_SCALE_Y = 6;

    public static float GetStraightawayLength(float lane0Length)
    {
        return (lane0Length - GetCurveLength(0) * 4) / 4;
    }

    public static float GetLaneLength(float lane0Length, int lane)
    {
        float straightPieceLength = GetStraightawayLength(lane0Length);
        return straightPieceLength * 4 + GetCurveLength(lane) * 4;
    }
    public static float GetCurveRadius(int lane)
    {
        return CURVE_LANE0_RADIUS + lane * LANE_SPACING;
    }
    public static float GetCurveLength(int lane)
    {
        return GetCurveRadius(lane) * math.PI / 2.0f;
    }

    /// <summary>
    /// Gets position of a car based on its lane and distance from the start in that lane.
    /// </summary>
    /// <param name="distance"></param>
    /// <param name="lane"></param>
    /// <param name="x"></param>
    /// <param name="z"></param>
    /// <param name="rotation">y rotation of the car, in radians.</param>
    public static void GetCarPosition(float lane0Length, float distance, int lane, out float x, out float z, out float rotation)
    {
        // keep distance in [0, length)
        distance = WrapDistance(lane0Length, distance, lane);

        float3 pos = float3.zero;
        quaternion rot = quaternion.identity;
        float angle = 0.0f;

        x = 0;
        z = 0;
        rotation = 0;

        float straightAwayLength = GetStraightawayLength(lane0Length);
        float curveLength = GetCurveLength(lane);

        for (int i = 0; i < 4; i++)
        {
            if (distance < straightAwayLength + curveLength)
            {
                float localX, localZ;
                if (distance < straightAwayLength)
                {
                    GetStraightPiecePosition(distance, lane, out localX, out localZ, out rotation);
                }
                else
                {
                    pos += math.mul(quaternion.RotateY(angle), new float3(0, 0, straightAwayLength));
                    distance -= straightAwayLength;
                    GetCurvePiecePosition(distance, lane, out localX, out localZ, out rotation);
                }
                RotateAroundOrigin(localX, localZ, angle, out x, out z);

                x += pos.x;
                z += pos.z;
                rotation += angle;
                break;
            }
            else
            {
                // Offset the position to the next pair of straightaway/curve pieces
                pos += math.mul(quaternion.RotateY(angle), new float3(0, 0, straightAwayLength));
                pos += math.mul(quaternion.RotateY(angle), new float3(MID_RADIUS, 0, MID_RADIUS));

                distance -= straightAwayLength + curveLength;
            }
            angle += math.PI / 2.0f;
        }
    }

    private static void GetStraightPiecePosition(float localDistance, int lane, out float x, out float z, out float rotation)
    {
        x = LANE_SPACING * ((NUM_LANES - 1) / 2f - lane);
        z = localDistance;
        rotation = 0;
    }

    private static void GetCurvePiecePosition(float localDistance, int lane, out float x, out float z, out float rotation)
    {
        float radius = GetCurveRadius(lane);
        float length = GetCurveLength(lane);
        float angle = (math.PI / 2.0f) * (localDistance / length);
        x = MID_RADIUS - math.cos(angle) * radius;
        z = math.sin(angle) * radius;
        rotation = angle;
    }

    private static void RotateAroundOrigin(float x, float z, float rotation, out float xOut, out float zOut)
    {
        float sin = math.sin(-rotation);
        float cos = math.cos(-rotation);

        xOut = x * cos - z * sin;
        zOut = x * sin + z * cos;
    }

    /// <summary>
    /// Wraps distance to be in [0, l), where l is the length of the given lane.
    /// </summary>
    public static float WrapDistance(float lane0Length, float distance, int lane)
    {
        float l = GetLaneLength(lane0Length, lane);
        return distance - math.floor(distance / l) * l;
    }

    public static float GetEquivalentDistance(float lane0Length, float distance, int lane, int otherLane)
    {
        // keep distance in [0, length)
        distance = WrapDistance(lane0Length, distance, lane);

        float accumulatedOtherDistance = 0.0f;

        float straightAwayLength = GetStraightawayLength(lane0Length);
        float curveLength = GetCurveLength(lane);
        float otherCurveLength = GetCurveLength(otherLane);

        for (int i = 0; i < 4; i++)
        {
            if (distance < straightAwayLength + curveLength)
            {
                if (distance <= straightAwayLength)
                {
                    accumulatedOtherDistance += distance;
                }
                else
                {
                    accumulatedOtherDistance += straightAwayLength;
                    distance -= straightAwayLength;

                    // We are in the curve section. Use the percentage along this lanes curve to calculate the distance along other curve
                    float otherCurveDistance = otherCurveLength * (distance / curveLength);
                    accumulatedOtherDistance += otherCurveDistance;
                }
                break;
            }
            else
            {
                distance -= straightAwayLength + curveLength;
                accumulatedOtherDistance += straightAwayLength + otherCurveLength;
            }
        }

        return accumulatedOtherDistance;
    }
}