using Unity.Entities;
using UnityMath = Unity.Mathematics;

public partial class Utils
{

    public readonly struct LinecastResult
    {
        public readonly bool isHit;
        public readonly UnityMath.float2 hitPoint;
        public readonly UnityMath.float2 hitNormal;
        public readonly float hitDistance;
        public readonly bool isFood;
        public readonly bool isObstacle;
        public readonly bool isColony;
        public readonly bool isPheromone;
        public readonly float pheromone;

        public LinecastResult(in bool isHit = false)
        {
            this.isHit = isHit;
            this.hitPoint = UnityMath.float2.zero;
            this.hitNormal = UnityMath.float2.zero;
            this.hitDistance = -1f;
            this.isFood = false;
            this.isObstacle = false;
            this.isColony = false;
            this.isPheromone = false;
            this.pheromone = 0;
        }

        public LinecastResult(
                in UnityMath.float2 hitPoint,
                in UnityMath.float2 hitNormal,
                in float hitDistance = -1f,
                in bool isFood = false,
                in bool isObstacle = false,
                in bool isColony = false,
                in bool isPheromone = false,
                in float pheromone = 0
            )
        {
            this.isHit = true;
            this.hitPoint = hitPoint;
            this.hitNormal = hitNormal;
            this.hitDistance = hitDistance;
            this.isFood = isFood;
            this.isObstacle = isObstacle;
            this.isColony = isColony;
            this.isPheromone = false;
            this.pheromone = pheromone;
        }
    }

    public readonly struct PositionAndRadius
    {
        public readonly float radius;
        public readonly UnityMath.float2 position;
    }

    public readonly struct LineCircleIntersectionResult
    {
        public readonly bool IsIntersection;
        public readonly UnityMath.float2 p1;
        public readonly UnityMath.float2 p2;

        public LineCircleIntersectionResult(
                in bool isValid = false
            )
        {
            this.IsIntersection = false;
            this.p1 = UnityMath.float2.zero;
            this.p2 = UnityMath.float2.zero;
        }

        public LineCircleIntersectionResult(
                in UnityMath.float2 p1,
                in UnityMath.float2 p2
            )
        {
            this.IsIntersection = true;
            this.p1 = p1;
            this.p2 = p2;
        }
    }

    public static LineCircleIntersectionResult LineCircleIntersection(in UnityMath.float2 start, in UnityMath.float2 d, in UnityMath.float2 center, in float r)
    {
        var f = start - center;
        var a = UnityMath.math.dot(d, d);
        var b = 2 * UnityMath.math.dot(f, d);
        var c = UnityMath.math.dot(f, f) - r * r;
        var discriminant = b * b - 4 * a * c;
        if (discriminant < 0) return new LineCircleIntersectionResult(false);

        discriminant = UnityMath.math.sqrt(b * b - 4 * a * c);
        float t1 = (-b - discriminant) / (2 * a);
        float t2 = (-b + discriminant) / (2 * a);

        return new LineCircleIntersectionResult(start + t1 * d, start + t2 * d);
    }

    public static bool LinecastObstacles(
        in Grid2D grid,
        in DynamicBuffer<ObstaclePositionAndRadius> obstacles,
        in UnityMath.float2 start,
        in UnityMath.float2 end,
        int steps = 3
        )
    {
        var diff = end - start;
        var dist = UnityMath.math.sqrt(UnityMath.math.dot(diff, diff));
        var norm = UnityMath.math.normalize(diff);
        float stepSize = dist - steps;

        for (int i = 1; i <= steps; i++)
        {
            var delta = norm * stepSize * i;
            var p = start + delta;
            var xIdx = (int)UnityMath.math.floor(UnityMath.math.clamp(p.x + 0.5f, 0, 1) * grid.rowLength);
            var yIdx = (int)UnityMath.math.floor(UnityMath.math.clamp(p.y + 0.5f, 0, 1) * grid.columnLength);
            var idx = UnityMath.math.min(xIdx + yIdx * grid.rowLength, obstacles.Length - 1);

            // check for obstacles
            if(obstacles[idx].IsValid) return true;
        }

        return false;
    }

    public static LinecastResult Linecast(
        in Grid2D grid,
        in DynamicBuffer<ObstaclePositionAndRadius> obstacles, 
        in DynamicBuffer<Pheromone> pheromone,
        in PositionAndRadius food,
        in PositionAndRadius home,
        in UnityMath.float2 start, 
        in UnityMath.float2 end, 
        int steps = 3)
    {
        var diff = end - start;
        var dist = UnityMath.math.sqrt(UnityMath.math.dot(diff,diff));
        var norm = UnityMath.math.normalize(diff);
        float stepSize = dist - steps;
        var worldBoundariesLowLeft = new UnityMath.float2(-0.5f, -0.5f);
        var worldBoundariesUpperRigth = new UnityMath.float2(0.5f, 0.5f);

        for (int i = 1; i <= steps; i++)
        {
            var delta = norm * stepSize * i;
            var p = start + delta;

            // check world boundaries
            if(p.x < worldBoundariesLowLeft.x)
            {
                if (p.y < worldBoundariesLowLeft.y) return new LinecastResult(worldBoundariesLowLeft, UnityMath.math.normalize(new UnityMath.float2(1f, 1f)));
                else if (p.y > worldBoundariesUpperRigth.y) return new LinecastResult(new UnityMath.float2(-0.5f, 0.5f), UnityMath.math.normalize(new UnityMath.float2(1f, -1f)));
                else return new LinecastResult(new UnityMath.float2(-0.5f, start.y + UnityMath.math.abs(-0.5f - start.x) * delta.y / delta.x), UnityMath.math.normalize(new UnityMath.float2(1f, 0.0f)));
            }
            else if(p.x > worldBoundariesUpperRigth.x)
            {
                if (p.y < worldBoundariesLowLeft.y) return new LinecastResult(new UnityMath.float2(0.5f, -0.5f), UnityMath.math.normalize(new UnityMath.float2(-1f, 1f)));
                else if (p.y > worldBoundariesUpperRigth.y) return new LinecastResult(new UnityMath.float2(0.5f, 0.5f), UnityMath.math.normalize(new UnityMath.float2(-1f, -1f)));
                else return new LinecastResult(new UnityMath.float2(0.5f, start.y + UnityMath.math.abs(0.5f - start.x) * delta.y / delta.x), UnityMath.math.normalize(new UnityMath.float2(-1f, 0.0f)));
            }
            else if(p.y < worldBoundariesLowLeft.y)
            {
                return new LinecastResult(new UnityMath.float2(start.x + UnityMath.math.abs(-0.5f - start.y) * delta.x / delta.y, -0.5f), UnityMath.math.normalize(new UnityMath.float2(0f, 1f)));
            }
            else if(p.y > worldBoundariesUpperRigth.y)
            {
                return new LinecastResult(new UnityMath.float2(start.x + UnityMath.math.abs(0.5f - start.y) * delta.x / delta.y, 0.5f), UnityMath.math.normalize(new UnityMath.float2(0f, -1f)));
            }

            var xIdx = (int)UnityMath.math.floor(UnityMath.math.clamp(p.x + 0.5f, 0, 1) * grid.rowLength);
            var yIdx = (int)UnityMath.math.floor(UnityMath.math.clamp(p.y + 0.5f, 0, 1) * grid.columnLength);
            var idx = UnityMath.math.min(xIdx + yIdx * grid.rowLength, obstacles.Length - 1);
            LineCircleIntersectionResult inters;

            // check for obstacles
            if (obstacles[idx].IsValid)
            {
                inters = LineCircleIntersection(start, delta, obstacles[idx].position, obstacles[idx].radius);
                var point = UnityMath.math.distancesq(start, inters.p1) < UnityMath.math.distancesq(start, inters.p2) ? inters.p1 : inters.p2;
                var normal = UnityMath.math.normalize(point - obstacles[idx].position);
                return new LinecastResult(point, normal, stepSize * i, false, true);
            }

            // check for food
            inters = LineCircleIntersection(start, delta, food.position, food.radius);
            if (inters.IsIntersection)
            {
                var point = UnityMath.math.distancesq(start, inters.p1) < UnityMath.math.distancesq(start, inters.p2) ? inters.p1 : inters.p2;
                var normal = UnityMath.math.normalize(point - food.position);
                return new LinecastResult(point, normal, stepSize * i, true);
            }

            // check for home
            inters = LineCircleIntersection(start, delta, home.position, home.radius);
            if (inters.IsIntersection)
            {
                var point = UnityMath.math.distancesq(start, inters.p1) < UnityMath.math.distancesq(start, inters.p2) ? inters.p1 : inters.p2;
                var normal = UnityMath.math.normalize(point - home.position);
                return new LinecastResult(point, normal, stepSize * i, true);
            }

            // check for pheromone
            if(pheromone[idx].Value > 0)
            {
                var point = p;
                var normal = -norm;
                return new LinecastResult(point, normal, stepSize * i, false, false, false, true, pheromone[idx].Value);
            }
        }

        return new LinecastResult();
    }
}