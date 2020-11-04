using Unity.Collections;
using Unity.Entities;
using Unity.Transforms;

namespace MetroECS
{
    public class PlatformGeneration : SystemBase
    {
        protected override void OnUpdate()
        {
            var prefab = GetSingleton<MetroData>().PlatformPrefab;
            var ecb = new EntityCommandBuffer(Allocator.Temp);

            Entities.ForEach((in PathRef pathdata) =>
            {
                ref var positions = ref pathdata.Data.Value.Positions;
                ref var markerTypes = ref pathdata.Data.Value.MarkerTypes;
                for (var i = 1; i < positions.Length; i++)
                {
                    if (!(markerTypes[i] == (int) RailMarkerType.PLATFORM_END &&
                          markerTypes[i - 1] == (int) RailMarkerType.PLATFORM_START))
                    {
                        continue;
                    }

                    var entity = ecb.Instantiate(prefab);
                    var translation = new Translation {Value = positions[i]};
                    ecb.SetComponent(entity, translation);
                }
            }).Run();

            ecb.Playback(EntityManager);
        
            Enabled = false;
        }
    }
}