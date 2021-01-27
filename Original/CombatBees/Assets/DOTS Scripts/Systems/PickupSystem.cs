using Unity.Entities;
using Unity.Transforms;
using Unity.Mathematics;

[UpdateAfter(typeof(BeeMovementSystem))]
public class PickupSystem : SystemBase
{
    protected override void OnUpdate()
    {
        var ecb = new EntityCommandBuffer(Unity.Collections.Allocator.Temp);

        // Entities.ForEach is a job generator, the lambda it contains will be turned
        // into a proper IJob by IL post processing.
        Entities
            .WithName("Pickup")
            .WithAll<BeeTag>()
            .WithNone<CarriedFood>()
            .ForEach((Entity e, ref Translation translation, ref TargetPosition t, in MoveTarget moveTarget) =>
            {
                if (MathUtil.IsWithinDistance(1.0f, t.Value, translation.Value))
                {
                    ecb.AddComponent( e, new CarriedFood() {Value = moveTarget.Value});
                    ecb.AddComponent( moveTarget.Value, new CarrierBee() {Value = e});

                    ecb.RemoveComponent<MoveTarget>( e);
                    t.Value = float3.zero;
                }
                
            }).Run();

        ecb.Playback(EntityManager);
        ecb.Dispose();
    }
}