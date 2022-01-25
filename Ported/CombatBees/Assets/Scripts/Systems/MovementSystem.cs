using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using Unity.Collections;

public partial class MovementSystem : SystemBase
{
    private EntityCommandBufferSystem sys;
    protected override void OnCreate()
    {
        sys = World.GetExistingSystem<EndSimulationEntityCommandBufferSystem>();
        RequireSingletonForUpdate<Spawner>();
    }

    protected override void OnUpdate()
    {
        var tAdd = Time.DeltaTime;

        var spawner = GetSingleton<Spawner>();

        //bits

        var ecb = sys.CreateCommandBuffer();
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
                    //destroy this entity and create/init a blood splat
                    ecb.DestroyEntity(e);
                    var instance = ecb.Instantiate(spawner.BloodPrefab);

                    var trans = new Translation
                    {
                        Value = translation.Value
                    };

                    ecb.SetComponent(instance, translation);

                }
            }).Schedule();
        sys.AddJobHandleForProducer(Dependency);

        //blood
        Entities
            .WithAll<BloodTag>()
            .ForEach((Entity e, ref Translation translation, ref PP_Movement ppMovement, in Velocity velocity) =>
            {
                // calculate blood scale and later maskOut

                // do orientation later

            }).Schedule();

        //bees
        Entities
            .WithAll<BeeTag>()
            .ForEach((ref Translation translation, ref PP_Movement ppMovement, in BeeTag beeTag, in Velocity velocity) =>
        {
            // do bee movement
            ppMovement.t += tAdd;
            translation.Value = math.lerp(ppMovement.startLocation, ppMovement.endLocation, math.smoothstep(0, 1, ppMovement.t));
        }).Schedule();

        //food, dependant on Bees.
        Entities
            .WithAll<FoodTag>()
            .ForEach((Entity e, ref Translation translation, ref PP_Movement ppMovement, in Velocity velocity) =>
            {
                // calculate bee movement
                ppMovement.t += tAdd;

                translation.Value = math.lerp(ppMovement.startLocation, ppMovement.endLocation,
                    math.smoothstep(0, 1, ppMovement.t));

                // do orientation later

            }).Schedule();
    }
}
