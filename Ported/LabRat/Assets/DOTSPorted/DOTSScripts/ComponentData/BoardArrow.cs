
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

[GenerateAuthoringComponent]
public struct BoardArrow : IComponentData
{
    public static void Shrink(EntityManager entityManager, Entity arrowEntity)
    {
        var scale = entityManager.GetComponentData<NonUniformScale>(arrowEntity);

        entityManager.SetComponentData(arrowEntity, new NonUniformScale()
        {
            Value = new float3(scale.Value.x * 0.5f, scale.Value.y * 0.5f, scale.Value.z * 0.5f),
        });
    }

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
