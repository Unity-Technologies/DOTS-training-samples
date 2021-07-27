using System;
using Unity.Collections;
using Unity.Entities;

namespace src.Components
{
    /// <summary>
    ///     
    /// </summary>
    [GenerateAuthoringComponent]
    public struct TeamId : IComponentData
    {
        // NW: Consider ushort.
        public int Id;
    }
}