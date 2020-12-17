using Unity.Entities;
using Unity.Transforms;
using Unity.Mathematics;
using Unity.Collections;

public class MovementSystem : SystemBase
{
    const float ySpeed = 2f;
    const float xzSpeed = 6f;
    const float k_walkSpeed = 3f;

    protected override void OnUpdate()
    {
        var settings = GetSingleton<CommonSettings>();
        var deltaTime = Time.DeltaTime;
        var ecb = new EntityCommandBuffer(Allocator.Temp);

        Entities
            .WithAll<Drone>()
            .ForEach((Entity entity, ref Translation translation, ref Drone drone, ref DroneCheckPoints checkPoints, ref Rotation rotation) =>
            {
                // translate drone to new place
                if (translation.Value.x != checkPoints.destination.x && translation.Value.z != checkPoints.destination.z)
                {
                    var moveSmoothing = 1f - math.pow(drone.moveSmooth, deltaTime);
                    drone.smoothPosition = math.lerp(drone.smoothPosition, translation.Value, moveSmoothing);

                    var tilt = new float3(translation.Value.x - drone.smoothPosition.x, 2f, translation.Value.z - drone.smoothPosition.z);
                    rotation.Value = FromToRotation(math.up(), tilt);
                    translation.Value = MoveTowards(translation.Value, checkPoints.destination, xzSpeed * deltaTime);
                    translation.Value.y = MoveTowards(translation.Value.y, checkPoints.destination.y, ySpeed * deltaTime);
                }
                else
                {
                    // We have arrived - adjust the current height on the Y

                    // set new destination

                    // will need the tile state later
                    // var tileBufferAccessor = this.GetBufferFromEntity<TileState>();
                    var random = new Random(1234);
                    // generate a random destination to act as placeholder crop
                    int x = random.NextInt(0, settings.GridSize.x);
                    int y = random.NextInt(0, settings.GridSize.y);
                    // each drone has the same destination right now - this should be an array with indexes to handle many drone bois
                    checkPoints.destination = new float3(x, 1f, y);
                }

            }).Run();

        Entities
            .ForEach((Entity entity, ref Translation translation, in Velocity velocity) =>
            {
                translation.Value = MoveTowards(translation.Value, velocity.Value, k_walkSpeed * deltaTime);
            }).Run();

        ecb.Playback(EntityManager);
    }

    public static float MoveTowards(float current, float target, float maxDelta)
    {
        if (math.abs(target - current) <= maxDelta)
            return target;
        return current + math.sign(target - current) * maxDelta;
    }

    public static float2 MoveTowards(float2 current, float2 target, float maxDistanceDelta)
    {
        // avoid vector ops because current scripting backends are terrible at inlining
        float toVector_x = target.x - current.x;
        float toVector_y = target.y - current.y;
        float sqDist = toVector_x * toVector_x + toVector_y * toVector_y;
        if (sqDist == 0 || (maxDistanceDelta >= 0 && sqDist <= maxDistanceDelta * maxDistanceDelta))
            return target;
        float dist = (float)math.sqrt(sqDist);
        return new float2(current.x + toVector_x / dist * maxDistanceDelta, current.y + toVector_y / dist * maxDistanceDelta);
    }

    public static float3 MoveTowards(float3 current, float3 target, float maxDistanceDelta)
    {
        // avoid vector ops because current scripting backends are terrible at inlining
        float toVector_x = target.x - current.x;
        float toVector_y = target.y - current.y;
        float toVector_z = target.z - current.z;
        float sqdist = toVector_x * toVector_x + toVector_y * toVector_y + toVector_z * toVector_z;
        if (sqdist == 0 || (maxDistanceDelta >= 0 && sqdist <= maxDistanceDelta * maxDistanceDelta))
            return target;
        var dist = (float)math.sqrt(sqdist);
        return new float3(current.x + toVector_x / dist * maxDistanceDelta,
            current.y + toVector_y / dist * maxDistanceDelta,
            current.z + toVector_z / dist * maxDistanceDelta);
    }

    public static quaternion FromToRotation(float3 from, float3 to)
     => quaternion.AxisAngle(
         angle: math.acos(math.clamp(math.dot(math.normalize(from), math.normalize(to)), -1f, 1f)),
         axis: math.normalize(math.cross(from, to))
     );
}