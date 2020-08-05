using Unity.Entities;
 
[GenerateAuthoringComponent]
public struct Progress : IComponentData
{
    public float Value;
}
