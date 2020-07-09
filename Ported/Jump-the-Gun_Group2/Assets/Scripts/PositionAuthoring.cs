using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

public class PositionAuthoring : UnityEngine.MonoBehaviour, IConvertGameObjectToEntity {
    public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem) {

        dstManager.AddComponentData(entity, new Position { });

        dstManager.RemoveComponent<Translation>(entity);
        dstManager.RemoveComponent<Unity.Transforms.Rotation>(entity);
        dstManager.RemoveComponent<Scale>(entity);
        dstManager.RemoveComponent<NonUniformScale>(entity);
    }
}
