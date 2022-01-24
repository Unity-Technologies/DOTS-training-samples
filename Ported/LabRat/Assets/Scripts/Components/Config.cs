

using Unity.Entities;

[GenerateAuthoringComponent]
public struct Config : IComponentData
{
    public float MouseMovementSpeed;
    public float CatMovementSpeed;
}
