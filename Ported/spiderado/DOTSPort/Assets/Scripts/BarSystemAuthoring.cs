using System.Collections.Generic;
using Unity.Entities;
using Unity.Transforms;
using UnityEngine;

[DisallowMultipleComponent]
[RequiresEntityConversion]
public class BarSystemAuthoring : MonoBehaviour, IConvertGameObjectToEntity, IDeclareReferencedPrefabs
{
    public GameObject barPrefab;

    public void DeclareReferencedPrefabs(List<GameObject> gameObjects) {
        gameObjects.Add(barPrefab);
    }

    public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
    {
        var barPrefab = conversionSystem.GetPrimaryEntity(this.barPrefab);

        BarPoint point1 = new BarPoint { index=1, anchor=0, neighborCount=1 };
        BarPoint point2 = new BarPoint { index=2, anchor=0, neighborCount=1 };
        BarPoint point3 = new BarPoint { index=3, anchor=0, neighborCount=1 };
        Entity pointEntity1 = conversionSystem.DstEntityManager.CreateEntity();
        Entity pointEntity2 = conversionSystem.DstEntityManager.CreateEntity();
        Entity pointEntity3 = conversionSystem.DstEntityManager.CreateEntity();

        conversionSystem.DstEntityManager.AddComponentData(pointEntity1, point1);
        conversionSystem.DstEntityManager.AddComponentData(pointEntity2, point2);
        conversionSystem.DstEntityManager.AddComponentData(pointEntity3, point3);

        Bar bar1 = new Bar { point1 = pointEntity1, point2 = pointEntity2 };
        Bar bar2 = new Bar { point1 = pointEntity2, point2 = pointEntity3 };
        Bar bar3 = new Bar { point1 = pointEntity3, point2 = pointEntity1 };
        Entity barEntity1 = dstManager.Instantiate(barPrefab);
        Entity barEntity2 = dstManager.Instantiate(barPrefab);
        Entity barEntity3 = dstManager.Instantiate(barPrefab);
        conversionSystem.DstEntityManager.AddComponentData(barEntity1, bar1);
        conversionSystem.DstEntityManager.AddComponentData(barEntity2, bar2);
        conversionSystem.DstEntityManager.AddComponentData(barEntity3, bar3);
    }
}
