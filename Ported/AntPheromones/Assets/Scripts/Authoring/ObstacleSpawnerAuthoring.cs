using System.Collections.Generic;
using Unity.Entities;
using UnityGameObject = UnityEngine.GameObject;
using UnityRangeAttribute = UnityEngine.RangeAttribute;
using UnityMonoBehaviour = UnityEngine.MonoBehaviour;

public class ObstacleSpawnerAuthoring : UnityMonoBehaviour
    , IConvertGameObjectToEntity
    , IDeclareReferencedPrefabs
{
    public UnityGameObject ObstaclePrefab;
    [UnityRange(0, 5)] public int obstacleRingCount;
    [UnityRange(0, 1)] public float ObstaclesPerRing;
    [UnityRange(.1f, 1)] public float ObstacleRadius;

    // This function is required by IDeclareReferencedPrefabs
    public void DeclareReferencedPrefabs(List<UnityGameObject> referencedPrefabs)
    {
        // Conversion only converts the GameObjects in the scene.
        // This function allows us to inject extra GameObjects,
        // in this case prefabs that live in the assets folder.
        referencedPrefabs.Add(ObstaclePrefab);
    }

    public void Convert(Entity entity, EntityManager dstManager
        , GameObjectConversionSystem conversionSystem)
    {
        dstManager.AddComponentData(entity, new ObstacleSpawner
        {
            ObstaclePrefab = conversionSystem.GetPrimaryEntity(ObstaclePrefab),
            obstacleRingCount = obstacleRingCount,
            ObstaclesPerRing = ObstaclesPerRing,
            ObstacleRadius = ObstacleRadius
        });
    }
}

