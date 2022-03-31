using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Rendering;
using Unity.Transforms;
using Color = UnityEngine.Color;


public partial class BrickUpdateSystem : SystemBase
{
    protected override void OnUpdate()
    {
        var entityElement = GetSingletonEntity<HeightElement>();
        var heightBuffer = this.GetBuffer<HeightElement>(entityElement, true);
        var terrainData = this.GetSingleton<TerrainData>();
        Entities
            .WithReadOnly(heightBuffer)
            .ForEach((Entity entity, ref URPMaterialPropertyBaseColor color, ref NonUniformScale scale, ref Translation translation) =>
            {
                var box = TerrainUtility.BoxFromLocalPosition(translation.Value, terrainData.TerrainWidth, terrainData.TerrainLength);
                int index = box.x + box.y * terrainData.TerrainWidth;
                // change color based on height
                if ((terrainData.MaxTerrainHeight - Constants.HEIGHT_MIN) < 0.5f) {
                    color.Value = Constants.MIN_HEIGHT_COLOR;
                } else {
                    var brickHeight = heightBuffer[index];
                    color.Value = math.lerp(Constants.MIN_HEIGHT_COLOR, Constants.MAX_HEIGHT_COLOR, (brickHeight - Constants.HEIGHT_MIN) / (terrainData.MaxTerrainHeight - Constants.HEIGHT_MIN));

                    // Set scale
                    scale.Value = new float3(1, brickHeight, 1);
                    // Set position
                    translation.Value.y = brickHeight / 2 + Constants.Y_OFFSET;

                }

            }).ScheduleParallel();
    }

}