using Unity.Entities;
using Unity.Mathematics;
using Unity.Rendering;

public class ColorSystem : SystemBase
{
    private static readonly float4 NormalColor = new float4(0.5f, 0.5f, 0.5f, 1f);

    private static readonly float4 BlockedColor = new float4(1, 0, 0, 1f);

    private static readonly float4 OvertakeColor = new float4(0,1, 0, 1f);

    protected override void OnUpdate()
    {
        Entities
            .WithNone<OvertakeTag, BlockSpeed>()
            .WithAll<Speed>()
            .ForEach((ref URPMaterialPropertyBaseColor baseColor) =>
        {
            baseColor.Value = NormalColor;
        }).ScheduleParallel();

        Entities
            .WithAll<BlockSpeed>()
            .ForEach((ref URPMaterialPropertyBaseColor baseColor) =>
        {
            baseColor.Value = BlockedColor;
        }).ScheduleParallel();

        Entities
            .WithAll<OvertakeTag>()
            .ForEach((ref URPMaterialPropertyBaseColor baseColor) =>
        {
            baseColor.Value = OvertakeColor;
        }).ScheduleParallel();
    }
}
