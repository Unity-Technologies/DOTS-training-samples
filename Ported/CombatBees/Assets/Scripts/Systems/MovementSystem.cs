using Newtonsoft.Json.Serialization;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

public partial class MovementSystem : SystemBase
{
    protected override void OnUpdate()
    {
        //bees and bits and blood
        var beeHandle = Entities.ForEach((ref Translation translation, ref PP_Movement ppMovement, in BeeTag beeTag, in Velocity velocity) =>
        {
            // do bee movement
            ppMovement.t += Time.DeltaTime;
            translation.Value = math.lerp(ppMovement.startLocation, ppMovement.endLocation, math.smoothstep(0, 1, ppMovement.t));
        }).Schedule(Dependency);

        //food, dependant on Bees.
        Entities
            .WithAll<FoodTag>()
            .ForEach((Entity e, ref Translation translation, ref PP_Movement ppMovement, in Velocity velocity) =>
            {
                // calculate bee movement
                ppMovement.t += Time.DeltaTime;
                var beePosition = math.lerp(ppMovement.startLocation, ppMovement.endLocation, math.smoothstep(0, 1, ppMovement.t));
                beePosition -= new float3(0, -1, 0); // food dangles below

                // do orientation later

            }).Schedule(beeHandle);
    }
}
