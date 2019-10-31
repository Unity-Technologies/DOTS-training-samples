using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;

public class MoveTowardsSystem : JobComponentSystem
{
    protected override JobHandle OnUpdate(JobHandle inputDependencies)
    {
        var translationContainer = GetComponentDataFromEntity<Translation>(true);
        return Entities.WithReadOnly(translationContainer).ForEach(
            (Entity entity, ref Velocity velocity, ref TargetVelocity targetVelocity, in TargetEntity targetEntity) =>
            {
                if (!translationContainer.Exists(targetEntity.Value)) return;

                var translation = translationContainer[entity];
                var targetTranslation = translationContainer[targetEntity.Value];
                var directionTowards = math.normalize(targetTranslation.Value - translation.Value);
                var desiredVelocity = directionTowards * targetVelocity.Value /* math.max(0.25f,1+noise.cnoise(velocity.Value))*/; //TODO: discuss with the team
                velocity.Value = math.lerp(velocity.Value, desiredVelocity, 0.5f);
            })
            .Schedule(inputDependencies);
    }
}