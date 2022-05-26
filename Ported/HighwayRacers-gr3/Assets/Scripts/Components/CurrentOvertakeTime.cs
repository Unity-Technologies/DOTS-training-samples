using Unity.Entities;

[GenerateAuthoringComponent]
struct CurrentOvertakeTime : IComponentData
{
    public float Value;
}
