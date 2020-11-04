using Unity.Entities;
using Unity.Transforms;

public class RailGeneration : SystemBase
{
    protected override void OnCreate()
    {
        var metroData = GetSingleton<MetroData>();
        var ecb = new EntityCommandBuffer();
        
        Entities.ForEach((in PathRef pathdata) =>
        {
            ref var positions = ref pathdata.Data.Value.Positions;
            for (var i = 0; i < positions.Length; i++)
            {
                var railEntity = ecb.Instantiate(metroData.RailPrefab);
                var railTranslation = GetComponent<Translation>(railEntity);
                railTranslation.Value = positions[i];
            }
        }).Schedule();
    }

    protected override void OnUpdate()
    {
        // Do nothing...
    }
}
