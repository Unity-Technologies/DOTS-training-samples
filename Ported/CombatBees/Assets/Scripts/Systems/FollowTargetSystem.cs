using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;

public partial class FollowTargetSystem : SystemBase
{
    protected override void OnUpdate()
    {
        var lookUpTranslation = GetComponentDataFromEntity<Translation>(true);

        //Move all Foods to TargetBy translation
        Entities
            .WithNativeDisableContainerSafetyRestriction(lookUpTranslation)
            .WithAll<Food>()
            .WithAll<IsCarried>()
            .WithNone<Ballistic>()
            .ForEach((Entity entity, ref Translation position, ref TargetedBy targetedBy) => 
            {
                var beeLocation = lookUpTranslation[targetedBy.Value];
                beeLocation.Value.y -= 1.5f; // Add an offset so we can see the bee carrying the food instead of the food being in the bee
                position.Value = beeLocation.Value;

            }).Schedule();
    }
}
