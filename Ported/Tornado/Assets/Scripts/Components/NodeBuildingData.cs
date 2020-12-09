using Unity.Entities;
using Unity.Transforms;

public struct NodeBuildingData : IBufferElementData
{
    public Entity nodeEntity;
    public Node node;
    public Translation translation;
}
