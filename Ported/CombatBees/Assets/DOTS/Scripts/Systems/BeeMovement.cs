using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;
using UnityVector3 = UnityEngine.Vector3;

public class BeeMovement : SystemBase
{
    protected override void OnUpdate()
    {
        var deltaTime = Time.DeltaTime;
        var cdfe = GetComponentDataFromEntity<Translation>(true);

        // TODO when a bee dies it's target must be removed
        // Move bees that are targetting (a Resource or Base) towards the target's position
        Entities
            .WithAll<IsBee>()
            .ForEach((ref Translation translation, in Target target, in Speed speed) => {
                var targetPosition = cdfe[target.Value].Value;

                // move toward target Resource
                var move = math.normalize(targetPosition - translation.Value) * speed.Value * deltaTime;
                translation.Value += move;
        }).Schedule();

        // Move bees that are not targetting a Resource in a random direction bounded by the Arena
        var arena = GetSingletonEntity<IsArena>();
        var arenaAABB = EntityManager.GetComponentData<Bounds>(arena).Value;
        
        var random = new Random(1234);

        Entities
            .WithAll<IsBee>()
            .WithNone<Target, IsDead, IsAttacking>()
            .ForEach((ref Translation translation, in Speed speed) => {
                var targetPosition = Utils.BoundedRandomPosition(arenaAABB, random);

                // move toward target Resource
                var move = math.normalize(targetPosition - translation.Value) * speed.Value * deltaTime;
                translation.Value += move;
        }).Schedule();
    }
}
