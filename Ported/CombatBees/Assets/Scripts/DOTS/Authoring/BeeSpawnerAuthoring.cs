using Unity.Entities;
using UnityEngine;

[GenerateAuthoringComponent]
public struct BeeSpawner : IComponentData
{
    public Entity Prefab;
    public int BeeCount;
}

//[RequiresEntityConversion]
//public class BeeSpawnerAuthoring : MonoBehaviour, IConvertGameObjectToEntity
//{

//    public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
//    {
//    }
//}