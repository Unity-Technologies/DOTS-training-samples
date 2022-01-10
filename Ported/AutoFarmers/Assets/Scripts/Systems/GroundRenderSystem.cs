using Unity.Entities;
using Unity.Rendering;
using Unity.Transforms;

[UpdateInGroup(typeof(FixedStepSimulationSystemGroup))]
public partial class GroundRenderSystem : SystemBase
{
    protected override void OnCreate()
    {
        RequireSingletonForUpdate<FarmConfig>();
    }

    protected override void OnUpdate()
    {
        var farmConfig = GetSingleton<FarmConfig>();
        var gridData = this.GetSingleton<GridData>();
        Entities
            .WithoutBurst()
            .WithAll<Ground>()
            .ForEach((ref URPMaterialPropertyBaseColor color, in Translation translation) =>
            {
                int hash = PathUtility.Hash((int)translation.Value.x, (int)translation.Value.z, farmConfig.MapSizeX);
                if(gridData.groundTiles[hash] == byte.MaxValue)
                    color.Value = farmConfig.TilledGroundColor;
            }).Run();
    }
}