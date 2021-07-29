using System;
using Unity.Entities;

namespace src.Components
{
    /// <summary>
    ///     Stores the current tick value to allow deterministic querying.
    /// </summary>
    [GenerateAuthoringComponent]
    public struct EcsTick : IComponentData
    {
        public int CurrentTick;
    }
}
