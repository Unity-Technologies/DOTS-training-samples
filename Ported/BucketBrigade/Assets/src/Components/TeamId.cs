using System;
using Unity.Entities;

namespace src.Components
{
    /// <summary>
    ///     
    /// </summary>
    public struct TeamId : IComponentData
    {
        // NW: Consider ushort.
        public int Id;
    }
}