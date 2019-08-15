using System.Collections.Generic;
using System.Linq;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;
using Random = UnityEngine.Random;

public class IntersectionSpawner : MonoBehaviour
{
    public Transform[] IntersectionPoints;
    public GameObject[] CarPrefabs;

    public GeneratedIntersectionDataObject intersectionDataObject;
    
    unsafe void Start()
    {
        var entityManager = World.Active.EntityManager;
        var entity = entityManager.CreateEntity(typeof(IntersectionPoint), typeof(Spline));
        var prefabs = new List<Entity>();
        foreach (var CarPrefab in CarPrefabs)
        {
            prefabs.Add(GameObjectConversionUtility.ConvertGameObjectHierarchy(CarPrefab, World.Active));
        }

        DynamicBuffer<IntersectionPoint> intersectionBuffer = entityManager.GetBuffer<IntersectionPoint>(entity);

        for (int i = 0; i < intersectionDataObject.intersections.Count; i++)
        {
            var intersectionData = intersectionDataObject.intersections[i];
            var intersection = new IntersectionPoint();
            intersection.Position = intersectionData.position;
            
            intersection.SplineId0= intersectionData.splineData1;
            intersection.SplineId1 = intersectionData.splineData2;
            intersection.SplineId2 = intersectionData.splineData3;
            intersection.SplineIdCount = intersectionData.splineCount;

            // Repeat the same neighbor at the end of the array, to cover for the case of having less than 3 neighbors
            if(intersectionData.splineCount == 1)
            {
                intersection.SplineId1 = -1;
                intersection.SplineId2 = -1;
            }
            if(intersectionData.splineCount == 2){
                intersection.SplineId2 = -1;
            }
            
            intersectionBuffer.Add(intersection);
        }
        
        DynamicBuffer<Spline> splineBuffer = entityManager.GetBuffer<Spline>(entity);

        for (int i = 0; i < intersectionDataObject.splines.Count; i++)
        {
            var splineData = intersectionDataObject.splines[i];
            
            var spline = new Spline();
            spline.StartPosition = splineData.startPoint;
            spline.EndPosition = splineData.endPoint;
            spline.Anchor1 = splineData.anchor1;
            spline.Anchor2 = splineData.anchor2;
            spline.StartNormal = new float3(splineData.startNormal.x, splineData.startNormal.y, splineData.startNormal.z);
            spline.EndNormal = new float3(splineData.endNormal.x, splineData.endNormal.y, splineData.endNormal.z);
            spline.StartTangent = new float3(splineData.startTangent.x, splineData.startTangent.y, splineData.startTangent.z);
            spline.EndTangent = new float3(splineData.endTangent.x, splineData.endTangent.y, splineData.endTangent.z);
            
            spline.EndIntersectionId = splineData.endIntersectionId;
            
            splineBuffer.Add(spline);
        }

        for (int i = 0; i < intersectionDataObject.intersections.Count; i++)
        {
            var intersectionData = intersectionDataObject.intersections[i];

            var car = entityManager.Instantiate(prefabs[i%prefabs.Count]);
            entityManager.AddComponent(car, typeof(ReachedEndOfSpline));
            entityManager.SetComponentData(car, new Translation{Value = intersectionData.position});
            entityManager.AddComponentData(car, 
                new ExitIntersectionData()
                {
                    IsIntersection = true,
                    TargetSplineId = intersectionData.splineData1
                });
            entityManager.AddComponent(car, typeof(SplineData));
        }
    }
}