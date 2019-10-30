using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

[RequiresEntityConversion]
public class SpawnerAuthoring_FromEntity : MonoBehaviour, IDeclareReferencedPrefabs, IConvertGameObjectToEntity
{
    public GameObject PrefabCat;
    public GameObject PrefabMouse;

    public void DeclareReferencedPrefabs(List<GameObject> referencedPrefabs)
    {
        referencedPrefabs.Add(PrefabCat);
        referencedPrefabs.Add(PrefabMouse);
    }

    public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
    {
        var spawnerData = new Spawner_FromEntity
        {
            PrefabCat = conversionSystem.GetPrimaryEntity(PrefabCat),
            PrefabMouse = conversionSystem.GetPrimaryEntity(PrefabMouse)
        };
        dstManager.AddComponentData(entity, spawnerData);
    }
}
