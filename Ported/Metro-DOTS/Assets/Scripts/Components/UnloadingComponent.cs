using Unity.Entities;

public struct UnloadingComponent : IComponentData, IEnableableComponent
{
    public float duration; // how long has it been since unloading started, in ms
}
