using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;

public partial class BeeAttackerSystem : SystemBase
{
    protected override void OnUpdate()
    {
        var dt = World.Time.DeltaTime;
        var sys = World.GetExistingSystem<EndSimulationEntityCommandBufferSystem>();
        var ecb = sys.CreateCommandBuffer();
        var lookUpTraslation = GetComponentDataFromEntity<Translation>(true);

        Entities
            .WithNativeDisableContainerSafetyRestriction(lookUpTraslation)
            .WithAll<BeeAttackMode>()
            .ForEach((Entity entity, ref Translation position, in Bee bee) =>
            {
                //var postion = lookUpTraslation[entity];
                var targetTranslation = lookUpTraslation[bee.TargetEntity];
                var distance = math.distance(targetTranslation.Value, position.Value);
                if (distance < 1)
                {
                    //TODO : Kill the bee, turn into blood
                    ecb.RemoveComponent<BeeAttackMode>(entity);
                    ecb.AddComponent<BeeIdleMode>(entity);
                }

                var speed = 2f;
                if (distance < 4)
                {
                    speed = 5f;
                }

                var vector = targetTranslation.Value - position.Value;
                position.Value += vector * speed * dt;
            }).Schedule();
        sys.AddJobHandleForProducer(Dependency);
    }
}