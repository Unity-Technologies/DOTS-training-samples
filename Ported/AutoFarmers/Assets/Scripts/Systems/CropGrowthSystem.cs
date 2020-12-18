using Unity.Collections;
using Unity.Entities;
using Unity.Transforms;
using Unity.Mathematics;

public class CropGrowthSystem : SystemBase
{
    protected override void OnUpdate()
    {
        var ecb = new EntityCommandBuffer(Allocator.Temp);
        var deltaTime = Time.DeltaTime;
        var data = GetSingletonEntity<CommonData>();
        var tileBuffer = GetBufferFromEntity<TileState>()[data];
        var settings = GetSingleton<CommonSettings>();

        Entities
            .ForEach((Entity entity, ref SeededTile tile, in Translation translation) =>
            {
                var farmerPosition = new int2((int)math.floor(translation.Value.x), (int)math.floor(translation.Value.z));
                var farmerLinearIndex = farmerPosition.x + farmerPosition.y * settings.GridSize.x;
                tileBuffer[farmerLinearIndex] = new TileState { Value = ETileState.Grown };

                var instance = ecb.Instantiate(settings.PlantPrefab);
                ecb.SetComponent(instance, new Translation { Value = new float3(farmerPosition.x , 0 , farmerPosition.y) });
            }).Run();

            ecb.Playback(EntityManager);
    }
}
