using System;
using Unity.Entities;

namespace src.Components
{
    /// <summary>
    ///    Added to a Worker entity, denotes that it's a worker.
    /// </summary>
    [GenerateAuthoringComponent]
    public struct WorkerTag : IComponentData
    {
    }
}
