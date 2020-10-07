using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

public class EntityBufferElementDataAuthoring : MonoBehaviour, IConvertGameObjectToEntity, IDeclareReferencedPrefabs
{
    public List<GameObject> Items;

    public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
    {
        var buf = dstManager.AddBuffer<EntityBufferElementData>(entity);

        foreach (var item in Items)
        {
            buf.Add(new EntityBufferElementData { Value = conversionSystem.GetPrimaryEntity(item) });
        }
    }


    public void DeclareReferencedPrefabs(List<GameObject> referencedPrefabs)
    {
        referencedPrefabs.AddRange(Items);
    }
}

public struct EntityBufferElementData : IBufferElementData
{
    public Entity Value;
}