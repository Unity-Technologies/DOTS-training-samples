using System;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;
using UnityGameObject = UnityEngine.GameObject;
using UnityRangeAttribute = UnityEngine.RangeAttribute;
using UnityMonoBehaviour = UnityEngine.MonoBehaviour;

public class CityAuthoring : UnityMonoBehaviour, IConvertGameObjectToEntity, IDeclareReferencedPrefabs
{
    public UnityGameObject BarPrefab;
    [UnityRange(1, 200)] public int NumberOfClusters;
    [UnityRange(1, 40)] public int TowerMinHeight;
    [UnityRange(1, 40)] public int TowerMaxHeight;

    public UnityGameObject Floor;

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
        var localScale = Floor.transform.localScale;
        dstManager.AddComponentData(entity, new CitySpawner
        {
            BarPrefab = conversionSystem.GetPrimaryEntity(BarPrefab),
            NumberOfClusters = NumberOfClusters,
            CityWidth = localScale.x * 4f,
            CityLength = localScale.z * 4f,
            MaxTowerHeight = TowerMaxHeight,
            MinTowerHeight = TowerMinHeight,
        });
    }
}
