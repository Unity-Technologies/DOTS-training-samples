using System;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Mathematics;
using UnityGameObject = UnityEngine.GameObject;
using UnityRangeAttribute = UnityEngine.RangeAttribute;
using UnityMonoBehaviour = UnityEngine.MonoBehaviour;

public class CityAuthoring : UnityMonoBehaviour, IConvertGameObjectToEntity, IDeclareReferencedPrefabs
{
    public UnityGameObject BarPrefab;
    [UnityRange(1, 200)] public int NumberOfClusters;
    [UnityRange(1, 40)] public int TowerMinHeight;
    [UnityRange(1, 40)] public int TowerMaxHeight;
    
    // TODO: use plane instead
    [UnityRange(20, 5000)] public int CityWidth;
    [UnityRange(20, 5000)] public int CityLength;

    void OnValidate()
    {
        TowerMaxHeight = math.max(TowerMaxHeight, TowerMinHeight);
    }

    public void DeclareReferencedPrefabs(List<UnityGameObject> referencedPrefabs)
    {
        referencedPrefabs.Add(BarPrefab);
    }

    public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
    {
        dstManager.AddComponentData(entity, new CitySpawner
        {
            BarPrefab = conversionSystem.GetPrimaryEntity(BarPrefab),
            NumberOfClusters = NumberOfClusters,
            CityWidth = CityWidth,
            CityLength = CityLength,
            MaxTowerHeight = TowerMaxHeight,
            MinTowerHeight = TowerMinHeight,
        });
    }
}
