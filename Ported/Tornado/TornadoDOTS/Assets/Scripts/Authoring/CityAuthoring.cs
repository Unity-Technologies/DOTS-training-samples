using System.Collections.Generic;
using Unity.Entities;
using UnityGameObject = UnityEngine.GameObject;
using UnityRangeAttribute = UnityEngine.RangeAttribute;
using UnityMonoBehaviour = UnityEngine.MonoBehaviour;

public class CityAuthoring : UnityMonoBehaviour, IConvertGameObjectToEntity, IDeclareReferencedPrefabs
{
    public UnityGameObject BarPrefab;
    [UnityRange(0, 100)] public int NumberOfTowers;
    
    public void DeclareReferencedPrefabs(List<UnityGameObject> referencedPrefabs)
    {
        referencedPrefabs.Add(BarPrefab);
    }

    public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
    {
        dstManager.AddComponentData(entity, new CitySpawner
        {
            BarPrefab = conversionSystem.GetPrimaryEntity(BarPrefab),
            NumberOfTowers = NumberOfTowers,
        });
    }
}
