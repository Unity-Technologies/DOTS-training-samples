using Unity.Entities;

public struct NewNodeSetup : IComponentData
{
    public Entity buildingEntity;
    public int constraintIndex;
    public bool isPointA;
}
