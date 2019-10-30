using System;

namespace Unity.Entities
{
    /// <summary>
    /// AlwaysSynchronizeSystem can be applied to a JobComponentSystem to force it to synchronize on all of its
    /// dependencies before every update.  This attribute should only be applied when a synchronization point is
    /// necessary every frame.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public class AlwaysSynchronizeSystemAttribute : Attribute
    {
    }
}