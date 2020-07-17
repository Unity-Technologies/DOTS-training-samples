using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;

[UpdateInGroup(typeof(LateSimulationSystemGroup))]
public class MoveResource : SystemBase
{
    protected override void OnUpdate()
    {
        var deltaTime = Time.DeltaTime;
        var carryStiffness = ResourceManager.Instance.carryStiffness;

        // Entities
        //     .WithAll<ResourceEntity, Carried>()
        //     .ForEach((ref Translation translation, ref Velocity velocity, in Carried carried) =>
        //     {
        //         var targetPos = carried.HolderPos;
        //         translation.Value = math.lerp(translation.Value, targetPos, carryStiffness * deltaTime);
        //         velocity.Value = carried.HolderVel;
        //     }).ScheduleParallel();

        var carrierTranslationLookup = GetComponentDataFromEntity<Translation>(true);
        var carrierVelocityLookup = GetComponentDataFromEntity<Velocity>(true);

        Entities
            .WithReadOnly(carrierTranslationLookup)
            .WithReadOnly(carrierVelocityLookup)
            .WithNativeDisableContainerSafetyRestriction(carrierTranslationLookup)
            .WithNativeDisableContainerSafetyRestriction(carrierVelocityLookup)
            .ForEach((ref Translation translation, ref Velocity velocity, in Carried carried) =>
            {
                var carrierPos = carrierTranslationLookup[carried.Value];
                var carrierVel = carrierVelocityLookup[carried.Value];
                translation.Value = math.lerp(translation.Value, carrierPos.Value, carryStiffness * deltaTime);
                velocity.Value = carrierVel.Value;
            }).Schedule();
    }
}
