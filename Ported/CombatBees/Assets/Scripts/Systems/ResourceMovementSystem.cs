using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEditor.Build.Utilities;

[UpdateAfter(typeof(BeeMovementSystem))]
public partial class ResourceMovementSystem : SystemBase
{
    private const float RESOURCE_SIZE = 0.75f;
    private const float SNAP_STIFFNESS = 2.0f;
    private const float CARRY_STIFFNESS = 15.0f;

    protected override void OnUpdate()
    {
        var deltaTime = Time.DeltaTime;

        Entities.WithoutBurst().ForEach((ref HeldByBeeComponent heldByBee, ref Translation translation) =>
        {
            if (heldByBee.HoldingBee == default)
                return;

            float3 velocity = float3.zero;

            // TODO: It probably makes more sense to add/remove the held by bee component as it's pickedup/dropped by the bees.
            var beeStateComponent = GetComponent<BeeStateComponent>(heldByBee.HoldingBee);
            if (beeStateComponent.Value == BeeStateComponent.BeeState.Dead)
            {
                heldByBee.HoldingBee = default;
            }
            else
            {
                // TODO: Probably also makes sense for the carrying bee to update all this info into the HeldByComponent instead?
                var beePosition = GetComponent<PositionComponent>(heldByBee.HoldingBee).Value;
                var beeVelocity = GetComponent<VelocityComponent>(heldByBee.HoldingBee).Value;
                var beeSize = GetComponent<BeeBaseSizeComponent>(heldByBee.HoldingBee).Value;

                float3 targetPos = beePosition - math.up() * (RESOURCE_SIZE + beeSize) * .5f;
                translation.Value = math.lerp(translation.Value, targetPos, CARRY_STIFFNESS * deltaTime);
                velocity = beeVelocity;
            }

            // TODO: Will have to make sure this happens for the "falling" as well.
            //translation.Value += velocity * deltaTime;

        }).ScheduleParallel();
    }
}
