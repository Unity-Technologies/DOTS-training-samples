using System.Collections.Generic;
using Unity.Entities;
using UnityDisallowMultipleComponent = UnityEngine.DisallowMultipleComponent;
using UnityMonoBehaviour = UnityEngine.MonoBehaviour;
using UnityGameObject = UnityEngine.GameObject;
using Random = Unity.Mathematics.Random;

[UnityDisallowMultipleComponent]
public class RandomVelocitySpawnerAuthoring : UnityMonoBehaviour, IConvertGameObjectToEntity, IDeclareReferencedPrefabs
{
    public UnityGameObject SpawnedPrefab;
    public int SpawnedCount = 10;
    public float MaxInitVelocity = 10f;
    
    public void DeclareReferencedPrefabs(List<UnityGameObject> referencedPrefabs)
    {
        referencedPrefabs.Add(SpawnedPrefab);
    }
    
    public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
    {
        dstManager.AddComponentData(entity, new RandomVelocitySpawnerData
        {
            PrefabToSpawn = conversionSystem.GetPrimaryEntity(SpawnedPrefab),
            Random = new Random((uint) (entity.Index + 1)), // +1 because seed can't be 0,
            SpawnedCount = SpawnedCount,
            Position = transform.position,
            MaxInitVelocity = MaxInitVelocity
        });
    }
}
