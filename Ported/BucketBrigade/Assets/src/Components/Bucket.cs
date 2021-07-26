using System;
using Unity.Entities;

namespace src.Components
{
    /// <summary>
    ///     
    /// </summary>
    [GenerateAuthoringComponent]
    public struct Bucket : IComponentData
    {
        public float WaterLevel;
    }
}