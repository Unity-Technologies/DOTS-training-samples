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
        var entity = entityManager.CreateEntity(typeof(IntersectionBuffer));
        var prefab = GameObjectConversionUtility.ConvertGameObjectHierarchy(CarPrefab, World.Active);

        DynamicBuffer<IntersectionBuffer> buffer = entityManager.GetBuffer<IntersectionBuffer>(entity);
        
        for (int i = 0; i < IntersectionPoints.Length; i++)
        {
            var intersectionBuffer = new IntersectionBuffer();
            intersectionBuffer.position = IntersectionPoints[i].position;
            //placeholder
            var nextIndex = i + 1 == IntersectionPoints.Length ? 0 : i + 1;
            intersectionBuffer.neighbors[0] = nextIndex;
            intersectionBuffer.neighbors[1] = nextIndex;
            intersectionBuffer.neighbors[2] = nextIndex;
            
            buffer.Add(intersectionBuffer);
        }
        
        for (int i = 0; i < IntersectionPoints.Length; i++)
        {
            var car = entityManager.Instantiate(prefab);
            entityManager.AddComponent(car, typeof(FindTargetComponent));
            entityManager.AddComponent(car, typeof(MovementComponent));
            entityManager.AddComponentData(car, new CurrentIntersectionIndexComponent{id = i});
            entityManager.SetComponentData(car, new Translation{Value = IntersectionPoints[i].position});
        }
    }
}