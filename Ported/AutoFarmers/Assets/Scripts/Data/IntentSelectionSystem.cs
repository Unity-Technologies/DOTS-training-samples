using Unity.Collections;
using Unity.Entities;

[UpdateInGroup(typeof(SimulationSystemGroup))]
public class IntentSelectionSystem : SystemBase
{
    private float _deltaTime;
    
    private EntityCommandBuffer _entityCommandBuffer;

    protected override void OnCreate()
    {
        _entityCommandBuffer = new EntityCommandBuffer(Allocator.Temp);
    }

    protected override void OnUpdate()
    {
        _deltaTime = UnityEngine.Time.deltaTime;
        
        Entities
            .ForEach((Entity entity, ref Position position, in Velocity velocity) =>
            {
                position.Value += velocity.Value * _deltaTime;
            }).ScheduleParallel();

        _entityCommandBuffer.Playback(EntityManager);
    }
}