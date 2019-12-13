using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;

namespace ECS_Port.Systems
{
    [UpdateBefore(typeof(UpdateArmIKChainSystem))]
    public class IdleHandMovementSystem : JobComponentSystem
    {
        protected override JobHandle OnUpdate(JobHandle inputDeps)
        {
            var time = UnityEngine.Time.time + TimeConstants.Offset;

            var idleUpdate = Entities
               .ForEach((ref Translation translation, ref ArmComponent arm) =>
               {
                   arm.IdleHandTarget = translation.Value + new float3(math.sin(time) * 0.35f, 1f + math.cos(time * 1.618f) * 0.5f, 1.5f);
               }).Schedule(inputDeps);

            return Entities.WithAll<IdleState>()
                .ForEach((ref Translation translation, ref ArmComponent arm) =>
            {
                float grabT = arm.ReachTimer;
                grabT = 3f * grabT * grabT - 2f * grabT * grabT * grabT;
                arm.HandTarget = math.lerp(arm.IdleHandTarget, arm.HandTarget, grabT);
            }).Schedule(idleUpdate);
        }
    }
}