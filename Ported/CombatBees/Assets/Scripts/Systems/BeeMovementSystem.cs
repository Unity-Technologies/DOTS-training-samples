using Unity.Entities;
using Unity.Collections;
using Unity.Transforms;
using Unity.Mathematics;

public partial class BeeMovementSystem : SystemBase
{
    protected override void OnUpdate()
    {
        var transLookup = GetComponentDataFromEntity<Translation>(true);

        // Update our target position
        Entities
            .WithReadOnly(transLookup)
            .WithAny<TeamRed, TeamBlue>()
            .ForEach((Entity beeEntity, ref Target target) =>
            {
                if (target.Value != Entity.Null)
                {
                    target.TargetPosition = transLookup[target.Value].Value;
                }
            }).ScheduleParallel();
        
        var dtTime = Time.DeltaTime;
        
        // Movement Update
        Entities
            .WithAny<TeamRed, TeamBlue>()
            .ForEach((Entity beeEntity, ref Translation translation, in Target target) =>
            {
                translation.Value += (math.normalize(target.TargetPosition -translation.Value) * 5.0f * dtTime);
            }).ScheduleParallel();
    }


}
