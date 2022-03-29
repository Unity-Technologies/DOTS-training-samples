using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Rendering;
using Unity.Transforms;


public partial class BrickColorUpdateSystem : SystemBase
{

    protected override void OnUpdate()
    {
        /*
        Entities
            .ForEach((Entity entity, in Brick brick) =>
            {
                // change color based on height
                var terrainData = this.GetSingleton<TerrainData>();
                if ((terrainData.maxTerrainHeight - HEIGHT_MIN) < 0.5f) {
                    brick.color = MIN_HEIGHT_COLOR;
                } else {
                    brick.color = Color.Lerp(MIN_HEIGHT_COLOR, MAX_HEIGHT_COLOR, (brick.height - HEIGHT_MIN) / (Game.instance.maxTerrainHeight - HEIGHT_MIN));
                }
            }).ScheduleParallel();
*/
    }

}