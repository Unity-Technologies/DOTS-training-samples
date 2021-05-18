using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;
using UnityVector3 = UnityEngine.Vector3;

[UpdateAfter(typeof(BeePerception))]
public class BeeMovement : SystemBase
{
    [ReadOnly]
    private ComponentDataFromEntity<Translation> cdfe;

    protected override void OnCreate()
    {
    }

    protected override void OnUpdate()
    {
        cdfe = GetComponentDataFromEntity<Translation>(true);
        var random = new Random(1234);
        var deltaTime = Time.DeltaTime;
        
        // Update all TargetPositions with current position of Target (deterministic!)
        Entities
            .WithoutBurst()
            .ForEach((ref TargetPosition targetPosition, in Target target) => {
                targetPosition.Value = cdfe[target.Value].Value;
        }).Run();


        // TODO when a bee dies it's target must be removed
        // Move bees that are targetting (a Resource or Base) towards the target's position
        Entities
            .WithAll<IsBee>()
            .ForEach((ref Translation translation, in TargetPosition targetPosition, in Speed speed) =>
            {
                // move toward target Resource
                var move = math.normalize(targetPosition.Value - translation.Value) * speed.Value * deltaTime;
                translation.Value += move;
            }).Schedule();

        // Move bees that are not targetting a Resource in a random direction bounded by the Arena
        var arena = GetSingletonEntity<IsArena>();
        var arenaAABB = EntityManager.GetComponentData<Bounds>(arena).Value;

        Entities
            .WithAll<IsBee>()
            .WithNone<Target, IsDead, IsAttacking>()
            .ForEach((ref Translation translation, in Speed speed) =>
            {
                var targetPosition = Utils.BoundedRandomPosition(arenaAABB, ref random);

                // move toward target Resource
                var move = math.normalize(targetPosition - translation.Value) * speed.Value * deltaTime;
                translation.Value += move;
            }).Schedule();
    }
}
