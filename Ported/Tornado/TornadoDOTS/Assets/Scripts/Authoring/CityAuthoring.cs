using System.Collections.Generic;
using Unity.Entities;
using UnityGameObject = UnityEngine.GameObject;
using UnityRangeAttribute = UnityEngine.RangeAttribute;
using UnityMonoBehaviour = UnityEngine.MonoBehaviour;

public class CityAuthoring : UnityMonoBehaviour, IConvertGameObjectToEntity, IDeclareReferencedPrefabs
{
    public UnityGameObject BarPrefab;
    [UnityRange(1, 10000)] public int NumberOfTowers;
    [UnityRange(1, 40)] public int TowerMinHeight;
    [UnityRange(1, 40)] public int TowerMaxHeight;
    
    [UnityRange(20, 5000)] public int CityWidth;
    [UnityRange(20, 5000)] public int CityLength;
    
    public void DeclareReferencedPrefabs(List<UnityGameObject> referencedPrefabs)
    {
        referencedPrefabs.Add(BarPrefab);
    }

    public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
    {
        dstManager.AddComponentData(entity, new CitySpawner
        {
            BarPrefab = conversionSystem.GetPrimaryEntity(BarPrefab),
            NumberOfClusters = NumberOfTowers,
            CityWidth = CityWidth,
            CityLength = CityLength,
            MaxTowerHeight = TowerMaxHeight,
            MinTowerHeight = TowerMinHeight,
        });
    }
}
