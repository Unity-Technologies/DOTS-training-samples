
using System.Collections.Generic;

using Unity.Entities;
using UnityEngine;

public class TankAuthoring : MonoBehaviour
    , IConvertGameObjectToEntity
    , IDeclareReferencedPrefabs
{
    public GameObject Prefab;

    public float ReloadTime;

    public void DeclareReferencedPrefabs(List<GameObject> referencedPrefabs)
    {
        referencedPrefabs.Add(Prefab);
    }

    public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
    {
        dstManager.AddComponentData(entity, new Tank
        {
            Prefab = conversionSystem.GetPrimaryEntity(Prefab),
        });

        dstManager.AddComponent<Position>(entity);
        dstManager.AddComponent<Direction>(entity);
        dstManager.AddComponent<BoardPosition>(entity);
        dstManager.AddComponent<TimeOffset>(entity);
        dstManager.AddComponent<TargetPosition>(entity);
    }
}
