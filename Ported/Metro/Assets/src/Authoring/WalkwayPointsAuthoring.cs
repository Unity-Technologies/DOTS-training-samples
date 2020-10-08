using System.Collections.Generic;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

public struct WalkwayPoints : IComponentData
{
    public Entity WalkwayFrontBottom;
    public Entity WalkwayFrontTop;
    public Entity WalkwayBackBottom;
    public Entity WalkwayBackTop;
}

public class WalkwayPointsAuthoring : UnityEngine.MonoBehaviour, IConvertGameObjectToEntity, IDeclareReferencedPrefabs
{
    public UnityEngine.GameObject WalkwayFrontBottom;
    public UnityEngine.GameObject WalkwayFrontTop;
    public UnityEngine.GameObject WalkwayBackBottom;
    public UnityEngine.GameObject WalkwayBackTop;

    public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
    {
        dstManager.AddComponentData(entity, new WalkwayPoints
        {
            WalkwayFrontBottom = conversionSystem.GetPrimaryEntity(WalkwayFrontBottom),
            WalkwayFrontTop = conversionSystem.GetPrimaryEntity(WalkwayFrontTop),
            WalkwayBackBottom = conversionSystem.GetPrimaryEntity(WalkwayBackBottom),
            WalkwayBackTop = conversionSystem.GetPrimaryEntity(WalkwayBackTop),
        });
    }

    public void DeclareReferencedPrefabs(List<GameObject> referencedPrefabs)
    {
        referencedPrefabs.Add(WalkwayFrontBottom);
        referencedPrefabs.Add(WalkwayFrontTop);
        referencedPrefabs.Add(WalkwayBackBottom);
        referencedPrefabs.Add(WalkwayBackTop);
    }
}
