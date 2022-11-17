using Unity.Entities;

namespace Components
{
    /// <summary>
    /// Only used by StackingSystem for easier query filtering
    /// </summary>
    public struct StackNeedsFix : IComponentData, IEnableableComponent
    {
    }
}