using Unity.Entities;
using Unity.Mathematics;

namespace src.Components
{
    /// <summary>
    ///     Denotes 2D entity position.
    /// </summary>
    public struct Position : IComponentData
    {
        public float2 Value;
    }
}
