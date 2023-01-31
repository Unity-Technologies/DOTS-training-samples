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
<<<<<<< HEAD
    public NativeArray<Entity> Platforms;
=======
}

[InternalBufferCapacity(5)]
public struct StationPlatform : IBufferElementData
{
    public Entity Platform;
>>>>>>> f2daeccabf380f6fa31566ab03d39fa6173e3100
}
