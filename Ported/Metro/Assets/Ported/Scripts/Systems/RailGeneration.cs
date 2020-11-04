using Unity.Collections;
using Unity.Entities;
using Unity.Transforms;

public class RailGeneration : SystemBase
{
    protected override void OnUpdate()
    {
        var railPrefab = GetSingleton<MetroData>().RailPrefab;
        var ecb = new EntityCommandBuffer(Allocator.Temp);

        Entities.ForEach((in PathRef pathdata) =>
        {
            ref var positions = ref pathdata.Data.Value.Positions;
            for (var i = 0; i < positions.Length; i++)
            {
                var railEntity = ecb.Instantiate(railPrefab);
                var railTranslation = new Translation {Value = positions[i]};
                ecb.SetComponent(railEntity, railTranslation);
            }
        }).Run();

        ecb.Playback(EntityManager);
        
        Enabled = false;
    }
}

namespace MetroECS
{
}