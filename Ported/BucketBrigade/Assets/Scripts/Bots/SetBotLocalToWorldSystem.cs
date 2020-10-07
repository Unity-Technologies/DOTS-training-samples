using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

[UpdateInGroup(typeof(TransformSystemGroup))]
[UpdateBefore(typeof(EndFrameParentSystem))]
public class SetBotLocalToWorldSystem : SystemBase
{
    protected override void OnUpdate()
    {
        Entities
            .WithName("SetBotLtWJob")
            .ForEach((ref LocalToWorld ltw, in Pos pos) =>
        {
            ltw.Value = new float4x4(
                1f, 0f, 0f, pos.Value.x,
                0f, 1f, 0f, 0f,
                0f, 0f, 1f, pos.Value.y,
                0f, 0f, 0f, 1f);
        }).ScheduleParallel();
    }
}