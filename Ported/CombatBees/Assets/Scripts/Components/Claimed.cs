using Unity.Entities;

namespace Components
{
    /// <summary>
    /// Used for easier query filtering of claimed resources
    /// </summary>
    public struct Claimed : IComponentData, IEnableableComponent
    {
    }
}