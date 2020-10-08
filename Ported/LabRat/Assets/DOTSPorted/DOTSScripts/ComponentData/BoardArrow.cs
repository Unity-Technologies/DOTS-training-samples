
using Unity.Entities;
using Unity.Mathematics;

[GenerateAuthoringComponent]
public struct BoardArrow : IComponentData
{
    public Entity targetEntity;
    public EntityDirection direction;
    public int2 gridPosition;
    public int playerIndex;
}

[InternalBufferCapacity(3)] // Hardcode the initial capacity to the maximum number of arrows allowed per player.
public struct BoardArrowBufferElement : IBufferElementData
{
    public Entity Value;
}
