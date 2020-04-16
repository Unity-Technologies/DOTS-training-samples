using Unity.Entities;
using Unity.Mathematics;
using Unity.Rendering;
using Unity.Transforms;

public class ColorSystem : SystemBase
{
    private static readonly float4 NormalColor = new float4(0.5f, 0.5f, 0.5f, 1f);

    private static readonly float4 BlockedColor = new float4(1, 0, 0, 1f);

    private static readonly float4 OvertakeColor = new float4(0,1, 0, 1f);

    protected override void OnUpdate()
    {
        Entities
            // Get the entities that are not overtaking or blocking, but have speed; the agents that are moving normally. 
            .WithNone<OvertakeTag, BlockSpeed>()
            .WithAll<Speed>()
            .ForEach((ref MaterialColor baseColor, in Parent parent) =>
            {
                // Update the color to normal.
                baseColor.Value = NormalColor;

            }).ScheduleParallel();

        Entities
            // Get the entities that are blocking
            .WithAll<BlockSpeed>()
            .ForEach((ref MaterialColor baseColor) =>
            {
                // Update the color to blocked.
                baseColor.Value = BlockedColor;
            })
            .ScheduleParallel();

        Entities
            // Get the entities that are overtaking
            .WithAll<OvertakeTag>()
            .ForEach((ref MaterialColor baseColor) =>
            {
                // Update the color to overtake.
                baseColor.Value = OvertakeColor;
            })
            .ScheduleParallel();

        // This system iterates through all the child objects that have renderers.
        Entities
            .ForEach((Entity entity, ref URPMaterialPropertyBaseColor baseColor, in Parent parent) =>
            {
                // Get the parent entity, which should be the agent.
                Entity parentEntity = parent.Value;

                // Get the color component of the agent.
                MaterialColor carColor = EntityManager.GetComponentData<MaterialColor>(parentEntity);

                // Copy the value over so that it affects the material color.
                baseColor.Value = carColor.Value;

            })
            .WithoutBurst()
            .Run();
    }
}
