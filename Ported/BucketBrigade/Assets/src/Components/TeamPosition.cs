using System;
using Unity.Entities;

namespace src.Components
{
    /// <summary>
    ///     Index of a workers position within a <see cref="TeamId.Id"/>.
    /// </summary>
    [GenerateAuthoringComponent]
    public struct TeamPosition : IComponentData
    {
        // NW: Consider ushort.
        public int Index;
    }
}