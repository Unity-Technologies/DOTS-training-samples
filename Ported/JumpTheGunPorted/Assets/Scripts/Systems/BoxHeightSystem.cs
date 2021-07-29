using Unity.Entities;
using Unity.Mathematics;
using Unity.Rendering;
using Unity.Transforms;

[UpdateAfter(typeof(CannonballBoxCollisionSystem))]
public class BoxHeightSystem : SystemBase
{
    protected override void OnCreate()
    {
        // We assume boxes are the only thing with non-uniform scales on the scene
        RequireForUpdate(GetEntityQuery(typeof(NonUniformScale)));
        
        RequireSingletonForUpdate<GameObjectRefs>();
        RequireSingletonForUpdate<HeightBufferElement>();
    }
    
    protected override void OnUpdate()
    {
        var boxMapEntity = GetSingletonEntity<HeightBufferElement>();
        var heightMap = GetBuffer<HeightBufferElement>(boxMapEntity);
        var config = this.GetSingleton<GameObjectRefs>().Config.Data;
        
        Entities
            .WithName("box_height_update")
            .WithReadOnly(heightMap)
            .ForEach((Entity box, int entityInQueryIndex, ref NonUniformScale scale, ref URPMaterialPropertyBaseColor color, ref Translation translation) =>
            {
                var x = (int) translation.Value.x;
                var y = (int) translation.Value.z;
                var height = heightMap[y * config.TerrainLength + x].Value;
                
                scale.Value = new float3(1, height, 1);
                color.Value = GetColorForHeight(height, config.MinTerrainHeight, config.MaxTerrainHeight);
                translation.Value = new float3(translation.Value.x, height / 2, translation.Value.z);
            }).ScheduleParallel();
    }
    
    /// <summary>
    /// Helper function to calculate terrain color to use for a given height
    /// TODO: how/where do we put to share between spawner and ball collision system when it recalculates color based on new height
    /// 
    /// </summary>
    /// <param name="height"></param>
    /// <param name="minTerrainHeight"></param>
    /// <param name="maxTerrainHeight"></param>
    /// <returns></returns>
    private static float4 GetColorForHeight(float height, float minTerrainHeight, float maxTerrainHeight)
    {
        float4 color;

        // change color based on height
        if (math.abs(maxTerrainHeight - minTerrainHeight) < math.EPSILON) // special case, if max is close to min just color as min height
        {
            color = Box.MIN_HEIGHT_COLOR;
        }
        else
        {
            color = math.lerp(Box.MIN_HEIGHT_COLOR, Box.MAX_HEIGHT_COLOR, (height - minTerrainHeight) / (maxTerrainHeight - minTerrainHeight));
        }

        return color;
    }
}
