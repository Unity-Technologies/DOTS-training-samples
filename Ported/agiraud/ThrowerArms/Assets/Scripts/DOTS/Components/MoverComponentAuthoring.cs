using Unity.Entities;
using UnityEngine;

public class MoverComponentAuthoring : MonoBehaviour, IConvertGameObjectToEntity
{
    public Vector3 velocity;

    public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
    {
        dstManager.AddComponentData(entity, new Mover { velocity = velocity });
    }
}
