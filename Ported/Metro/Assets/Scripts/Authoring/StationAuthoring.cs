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
            AddComponent(new Station()
            {
                Platforms = authoring.Platforms.ConvertAll(x => GetEntity(x)).ToNativeList(Allocator.Persistent)
            });
        }
    }
}

struct Station : IComponentData
{
    public NativeArray<Entity> Platforms;
}
