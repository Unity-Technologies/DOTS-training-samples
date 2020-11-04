using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

[DisallowMultipleComponent]
public class PlayerAuthoring : MonoBehaviour, IConvertGameObjectToEntity
{
    public float playerSpeed = 1;

    public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
    {
#if UNITY_EDITOR
        dstManager.SetName(entity, "Player");
#endif
        dstManager.AddComponent<PlayerTag>(entity);
        dstManager.AddComponentData(entity, new Position());
        dstManager.AddComponentData(entity, new Direction());
        dstManager.AddComponentData(entity, new Speed(playerSpeed));

    }
}
