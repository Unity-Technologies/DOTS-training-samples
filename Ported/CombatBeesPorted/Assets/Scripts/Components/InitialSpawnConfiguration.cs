using Unity.Entities;

[GenerateAuthoringComponent]
public struct InitialSpawnConfiguration: IComponentData
{
    public int BeeCount;
    public int FoodCount;
}