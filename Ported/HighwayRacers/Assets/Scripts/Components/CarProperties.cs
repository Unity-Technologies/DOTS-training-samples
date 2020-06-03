using Unity.Entities;

[GenerateAuthoringComponent]
public struct CarProperties : IComponentData
{
    public float DefaultSpeed;
    public float OvertakeSpeed;
    public float DistanceToCarBeforeOvertaking;
    public float OvertakeEagerness;
    public float MergeSpace;
}