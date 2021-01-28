using Unity.Entities; 

[GenerateAuthoringComponent]
public struct BeeWithInvalidtarget : IComponentData
{
    public int teamId;
}