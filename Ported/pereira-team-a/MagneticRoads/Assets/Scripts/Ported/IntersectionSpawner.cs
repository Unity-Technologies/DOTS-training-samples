using System.Collections.Generic;
using System.Linq;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

public class IntersectionSpawner : MonoBehaviour
{
    public Transform[] IntersectionPoints;
    public GameObject CarPrefab;

    unsafe void Start()
    {
        var entityManager = World.Active.EntityManager;
        var entity = entityManager.CreateEntity(typeof(IntersectionPoint), typeof(Spline));
        var prefab = GameObjectConversionUtility.ConvertGameObjectHierarchy(CarPrefab, World.Active);

        DynamicBuffer<IntersectionPoint> intersectionBuffer = entityManager.GetBuffer<IntersectionPoint>(entity);
        
        for (int i = 0; i < IntersectionPoints.Length; i++)
        {
            var intersection= new IntersectionPoint();
            intersection.Position = IntersectionPoints[i].position;
            //placeholder
            intersection.Neighbors[0] = i;
            intersection.Neighbors[1] = i;
            intersection.Neighbors[2] = i;
            
            intersectionBuffer.Add(intersection);
        }
        
        DynamicBuffer<Spline> splineBuffer = entityManager.GetBuffer<Spline>(entity);

        for (int i = 0; i < 4; i++)
        {
            var nextIndex = i + 1 == 4 ? 0 : i + 1;
            
            var spline = new Spline();
            spline.EndIntersectionId = nextIndex;
            spline.Anchor1 = float3.zero;
            spline.Anchor2 = float3.zero;
            spline.StartNormal = float3.zero;
            spline.EndNormal = float3.zero;
            spline.StartTargent = float3.zero;
            spline.EndTangent = float3.zero;
            
            splineBuffer.Add(spline);
        }
        
        for (int i = 0; i < IntersectionPoints.Length; i++)
        {
            var car = entityManager.Instantiate(prefab);
            entityManager.AddComponent(car, typeof(FindTarget));
            entityManager.AddComponent(car, typeof(SplineData));
            entityManager.AddComponentData(car, new TargetIntersectionIndex{Value = i});
            entityManager.SetComponentData(car, new Translation{Value = IntersectionPoints[i].position});
        }
    }
}