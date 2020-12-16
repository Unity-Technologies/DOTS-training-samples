using Unity.Entities;
using Unity.Collections;
using Unity.Transforms;
using UnityEngine;
using Unity.Mathematics;

public class MovementSystem : SystemBase
{
    static float3 destination;

    protected void SetDestination()
    {
        var settingsEntity = GetSingletonEntity<Settings>();
        var settings = GetComponentDataFromEntity<Settings>()[settingsEntity];
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
                    // this can be simplified
                    if (translation.Value.x > destination.x)
                    {
                        translation.Value.x--;
                    }
                    else if (translation.Value.x < destination.x)
                    {
                        translation.Value.x++;
                    }
                    if (translation.Value.z > destination.z)
                    {
                        translation.Value.z--;
                    }
                    else if (translation.Value.z < destination.z)
                    {
                        translation.Value.z++;
                    }
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
}
