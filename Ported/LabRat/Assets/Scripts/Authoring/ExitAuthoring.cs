using Unity.Entities;
using UnityEngine;

public class ExitAuthoring : MonoBehaviour, IConvertGameObjectToEntity
{
    public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
    {
        dstManager.AddComponent<Tile>(entity);
        
    }
}