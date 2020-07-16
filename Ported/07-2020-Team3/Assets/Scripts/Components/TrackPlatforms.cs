using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;


[InternalBufferCapacity(16)]
public struct TrackPlatforms : IBufferElementData
{
    public Entity platform;
    public float location;
}
