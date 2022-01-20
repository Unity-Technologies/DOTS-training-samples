using System.Collections.Generic;
using Unity.Entities;
using UnityGameObject = UnityEngine.GameObject;
using UnityRangeAttribute = UnityEngine.RangeAttribute;
using UnityMonoBehaviour = UnityEngine.MonoBehaviour;
using Random = Unity.Mathematics.Random;

public class GridSpawnerAuthoring : UnityMonoBehaviour
    , IConvertGameObjectToEntity
    , IDeclareReferencedPrefabs
{
    public UnityGameObject SpawnedPrefab;
    public int GridCellsX;
    public int GridCellsZ;
    public float CellSize;
    public int SpawnedCount;

    public void DeclareReferencedPrefabs(List<UnityGameObject> referencedPrefabs)
    {
        referencedPrefabs.Add(SpawnedPrefab);
    }

    public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
    {
        dstManager.AddComponentData(entity, new GridDimensions
        {
            CellsX = GridCellsX,
            CellsZ = GridCellsZ,
            CellSize = CellSize
        });
        
        dstManager.AddComponentData(entity, new Spawner
        {
            PrefabToSpawn = conversionSystem.GetPrimaryEntity(SpawnedPrefab),
            Random = new Random((uint) (entity.Index + 1)), // +1 because seed can't be 0,
            SpawnedCount = SpawnedCount,
        });
    }
}