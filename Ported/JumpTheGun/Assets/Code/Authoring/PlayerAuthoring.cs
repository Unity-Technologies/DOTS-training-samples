
using System.Collections.Generic;

using Unity.Entities;
using Unity.Transforms;
using UnityEngine;
using UnityGameObject = UnityEngine.GameObject;

public class PlayerAuthoring : MonoBehaviour
    , IConvertGameObjectToEntity
    , IDeclareReferencedPrefabs
{
    public UnityGameObject PlayerBallPrefab;

    public void DeclareReferencedPrefabs(List<GameObject> referencedPrefabs)
    {
        referencedPrefabs.Add(PlayerBallPrefab);
    }

    public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
    {
        dstManager.AddComponent<Arc>(entity);
        dstManager.AddComponent<Translation>(entity);
        dstManager.AddComponent<Direction>(entity);
        dstManager.AddComponent<TargetPosition>(entity);
        dstManager.AddComponent<PlayerSpawnerTag>(entity);
        dstManager.AddComponentData(entity, new Player() { PlayerBallPrefab = conversionSystem.GetPrimaryEntity(PlayerBallPrefab) });
    }
}
