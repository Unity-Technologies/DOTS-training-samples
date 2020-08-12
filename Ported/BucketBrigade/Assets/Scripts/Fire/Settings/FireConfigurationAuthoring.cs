using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

public struct FireConfiguration : IComponentData
{
    public float CellSize;
    public int GridWidth;
    public int GridHeight;

    public Entity Prefab;
}

public class FireConfigurationAuthoring : UnityEngine.MonoBehaviour, IConvertGameObjectToEntity
{
    public float CellSize = 0.3f;
    public int GridWidth = 50;
    public int GridHeight = 50;
    
    public GameObject PrefabGO;
    
    public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
    {
        var prefabEntity = conversionSystem.GetPrimaryEntity(PrefabGO);
        
        dstManager.AddComponentData(entity, new FireConfiguration
        {
            CellSize = CellSize,
            GridWidth = GridWidth,
            GridHeight = GridHeight,
            Prefab = prefabEntity
        });
    }
    
    public void DeclareReferencedPrefabs(List<GameObject> gameObjects)
    {
        gameObjects.Add(PrefabGO);
    }
}
