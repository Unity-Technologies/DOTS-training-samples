using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

public class PositionSystem : SystemBase
{
    protected override void OnUpdate()
    {
        Entities
            .ForEach((ref Translation translation, in Position position) =>
            {
                translation.Value = new float3(position.Value.x, 0, position.Value.y);
            }).ScheduleParallel();
    }
}
