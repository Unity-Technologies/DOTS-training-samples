using System.Collections.Generic;
using System.Linq;
using Unity.Entities;
using Unity.Transforms;
using UnityEngine;

public class IntersectionSpawner : MonoBehaviour
{
    public Transform[] IntersectionPoints;
    public GameObject CarPrefab;

    unsafe void Start()
    {
        var entityManager = World.Active.EntityManager;
        var entity = entityManager.CreateEntity(typeof(IntersectionPoint));
        var prefab = GameObjectConversionUtility.ConvertGameObjectHierarchy(CarPrefab, World.Active);

        DynamicBuffer<IntersectionPoint> buffer = entityManager.GetBuffer<IntersectionPoint>(entity);
        
        for (int i = 0; i < IntersectionPoints.Length; i++)
        {
            var intersectionBuffer = new IntersectionPoint();
            intersectionBuffer.Position = IntersectionPoints[i].position;
            //placeholder
            var nextIndex = i + 1 == IntersectionPoints.Length ? 0 : i + 1;
            intersectionBuffer.Neighbors[0] = nextIndex;
            intersectionBuffer.Neighbors[1] = nextIndex;
            intersectionBuffer.Neighbors[2] = nextIndex;
            
            buffer.Add(intersectionBuffer);
        }
        
        for (int i = 0; i < IntersectionPoints.Length; i++)
        {
            var car = entityManager.Instantiate(prefab);
            entityManager.AddComponent(car, typeof(FindTarget));
            entityManager.AddComponent(car, typeof(TargetPosition));
            entityManager.AddComponentData(car, new TargetIntersectionIndex{Value = i});
            entityManager.SetComponentData(car, new Translation{Value = IntersectionPoints[i].position});
        }
    }
}