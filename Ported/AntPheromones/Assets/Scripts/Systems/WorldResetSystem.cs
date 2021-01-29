using UnityEngine;
using Unity.Entities;
using Unity.Mathematics;

public class WorldResetSystem : SystemBase
{
    EntityQuery destroyMapSpawnedEntities;
    protected override void OnCreate()
    {
        RequireSingletonForUpdate<ObstacleBuilder>();

        destroyMapSpawnedEntities = GetEntityQuery(new EntityQueryDesc
        {
            Any = new ComponentType[]
                {
                    typeof(Obstacle),
                    typeof(Home),
                    typeof(Food),
                    typeof(RingElement)
                }
        });
    }

    protected override void OnUpdate()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            EntityCommandBuffer ecb = new EntityCommandBuffer(Unity.Collections.Allocator.TempJob);
            
            var pheromoneEntity = GetSingletonEntity<PheromoneStrength>();
            var pheromoneBuffer = GetBuffer<PheromoneStrength>(pheromoneEntity);
            
            for (int i = 0; i < pheromoneBuffer.Length; i++)
            {
                pheromoneBuffer[i] = 0;
            }
            
            ecb.DestroyEntity(destroyMapSpawnedEntities);

            //Can't be part of Query as it has linked entity reference
            Entities
                .WithAll<AntFoodEntityTracker>()
                .ForEach((Entity entity,in AntFoodEntityTracker antFoodEntityTracker) =>
                {
                    ecb.DestroyEntity(antFoodEntityTracker.AntFoodEntity);
                }).Run();
            
            Entities
                .WithAll<AntPathing>()
                .ForEach((Entity entity) =>
                {
                    ecb.DestroyEntity(entity);
                }).Run();

            Entities
               .WithAll<Initialized>()
               .ForEach((Entity entity) =>
               {
                   ecb.RemoveComponent<Initialized>(entity);
               }).Run();
            
            ecb.Playback(EntityManager);
            ecb.Dispose();
        }
    }

    public static float GetAngleFromorigin(float2 point)
    {
        point = math.normalize(point);

        float dot = math.dot(point, new float2(1, 0));
        float acos = math.acos(dot);

        float rot = 0 < math.dot(point, new float2(0, 1)) ? acos : (math.PI * 2.0F) - acos;

        return rot;
    }

    public static bool IsPointInsideRing(RingElement ring, float2 point)
    {
        if (ring.offsets.x-ring.halfThickness >= math.length(point))
        {
            return true;
        }

        return false;
    }

    public static bool IsBetween(float2 ab, float value)
    {
        if (ab.x < ab.y)
            return ab.x < value && ab.y > value;
        else
        {
            float delta = (ab.y + math.PI * 2.0F) - ab.x;
            if (value > ab.x)
                return value - ab.x < delta;

            if (value < ab.y)
                return ab.y - value < delta;

            return false;
        }
    }

    public static bool IsWithinOpening(RingElement ring, float2 point)
    {
        float yaw = GetAngleFromorigin(point);

        switch (ring.numberOfOpenings)
        {
            case 1:
                {
                    if (IsBetween(ring.opening0.angles, yaw))
                        return true;
                }
                break;
            case 2:
                {
                    if (IsBetween(ring.opening0.angles, yaw))
                        return true;
                    else if (IsBetween(ring.opening1.angles, yaw))
                        return true;
                }
                break;
        }

        return false;
    }

    public static float2 OffsetPointByRadius(float2 a, float2 b, float r)
    {
        float2 dir = b - a;
        float msq = math.lengthsq(dir);
        if (math.abs(math.pow(r, 2)) < msq)
        {
            dir = math.normalize(dir);
            float m = math.sqrt(msq);

            return a + dir * m - r;
        }

        return a;
    }

    static bool IsPointInCircle(float2 point, float2 center, float radius)
    {
        float2 line = point - center;
        float msq = math.lengthsq(line);

        return msq < math.pow(radius, 2);
    }

    public static bool DoesPathCollideWithRing(RingElement ring, float2 start, float2 end, out float2 at, out bool outwards)
    {
        outwards = math.lengthsq(start) < math.lengthsq(end);
        return CollideWithRing(ring, start, end, out at);
    }

    public static bool CollideWithRing(RingElement ring, float2 a, float2 b, out float2 at)
    {
        if (Inside(ring, a)) // starting off inside the ring
        {
            if (!Inside(ring, b)) // end point is outside the ring
            {
                int count = Intersections(ring.offsets.x, a, b, out float2 i0, out float2 i1);
                switch (count)
                {
                    case 1:
                        {
                            if (OnPath(a, b, i0))
                            {
                                if (InOpening(ring, i0))
                                {
                                    at = b;
                                    return false;
                                }
                                else
                                {
                                    at = OffsetPath(a, i0, ring.halfThickness);
                                    return true;
                                }
                            }
                            else
                            {
                                at = b;
                                return false;
                            }
                        }
                    case 2:
                        {
                            if (OnPath(a, b, i0))
                            {
                                if (OnPath(a, b, i1))
                                {
                                    float2 point = ClosestPoint(a, i0, i1);

                                    if (InOpening(ring, point))
                                    {
                                        at = b;
                                        return false;
                                    }
                                    else
                                    {
                                        at = OffsetPath(a, point, ring.halfThickness);
                                        return true;
                                    }
                                }
                                else // if (!OnPath(a, b, i1))
                                {
                                    if (InOpening(ring, i0))
                                    {
                                        at = b;
                                        return false;
                                    }
                                    else
                                    {
                                        at = OffsetPath(a, i0, ring.halfThickness);
                                        return true;
                                    }
                                }
                            }
                            else if (OnPath(a, b, i1)) // && !OnPath(a, b, i0))
                            {
                                if (InOpening(ring, i1))
                                {
                                    at = b;
                                    return false;
                                }
                                else
                                {
                                    at = OffsetPath(a, i1, ring.halfThickness);
                                    return true;
                                }
                            }
                            else
                            {
                                at = b;
                                return false;
                            }
                        }
                    default:
                        {
                            throw new System.Exception("WorldResetSystem.CollideWithRing() - inside->outside miss error.");
                        }
                }
            }
            else // Both points are inside the ring so ignore.
            {
                at = b;
                return false;
            }
        }
        else // if (Outside(ring, a))
        {
            int count = Intersections(ring.offsets.x, a, b, out float2 i0, out float2 i1);
            switch (count)
            {
                case 1:
                    {
                        if (OnPath(a, b, i0))
                        {
                            if (InOpening(ring, i0))
                            {
                                at = b;
                                return false;
                            }
                            else
                            {
                                at = OffsetPath(a, i0, ring.halfThickness);
                                return true;
                            }
                        }
                        else
                        {
                            at = b;
                            return false;
                        }
                    }
                case 2:
                    {
                        bool hit0 = OnPath(a, b, i0) && !InOpening(ring, i0);
                        bool hit1 = OnPath(a, b, i1) && !InOpening(ring, i1);
                        if (hit0 && hit1)
                        {
                            float2 point = ClosestPoint(a, i0, i1);
                            at = OffsetPath(a, point, ring.halfThickness);
                            return true;
                        }
                        else if (hit0)
                        {
                            at = OffsetPath(a, i0, ring.halfThickness);
                            return true;
                        }
                        else if (hit1)
                        {
                            at = OffsetPath(a, i1, ring.halfThickness);
                            return true;
                        }
                        else
                        {
                            at = b;
                            return false;
                        }
                    }
                default:
                    {
                        at = b;
                        return false;
                    }
            }
        }
    }

    static bool InOpening(RingElement ring, float2 point)
    {
        float rot = WorldResetSystem.GetAngleFromorigin(point);
        switch (ring.numberOfOpenings)
        {
            case 1:
                {
                    if (IsBetween(ring.opening0.angles, rot))
                    {
                        return true;
                    }
                }
                break;
            case 2:
                {
                    if (IsBetween(ring.opening0.angles, rot))
                    {
                        return true;
                    }

                    if (IsBetween(ring.opening1.angles, rot))
                    {
                        return true;
                    }
                }
                break;
        }

        return false;
    }

    static float2 OffsetPath(float2 a, float2 b, float offset)
    {
        float2 ab = b - a;
        if (math.pow(offset, 2) < math.lengthsq(ab))
        {
            float m = math.length(ab) - offset;
            return a + math.normalize(ab) * m;
        }

        return a;
    }

    static float2 ClosestPoint(float2 a, float2 point0, float2 point1)
    {
        return math.lengthsq(point0 - a) < math.lengthsq(point1 - a) ? point0 : point1;
    }

    static bool OnPath(float2 a, float2 b, float2 point)
    {
        float2 ab = b - a;
        float2 aPoint = point - a;

        if (0 < math.dot(ab, aPoint))
        {
            return math.lengthsq(ab) > math.lengthsq(aPoint);
        }

        return false;
    }

    static bool Inside(RingElement ring, float2 point)
    {
        return math.lengthsq(point) <= math.pow(ring.offsets.x, 2.0F);
    }

    // https://stackoverflow.com/questions/23016676/line-segment-and-circle-intersection
    public static float2 ClosestIntersection(float cx, float cy, float radius, float2 lineStart, float2 lineEnd)
    {
        float2 intersection1;
        float2 intersection2;
        int intersections = FindLineCircleIntersections(cx, cy, radius, lineStart, lineEnd, out intersection1, out intersection2);

        if (intersections == 1)
            return intersection1; // one intersection

        if (intersections == 2)
        {
            double dist1 = math.distance(intersection1, lineStart);
            double dist2 = math.distance(intersection2, lineStart);

            if (dist1 < dist2)
                return intersection1;
            else
                return intersection2;
        }

        return lineEnd;
    }

    [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
    static int Intersections(float radius, float2 point1, float2 point2, out float2 intersection1, out float2 intersection2)
    {
        return FindLineCircleIntersections(0, 0, radius, point1, point2, out intersection1, out intersection2);
    }
    static int FindLineCircleIntersections(float cx, float cy, float radius, float2 point1, float2 point2, out float2 intersection1, out float2 intersection2)
    {
        float dx, dy, A, B, C, det, t;

        dx = point2.x - point1.x;
        dy = point2.y - point1.y;

        A = dx * dx + dy * dy;
        B = 2 * (dx * (point1.x - cx) + dy * (point1.y - cy));
        C = (point1.x - cx) * (point1.x - cx) + (point1.y - cy) * (point1.y - cy) - radius * radius;

        det = B * B - 4 * A * C;
        if ((A <= 0.0000001) || (det < 0))
        {
            // No real solutions.
            intersection1 = default(float2);
            intersection2 = default(float2);

            return 0;
        }
        else if (det == 0)
        {
            // One solution.
            t = -B / (2 * A);
            intersection1 = new float2(point1.x + t * dx, point1.y + t * dy);
            intersection2 = default(float2);

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
