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
    [Tooltip("One cluster has 1-5 towers in them")]
    public int NumberOfClusters;
    [UnityRange(1, 40)] public int TowerMinHeight;
    [UnityRange(1, 40)] public int TowerMaxHeight;

    [Tooltip("Defines the floor for the city")]
    public UnityGameObject Floor;

    void OnValidate()
    {
        TowerMaxHeight = math.max(TowerMaxHeight, TowerMinHeight);
    }

    public void DeclareReferencedPrefabs(List<UnityGameObject> referencedPrefabs)
    {
        referencedPrefabs.Add(BarPrefab);
    }

    const float k_FloorMultiplier = 4f;
    public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
    {
        var localScale = Floor.transform.localScale;
        dstManager.AddComponentData(entity, new CitySpawner
        {
            BarPrefab = conversionSystem.GetPrimaryEntity(BarPrefab),
            NumberOfClusters = NumberOfClusters,
            CityWidth = localScale.x * k_FloorMultiplier,
            CityLength = localScale.z * k_FloorMultiplier,
            MaxTowerHeight = TowerMaxHeight,
            MinTowerHeight = TowerMinHeight,
        });
    }
}
