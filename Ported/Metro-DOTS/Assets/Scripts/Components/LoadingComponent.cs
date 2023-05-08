using Unity.Entities;

public struct LoadingComponent : IComponentData, IEnableableComponent
{
    public float duration; // how long has it been since loading started, in ms
}
