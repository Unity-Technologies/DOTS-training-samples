using Unity.Entities;

namespace CombatBees.Testing.BeeFlight
{
    [GenerateAuthoringComponent]
    public struct SingeltonBeeMovement : IComponentData
    {
        // Assigned to a game object in the sub-scene. All systems that shall run only in this sub-scene
        // must have "RequireSingletonForUpdate<SingeltonBeeMovement>();" in their "OnCreate()".
    }
}