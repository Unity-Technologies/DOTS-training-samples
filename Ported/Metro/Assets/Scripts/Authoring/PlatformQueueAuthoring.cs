using Unity.Collections;
using Unity.Entities;
using UnityEngine;

public class PlatformQueueAuthoring : MonoBehaviour
{
    
}

class PlatformQueueBaker : Baker<PlatformQueueAuthoring>
{
    public override void Bake(PlatformQueueAuthoring authoring)
    {
        AddComponent(new PlatformQueue
        {
            //Passengers = new NativeList<Entity>();
        });
        AddBuffer<PlatformQueueBuffer>();
    }
}