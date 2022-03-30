using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

[UpdateInGroup(typeof(PresentationSystemGroup))]
public partial class PositioningSystem : SystemBase
{
    protected override void OnUpdate()
    {
        Entities
            .ForEach((ref Translation translation, in Position position) =>
            {
                translation.Value = new float3(position.Value.x, translation.Value.y, position.Value.y);
            }).ScheduleParallel();
        
        Entities
            .ForEach((ref NonUniformScale scale, in Scale newScale) =>
            {
                scale.Value = newScale.Value;
            }).ScheduleParallel();
    }
}
