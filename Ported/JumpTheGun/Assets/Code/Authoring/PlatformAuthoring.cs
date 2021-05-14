
using Unity.Entities;
using Unity.Rendering;
using Unity.Transforms;
using UnityEngine;

public class PlatformAuthoring : MonoBehaviour
    , IConvertGameObjectToEntity
{

    public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
    {
        dstManager.AddComponent<Platform>(entity);

        dstManager.AddComponent<BoardPosition>(entity);
        dstManager.AddComponent<CurrentLevel>(entity);
        dstManager.AddComponent<WasHit>(entity);
        dstManager.AddComponent<URPMaterialPropertyBaseColor>(entity);

        dstManager.RemoveComponent<Translation>(entity);
        dstManager.RemoveComponent<Rotation>(entity);
    }
}
