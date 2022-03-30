using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Rendering;
using Unity.Transforms;
using Color = UnityEngine.Color;


public partial class BrickColorUpdateSystem : SystemBase
{
    protected override void OnUpdate()
    {
        var terrainData = this.GetSingleton<TerrainData>();
        Entities
            .ForEach((Entity entity, ref Brick brick, ref URPMaterialPropertyBaseColor color) =>
            {
                // change color based on height
                if ((terrainData.MaxTerrainHeight - Constants.HEIGHT_MIN) < 0.5f) {
                    color.Value = Constants.MIN_HEIGHT_COLOR;
                } else {
                    color.Value = math.lerp(Constants.MIN_HEIGHT_COLOR, Constants.MAX_HEIGHT_COLOR, (brick.height - Constants.HEIGHT_MIN) / (terrainData.MaxTerrainHeight - Constants.HEIGHT_MIN));
                }

            }).ScheduleParallel();
    }

}