using Unity.Entities;

[GenerateAuthoringComponent]
public struct DepositedPlantCount : IComponentData
{
    public float ForFarmers;
    public float ForDrones;
}