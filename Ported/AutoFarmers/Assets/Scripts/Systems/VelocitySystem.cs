using Unity.Entities;
using Unity.Mathematics;

[UpdateInGroup(typeof(SimulationSystemGroup))]
public class VelocitySystem : SystemBase
{
    protected override void OnUpdate()
    {
        float delta = UnityEngine.Time.deltaTime;
        
        Entities
            .ForEach((Entity entity, ref Velocity velocity, in Target target) =>
            {
                //velocity.Value = GetVelocity();
            }).ScheduleParallel();
    }

    private float2 GetVelocity()
    {
        throw new System.NotImplementedException();
    }
}