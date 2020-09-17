using Unity.Mathematics;

public class MathHelper
{
    //circlePos.x,circlePos.y is center point of the circle 
    public static float2 ClosestIntersection(float2 circlePos, float radius,
                                      float2 lineStart, float2 lineEnd)
    {
        float2 intersection1;
        float2 intersection2;
        int intersections = FindLineCircleIntersections(circlePos, radius, lineStart, lineEnd, out intersection1, out intersection2);

        if (intersections == 1)
            return intersection1; // one intersection

        if (intersections == 2)
        {
            double dist1 = Distance(intersection1, lineStart);
            double dist2 = Distance(intersection2, lineStart);

            if (dist1 < dist2)
                return intersection1;
            else
                return intersection2;
        }

        return float2.zero; // no intersections at all
    }

    private static double Distance(float2 p1, float2 p2)
    {
        return math.sqrt(math.pow(p2.x - p1.x, 2) + math.pow(p2.y - p1.y, 2));
    }

    // Find the points of intersection.
    public static int FindLineCircleIntersections(float2 circlePos, float radius,
                                            float2 point1, float2 point2, out
                                            float2 intersection1, out float2 intersection2)
    {
        float dx, dy, A, B, C, det, t;

        dx = point2.x - point1.x;
        dy = point2.y - point1.y;

        A = dx * dx + dy * dy;
        B = 2 * (dx * (point1.x - circlePos.x) + dy * (point1.y - circlePos.y));
        C = (point1.x - circlePos.x) * (point1.x - circlePos.x) + (point1.y - circlePos.y) * (point1.y - circlePos.y) - radius * radius;

        det = B * B - 4 * A * C;
        if ((A <= 0.0000001) || (det < 0))
        {
            // No real solutions.
            intersection1 = new float2(float.NaN, float.NaN);
            intersection2 = new float2(float.NaN, float.NaN);
            return 0;
        }
        else if (det == 0)
        {
            // One solution.
            t = -B / (2 * A);
            intersection1 = new float2(point1.x + t * dx, point1.y + t * dy);
            intersection2 = new float2(float.NaN, float.NaN);
            return 1;
        }
        else
        {
            // Two solutions.
            t = (float)((-B + math.sqrt(det)) / (2 * A));
            intersection1 = new float2(point1.x + t * dx, point1.y + t * dy);
            t = (float)((-B - math.sqrt(det)) / (2 * A));
            intersection2 = new float2(point1.x + t * dx, point1.y + t * dy);
            return 2;
        }
    }
}
