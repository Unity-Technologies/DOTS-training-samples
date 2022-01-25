using Newtonsoft.Json.Serialization;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

public partial class MovementSystem : SystemBase
{
    protected override void OnUpdate()
    {
        var tAdd = Time.DeltaTime;

        /*
        //bits
        Entities
            .WithAll<BeeBitsTag>()
            .ForEach((Entity e, ref Translation translation) =>
            {
                // calculate bits falling movement - straight down for now
                if (translation.Value.y > 0)
                {
                    translation.Value = translation.Value * -9.8f * tAdd;
                }
                else
                {
                    //create an ECB, kill this Entity, Spawn a Blood here
                }
            }).Schedule();

        //blood
        Entities
            .WithAll<FoodTag>()
            .ForEach((Entity e, ref Translation translation, ref PP_Movement ppMovement, in Velocity velocity) =>
            {
                // calculate bee movement
                ppMovement.t += tAdd;
                var beePosition = math.lerp(ppMovement.startLocation, ppMovement.endLocation, math.smoothstep(0, 1, ppMovement.t));
                beePosition -= new float3(0, -1, 0); // food dangles below
                translation.Value = beePosition;

                // do orientation later

            }).Schedule();
*/

        //bees
        Dependency = Entities.ForEach((ref Translation translation, ref PP_Movement ppMovement, in BeeTag beeTag, in Velocity velocity) =>
        {
            // do bee movement
            ppMovement.t += tAdd;
            translation.Value = math.lerp(ppMovement.startLocation, ppMovement.endLocation, math.smoothstep(0, 1, ppMovement.t));
        }).Schedule(Dependency);

        //food, dependant on Bees.
        Dependency = Entities
            .WithAll<FoodTag>()
            .ForEach((Entity e, ref Translation translation, ref PP_Movement ppMovement, in Velocity velocity) =>
            {
                // calculate bee movement
                ppMovement.t += tAdd;

                if (!ppMovement.startLocation.Equals(ppMovement.endLocation))
                {
                    var beePosition = math.lerp(ppMovement.startLocation, ppMovement.endLocation,
                        math.smoothstep(0, 1, ppMovement.t));
                    beePosition -= new float3(0, 1, 0); // food dangles below
                    translation.Value = beePosition;
                }

                // do orientation later

            }).Schedule(Dependency);
    }
}
