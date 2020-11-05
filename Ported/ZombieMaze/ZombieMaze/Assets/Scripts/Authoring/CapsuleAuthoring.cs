using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

[DisallowMultipleComponent]
public class CapsuleAuthoring : MonoBehaviour, IConvertGameObjectToEntity
{
    public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
    {
        dstManager.AddComponent<CapsuleRotation>(entity);
        dstManager.AddComponentData(entity, new Position());
        dstManager.AddComponentData(entity, new Random((uint)UnityEngine.Random.Range(0, int.MaxValue)));
    }
}
