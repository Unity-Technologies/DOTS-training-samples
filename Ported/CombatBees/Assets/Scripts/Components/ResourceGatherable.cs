using Unity.Entities;

/// <summary>
/// A resource is gatherable when it is at the top of a stack
/// </summary>
struct ResourceGatherable : IComponentData, IEnableableComponent
{
}
