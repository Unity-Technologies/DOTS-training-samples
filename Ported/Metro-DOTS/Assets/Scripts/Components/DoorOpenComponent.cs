using Unity.Entities;

public struct DoorOpenComponent : IComponentData, IEnableableComponent
{
    public float duration; // how long has it been since door open started, in ms
}
