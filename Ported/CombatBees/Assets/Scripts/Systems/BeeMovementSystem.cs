using Unity.Entities;
using Unity.Collections;
using Unity.Transforms;
using Unity.Mathematics;


public partial class BeeMovementSystem : SystemBase
{
    protected override void OnUpdate()
    {
        TargetCandidates targets = GetSingleton<TargetCandidates>();

        var transLookup = GetComponentDataFromEntity<Translation>(true);

        Entities
            .WithReadOnly(transLookup)
            .WithAll<Team>()
            .ForEach((Entity beeEntity, ref Target target) =>
            {
                if (target.Value == Entity.Null)
                {
                    target.Value = targets.Food;
                }

                target.TargetPosition = transLookup[target.Value].Value;

            }).ScheduleParallel();
        var dtTime = Time.DeltaTime;
        Entities
            .WithAll<Team>()
            .ForEach((Entity beeEntity,ref Translation translation, in Target target) =>
            {

                translation.Value += (math.normalize(target.TargetPosition -translation.Value) * 5.0f * dtTime);

            }).ScheduleParallel();

    }


}
