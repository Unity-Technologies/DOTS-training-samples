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

    public GeneratedIntersectionDataObject intersectionDataObject;
    
    unsafe void Start()
    {
        var entityManager = World.Active.EntityManager;
        var entity = entityManager.CreateEntity(typeof(IntersectionPoint), typeof(Spline));
        var prefab = GameObjectConversionUtility.ConvertGameObjectHierarchy(CarPrefab, World.Active);

        DynamicBuffer<IntersectionPoint> intersectionBuffer = entityManager.GetBuffer<IntersectionPoint>(entity);

        for (int i = 0; i < intersectionDataObject.intersections.Count; i++)
        {
            var intersectionData = intersectionDataObject.intersections[i];
            var intersection = new IntersectionPoint();
            intersection.Position = intersectionData.position;
            
            intersection.Neighbors[0] = intersectionData.splineData1;
            intersection.Neighbors[1] = intersectionData.splineData2;
            intersection.Neighbors[2] = intersectionData.splineData3;

            // Repeat the same neighbor, just to cover for the case of having fewer neighbor
            if (intersectionData.splineCount < 3)
                intersection.Neighbors[2] = intersection.Neighbors[1];
            if(intersectionData.splineCount < 2)
                intersection.Neighbors[1] = intersection.Neighbors[0];
            
            intersectionBuffer.Add(intersection);
        }
        /*
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
        */
        DynamicBuffer<Spline> splineBuffer = entityManager.GetBuffer<Spline>(entity);

        for (int i = 0; i < intersectionDataObject.splines.Count; i++)
        {
            var splineData = intersectionDataObject.splines[i];
            
            var spline = new Spline();
            spline.EndIntersectionId = splineData.endIntersectionId;
            spline.Anchor1 = splineData.anchor1;
            spline.Anchor2 = splineData.anchor2;
            spline.StartNormal = new float3(splineData.startNormal.x, splineData.startNormal.y, splineData.startNormal.z);
            spline.EndNormal = new float3(splineData.endNormal.x, splineData.endNormal.y, splineData.endNormal.z);
            spline.StartTargent = new float3(splineData.startTangent.x, splineData.startTangent.y, splineData.startTangent.z);
            spline.EndTangent = new float3(splineData.endTangent.x, splineData.endTangent.y, splineData.endTangent.z);
            
            splineBuffer.Add(spline);
        }
        
        /*
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
        */
        
        for (int i = 0; i < intersectionDataObject.intersections.Count; i++)
        {
            var intersectionData = intersectionDataObject.intersections[i];
            
            var car = entityManager.Instantiate(prefab);
            entityManager.AddComponent(car, typeof(FindTarget));
            entityManager.AddComponent(car, typeof(SplineData));
            entityManager.AddComponentData(car, new TargetIntersectionIndex{Value = intersectionData.id});
            entityManager.SetComponentData(car, new Translation{Value = intersectionData.position});
        }
        
        /*for (int i = 0; i < IntersectionPoints.Length; i++)
        {
            var car = entityManager.Instantiate(prefab);
            entityManager.AddComponent(car, typeof(FindTarget));
            entityManager.AddComponent(car, typeof(SplineData));
            entityManager.AddComponentData(car, new TargetIntersectionIndex{Value = i});
            entityManager.SetComponentData(car, new Translation{Value = IntersectionPoints[i].position});
        }*/
    }
}