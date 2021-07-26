using System;
using Unity.Entities;

namespace src.Components
{
    /// <summary>
    ///     
    /// </summary>
    public struct TeamPosition : IComponentData
    {
        // NW: Consider ushort.
        public int Id;
    }
}