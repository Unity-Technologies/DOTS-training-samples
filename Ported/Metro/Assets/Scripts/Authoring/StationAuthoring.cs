using System.Collections.Generic;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

public class StationAuthoring : MonoBehaviour
{
    public List<GameObject> Platforms;
    public List<float4> PlatformColors;

    class Baker : Baker<StationAuthoring>
    {
        public override void Bake(StationAuthoring authoring)
        {
            var stationPlatforms = AddBuffer<StationPlatform>();
            var platformColors = new NativeArray<float4>(authoring.PlatformColors.Count, Allocator.Persistent);
            platformColors.CopyFrom(authoring.PlatformColors.ToArray());

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

            });
        }
    }
}

struct Station : IComponentData
{

}

[InternalBufferCapacity(5)]
public struct StationPlatform : IBufferElementData
{
    public Entity Platform;
}
