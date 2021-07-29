using System;
using Unity.Entities;

namespace src.Components
{
    /// <summary>
    ///    These workers sole job is to fill up their teams empty buckets, only at the water source though. 
    /// </summary>
    [GenerateAuthoringComponent]
    public struct BucketFillersTag : IComponentData
    {
    }
}
