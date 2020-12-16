using Unity.Entities;
using Unity.Collections;
using Unity.Transforms;
using UnityEngine;
using Unity.Mathematics;

public class MovementSystem : SystemBase
{
    const float ySpeed = 2f;
    const float xzSpeed = 6f;
    static float3 destination;

    protected void SetDestination()
    {
        var settings = GetSingleton<CommonSettings>();
        
        // will need the tile state later
        // var tileBufferAccessor = this.GetBufferFromEntity<TileState>();

        // generate a random destination to act as placeholder crop
        int x = UnityEngine.Random.Range(0, settings.GridSize.x);
        int y = UnityEngine.Random.Range(0, settings.GridSize.y);
        // each drone has the same destination right now - this should be an array with indexes to handle many drone bois
        destination = new float3(x, 1f, y);
    }

    protected override void OnStartRunning()
    {
        base.OnStartRunning();
        SetDestination();
    }

    protected override void OnUpdate()
    {
        var deltaTime = Time.DeltaTime;
        Entities
            .WithAll<Drone>()
            .ForEach((Entity entity, ref Translation translation, ref Drone drone) =>
            {
                // translate drone to new place
                if (translation.Value.x != destination.x && translation.Value.z != destination.z)
                {
                    translation.Value.x = Mathf.MoveTowards(translation.Value.x, destination.x, xzSpeed * deltaTime);
                    translation.Value.y = Mathf.MoveTowards(translation.Value.y, destination.y, ySpeed * deltaTime);
                    translation.Value.z = Mathf.MoveTowards(translation.Value.z, destination.z, xzSpeed * deltaTime);
                }
                else
                {
                    // We have arrived - adjust the current height on the Y

                    // set new dest
                    SetDestination();
                }

            }).WithoutBurst().Run();

        Entities
            .ForEach((Entity entity, ref Translation translation, in Velocity velocity) =>
            {
                translation.Value += velocity.Value * deltaTime;
            }).Run();
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
}