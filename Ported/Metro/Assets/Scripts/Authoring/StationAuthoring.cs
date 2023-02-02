using System.Collections.Generic;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

public class StationAuthoring : MonoBehaviour
{
    public List<GameObject> Platforms;
    public List<Color> PlatformColors;

    class Baker : Baker<StationAuthoring>
    {
        public override void Bake(StationAuthoring authoring)
        {
            var stationPlatforms = AddBuffer<StationPlatform>();
            var platformColors = new NativeList<float4>(authoring.PlatformColors.Count, Allocator.Persistent);
            foreach (var c in authoring.PlatformColors)
            {
                platformColors.Add(new float4((Vector4)c));
            }

            var platforms = new NativeList<Entity>(authoring.Platforms.Count, Allocator.Persistent);
            foreach (var p in authoring.Platforms)
            {
                platforms.Add(GetEntity(p));
            }

            foreach (var platform in authoring.Platforms)
            {
                stationPlatforms.Add(new StationPlatform()
                {
                    Platform = GetEntity(platform)
                });
            }

            AddComponent(new Station()
            {
                Platforms = platforms,
                Colors = platformColors
            });
        }
    }
}

struct Station : IComponentData
{
    public NativeList<Entity> Platforms;
    public NativeList<float4> Colors;

}

[InternalBufferCapacity(5)]
public struct StationPlatform : IBufferElementData
{
    public Entity Platform;
}
