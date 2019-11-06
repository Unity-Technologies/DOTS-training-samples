using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

public struct PrefabCollectionComponent : IComponentData
{
    public Entity HomebasePrefab;
    public Entity OverlayPrefab;
    public Entity OverlayColorPrefab;
    public Entity PlayerPrefab;
}

public class PrefabCollection : MonoBehaviour, IConvertGameObjectToEntity, IDeclareReferencedPrefabs
{
    public GameObject HomebasePrefab;
    public GameObject OverlayPrefab;
    public GameObject OverlayColorPrefab;
    public GameObject PlayerPrefab;

    public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
    {
        dstManager.AddComponentData(entity,
            new PrefabCollectionComponent {
                HomebasePrefab = conversionSystem.GetPrimaryEntity(HomebasePrefab),
                OverlayPrefab = conversionSystem.GetPrimaryEntity(OverlayPrefab),
                OverlayColorPrefab = conversionSystem.GetPrimaryEntity(OverlayColorPrefab),
                PlayerPrefab = conversionSystem.GetPrimaryEntity(PlayerPrefab)});
    }

    public void DeclareReferencedPrefabs(List<GameObject> referencedPrefabs)
    {
        referencedPrefabs.Add(HomebasePrefab);
        referencedPrefabs.Add(OverlayPrefab);
        referencedPrefabs.Add(OverlayColorPrefab);
        referencedPrefabs.Add(PlayerPrefab);
    }
}
