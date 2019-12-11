using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;

namespace ECS_Port.Systems
{
    public class IdleHandMovementSystem : JobComponentSystem
    {
        protected override JobHandle OnUpdate(JobHandle inputDeps)
        {
            var time = UnityEngine.Time.time + TimeConstants.Offset;
            
            return Entities.ForEach((ref Translation translation, ref IdleState idleState, in ArmComponent arm) =>
                {
                    idleState.HandTarget = translation.Value + new float3(math.sin(time) * 0.35f, 1f + math.cos(time * 1.618f) * 0.5f, 1.5f);
                }).Schedule(inputDeps);
        }
    }
}