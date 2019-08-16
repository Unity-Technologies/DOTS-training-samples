using System.Collections.Generic;
using System.Linq;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;
using Random = UnityEngine.Random;

// TODO: Move to subscene or conversion step.

public class IntersectionSpawner : MonoBehaviour
{
    public GameObject[] CarPrefabs;
    public GeneratedIntersectionDataObject intersectionDataObject;
    
    public int numCars = 50000;

    public UnityEngine.UI.Text carsText;
    
    void Start()
    {
        var entityManager = World.Active.EntityManager;
        var entity = entityManager.CreateEntity(typeof(IntersectionBufferElementData), typeof(SplineBufferElementData));
        var prefabs = new List<Entity>();
        foreach (var CarPrefab in CarPrefabs)
        {
            prefabs.Add(GameObjectConversionUtility.ConvertGameObjectHierarchy(CarPrefab, World.Active));
        }

        DynamicBuffer<IntersectionBufferElementData> intersectionBuffer = entityManager.GetBuffer<IntersectionBufferElementData>(entity);

        for (int i = 0; i < intersectionDataObject.intersections.Count; i++)
        {
            var intersectionData = intersectionDataObject.intersections[i];
            var intersection = new IntersectionBufferElementData();
            intersection.Position = intersectionData.position;
            intersection.Normal = intersectionData.normal;
            
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
        
        DynamicBuffer<SplineBufferElementData> splineBuffer = entityManager.GetBuffer<SplineBufferElementData>(entity);

        for (int i = 0; i < intersectionDataObject.splines.Count; i++)
        {
            var splineData = intersectionDataObject.splines[i];
            
            var spline = new SplineBufferElementData();
            
            spline.SplineId = i;
            spline.StartPosition = splineData.startPoint;
            spline.EndPosition = splineData.endPoint;
            spline.Anchor1 = splineData.anchor1;
            spline.Anchor2 = splineData.anchor2;
            spline.StartNormal = new float3(splineData.startNormal.x, splineData.startNormal.y, splineData.startNormal.z);
            spline.EndNormal = new float3(splineData.endNormal.x, splineData.endNormal.y, splineData.endNormal.z);
            spline.StartTangent = new float3(splineData.startTangent.x, splineData.startTangent.y, splineData.startTangent.z);
            spline.EndTangent = new float3(splineData.endTangent.x, splineData.endTangent.y, splineData.endTangent.z);
            
            spline.EndIntersectionId = splineData.endIntersectionId;
            if (i % 2 == 0)
                spline.OppositeDirectionSplineId = i + 1;
            else
                spline.OppositeDirectionSplineId = i - 1;
            
            splineBuffer.Add(spline);
        }

        for (int i = 0; i < numCars; i++)
        {
            var intersectionData = intersectionDataObject.intersections[Random.Range(0,intersectionDataObject.intersections.Count)];
            var carEntity = entityManager.Instantiate(prefabs[i % prefabs.Count]);

            var spline = intersectionDataObject.splines[intersectionData.splineData1];
            var startPoint = intersectionDataObject.splines[intersectionData.splineData1].startPoint;
            var endPoint = intersectionDataObject.splines[intersectionData.splineData1].endPoint;
            float randomPosition = Random.value;
            var randomPoint = startPoint + (endPoint - startPoint).normalized * randomPosition; 
            
            entityManager.AddComponent(carEntity, typeof(ReachedEndOfSplineComponent));
            entityManager.SetComponentData(carEntity, new Translation { Value = randomPoint });
            entityManager.AddComponentData(carEntity, new SplineComponent
                {
                    splineId = intersectionData.splineData1,
                    Spline = new SplineBufferElementData
                    {
                        StartPosition = spline.startPoint,
                        EndPosition = spline.endPoint,
                        Anchor1 = spline.anchor1,
                        Anchor2 = spline.anchor2,
                        StartNormal = new float3(spline.startNormal.x, spline.startNormal.y, spline.startNormal.z),
                        EndNormal = new float3(spline.endNormal.x, spline.endNormal.y, spline.endNormal.z),
                        StartTangent = new float3(spline.startTangent.x, spline.startTangent.y, spline.startTangent.z),
                        EndTangent = new float3(spline.endTangent.x, spline.endTangent.y, spline.endTangent.z),
                        EndIntersectionId = spline.endIntersectionId,
                        OppositeDirectionSplineId = spline.id % 2 == 0 ? spline.id + 1 : spline.id - 1,
                        SplineId = spline.id
                    }, 
                    IsInsideIntersection = false,
                    t = 0
                });
            
            /*entityManager.AddComponentData(carPrefab,
                new ExitIntersectionComponent()
                {
                    TargetSplineId = intersectionData.splineData1
                });*/
            //entityManager.AddComponent(carEntity, typeof(SplineComponent));
        }
        
        carsText.text = "Cars: " + numCars; 
        
        RoadGenerator.ready = true;
        RoadGenerator.useECS = true;
    }
}