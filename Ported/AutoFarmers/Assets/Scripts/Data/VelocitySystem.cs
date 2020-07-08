using Unity.Entities;

[UpdateInGroup(typeof(SimulationSystemGroup))]
public class VelocitySystem : SystemBase
{
    private float _deltaTime;
    
    protected override void OnUpdate()
    {
        _deltaTime = UnityEngine.Time.deltaTime;
        
        Entities
            .ForEach((Entity entity, ref Position position, in Velocity velocity) =>
            {
                position.Value += velocity.Value * _deltaTime;
            }).ScheduleParallel();
    }
}