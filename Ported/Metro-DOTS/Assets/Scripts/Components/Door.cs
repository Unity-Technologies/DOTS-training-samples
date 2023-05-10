using Unity.Entities;

public struct DoorComponent : IComponentData, IEnableableComponent
{
    public float duration; // how long has it been since door close started, in ms
}