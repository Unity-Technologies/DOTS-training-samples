using System.Collections.Generic;
using Unity.Collections;
using Unity.Entities;
using UnityEngine;

public class StationAuthoring : MonoBehaviour
{
    public List<GameObject> Platforms;

    class Baker : Baker<StationAuthoring>
    {
        public override void Bake(StationAuthoring authoring)
        {
            var stationPlatforms = AddBuffer<StationPlatform>();
            foreach (var platform in authoring.Platforms)
            {
                stationPlatforms.Add(new StationPlatform()
                {
                    Platform = GetEntity(platform)
                });
            }

            AddComponent(new Station());
        }
    }
}

struct Station : IComponentData
{
    public NativeArray<Entity> Platforms;
}

[InternalBufferCapacity(5)]
public struct StationPlatform : IBufferElementData
{
    public Entity Platform;
}
